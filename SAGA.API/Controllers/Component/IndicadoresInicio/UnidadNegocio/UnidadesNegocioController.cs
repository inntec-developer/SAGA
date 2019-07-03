using SAGA.API.Dtos;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Component.IndicadoresInicio.UnidadNegocio
{
    [RoutePrefix("api/contadores")]
    [Authorize]
    public class UnidadesNegocioController : ApiController
    {
        private SAGADBContext db;
        private VacantesReclutador vr;

        public UnidadesNegocioController()
        {
            db = new SAGADBContext();
            vr = new VacantesReclutador();
        }

        [HttpGet]
        [Route("unidadMty")]
        public IHttpActionResult Monterrey()
        {
            try
            {
                int[] estatus = { 4, 5, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 44, 46 };
                int?[] mty = { 6, 7, 10, 19, 28, 24 };
                
                var DateActivas = DateTime.Now.AddDays(3);


                var Vigentes = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento >= DateActivas)
                    .Where(r => mty.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId)
                        && !r.Confidencial)
                        .Count();

                var PorVencer = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                    .Where(r => mty.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId)
                        && !r.Confidencial)
                        .Count();

                var Vencidas = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento < DateTime.Now)
                    .Where(r => mty.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId)
                        && !r.Confidencial)
                        .Count();

                var obj = new
                {
                    vigentes = Vigentes,
                    porVencer = PorVencer,
                    vencidas = Vencidas
                };

                return Ok(obj);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("unidadGdl")]
        public IHttpActionResult Guadalajara()
        {
            try
            {
                int?[] gdl = { 1, 3, 8, 10, 11, 14, 16, 18, 2, 25, 26, 32 };
                int[] estatus = { 4, 5, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 44, 46 };
                var DateActivas = DateTime.Now.AddDays(3);

                var Vigentes = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento >= DateActivas)
                    .Where(r => gdl.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId)
                        && !r.Confidencial)
                        .Count();

                var PorVencer = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                    .Where(r => gdl.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId)
                        && !r.Confidencial)
                        .Count();

                var Vencidas = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento < DateTime.Now)
                    .Where(r => gdl.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId)
                        && !r.Confidencial)
                        .Count();

                var obj = new
                {
                    vigentes = Vigentes,
                    porVencer = PorVencer,
                    vencidas = Vencidas
                };

                return Ok(obj);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("unidadMx")]
        public IHttpActionResult Mexico()
        {
            try
            {
                int?[] mx = { 4, 5, 9, 12, 13, 15, 17, 20, 21, 22, 23, 27, 29, 30, 31 };
                int[] estatus = { 4, 5, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 44, 46 };
                var DateActivas = DateTime.Now.AddDays(3);

                var Vigentes = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento >= DateActivas)
                    .Where(r => mx.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId)
                        && !r.Confidencial)
                        .Count();

                var PorVencer = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                    .Where(r => mx.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId)
                        && !r.Confidencial)
                        .Count();

                var Vencidas = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento < DateTime.Now)
                    .Where(r => mx.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId)
                        && !r.Confidencial)
                        .Count();

                var obj = new
                {
                    vigentes = Vigentes,
                    porVencer = PorVencer,
                    vencidas = Vencidas
                };

                return Ok(obj);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
    }
}
