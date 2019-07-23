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
                if (TipoUsuario == 8 || TipoUsuario == 3 || TipoUsuario == 12 || TipoUsuario == 13 || TipoUsuario == 14)
                {
                    var Vigentes = db.Requisiciones
                        .Where(r => r.fch_Cumplimiento >= DateActivas)
                        .Where(e => e.Activo
                            && !e.Confidencial
                            && estatus.Contains(e.EstatusId))
                        .Count();
                    var PorVencer = db.Requisiciones
                        .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                        .Where(e => e.Activo
                            && !e.Confidencial
                            && estatus.Contains(e.EstatusId))
                        .Count();
                    var vencidas = db.Requisiciones
                        .Where(r => r.fch_Cumplimiento < DateTime.Now)
                        .Where(e => e.Activo
                            && !e.Confidencial
                            && estatus.Contains(e.EstatusId))
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

                    var asignadas = db.AsignacionRequis
                            .OrderByDescending(e => e.Id)
                            .Where(a => uids.Distinct().Contains(a.GrpUsrId)
                                && estatus.Contains(a.Requisicion.EstatusId))
                            .Select(a => a.RequisicionId)
                            .Distinct()
                            .ToList();

                    var requis = db.Requisiciones
                    .Where(e => (uids.Contains(e.AprobadorId) || uids.Contains(e.PropietarioId))
                        && estatus.Contains(e.EstatusId))
                    .Select(a => a.Id).ToList();

                    var AllRequis = requis.Union(asignadas);

                    var VigentesR = db.Requisiciones
                        .Where(r => r.fch_Cumplimiento >= DateActivas)
                        .Where(e => AllRequis.Distinct().Contains(e.Id))
                        .Count();

                    var PorVencerR = db.Requisiciones
                        .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                        .Where(e => AllRequis.Distinct().Contains(e.Id))
                        .Count();

                    var vencidasR = db.Requisiciones.Where(r => r.Activo.Equals(true))
                        .Where(r => r.fch_Cumplimiento < DateTime.Now)
                        .Where(e => AllRequis.Distinct().Contains(e.Id))
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
                if (tipo == 8 || tipo == 3 || tipo == 12 || tipo == 13 || tipo == 14)
                {
                    if (estado == "Todas")
                    {
                        var requisicion = db.Requisiciones
                        .Where(r => r.Activo.Equals(true)
                        && estatus.Contains(r.EstatusId)
                        && !r.Confidencial)
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
                    if (estado == "Vigentes")
                    {
                        var requisicion = db.Requisiciones
                        .Where(r => r.fch_Cumplimiento > DateActivas
                        && r.Activo.Equals(true)
                        && estatus.Contains(r.EstatusId) 
                        && !r.Confidencial)
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
                        .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas
                        && r.Activo.Equals(true)
                        && estatus.Contains(r.EstatusId)
                        && !r.Confidencial)
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
                        .Where(r => r.fch_Cumplimiento < DateTime.Now 
                        && r.Activo.Equals(true)
                        && estatus.Contains(r.EstatusId)
                        && !r.Confidencial)
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
                    }else if (estado == "Cubiertas parcialmente")
                    {
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo.Equals(true))

                        .Where(r => r.EstatusId == 35)
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
                    else if (estado == "Cubiertas")
                    {
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo.Equals(true))
                        .Where(r => r.EstatusId == 34)
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
                    else if (estado == "Cubiertas por medios")
                    {
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo.Equals(true))
                        .Where(r => r.EstatusId == 36)
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
                    else if (estado == "Cubiertas por el cliente")
                    {
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo.Equals(true))
                        .Where(r => r.EstatusId == 37)
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
                    else if (estado == "Captado")
                    {
                        var datos = db.ProcesoCandidatos.Where(e => e.EstatusId != 24).ToList();
                        var capta = datos.Select(e => e.RequisicionId).Distinct().ToList();

                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo == true && capta.Contains(e.Id))
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
                    else if (estado == "Contratado")
                    {
                        var datos = db.ProcesoCandidatos.Where(e => e.EstatusId == 24).ToList();
                        var capta = datos.Select(e => e.RequisicionId).Distinct().ToList();

                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo == true && capta.Contains(e.Id))
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
                    else if (estado == "Masivo")
                    {
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo == true && e.ClaseReclutamientoId == 3)
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
                    else if (estado == "Operativo")
                    {
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo == true && e.ClaseReclutamientoId == 2)
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
                    else if (estado == "Especial")
                    {
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo == true && e.ClaseReclutamientoId == 1)
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

                    var asignadas = db.AsignacionRequis
                        .Where(x => uids.Contains(x.GrpUsrId)
                        && estatus.Contains(x.Requisicion.EstatusId))
                        .Select(a => a.RequisicionId)
                        .ToList();

                    var requis = db.Requisiciones
                    .Where(e => (uids.Contains(e.AprobadorId) || uids.Contains(e.PropietarioId))
                        && estatus.Contains(e.EstatusId))
                    .Select(a => a.Id).ToList();

                    var AllRequis = requis.Union(asignadas);
                    if(estado == "Todas")
                    {
                        var requisicion = db.Requisiciones
                        .Where(e => AllRequis.Distinct().Contains(e.Id))
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

                    if (estado == "Vigentes")
                    {
                        var requisicion = db.Requisiciones
                        .Where(r => r.fch_Cumplimiento >= DateActivas)
                        .Where(e => AllRequis.Distinct().Contains(e.Id))
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
                        .Where(r => r.fch_Cumplimiento >= DateTime.Now && r.fch_Cumplimiento < DateActivas)
                        .Where(e => AllRequis.Distinct().Contains(e.Id))
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
                        .Where(r => r.fch_Cumplimiento < DateTime.Now)
                        .Where(e => AllRequis.Distinct().Contains(e.Id))
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
                    }else if (estado == "Parcialmente")
                    {
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || uids.Contains(e.PropietarioId) && e.Activo == true).ToList();
                        var requisicion = datos
                        .Where(r => r.EstatusId == 35)
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
                    else if (estado == "Cubiertas")
                    {
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || uids.Contains(e.PropietarioId) && e.Activo == true).ToList();
                        var requisicion = datos
                        .Where(r => r.EstatusId == 34)
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
                    else if (estado == "Cubiertas por medios")
                    {
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || uids.Contains(e.PropietarioId) && e.Activo == true).ToList();
                        var requisicion = datos
                        .Where(r => r.EstatusId == 36)
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
                    else if (estado == "Por el Cliente")
                    {
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || uids.Contains(e.PropietarioId) && e.Activo == true).ToList();
                        var requisicion = datos
                        .Where(r => r.EstatusId == 37)
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
                    else if (estado == "Promocion Interna")
                    {
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || uids.Contains(e.PropietarioId) && e.Activo == true).ToList();
                        var requisicion = datos
                        .Where(r => r.EstatusId == 47)
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
                    else if (estado == "Operaciones")
                    {
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo.Equals(true))
                        .Where(r => requis.Contains(r.Id) || r.PropietarioId.Equals(UsuarioId))
                        .Where(r => r.EstatusId == 48)
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
                    else if (estado == "Captado")
                    {
                        var datos = db.ProcesoCandidatos.Where(e=> uids.Distinct().Contains(e.ReclutadorId)).ToList();
                        var capta = datos.Select(e => e.RequisicionId).Distinct().ToList();

                        var requisicion = db.Requisiciones
                        .Where(e => capta.Contains(e.Id))
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
                    else if (estado == "Contratado")
                    {
                        var datos = db.ProcesoCandidatos.Where(e => uids.Distinct().Contains(e.ReclutadorId) && e.EstatusId == 24).ToList();
                        var capta = datos.Select(e => e.RequisicionId).Distinct().ToList();

                        var requisicion = db.Requisiciones
                        .Where(e => capta.Contains(e.Id))
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
                    else if (estado == "Masivo")
                    {
                        int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39 };
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || uids.Contains(e.PropietarioId) && e.Activo == true).ToList();
                        var requisicion = datos
                        .Where(e => e.ClaseReclutamientoId == 3 && EstatusList.Contains(e.EstatusId))
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
                    else if (estado == "Operativo")
                    {
                        int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39 };
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || uids.Contains(e.PropietarioId) && e.Activo == true).ToList();
                        var requisicion = datos
                        .Where(e => e.ClaseReclutamientoId == 2 && EstatusList.Contains(e.EstatusId))
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
                    else if (estado == "Especial")
                    {
                        int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39 };
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || uids.Contains(e.PropietarioId) && e.Activo == true).ToList();
                        var requisicion = datos
                        .Where(e => e.ClaseReclutamientoId == 1 && EstatusList.Contains(e.EstatusId))
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
                    else if (estado == "Nuevas" || estado == "Aprobadas" || estado == "Publicadas" || estado == "Búsqueda de candidatos" 
                        || estado == "Envió al cliente" || estado == "Nueva busqueda" || estado == "Socioeconomicos" || estado == "En espera de contratación" ||
                        estado == "Pausadas" || estado == "Garantía de búsqueda")
                    {
                       
                        int valor = 0;
                        valor = estado == "Nuevas" ? 4 : valor;
                        valor = estado == "Aprobadas" ? 6 : valor;
                        valor = estado == "Publicadas" ? 7 : valor;
                        valor = estado == "Búsqueda de candidatos" ? 29 : valor;
                        valor = estado == "Envió al cliente" ? 30 : valor;
                        valor = estado == "Nueva busqueda" ? 31 : valor;
                        valor = estado == "Socioeconomicos" ? 32 : valor;
                        valor = estado == "En espera de contratación" ? 33 : valor;
                        valor = estado == "Pausadas" ? 39 : valor;
                        valor = estado == "Garantía de búsqueda" ? 38 : valor;
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || uids.Contains(e.PropietarioId) && e.Activo == true).ToList();
                        datos = datos.Where(e => e.EstatusId == valor).ToList();
                        var requisicion = datos
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

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        
    }
}
