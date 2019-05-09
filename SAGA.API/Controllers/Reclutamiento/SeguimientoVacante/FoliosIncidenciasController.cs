using SAGA.API.Dtos;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Net.Mail;
using System.Configuration;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/reclutamiento/SeguimientoVacante")]
    public class FoliosIncidenciasController : ApiController
    {
        private SAGADBContext db;

        public FoliosIncidenciasController()
        {
            db = new SAGADBContext();
        }

        //[Route("generarFolio")]
        //[HttpPost]
        public IHttpActionResult GenerarFolio(int estatus, Guid comentarioId)
        {
            DateTime fecha = new DateTime();

            FolioIncidencia obj = new FolioIncidencia();

            try
            {
                fecha = db.ComentariosVacantes.Where(x => x.Id.Equals(comentarioId)).Select(f => f.fch_Creacion).FirstOrDefault();

                var c = db.FolioIncidencia.Where(x => x.EstatusId.Equals(estatus)).Count();

                string folio = fecha.Year.ToString() + fecha.Month.ToString() + fecha.Day.ToString().PadLeft(2,'0') + (c + 1).ToString().PadLeft(4, '0');

                obj.EstatusId = estatus;
                obj.Folio = folio;
                obj.ComentarioId = comentarioId;

                db.FolioIncidencia.Add(obj);
                db.SaveChanges();

                return Ok(HttpStatusCode.Created);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        public bool GenerarFolioNR(int estatus, Guid comentarioId, DateTime fecha)
        {
            //DateTime fecha = new DateTime();

            FolioIncidenciasCandidatos obj = new FolioIncidenciasCandidatos();
            
            try
            {
                //fecha = db.ComentariosEntrevistas.Where(x => x.Id.Equals(comentarioId)).Select(f => f.fch_Creacion).FirstOrDefault();

                var c = db.FoliosIncidendiasCandidatos.Where(x => x.EstatusId.Equals(estatus)).Count();

                string folio = fecha.Year.ToString() + fecha.Month.ToString() + fecha.Day.ToString().PadLeft(2, '0') + (c + 1).ToString().PadLeft(4, '0');

                obj.EstatusId = estatus;
                obj.Folio = folio;
                obj.ComentarioId = comentarioId;

                db.FoliosIncidendiasCandidatos.Add(obj);

               db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //[Route("generarFolio")]
        //[HttpPost]
        public IHttpActionResult EnviarEmail(int estatus, Guid requi, Guid reclutador)
        {

            FolioIncidencia obj = new FolioIncidencia();
            //revisar para sacar solo la de pausa
            try
            {
                var propietario = db.Requisiciones.Where(x => x.Id.Equals(requi)).Select(p => new {
                    propietario = p.AprobadorId, 
                    solicitante = p.PropietarioId,
                    folio = p.Folio, 
                    vbtra = p.VBtra
                }).FirstOrDefault();
                var emailPropietario = db.Emails.Where(x => x.EntidadId.Equals(propietario.propietario)).Select(e => e.email).FirstOrDefault();
                var emailSolicitante = db.Emails.Where(x => x.EntidadId.Equals(propietario.solicitante)).Select(e => e.email).FirstOrDefault();

                var usuario = db.Usuarios.Where(x => x.Id.Equals(reclutador)).Select(n => new { nombre = n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno,
                    email = n.emails.Select(ee => ee.email).FirstOrDefault()
                }).FirstOrDefault();

             
                //email = "bmorales@damsa.com.mx";
                string body = "";
               // email = "idelatorre@damsa.com.mx";
                if (emailPropietario != "")
                {
                    string from = "noreply@damsa.com.mx";
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "SAGA Inn");
                    m.Subject = "Solicitud vacante en pausa Requisición, " + propietario.folio;

                    m.To.Add(emailPropietario);
                    m.Bcc.Add(emailSolicitante);
                    m.Bcc.Add(usuario.email.ToString());

                    body = "<html><head></head>";
                    body = body + "<body style=\"text-align:justify; font-size:14px; font-family:'calibri'\">";
                    body = body + string.Format("<p>Se comunica que el usuario <strong>{0}</strong>, levant&oacute; una solicitud de vacante \"en pausa\", Vacante <strong>{1}</strong> la cual se encuentra con un folio de requisici&oacute;n: <strong>{2}</strong></p>", usuario.nombre, propietario.vbtra, propietario.folio );
                    body = body + "<p>Para validar la solicitud ser&aacute; necesario ingresar a Reclutamiento, selecciona la opci&oacute;n Vacantes, dar clic en el bot&oacute;n Visualizar Vacantes en Pausa, para dar el seguimiento correspondiente.</p>";
                    body = body + "<br/><p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                    body = body + "</body></html>";

                    m.Body = body;
                    m.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                    smtp.Send(m);

                 }

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        public IHttpActionResult EnviarEmailNR(Guid candidatoId, Guid requi, Guid reclutador)
        {

            FolioIncidencia obj = new FolioIncidencia();

            try
            {
                var email = "idelatorre@damsa.com.mx";
                var aprovadorEmail = "idelatorre@damsa.com.mx";
                var folio = "000000000000";
                var vbtra = "No se encontró vacante";

                var propietario = db.Requisiciones.Where(x => x.Id.Equals(requi)).Select(p => new {
                    propietario = p.PropietarioId,
                    folio = p.Folio.ToString(),
                    vbtra = p.VBtra,
                    aprobador = p.AprobadorId,
                }).FirstOrDefault();
                
                if(propietario != null)
                {
                    email = db.Emails.Where(x => x.EntidadId.Equals(propietario.propietario)).Select(e => e.email).FirstOrDefault();
                    aprovadorEmail = db.Emails.Where(x => x.EntidadId.Equals(propietario.aprobador)).Select(e => e.email).FirstOrDefault();
                    folio = propietario.folio;
                    vbtra = propietario.vbtra;
                }

                var usuario = db.Usuarios.Where(x => x.Id.Equals(reclutador)).Select(n => new {
                    nombre = n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno,
                    email = n.emails.Select(e => e.email).FirstOrDefault()
                }).FirstOrDefault();

                var candidato = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(candidatoId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault();
                var motivo = db.ComentariosEntrevistas.OrderByDescending(x => x.fch_Creacion)
                    .Where(x => x.CandidatoId.Equals(candidatoId) 
                                & x.RequisicionId.Equals(requi) 
                                & x.ReclutadorId.Equals(reclutador) 
                                & x.Motivo.EstatusId == 28)
                    .Select(m => m.Motivo.Descripcion).FirstOrDefault();
                var comentario = db.ComentariosEntrevistas.OrderByDescending(x => x.fch_Creacion)
                    .Where(x => x.CandidatoId.Equals(candidatoId) 
                                & x.RequisicionId.Equals(requi) 
                                & x.ReclutadorId.Equals(reclutador) 
                                & x.Motivo.EstatusId == 28)
                    .Select(m => m.Comentario).FirstOrDefault();
                //tengo que sacar el que le corresponde por estatus
                string body = "";
                
                if (email != "")
                {
                    string from = "noreply@damsa.com.mx";
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "SAGA Inn");
                    m.Subject = "Reporte posible NR en Requisición, " + folio;

                    m.To.Add(email);
                    m.CC.Add(usuario.email.ToString());
                    m.CC.Add(aprovadorEmail);
                    //usuario, candidato, motivo, vbtra, folio
                    body = "<html><head></head>";
                    body = body + "<body style=\"text-align:justify; font-size:14px; font-family:'calibri'\">";
                    body = body + string.Format("<label>Informaci&oacute;n de candidato en Posible NR</label><p> Se comunica que el usuario / reclutador {0},report&oacute; un incidente al siguiente candidato: </p>", usuario.nombre);
                    body = body + "<table style=\"width: 75%; background-color: #f1f1c1; border-spacing: 10px;\"><tr><th>Folio:</th><th>Vacante</th><th>Candidato</th><th>Motivo</th><th>Comentario</th></tr>";
                    body = body + string.Format("<tr><td style=\"color:green; text-align: center;\">{0}</td><td style=\"text-align: center;\">{1}</td><td style=\"text-align: center;\">{2}</td><td style=\"color:red; text-align: center;\">{3}</td><td style=\"text-align: center;\">{4}</td></tr></table>", folio, vbtra, candidato, motivo, comentario);
                    body = body + "<p>Para validar y dar seguimineto a la informaci&oacute;n reportada ser&aacute; necesario ingresar a:<p/> ";
                    body = body + "<ol><li>Secci&oacute;n de Reclutamiento.</li><li>Posteriormente en Vacantes.</li><li>Dar clic en bot&oacute;n Incidente, en la parte superior.</li><li>Identificar Candidato y agregar comentario de resultado.</li><li>Aceptar o Rechazar la solicitud.</li></ol>";
                    body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                    body = body + "</body></html>";

                    m.Body = body;
                    m.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                    smtp.Send(m);

                }

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
    }
}
