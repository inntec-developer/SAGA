using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;

namespace SAGA.API.Controllers.Reclutamiento.SeguimientoVacante
{
    [RoutePrefix("api/reclutamiento/SeguimientoVacante")]
    public class DetailController : ApiController
    {
        private SAGADBContext db;

        public DetailController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getvacantesdtl")]
        public IHttpActionResult GetVacantesdtl(Guid VacanteId)
        {
            DetailDto Details = new DetailDto();
            Details.Requisicion = db.Requisiciones
                .Where(v => v.Id.Equals(VacanteId))
                .Single();
            Details.DocumentosDamsa = db.DocumentosDamsa
                .ToList();
            Details.PrestacionesLey = db.PrestacionesLey
                .ToList();

            return Ok(Details);
        }
    }
}
