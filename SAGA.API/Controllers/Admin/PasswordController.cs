using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using System.Data.Entity;
using System.Collections;
using System.Data.SqlClient;
using System.Configuration;

namespace SAGA.API.Controllers.Admin
{
    [RoutePrefix("api/admin")]
    public class PasswordController : ApiController
    {
        private SAGADBContext db;
        public PasswordController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("EnviaCorreo")]
        public IHttpActionResult EnviaCorreoPass(string correo, string pass)
        {
            // Generar código
            Random rnd = new Random();
            string code = rnd.Next(100000, 999999).ToString();

            System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();
            mmsg.To.Add(correo);
            mmsg.Subject = "Restablecer contraseña";
            mmsg.SubjectEncoding = System.Text.Encoding.UTF8;
            //Cuerpo del Mensaje
            var body = "<!DOCTYPE html>";
            body = body + "<html lang=\"es\">";
            body = body + "<meta http-equiv=\"Content - Type\" content=\"text / html; charset = utf - 8\"/>";
            body = body + "<link href=\"{ { URL::Content('css/bootstrap.min.css') } }\" rel=\"stylesheet\" type=\"text / css\" media=\"all\" />";
            body = body + "<body> <div style=\"text-align: center; font-family:'calibri';\">";
            body = body + "<p>Se ha recibido una petición para restablecer la contraseña de la siguiente cuenta. " + correo + "<p>Tu nueva contraseña es: <strong>" + pass + "</strong> </p>";
            body = body + "<p>Ingresa el siguiente código para confirmar el cambio: <strong>" + code + "</strong></p>";
            body = body + "<br> Esta notificación es por seguridad y sirve para que nadie use tu cuenta de correo electrónico sin autorización. <br></ p > ";
            body = body + "<p><small>Por favor no respondas a este correo.<br>";
            body = body + "Gracias.<br>El equipo <strong> INNTEC.</strong></small></p><hr>";
            body = body + "</div></body></html>";
            mmsg.Body = body;
            mmsg.BodyEncoding = System.Text.Encoding.UTF8;
            mmsg.IsBodyHtml = true; //Si queremos que se envíe como HTML

            mmsg.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["ToEmail"], "SAGA Inntec");
            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
            cliente.EnableSsl = true;

            cliente.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);

            cliente.Send(mmsg);

            return Ok(code);
        }

        [HttpPost]
        [Route("updatePassword")]
        public IHttpActionResult PostPassowrd(UpPasswordDto datos)
        {
            var Id = db.Emails
                .Where(e => e.email.Equals(datos.Email))
                .Select(m => m.EntidadId)
                .ToList();

            var IdEntidad = db.Usuarios
                .Where(e => Id.Contains(e.Id))
                .Select(m => m.Id)
                .FirstOrDefault();

            object[] _EncryptPass = {
                        new SqlParameter("@Id", IdEntidad),
                        new SqlParameter("@Pass", datos.Password),
                    };

            var returnData = db.Database.SqlQuery<Int32>("exec sp_EncryptUpdatePassworSAGA @Id, @Pass", _EncryptPass).FirstOrDefault();

            
            return Ok(returnData);
        }

    }
}
