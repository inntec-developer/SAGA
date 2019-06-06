using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.API.Dtos;
using SAGA.BOL;

namespace SAGA.API.Controllers.Component.Graficas
{
    [RoutePrefix("api/Graficas")]
    public class GraficaVacantesPolarAreaController : ApiController
    {
        private SAGADBContext db;
        private List<Guid> uids;
        private VacantesReclutador vr;

        GraficaVacantesPolarAreaController()
        {
            db = new SAGADBContext();
            uids = new List<Guid>();
            vr = new VacantesReclutador();
        }

        public List<Guid> GetSub(List<Guid> uid, List<Guid> listaIds)
        {
            foreach (var u in uid)
            {
                listaIds.Add(u);
                var listadoNuevo = db.Subordinados
                  .Where(g => g.LiderId.Equals(u))
                         .Select(g => g.UsuarioId)
                         .ToList();

                GetSub(listadoNuevo, listaIds);

            }
            return listaIds;
        }

        [HttpGet]
        [Route("vacantesInicio")]
        public IHttpActionResult VacantesInicio(Guid UsuarioId)
        {
            try
            {
                int[] estatus = { 4, 5, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 44, 46 };
                var DateActivas = DateTime.Now.AddDays(3);

                var TipoUsuario = db.Usuarios.Where(u => u.Id.Equals(UsuarioId)).Select(u => u.TipoUsuarioId).FirstOrDefault();
                if (TipoUsuario == 8)
                {
                    var Vigentes = db.Requisiciones
                        .Where(r => r.Activo.Equals(true))
                        .Where(r => r.fch_Cumplimiento > DateActivas)
                        .Where(r => estatus.Contains(r.EstatusId))
                        .Count();
                    var PorVencer = db.Requisiciones
                        .Where(r => r.Activo.Equals(true))
                        .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                        .Where(r => estatus.Contains(r.EstatusId))
                        .Count();
                    var vencidas = db.Requisiciones.Where(r => r.Activo.Equals(true))
                        .Where(r => r.fch_Cumplimiento < DateTime.Now)
                        .Where(r => estatus.Contains(r.EstatusId))
                        .Count();

                    vr.Vigentes = Vigentes;
                    vr.PorVencer = PorVencer;
                    vr.Vencidas = vencidas;

                }
                else
                {
                    if (db.Subordinados.Count(x => x.LiderId.Equals(UsuarioId)) > 0)
                    {
                        var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(UsuarioId) && x.LiderId.Equals(UsuarioId)).Select(u => u.UsuarioId).ToList();

                        uids = GetSub(ids, uids);

                    }

                    uids.Add(UsuarioId);

                    var requis = db.AsignacionRequis
                            .OrderByDescending(e => e.Id)
                            .Where(a => uids.Distinct().Contains(a.GrpUsrId))
                            .Select(a => a.RequisicionId)
                            .Distinct()
                            .ToList();
                    var VigentesR = db.Requisiciones
                        .Where(r => r.Activo.Equals(true))
                        .Where(r => requis.Contains(r.Id) || r.PropietarioId.Equals(UsuarioId))
                        .Where(r => r.fch_Cumplimiento > DateActivas)
                        .Where(r => estatus.Contains(r.EstatusId))
                        .Count();
                    var PorVencerR = db.Requisiciones
                        .Where(r => r.Activo.Equals(true))
                        .Where(r => requis.Contains(r.Id) || r.PropietarioId.Equals(UsuarioId))
                        .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                        .Where(r => estatus.Contains(r.EstatusId))
                        .Count();
                    var vencidasR = db.Requisiciones.Where(r => r.Activo.Equals(true))
                        .Where(r => requis.Contains(r.Id) || r.PropietarioId.Equals(UsuarioId))
                        .Where(r => r.fch_Cumplimiento < DateTime.Now)
                        .Where(r => estatus.Contains(r.EstatusId))
                        .Count();

                    vr.Vigentes = VigentesR;
                    vr.PorVencer = PorVencerR;
                    vr.Vencidas = vencidasR;
                }
                return Ok(vr);
            }
            catch
            {
                return Ok(HttpStatusCode.NotFound);
            }      

        }

        [HttpGet]
        [Route("getRequisicionesGPA")]
        public IHttpActionResult GetRequisicionesGPA(string estado, Guid UsuarioId)
        {
            List<Guid> uids = new List<Guid>();
            int[] estatus = { 4, 5, 6, 7, 29, 30, 39, 31, 32, 33, 38, 39, 43, 44, 46 };
            var DateActivas = DateTime.Now.AddDays(3);
            try
            {
                var tipo = db.Usuarios.Where(x => x.Id.Equals(UsuarioId)).Select(u => u.TipoUsuarioId).FirstOrDefault();
                if (tipo == 8)
                {
                    if (estado == "Vigentes")
                    {
                        var requisicion = db.Requisiciones
                        .Where(r => r.Activo.Equals(true))
                        .Where(r => r.fch_Cumplimiento > DateActivas)
                        .Where(r => estatus.Contains(r.EstatusId))
                       .Select(e => new
                       {
                           Id = e.Id,
                           Folio = e.Folio,
                           fch_Creacion = e.fch_Creacion,
                           fch_Cumplimiento = e.fch_Cumplimiento,
                           Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                           VBtra = e.VBtra.ToUpper(),
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
                           ).ToList()
                       }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else
                    if (estado == "Por Vencer")
                    {
                        var requisicion = db.Requisiciones
                        .Where(r => r.Activo.Equals(true))
                        .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                        .Where(r => estatus.Contains(r.EstatusId))
                        .Select(e => new
                        {
                            Id = e.Id,
                            Folio = e.Folio,
                            fch_Creacion = e.fch_Creacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                            VBtra = e.VBtra.ToUpper(),
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
                        return Ok(requisicion);
                    }
                    else
                    if (estado == "Vencidas")
                    {
                        var requisicion = db.Requisiciones
                        .Where(r => r.Activo.Equals(true))
                        .Where(r => r.fch_Cumplimiento < DateTime.Now)
                        .Where(r => estatus.Contains(r.EstatusId))
                        .Select(e => new
                        {
                            Id = e.Id,
                            Folio = e.Folio,
                            fch_Creacion = e.fch_Creacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                            VBtra = e.VBtra.ToUpper(),
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
                        return Ok(requisicion);
                    }
                    return NotFound();
                }
                else
                {
                    if (db.Subordinados.Count(x => x.LiderId.Equals(UsuarioId)) > 0)
                    {
                        var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(UsuarioId) && x.LiderId.Equals(UsuarioId)).Select(u => u.UsuarioId).ToList();

                        uids = GetSub(ids, uids);

                    }

                    uids.Add(UsuarioId);

                    var requis = db.AsignacionRequis
                            .OrderByDescending(e => e.Id)
                            .Where(a => uids.Distinct().Contains(a.GrpUsrId))
                            .Select(a => a.RequisicionId)
                            .Distinct()
                            .ToList();
                   if (estado == "Vigentes")
                    {
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo.Equals(true))
                        .Where(r => requis.Contains(r.Id) || r.PropietarioId.Equals(UsuarioId))
                        .Where(r => r.fch_Cumplimiento > DateActivas)
                        .Where(r => estatus.Contains(r.EstatusId))
                        .Select(e => new
                        {
                            Id = e.Id,
                            Folio = e.Folio,
                            fch_Creacion = e.fch_Creacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                            VBtra = e.VBtra.ToUpper(),
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
                        return Ok(requisicion);
                    }
                    else
                    if (estado == "Por Vencer")
                    {
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo.Equals(true))
                        .Where(r => requis.Contains(r.Id) || r.PropietarioId.Equals(UsuarioId))
                        .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                        .Where(r => estatus.Contains(r.EstatusId))
                        .Select(e => new
                        {
                            Id = e.Id,
                            Folio = e.Folio,
                            fch_Creacion = e.fch_Creacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                            VBtra = e.VBtra.ToUpper(),
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
                        return Ok(requisicion);
                    }
                    else
                    if (estado == "Vencidas")
                    {
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo.Equals(true))
                        .Where(r => requis.Contains(r.Id) || r.PropietarioId.Equals(UsuarioId))
                        .Where(r => r.fch_Cumplimiento < DateTime.Now)
                        .Where(r => estatus.Contains(r.EstatusId))
                        .Select(e => new
                        {
                            Id = e.Id,
                            Folio = e.Folio,
                            fch_Creacion = e.fch_Creacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                            VBtra = e.VBtra.ToUpper(),
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
                        //OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.Folio).ToList();
                        return Ok(requisicion);
                    }
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        
    }
}
