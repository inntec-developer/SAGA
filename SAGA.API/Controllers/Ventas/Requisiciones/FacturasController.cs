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

namespace SAGA.API.Controllers.Ventas.Requisiciones
{
    [RoutePrefix("api/Requisiciones")]
    public class FacturasController : ApiController
    {
        private SAGADBContext db;

        public FacturasController()
        {
            db = new SAGADBContext();
        }

        [HttpPost]
        [Route("insertDtosFactura")]
        public IHttpActionResult InsertDtosFactura(FacturacionPuro Datos)
        {
            try
            {
                PostulateVacantController obj = new PostulateVacantController();

                Datos.fch_Creacion = DateTime.Now;
                Datos.fch_Modificacion = DateTime.Now;

                db.FacturacionPuro.Add(Datos);
                db.SaveChanges();

                //ProcesoDto d = new ProcesoDto();
                //d.requisicionId = Datos.RequisicionId;
                //d.estatusId = estatus;

                //var mocos = obj.UpdateStatusVacante(d);


                return Ok(HttpStatusCode.OK);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }


    }
}
