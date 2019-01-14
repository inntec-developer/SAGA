using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Services.Description;

namespace SAGA.API.Controllers.Component
{
    [RoutePrefix("api/CalendarEvent")]
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
                ce.Start = _event.Start.AddHours(-6);
                ce.End = _event.End.AddHours(-6);
                ce.AllDay = _event.AllDay;
                ce.backgroundColor = _event.backgroundColor;
                ce.borderColor = _event.borderColor;
                ce.TipoActividadId = _event.TipoActividadId;
                ce.Activo = true;
                db.CalendarioEvent.Add(ce);
                db.SaveChanges();
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
                _event.Start = _Event.Start.AddHours(-6);
                _event.End = _Event.End.AddHours(-6);
                _event.AllDay = _Event.AllDay;
                _event.backgroundColor = _Event.backgroundColor;
                _event.borderColor = _Event.borderColor;
                _event.TipoActividadId = _Event.TipoActividadId;
                //db.CalendarioEvent.Add(_event);
                db.SaveChanges();


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
    }
}
