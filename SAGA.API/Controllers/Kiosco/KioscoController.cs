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
                    username = datos.Telefono[0].ClaveLada.ToString() + datos.Telefono[0].telefono.ToString();
                }
                usuario.Id = Guid.NewGuid().ToString();
                usuario.PhoneNumber = datos.Telefono == null ? "00000000" : datos.Telefono[0].telefono.ToString();
                usuario.Clave = "00000";
                usuario.Pasword = pass;
                usuario.RegistroClave = DateTime.Now;
                usuario.PhoneNumberConfirmed = false;
                usuario.EmailConfirmed = true;
                usuario.LockoutEnabled = false;
                usuario.AccessFailedCount = 0;
                usuario.Email = datos.Email == null ? "SIN REGISTRO" : datos.Email[0].email.ToString();
                usuario.UserName = username;
                usuario.Activo = 0;

                db.AspNetUsers.Add(usuario);
                db.SaveChanges();

                var add = db.Database.ExecuteSqlCommand("spEncriptarPasword @id", new SqlParameter("id", usuario.Id));

                var candidato = new Candidato();

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

                var t = db.AspNetUsers.Find(usuario.Id);
                db.Entry(t).Property(x => x.IdPersona).IsModified = true;

                t.IdPersona = candidato.Id;
                db.SaveChanges();


                login.Id = candidato.Id;
                login.username = username;
                login.pass = pass;
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
                    this.SendEmailRegistroBT(datos, pass);
                }
                else
                {
                    var tel = datos.Telefono[0].ClaveLada.ToString() + datos.Telefono[0].telefono.ToString();
                    var mocos = this.EnviarSMS(tel, username, pass);
                }

                return Ok(new { login, generales });
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(login.username))
                {
                    return Ok(HttpStatusCode.GatewayTimeout);
                }
                else
                {
                    return Ok(HttpStatusCode.ExpectationFailed);
                }
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
            List<string> Destino = new List<string>(1) { ConfigurationManager.AppSettings["Lada"] + telefono };
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
    }
}
