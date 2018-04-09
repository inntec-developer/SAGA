using SAGA.DAL;
using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Requisiciones")]
    public class RequisicionesController : ApiController
    {
        private SAGADBContext db;

        public RequisicionesController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getDamfos")]
        public IHttpActionResult Get()
      {
            return Ok(db.DAMFO290.ToList());
        }
    }
}
