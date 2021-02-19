using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using SAGA.API.Utilerias;
using System.IO;
using SAGA.API.Dtos.SistTickets;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Configuration;
using System.Text.RegularExpressions;
using Infobip.Api.Config;
using Infobip.Api.Client;
using System.Text;
using Infobip.Api.Model.Sms.Mt.Send.Textual;
using Infobip.Api.Model.Sms.Mt.Send;
using System.Threading.Tasks;
using System.Web;
using SAGA.API.Controllers.BTDamsa;
using RestSharp;
using System.Threading;

namespace SAGA.API.Controllers.Kiosko
{
    [RoutePrefix("api/Kiosco")]
    public class KioscoController : ApiController
    {
        private SAGADBContext db;

        public KioscoController()
        {
            db = new SAGADBContext();
        }
        [HttpPost]
        [Route("uploadCV")]
        public IHttpActionResult UploadCV()
        {
            string fileName = null;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var postedFile = httpRequest.Files["file"];

                fileName = Path.GetFileName(postedFile.FileName);
                var idx = fileName.LastIndexOf('_') + 1;
                var lon = fileName.Length - idx;
                var id = fileName.Substring(idx, lon);

                string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/Files/users/");
                if (!Directory.Exists(fullPath + '/' + id))
                {
                    Directory.CreateDirectory(fullPath + '/' + id);
                }

                fileName = fileName.Substring(0, idx - 1);

                fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/Files/users/" + id + '/' + fileName + ".pdf");

                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                postedFile.SaveAs(fullPath);

                var ccv = db.MiCVUpload.Where(x => x.CandidatoId.Equals(new Guid(id))).Count();
                if(ccv == 0)
                {
                    MiCVUpload cv = new MiCVUpload();
                    cv.CandidatoId = new Guid(id);
                    cv.UrlCV = "Files/users/" + id + '/' + fileName + ".pdf";

                    db.MiCVUpload.Add(cv);
                    db.SaveChanges();
                }
                return Ok(HttpStatusCode.OK); 

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.InternalServerError);
            }

        }
        [HttpGet]
        [Route("getEstado")]
        public IHttpActionResult GetEstado()
        {
            var estado = db.Estados
                .OrderBy(x => x.estado)
                .Where(x => x.PaisId.Equals(42))
                .Select(x => new
                {
                    x.Id,
                    x.estado,
                    x.Clave
                })
                .ToList();
            return Ok(estado);
        }
        [HttpPost]
        [Route("registrarCandidatoBT")]
        public IHttpActionResult RegistrarCandidatoBT(CandidatosGralDto datos)
        {
            LoginDto login = new LoginDto();

            try
            {
                AspNetUsers usuario = new AspNetUsers();
                var username = "";
                var pass = datos.Nombre.Substring(0, 1).ToLower() + datos.ApellidoPaterno.Trim().ToLower();
                if (datos.OpcionRegistro == 1)
                {
                    username = datos.Email[0].email.ToString();
                }
                else
                {
                    username = datos.Telefono[0].telefono.ToString();
                }
                usuario.Id = Guid.NewGuid().ToString();
                usuario.PhoneNumber = datos.Telefono == null ? "00000000" : datos.Telefono[0].telefono.ToString();
                usuario.Clave = "00000";
                usuario.Pasword = pass;
                usuario.RegistroClave = DateTime.Now;
                usuario.PhoneNumberConfirmed = false;
                usuario.EmailConfirmed = false;
                usuario.LockoutEnabled = false;
                usuario.AccessFailedCount = 0;
                usuario.Email = datos.Email == null ? "SIN REGISTRO" : datos.Email[0].email.ToString();
                usuario.UserName = username;
                usuario.Activo = 1;

                db.AspNetUsers.Add(usuario);
                db.SaveChanges();

                var add = db.Database.ExecuteSqlCommand("spEncriptarPasword @id", new SqlParameter("id", usuario.Id));

                if (datos.OpcionRegistro == 1)
                {
                    var code = this.ValidarCorreo(datos.Email[0].email.ToString());
                   return Ok(new { code, usuario.Id });
                }
                else
                {
                    var code = this.EnviarSMS(datos.Telefono[0].telefono.ToString());
                    return Ok(new { code, usuario.Id });
                }
            

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
                //if (string.IsNullOrEmpty(login.username))
                //{
                //    return Ok(HttpStatusCode.GatewayTimeout);
                //}
                //else
                //{
                //    return Ok(HttpStatusCode.ExpectationFailed);
                //}
            }
        }

        [HttpPost]
        [Route("activarCandidatoBT")]
        public IHttpActionResult ActivarCandidatoBT(CandidatosGralDto datos)
        {
            try
            {
                var candidato = new Candidato();
                LoginDto login = new LoginDto();
                candidato.CURP = datos.Curp;
                candidato.Nombre = datos.Nombre;
                candidato.ApellidoPaterno = datos.ApellidoPaterno;
                candidato.ApellidoMaterno = datos.ApellidoMaterno;
                candidato.emails = datos.Email;
                candidato.PaisNacimientoId = 42;
                candidato.EstadoNacimientoId = datos.EstadoNacimientoId;
                candidato.MunicipioNacimientoId = datos.MunicipioNacimientoId;
                candidato.telefonos = datos.Telefono;
                candidato.GeneroId = datos.GeneroId;
                candidato.TipoEntidadId = 2;
                candidato.FechaNacimiento = datos.FechaNac;

                db.Candidatos.Add(candidato);

                db.SaveChanges();

                PerfilCandidato PC = new PerfilCandidato();

                PC.CandidatoId = candidato.Id;
                PC.Estatus = 41;

                db.PerfilCandidato.Add(PC);

                db.SaveChanges();

                var t = db.AspNetUsers.Find(datos.Id.ToString());
                db.Entry(t).Property(x => x.IdPersona).IsModified = true;
                db.Entry(t).Property(x => x.PhoneNumberConfirmed).IsModified = true;
                db.Entry(t).Property(x => x.EmailConfirmed).IsModified = true;
                db.Entry(t).Property(x => x.Activo).IsModified = true;
                db.Entry(t).Property(x => x.Clave).IsModified = true;

                t.IdPersona = candidato.Id;
                t.Clave = datos.Code;
                t.Activo = 0;
                if (datos.OpcionRegistro == 1)
                {
                    t.EmailConfirmed = true;
                    t.PhoneNumberConfirmed = false;
                }
                else
                {
                    t.EmailConfirmed = false;
                    t.PhoneNumberConfirmed = true;
                }
                db.SaveChanges();
                var pd = db.Database.SqlQuery<String>("dbo.spDesencriptarPasword @id", new SqlParameter("id", datos.Id)).FirstOrDefault();

                login.Id = candidato.Id;
                login.username = t.UserName;
                login.pass = pd;

                var generales = db.Candidatos.Where(x => x.Id.Equals(candidato.Id)).Select(d => new
                {
                    nombre = d.Nombre + " " + d.ApellidoPaterno + " " + d.ApellidoMaterno,
                    fechaNac = d.FechaNacimiento,
                    genero = d.Genero.genero,
                    estado = d.estadoNacimiento.estado,
                    telefono = d.telefonos.Select(tel => tel.telefono).FirstOrDefault(),
                    email = d.emails.Select(e => e.email).FirstOrDefault()
                }).FirstOrDefault();


                if (datos.OpcionRegistro == 1)
                {
                    this.SendEmailRegistroBT(datos, pd);
                }
                else
                {
                    var tel = datos.Telefono[0].telefono.ToString();
                    var mocos = this.EnviarSMS(tel, t.UserName, pd);
                }

                return Ok(new { login, generales });
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }

        public void SendEmailRegistroBT(CandidatosGralDto dtos, string pass)
        {
            try
            {
                string body = "";
                string email = dtos.Email[0].email;

                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "Bolsa de Trabajo DAMSA");
                m.Subject = "Datos de Registro a Bolsa de Trabajo DAMSA";
                m.To.Add(email);


                m.Subject = "Tu acceso a Bolsa de Trabajo DAMSA está listo!";
                body = "<html><body><table width=\"80%\" style=\"font-family:'calibri'\">";
                body = body + "<tr><th bgcolor=\"#044464\" style=\"color:white; text-align:left;\">Se creó una nueva cuenta para BOLSA TRABAJO DAMSA </th></ tr>";
                body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Usuario:</font></td></tr>";
                body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td>{0}</td></tr>", email);
                body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Nombre :</font></td></tr>";
                body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td> {0} {1} {2} </td></tr>", dtos.Nombre, dtos.ApellidoPaterno, dtos.ApellidoMaterno);
                body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Correo :</font></td></tr>";
                body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td> {0} </td></tr>", email);
                body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Contraseña :</font></td></tr>";
                body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td> {0} </td></tr>", pass);
                body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Registrado :</font></td></tr>";
                body = body + string.Format("<tr bgcolor=\"#FDC613\"><td>{0}<br/>", DateTime.Now.ToShortDateString());
                body = body + "<p> Podrás acceder mediante la siguiente dirección: https://www.damsa.com.mx/bt <br/>";
                body = body + "Quedamos a tus órdenes para cualquier relativo al correo inntec@damsa.com.mx </p></td></tr></table>";
                body = body + "</body></html>";

                m.Body = body;
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                smtp.Send(m);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public async Task<IHttpActionResult> EnviarSMS(string telefono, string usuario, string pass)
        {
            Regex reg = new Regex("[^a-zA-Z0-9] ");
            List<string> Destino = new List<string>(1) { telefono };
            BasicAuthConfiguration BASIC_AUTH_CONFIGURATION = new BasicAuthConfiguration(ConfigurationManager.AppSettings["BaseUrl"], ConfigurationManager.AppSettings["UserInfobip"], ConfigurationManager.AppSettings["PassInfobip"]);

            SendSingleTextualSms smsClient = new SendSingleTextualSms(BASIC_AUTH_CONFIGURATION);


            string texto = "Datos de acceso a Bolsa Trabajo DAMSA: Usuario: " + usuario + " Contraseña: " + pass + " Da click https://www.damsa.com.mx/bt para cambiar contraseña y actualizar tu perfil";
            texto = texto.Normalize(NormalizationForm.FormD);
            texto = reg.Replace(texto, " ");

            SMSTextualRequest request = new SMSTextualRequest
            {
                From = "DAMSA",
                To = Destino,
                Text = ConfigurationManager.AppSettings["NameAppMsj"] + texto

            };

            SMSResponse smsResponse = await smsClient.ExecuteAsync(request); // Manda el mensaje con código.

            SMSResponseDetails sentMessageInfo = smsResponse.Messages[0];


            return Ok(HttpStatusCode.Created);
        }
        [HttpPost]
        [Route("loginBT")]
        public IHttpActionResult LoginBolsaTrabajo(LoginDto datos)
        {
            try
            {
                var p = db.AspNetUsers.Where(x => x.UserName.Equals(datos.username)).Select(U => new
                {
                    Id = U.IdPersona,
                    userId = U.Id,
                    pass = U.Pasword,
                    username = U.UserName,
                    generales = db.Candidatos.Where(x => x.Id.Equals(U.IdPersona)).Select(d => new
                    {
                        nombre = d.Nombre + " " + d.ApellidoPaterno + " " + d.ApellidoMaterno,
                        fechaNac = d.FechaNacimiento,
                        genero = d.Genero.genero,
                        estado = d.estadoNacimiento.estado,
                        telefono = d.telefonos.Select(t => t.telefono).FirstOrDefault(),
                        email = d.emails.Select(e => e.email).FirstOrDefault()
                    }).FirstOrDefault(),
                    nombre = db.Entidad.Where(x => x.Id.Equals(U.IdPersona)).Select(u => u.Nombre + " " + u.ApellidoPaterno + " " + u.ApellidoMaterno).FirstOrDefault()
                }).FirstOrDefault();

                if (p != null)
                {
                    var pd = db.Database.SqlQuery<String>("dbo.spDesencriptarPasword @id", new SqlParameter("id", p.userId)).FirstOrDefault();
                    if (pd.Equals(datos.pass))
                    {

                        return Ok(p);
                    }
                    else
                    {
                        return Ok(HttpStatusCode.Ambiguous); //300 diferentes contraseñas
                    }
                }
                else
                {
                    return Ok(HttpStatusCode.NotFound); //404
                }
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        [Route("postulacion")]
        public IHttpActionResult Postulacion(ProcesoDto datos)
        {
            try
            {
                Postulacion obj = new Postulacion();
                obj.CandidatoId = datos.candidatoId;
                obj.fch_Postulacion = DateTime.Now;
                obj.RequisicionId = datos.requisicionId;
                obj.StatusId = 1;

                db.Postulaciones.Add(obj);
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
            
        }

        [HttpGet]
        [Route("getVacantes")]
        public IHttpActionResult GetVacantes()
        {
            try
            {
                List<int> estatus = new List<int> { 8, 9, 34, 35, 36, 37, 47, 48 };

                //Guid mocos = new Guid("1FF62A23-3664-E811-80E1-9E274155325E");
                //var usuarios = db.Usuarios.Where(x => x.SucursalId.Equals(mocos)).Select(U => U.Id).ToList();
                //var requis = db.AsignacionRequis.Where(x => usuarios.Contains(x.GrpUsrId)).Select(R => R.RequisicionId).ToArray();

                var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                    .Where(e => e.Activo && !e.Confidencial && !estatus.Contains(e.EstatusId))
                    .Select(e => new
                    {
                        Id = e.Id,
                        estatus = e.Estatus.Descripcion,
                        Folio = e.Folio,
                        fch_Creacion = e.fch_Creacion,
                        //Cliente = e.Cliente.Nombrecomercial,
                        //ClienteId = e.Cliente.Id,
                        estado = e.Cliente.direcciones.Select(x => x.Municipio.municipio + " " + x.Estado.estado + " " + x.Estado.Pais.pais).FirstOrDefault(),
                        //domicilio_trabajo = e.Direccion.Calle + " " + e.Direccion.NumeroExterior + " " + e.Direccion.Colonia.colonia + " " + e.Direccion.Municipio.municipio + " " + e.Direccion.Estado.estado,
                        //Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        VBtra = e.VBtra,
                        requisitos = e.DAMFO290.escolardadesPerfil.Select(esc => esc.Escolaridad.gradoEstudio).Take(3),
                        Actividades = e.DAMFO290.actividadesPerfil.Select(a => a.Actividades).Take(3),
                        aptitudes = e.DAMFO290.aptitudesPerfil.Select(ap => ap.Aptitud.aptitud).Take(3),
                        experiencia = e.Experiencia,
                        categoria = e.Area.areaExperiencia,
                        categoriaId = e.Area.Id,
                        icono = e.Area.Icono,
                        areaId = e.AreaId,
                        cubierta = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) - db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count() : 0,
                        // arte = ValidarArte(e.Id.ToString()) // @"https://apisb.damsa.com.mx/Utilerias/" + "img/ArteRequi/Arte/" + e.Id + ".png"
                    }).ToList();


                var v = vacantes.Where(x => x.cubierta > 0).Select(e => new
                {
                    Id = e.Id,
                    estatus = e.estatus,
                    Folio = e.Folio,
                    VBtra = e.VBtra,
                    experiencia = e.experiencia,
                    categoria = e.categoria,
                    icono = e.icono,
                    areaId = e.areaId,
                    cubierta = e.cubierta,
                    arte = this.ValidarArte(e.Id.ToString()),
                    estado = e.estado,
                    requisitos = e.requisitos,
                    actividades = e.Actividades,
                    aptitudes = e.aptitudes

                }).ToList();


                return Ok(v);

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        [HttpGet]
        [Route("getDetalleVacante")]
        public IHttpActionResult GetDetalleVacante(Guid id)
        {
            try
            {
                var vacantes = db.Requisiciones
                 .OrderBy(r => r.fch_Cumplimiento)
                 .Where(x => x.Id.Equals(id))
                .Select(x => new
                {
                    Id = x.Id,
                    Folio = x.Folio,
                    VBtra = x.VBtra,
                    AreaId = x.AreaId,
                    Area = x.Area,
                    ClaseReclutamientoId = x.ClaseReclutamientoId,
                    ClaseReclutamiento = x.ClaseReclutamiento,
                    ClienteId = x.ClienteId,
                    Confidencial = x.Confidencial,
                    DireccionId = x.DireccionId,
                    Direccion = x.Direccion,
                    Experiencia = x.Experiencia,
                    fch_Aprobacion = x.fch_Aprobacion,
                    fch_Creacion = x.fch_Creacion,
                    fch_Cumplimiento = x.fch_Cumplimiento,
                    fch_Limite = x.fch_Limite,
                    fch_Modificacion = x.fch_Modificacion,
                    SueldoMaximo = x.SueldoMaximo,
                    SueldoMinimo = x.SueldoMinimo,
                    horariosRequi = x.horariosRequi.Where(xx => xx.Activo).Select(h => new {
                        h.Nombre,
                        h.aDia,
                        h.aDiaId,
                        h.deDia,
                        h.deDiaId,
                        h.deHora, 
                        h.aHora,
                        h.numeroVacantes,
                        h.Especificaciones
                    }).ToList(),
                    ConfiguracionRequi = db.ConfiguracionRequis.Where(y => y.RequisicionId.Equals(x.Id)).Select(c => new {
                        c.Campo,
                        c.Detalle,
                        c.IdEstructura,
                        c.Resumen,
                        c.R_D,
                        c.RequisicionId
                    }).OrderBy(o => o.IdEstructura).ToList(),
                    aptitudesRequi = x.aptitudesRequi.Select(a => a.Aptitud.aptitud).ToList(),
                    escolaridadesRequi = x.escolaridadesRequi.Select(e => new
                    {
                        e.Escolaridad.gradoEstudio,
                        e.EstadoEstudio.estadoEstudio
                    }).ToList(),
                    beneficiosRequi = db.BeneficiosRequis.Where(b => b.RequisicionId.Equals(x.Id)).Select(b => new
                    {
                        b.TipoBeneficio.tipoBeneficio,
                        b.Observaciones,
                        b.Cantidad
                    }).ToList(),
                    actividadesRequi = x.actividadesRequi.Select(a => a.Actividades).ToList(),
                    documentosClienteRequi = x.documentosClienteRequi.Select(d => d.Documento).ToList(),
                    prestacionesClienteRequi = x.prestacionesClienteRequi.Select(p => p.Prestamo).ToList(),
                    TiempoContratoId = x.TiempoContratoId,
                    TiempoContrato = x.TiempoContrato,
                    EstatusId = x.EstatusId,
                    DocumentosDamsa = db.DocumentosDamsa.ToList(),
                    observacionesRequi = db.ObservacionesRequis.Where(r => r.RequisicionId.Equals(x.Id)).Select(g => g.Observaciones).ToList(),
                    Publicado = x.Publicado
                }).FirstOrDefault();


                return Ok(vacantes);
            }
            catch (Exception ex) {
                return Ok(HttpStatusCode.BadRequest);
            }

        }
        public string ValidarArte(string requisicionId)
        {
            DirectoryInfo folderInfo = new DirectoryInfo(System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/ArteRequi/Arte"));
            List<string> extensions = folderInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly).Select(x => x.FullName).ToList();


            var files = folderInfo.GetFiles(
                    "*.*",
                    SearchOption.AllDirectories).Select(x => new
                    {
                        fullPath = x.FullName,
                        nom = x.Name,
                        sinext = x.Name.Remove(x.Name.IndexOf('.')),
                        ext = x.Extension,
                        size = (long)x.Length / 1024,
                        fc = x.LastWriteTime.ToShortDateString()
                    }).OrderByDescending(o => o.fc);

            string arte = files.Where(x => x.sinext.Equals(requisicionId)).Select(a => a.nom).FirstOrDefault();

            if (arte != null)
            {
                arte = "img/ArteRequi/Arte/" + requisicionId + ".png";
            }
          
       
            return arte;
        }

        public string ValidarCorreo(string correo)
        {
            try
            {
                // Generar código
                Random rnd = new Random();
                string code = rnd.Next(100000, 999999).ToString();

                var xml = "";
                var asunto = string.Format("Bolsa de Trabajo Damsa - Registro");
                var copia = "mventura@damsa.com.mx";
                //Cuerpo del Mensaje
                var body = "&lt;!DOCTYPE html>";
                body = body + "&lt;html lang=&quot;es&quot;>";
                body = body + "&lt;meta http-equiv=&quot;Content - Type&quot; content=&quot;text / html; charset = utf - 8&quot;/>";
                body = body + "&lt;link href=&quot;{ { URL::Content('css/bootstrap.min.css') } }&quot; rel=&quot;stylesheet&quot; type=&quot;text / css&quot; media=&quot;all&quot; />";
                body = body + "&lt;body> &lt;div style=&quot;text-align: center; font-family:'calibri';&quot;>";
                body = body + "&lt;p>Se ha recibido una petición para activar registro en bolsa de trabajo DAMSA.&lt;/p>";
                body = body + "&lt;p>Ingresa el siguiente código para activar registro: &lt;strong>" + code + "&lt;/strong>&lt;/p>";
                body = body + "&lt;br> Esta notificación es por seguridad y sirve para que nadie use tu cuenta de correo electrónico sin autorización. &lt;br>&lt;/ p > ";
                body = body + "&lt;p>&lt;small>Por favor no respondas a este correo.&lt;br>";
                body = body + "Gracias.&lt;br>El equipo &lt;strong> INNTEC.&lt;/strong>&lt;/small>&lt;/p>&lt;hr>";
                body = body + "&lt;/div>&lt;/body>&lt;/html>";


                xml = string.Format("<Parametros><Parametro Id_Sistema=\"SISTEMA_DEMO\" De=\"noreply@damsa.com.mx\" "
                                   + "Para=\"{0}\" Copia=\"{1}\"  CopiaOculta=\"\" Asunto=\"{2}\" Msg=\"{3}\"/> "
                                   + "</Parametros>", correo, copia, asunto, body);

                SqlParameter[] Parameters = { new SqlParameter("@ParametrosXML", xml) };
                db.Database.ExecuteSqlCommand("sp_emailFirmas @ParametrosXML", Parameters);
                return code;
            }
            catch (Exception ex)
            {
                return "400";
            }
        }
 
        public string EnviarSMS(string telefono)
        {
            try
            {
               

                Random rnd = new Random();
                string code = rnd.Next(100000, 999999).ToString();
                Regex reg = new Regex("[^a-zA-Z0-9] ");

                var client = new RestClient("https://api.infobip.com/sms/1/text/single");
                var request = new RestRequest(Method.POST);
                request.AddHeader("accept", "application/json");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "Basic " + ConfigurationManager.AppSettings["InfobipToken"]);
                request.AddParameter("application/json", "{\n  \"from\": \"Damsa\",\n  \"to\": \"" +
               telefono + "\",\n  \"text\": \"" + ConfigurationManager.AppSettings["NameAppMsj"] + "te envia tu código de verificacion: " + code + "\"\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);




                //List<string> Destino = new List<string>(1) { telefono };
                //BasicAuthConfiguration BASIC_AUTH_CONFIGURATION = new BasicAuthConfiguration(ConfigurationManager.AppSettings["BaseUrl"], ConfigurationManager.AppSettings["UserInfobip"], ConfigurationManager.AppSettings["PassInfobip"]);

                //SendSingleTextualSms smsClient = new SendSingleTextualSms(BASIC_AUTH_CONFIGURATION);

                //string texto = "Bolsa Trabajo DAMSA codigo para validacion de registro " + code;
                //texto = texto.Normalize(NormalizationForm.FormD);
                //texto = reg.Replace(texto, " ");

                //SMSTextualRequest request = new SMSTextualRequest
                //{
                //    From = "DAMSA",
                //    To = Destino,
                //    Text = ConfigurationManager.AppSettings["NameAppMsj"] + texto

                //};
                //try
                //{


                //    SMSResponse smsResponse = await smsClient.ExecuteAsync(request); // Manda el mensaje con código.
                //    SMSResponseDetails sentMessageInfo = smsResponse.Messages[0];
                //}
                //catch (TaskCanceledException)
                //{
                //    Console.WriteLine("\nTasks cancelled: timed out.\n");
                //}


                return code;
            }
            catch (Exception ex)
            {
                return "400";
            }

        }
   
    }
}
