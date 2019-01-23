using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Examenes")]
    public class PsicometricosController : ApiController
    {
        private SAGADBContext db;
        public PsicometricosController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getRequisiciones")]
        public IHttpActionResult GetRequisiciones()
        {
            try
            {

                var requisiciones = db.PsicometriasDamsaRequis.Select(R => R.RequisicionId).ToList().Distinct();

                var psico = db.Requisiciones.Where(x => requisiciones.Contains(x.Id)).Select(R => new
                {
                    requisicionId = R.Id,
                    folio = R.Folio,
                    vBtra = R.VBtra,
                    psicometricos = db.PsicometriasDamsaRequis.Where(x => x.RequisicionId.Equals(R.Id)).Select(C => new { nombre = C.Psicometria.tipoPsicometria }).ToList(),
                    claves = db.RequiClaves.Where(x => x.RequisicionId.Equals(R.Id)).Count()
                }).ToList();

                return Ok(psico);
            }
            catch(Exception)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

           

        }
    }

   
}
