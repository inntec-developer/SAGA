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
                    #region tipo admin
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
                    }
                    else if (estado == "Masivo")
                    {
                        int[] EstatusActivo = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 46 };
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo == true && e.ClaseReclutamientoId == 3 && e.Confidencial == false && EstatusActivo.Contains(e.EstatusId))
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
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "Operativo")
                    {
                        int[] EstatusActivo = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 46 };
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo == true && e.ClaseReclutamientoId == 2 && e.Confidencial == false && EstatusActivo.Contains(e.EstatusId))
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
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "Especializado")
                    {
                        int[] EstatusActivo = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 46 };
                        var requisicion = db.Requisiciones
                        .Where(e => e.Activo == true && e.ClaseReclutamientoId == 1 && e.Confidencial == false && EstatusActivo.Contains(e.EstatusId))
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
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "Nuevas" || estado == "Aprobadas" || estado == "Publicadas" || estado == "Búsqueda de candidatos"
                        || estado == "Envió al cliente" || estado == "Nueva busqueda" || estado == "Socioeconomicos" || estado == "En espera de contratación" ||
                        estado == "Pausadas" || estado == "Garantía de búsqueda" || estado == "Pendiente" || estado == "PendienteGG")
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
                        valor = estado == "Pendiente" ? 43 : valor;
                        valor = estado == "PendienteGG" ? 46 : valor;
                    //    var asigna = db.AsignacionRequis.Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e =>  e.Activo == true && e.Confidencial == false).ToList();
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
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                 db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "Folios" || estado == "Posiciones" || estado == "Cubiertas" || estado == "Faltantes")
                    {
                        int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 46 };
                        var asigna = db.AsignacionRequis.Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e => e.Activo == true && e.Confidencial == false).ToList();
                        datos = datos.Where(e => EstatusList.Contains(e.EstatusId)).ToList();
                        if (estado == "Cubiertas")
                        {
                            var requien = datos.Select(e => e.Id).ToList();
                            var proseso = db.ProcesoCandidatos.Where(e => requien.Contains(e.RequisicionId)).ToList();
                            var cubierto = proseso.Where(e => e.EstatusId == 24).Select(e => e.RequisicionId).Distinct().ToList();
                            datos = datos.Where(e => cubierto.Contains(e.Id)).ToList();
                        }

                        if (estado == "Faltantes")
                        {
                            var requien = datos.Select(e => e.Id).ToList();
                            var proseso = db.ProcesoCandidatos.Where(e => requien.Contains(e.RequisicionId)).ToList();
                            var cubierto = proseso.Where(e => e.EstatusId == 24).Select(e => e.RequisicionId).Distinct().ToList();
                            datos = datos.Where(e => !cubierto.Contains(e.Id)).ToList();
                        }
                        var requisicion = datos
                        .Where(e => EstatusList.Contains(e.EstatusId))
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
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "diciembre" || estado == "noviembre" || estado == "octubre" || estado == "septiembre" || estado == "agosto"
                      || estado == "julio" || estado == "junio" || estado == "mayo" || estado == "abril" || estado == "marzo"
                      || estado == "febrero" || estado == "enero")
                    {
                        DateTime fechaInicio = DateTime.Now;
                        DateTime fechaFinal = DateTime.Now;
                        if (estado == "diciembre" || estado == "noviembre" || estado == "octubre" || estado == "septiembre" || estado == "agosto"
                        || estado == "julio")
                        {
                            if (fechaInicio.Month < 6)
                            {
                                int valor = fechaInicio.Year - 1;
                                fechaInicio = new DateTime(valor, 1, 1);
                            }
                        }
                        switch (estado)
                        {
                            case ("enero"):
                                fechaInicio = new DateTime(fechaInicio.Year, 1, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 1, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("febrero"):
                                fechaInicio = new DateTime(fechaInicio.Year, 2, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 2, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("marzo"):
                                fechaInicio = new DateTime(fechaInicio.Year, 3, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 3, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("abril"):
                                fechaInicio = new DateTime(fechaInicio.Year, 4, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 4, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("mayo"):
                                fechaInicio = new DateTime(fechaInicio.Year, 5, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 5, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("junio"):
                                fechaInicio = new DateTime(fechaInicio.Year, 6, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 6, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("julio"):
                                fechaInicio = new DateTime(fechaInicio.Year, 7, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 7, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("agosto"):
                                fechaInicio = new DateTime(fechaInicio.Year, 8, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 8, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("septiembre"):
                                fechaInicio = new DateTime(fechaInicio.Year, 9, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 9, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("octubre"):
                                fechaInicio = new DateTime(fechaInicio.Year, 10, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 10, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("noviembre"):
                                fechaInicio = new DateTime(fechaInicio.Year, 11, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 11, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("diciembre"):
                                fechaInicio = new DateTime(fechaInicio.Year, 12, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 12, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                        }
                        var datos2 = db.EstatusRequisiciones.Select(e => e.RequisicionId).ToList();
                        var asigna = db.AsignacionRequis.Select(e => e.RequisicionId).ToList();
                        var datos3 = db.Requisiciones.Where(e => datos2.Contains(e.Id) || asigna.Contains(e.Id)).Select(e => e.Id).ToList();
                        var datos = db.Requisiciones.Where(e => datos3.Contains(e.Id) && e.Activo == true && e.Confidencial == false).ToList();
                        int[] EstatusList = new[] { 34, 35, 36, 37, 47, 48 };
                        datos = datos.Where(e => EstatusList.Contains(e.EstatusId) && e.fch_Modificacion > fechaInicio && e.fch_Modificacion <= fechaFinal).ToList();
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
                              coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                              Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                              reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                  db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                ).Distinct().ToList()
                          }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "No Cubiertos")
                    {

                        int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 46 };
                        var datos = db.Requisiciones.Where(e => e.Activo == true && e.Confidencial == false).ToList();
                        datos = datos.Where(e => EstatusList.Contains(e.EstatusId)).ToList();
                        var requien = datos.Select(e => e.Id).ToList();
                        var proseso = db.ProcesoCandidatos.Where(e => requien.Contains(e.RequisicionId)).ToList();
                        var cubierto = proseso.Where(e => e.EstatusId == 24).Select(e => e.RequisicionId).Distinct().ToList();
                        datos = datos.Where(e => !cubierto.Contains(e.Id)).ToList();
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
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "Cubiertos")
                    {
                        int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 46 };
                        var datos = db.Requisiciones.Where(e => e.Activo == true && e.Confidencial == false).ToList();
                        datos = datos.Where(e => EstatusList.Contains(e.EstatusId)).ToList();
                        var requien = datos.Select(e => e.Id).ToList();
                        var proseso = db.ProcesoCandidatos.Where(e => requien.Contains(e.RequisicionId)).ToList();
                        var cubierto = proseso.Where(e => e.EstatusId == 24).Select(e => e.RequisicionId).Distinct().ToList();
                        datos = datos.Where(e => cubierto.Contains(e.Id)).ToList();
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
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    return NotFound();
                    #endregion
                }
                else
                {
                    #region tipo reclu
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
                    }
                   
                    else if (estado == "Masivo")
                    {
                        int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 46 };
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
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "Operativo")
                    {
                        int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39,43,46 };
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
                           coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                            //reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Count() > 0 ? db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                            //    db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            //).Distinct().ToList() : null
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "Especializado")
                    {
                        int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 46 };
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
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "Nuevas" || estado == "Aprobadas" || estado == "Publicadas" || estado == "Búsqueda de candidatos" 
                        || estado == "Envió al cliente" || estado == "Nueva busqueda" || estado == "Socioeconomicos" || estado == "En espera de contratación" ||
                        estado == "Pausadas" || estado == "Garantía de búsqueda" || estado == "Pendiente" || estado == "PendienteGG")
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
                        valor = estado == "Pendiente" ? 43 : valor;
                        valor = estado == "PendienteGG" ? 46 : valor;
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
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "Folios" || estado == "Posiciones" || estado == "Cubiertas" || estado == "Faltantes")
                    {
                        int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 46 };
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || uids.Contains(e.AprobadorId) && e.Activo == true && e.Confidencial == false).ToList();
                        datos = datos.Where(e => EstatusList.Contains(e.EstatusId)).ToList();
                        if (estado == "Cubiertas")
                        {
                            var requien = datos.Select(e => e.Id).ToList();
                            var proseso = db.ProcesoCandidatos.Where(e => requien.Contains(e.RequisicionId)).ToList();
                            var cubierto = proseso.Where(e => e.EstatusId == 24).Select(e=>e.RequisicionId).Distinct().ToList();
                            datos = datos.Where(e => cubierto.Contains(e.Id)).ToList();
                        }

                        if (estado == "Faltantes")
                        {
                            var requien = datos.Select(e => e.Id).ToList();
                            var proseso = db.ProcesoCandidatos.Where(e => requien.Contains(e.RequisicionId)).ToList();
                            var cubierto = proseso.Where(e => e.EstatusId == 24).Select(e => e.RequisicionId).Distinct().ToList();
                            datos = datos.Where(e => !cubierto.Contains(e.Id)).ToList();
                            //var requien = datos.Select(e => e.Id).ToList();
                            //var proseso = db.ProcesoCandidatos.Where(e => requien.Contains(e.RequisicionId)).ToList();
                            //var posicion = db.HorariosRequis.Where(e => requien.Contains(e.RequisicionId)).Select(e => new { e.RequisicionId, e.numeroVacantes }).ToList();
                            //var cubiertorequi = proseso.Where(e => e.EstatusId == 24).ToList();
                            //var numeroRequien = posicion.Select(e => new { e.RequisicionId, e.numeroVacantes, cubierto = cubiertorequi.Where(x => x.RequisicionId == e.RequisicionId).Count() }).ToList();
                            //var cubierto = numeroRequien.Where(e => e.numeroVacantes > e.cubierto).Select(e => e.RequisicionId).ToList();
                        }
                        var requisicion = datos
                        .Where(e => EstatusList.Contains(e.EstatusId))
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
                            porsentaje = e.horariosRequi.Sum(s => s.numeroVacantes) > 0 ? (db.ProcesoCandidatos.Where(p => p.RequisicionId == e.Id && p.EstatusId == 24).Count()) * 100 / e.horariosRequi.Sum(s => s.numeroVacantes) : 0,
                            Confidencial = e.Confidencial,
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        if (estado == "Posiciones" || estado == "Faltantes")
                        {
                            requisicion = requisicion.OrderByDescending(e => e.Vacantes).ToList();
                        }
                        if (estado == "Cubiertas")
                        {
                            requisicion = requisicion.OrderBy(e => e.porsentaje).ToList();
                        }

                        return Ok(requisicion);
                    }
                    else if (estado == "diciembre" || estado == "noviembre" || estado == "octubre" || estado == "septiembre" || estado == "agosto"
                        || estado == "julio" || estado == "junio" || estado == "mayo" || estado == "abril" || estado == "marzo" 
                        || estado == "febrero" || estado == "enero")
                    {
                        DateTime fechaInicio = DateTime.Now;
                        DateTime fechaFinal = DateTime.Now;
                        if (estado == "diciembre" || estado == "noviembre" || estado == "octubre" || estado == "septiembre" || estado == "agosto"
                        || estado == "julio")
                        {
                            if (fechaInicio.Month < 6)
                            {
                                int valor = fechaInicio.Year - 1;
                                fechaInicio = new DateTime(valor, 1, 1);
                            }
                        }
                        switch (estado)
                        {
                            case("enero"):
                                fechaInicio = new DateTime(fechaInicio.Year, 1, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 1, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("febrero"):
                                fechaInicio = new DateTime(fechaInicio.Year, 2, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 2, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("marzo"):
                                fechaInicio = new DateTime(fechaInicio.Year, 3, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 3, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("abril"):
                                fechaInicio = new DateTime(fechaInicio.Year, 4, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 4, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("mayo"):
                                fechaInicio = new DateTime(fechaInicio.Year,5, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 5, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("junio"):
                                fechaInicio = new DateTime(fechaInicio.Year, 6, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 6, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("julio"):
                                fechaInicio = new DateTime(fechaInicio.Year, 7, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 7, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("agosto"):
                                fechaInicio = new DateTime(fechaInicio.Year, 8, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 8, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("septiembre"):
                                fechaInicio = new DateTime(fechaInicio.Year, 9, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 9, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("octubre"):
                                fechaInicio = new DateTime(fechaInicio.Year, 10, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 10, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("noviembre"):
                                fechaInicio = new DateTime(fechaInicio.Year, 11, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 11, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("diciembre"):
                                fechaInicio = new DateTime(fechaInicio.Year, 12, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 12, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                        }
                        var datos2 = db.EstatusRequisiciones.Where(e => uids.Contains(e.PropietarioId)).Select(e => e.RequisicionId).ToList();
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos3 = db.Requisiciones.Where(e => datos2.Contains(e.Id) || asigna.Contains(e.Id)).Select(e => e.Id).ToList();
                        var datos = db.Requisiciones.Where(e => datos3.Contains(e.Id) && e.Activo == true && e.Confidencial == false).ToList();
                        int[] EstatusList = new[] { 34, 35, 36, 37, 47, 48 };
                        datos = datos.Where(e => EstatusList.Contains(e.EstatusId) && e.fch_Modificacion > fechaInicio && e.fch_Modificacion <= fechaFinal).ToList();
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
                              coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                              Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                              reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                  db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                ).Distinct().ToList()
                          }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "No Cubiertos")
                    {

                        int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 46 };
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || uids.Contains(e.AprobadorId) && e.Activo == true && e.Confidencial == false).ToList();
                        datos = datos.Where(e => EstatusList.Contains(e.EstatusId)).ToList();
                        var requien = datos.Select(e => e.Id).ToList();
                        var proseso = db.ProcesoCandidatos.Where(e => requien.Contains(e.RequisicionId)).ToList();
                        var cubierto = proseso.Where(e => e.EstatusId == 24).Select(e => e.RequisicionId).Distinct().ToList();
                        datos = datos.Where(e => !cubierto.Contains(e.Id)).ToList();
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
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "Cubiertos")
                    {
                        int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 46 };
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos = db.Requisiciones.Where(e => asigna.Contains(e.Id) || uids.Contains(e.AprobadorId) && e.Activo == true && e.Confidencial == false).ToList();
                        datos = datos.Where(e => EstatusList.Contains(e.EstatusId)).ToList();
                        var requien = datos.Select(e => e.Id).ToList();
                        var proseso = db.ProcesoCandidatos.Where(e => requien.Contains(e.RequisicionId)).ToList();
                        var cubierto = proseso.Where(e => e.EstatusId == 24).Select(e => e.RequisicionId).Distinct().ToList();
                        datos = datos.Where(e => cubierto.Contains(e.Id)).ToList();
                        
                        //List<Guid> faltante = new List<Guid>();
                        //foreach (var item in datos)
                        //{
                        //    int numeropos = db.HorariosRequis.Where(x => x.RequisicionId == item.Id).Sum(x => x.numeroVacantes);
                        //    int cubierto = db.ProcesoCandidatos.Where(e => e.EstatusId == 24 && e.RequisicionId == item.Id).ToList().Count;
                        //    if (numeropos == cubierto)
                        //    {
                        //        faltante.Add(item.Id);
                        //    }
                        //}
                       // datos = datos.Where(e => faltante.Contains(e.Id)).ToList();
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
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "A un vigentes" || estado == "Ya vencidas")
                    {
                       
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 46 };
                        var requi = db.Requisiciones.Where(e => asigna.Contains(e.Id) || uids.Contains(e.AprobadorId) && e.Activo == true && e.Confidencial == false).ToList();
                        requi = requi.Where(e => EstatusList.Contains(e.EstatusId)).ToList();
                      
                        var datos = requi.Where(e => e.fch_Cumplimiento > DateTime.Now.AddDays(-1)).ToList();
                        if (estado == "Ya vencidas")
                        {
                            datos = requi.Where(e => e.fch_Cumplimiento < DateTime.Now).ToList();
                        }
                        
                        var requisicion = datos
                        .Select(e => new
                        {
                            Id = e.Id,
                            Folio = e.Folio,
                            fch_Creacion = e.fch_Creacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            fch_Asignacion = e.fch_Aprobacion,
                            fch_Modificacion = e.fch_Modificacion,
                            e.ClaseReclutamiento.clasesReclutamiento,
                            dias = DateTime.Now.Date.Subtract(DateTime.Parse(e.fch_Aprobacion.ToString()).Date).Days,
                            Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                            VBtra = e.VBtra.ToUpper(),
                            Estatus = e.Estatus.Descripcion.ToUpper(),
                            EstatusId = e.EstatusId,
                            EstatusOrden = e.Estatus.Orden,
                            Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count(),
                            Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                            Confidencial = e.Confidencial,
                            coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    else if (estado == "diciembre1" || estado == "noviembre1" || estado == "octubre1" || estado == "septiembre1" || estado == "agosto1"
                     || estado == "julio1" || estado == "junio1" || estado == "mayo1" || estado == "abril1" || estado == "marzo1"
                     || estado == "febrero1" || estado == "enero1")
                    {
                        estado = estado.TrimEnd('1');
                    //    estado = estado.Substring(estado.Length, -1);
                        DateTime fechaInicio = DateTime.Now;
                        DateTime fechaFinal = DateTime.Now;
                        if (estado == "diciembre" || estado == "noviembre" || estado == "octubre" || estado == "septiembre" || estado == "agosto"
                        || estado == "julio")
                        {
                            if (fechaInicio.Month < 6)
                            {
                                int valor = fechaInicio.Year - 1;
                                fechaInicio = new DateTime(valor, 1, 1);
                            }
                        }
                        switch (estado)
                        {
                            case ("enero"):
                                fechaInicio = new DateTime(fechaInicio.Year, 1, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 1, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("febrero"):
                                fechaInicio = new DateTime(fechaInicio.Year, 2, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 2, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("marzo"):
                                fechaInicio = new DateTime(fechaInicio.Year, 3, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 3, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("abril"):
                                fechaInicio = new DateTime(fechaInicio.Year, 4, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 4, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("mayo"):
                                fechaInicio = new DateTime(fechaInicio.Year, 5, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 5, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("junio"):
                                fechaInicio = new DateTime(fechaInicio.Year, 6, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 6, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("julio"):
                                fechaInicio = new DateTime(fechaInicio.Year, 7, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 7, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("agosto"):
                                fechaInicio = new DateTime(fechaInicio.Year, 8, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 8, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("septiembre"):
                                fechaInicio = new DateTime(fechaInicio.Year, 9, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 9, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("octubre"):
                                fechaInicio = new DateTime(fechaInicio.Year, 10, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 10, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("noviembre"):
                                fechaInicio = new DateTime(fechaInicio.Year, 11, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 11, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                            case ("diciembre"):
                                fechaInicio = new DateTime(fechaInicio.Year, 12, 1);
                                fechaFinal = new DateTime(fechaInicio.Year, 12, fechaInicio.AddMonths(1).AddDays(-1).Day);
                                break;
                        }
                        var datos2 = db.EstatusRequisiciones.Where(e => uids.Contains(e.PropietarioId)).Select(e => e.RequisicionId).ToList();
                        var asigna = db.AsignacionRequis.Where(e => uids.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        var datos3 = db.Requisiciones.Where(e => datos2.Contains(e.Id) || asigna.Contains(e.Id)).Select(e => e.Id).ToList();
                        var datos = db.Requisiciones.Where(e => datos3.Contains(e.Id) && e.Activo == true && e.Confidencial == false).ToList();
                        int[] EstatusList = new[] { 34, 35, 36, 37, 47, 48 };
                        datos = datos.Where(e => EstatusList.Contains(e.EstatusId) && e.fch_Modificacion > fechaInicio && e.fch_Modificacion <= fechaFinal).ToList();
                        var requisicion = datos
                         .Select(e => new
                         {
                             Id = e.Id,
                             Folio = e.Folio,
                             fch_Creacion = e.fch_Creacion,
                             fch_Cumplimiento = e.fch_Cumplimiento,
                             fch_Asignacion = e.fch_Aprobacion,
                             fch_Modificacion = e.fch_Modificacion,
                             e.ClaseReclutamiento.clasesReclutamiento,
                             dias = (DateTime.Parse(e.fch_Modificacion.ToString()).Date.Subtract(DateTime.Parse(e.fch_Aprobacion.ToString()).Date).Days) < 0?0: DateTime.Parse(e.fch_Modificacion.ToString()).Date.Subtract(DateTime.Parse(e.fch_Aprobacion.ToString()).Date).Days,
                             Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                             VBtra = e.VBtra.ToUpper(),
                             Estatus = e.Estatus.Descripcion.ToUpper(),
                             EstatusId = e.EstatusId,
                             EstatusOrden = e.Estatus.Orden,
                             Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count(),
                             Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                             Confidencial = e.Confidencial,
                             coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Count() == 0 ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                             Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                             reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                                 db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                             ).Distinct().ToList()
                        }).OrderBy(x => x.fch_Cumplimiento).ToList();
                        return Ok(requisicion);
                    }
                    return NotFound();
                    #endregion
                }

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        
    }
}
