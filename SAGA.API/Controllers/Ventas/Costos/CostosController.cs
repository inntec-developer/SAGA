using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Costos")]
    public class CostosController : ApiController
    {
        private SAGADBContext db;
        public CostosController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getCostos")]
        [Authorize]
        public IHttpActionResult GetCostos()
        {
            try
            {
                var costos = db.Costos.Select(c => new
                {
                    costo = c.Descripcion,
                    tipos = db.TipoCostos.Where(x => x.CostosId.Equals(c.Id)).Select(cc => new
                    {
                        tipoId = cc.Id,
                        tipo = cc.Descripcion
                    })
                });
                return Ok(costos);
            }
            catch (Exception)
            {
                return Ok(HttpStatusCode.BadRequest);
                throw;
            }
        }
    }
}
