using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Text;
using System.Web.Http;
using SAGA.API.Dtos;
using SAGA.API.Dtos.Reclutamiento.Seguimientovacantes;
using System.Net;
using System.Globalization;

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
            try
            {
                string body = "";
                string email = dtos.Email.Select(x => x.email).FirstOrDefault().ToString();
                string webERP = ConfigurationManager.AppSettings["WEBERP"].ToString();

                var aux = db.Usuarios.Where(x => x.Id.Equals(dtos.EntidadId)).Select(f => new
                {
                    fecha = f.fch_Creacion,
                    pass = f.Password

                }).FirstOrDefault();

                var emails = db.Usuarios.Where(x => x.TipoUsuarioId.Equals(1) && x.Activo.Equals(true)).Select(e => new
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
                body = body + string.Format("<p> Podrás acceder mediante la siguiente dirección: {0} <br/>", webERP);
                body = body + "Quedamos a tus órdenes para cualquier relativo al correo inntec@damsa.com.mx </p></td></tr></table></body></html>";

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

        public void SendEmaiNewRegistro(PersonasDtos dtos)
        {
            try
            {
                string body = "";
                string email = dtos.Email.Select(x => x.email).FirstOrDefault().ToString();
                string webERP = ConfigurationManager.AppSettings["WEBERP"].ToString();
                DateTime fechaCreacion = DateTime.Now;

                

                var emails = db.Usuarios.Where(x => x.TipoUsuarioId.Equals(1) && x.Activo.Equals(true)).Select(e => new
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
                body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td> {0} </td></tr>", dtos.Password);
                body = body + "<tr bgcolor=\"#1D7FB0\"><td><font color=\"white\"> Registrado :</font></td></tr>";
                body = body + string.Format("<tr bgcolor=\"#FDC613\"><td>{0}<br/>",fechaCreacion);
                body = body + string.Format("<p> Podrás acceder mediante la siguiente dirección: {0} <br/>", webERP);
                body = body + "Quedamos a tus órdenes para cualquier relativo al correo inntec@damsa.com.mx </p></td></tr></table></body></html>";

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

        public bool SendEmailRequisPuras(Guid RequisicionId)
        {
            try
            {
                bool facturada = false;
                int[] estatus = { 44,45,46 };
                var estatusRequi = db.EstatusRequisiciones
                    .Where(e => e.RequisicionId.Equals(RequisicionId))
                    .Select(e => e.EstatusId ).ToList();
                foreach(var e in estatusRequi)
                {
                    if (estatus.Contains(e))
                    {
                        facturada = true;
                        break;
                    }
                }


                var requi = db.Requisiciones
                    .Where(r => r.Id.Equals(RequisicionId))
                    .Select(x => new
                    {
                        folio = x.Folio,
                        fch_Creacion = x.fch_Creacion,
                        solicita = 
                            db.Entidad
                            .Where(en => en.Id.Equals(x.PropietarioId))
                            .Select(em => new
                            {
                                nombre = em.Nombre + " " + em.ApellidoPaterno + " " + (em.ApellidoMaterno != null ? em.ApellidoMaterno : "")
                            })
                        .FirstOrDefault(),
                        empresa = x.Cliente.RazonSocial,
                        noVacantes = x.horariosRequi.Sum(h => h.numeroVacantes),
                        puesto = x.VBtra,
                        sueldoMinimo = x.SueldoMinimo,
                        sueldoMaximo = x.SueldoMaximo,
                        estatusId = x.EstatusId,
                        estaus = x.Estatus.Descripcion,
                        estado = x.Direccion.Estado.estado,
                        propietarioid = x.PropietarioId
                    }).FirstOrDefault();

                var unidadDeNegocio = db.Usuarios
                    .Where(u => u.Id.Equals(requi.propietarioid))
                    .Select(u => u.Sucursal.UnidadNegocioId)
                    .FirstOrDefault();

                var email = new List<string>();
                var sucursalesMTY = new List<Guid>();
                var sucursales = db.OficinasReclutamiento
                    .Where(U => U.UnidadNegocioId.Equals(unidadDeNegocio))
                    .Select(u => u.Id).ToList();
                
                if(unidadDeNegocio != 3)
                {
                    sucursalesMTY = db.OficinasReclutamiento
                            .Where(U => U.UnidadNegocioId.Equals(3))
                            .Select(u => u.Id).ToList();
                    foreach (var s in sucursalesMTY)
                    {
                        sucursales.Add(s);
                    }
                }

                email = db.Usuarios
                        .Where(x => x.Activo.Equals(true))
                        .Where(x => x.TipoUsuarioId.Equals(3))
                        .Where(x => x.Departamento.Clave.Equals("VTAS"))
                        .Where(x => sucursales.Contains(x.SucursalId))
                        .Select(x =>
                               x.emails
                                .Where(e => e.EntidadId.Equals(x.Id))
                                .Select(e => e.email)
                                .FirstOrDefault()
                            )
                        .ToList();
                
                var emailProp = db.Emails.Where(x => x.EntidadId.Equals(requi.propietarioid)).Select(x => x.email).FirstOrDefault();

                string body = "";
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "SAGA Inn");
                //m.To.Add(email);
                

                if (facturada)
                {
                    m.To.Add(ConfigurationManager.AppSettings["FacturacionEmail"].ToString());
                    foreach (var e in email)
                    {
                        m.CC.Add(e.ToString());
                    }
                }
                else
                {
                    foreach (var e in email)
                    {
                        m.To.Add(e.ToString());
                    }
                }
                m.CC.Add(emailProp);
                switch (requi.estatusId)
                {
                    case 8:
                        m.Subject = string.Format("Cancelación de Vacante con Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                        body = string.Format("<p>Por este medio se les informa que se ha cancelado el Reclutamiento Puro con el número de folio <strong><a href=\"https://weberp.damsa.com.mx\">{0}</a></strong>:</p>", requi.folio);
                        break;
                    case 9:
                        m.Subject = string.Format("Eliminación de Vacante con Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                        body = string.Format("<p>Por este medio se les informa que se ha Eliminado el Reclutamiento Puro con el número de folio <strong><a href=\"https://weberp.damsa.com.mx\">{0}</a></strong>:</p>", requi.folio);
                        break;
                    case 43:
                        m.Subject = string.Format("Nueva Vacante con Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                        body = string.Format("<p>Por este medio se les informa que existe un Nuevo Reclutamiento Puro con el número de folio <strong><a href=\"https://weberp.damsa.com.mx\">{0}</a></strong>:</p>", requi.folio);
                        break;
                    case 44:
                        m.Subject = string.Format("Solicitud de Facturación de Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                        body = string.Format("<p>Por este medio se les informa, que se requiere factura para el nuevo Reclutamiento Puro con el número de folio <strong><a href=\"https://weberp.damsa.com.mx\">{0}</a></strong>:</p>", requi.folio);
                        break;
                    case 46:
                        m.Subject = string.Format("Seguimiento de Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                        break;
                }
                
                body = body + string.Format("<p style=\"font-size:12px;\"><strong> FECHA SOLICITUD: </strong>{0}<p>", requi.fch_Creacion);
                body = body + string.Format("<p style=\"font-size:12px;\"><strong> SOLICITANTE: </strong>{0}<p>", requi.solicita.nombre.ToUpper());
                body = body + string.Format("<p style=\"font-size:12px;\"><strong> EMPRESA: </strong>{0}<p>", requi.empresa.ToUpper());
                body = body + string.Format("<p style=\"font-size:12px;\"><strong> ESTADO: </strong>{0}<p>", requi.estado.ToUpper());
                body = body + string.Format("<p style=\"font-size:12px;\"><strong> NUMERO VACANTES: </strong>{0}<p>", requi.noVacantes);
                body = body + string.Format("<p style=\"font-size:12px;\"><strong> PUESTO: </strong>{0}<p>", requi.puesto.ToUpper());
                body = body + string.Format("<p style=\"font-size:12px;\"><strong> SUELDO: </strong>{0} a {1}<p>", String.Format("{0:C}", requi.sueldoMinimo), String.Format("{0:C}", requi.sueldoMaximo));
                body = body + string.Format("<p style=\"font-size:12px;\"><strong> ESTATUS VACANTE: </strong>{0}<p>", requi.estaus);
                body = body + string.Format("<p><strong> Favor de corroborar esta información y dar el seguimiento correspondiente. </strong><p>");
                body = body + string.Format("<p>Me despido de usted agradeciendo su atención y enviandole un cordial saludo.<p>");
                m.Body = body;
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                smtp.Send(m);

                return true;

            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }
    }
}