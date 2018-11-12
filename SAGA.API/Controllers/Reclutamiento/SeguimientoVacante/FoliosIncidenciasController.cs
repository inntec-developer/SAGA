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

        //[Route("generarFolio")]
        //[HttpPost]
        public IHttpActionResult EnviarEmail(int estatus, Guid requi, Guid reclutador)
        {

            FolioIncidencia obj = new FolioIncidencia();

            try
            {
                var propietario = db.Requisiciones.Where(x => x.Id.Equals(requi) & x.AprobadorId.Equals(reclutador)).Select(p => new {
                    propietario = p.PropietarioId, 
                    folio = p.Folio, 
                    vbtra = p.VBtra
                }).FirstOrDefault();
                var email = db.Emails.Where(x => x.EntidadId.Equals(reclutador)).Select(e => e.email).FirstOrDefault();
                var usuario = db.Usuarios.Where(x => x.Id.Equals(reclutador)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault();
                
                string body = "";
               
                if (email != "")
                {
                    string from = "noreply@damsa.com.mx";
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "SAGA Inn");
                    m.Subject = "Solicitud vacante en pausa Requisici&oacute;n, " + propietario.folio;

                    m.To.Add(email);
                    body = "<html><head></head>";
                    body = body + "<body style=\"text-align:center; font-size:14px; font-family:'calibri'\">";
                    body = body + string.Format("<p>Se comunica que el usuario {0}, levant&oacute; una solicitud de vacante \"en pausa\", Vacante {1} la cual se encuentra con un folio de requisici&oacute;n: {2}</p>", usuario, propietario.vbtra, propietario.folio );
                    body = body + "<p>Para validar la solicitud ser&aacute; necesario ingresar a Reclutamiento, seguido de entidades de reclutamiento, selecciona la opci&oacute;n Vacantes, dar clic en el bot&oacute;n Revisi&oacute;n de Vacante, para dar el seguimiento correspondiente.</p>";
                    body = body + "<br/><br/><p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
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
