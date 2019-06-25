using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web.Http;
using System.Web.Services.Description;

namespace SAGA.API.Controllers.Component
{
    [RoutePrefix("api/CalendarEvent")]
    //[Authorize]
    public class CalendarEventController : ApiController
    {
        private SAGADBContext db;

        public CalendarEventController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("GetEvent")]
        public IHttpActionResult GetEvent(Guid userId)
        {
            try
            {
                var _event = db.CalendarioEvent
                    .Where(x => x.EntidadId.Equals(userId))
                    .Select(x => new {
                        x.Id,
                        x.EntidadId,
                        x.TipoActividadId,
                        x.TipoActividad.Actividad,
                        x.Title, 
                        x.Message,
                        x.Start,
                        x.End,
                        x.AllDay, 
                        x.backgroundColor,
                        x.borderColor,
                        x.Activo
                    })
                    .ToList()
                    .OrderBy(x => x.Start);

                return Ok(_event);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        [HttpPost]
        [Route("AddEvent")]
        public IHttpActionResult AddEvent(CalendarioEvent _event)
        {
            try
            {
                CalendarioEvent ce = new CalendarioEvent();
                ce.EntidadId = _event.EntidadId;
                ce.Title = _event.Title;
                ce.Message = _event.Message;
                ce.Start = _event.Start;
                ce.End = _event.End;
                ce.AllDay = _event.AllDay;
                ce.backgroundColor = _event.backgroundColor;
                ce.borderColor = _event.borderColor;
                ce.TipoActividadId = _event.TipoActividadId;
                ce.Activo = true;
                db.CalendarioEvent.Add(ce);
                db.SaveChanges();

                var actividad = db.TipoActividadReclutador.Where(x => x.Id.Equals(_event.TipoActividadId)).Select(a => a.Actividad).FirstOrDefault();

                this.AttachmentICS(_event.Start, _event.End, actividad, actividad + " " +_event.Message, "REQUEST", _event.EntidadId, ce.Id);

                return Ok(HttpStatusCode.OK);

            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("UpdateEvent")]
        public IHttpActionResult UpdateEvent(CalendarioEvent _Event)
        {
            try
            {
                var _event = db.CalendarioEvent.Find(_Event.Id);
                db.Entry(_event).State = EntityState.Modified;
                _event.EntidadId = _Event.EntidadId;
                _event.Title = _Event.Title;
                _event.Message = _event.Message;
                _event.Start = TimeZoneInfo.ConvertTimeToUtc(_Event.Start);
                _event.Start = _Event.Start;
                _event.End = _Event.End;
                _event.AllDay = _Event.AllDay;
                _event.backgroundColor = _Event.backgroundColor;
                _event.borderColor = _Event.borderColor;
                _event.TipoActividadId = _Event.TipoActividadId;
                //db.CalendarioEvent.Add(_event);
                db.SaveChanges();

                var actividad = db.TipoActividadReclutador.Where(x => x.Id.Equals(_Event.TipoActividadId)).Select(a => a.Actividad).FirstOrDefault();
                this.AttachmentICS(_Event.Start, _Event.End, actividad, actividad + " ACTUALIZADA", "REQUEST", _Event.EntidadId, _Event.Id);
                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);

            }
        }

        [HttpPost]
        [Route("DeleteEvent")]
        public IHttpActionResult DeleteEvent(CalendarioEvent _Event)
        {
            try
            {
                var r = db.CalendarioEvent.Find(_Event.Id);
                db.Entry(r).State = EntityState.Deleted;
                db.SaveChanges();

                var actividad = db.TipoActividadReclutador.Where(x => x.Id.Equals(_Event.TipoActividadId)).Select(a => a.Actividad).FirstOrDefault();
                this.AttachmentICS(_Event.Start, _Event.End, actividad + " CANCELADA", actividad + " CANCELADA", "CANCEL", _Event.EntidadId, _Event.Id);

                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("CulminarEvent")]
        public IHttpActionResult CulminarEvent(Guid Id)
        {
            try
            {
                var _event = db.CalendarioEvent.Find(Id);
                db.Entry(_event).State = EntityState.Modified;
                _event.Activo = false;
                db.SaveChanges();
                return Ok(HttpStatusCode.OK);
                
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        public IHttpActionResult AttachmentICS(DateTime start, DateTime end, string title, string msg, string method, Guid entidadId, Guid eventId)
        {
            string DateFormat = "yyyyMMddTHHmm00";

            string from = "noreply@damsa.com.mx";
            MailMessage m = new MailMessage();
            m.From = new MailAddress(from, "SAGA INN");
            m.Subject = title;

            var email = db.Usuarios.Where(x => x.Id.Equals(entidadId)).Select(mail => mail.emails.Select(x => x.email).FirstOrDefault()).FirstOrDefault();

            m.To.Add(email);
            //m.Bcc.Add("idelatorre@damsa.com.mx");
            m.Body = msg;

            DateTime s = start.ToUniversalTime();
            DateTime e = end.ToUniversalTime();
            StringBuilder str = new StringBuilder();
            str.AppendLine("BEGIN:VCALENDAR");
            str.AppendLine("PRODID:-//Schedule a Meeting");
            str.AppendLine("VERSION:2.0");
            str.AppendLine("METHOD:" + method);
            str.AppendLine("CREATED:" + DateTime.Now.ToUniversalTime().ToString(DateFormat));
            str.AppendLine("BEGIN:VEVENT");
            str.AppendLine("DTSTART:" + s.ToUniversalTime().ToString(DateFormat));
            str.AppendLine("DTSTAMP:" + DateTime.Now.ToUniversalTime().ToString(DateFormat));
            str.AppendLine(string.Format("DTEND:{0}", e.ToUniversalTime().ToString(DateFormat)));
            //str.AppendLine("LOCATION: " + "DAMSA");
            str.AppendLine(string.Format("UID:{0}", eventId));
            str.AppendLine(string.Format("DESCRIPTION:{0}", m.Body));
            str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", m.Body));
            str.AppendLine(string.Format("SUMMARY:{0}", m.Subject + " " + msg));
            //str.AppendLine(string.Format("ORGANIZER;CN=\"{0}\";MAILTO:{1}", "SAGA INN", m.From.Address));

            //str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", m.To[0].DisplayName, m.To[0].Address));
            if (method == "REQUEST")
            {
                str.AppendLine("BEGIN:VALARM");
                str.AppendLine("TRIGGER:-PT15M");
                str.AppendLine("ACTION:DISPLAY");
                str.AppendLine("DESCRIPTION:Reminder");
                str.AppendLine("END:VALARM");
            }

            str.AppendLine("END:VEVENT");
            str.AppendLine("END:VCALENDAR");

            byte[] byteArray = Encoding.ASCII.GetBytes(str.ToString());
            MemoryStream stream = new MemoryStream(byteArray);

            Attachment attach = new Attachment(stream, "event" + DateTime.Now.ToUniversalTime().ToString(DateFormat) + ".ics");

            m.Attachments.Add(attach);

            System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
            contype.Parameters.Add("method", method);
            //  contype.Parameters.Add("name", "Meeting.ics");
            AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), contype);
            m.AlternateViews.Add(avCal);

            //Now sending a mail with attachment ICS file.      

            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
            smtp.Send(m);

            return Ok();
        }
    }
}
