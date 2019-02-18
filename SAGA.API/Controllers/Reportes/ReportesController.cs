using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Reportes
{
    public class ReportesController : ApiController
    {
        private SAGADBContext db;


        [HttpGet]
        [Route("get")]
        public IHttpActionResult Get()
        {
            var datos = db.Requisiciones.ToList();
            return Ok("");
        }
    }
}
