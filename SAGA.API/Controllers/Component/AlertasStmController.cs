using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Component
{
    [RoutePrefix("api/AlertSTM")]
    public class AlertasStmController : ApiController
    {
        private SAGADBContext _db;

        public AlertasStmController()
        {
            _db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getAlert")]
        public IHttpActionResult GetAlert(Guid Id)
        {
            try
            {
                var alert = _db.AlertasStm
                    .Where(x => x.EntidadId.Equals(Id))
                    .Select(x => new {
                        x.Id,
                        x.Icon,
                        x.Alert,
                        x.Activo,
                        x.Creacion
                    })
                    .ToList().OrderByDescending(x => x.Creacion).Take(10);
                return Ok(alert);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("deleteAlert")]
        public IHttpActionResult DeleteAlert(Guid Id, bool all)
        {
            try
            {
                if (!all)
                {
                    var delete = _db.AlertasStm.Find(Id);
                    _db.Entry(delete).Property(u => u.Activo).IsModified = true;
                    delete.Activo = false;
                    _db.SaveChanges();
                }
                else
                {
                    var delete = _db.AlertasStm
                        .Where(x => x.EntidadId.Equals(Id) && x.Activo.Equals(true))
                        .ToList();
                    foreach (AlertasStm a  in delete)
                    {
                        _db.Entry(a).Property(x => x.Activo).IsModified = true;
                        a.Activo = false;
                    }
                    _db.SaveChanges();
                }
               
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }


    }
}
