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


                return Ok(HttpStatusCode.OK);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }


        [HttpGet]
        [Route("getRequisPendientes")]
        [Authorize]
        public IHttpActionResult GetRequisPendientes()
        {
            try {
                var requis = db.FacturacionPuro.Where(x => x.Requisicion.EstatusId.Equals(46) && x.Porcentaje > 0 && x.Porcentaje < 50).Select(e => new
                {
                    Id = e.Id,
                    requisicionId = e.RequisicionId,
                    VBtra = e.Requisicion.VBtra,
                    fch_Creacion = e.fch_Creacion,
                    fch_Modificacion = e.fch_Modificacion,
                    fch_Cumplimiento = e.Requisicion.fch_Cumplimiento,
                    Estatus = e.Requisicion.Estatus.Descripcion,
                    EstatusId = e.Requisicion.EstatusId,
                    Prioridad = e.Requisicion.Prioridad.Descripcion,
                    PrioridadId = e.Requisicion.PrioridadId,
                    EstatusOrden = e.Requisicion.Estatus.Orden,
                    SueldoMaximo = e.Requisicion.SueldoMaximo,
                    razon = e.Requisicion.Cliente.RazonSocial,
                    Cliente = e.Requisicion.Cliente.Nombrecomercial,
                    Vacantes = e.Requisicion.horariosRequi.Count() > 0 ? e.Requisicion.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                    Folio = e.Requisicion.Folio,
                    Porcentaje = e.Porcentaje,
                    Monto = e.Monto,
                    PerContratado = e.PerContratado,
                    MontoContratado = e.MontoContratado,
                    Propietario = db.Usuarios.Where(x => x.Id.Equals(e.Requisicion.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                    reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.Requisicion.AprobadorId)).Select(a => new
                    {
                        reclutador = db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault()
                    }).Distinct().ToList(),
                    //ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList()
                }).OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.fch_Modificacion).ToList();

                return Ok(requis);
            }
            catch
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
    }
}
