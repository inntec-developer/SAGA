using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using SAGA.API.Dtos;
using System.Web.Http;
using System.Data.SqlClient;
using SAGA.API.Dtos.SistFirmas;
using System.Net;
using System.IO;

namespace SAGA.API.Utilerias
{
    public class SendEmails
    {
        SAGADBContext db;
        List<string> emails;
        List<string> emailNoChange;
        List<string> AddEmail;
        List<GrupoEmpleados> grpUser;
        List<GrupoEmpleados> grpUserNotChange;
        IEnumerable<string> distintEmails;
        string emailString = string.Empty;
        string sitioWeb = ConfigurationManager.AppSettings["WEBERP"];

        public SendEmails()
        {
            db = new SAGADBContext();
            emails = new List<string>();
            emailNoChange = new List<string>();
            AddEmail = new List<string>();
            grpUser = new List<GrupoEmpleados>();
            grpUserNotChange = new List<GrupoEmpleados>();
        }

        public bool EmailSistFirmas(FirmasDto datos, string path, string fileName)
        {
            try
            {
                string body = "";
                var xml = "";
                var asunto = string.Format("[SAGA] Sist. Firmas - {0}", datos.subject);
                var estatusnomina = db.FIRM_EstatusBitacora.Where(x => x.Activo && x.Tipo > 1).Select(e => e.Estatus.ToLower()).ToList();
                if (estatusnomina.Contains(datos.estatus.ToLower()))
                {
                    body = body + string.Format("&lt;html&gt;&lt;body style=&quot;text-align:left; font-family:'calibri'; font-size:12pt;&quot;>&lt;h3>A quien corresponda&lt;/h3>&lt;p>Por medio del presente se informa que cuenta "
                  + "con información de &lt;strong>{0}&lt;/strong> de nominas para su pago con fecha &lt;strong>{1}&lt;/strong>. &lt;/p>"
                  + "&lt;p>Para dar proceso de seguimiento de nomina ingresa a &lt;strong>Nomina&lt;/strong> del menú de Sistema de Firmas en SAGA.&lt;/p>"
                  + "&lt;p>Podrás acceder mediante la siguiente dirección: https://weberp.damsa.com.mx/login &lt;/p>"
                  + "&lt;p>Quedamos a tus órdenes para cualquier relativo al correo inntec@damsa.com.mx &lt;/p>&lt;br>&lt;br>"
                  + "&lt;p>Gracias por tu atención. Saludos&lt;/p>"
                  + "&lt;p>Este correo lo debería recibir {2} y con copia {3}" 
                  + "&lt;/body&gt;&lt;/html>", datos.estatus.ToUpper(), Convert.ToDateTime(datos.fecha).ToString("dd-MM-yyyy"), datos.email_envio, datos.email_copia);

                    //var path = "~/utilerias/Files/users/" + entidadId.ToString(); "~/Utilerias/files/SistFirmas/Nominas/"
                    var pathAdjuntos = "E:\\inetpub\\wwwroot\\sagainn\\Saga\\API.sb\\Utilerias\\files\\SistFirmas\\Nominas\\" + datos.estatus.ToLower() + Convert.ToDateTime(datos.fecha).ToString("yyyyMMdd");
                    string pathDir = System.Web.Hosting.HostingEnvironment.MapPath("~/Utilerias/files/SistFirmas/Nominas/" + datos.estatus.ToLower() + Convert.ToDateTime(datos.fecha).ToString("yyyyMMdd"));
                    DirectoryInfo folderInfo = new DirectoryInfo(pathDir);
                        
                    var files = folderInfo.GetFiles(
                                "*.*",
                                SearchOption.AllDirectories).Select(x => new
                                {
                                    fullPath = x.FullName,
                                    nom = x.Name,
                                    ext = x.Extension,
                                    size = (long)x.Length / 1024,
                                    fc = x.LastWriteTime.ToShortDateString()
                                }).OrderByDescending(o => o.fc);
                    var adjuntos = "";
                    foreach(var f in files)
                    {
                        adjuntos = adjuntos + string.Format("<Adjunto Ruta_Archivo=\"{0}\" Nombre_Archivo=\"{1}\" Eliminar_Archivo=\"0\" />", pathAdjuntos, f.nom );

                    }
                    //+"<Adjuntos>{4}</Adjuntos>"
                        xml = string.Format("<Parametros><Parametro Id_Sistema=\"SISTEMA_DEMO\" De=\"noreply@damsa.com.mx\" "
                                 + "Para=\"{0}\" Copia=\"{1}\"  CopiaOculta=\"\" Asunto=\"{2}\" Msg=\"{3}\"/> "
                                 + "<Adjuntos>{4}</Adjuntos>"
                                 + "</Parametros>", datos.email_envio, datos.email_copia, asunto, body, adjuntos);
                    
                }
                else
                {

                    body = body + string.Format("&lt;html&gt;&lt;body style=&quot;text-align:left; font-family:''calibri''; font-size:12pt;&quot;>&lt;h3>A quien corresponda&lt;/h3>&lt;p>Por medio del presente se informa que se anexa "
                        + "archivo de &lt;strong>{0}&lt;/strong> del proyecto. &lt;/p>"
                        + "&lt;table width=&quot;80%&quot;>&lt;tr>&lt;th bgcolor=&quot;#007bff&quot; style=&quot;color:white; text-align:left;&quot;>EMPRESA&lt;/th>"
                        + "&lt;th bgcolor=&quot;#007bff&quot; style=&quot;color:white; text-align:left;&quot;>{1}&lt;/th>&lt;/tr>"
                        + "&lt;tr>&lt;th bgcolor=&quot;#2790ff&quot; style=&quot;color:white; text-align:left;&quot;>SOPORTE FACTURA&lt;/th>"
                        + "&lt;th bgcolor=&quot;#2790ff&quot; style=&quot;color:white; text-align:left;&quot;>{2}&lt;/th>&lt;/tr>"
                        + "&lt;tr>&lt;th bgcolor=&quot;#007bff&quot; style=&quot;color:white; text-align:left;&quot;>CLAVE REGISTRO PATRONAL&lt;/th>"
                        + "&lt;th bgcolor=&quot;#007bff&quot; style=&quot;color:white; text-align:left;&quot;>{3}&lt;/th>&lt;/tr>"
                        + "&lt;tr>&lt;th bgcolor=&quot;#2790ff&quot; style=&quot;color:white; text-align:left;&quot;>REGISTRO PATRONAL&lt;/th>"
                        + "&lt;th bgcolor=&quot;#2790ff&quot; style=&quot;color:white; text-align:left;&quot;>{4}&lt;/th>&lt;/tr>"
                        + "&lt;tr>&lt;th bgcolor=&quot;#007bff&quot; style=&quot;color:white; text-align:left;&quot;>TIPO NOMINA&lt;/th>"
                        + "&lt;th bgcolor=&quot;#007bff&quot; style=&quot;color:white; text-align:left;&quot;>{5}&lt;/th>&lt;/tr>&lt;/table>"
                        + "&lt;p>Para dar proceso de seguimiento de nomina ingresa a Bitácora del menú de Sistema de Firmas en SAGA.&lt;/p>"
                        + "&lt;p>Podrás acceder mediante la siguiente dirección: https://weberp.damsa.com.mx/login &lt;/p>"
                        + "&lt;p>Quedamos a tus órdenes para cualquier relativo al correo inntec@damsa.com.mx &lt;/p>&lt;br>&lt;br>"
                        + "&lt;p>Gracias por tu atención. Saludos&lt;/p>&lt;/body&gt;&lt;/html>", datos.estatus.ToUpper(), datos.cliente.ToUpper(), datos.soporte, datos.clave, datos.registro, datos.nomina);

                    if (path.Length > 0)
                    {
                        xml = string.Format("<Parametros><Parametro Id_Sistema=\"SISTEMA_DEMO\" De=\"noreply@damsa.com.mx\" "
                                 + "Para=\"{0}\" Copia=\"{1}\" CopiaOculta=\"\" Asunto=\"{2}\" Msg=\"{3}\"/> "
                                 + "<Adjuntos><Adjunto Ruta_Archivo=\"{4}\" Nombre_Archivo=\"{5}\" Eliminar_Archivo=\"0\"/></Adjuntos>"
                                 + "</Parametros>", datos.email_envio, datos.email_copia, asunto, body, path, fileName);
                      //  datos.email_envio, datos.email_copia
                        //< Adjuntos >< Adjunto Ruta_Archivo =\"{4}\" Nombre_Archivo=\"{5}\" Eliminar_Archivo=\"0\" /></Adjuntos>
                    }
                    else
                    {
                        xml = string.Format("<Parametros><Parametro Id_Sistema=\"SISTEMA_DEMO\" De=\"noreply@damsa.com.mx\" "
                                 + "Para=\"{0}\" Copia=\"{1}\"  CopiaOculta=\"\" Asunto=\"{2}\" Msg=\"{3}\"/>"
                                 + "</Parametros>", datos.email_envio, datos.email_copia, asunto, body, path, fileName);
                    }
                }


                SqlParameter[] Parameters = { new SqlParameter("@ParametrosXML", xml) };
                db.Database.ExecuteSqlCommand("sp_emailFirmas @ParametrosXML", Parameters);

                //xml = string.Format("<Parametros><Parametro Id_Sistema=\"SISTEMA_DEMO\" De=\"noreply@damsa.com.mx\" "
                //              + "Para=\"{0}\" Copia=\"{1}\"  CopiaOculta=\"\" Asunto=\"{2}\" Msg=\"{3}\"/> "
                //              + "<Adjuntos>< Adjunto Ruta_Archivo =\"{4}\" Nombre_Archivo=\"{5}\" Eliminar_Archivo=\"0\" /></Adjuntos>"
                //              + "</Parametros>", datos.email_envio, datos.email_copia, asunto, body, path, fileName);

                //SqlParameter[] Parameters2 = { new SqlParameter("@ParametrosXML", xml) };
                //db.Database.ExecuteSqlCommand("sp_emailFirmas @ParametrosXML", Parameters2);
                return true;

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
            //// Create  the file attachment for this email message.
            //Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
            //// Add time stamp information for the file.
            //ContentDisposition disposition = data.ContentDisposition;
            //disposition.CreationDate = System.IO.File.GetCreationTime(file);
            //disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
            //disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
            //m.Attachments.Add(data);
        }
        public bool EnviarEmailTransfer(Guid requi, Guid usuario, string desc, Guid antId, Guid actId)
        {

            FolioIncidencia obj = new FolioIncidencia();
            //revisar para sacar solo la de pausa
            try
            {
                List<Guid> ids = new List<Guid>();
                var propietario = db.Requisiciones.Where(x => x.Id.Equals(requi)).Select(p => new {
                    coordinador = p.AprobadorId,
                    solicitante = p.PropietarioId,
                    folio = p.Folio,
                    vbtra = p.VBtra
                }).FirstOrDefault();

                ids.Add(propietario.coordinador);
                ids.Add(propietario.solicitante);
                ids.Add(antId);
                ids.Add(actId);

                var emails = db.Emails.Where(x => ids.Distinct().Contains(x.EntidadId)).Select(e => e.email).ToArray();
         
                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi) && !ids.Distinct().Contains(x.GrpUsrId) && x.Tipo == 2).Select(A => new
                {
                    emails = db.Emails.Where(e => e.EntidadId.Equals(A.GrpUsrId)).Select(ee => ee.email).FirstOrDefault()
                }).ToList();

                var user = db.Usuarios.Where(x => x.Id.Equals(usuario)).Select(n => new
                {
                    nombre = n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno,
                    email = n.emails.Select(ee => ee.email).FirstOrDefault()
                }).FirstOrDefault();


                string body = "";
                if (emails.Length > 0)
                {
                    //string from = "noreply@damsa.com.mx";
                    //MailMessage m = new MailMessage();
                    //m.From = new MailAddress(from, "SAGA Inn");
                    //m.Subject = "[SAGA] Transferencia de Requisición";

                    //m.To.Add("idelatorre@damsa.com.mx");
                    // m.To.Add(user.email);
                    string asunto = "[SAGA] Transferencia de Requisición";
                    string email_copia = "";
                    foreach (var e in emails)
                    {
                        email_copia = email_copia + e.ToString() + ", ";
                       // m.Bcc.Add(e.ToString());
                    }

                    foreach (var e in asignados)
                    {
                        // m.Bcc.Add(e.emails.ToString());
                        email_copia = email_copia + e.emails.ToString() + ",";
                    }

                    //  m.Bcc.Add("mventura@damsa.com.mx");
                    email_copia = email_copia + "mventura@damsa.com.mx";
                    body = "&lt;html>&lt;body style=&quot;text-align:left; font-family:'calibri'; font-size:10pt;&quot;>";
                    body = body + string.Format("&lt;p>Se comunica que el usuario &lt;strong>{0}&lt;/strong>, realizó una transferencia; vacante &lt;strong>{1}&lt;/strong> la cual se encuentra con un folio de requisición: &lt;strong>{2}&lt;/strong>&lt;/p>", user.nombre, propietario.vbtra, propietario.folio);
                    body = body + string.Format("&lt;p>{0}&lt;/p>", desc);
                    body = body + "&lt;br/>&lt;p>Este correo es enviado de manera automatica con fines informativos, por favor no responda a esta dirección&lt;/p>";
                    body = body + "&lt;br/>&lt;p>&lt;/p>&lt;p>&lt;a href=&quot;https://weberp.damsa.com.mx&quot;>&lt;h4>Link de acceso al ERP &lt;/h4>&lt;/a>&lt;/p>";
                    body = body + "&lt;/body>&lt;/html>";

                    var xml = string.Format("<Parametros><Parametro Id_Sistema=\"SISTEMA_DEMO\" De=\"noreply@damsa.com.mx\" "
                             + "Para=\"{0}\" Copia=\"\" CopiaOculta=\"{1}\" Asunto=\"{2}\" Msg=\"{3}\"/> "
                             + "</Parametros>", user.email, email_copia, asunto, body);


                    SqlParameter[] Parameters = { new SqlParameter("@ParametrosXML", xml) };
                    db.Database.ExecuteSqlCommand("sp_emailFirmas @ParametrosXML", Parameters);

                    //m.Body = body;
                    //m.IsBodyHtml = true;
                    //SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                    //smtp.EnableSsl = true;
                    //smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                    //smtp.Send(m);

                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void EmailNuevaRequisicion(Int64 Folio, string VBtra, string email)
        {
            try
            {
                string body = "";
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.Priority = MailPriority.High;
                m.From = new MailAddress(from, "SAGA Inn");

                m.To.Add(email);
                m.Subject = string.Format("[SAGA] Nueva Requisición {0}", Folio);
                var inicio = "<html><head><style>td { border: solid #2471A3 1px; padding-left:5px; padding-right:5px;padding-top:1px; padding-bottom:1px;font-size:9pt; color: #3498DB;font-family:'calibri'; width: 25%; text-align: left; vertical-align: top; border-spacing: 0; border-collapse: collapse;} ";
                inicio = inicio + "p { font - family:'calibri'; } th { font - family:'calibri'; width: 25 %; text - align: left; vertical - align: top; border: solid blue 1px; border - spacing: 0; border - collapse: collapse; background: #3498DB; color:white;}";
                inicio = inicio + "h3 { font - family:'calibri'; } table { width: 100 %; }</style></head><body style =\"text-align:center; font-family:'calibri'; font-size:10pt;\"><br><br><p> Nueva Requisición:</p>";

                body = inicio;
                body = body + string.Format("<p style=\"color: #159EF7\">Por este medio se informa la manera más atenta que has generado una nueva requisición con el folio <a href=\"{0}/login/{1}\">{1}</a> – {2}, el cual es necesario que se le dé el seguimiento correspondiente. </p>", sitioWeb, Folio, VBtra);
                body = body + string.Format("<p style=\"color: #159EF7\">Sin más por el momento, me despido, saludos cordiales.</p>");
                body = body + string.Format("<p></p><p><a href=\"https://weberp.damsa.com.mx\"><h4>Link de acceso al ERP </h4></a></p>");
                body = body + string.Format("<p><small>Favor de no responder este mensaje, ya que solo es de carácter informativo y son enviados automáticamente por el sistema </small></p>");
                m.Body = body;
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                smtp.Send(m);

            }
            catch(Exception ex)
            {
                string msg = ex.Message;
            }
        }

        /*Recupera la lista de E-mail a la cual se le mandara correo electrónico, en caso de que encuentre una Célula/Grupo este recorrerá los 
         * Usuarios que estén dentro de las mismas, sin importar cuentas células encuentre, este seguirá repitiendo hasta buscar en la Ultima celular/grupo.
         */

        public List<string> checkEmails(List<Guid> grp)
        {
            foreach (var gp in grp)
            {
                //var grupoInGrupo = db.GruposUsuarios.Where(x => x.GrupoId.Equals(gp)).ToList();
                //if(grupoInGrupo.Count > 0)
                //{
                //    foreach (var grupo in grupoInGrupo)
                //    {
                //        var email = db.Emails.Where(x => x.EntidadId.Equals(grupo.EntidadId)).Select(x => x.email).FirstOrDefault();
                //        var tipoEntidad = grupo.Entidad.TipoEntidadId;
                //        if (tipoEntidad != 4)
                //        {
                //            /*Agrega el correo encontrado a la lista de correos.*/
                //            AddEmail.Add(email);
                //        }
                //    }
                //}
                //else
                //{
                    var email = db.Emails.Where(x => x.EntidadId.Equals(gp)).Select(x => x.email).FirstOrDefault();
                    AddEmail.Add(email);
                //}
               
            }
            return AddEmail;
        }

        /* Recupera la lista de E-mails que sufrirán cambios y posteriormente estos serán excluidos de la lista generada anteriormente
         * Si la entidad está en dos células diferentes y una de ellas se elimina de la requisición este usuario no recibirá el correo
         * electrónico ya que dentro de otra célula aún tiene permisos para visualizarla, lo mismo para si La entidad esta propia(usuario)
         * y este se elimina de la célula donde se encuentra, no recibirá el correo ya que esta como individual en la requisición.
        */

        public List<string> EmailsNotChange(List<GrupoEmpleados> grpNc)
        {
            foreach (GrupoEmpleados gp in grpNc)
            {
                //var grupoInGrupo = db.GruposUsuarios.Where(x => x.GrupoId.Equals(gp)).ToList();
                //foreach(var grupo in grpNc)
                //{
                    var email = db.Emails.Where(x => x.EntidadId.Equals(gp)).Select(x => x.email).FirstOrDefault();
                var tipoEntidad = db.Usuarios.Where(x => x.Id.Equals(gp)).Select(x => x.TipoEntidadId).FirstOrDefault();
                    if (tipoEntidad != 4)
                    {
                        /*Agrega el correo encontrado a la lista de correos.*/
                        emailNoChange.Add(email);
                    }
                //}
            }
            return emailNoChange;
        }

        public List<Guid> GetGrupo(Guid grupo, List<Guid> listaIds)
        {
            //if (!listaIds.Contains(grupo))
            //{
            //    listaIds.Add(grupo);
            //    var listadoNuevo = db.GruposUsuarios
            //        .Where(g => g.GrupoId.Equals(grupo) & g.Grupo.Activo)
            //               .Select(g => g.GrupoId)
            //               .ToList();
            //    foreach (Guid g in listadoNuevo)
            //    {
            //        var gp = db.GruposUsuarios
            //            .Where(x => x.GrupoId.Equals(g))
            //            .Select(x => x.EntidadId)
            //            .ToList();
            //        foreach (Guid gr in gp)
            //        {
            //            GetGrupo(gr, listaIds);
            //        }
            //    }
            //}
            return listaIds;
        }

        public void ConstructEmail(List<AsignacionRequi> asignaciones, List<AsignacionRequi> NotChange, string action, Int64 Folio, string Usuario, string VBr, List<CoincidenciasDto> Coincidencias)
        {
            try
            {
                List<Guid> grp = new List<Guid>();
                foreach (AsignacionRequi asg in asignaciones)
                {
                    var sendEmail = db.Emails.Where(x => x.EntidadId.Equals(asg.GrpUsrId)).Select(x => x.email).FirstOrDefault();
                    if (sendEmail != null)
                    {
                        AddEmail.Add(sendEmail);
                    }    
                }

                if (NotChange != null)
                {
                    if (NotChange.Count() > 0)
                    {
                        foreach (AsignacionRequi nc in NotChange)
                        {
                            var sendEmail = db.Emails.Where(x => x.EntidadId.Equals(nc.GrpUsrId)).Select(x => x.email).FirstOrDefault();
                            if (sendEmail != null)
                            {
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
                    string Escolaridades = string.Empty;
                    string from = ConfigurationManager.AppSettings["ToEmail"];
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "SAGA Inn");

                    if (Coincidencias != null && Coincidencias.Count() > 0)
                    {
                        var index = Coincidencias[0].Requisicion.EscolaridadesDesc.Count() - 1;

                        for (int e = 0; e < Coincidencias[0].Requisicion.EscolaridadesDesc.Count(); e++)
                        {
                            if (e < index)
                            {
                                Escolaridades = Escolaridades + Coincidencias[0].Requisicion.EscolaridadesDesc[e] + ", ";
                            }
                            else
                            {
                                Escolaridades = Escolaridades + Coincidencias[0].Requisicion.EscolaridadesDesc[e];
                            }
                        }

                        
                    }
                    //foreach (string x in distintEmails)
                    //{
                    //    m.To.Add(x.ToString());
                    //}
                    var to = "";
                    foreach (string x in distintEmails)
                    {
                       to = to + x.ToString() + ',';
                    }
                    var asunto = "";
                    if (action == "C")
                    {

                        //m.Subject = "[SAGA] Asignación de Requisición " + Folio;
                        asunto = "[SAGA] Asignación de Requisición " + Folio;

                        var inicio = "<html><head><style>td { border: solid #2471A3 1px; padding-left:5px; padding-right:5px;padding-top:1px; padding-bottom:1px;font-size:9pt; color: #3498DB;font-family:'calibri'; width: 25%; text-align: left; vertical-align: top; border-spacing: 0; border-collapse: collapse;} ";
                        inicio = inicio + "p { font - family:'calibri'; } th { font - family:'calibri'; width: 25 %; text - align: left; vertical - align: top; border: solid blue 1px; border - spacing: 0; border - collapse: collapse; background: #3498DB; color:white;}";
                        inicio = inicio + "h3 { font - family:'calibri'; } table { width: 100 %; }</style></head><body style =\"font-family:'calibri'; font-size:12pt;\"><br><br><p> Asignación de Requisición:</p>";

                        
                        body = inicio;
                        body = body + string.Format("<br/>El usuario <strong>{0}</strong> te ha asignado para trabajar la vacante <strong>{1}</strong> la cual se encuentra con un folio de requisición: <strong style='background-color:yellow;'><big><a href=\"{3}/login/{2}\">{2}</a></big></strong>.", Usuario, VBr, Folio, sitioWeb);

                        if (Coincidencias != null && Coincidencias.Count() > 0)
                        {
                            body = body + string.Format("<br><p>Los siguientes candidatos coinciden para <b>Categoría</b> ("+Coincidencias[0].Requisicion.CategoriaDesc+"), <b>Salario</b> ($"+Coincidencias[0].Requisicion.SalarioMinimo+"- $"+Coincidencias[0].Requisicion.SalarioMaximo+"), <b>Genero</b> ("+Coincidencias[0].Requisicion.GeneroDesc+"),");
                            body = body + string.Format(" <b>Edad:</b> (" + Coincidencias[0].Requisicion.EdadMinima + "-" + Coincidencias[0].Requisicion.EdadMaxima + "), <b>Estado civil</b> (" + Coincidencias[0].Requisicion.EstadoCivilDesc + "), <b>Escolaridad</b> ("+Escolaridades+")</p>");
                            body = body + "<table class='table'>";
                            body = body + "<tr><th align=center>Candidato</th><th align=center>Subcategoria</th><th align=center>Rango Salarial</th><th align=center>Edad</th></tr>";
                            for (int i = 0; i < Coincidencias.Count(); i++)
                            {
                                body = body + "<tr><td align=center> " + Coincidencias[i].Nombre + "</td><td align=center>" + Coincidencias[i].Subcategoria + "</td><td align=center>" + Coincidencias[i].SueldoMinimo + " - " +  Coincidencias[i].SueldoMaximo + "</td><td align=center>" + Coincidencias[i].Edad + "</td></tr>";
                            }
                            body = body + "</table>";
                        }
                        body = body + "<p>Para ver tus requisiciones asignadas ingresa a tu panel de <b>Reclutamiento</b>, selecciona la opción de <b>Vacantes</b> para dar el seguimiento correspondiente.</p> ";
                        body = body + "<p>Podrás acceder mediante la siguiente dirección: https://weberp.damsa.com.mx/login" + "</p>";
                        body = body + "<p>Quedamos a tus órdenes para cualquier relativo al correo inntec@damsa.com.mx</p><p>Gracias por tu atención. </p><p>Saludos</p>";
                        body = body + "</body></html>";
                    }
                    if (action == "D")
                    {
                        //m.Subject = "[SAGA] Des-asignación  de Requisición";
                        asunto = "[SAGA] Des-asignación  de Requisición";
                        body = "<p>Des-asignación  de Requisición:</p>";
                        body = body + string.Format("<br/>Se comunica de la manera más atenta que el usuario <strong>{0}</strong> te ha desasignado de la vacante <strong>{1}</strong> la cual se encuentra en la requisición FOLIO: <strong><big><a href=\"{3}/login/{2}\">{2}</a></big></strong>.", Usuario, VBr, Folio, sitioWeb);
                        body = body + "<p>Gracias por tu atención. </p> <p>Saludos.</p>";
                    }
                    if (action == "RD")
                    {
                        //m.Subject = "[SAGA] Eliminación de Requisicion";
                        asunto = "[SAGA] Eliminación de Requisicion";
                        body = "<p>Eliminación de Requisición:</p>";
                        body = body + string.Format("<br/>Se comunica de la manera más atenta que la vacante <strong>{0}</strong> la cual se encuentra en la requisición FOLIO: <strong><big><a href=\"{2}/login/{1}\">{1}</a></big></strong>, fue eliminada.", VBr, Folio, sitioWeb);
                        body = body + "<p>Gracias por tu atención. </p> <p>Saludos.</p>";
                    }

                    if (action == "RU")
                    {
                        //m.Subject = "[SAGA] Cancelación de Requisición";
                        asunto = "[SAGA] Cancelación de Requisición";
                        body = "<p>Cancelación de Requisición:</p>";
                        body = body + string.Format("<br/>Se comunica de la manera más atenta que la vacante <strong>{0}</strong> la cual se encuentra en la requisición FOLIO: <strong><big><a href=\"{2}/login/{1}\">{1}</a></big></strong>, fue cancelada.", VBr, Folio, sitioWeb);
                        body = body + "<p>Gracias por tu atención. </p> <p>Saludos.</p>";
                    }

                    body = body + string.Format("<p></p><p><a href=\"https://weberp.damsa.com.mx\"><h4>Link de acceso al ERP </h4></a></p>");

                    body = body.Replace("<", "&lt;");
                    body = body.Replace("\"", "&quot;");
                    var xml = string.Format("<Parametros><Parametro Id_Sistema=\"SISTEMA_DEMO\" De=\"noreply@damsa.com.mx\" "
                                 + "Para=\"{0}\" Copia=\"\"  CopiaOculta=\"{1}\" Asunto=\"{2}\" Msg=\"{3}\"/>"
                                 + "</Parametros>", to, "bmorales@damsa.com.mx, mventura@damsa.com.mx", asunto, body);
        
                    SqlParameter[] Parameters = { new SqlParameter("@ParametrosXML", xml) };
                    db.Database.ExecuteSqlCommand("sp_emailFirmas @ParametrosXML", Parameters);


                    //m.Bcc.Add("mventura@damsa.com.mx");
                    //m.Body = body;
                    //m.IsBodyHtml = true;
                    //SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                    //smtp.EnableSsl = true;
                    //smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                    //smtp.Send(m);
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

                var pass = db.Database.SqlQuery<UpPasswordDto>("dbo.sp_DecryptPassSAGA @id", new SqlParameter("id", dtos.EntidadId)).ToList();

                var aux = db.Usuarios.Where(x => x.Id.Equals(dtos.EntidadId)).Select(f => new
                {
                    fecha = f.fch_Creacion,

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
                body = body + string.Format("<tr bgcolor=\"#E7EBEC\"><td> {0} </td></tr>", pass[0].Password);
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
                throw;
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

        public void SendEmailPeticionLiberar(ProcesoDto dtos)
        {
            try
            {
                string body = "";
                string webERP = ConfigurationManager.AppSettings["WEBERP"].ToString();
                DateTime fechaCreacion = DateTime.Now;

                var email = db.Usuarios.Where(x => x.Id.Equals(dtos.ReclutadorId)).Select(e => e.emails.Select(ee => ee.email).FirstOrDefault()).FirstOrDefault();

                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "SAGA Inn");
                m.To.Add(email);

                m.Bcc.Add("bmorales@damsa.com.mx");

                m.Subject = "[SAGA] Solicitud liberación de candidato";
                body = "<html><body>";
                body = body + string.Format("<p>Por este medio se le informa que {0} gener&oacute; una solicitud de liberaci&oacute;n</p>", dtos.nombre);
                body = body + string.Format("<p>Candidato {0} para iniciar proceso en otra vacante que cumple con el perfil.</p>", dtos.nombreCandidato);
                body = body + "<p>Si no necesita al candidato favor de liberarlo</p><br/>";
                body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
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

        public void SendEmailAignacionRequiPuro(string email, string user, string Folio, string vBtra)
        {
            try
            {
                string body = "";
                string webERP = ConfigurationManager.AppSettings["WEBERP"].ToString();
                DateTime fechaCreacion = DateTime.Now;

                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "SAGA Inn");
                m.To.Add(email);

                m.Subject = "[SAGA] Asignacion de Requisicion " + Folio;

                var inicio = "<html><head><style>td { border: solid #2471A3 1px; padding-left:5px; padding-right:5px;padding-top:1px; padding-bottom:1px;font-size:9pt; color: #3498DB;font-family:'calibri'; width: 25%; text-align: left; vertical-align: top; border-spacing: 0; border-collapse: collapse;} ";
                inicio = inicio + "p { font - family:'calibri'; } th { font - family:'calibri'; width: 25 %; text - align: left; vertical - align: top; border: solid blue 1px; border - spacing: 0; border - collapse: collapse; background: #3498DB; color:white;}";
                inicio = inicio + "h3 { font - family:'calibri'; } table { width: 100 %; }</style></head><body style =\"text-align:center; font-family:'calibri'; font-size:10pt;\"><br><br><p> Asignación de Requisición:</p>";


                body = inicio;
                body = body + string.Format("<br/>El usuario <strong>{0}</strong> te ha asignado para trabajar la vacante <strong>{1}</strong> la cual se encuentra con un folio de requisición: <strong style='background-color:yellow;'><big><a href=\"{3}/login/{2}\">{2}</a></big></strong>.", user, vBtra, Folio, sitioWeb);

                body = body + "<p>Para ver tus requisiciones asignadas ingresa a tu panel de <b>Reclutamiento</b>, selecciona la opción de <b>Vacantes</b> para dar el seguimiento correspondiente.</p> ";
                body = body + "<p>Podrás acceder mediante la siguiente dirección: https://weberp.damsa.com.mx/login" + "</p>";
                body = body + "<p>Quedamos a tus órdenes para cualquier relativo al correo inntec@damsa.com.mx</p><p>Gracias por tu atención. </p><p>Saludos</p>";
                body = body + "</body></html>";

                m.Body = body;
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                smtp.Send(m);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool SendEmailRequisPuras(Guid RequisicionId)
        {
            try
            {
                string GrVtasEmail = "", GVtasEmail = "", GGEmail = "";
                List<string> GrVEmails = null;
                List<string> GVEmails = null;

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
                        ajustes = db.ComentariosVacantes.Where(xx => xx.RequisicionId.Equals(RequisicionId) && xx.MotivoId.Equals(19)).OrderByDescending(o => o.fch_Creacion).Select(c => c.Comentario).FirstOrDefault()
                    }).FirstOrDefault();

                var facturacion = db.FacturacionPuro.Where(f => f.RequisicionId.Equals(RequisicionId))
                    .Select(f => new
                    {
                        porcentaje = f.Porcentaje,
                        monto = f.Monto,
                        perContratado = f.PerContratado,
                        montoContratado = f.MontoContratado,
                    }).FirstOrDefault();

                GrVtasEmail = db.Usuarios.Where(u => u.TipoUsuarioId.Equals(12) && u.Departamento.Clave.Equals("VTAS")
                    && u.Sucursal.estadoId.Equals(requi.estadoId) && u.Activo.Equals(true))
                         .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                         .FirstOrDefault();
                GVtasEmail = db.Usuarios
                        .Where(u => u.TipoUsuarioId.Equals(3) && u.Departamento.Clave.Equals("VTAS") 
                        && u.Sucursal.estadoId.Equals(requi.estadoId) && u.Activo.Equals(true))
                        .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                        .FirstOrDefault();
        
                    if (requi.estatusId == 46)
                    {
                        GGEmail = db.Usuarios
                            .Where(u => u.TipoUsuarioId.Equals(14) && u.Departamento.Clave.Equals("GRTS") && u.Activo.Equals(true))
                            .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                            .FirstOrDefault();
                    }

                var coorEmail = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(RequisicionId)).Select(r =>
                   db.Usuarios.Where(x => x.Id.Equals(r.GrpUsrId)).Select(ee => ee.emails.Select(eee => eee.email).FirstOrDefault()).FirstOrDefault()
                  ).FirstOrDefault();

                var emailProp = db.Emails.Where(x => x.EntidadId.Equals(requi.propietarioid)).Select(x => x.email).FirstOrDefault();

                    string body = "";
                    string from = "noreply@damsa.com.mx";
                    MailMessage m = new MailMessage();
                    m.Priority = MailPriority.High;
                    m.From = new MailAddress(from, "SAGA Inn");
                    body = string.Format("<html><head></head> <body style=\"text-align:justify; font-size:14px; font-family:'calibri'\"><div style =\"margin-left: 5px\">");
                    var auxEstatusId = (requi.estatusId == 34 || requi.estatusId == 35 || requi.estatusId == 36) ? 34 : requi.estatusId;
                    switch (auxEstatusId)
                    {
                        case 4:
                            m.To.Add(ConfigurationManager.AppSettings["FacturacionEmail"].ToString());

                        m.CC.Add(GrVtasEmail != null ? GrVtasEmail : emailProp);
                        m.CC.Add(GVtasEmail != null ? GVtasEmail : emailProp);
                           
                        m.CC.Add(emailProp);
                        m.Subject = string.Format("[SAGA] FACTURAR FOLIO Solicitud de Facturación de Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                        body = body + string.Format("<strong style=\"color: #159EF7\">Por este medio se les informa, que se requiere factura para el nuevo Reclutamiento Puro con el número de folio <a href=\"{0}/login/{1}\">{1}</a>.</strong>", sitioWeb, requi.folio);

                        SendEmailAignacionRequiPuro(coorEmail, requi.solicita.nombre.ToUpper(), requi.folio.ToString(), requi.puesto);
                        break;
                  case 8:
                        m.To.Add(emailProp);
                         
                        m.CC.Add(GrVtasEmail != null ? GrVtasEmail : emailProp);
                        if(coorEmail.Length > 0)
                        {
                            m.CC.Add(coorEmail);

                        }
                            
                            if (facturada)
                            {
                                m.CC.Add(ConfigurationManager.AppSettings["FacturacionEmail"].ToString());
                                m.Bcc.Add(ConfigurationManager.AppSettings["FacturacionEmail2"].ToString());
                                m.Bcc.Add(ConfigurationManager.AppSettings["FacturacionEmail3"].ToString());
                            }
                            m.Subject = string.Format("[SAGA] Cancelación de Vacante con Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                            body = body + string.Format("<strong style=\"color: #159EF7\">Por este medio se les informa que se ha cancelado el Reclutamiento Puro con el número de folio <a href=\"{1}/login/{0}\">{0}</a></strong>", requi.folio, sitioWeb);
                            break;
                        case 9:
                            m.To.Add(emailProp);
                          
                            m.CC.Add(GrVtasEmail != null ? GrVtasEmail : emailProp);

                            if (coorEmail.Length > 0)
                            {
                                m.CC.Add(coorEmail);

                            }
                        if (facturada)
                            {
                                m.To.Add(ConfigurationManager.AppSettings["FacturacionEmail"].ToString());
                                m.Bcc.Add(ConfigurationManager.AppSettings["FacturacionEmail2"].ToString());
                                m.Bcc.Add(ConfigurationManager.AppSettings["FacturacionEmail3"].ToString());
                            }
                            m.Subject = string.Format("[SAGA] Eliminación de Vacante con Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                            body = body + string.Format("<strong style=\"color: #159EF7\">Por este medio se les informa que se ha elimino el Reclutamiento Puro con el número de folio <a href=\"{1}/login/{0}\">{0}</a>.</strong>", requi.folio, sitioWeb);
                            m.To.Add(emailProp);
                            break;
                        case 43:
                            m.To.Add(emailProp);
                           m.CC.Add(GrVtasEmail != null ? GrVtasEmail : emailProp);
                               
              
                            m.Subject = string.Format("[SAGA] AUTORIZAR FOLIO Nueva Vacante con Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                            body = body + string.Format("<strong style=\"color: #159EF7\">Por este medio se les informa que existe un Nuevo Reclutamiento Puro con el número de folio <a href=\"{0}/login/{1}\">{1}</a>.</strong>", sitioWeb, requi.folio);
                            break;
                        case 34:
                            m.To.Add(ConfigurationManager.AppSettings["FacturacionEmail"].ToString());
                           
                            m.CC.Add(GrVtasEmail != null ? GrVtasEmail : emailProp);
                        m.CC.Add(emailProp);
                         
                            m.Subject = string.Format("[SAGA] Vacante con Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                            body = body + string.Format("<strong>Por este medio se les informa que se requiere facturar el cierre de reclutamiento puro con el n&uacute;mero de folio {0} </strong>", requi.folio);
                            body = body + "<p><h3>INFORMACIÓN DE AJUSTE.</h3></p>";
                            body = body + string.Format("<p>{0}</p>", requi.ajustes);
                            break;
                        case 45:
                       
                            m.CC.Add(GrVtasEmail != null ? GrVtasEmail : emailProp);
                              
                            m.To.Add(emailProp);
                            m.Subject = string.Format("[SAGA] AUTORIZADA PENDIENTE PAGO Seguimiento de Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                            body = body + string.Format("<strong style=\"color: #159EF7\">La requisiciones esta autorizada, con un pago pendiente.</strong>");
                            break;
                        case 46:
                            m.To.Add(GGEmail);
                            
                                m.CC.Add(GrVtasEmail != null ? GrVtasEmail : emailProp);
                          
                            m.CC.Add(emailProp);
                            m.Subject = string.Format("[SAGA] AUTORIZAR FOLIO Vacante con Reclutamiento Puro Porcentaje menor de 50% {0} - {1}", requi.folio, requi.empresa.ToUpper());
                            body = body + string.Format("<strong style=\"color: #159EF7\">Por este medio se les informa que existe un Reclutamiento Puro con el número de folio <a href=\"{0}/login/{1}\">{1}</a>, el cual se esta solicitando una facturación por debajo del 50%. Es necesaria previa autorización para continuar con el proceso. </strong>", sitioWeb, requi.folio);
                            break;
                    }
                    
                    if (facturacion != null)
                    {
                        body = body + string.Format("<p><h3>INFORMACIÓN PRINCIPAL DE FACTURACIÓN.</h3></p> ");
                        body = body + string.Format("<div style=\"background-color: #FFFAD6; width: max-content; margin-left: 15px;\"><div style=\"padding: 15px 20px 15px\"> ");
                        body = body + string.Format("<p><label><strong style=\"color: #159EF7\">FACTURAR: </strong>{0}%</label></p>", facturacion.porcentaje);
                        body = body + string.Format("<p><label><strong style=\"color: #159EF7\"> MONTO A FACTURAR </strong>{0}</label><p>", String.Format("{0:C}", facturacion.monto));
                        body = body + string.Format("<p><label><strong style=\"color: #159EF7\"> AL CUBRIR: </strong>{0} %</label><p>", facturacion.perContratado);
                        body = body + string.Format("<p><label><strong style=\"color: #159EF7\"> MONTO AL CUBRIR </strong>{0}</label><p>", String.Format("{0:C}", facturacion.montoContratado));
                        body = body + string.Format("</div></div>");
                    }

                    body = body + string.Format("<p><h3>INFORMACIÓN DE REQUISICIÓN</h3></p> ");
                    body = body + string.Format("<div style=\"background-color: #FFFAD6; width: max-content; margin-left: 15px;\"><div style=\"padding: 15px 20px 15px\"> ");
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\">FECHA SOLICITUD: </strong>{0}</label><p>", requi.fch_Creacion);
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\">SOLICITANTE: </strong>{0}</label></p>", requi.solicita.nombre.ToUpper());
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\">EMPRESA: </strong>{0}</label></p>", requi.empresa.ToUpper());
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\">ESTADO: </strong>{0}</label></p>", requi.estado.ToUpper());
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\">NÚMERO VACANTES: </strong>{0}</label></p>", requi.noVacantes);
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\">PUESTO: </strong>{0}</label></p>", requi.puesto.ToUpper());
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\">SUELDO: </strong>{0} a {1}</label></p>", String.Format("{0:C}", requi.sueldoMinimo), String.Format("{0:C}", requi.sueldoMaximo));
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\">ESTATUS VACANTE: </strong>{0}</label></p>", requi.estaus);
                    body = body + string.Format("</div></div>");
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\"> Favor de corroborar esta información y dar el seguimiento correspondiente </strong></label></p>");
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\">Me despido de usted(es) agradeciendo su atención y enviándole un cordial saludo. </strong></label></p>");
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
                Emails = db.Emails.Where(x => x.EntidadId.Equals(requi.propietarioid) || x.EntidadId.Equals(requi.aprobadorId)).Select(x => x.email).ToList();
                string body = "";
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "SAGA Inn");
                m.To.Add(ConfigurationManager.AppSettings["Medios"].ToString());
                m.To.Add(ConfigurationManager.AppSettings["Medios2"].ToString());
                m.To.Add(ConfigurationManager.AppSettings["Medios3"].ToString());
                m.To.Add(ConfigurationManager.AppSettings["Medios4"].ToString());
                foreach (var e  in Emails){
                    m.CC.Add(e);
                }
                m.Subject = string.Format("Publicacion de Vacante en Redes Sociales {0} - {1}", requi.folio, requi.empresa.ToUpper());
                body = string.Format("<p style=\"font-size:12px;\">Por este medio se les informa que se ha solicitado publicación en redes sociales la vacante con número de folio <a href=\"{0}/login/{1}\">{1}</a></a></strong>:</p>",sitioWeb, requi.folio);

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

        public bool SendEmailRequisPurasHafacturar(Guid RequisicionId)
        {
            try
            {
                int[] mty = { 6, 7, 10, 19, 28, 24 };
                int[] gdl = { 1, 3, 8, 11, 14, 16, 18, 2, 25, 26, 32 };
                int[] mx = { 4, 5, 9, 12, 13, 15, 17, 20, 21, 22, 23, 27, 29, 30, 31 };
                string GrVtasEmail = "", GVtasEmail = "", GGEmail = "";
                List<string> GrVEmails = null;
                List<string> GVEmails = null;
                bool isDurango = false;


                bool facturada = false;
                int[] estatus = { 44, 45, 46 };
                var estatusRequi = db.EstatusRequisiciones
                    .Where(e => e.RequisicionId.Equals(RequisicionId))
                    .Select(e => e.EstatusId).ToList();

                foreach (var e in estatusRequi)
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
                        porcentaje = f.Porcentaje,
                        monto = f.Monto,
                        perContratado = f.PerContratado,
                        montoContratado = f.MontoContratado,
                    }).FirstOrDefault();

               GrVtasEmail = db.Usuarios
                             .Where(u => u.TipoUsuarioId.Equals(12) && u.Departamento.Clave.Equals("VTAS") && 
                             u.Sucursal.estadoId.Equals(requi.estadoId) && u.Activo.Equals(true))
                             .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                             .FirstOrDefault();
                GVtasEmail = db.Usuarios
                    .Where(u => u.TipoUsuarioId.Equals(3) && u.Departamento.Clave.Equals("VTAS") && 
                    u.Sucursal.estadoId.Equals(1) && u.Activo.Equals(true))
                    .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                    .FirstOrDefault();
            
                if (requi.estatusId == 46)
                {
                    GGEmail = db.Usuarios
                        .Where(u => u.TipoUsuarioId.Equals(14) && u.Departamento.Clave.Equals("GRTS") && u.Activo.Equals(true))
                        .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                        .FirstOrDefault();
                }


                var emailProp = db.Emails.Where(x => x.EntidadId.Equals(requi.propietarioid)).Select(x => x.email).FirstOrDefault();

                string body = "";
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.Priority = MailPriority.High;
                m.From = new MailAddress(from, "SAGA Inn");
                body = string.Format("<html><head></head> <body style=\"text-align:justify; font-size:14px; font-family:'calibri'\"><div style =\"margin-left: 5px\">");

                m.To.Add(ConfigurationManager.AppSettings["FacturacionEmail"].ToString());
                m.Bcc.Add(ConfigurationManager.AppSettings["FacturacionEmail2"].ToString());
                m.Bcc.Add(ConfigurationManager.AppSettings["FacturacionEmail3"].ToString());
             
                m.CC.Add(GrVtasEmail != null ? GrVtasEmail : emailProp);
                m.CC.Add(GVtasEmail != null ? GVtasEmail : emailProp);
              
                m.CC.Add(emailProp);
                m.Subject = string.Format("[FACTURAR FOLIO] Solicitud de Facturación de Reclutamiento Puro {0} - {1}", requi.folio, requi.empresa.ToUpper());
                body = body + string.Format("<strong style=\"color: #159EF7\">Por este medio se les informa, que se requiere factura para el nuevo Reclutamiento Puro con el número de folio <a href=\"{0}/login/{1}\">{1}</a>.</strong>", sitioWeb, requi.folio);

                if (facturacion != null)
                {
                    body = body + string.Format("<p><h3>INFORMACIÓN PRINCIPAL DE FACTURACIÓN.</h3></p> ");
                    body = body + string.Format("<div style=\"background-color: #FFFAD6; width: max-content; margin-left: 15px;\"><div style=\"padding: 15px 20px 15px\"> ");
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\">FACTURAR: </strong>{0}%</label></p>", facturacion.porcentaje);
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\"> MONTO A FACTURAR </strong>{0}</label><p>", String.Format("{0:C}", facturacion.monto));
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\"> AL CUBRIR: </strong>{0} %</label><p>", facturacion.perContratado);
                    body = body + string.Format("<p><label><strong style=\"color: #159EF7\"> MONTO AL CUBRIR </strong>{0}</label><p>", String.Format("{0:C}", facturacion.montoContratado));
                    body = body + string.Format("</div></div>");
                }

                body = body + string.Format("<p><h3>INFORMACIÓN DE REQUISICIÓN</h3></p> ");
                body = body + string.Format("<div style=\"background-color: #FFFAD6; width: max-content; margin-left: 15px;\"><div style=\"padding: 15px 20px 15px\"> ");
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">FECHA SOLICITUD: </strong>{0}</label><p>", requi.fch_Creacion);
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">SOLICITANTE: </strong>{0}</label></p>", requi.solicita.nombre.ToUpper());
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">EMPRESA: </strong>{0}</label></p>", requi.empresa.ToUpper());
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">ESTADO: </strong>{0}</label></p>", requi.estado.ToUpper());
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">NÚMERO VACANTES: </strong>{0}</label></p>", requi.noVacantes);
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">PUESTO: </strong>{0}</label></p>", requi.puesto.ToUpper());
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">SUELDO: </strong>{0} a {1}</label></p>", String.Format("{0:C}", requi.sueldoMinimo), String.Format("{0:C}", requi.sueldoMaximo));
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">ESTATUS VACANTE: </strong>{0}</label></p>", requi.estaus);
                body = body + string.Format("</div></div>");
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\"> Favor de corroborar esta información y dar el seguimiento correspondiente </strong></label></p>");
                body = body + string.Format("<p><label><strong style=\"color: #159EF7\">Me despido de usted(es) agradeciendo su atención y enviándole un cordial saludo. </strong></label></p>");
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
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }
        }
    }
}