using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
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

        public void ConstructEmail(List<AsignacionRequi> asignaciones, List<AsignacionRequi> NotChange, string action, Int64 Folio, string Usuario, string VBr, List<CoincidenciasDto> Coincidencias)
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

                        var inicio = "<html><head><style>td { border: solid #2471A3 1px; padding-left:5px; padding-right:5px;padding-top:1px; padding-bottom:1px;font-size:9pt; color: #3498DB;font-family:'calibri'; width: 25%; text-align: left; vertical-align: top; border-spacing: 0; border-collapse: collapse;} ";
                        inicio = inicio + "p { font - family:'calibri'; } th { font - family:'calibri'; width: 25 %; text - align: left; vertical - align: top; border: solid blue 1px; border - spacing: 0; border - collapse: collapse; background: #3498DB; color:white;}";
                        inicio = inicio + "h3 { font - family:'calibri'; } table { width: 100 %; }</style></head><body style =\"text-align:center; font-family:'calibri'; font-size:10pt;\"><br><br><p> Asignación de Requisición:</p>";

                        
                        body = inicio;
                        body = body + string.Format("<br/>Se comunica de la manera más atenta que el usuario <strong>{0}</strong> te ha asignado para trabajar la vacante <strong>{1}</strong> la cual se encuentra con un folio de requisición: <strong style='background-color:yellow;'><big>{2}</big></strong>.", Usuario, VBr, Folio);

                        if (Coincidencias.Count > 0)
                        {
                            body = body + string.Format("<br><p>Coincidencias Candidatos:</p>");
                            body = body + "<table class='table'>";
                            body = body + "<tr><th align=center>Candidato</th><th align=center>Subcategoria</th><th align=center>Rango Salarial</th><th align=center>Edad</th></tr>";
                            for (int i = 0; i < Coincidencias.Count(); i++)
                            {
                                body = body + "<tr><td align=center> " + Coincidencias[i].Nombre + "</td><td align=center>" + Coincidencias[i].Subcategoria + "</td><td align=center>" + Coincidencias[i].SueldoMinimo + "-" +  Coincidencias[i].SueldoMaximo + "</td><td align=center>" + Coincidencias[i].Edad + "</td></tr>";
                            }
                            body = body + "</table>";
                        }
                        body = body + "<p>Para ver tus requisiciones asignadas ingresa a tu panel de reclutamiento seguido de entidades de reclutamiento, selecciona la opción de vacantes, para dar el seguimiento correspondiente.</p> ";
                        body = body + "<p>Gracias por tu atención. </p> <p>Saludos.</p>";
                        body = body + "</body></html>";
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

                    body = body + string.Format("<p></p><p><a href=\"https://weberp.damsa.com.mx\"><h4>Link de acceso al ERP </h4></a></p>");

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

        public void SendEmailRegistro(PersonaSendEmail dtos)
        {
            try
            {
                string body = "";
                string email = dtos.Email;
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
                body = body + "Quedamos a tus órdenes para cualquier relativo al correo inntec@damsa.com.mx </p></td></tr></table>";
                body = body + string.Format("<p></p><p><a href=\"https://weberp.damsa.com.mx\"><h4>Link de acceso al ERP </h4></a></p></body></html>");

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
                body = body + "<tr><th bgcolor=\"#044464\" style=\"color:white; text-align:left;\">Se creó una nueva cuenta para SAGA ERP </th></tr>";
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
                body = body + "Quedamos a tus órdenes para cualquier relativo al correo inntec@damsa.com.mx </p></td></tr></table>";
                body = body + string.Format("<p></p><p><a href=\"https://weberp.damsa.com.mx\"><h4>Link de acceso al ERP </h4></a></p></body></html>");

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
                int[] mty = {6,7,10,19,28,24};
                int[] gdl = {1,3,8,10,11,14,16,18,2,25,26,32};
                int[] mx = {4,5,9,12,13,15,17,20,21,22,23,27,29,30,31};
                string GrVtasEmail = "", GVtasEmail = "", GGEmail = "";
                List<string> GrVEmails = null;
                List<string> GVEmails = null;
                bool isDurango = false;


                bool facturada = false;
                int[] estatus = { 44,45,46};
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
                        propietarioid = x.PropietarioId,
                        estadoId = x.Direccion.EstadoId,
                    }).FirstOrDefault();

                var facturacion = db.FacturacionPuro.Where(f => f.RequisicionId.Equals(RequisicionId))
                    .Select(f => new
                    {
                        porcentage = f.Porcentaje,
                        monto = f.Monto,
                        perContratado = f.PerContratado,
                        montoContratado = f.MontoContratado,
                    }).FirstOrDefault();

                if(requi.estadoId != 10)
                {
                    if (gdl.Contains(Convert.ToInt32(requi.estadoId)))
                    {
                        GrVtasEmail = db.Usuarios
                             .Where(u => u.TipoUsuarioId.Equals(12) && u.Departamento.Clave.Equals("VTAS") && u.Sucursal.UnidadNegocio.Id.Equals(1) && u.Activo.Equals(true))
                             .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                             .FirstOrDefault();
                        GVtasEmail = db.Usuarios
                            .Where(u => u.TipoUsuarioId.Equals(3) && u.Departamento.Clave.Equals("VTAS") && u.Sucursal.UnidadNegocio.Id.Equals(1) && u.Activo.Equals(true))
                            .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                            .FirstOrDefault();

                    }

                    if (mx.Contains(Convert.ToInt32(requi.estadoId)))
                    {
                        GrVtasEmail = db.Usuarios
                             .Where(u => u.TipoUsuarioId.Equals(12) && u.Departamento.Clave.Equals("VTAS") && u.Sucursal.UnidadNegocio.Id.Equals(2) && u.Activo.Equals(true))
                             .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                             .FirstOrDefault();
                        GVtasEmail = db.Usuarios
                            .Where(u => u.TipoUsuarioId.Equals(3) && u.Departamento.Clave.Equals("VTAS") && u.Sucursal.UnidadNegocio.Id.Equals(2) && u.Activo.Equals(true))
                            .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                            .FirstOrDefault();
                    }

                    if (mty.Contains(Convert.ToInt32(requi.estadoId)))
                    {
                        GrVtasEmail = db.Usuarios
                            .Where(u => u.TipoUsuarioId.Equals(12) && u.Departamento.Clave.Equals("VTAS") && u.Sucursal.UnidadNegocio.Id.Equals(3) && u.Activo.Equals(true))
                            .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                            .FirstOrDefault();
                        GVtasEmail = db.Usuarios
                            .Where(u => u.TipoUsuarioId.Equals(3) && u.Departamento.Clave.Equals("VTAS") && u.Sucursal.UnidadNegocio.Id.Equals(3) && u.Activo.Equals(true))
                            .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                            .FirstOrDefault();
                    }
                }
                else
                {
                    GrVEmails = db.Usuarios
                            .Where(u => u.TipoUsuarioId.Equals(12) && u.Departamento.Clave.Equals("VTAS") && (u.Sucursal.UnidadNegocio.Id.Equals(1) || u.Sucursal.UnidadNegocio.Id.Equals(3)) && u.Activo.Equals(true))
                            .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                            .ToList();
                    GVEmails = db.Usuarios
                        .Where(u => u.TipoUsuarioId.Equals(3) && u.Departamento.Clave.Equals("VTAS") && (u.Sucursal.UnidadNegocio.Id.Equals(1) || u.Sucursal.UnidadNegocio.Id.Equals(3)) && u.Activo.Equals(true))
                        .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                        .ToList();

                    isDurango = true;
                }
                
                if(facturacion != null)
                {
                    if (facturacion.porcentage < 50)
                    {
                        GGEmail = db.Usuarios
                            .Where(u => u.TipoUsuarioId.Equals(14) && u.Departamento.Clave.Equals("GRTS") && u.Activo.Equals(true))
                            .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                            .FirstOrDefault();
                    }
                }
                
                
                var emailProp = db.Emails.Where(x => x.EntidadId.Equals(requi.propietarioid)).Select(x => x.email).FirstOrDefault();

                string body = "";
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.Priority = MailPriority.High;
                m.From = new MailAddress(from, "SAGA Inn");
                body = string.Format("<html><head></head> <body style=\"text-align:justify; font-size:14px; font-family:'calibri'\"><div style =\"margin-left: 5px\">");
                switch (requi.estatusId)
                {
                    case 8:
                        m.To.Add(emailProp);

                        if (!isDurango)
                        {
                            m.CC.Add(GrVtasEmail);
                            m.CC.Add(GVtasEmail);
                        }
                        else
                        {
                            foreach (var e in GrVEmails)
                            {
                                m.CC.Add(e);
                            }
                            foreach (var e in GVEmails)
                            {
                                m.CC.Add(e);
                            }
                        };
                        if (facturada)
                        {
                            m.CC.Add(ConfigurationManager.AppSettings["FacturacionEmail"].ToString());
                        }
                        m.Subject = string.Format("Cancelación de Vacante con Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                        body = body + string.Format("<strong style=\"color: #159EF7\">Por este medio se les informa que se ha cancelado el Reclutamiento Puro con el número de folio {0}</strong>", requi.folio);
                        break;
                    case 9:
                        m.To.Add(emailProp);
                        if (!isDurango)
                        {
                            m.CC.Add(GrVtasEmail);
                            m.CC.Add(GVtasEmail);
                        }
                        else
                        {
                            foreach(var e in GrVEmails)
                            {
                                m.CC.Add(e);
                            }
                            foreach(var e in GVEmails)
                            {
                                m.CC.Add(e);
                            }
                        }
                        
                        if (facturada)
                        {
                            m.CC.Add(ConfigurationManager.AppSettings["FacturacionEmail"].ToString());
                        }
                        m.Subject = string.Format("Eliminación de Vacante con Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                        body = body + string.Format("<strong style=\"color: #159EF7\">Por este medio se les informa que se ha elimino el Reclutamiento Puro con el número de folio {0}.</strong>", requi.folio);
                        m.To.Add(emailProp);
                        break;
                    case 43:
                        
                        if (facturacion == null){
                            m.To.Add(GrVtasEmail);
                            m.CC.Add(emailProp);
                            m.CC.Add(GVtasEmail != null ? GVtasEmail : "");
                            m.Subject = string.Format("[AUTORIZAR FOLIO] Nueva Vacante con Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                            body = body + string.Format("<strong style=\"color: #159EF7\">Por este medio se les informa que existe un Nuevo Reclutamiento Puro con el número de folio {0}.</strong>", requi.folio);
                        }
                        else
                        {
                            m.To.Add(GGEmail);
                            m.CC.Add(GrVtasEmail);
                            m.CC.Add(emailProp);
                            m.CC.Add(GVtasEmail);
                            m.Subject = string.Format("[AUTORIZAR FOLIO] Vacante con Reclutamiento Puro Porcentage menor de 50% {0} - {1}", requi.folio, requi.empresa.ToUpper());
                            body = body + string.Format("<strong style=\"color: #159EF7\">Por este medio se les informa que existe un Reclutamiento Puro con el número de folio {0}, el cual se esta solicitando una facturación por debajo del 50%. Es necesaria previa autorización para continuar con el proceso. </strong>", requi.folio);
                        }
                       
                        break;
                    case 44:
                        m.To.Add(ConfigurationManager.AppSettings["FacturacionEmail"].ToString());
                        if (!isDurango)
                        {
                            m.CC.Add(GrVtasEmail);
                            m.CC.Add(GVtasEmail);
                        }
                        else
                        {
                            foreach (var e in GrVEmails)
                            {
                                m.CC.Add(e);
                            }
                            foreach (var e in GVEmails)
                            {
                                m.CC.Add(e);
                            }
                        }
                        m.CC.Add(emailProp);
                        m.Subject = string.Format("[FACTURAR FOLIO] Solicitud de Facturación de Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                        body = body + string.Format("<strong style=\"color: #159EF7\">Por este medio se les informa, que se requiere factura para el nuevo Reclutamiento Puro con el número de folio {0}.</strong>", requi.folio);
                        break;
                    case 45:
                        if (!isDurango)
                        {
                            m.CC.Add(GrVtasEmail);
                            m.To.Add(GVtasEmail);
                        }
                        else
                        {
                            foreach (var e in GrVEmails)
                            {
                                m.CC.Add(e);
                            }
                            foreach (var e in GVEmails)
                            {
                                m.To.Add(e);
                            }
                        }
                        m.CC.Add(emailProp);
                        m.Subject = string.Format("[aUTORIZADA PENDIENTE PAGO] Seguimiento de Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                        body = body + string.Format("<strong style=\"color: #159EF7\">La requisiciones esta autorizada, con un pago pendiente.</strong>");
                        break;
                    case 46:
                        if (!isDurango)
                        {
                            m.CC.Add(GrVtasEmail);
                            m.To.Add(GVtasEmail);
                        }
                        else
                        {
                            foreach (var e in GrVEmails)
                            {
                                m.CC.Add(e);
                            }
                            foreach (var e in GVEmails)
                            {
                                m.To.Add(e);
                            }
                        }
                        m.CC.Add(emailProp);
                        m.Subject = string.Format("[FOLIO ASIGNADO] Seguimiento de Reclutamiento Puro {0} - {1}.", requi.folio, requi.empresa.ToUpper());
                        body = body + string.Format("<strong style=\"color: #159EF7\">La requisición fue asignada al Gerente de Reclutamiento.</strong>");
                        break;
                }
                
                if(facturacion != null)
                {
                    body = body + string.Format("<p><h3>INFORMACIÓN PRINCIPAL DE FACTURACIÓN.</h3></p> ");
                    body = body + string.Format("<div style=\"background-color: #FFFAD6; width: max-content; margin-left: 15px;\"><div style=\"padding: 15px 20px 15px\"> ");
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\">PORCENTAGE: </strong>{0}%</label></p>", facturacion.porcentage);
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\"> MONTO </strong>{0}</label><p>", String.Format("{0:C}", facturacion.monto));
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\"> PER-CONTRATADO </strong>{0} %</label><p>", facturacion.perContratado);
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\">> MONTO-CONTRATADO </strong>{0}</label><p>", String.Format("{0:C}", facturacion.montoContratado));
                    body = body + string.Format("</div></div>");
                }
               
                body = body + string.Format("<p><h3>INFORMACIÓN DE REQUISICION</h3></p> ");
                body = body + string.Format("<div style=\"background-color: #FFFAD6; width: max-content; margin-left: 15px;\"><div style=\"padding: 15px 20px 15px\"> ");
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">FECHA SOLICITUD: </strong>{0}</label><p>", requi.fch_Creacion);
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">SOLICITANDE: </strong>{0}</label></p>", requi.solicita.nombre.ToUpper());
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">EMPRESA: </strong>{0}</label></p>", requi.empresa.ToUpper());
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">ESTADO: </strong>{0}</label></p>", requi.estado.ToUpper());
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">NÚMERO VACANTES: </strong>{0}</label></p>", requi.noVacantes);
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">PUESTO: </strong>{0}</label></p>", requi.puesto.ToUpper());
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">SUELDO: </strong>{0} a {1}</label></p>", String.Format("{0:C}", requi.sueldoMinimo), String.Format("{0:C}", requi.sueldoMaximo));
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">ESTATUS VACANTE: </strong>{0}</label></p>", requi.estaus);
                body = body + string.Format("</div></div>");
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\"> Favor de corroborar esta información y dar el seguimeiento correspondiente </strong></label></p>");
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">Me despido de usted(es) agradeciendo su atención y enviandole un cordial saludo. </strong></label></p>");
                body = body + string.Format("<p></p><p><a href=\"https://weberp.damsa.com.mx\"><h4>Link de acceso al ERP </h4></a></p>");
                body = body + string.Format("</div></body></html>");
                m.Body = body;
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                smtp.Send(m);
                m.Dispose();

                return true;

            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }

        public bool SendEmailRedesSociales(Guid RequisicionId, string Oficio, string Comentario)
        {
            try
            {
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
                       propietarioid = x.PropietarioId,
                       escolaridades = x.escolaridadesRequi,
                       experiencia = x.Experiencia,
                       actividades = x.actividadesRequi,
                       aptitudes = x.aptitudesRequi,
                       beneficios = x.beneficiosRequi,
                       prestaciones = x.prestacionesClienteRequi,
                       aprobadorId = x.AprobadorId,
                   }).FirstOrDefault();
                List<string> Emails = new List<string>();
                var emailsProp = db.Emails.Where(x => x.EntidadId.Equals(requi.propietarioid) || x.EntidadId.Equals(requi.aprobadorId)).Select(x => x.email).ToList();
                string body = "";
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "SAGA Inn");
                m.To.Add(ConfigurationManager.AppSettings["Medios"].ToString());
                foreach(var e  in Emails){
                    m.CC.Add(e);
                }
                m.Subject = string.Format("Publicacion de Vacante en Redes Sociales {0} - {1}", requi.folio, requi.empresa.ToUpper());
                body = string.Format("<p style=\"font-size:12px;\">Por este medio se les informa que se ha solicitado publicación en redes sociales la vacante con número de folio <strong><a href=\"https://weberp.damsa.com.mx\">{0}</a></strong>:</p>", requi.folio);

                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\"> OFICIO: </strong><label>{0}</label></p>", Oficio);
                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\"> FECHA SOLICITUD: </strong><label>{0}</label></p>", requi.fch_Creacion);
                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\"> SOLICITANTE: </strong><label>{0}</label></p>", requi.solicita.nombre.ToUpper());
                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\"> EMPRESA: </strong><label>{0}</label></p>", requi.empresa.ToUpper());
                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\"> ESTADO: </strong><label>{0}</label></p>", requi.estado.ToUpper());
                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\"> NUMERO VACANTES: </strong><label>{0}</label></p>", requi.noVacantes);
                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\"> PUESTO: </strong><label>{0}</label></p>", requi.puesto.ToUpper());
                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\"> SUELDO: </strong><label>{0} a {1}</label></p>", String.Format("{0:C}", requi.sueldoMinimo), String.Format("{0:C}", requi.sueldoMaximo));
                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\"> ESTATUS VACANTE: </strong><label>{0}</label></p>", requi.estaus);
                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\"> EXPERIENCIA: </strong><label>{0}</label></p>", requi.experiencia.ToUpper());
                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\">ACTIVIDADES</strong></p><ul>");
                foreach (var e in requi.actividades)
                {
                    body = body + string.Format("<li style=\"font-size:12px;\">{0}</li>", e.Actividades.ToUpper());
                }
                body = body + string.Format("</ul>");

                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\">ESCOLARIDADES</strong></p><ul>");
                foreach (var e in requi.escolaridades)
                {
                    body = body + string.Format("<li style=\"font-size:12px;\">{0} - {1}</li>", e.Escolaridad.gradoEstudio, e.EstadoEstudio.estadoEstudio);
                }
                body = body + string.Format("</ul>");

                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\">APTITUDES</strong></p><ul>");
                foreach (var e in requi.aptitudes)
                {
                    body = body + string.Format("<li style=\"font-size:12px;\">{0}</li>", e.Aptitud.aptitud.ToUpper());
                }
                body = body + string.Format("</ul>");

                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\">BENEFICIOS</strong></p><ul>");
                foreach (var e in requi.beneficios)
                {
                    body = body + string.Format("<li style=\"font-size:12px;\">{0} - {1} - {2}</li>", e.TipoBeneficio.tipoBeneficio.ToUpper(), String.Format("{0:C}", e.Cantidad), e.Observaciones.ToUpper());
                }
                body = body + string.Format("</ul>");

                body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\">PRESTACIONES SUPERIORES</strong></p><ul>");
                foreach (var e in requi.prestaciones)
                {
                    body = body + string.Format("<li style=\"font-size:12px;\">{0}</li>", e.Prestamo.ToUpper());
                }
                body = body + string.Format("</ul>");

                if(Comentario.Length > 0)
                {
                    body = body + string.Format("<p style=\"font-size:12px;\"><strong style=\"color: #0049FF\"> COMENTARIO ADICIONAL: </strong></p><p style=\"font-size:12px;\"><label>{0}</label></p>", Comentario);

                }

                body = body + string.Format("<p style=\"font-size:12px;\"><strong> Favor de corroborar esta información y dar el seguimiento correspondiente. </strong></p>");
                body = body + string.Format("<p style=\"font-size:12px;\">Me despido de usted agradeciendo su atención y enviándole un cordial saludo.</p>");
                body = body + string.Format("<p></p><p><a href=\"https://weberp.damsa.com.mx\"><h4>Link de acceso al ERP </h4></a></p>");

                m.Body = body;
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                smtp.Send(m);

                return true;

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }
    }
}