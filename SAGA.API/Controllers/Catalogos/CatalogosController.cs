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
    [RoutePrefix("api/Catalogos")]
    public class CatalogosController : ApiController
    {
        private SAGADBContext db;
        public CatalogosController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getDocDamsa")]
        public IHttpActionResult GetDocDamsa()
        {
            try
            {
                var documentosDamsa = db.DocumentosDamsa.ToList();
                return Ok(documentosDamsa);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet]
        [Route("getPrestacionesLey")]
        public IHttpActionResult GetPrestacionesLey()
        {
            try
            {
                var prestacionesLey = db.PrestacionesLey.ToList();
                return Ok(prestacionesLey);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
