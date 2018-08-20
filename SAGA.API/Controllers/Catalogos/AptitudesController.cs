using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Aptitud")]
    public class AptitudesController : ApiController
    {
        private SAGADBContext db;

        public AptitudesController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("get")]
        public IHttpActionResult Get()
        {
            return Ok(db.Aptitudes.ToList());
        }
    }
}
