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
    [RoutePrefix("api/Campo")]
    public class CampoController : ApiController
    {
        private SAGADBContext db;
        public CampoController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getReclutadores")]
        public IHttpActionResult GetReclutadores()
        {
            int[] estatusId = new int[] { 8, 9, 34, 35, 36, 37, 47, 48 };
            try
            {
                var reclutadores = db.AsignacionRequis.Where(x => x.Requisicion.Activo && !estatusId.Contains(x.Requisicion.EstatusId) && !x.Requisicion.Confidencial).GroupBy(g => g.GrpUsrId)
                    .Select(R => new
                    {
                        nombre = db.Usuarios.Where(x => x.Id.Equals(R.Key)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                        reclutadorId = R.Key,
                        requisiciones = R.Select(r => r.RequisicionId).Count()
                    });
                return Ok(reclutadores);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpGet]
        [Route("getRequisReclutadores")]
        public IHttpActionResult GetRequiReclutadores(Guid reclutadorId)
        {
            int[] estatusId = new int[] { 8, 9, 34, 35, 36, 37, 47, 48 };
            try
            {
                var reclutadores = db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(reclutadorId) && x.Requisicion.Activo && !estatusId.Contains(x.Requisicion.EstatusId) && !x.Requisicion.Confidencial)
                    .Select(r => new
                    {
                      
                            r.Requisicion.Id,
                            r.Requisicion.Folio,
                            r.Requisicion.VBtra,
                            cliente = r.Requisicion.Cliente.Nombrecomercial,
                            r.Requisicion.fch_Cumplimiento,
                            Vacantes = r.Requisicion.horariosRequi.Count() > 0 ? r.Requisicion.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                            Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(r.Requisicion.Id) && p.EstatusId.Equals(24)).Count(),
                    });
                return Ok(reclutadores);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
}
}
