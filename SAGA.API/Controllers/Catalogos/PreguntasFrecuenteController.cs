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
    [RoutePrefix("api/PreguntasFrecuente")]
    public class PreguntasFrecuenteController : ApiController
    {
        private SAGADBContext db;


        [HttpGet]
        [Route("get")]
        public IHttpActionResult Get()
        {
            return Ok("");
        }
    }
}