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
using System.Globalization;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/reclutamiento/SeguimientoVacante")]
    public class FoliosIncidenciasController : ApiController
    {
        private SAGADBContext db;
        public Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");
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

        public IHttpActionResult TransferRequiReclutador(Guid requi, Guid reclutadorId, Guid reclutadorId2, Guid usuario )
        {
            try
            {
                RastreabilidadMes RM = new RastreabilidadMes();
                Transferencias Transf = new Transferencias();

                var folio = db.Requisiciones.Where(x => x.Id.Equals(requi)).Select(F => F.Folio).FirstOrDefault();
                var datos = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(requi) && x.ReclutadorId.Equals(reclutadorId)).Select(r => new
                {
                    id = r.Id,
                    candidato = db.Usuarios.Where(x => x.Id.Equals(r.CandidatoId)).Select(U => U.Nombre + " " + U.ApellidoPaterno + " " + U.ApellidoMaterno).FirstOrDefault(),
                    candidatoId = r.CandidatoId
                }).ToList();

                var aux = db.TrazabilidadesMes.Select(ss => new { id = ss.Id, folio = ss.Folio.ToString() }).ToList();
                var tmId = aux.Where(x => x.folio == folio.ToString()).Select(ID => new { id = ID.id }).ToList();

               var trans = db.Database.BeginTransaction();

                try
                {
                    Transf.antId = reclutadorId;
                    Transf.actId = reclutadorId2;
                    Transf.requisicionId = requi;
                    Transf.tipoTransferenciaId = 2;
                    Transf.fch_Modificacion = DateTime.Now;

                    db.Transferencias.Add(Transf);
                    db.SaveChanges();

                    RM.TrazabilidadMesId = tmId[0].id;
                    RM.TipoAccionId = 3;
                    RM.UsuarioMod = db.Usuarios.Where(x => x.Id.Equals(usuario)).Select(U => U.Usuario).FirstOrDefault();
                    RM.Descripcion = "Actualizacion (UPDATE)";
                    db.RastreabilidadMes.Add(RM);
                    db.SaveChanges();

                 
                    foreach (var p in datos) //nuevo reclutador
                    {
                        var PC = db.ProcesoCandidatos.Find(p.id);

                        db.Entry(PC).Property(r => r.ReclutadorId).IsModified = true;
                        PC.ReclutadorId = reclutadorId2;

                        db.SaveChanges();
                    }

                    var A = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi) && x.GrpUsrId.Equals(reclutadorId)).Select(ID => ID.Id).FirstOrDefault(); //elimino reclutador
                    if (A != null)
                    {
                        var AID = db.AsignacionRequis.Find(A);
                        db.Entry(AID).State = EntityState.Deleted;

                        db.SaveChanges();
                    }

                    var B = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi) && x.GrpUsrId.Equals(reclutadorId2)).Select(ID => ID.Id).FirstOrDefault(); //nuevo reclutador
                    if (B == auxID)
                    {
                        AsignacionRequi AR = new AsignacionRequi();
                        AR.RequisicionId = requi;
                        AR.GrpUsrId = reclutadorId2;
                        AR.UsuarioMod = db.Usuarios.Where(x => x.Id.Equals(usuario)).Select(U => U.Usuario).FirstOrDefault();
                        AR.UsuarioAlta = db.Usuarios.Where(x => x.Id.Equals(usuario)).Select(U => U.Usuario).FirstOrDefault();

                        db.AsignacionRequis.Add(AR);
                        db.SaveChanges();
                    }

                    trans.Commit();

                    var descripcion = "Se realizó una transferencia del usuario " + db.Usuarios.Where(x => x.Id.Equals(reclutadorId)).Select(U => U.Nombre + " " + U.ApellidoPaterno + " " + U.ApellidoMaterno).FirstOrDefault() + " a " + db.Usuarios.Where(x => x.Id.Equals(reclutadorId2)).Select(U => U.Nombre + " " + U.ApellidoPaterno + " " + U.ApellidoMaterno).FirstOrDefault() + " se transfirieron " + datos.Count() + " candidatos en proceso";
                    this.EnviarEmailTransfer(requi, usuario, descripcion, reclutadorId, reclutadorId2);

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }

        public IHttpActionResult TransferRequi(Guid requi, Guid coorId, Guid usuario, int tipo)
        {
            try
            {
                RastreabilidadMes RM = new RastreabilidadMes();
                Transferencias Transf = new Transferencias();
                RequisicionesController rc = new RequisicionesController();
                
                string descripcion = "";
                Guid usuarioAux;
                var datos = db.Requisiciones.Where(x => x.Id.Equals(requi)).Select(r => new
                {
                    folio = r.Folio,
                    aprobadorId = r.AprobadorId,
                    coordinador = db.Usuarios.Where(x => x.Id.Equals(r.AprobadorId)).Select( U => U.Nombre + " " + U.ApellidoPaterno + " " + U.ApellidoMaterno).FirstOrDefault(),
                    propietarioId = r.PropietarioId,
                    solicitante = db.Usuarios.Where(x => x.Id.Equals(r.PropietarioId)).Select(U => U.Nombre + " " + U.ApellidoPaterno + " " + U.ApellidoMaterno).FirstOrDefault(),
                    tipoReclutamientoId =r.TipoReclutamientoId,
                    estatusId = r.EstatusId,
                    VBtra = r.VBtra
                }).ToList();

                var aux = db.TrazabilidadesMes.Select(ss => new { id = ss.Id, folio = ss.Folio.ToString() }).ToList();
                var tmId = aux.Where(x => x.folio == datos[0].folio.ToString()).Select(ID => new { id = ID.id }).ToList();

                if (tipo == 1) //cambio coord
                {
                    usuarioAux = datos[0].aprobadorId;
                    descripcion = "Se realizó una transferencia del usuario " + datos[0].coordinador + " a " + db.Usuarios.Where(x => x.Id.Equals(coorId)).Select(U => U.Nombre + " " + U.ApellidoPaterno + " " + U.ApellidoMaterno).FirstOrDefault();
                }
                else //cambio propietario
                {
                    usuarioAux = datos[0].propietarioId;
                    descripcion = "Se realizó una transferencia del usuario " + datos[0].solicitante + " a " + db.Usuarios.Where(x => x.Id.Equals(coorId)).Select(U => U.Nombre + " " + U.ApellidoPaterno + " " + U.ApellidoMaterno).FirstOrDefault();
                }
                
                var trans = db.Database.BeginTransaction();

                try
                {
                    Transf.antId = usuarioAux;
                    Transf.actId = coorId;
                    Transf.requisicionId = requi;
                    Transf.tipoTransferenciaId = tipo;
                    Transf.fch_Modificacion = DateTime.Now;

                    db.Transferencias.Add(Transf);
                    db.SaveChanges();

                    RM.TrazabilidadMesId = tmId[0].id;
                    RM.TipoAccionId = 3;
                    RM.UsuarioMod = db.Usuarios.Where(x => x.Id.Equals(usuario)).Select(U => U.Usuario).FirstOrDefault();
                    RM.Descripcion = "Actualizacion (UPDATE)";
                    db.RastreabilidadMes.Add(RM);
                    db.SaveChanges();

                    var R = db.Requisiciones.Find(requi);
                    if (tipo == 1)
                    {
                        if (datos[0].estatusId == 4 && datos[0].tipoReclutamientoId == 1)
                        {
                            List<AsignacionRequi> ar = new List<AsignacionRequi>();
                            var asg = new AsignacionRequi
                            {
                                RequisicionId = requi,
                                GrpUsrId = coorId,
                                CRUD = "",
                                UsuarioAlta = RM.UsuarioMod,
                                UsuarioMod = RM.UsuarioMod,
                                fch_Modificacion = DateTime.Now
                            };
                            ar.Add(asg);
                            rc.AlterAsignacionRequi(ar,requi, datos[0].folio, asg.UsuarioAlta, datos[0].VBtra, null);
                        }
                        else
                        {
                            db.Entry(R).Property(r => r.AprobadorId).IsModified = true;
                            R.AprobadorId = coorId;
                            db.SaveChanges();
                        }
                        
                    }
                    else
                    {
                        db.Entry(R).Property(r => r.PropietarioId).IsModified = true;
                        R.PropietarioId = coorId;
                        db.SaveChanges();
                    }
                    trans.Commit();

                    this.EnviarEmailTransfer(requi, usuario, descripcion, usuarioAux, coorId);

                }
                catch(Exception ex)
                {
                    trans.Rollback();
                }
               
                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
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
                    coordinador = p.AprobadorId, 
                    solicitante = p.PropietarioId,
                    folio = p.Folio, 
                    vbtra = p.VBtra
                }).FirstOrDefault();
                var emailCoord = db.Emails.Where(x => x.EntidadId.Equals(propietario.coordinador)).Select(e => e.email).FirstOrDefault();
                var emailSolicitante = db.Emails.Where(x => x.EntidadId.Equals(propietario.solicitante)).Select(e => e.email).FirstOrDefault();
                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi)).Select(A => new
                {
                    emails = db.Emails.Where(e => e.EntidadId.Equals(A.GrpUsrId)).Select(ee => ee.email).FirstOrDefault()
                }).ToList();

                var usuario = db.Usuarios.Where(x => x.Id.Equals(reclutador)).Select(n => new
                {
                    nombre = n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno,
                    email = n.emails.Select(ee => ee.email).FirstOrDefault()
                }).FirstOrDefault();


                //email = "bmorales@damsa.com.mx";
                string body = "";
               // email = "idelatorre@damsa.com.mx";
                if (emailSolicitante != "")
                {
                    string from = "noreply@damsa.com.mx";
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "SAGA Inn");
                    m.Subject = "Solicitud vacante en pausa Requisición, " + propietario.folio;

                    m.To.Add(emailCoord);
                    m.Bcc.Add(emailSolicitante);
                    foreach (var e in asignados)
                    {
                        m.Bcc.Add(e.emails.ToString());
                    }

                    body = "<html><head></head>";
                    body = body + "<body style=\"text-align:justify; font-size:14px; font-family:'calibri'\">";
                    body = body + string.Format("<p>Se comunica que el usuario <strong>{0}</strong>, levant&oacute; una solicitud de vacante \"en pausa\", Vacante <strong>{1}</strong> la cual se encuentra con un folio de requisici&oacute;n: <strong>{2}</strong></p>", usuario.nombre, propietario.vbtra, propietario.folio );
                    body = body + "<p>Para validar la solicitud ser&aacute; necesario ingresar a Reclutamiento, selecciona la opci&oacute;n Vacantes, dar clic en el bot&oacute;n Visualizar Vacantes en Pausa, para dar el seguimiento correspondiente.</p>";
                    body = body + "<br/><p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                    body = body + "<br/><p></p><p><a href=\"https://weberp.damsa.com.mx\"><h4>Link de acceso al ERP </h4></a></p>";
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
        public IHttpActionResult EnviarEmail2(int estatus, Guid requi, Guid reclutador)
        {

            FolioIncidencia obj = new FolioIncidencia();
            int[] motivosId = new int[] { 14, 15, 16 };
            //envia el email cuando se des pausa
            try
            {
                var propietario = db.Requisiciones.Where(x => x.Id.Equals(requi)).Select(p => new {
                    coordinador = p.AprobadorId,
                    solicitante = p.PropietarioId,
                    folio = p.Folio,
                    vbtra = p.VBtra, 
                    comentario = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(p.Id) && motivosId.Contains(x.MotivoId)).OrderByDescending(o => o.fch_Creacion).Select(c => 
                        db.ComentariosVacantes.Where(x => x.RespuestaId.Equals(c.Id)).OrderByDescending(oo => oo.fch_Creacion).Select(cc => cc.Comentario).FirstOrDefault()).FirstOrDefault(),
                    estatus = p.Estatus.Descripcion
                }).FirstOrDefault();

                var emailCoord = db.Emails.Where(x => x.EntidadId.Equals(propietario.coordinador)).Select(e => e.email).FirstOrDefault();
                var emailSolicitante = db.Emails.Where(x => x.EntidadId.Equals(propietario.solicitante)).Select(e => e.email).FirstOrDefault();
                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi)).Select(A => new
                {
                    emails = db.Emails.Where(e => e.EntidadId.Equals(A.GrpUsrId)).Select(ee => ee.email).FirstOrDefault()
                }).ToList();

                var usuario = db.Usuarios.Where(x => x.Id.Equals(reclutador)).Select(n => new
                {
                    nombre = n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno,
                    email = n.emails.Select(ee => ee.email).FirstOrDefault()
                }).FirstOrDefault();


                //email = "bmorales@damsa.com.mx";
                string body = "";
                // email = "idelatorre@damsa.com.mx";
                if (emailSolicitante != "")
                {
                    string from = "noreply@damsa.com.mx";
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "SAGA Inn");
                    m.Subject = "Solicitud vacante en pausa Requisición, " + propietario.folio;

                    m.To.Add(emailCoord);
                    m.Bcc.Add(emailSolicitante);
                    foreach (var e in asignados)
                    {
                        m.Bcc.Add(e.emails.ToString());
                    }

                    body = "<html><head></head>";
                    body = body + "<body style=\"text-align:justify; font-size:14px; font-family:'calibri'\">";
                    body = body + string.Format("<p>Se comunica que el usuario/coordinador <strong>{0}</strong>, dio seguimiento al reporte solicitado con la siguiente informacion:</p>", usuario.nombre); 
                    body = body + string.Format("<br/><p>Folio <strong>{0}</strong> Vacante <strong>{1}</strong> Comentario <strong>{2}.</strong> Estatus <strong>{3}.</strong></p>", propietario.folio, propietario.vbtra, propietario.comentario, propietario.estatus);
               
                    body = body + "<br/><p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                    body = body + "<br/><p></p><p><a href=\"https://weberp.damsa.com.mx\"><h4>Link de acceso al ERP </h4></a></p>";
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
                var emailSolicitante = "idelatorre@damsa.com.mx";
                var aprovadorEmail = "idelatorre@damsa.com.mx";
                var folio = "000000000000";
                var vbtra = "";

                var propietario = db.Requisiciones.Where(x => x.Id.Equals(requi)).Select(p => new {
                    propietario = p.PropietarioId,
                    aprobador = p.AprobadorId,
                    folio = p.Folio.ToString(),
                    vbtra = p.VBtra,
                }).FirstOrDefault();
                
                if(propietario != null)
                {
                    emailSolicitante = db.Emails.Where(x => x.EntidadId.Equals(propietario.propietario)).Select(e => e.email).FirstOrDefault();
                    aprovadorEmail = db.Emails.Where(x => x.EntidadId.Equals(propietario.aprobador)).Select(e => e.email).FirstOrDefault();

                    folio = propietario.folio;
                    vbtra = propietario.vbtra;
                }

                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi) && !x.GrpUsrId.Equals(propietario.aprobador)).Select(A => new
                {
                    emails = db.Emails.Where(e => e.EntidadId.Equals(A.GrpUsrId)).Select(ee => ee.email).FirstOrDefault()
                }).ToList();

                var usuario = db.Usuarios.Where(x => x.Id.Equals(reclutador)).Select(n => new {
                    nombre = n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno,
                    email = n.emails.Select(e => e.email).FirstOrDefault()
                }).FirstOrDefault();

                var candidato = db.Candidatos.Where(x => x.Id.Equals(candidatoId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault();
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
                
                if (emailSolicitante != "")
                {
                    string from = "noreply@damsa.com.mx";
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "SAGA Inn");
                    m.Subject = "Reporte posible NR en Requisición, " + folio;

                    m.To.Add(emailSolicitante);
                    m.Bcc.Add(usuario.email.ToString());
                    m.Bcc.Add(aprovadorEmail);
                    m.Bcc.Add("bmorales@damsa.com.mx");
                    foreach (var e in asignados)
                    {
                        m.Bcc.Add(e.emails.ToString());
                    }
                    //usuario, candidato, motivo, vbtra, folio
                    body = "<html><head></head>";
                    body = body + "<body style=\"text-align:justify; font-size:14px; font-family:'calibri'\">";
                    body = body + string.Format("<label>Informaci&oacute;n de candidato en Posible NR</label><p> Se comunica que el usuario / reclutador {0}, report&oacute; un incidente al siguiente candidato: </p>", usuario.nombre);
                    body = body + "<table style=\"width: 75%; background-color: #f1f1c1; border-spacing: 10px;\"><tr><th>Folio:</th><th>Vacante</th><th>Candidato</th><th>Motivo</th><th>Comentario</th></tr>";
                    body = body + string.Format("<tr><td style=\"color:green; text-align: center;\">{0}</td><td style=\"text-align: center;\">{1}</td><td style=\"text-align: center;\">{2}</td><td style=\"color:red; text-align: center;\">{3}</td><td style=\"text-align: center;\">{4}</td></tr></table>", folio, vbtra, candidato, motivo, comentario);
                    body = body + "<p>Para validar y dar seguimiento a la informaci&oacute;n reportada ser&aacute; necesario ingresar a:<p/> ";
                    body = body + "<ol><li>Secci&oacute;n de Reclutamiento.</li><li>Posteriormente en Vacantes.</li><li>Dar clic en bot&oacute;n Incidente, en la parte superior.</li><li>Identificar Candidato y agregar comentario de resultado.</li><li>Aceptar o Rechazar la solicitud.</li></ol>";
                    body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                    body = body + "<br/><p></p><p><a href=\"https://weberp.damsa.com.mx\"><h4>Link de acceso al ERP </h4></a></p>";
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
        public IHttpActionResult EnviarEmailNR2(ComentariosEntrevistaDto datos)
        {

            FolioIncidencia obj = new FolioIncidencia();

            try
            {
                var emailSolicitante = "idelatorre@damsa.com.mx";
                var aprovadorEmail = "idelatorre@damsa.com.mx";
                var folio = "000000000000";
                var vbtra = "";
                var propietario = db.Requisiciones.Where(x => x.Id.Equals(datos.RequisicionId)).Select(p => new {
                    propietario = p.PropietarioId,
                    aprobador = p.AprobadorId,
                    coordinador = db.Usuarios.Where(x => x.Id.Equals(p.AprobadorId)).Select(nom => nom.Nombre + " " + nom.ApellidoPaterno + " " + nom.ApellidoMaterno).FirstOrDefault(),
                    folio = p.Folio.ToString(),
                    vbtra = p.VBtra,
                }).FirstOrDefault();

                if (propietario != null)
                {
                    emailSolicitante = db.Emails.Where(x => x.EntidadId.Equals(propietario.propietario)).Select(e => e.email).FirstOrDefault();
                    aprovadorEmail = db.Emails.Where(x => x.EntidadId.Equals(propietario.aprobador)).Select(e => e.email).FirstOrDefault();

                    folio = propietario.folio;
                    vbtra = propietario.vbtra;
                }

                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(datos.RequisicionId) && !x.GrpUsrId.Equals(propietario.aprobador)).Select(A => new
                {
                    emails = db.Emails.Where(e => e.EntidadId.Equals(A.GrpUsrId)).Select(ee => ee.email).FirstOrDefault()
                }).ToList();

                //var usuario = db.Usuarios.Where(x => x.Id.Equals(datos)).Select(n => new {
                //    nombre = n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno,
                //    email = n.emails.Select(e => e.email).FirstOrDefault()
                //}).FirstOrDefault();

                var candidato = db.Candidatos.Where(x => x.Id.Equals(datos.CandidatoId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault();
                var motivo = db.ComentariosEntrevistas.OrderByDescending(x => x.fch_Creacion)
                    .Where(x => x.CandidatoId.Equals(datos.CandidatoId)
                                & x.RequisicionId.Equals(datos.RequisicionId)
                                & x.Motivo.EstatusId == 28)
                    .Select(m => m.Motivo.Descripcion).FirstOrDefault();

                var comentario = db.ComentariosEntrevistas.OrderByDescending(x => x.fch_Creacion)
                    .Where(x => x.CandidatoId.Equals(datos.CandidatoId)
                                & x.RequisicionId.Equals(datos.RequisicionId)
                                & x.Motivo.EstatusId == 28
                                & !x.RespuestaId.Equals(datos.RespuestaId)).OrderByDescending( o=> o.fch_Creacion)
                    .Select(m => m.Comentario).FirstOrDefault();

                //tengo que sacar el que le corresponde por estatus
                string body = "";

                if (emailSolicitante != "")
                {
                    string from = "noreply@damsa.com.mx";
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "SAGA Inn");
                    m.Subject = "Seguimiento posible NR Requisición, " + folio;

                    m.To.Add(emailSolicitante);
                    m.Bcc.Add(aprovadorEmail);

                    foreach (var e in asignados)
                    {
                        m.Bcc.Add(e.emails.ToString());
                    }
                    //usuario, candidato, motivo, vbtra, folio
                    body = "<html><head></head>";
                    body = body + "<body style=\"text-align:justify; font-size:14px; font-family:'calibri'\">";
                    body = body + string.Format("<label>Informaci&oacute;n de candidato Posible NR</label><p> Se comunica que el usuario / coordinador {0}, dio seguimiento al reporte solicitado con la siguiente informacion: </p>", propietario.coordinador);
                    body = body + "<table style=\"width: 75%; background-color: #f1f1c1; border-spacing: 10px;\"><tr><th>Folio:</th><th>Vacante</th><th>Candidato</th><th>Motivo</th><th>Comentario</th><th>Resultado</th></tr>";
                    body = body + string.Format("<tr><td style=\"color:green; text-align: center;\">{0}</td><td style=\"text-align: center;\">{1}</td><td style=\"text-align: center;\">{2}</td><td style=\"color:red; text-align: center;\">{3}</td><td style=\"text-align: center;\">{4}</td><td style=\"text-align: center;\">{5}</td></tr></table>", folio, vbtra, candidato, motivo, comentario, datos.Comentario);
                    body = body + "<p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                    body = body + "<br/><p></p><p><a href=\"https://weberp.damsa.com.mx\"><h4>Link de acceso al ERP </h4></a></p>";
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
        public IHttpActionResult EnviarEmailTransfer(Guid requi, Guid usuario, string desc, Guid antId, Guid actId)
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
                //var emailSolicitante = db.Emails.Where(x => x.EntidadId.Equals(propietario.solicitante)).Select(e => e.email).FirstOrDefault();
                //var emailAnt = db.Emails.Where(x => x.EntidadId.Equals(antId)).Select(e => e.email).FirstOrDefault();
                //var emailAct= db.Emails.Where(x => x.EntidadId.Equals(actId)).Select(e => e.email).FirstOrDefault();

                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi) && !ids.Distinct().Contains(x.GrpUsrId)).Select(A => new
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
                    string from = "noreply@damsa.com.mx";
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "SAGA Inn");
                    m.Subject = "Transferencia de Requisición";

                    //m.To.Add("idelatorre@damsa.com.mx");
                    m.To.Add(user.email);
                    //m.Bcc.Add(emailAct); //no esta llegando
                    //m.Bcc.Add(emailCoord);
                    //m.Bcc.Add(emailSolicitante);
                    foreach (var e in emails)
                    {
                        m.Bcc.Add(e.ToString());
                    }

                    foreach (var e in asignados)
                    {
                        m.Bcc.Add(e.emails.ToString());
                    }

                    m.Bcc.Add("idelatorre@damsa.com.mx");

                    body = "<html><head></head>";
                    body = body + "<body style=\"text-align:justify; font-size:14px; font-family:'calibri'\">";
                    body = body + string.Format("<p>Se comunica que el usuario <strong>{0}</strong>, realiz&oacute; una transferencia vacante <strong>{1}</strong> la cual se encuentra con un folio de requisici&oacute;n: <strong>{2}</strong></p>", user.nombre, propietario.vbtra, propietario.folio);
                    body = body + string.Format("<p>{0}</p>", desc);
                    body = body + "<br/><p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                    body = body + "<br/><p></p><p><a href=\"https://weberp.damsa.com.mx\"><h4>Link de acceso al ERP </h4></a></p>";
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
