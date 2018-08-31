using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Text;
using SAGA.API.Dtos;

namespace SAGA.API.Utilerias
{
    public class SendEmails
    {
        SAGADBContext db;
        List<string> emails;
        List<string> emailNoChange;
        List<string> AddEmail;
        List<GrupoUsuarios> grpUser;
        List<GrupoUsuarios> grpUserNotChange;
        IEnumerable<string> distintEmails;
        string emailString = string.Empty;

        public SendEmails()
        {
            db = new SAGADBContext();
            emails = new List<string>();
            emailNoChange = new List<string>();
            AddEmail = new List<string>();
            grpUser = new List<GrupoUsuarios>();
            grpUserNotChange = new List<GrupoUsuarios>();
        }

        /*Recupera la lista de E-mail a la cual se le mandara correo electrónico, en caso de que encuentre una Célula/Grupo este recorrerá los 
         * Usuarios que estén dentro de las mismas, sin importar cuentas células encuentre, este seguirá repitiendo hasta buscar en la Ultima celular/grupo.
         */

        public List<string> checkEmails(List<Guid> grp)
        {
            foreach (var gp in grp)
            {
                var grupoInGrupo = db.GruposUsuarios.Where(x => x.GrupoId.Equals(gp)).ToList();
                if(grupoInGrupo.Count > 0)
                {
                    foreach (var grupo in grupoInGrupo)
                    {
                        var email = db.Emails.Where(x => x.EntidadId.Equals(grupo.EntidadId)).Select(x => x.email).FirstOrDefault();
                        var tipoEntidad = grupo.Entidad.TipoEntidadId;
                        if (tipoEntidad != 4)
                        {
                            /*Agrega el correo encontrado a la lista de correos.*/
                            AddEmail.Add(email);
                        }
                    }
                }
                else
                {
                    var email = db.Emails.Where(x => x.EntidadId.Equals(gp)).Select(x => x.email).FirstOrDefault();
                    AddEmail.Add(email);
                }
               
            }
            return AddEmail;
        }

        /* Recupera la lista de E-mails que sufrirán cambios y posteriormente estos serán excluidos de la lista generada anteriormente
         * Si la entidad está en dos células diferentes y una de ellas se elimina de la requisición este usuario no recibirá el correo
         * electrónico ya que dentro de otra célula aún tiene permisos para visualizarla, lo mismo para si La entidad esta propia(usuario)
         * y este se elimina de la célula donde se encuentra, no recibirá el correo ya que esta como individual en la requisición.
        */

        public List<string> EmailsNotChange(List<GrupoUsuarios> grpNc)
        {
            foreach (GrupoUsuarios gp in grpNc)
            {
                var grupoInGrupo = db.GruposUsuarios.Where(x => x.GrupoId.Equals(gp)).ToList();
                foreach(var grupo in grupoInGrupo)
                {
                    var email = db.Emails.Where(x => x.EntidadId.Equals(grupo.EntidadId)).Select(x => x.email).FirstOrDefault();
                    var tipoEntidad = grupo.Entidad.TipoEntidadId;
                    if (tipoEntidad != 4)
                    {
                        /*Agrega el correo encontrado a la lista de correos.*/
                        emailNoChange.Add(email);
                    }
                }
            }
            return emailNoChange;
        }

        public List<Guid> GetGrupo(Guid grupo, List<Guid> listaIds)
        {
            if (!listaIds.Contains(grupo))
            {
                listaIds.Add(grupo);
                var listadoNuevo = db.GruposUsuarios
                    .Where(g => g.GrupoId.Equals(grupo) & g.Grupo.Activo)
                           .Select(g => g.GrupoId)
                           .ToList();
                foreach (Guid g in listadoNuevo)
                {
                    var gp = db.GruposUsuarios
                        .Where(x => x.GrupoId.Equals(g))
                        .Select(x => x.EntidadId)
                        .ToList();
                    foreach (Guid gr in gp)
                    {
                        GetGrupo(gr, listaIds);
                    }
                }
            }
            return listaIds;
        }

        public void ConstructEmail(List<AsignacionRequi> asignaciones, List<AsignacionRequi> NotChange, string action, Int64 Folio, string Usuario, string VBr)
        {
            try
            {
                List<Guid> grp = new List<Guid>();
                foreach (AsignacionRequi asg in asignaciones)
                {
                    grpUser = db.GruposUsuarios.Where(x => x.GrupoId.Equals(asg.GrpUsrId)).ToList();
                    foreach (var grps in grpUser)
                    {
                        grp = GetGrupo(grps.EntidadId, grp);
                    }

                    if (grpUser.Count() > 0)
                    {
                        var emails = checkEmails(grp).Distinct();
                    }
                    else
                    {
                        var sendEmail = db.Emails.Where(x => x.EntidadId.Equals(asg.GrpUsrId)).Select(x => x.email).FirstOrDefault();
                        if (sendEmail != null)
                            AddEmail.Add(sendEmail);

                    }
                }

                if (NotChange != null)
                {
                    if (NotChange.Count() > 0)
                    {
                        foreach (AsignacionRequi nc in NotChange)
                        {
                            grpUserNotChange = db.GruposUsuarios.Where(x => x.GrupoId.Equals(nc.GrpUsrId)).ToList();
                            if (grpUserNotChange.Count() > 0)
                            {
                                var emails = EmailsNotChange(grpUserNotChange).Distinct();
                            }
                            else
                            {
                                var sendEmail = db.Emails.Where(x => x.EntidadId.Equals(nc.GrpUsrId)).Select(x => x.email).FirstOrDefault();
                                emailNoChange.Add(sendEmail);
                            }
                        }
                    }
                }
                /* Exluir los correo de la lista AddEmail que no deben recibir el correo electronico.*/
                distintEmails = AddEmail.Except(emailNoChange);
                if (distintEmails.Count() > 0)
                {
                    string body = string.Empty;
                    string from = ConfigurationManager.AppSettings["ToEmail"];
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "SAGA Inn");

                    foreach (string x in distintEmails)
                    {
                        m.To.Add(x.ToString());
                    }

                    if (action == "C")
                    {
                        m.Subject = "Asignacion de Requisicion " + Folio;
                        body = "<p>Asignación de Requisición:</p>";
                        body = body + string.Format("<br/>Se comunica de la manera más atenta que el usuario <strong>{0}</strong> te ha asignado para trabajar la vacante <strong>{1}</strong> la cual se encuentra con un folio de requisición: <strong style='background-color:yellow;'><big>{2}</big></strong>. ", Usuario, VBr, Folio);
                        body = body + "<p>Para ver tus requisiciones asignadas ingresa a tu panel de reclutamiento seguido de entidades de reclutamiento, selecciona la opción de vacantes, para dar el seguimiento correspondiente.</p> ";
                        body = body + "<p>Gracias por tu atención. </p> <p>Saludos.</p>";
                    }
                    if (action == "D")
                    {
                        m.Subject = "Des-asignación  de Requisicion";
                        body = "<p>Des-asignación  de Requisición:</p>";
                        body = body + string.Format("<br/>Se comunica de la manera más atenta que el usuario <strong>{0}</strong> te ha desasignado de la vacante <strong>{1}</strong> la cual se encuentra en la requisición FOLIO: <strong><big>{2}</big></strong>.", Usuario, VBr, Folio);
                        body = body + "<p>Gracias por tu atención. </p> <p>Saludos.</p>";
                    }
                    if (action == "RD")
                    {
                        m.Subject = "Eliminación de Requisicion";
                        body = "<p>Eliminación de Requisición:</p>";
                        body = body + string.Format("<br/>Se comunica de la manera más atenta que la vacante <strong>{0}</strong> la cual se encuentra en la requisición FOLIO: <strong><big>{1}</big></strong>, fue eliminada.", VBr, Folio);
                        body = body + "<p>Gracias por tu atención. </p> <p>Saludos.</p>";
                    }

                    if (action == "RU")
                    {
                        m.Subject = "Cancelación de Requisicion";
                        body = "<p>Cancelación de Requisición:</p>";
                        body = body + string.Format("<br/>Se comunica de la manera más atenta que la vacante <strong>{0}</strong> la cual se encuentra en la requisición FOLIO: <strong><big>{1}</big></strong>, fue cancelada.", VBr, Folio);
                        body = body + "<p>Gracias por tu atención. </p> <p>Saludos.</p>";
                    }

                    m.Body = body;
                    m.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                    smtp.Send(m);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public void SendEmailRegistro(PersonasDtos dtos)
        {
            string body = "";
            string email = dtos.Email.Select(x => x.email).FirstOrDefault().ToString();

            var aux = db.Usuarios.Where(x => x.Id.Equals(dtos.EntidadId)).Select(f => new
            {
                fecha = f.fch_Creacion,
                pass = f.Password

            }).FirstOrDefault();

            var emails = db.Usuarios.Where(x => x.TipoUsuarioId.Equals(1)).Select(e => new
            {
                email = db.Emails.Where(x => x.EntidadId.Equals(e.Id)).Select(em => em.email).FirstOrDefault()

            }).ToList();

            string from = "noreply@damsa.com.mx";
            MailMessage m = new MailMessage();
            m.From = new MailAddress(from, "SAGA Inn");
            m.To.Add(email);
            foreach (var e in emails)
            {
                m.Bcc.Add(e.email.ToString());
            }

            m.Subject = "Tu acceso al sistema SAGA ERP de DAMSA está listo!";
            body = "<html><body><table width=\"80%\" style=\"font-family:'calibri'\">";
            body = body + "<tr><th bgcolor=\"#044464\" style=\"color:white; text-align:left;\">Se creó una nueva cuenta para SAGA ERP </th></ tr>";
            body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Clave / Usuario de Empleado :</font></td></tr>";
            body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td> {0} / {1} </td></tr>", dtos.Clave, dtos.Usuario);
            body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Nombre :</font></td></tr>";
            body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td> {0} {1} {2} </td></tr>", dtos.nombre, dtos.apellidoPaterno, dtos.apellidoMaterno);
            body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Correo :</font></td></tr>";
            body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td> {0} </td></tr>", email);
            body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Contraseña :</font></td></tr>";
            body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td> {0} </td></tr>", aux.pass);
            body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Registrado :</font></td></tr>";
            body = body + string.Format("<tr bgcolor=\"#FDC613\"><td>{0}<br/>", aux.fecha);
            body = body + "<p> Podrás acceder mediante la siguiente dirección: http://websb.damsa.com.mx<br/>";
            body = body + "Quedamos a tus órdenes para cualquier relativo al correo inntec@damsa.com.mx </p></td></tr></table></body></html>";

            m.Body = body;
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
            smtp.Send(m);
        }


    }
}