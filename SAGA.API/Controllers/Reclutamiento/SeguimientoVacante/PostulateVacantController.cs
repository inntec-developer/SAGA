using SAGA.API.Dtos.Reclutamiento.Seguimientovacantes;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Mail;
using System.Configuration;
using Infobip.Api.Model.Sms.Mt.Send;
using Infobip.Api.Model.Sms.Mt.Send.Textual;
using Infobip.Api.Client;
using Infobip.Api.Config;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/reclutamiento/SeguimientoVacante")]
    public class PostulateVacantController : ApiController
    {
        public SAGADBContext db;

        public PostulateVacantController()
        {
            db = new SAGADBContext();
        }
            
        [HttpGet]
        [Route("getPostulate")]
        public IHttpActionResult GetPostulate(Guid VacanteId)
        {
           // var postulate = db.Postulaciones.Where(x => x.RequisicionId.Equals(VacanteId) && x.StatusId.Equals(1)).Select(x => x.CandidatoId).ToList();
            var postulate = db.Postulaciones.Where(p => p.RequisicionId.Equals(VacanteId) && p.StatusId.Equals(1)).Select(c => c.CandidatoId).Except(db.ProcesoCandidatos.Where(xx => xx.RequisicionId.Equals(VacanteId) && xx.EstatusId.Equals(27)).Select(cc => cc.CandidatoId)).ToList();

            var candidatos = db.PerfilCandidato.Where(x => postulate.Contains(x.CandidatoId)).Select(x => new {
                CandidatoId = x.CandidatoId,
                nombre = x.Candidato.Nombre + " " + x.Candidato.ApellidoPaterno + " " + x.Candidato.ApellidoMaterno,
                AreaExp = x.AboutMe.Select(ae => ae.AreaExperiencia.areaExperiencia).FirstOrDefault() != null ? x.AboutMe.Select(ae => ae.AreaExperiencia.areaExperiencia).FirstOrDefault() : "" ,
                AreaInt = x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() != null ? x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() : "" ,
                localidad = x.Candidato.direcciones.Select(d => d.Municipio.municipio).FirstOrDefault() + " / " + x.Candidato.direcciones.Select(d => d.Estado.estado).FirstOrDefault(),
                sueldoMinimo = x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault().ToString() != null ? x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault() : 0 ,
                edad = x.Candidato.FechaNacimiento,
                rfc = x.Candidato.RFC != null ? x.Candidato.RFC : "",
                curp = x.Candidato.CURP, 
                EstatusId = db.ProcesoCandidatos.Where(c => c.CandidatoId.Equals(x.CandidatoId)).Select(cc => cc.EstatusId).FirstOrDefault()
            }).ToList();
            return Ok(candidatos);
        }

        [HttpGet]
        [Route("getProceso")]
        public IHttpActionResult GetProceso(Guid VacanteId, Guid ReclutadorId)
        {
            try
            {
                var postulate = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(VacanteId) & x.ReclutadorId.Equals(ReclutadorId) & x.EstatusId != 27 & x.EstatusId != 40).Select(c => new
                {
                    Id = c.Id,
                    folio = c.Folio,
                    candidatoId = c.CandidatoId,
                    estatus = c.Estatus.Descripcion,
                    estatusId = c.EstatusId,
                    horarioId = c.HorarioId,
                    horario = db.HorariosRequis.Where(x => x.Id.Equals(c.HorarioId)).Select(h => h.aHora.Hour - h.deHora.Hour == 9 ? "Completo de " + h.deHora.Hour + " a " + h.aHora.Hour : h.deHora.Hour > 12 ? "Vespertino de " + h.deHora.Hour + " a " + h.aHora.Hour : "Matutino de " + h.deHora.Hour + " a " + h.aHora.Hour).FirstOrDefault(),
                    perfil = db.PerfilCandidato.Where(x => x.CandidatoId.Equals(c.CandidatoId)).Select(x => new
                    {
                        nombre = x.Candidato.Nombre,
                        apellidoPaterno = x.Candidato.ApellidoPaterno,
                        apellidoMaterno = x.Candidato.ApellidoMaterno,
                        AreaExp = x.AboutMe.Select(ae => ae.AreaExperiencia.areaExperiencia).FirstOrDefault() != null ? x.AboutMe.Select(ae => ae.AreaExperiencia.areaExperiencia).FirstOrDefault() : "",
                        AreaInt = x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() != null ? x.AboutMe.Select(ai => ai.AreaInteres.areaInteres).FirstOrDefault() : "",
                        localidad = x.Candidato.direcciones.Select(d => d.Municipio.municipio).FirstOrDefault() + " / " + x.Candidato.direcciones.Select(d => d.Estado.estado).FirstOrDefault(),
                        sueldoMinimo = x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault().ToString() != null ? x.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault() : 0,
                        edad = x.Candidato.FechaNacimiento,
                        rfc = x.Candidato.RFC != null ? x.Candidato.RFC : "",
                        curp = x.Candidato.CURP != null ? x.Candidato.CURP : ""
                    }),
                    usuario = c.Reclutador,
                    fecha = c.Fch_Modificacion
                }).ToList();

                return Ok(postulate);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }



        }

        [HttpPost]
        [Route("updateStatusVacante")]
        public IHttpActionResult UpdateStatusVacante(ProcesoDto datos)
        {
            try
            {
                var R = db.Requisiciones.Find(datos.requisicionId);
                db.Entry(R).State = System.Data.Entity.EntityState.Modified;
                R.EstatusId = datos.estatusId;

                db.SaveChanges();

                if(datos.estatusId >= 34 && datos.estatusId <= 37)
                {
                    UpdateStatusBolsaFinalizado(datos);
                }

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpPost]
        [Route("updateStatus")]
        public IHttpActionResult UpdateStatus(ProcesoDto datos)
        {
            try
            {
                var id = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(datos.candidatoId) && x.RequisicionId.Equals(datos.requisicionId)).Select(x => x.Id).FirstOrDefault();
                var c = db.ProcesoCandidatos.Find(id);

                if (datos.estatusId == 12 && (c.EstatusId == 27 || c.EstatusId == 10))
                {
                    db.Entry(c).State = System.Data.Entity.EntityState.Modified;
                    c.EstatusId = datos.estatusId;
                    c.HorarioId = datos.horarioId;

                    db.SaveChanges();

                    return Ok(HttpStatusCode.Created);
                }
                else if(datos.estatusId != 12)
                {
                    db.Entry(c).State = System.Data.Entity.EntityState.Modified;
                    c.EstatusId = datos.estatusId;
                    c.HorarioId = datos.horarioId;

                    db.SaveChanges();

                    return Ok(HttpStatusCode.Created);
                }
                else
                {
                    return Ok(HttpStatusCode.Ambiguous);
                }
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
            
        }
        public IHttpActionResult LiberarCandidatos(ProcesoDto datos)
        {
            Guid aux = new Guid("00000000-0000-0000-0000-000000000000");
            try
            {
                var id = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(datos.candidatoId) && x.RequisicionId.Equals(datos.requisicionId)).Select(x => x.Id).FirstOrDefault();
                var c = db.ProcesoCandidatos.Find(id);

                db.Entry(c).State = System.Data.Entity.EntityState.Modified;
                c.EstatusId = datos.estatusId;

                db.SaveChanges();

                var ids = db.Postulaciones.Where(x => x.RequisicionId.Equals(datos.requisicionId) && x.CandidatoId.Equals(datos.candidatoId)).Select(x => x.Id).FirstOrDefault();
                if (ids != aux)
                {
                    var cc = db.Postulaciones.Find(ids);

                    db.Entry(cc).State = System.Data.Entity.EntityState.Modified;
                    cc.StatusId = 5;

                    db.SaveChanges();
                }
                    
                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }
        [HttpPost]
        [Route("updateStatusBolsa")]
        public IHttpActionResult UpdateStatusBolsa(ProcesoDto datos)
        {
            try
            {
                Guid aux = new Guid("00000000-0000-0000-0000-000000000000");

                var id = db.Postulaciones.Where(x => x.CandidatoId.Equals(datos.candidatoId) && x.RequisicionId.Equals(datos.requisicionId)).Select(x => x.Id).FirstOrDefault();

                if (id == aux)
                {
                    Postulacion obj = new Postulacion();
                    obj.RequisicionId = datos.requisicionId;
                    obj.CandidatoId = datos.candidatoId;
                    obj.StatusId = datos.estatusId;

                    db.Postulaciones.Add(obj);
                    db.SaveChanges();

                    return Ok(HttpStatusCode.Created);
                }
                else
                {

                    var c = db.Postulaciones.Find(id);
                    db.Entry(c).State = System.Data.Entity.EntityState.Modified;
                    c.StatusId = datos.estatusId;

                    db.SaveChanges();

                    return Ok(HttpStatusCode.Created);
                }
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        public IHttpActionResult UpdateStatusBolsaFinalizado(ProcesoDto datos)
        {
            try
            {
                Guid aux = new Guid("00000000-0000-0000-0000-000000000000");
                var ids = db.Postulaciones.Where(x => x.RequisicionId.Equals(datos.requisicionId)).Select(x => x.Id).ToList();

                foreach(Guid id in ids)
                {

                    if (id == aux)
                    {
                        Postulacion obj = new Postulacion();
                        obj.RequisicionId = datos.requisicionId;
                        obj.CandidatoId = datos.candidatoId;
                        obj.StatusId = 5;

                        db.Postulaciones.Add(obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        var c = db.Postulaciones.Find(id);
                        db.Entry(c).State = System.Data.Entity.EntityState.Modified;
                        c.StatusId = 5;

                        db.SaveChanges();

                    }

                }

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        public async Task<IHttpActionResult> EnviarSMS(string telefono, string vacante, int estatusId)
        {
            Regex reg = new Regex("[^a-zA-Z0-9] ");
            List<string> Destino = new List<string>(1) { ConfigurationManager.AppSettings["Lada"] + telefono };
            BasicAuthConfiguration BASIC_AUTH_CONFIGURATION = new BasicAuthConfiguration(ConfigurationManager.AppSettings["BaseUrl"], ConfigurationManager.AppSettings["UserInfobip"], ConfigurationManager.AppSettings["PassInfobip"]);

            SendSingleTextualSms smsClient = new SendSingleTextualSms(BASIC_AUTH_CONFIGURATION);

            if (estatusId == 17)
            {
                string texto = "Bolsa Trabajo DAMSA te felicita por Iniciar proceso a la vacante " + vacante + ". Da click http://btweb.damsa.com.mx/ para dar seguimiento";
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

                vacante = "";
            }
            else if(estatusId == 21)
            {
                string texto = "Bolsa Trabajo DAMSA, te felicita por ser finalista a la vacante " + vacante + ". Da click http://btweb.damsa.com.mx/ para dar seguimiento";
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

            }
            else if (estatusId == 27)
            {
                string texto = string.Format("Bolsa Trabajo DAMSA, informa que el cliente cubrió la vacante {0}. http://btweb.damsa.com.mx/", vacante);

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
            }
            else if(estatusId == 24)
            {
                string texto = string.Format("Bolsa Trabajo DAMSA, te felicita por ser contratado a la vacante {0}. http://btweb.damsa.com.mx/", vacante);

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

            }
            return Ok(HttpStatusCode.Created);
        }

        [HttpPost]
        [Route("sendEmailCandidato")]
        public IHttpActionResult SendEmailCandidato(ProcesoDto datos)
        {
            var path = "~/logo/logo.png";
            string fullPath = "http://192.168.8.124:333/logo/logo.png";
            path = "~/logo/boton.png";
            string fullPath2 = "http://192.168.8.124:333/logo/boton.png";
            string body = "";
            string usuario = "";
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SAGADB"].ConnectionString);
                SqlCommand cmd = new SqlCommand();
             

                cmd.CommandText = "SELECT * FROM sist.AspNetUsers WHERE IdPersona=@candidatoId";
                cmd.Parameters.AddWithValue("@candidatoId", datos.candidatoId);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = reader["UserName"].ToString();
                    }
                }

            
                conn.Close();

              // usuario = "niij_06@hotmail.com";

                if (usuario != "")
                {
                    string from = "noreply@damsa.com.mx";
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "SAGA Inn");
                    m.Subject = "Bolsa de Trabajo DAMSA";

                    if (datos.estatusId == 17)
                    {
                        if (usuario.Contains("@"))
                        {
                            m.To.Add(usuario);
                            body = "<html><head></head>";
                            body = body + "<body style=\"text-align:center; font-size:14px; font-family:'calibri'\">";
                            body = body + string.Format("<img style=\"max-width:10% !important;\" align=\"right\" src=\"{0}\" alt=\"App Logo\"/>", fullPath);
                            body = body + string.Format("<p style=\"text-align:left; font-size:14px;\">Hola, {0}</p>", datos.nombre);
                            body = body + "<br/><br/><h1>¡Felicidades!</h1>";
                            body = body + "<p>Eres uno(a) de los/las candidatos/as que inicia proceso para la vacante de</p>";
                            body = body + string.Format("<h1 style=\"color:#3366cc;\">{0}</h1>", datos.vacante);
                            body = body + "<p>Solo puedes estar en un proceso de seguimiento</p>";
                            body = body + "<p> Si esta vacante no es de tu inter&eacute;s puedes declinar a esta postulaci&oacute;n.</p>";
                            body = body + string.Format("<a href=\"http://btweb.damsa.com.mx/\" target =\"_blank\"><img src=\"{0}\"></a>", fullPath2);
                            body = body + string.Format("<p style=\"text-decoration: none;\">Este mensaje fu&eacute; dirigido a: <font style:\"text-decoration:none\"; color=\"#5d9cec\">{0}</font></p>", usuario);
                            body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                            body = body + "</body></html>";
                            m.Body = body;
                            m.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                            smtp.EnableSsl = true;
                            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                            smtp.Send(m);

                        }
                        else
                        {
                            return Ok(EnviarSMS(usuario, datos.vacante, datos.estatusId));
                        }

                    }
                    else if (datos.estatusId == 21)
                    {
                        if (usuario.Contains("@"))
                        {
                            m.To.Add(usuario);
                            body = "<html><head></head>";
                            body = body + "<body style=\"text-align:center; font-size:14px; font-family:'calibri'\">";
                            body = body + string.Format("<img style=\"max-width:10% !important;\" align=\"right\" src=\"{0}\" alt=\"App Logo\"/>", fullPath);
                            body = body + string.Format("<p style=\"text-align:left; font-size:14px;\">Hola, {0}</p>", datos.nombre);
                            body = body + "<br/><br/><h1>¡Felicidades!</h1>";
                            body = body + string.Format("<p>Eres uno(a) de los/las finalistas para la vacante de <h1 style=\"color:#3366cc;\">{0}</h1></p>", datos.vacante);
                            body = body + "<p>Solo puedes estar en un proceso de seguimiento</p>";
                            body = body + "<p> Si esta vacante no es de tu inter&eacute;s puedes declinar a esta postulaci&oacute;n.</p>";
                            body = body + string.Format("<a href=\"http://btweb.damsa.com.mx/\" target =\"_blank\"><img src=\"{0}\"></a>", fullPath2);
                            body = body + string.Format("<p style=\"text-decoration: none;\">Este mensaje fu&eacute; dirigido a: <font style:\"text-decoration:none\"; color=\"#5d9cec\">{0}</font></p>", usuario);
                            body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                            body = body + "</body></html>";
                            m.Body = body;
                            m.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                            smtp.EnableSsl = true;
                            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                            smtp.Send(m);
                        }
                        else
                        {
                            return Ok(EnviarSMS(usuario, datos.vacante, datos.estatusId));
                        }

                    }
                    else if (datos.estatusId == 27)
                    {
                        if (usuario.Contains("@"))
                        {
                            m.To.Add(usuario);
                            body = "<html><head></head><body style=\"text-align:center; font-size:14px; font-family:'calibri'\">";
                            body = body + string.Format("<img style=\"max-width:10% !important;\" align=\"right\" src=\"{0}\" alt=\"App Logo\"/>", fullPath);
                            body = body + string.Format("<p style=\"text-align:left; font-size:14px;\">Hola, {0}</p>", datos.nombre);
                            body = body + "<br/><br/>";
                            body = body + string.Format("<p>Gracias por tu inter&eacute;s en nuestra empresa y por el tiempo que has dedicado para el proceso de <h1 style=\"color:#3366cc;\">{0}</h1></p>", datos.vacante);
                            body = body + "<p>Te escribimos para informarte que el cliente ha seleccionado un candidato, sin embargo y con tu conformidad, conservaremos tu CV en nuestra base de datos para futuras selecciones.</p>";
                            body = body + "<p>Agradecemos tu participaci&oacute;n</p>";
                            body = body + "<p>En la siguiente liga puedes encontrar vacantes similares:</p>";
                            body = body + string.Format("<a href=\"http://btweb.damsa.com.mx/\" target =\"_blank\"><img src=\"{0}\"></a>", fullPath2);
                            body = body + string.Format("<p style=\"text-decoration: none;\">Este mensaje fu&eacute; dirigido a: <font style:\"text-decoration:none\"; color=\"#5d9cec\">{0}</font></p>", usuario);
                            body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                            body = body + "</body></html>";

                            m.Body = body;
                            m.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                            smtp.EnableSsl = true;
                            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                            smtp.Send(m);
                        }
                        else
                        {
                            return Ok(EnviarSMS(usuario, datos.vacante, datos.estatusId));
                        }
                    }
                    else if(datos.estatusId == 24)
                    {
                        if (usuario.Contains("@"))
                        {
                            m.To.Add(usuario);
                            body = "<html><head></head><body style=\"text-align:center; font-size:14px; font-family:'calibri'\">";
                            body = body + string.Format("<img style=\"max-width:10% !important;\" align=\"right\" src=\"{0}\" alt=\"App Logo\"/>", fullPath);
                            body = body + string.Format("<p style=\"text-align:left; font-size:14px;\">Hola, {0}</p>", datos.nombre);
                            body = body + "<br/><br/><h1>¡Felicidades!</h1>";
                            body = body + string.Format("<p>Eres uno(a) de los/las contratados para la vacante <h1 style=\"color:#3366cc;\">{0}</h1></p>", datos.vacante);
                            body = body + string.Format("<p style=\"text-decoration: none;\">Este mensaje fu&eacute; dirigido a: <font style:\"text-decoration:none\"; color=\"#5d9cec\">{0}</font></p>", usuario);
                            body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                            body = body + "</body></html>";

                            m.Body = body;
                            m.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                            smtp.EnableSsl = true;
                            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                            smtp.Send(m);
                        }
                        else
                        {
                            return Ok(EnviarSMS(usuario, datos.vacante, datos.estatusId));
                        }

                    }
                }
                return Ok(HttpStatusCode.Created);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

            //
       
        }

        //falta mensaje
        [HttpPost]
        [Route("sendEmailsNoContratado")]
        public IHttpActionResult SendEmailsNoContratados(List<ProcesoDto> datos)
        {
            var path = "~/utilerias/img/logo/logo.png";
            string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(path);
            path = "~/utilerias/img/logo/boton.png";
            string fullPath2 = System.Web.Hosting.HostingEnvironment.MapPath(path);
            string body = "";
            string usuario = "bmorales@damsa.com.mx";

            string from = "noreply@damsa.com.mx";
            MailMessage m = new MailMessage();
            m.From = new MailAddress(from, "SAGA Inn");
            m.Subject = "Bolsa de Trabajo DAMSA";

            try
            {
                foreach (var e in datos)
                {
                    if (e.email.Contains("@"))
                    {
                        var res = LiberarCandidatos(e);

                        m.Bcc.Add(e.email);
                        body = "<html><head></head><body style=\"text-align:center; font-family:'calibri'\">";
                        body = body + string.Format("<img style=\"max-width:10% !important;\" align=\"right\" src=\"{0}\" alt=\"App Logo\"/>", fullPath);
                        body = body + string.Format("<p style=\"text-align:left; font-size:14px;\">Hola, {0}</p>", e.nombre);
                        body = body + "<br/><br/><br/>";
                        body = body + string.Format("<p>Gracias por tu inter&eacute;s en nuestra empresa y por el tiempo que has dedicado para el proceso de <h1 style=\"color:#3366cc;\">{0}</h1></p>", e.vacante);
                        body = body + "<p>Te escribimos para informarte que el cliente ha seleccionado un candidato, sin embargo y con tu conformidad, conservaremos tu CV en nuestra base de datos para futuras selecciones.</p>";
                        body = body + "<p>Agradecemos tu participaci&oacute;n</p>";
                        body = body + "<p>En la siguiente liga puedes encontrar vacantes similares:</p>";
                        body = body + string.Format("<a href=\"http://btweb.damsa.com.mx/\" target =\"_blank\"><img src=\"{0}\"></a>", fullPath2);
                        body = body + string.Format("<p style=\"text-decoration: none;\">Este mensaje fu&eacute; dirigido a: <font color=\"#5d9cec\">{0}</font></p>", e.email);
                        body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                        body = body + "</body></html>";

                        m.Body = body;
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                        smtp.Send(m);
                    }
                    else
                    {
                        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SAGADB"].ConnectionString);
                        SqlCommand cmd = new SqlCommand();


                        cmd.CommandText = "SELECT * FROM sist.AspNetUsers WHERE IdPersona=@candidatoId";
                        cmd.Parameters.AddWithValue("@candidatoId", e.candidatoId);
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                usuario = reader["UserName"].ToString();
                            }
                        }


                        conn.Close();

                        var result = LiberarCandidatos(e);
                        

                        if (usuario != "")
                        {
                            var res = EnviarSMS(usuario, e.vacante, 27);
                        }

                    }
                }

               return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

            //

        }
    }
}
