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
    //[Authorize]
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
                        && estatus.Contains(r.EstatusId))
                        .Select(r => new {
                            Vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        })
                        .ToList();

                var PorVencer = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                    .Where(r => mty.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId)
                        && !r.Confidencial)
                        .Select(r => new {
                            Vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        })
                        .ToList();

                var Vencidas = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento < DateTime.Now)
                    .Where(r => mty.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId))
                        .Select(r => new {
                            Vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        })
                        .ToList();

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
                int?[] gdl = { 1, 3, 8, 11, 14, 16, 18, 2, 25, 26, 32 };
                int[] estatus = { 4, 5, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 44, 46 };
                var DateActivas = DateTime.Now.AddDays(3);

                var Vigentes = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento >= DateActivas)
                    .Where(r => gdl.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId))
                        .Select(r => new {
                            Vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        })
                        .ToList();

                var PorVencer = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                    .Where(r => gdl.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId))
                        .Select(r => new {
                            Vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        })
                        .ToList();

                var Vencidas = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento < DateTime.Now)
                    .Where(r => gdl.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId))
                        .Select(r => new {
                            Vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        })
                        .ToList();

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
                        && estatus.Contains(r.EstatusId))
                        .Select(r => new {
                            Vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        })
                        .ToList();

                var PorVencer = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                    .Where(r => mx.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId))
                        .Select(r => new {
                            Vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        })
                        .ToList();

                var Vencidas = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento < DateTime.Now)
                    .Where(r => mx.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId))
                        .Select(r => new {
                            Vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        })
                        .ToList();

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

        [HttpPost]
        [Route("getRequiUnidadNegocio")]
        public IHttpActionResult GetRequiUnidadNegocio(RequiUNDto data)
        {
            try
            {
                var DateActivas = DateTime.Now.AddDays(3);
                int[] estatus = { 4, 5, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 44, 46 };
                int?[] unidadNegocio= {0};
                switch (data.UnidadNegocio)
                {
                    case "Mty":
                        int?[] mty = { 6, 7, 10, 19, 28, 24 };
                        unidadNegocio = mty;
                        break;
                    case "Gdl":
                        int?[] gdl = { 1, 3, 8, 11, 14, 16, 18, 2, 25, 26, 32 };
                        unidadNegocio = gdl;
                        break;
                    case "Mx":
                        int?[] mx = { 4, 5, 9, 12, 13, 15, 17, 20, 21, 22, 23, 27, 29, 30, 31 };
                        unidadNegocio = mx;
                        break;
                    default:
                        break;
                }
                if (data.EstadoVacante == "Todas")
                {
                    var Todas = db.Requisiciones
                   .Where(r => unidadNegocio.Contains(r.Direccion.EstadoId)
                       && estatus.Contains(r.EstatusId))
                       .Select(e => new
                       {
                           Id = e.Id,
                           Folio = e.Folio,
                           fch_Creacion = e.fch_Creacion,
                           fch_Cumplimiento = e.fch_Cumplimiento,
                           Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                           VBtra = e.Confidencial ? "CONFIDENCIAL" : e.VBtra.ToUpper(),
                           Estatus = e.Estatus.Descripcion.ToUpper(),
                           EstatusId = e.EstatusId,
                           EstatusOrden = e.Estatus.Orden,
                           Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count(),
                           Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                           Confidencial = e.Confidencial,
                           coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                           Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                           reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>

                               db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                           ).Distinct().ToList()
                       }).OrderBy(x => x.fch_Cumplimiento).ToList();
                    return Ok(Todas);
                }
                if (data.EstadoVacante == "Vigentes")
                {
                    var Vigentes = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento >= DateActivas)
                    .Where(r => unidadNegocio.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId))
                        .Select(e => new
                        {
                            Id = e.Id,
                            Folio = e.Folio,
                            fch_Creacion = e.fch_Creacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                            VBtra = e.Confidencial ? "CONFIDENCIAL" : e.VBtra.ToUpper(),
                            Estatus = e.Estatus.Descripcion.ToUpper(),
                            EstatusId = e.EstatusId,
                            EstatusOrden = e.Estatus.Orden,
                            Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count(),
                            Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                            Confidencial = e.Confidencial,
                            coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>

                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                    return Ok(Vigentes);
                }
                if (data.EstadoVacante == "Por Vencer")
                {
                    var PorVencer = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                    .Where(r => unidadNegocio.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId))
                        .Select(e => new
                        {
                            Id = e.Id,
                            Folio = e.Folio,
                            fch_Creacion = e.fch_Creacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                            VBtra = e.Confidencial ? "CONFIDENCIAL" : e.VBtra.ToUpper(),
                            Estatus = e.Estatus.Descripcion.ToUpper(),
                            EstatusId = e.EstatusId,
                            EstatusOrden = e.Estatus.Orden,
                            Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count(),
                            Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                            Confidencial = e.Confidencial,
                            coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>

                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                    return Ok(PorVencer);
                }
                if (data.EstadoVacante == "Vencidas")
                {
                    var Vencidas = db.Requisiciones
                    .Where(r => r.fch_Cumplimiento < DateTime.Now)
                    .Where(r => unidadNegocio.Contains(r.Direccion.EstadoId)
                        && estatus.Contains(r.EstatusId))
                        .Select(e => new
                        {
                            Id = e.Id,
                            Folio = e.Folio,
                            fch_Creacion = e.fch_Creacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                            VBtra = e.Confidencial ? "CONFIDENCIAL" : e.VBtra.ToUpper(),
                            Estatus = e.Estatus.Descripcion.ToUpper(),
                            EstatusId = e.EstatusId,
                            EstatusOrden = e.Estatus.Orden,
                            Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count(),
                            Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                            Confidencial = e.Confidencial,
                            coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>

                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                    return Ok(Vencidas);
                }
                return Ok(HttpStatusCode.NotFound);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
    }
}
