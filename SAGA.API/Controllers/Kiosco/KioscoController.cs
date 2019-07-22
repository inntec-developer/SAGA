using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using SAGA.API.Utilerias;

namespace SAGA.API.Controllers.Kiosko
{
    [RoutePrefix("api/Kiosco")]
    public class KioscoController : ApiController
    {
        private SAGADBContext db;

        public KioscoController()
        {
            db = new SAGADBContext();
        }

        [HttpPost]
        [Route("postulacion")]
        public IHttpActionResult Postulacion(ProcesoDto datos)
        {
            try
            {
                Postulacion obj = new Postulacion();
                obj.CandidatoId = datos.candidatoId;
                obj.fch_Postulacion = DateTime.Now;
                obj.RequisicionId = datos.requisicionId;
                obj.StatusId = 1;

                db.Postulaciones.Add(obj);
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
            
        }
    }
}
