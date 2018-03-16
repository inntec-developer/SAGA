using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using System.Web.Script.Serialization;


namespace SAGA.API.Controllers
{
    [RoutePrefix("api/dvacante")]
    public class DesignerVacanteController : ApiController
    {
        private SAGADBContext db;
        public DesignerVacanteController()
        {
            db = new SAGADBContext();
        }
        [HttpGet]
        [Route("get")]
        public IHttpActionResult Action()
        {
            var datos = db.Requisiciones.Select(e=> new { e.Id , e.VBtra, e.Experiencia }).ToList();
            
            return Ok(datos);
        }
    }
} 
