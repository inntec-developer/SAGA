using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using SAGA.API.Utilerias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.Entity;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Requisiciones")]
    public class RequisicionesController : ApiController
    {
        private SAGADBContext db;
        private Damfo290Dto DamfoDto;
        private BusinessDay businessDay;
        private Rastreabilidad rastreabilidad;
        private SendEmails SendEmail;

        public RequisicionesController()
        {
            db = new SAGADBContext();
            DamfoDto = new Damfo290Dto();
            businessDay = new BusinessDay();
            rastreabilidad = new Rastreabilidad();
            SendEmail = new SendEmails();
        }

        [HttpGet]
        [Route("getAddress")]
        public IHttpActionResult GetAddress(Guid Id)
        {
            try
            {
                DamfoDto.Damfo290Address = (from damfo in db.DAMFO290
                                            join cliente in db.Clientes on damfo.ClienteId equals cliente.Id
                                            join persona in db.Entidad on cliente.Id equals persona.Id
                                            join direccion in db.Direcciones on persona.Id equals direccion.EntidadId
                                            join tipoDireccion in db.TiposDirecciones on direccion.TipoDireccionId equals tipoDireccion.Id
                                            join pais in db.Paises on direccion.PaisId equals pais.Id
                                            join estado in db.Estados on direccion.EstadoId equals estado.Id
                                            join municipio in db.Municipios on direccion.MunicipioId equals municipio.Id
                                            join colonia in db.Colonias on direccion.ColoniaId equals colonia.Id
                                            where damfo.Id == Id && direccion.Activo == true
                                            select new Damfo290AddressDto
                                            {
                                                Id = direccion.Id,
                                                TipoDireccion = tipoDireccion.tipoDireccion,
                                                Pais = pais.pais,
                                                Estado = estado.estado,
                                                Municipio = municipio.municipio,
                                                Colonia = colonia.colonia,
                                                Calle = direccion.Calle + " " + direccion.NumeroExterior + " C.P: " + direccion.CodigoPostal + " Col: " + colonia.colonia,
                                                CodigoPostal = direccion.CodigoPostal,
                                                NumeroExterior = direccion.NumeroExterior,
                                                NumeroInterior = direccion.NumeroInterior

                                            }).ToList();
                return Ok(DamfoDto.Damfo290Address);

            }
            catch (Exception ex)
            {
                string messg = ex.Message;
                return Ok(messg);
            }
        }

        [HttpGet]
        [Route("getById")]
        public IHttpActionResult GetRequisicion(Guid Id)
        {
            if (Id != null)
            {
                var requisicion = db.Requisiciones.FirstOrDefault(x => x.Id.Equals(Id));
                return Ok(requisicion);
            }
            else
            {
                return NotFound();
            }

        }

        [HttpGet]
        [Route("getByFolio")]
        public IHttpActionResult GetRequisicionFolio(Int64 folio)
        {
            if (folio != 0)
            {

                var requisicion = db.Requisiciones.Where(x => x.Folio.Equals(folio)).Select(r => new {
                    r.Id,
                    r.Folio,
                    r.fch_Cumplimiento,
                    r.fch_Creacion,
                    r.fch_Limite,
                    r.Prioridad,
                    r.Confidencial,
                    r.Estatus,
                    asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id)).Select(x => x.GrpUsrId).ToList(),
                    asignadosN = r.AsignacionRequi.Where(x => x.RequisicionId.Equals(r.Id)).Select(x => new {
                        x.GrpUsr.Nombre,
                        x.GrpUsr.ApellidoMaterno,
                        x.GrpUsr.ApellidoPaterno
                    }),
                    vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                    r.VBtra,
                    HorariosDamfo = db.HorariosPerfiles.Where(h => h.DAMFO290Id.Equals(r.DAMFO290Id)).ToList()
                }).FirstOrDefault();
                return Ok(requisicion);
            }
            else
            {
                return NotFound();
            }

        }

        [HttpPost]
        [Route("createRequi")]
        public IHttpActionResult Clon(CreateRequiDto cr)
        {
            try
            {
                object[] _params = {
                    new SqlParameter("@Id", cr.IdDamfo),
                    new SqlParameter("@IdAddress", cr.IdAddress),
                    new SqlParameter("@UserAlta", cr.Usuario),
                    new SqlParameter("@UsuarioId", cr.UsuarioId)
                };

                var requi = db.Database.SqlQuery<Requisicion>("exec createRequisicion @Id, @IdAddress, @UserAlta, @UsuarioId ", _params).SingleOrDefault();

                Guid RequisicionId = requi.Id;
                Int64 Folio = requi.Folio;


                return Ok(requi);
            }
            catch (Exception ex)
            {
                string messg = ex.Message;
                return Ok(messg);
            }

        }

        [HttpGet]
        [Route("getRequisiciones")]
        public IHttpActionResult GetRequisiciones(Guid propietario)
        {
            var requisicion = db.Requisiciones
                .Where(e => e.Activo.Equals(true) && e.PropietarioId.Equals(propietario))
                .Select(e => new
                {
                    Id = e.Id,
                    VBtra = e.VBtra,
                    TipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento,
                    tipoReclutamientoId = e.TipoReclutamientoId,
                    ClaseReclutamiento = e.ClaseReclutamiento.clasesReclutamiento,
                    ClaseReclutamientoId = e.ClaseReclutamientoId,
                    SueldoMinimo = e.SueldoMinimo,
                    SueldoMaximo = e.SueldoMaximo,
                    fch_Creacion = e.fch_Creacion,
                    fch_Cumplimiento = e.fch_Cumplimiento,
                    Estatus = e.Estatus.Descripcion,
                    EstatusId = e.EstatusId,
                    Prioridad = e.Prioridad.Descripcion,
                    PrioridadId = e.PrioridadId,
                    Cliente = e.Cliente.Nombrecomercial,
                    GiroEmpresa = e.Cliente.GiroEmpresas.giroEmpresa,
                    ActividadEmpresa = e.Cliente.ActividadEmpresas.actividadEmpresa,
                    Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                    Folio = e.Folio,
                    DiasEnvio = e.DiasEnvio,
                    Confidencial = e.Confidencial,
                    Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) & p.StatusId.Equals(1)).Count(),
                    PostuladosN = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) & p.StatusId.Equals(1)).Select(p => new
                    {
                        p.CandidatoId,
                        p.Candidato.Nombre,
                        p.Candidato.ApellidoPaterno,
                        p.Candidato.ApellidoMaterno,
                        p.Candidato.CURP,
                        email = p.Candidato.emails.Select(m => m.email).FirstOrDefault(),
                        p.StatusId,
                        //estatusId = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(pp => pp.RequisicionId.Equals(e.Id) && pp.CandidatoId.Equals(p.CandidatoId) && pp.EstatusId != 24 && pp.EstatusId != 27 && pp.EstatusId != 40 && pp.EstatusId != 28 && pp.EstatusId != 42).Select(d => d.EstatusId).FirstOrDefault()
                    }),
                    EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Count(),
                    EnProcesoN = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 24 && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Select(d => new
                    {
                        candidatoId = d.CandidatoId,
                        nombre = db.Candidatos.Where(x => x.Id.Equals(d.CandidatoId)).Select( cc => cc.Nombre + " " + cc.ApellidoPaterno + " " + cc.ApellidoMaterno).FirstOrDefault(),
                        email = db.Emails.Where(x => x.EntidadId.Equals(d.CandidatoId)).Select(m => m.email).FirstOrDefault(),
                        estatusId = d.EstatusId
                    }),
                    Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count()
                }).ToList().OrderByDescending(x => x.Folio);
            return Ok(requisicion);
        }

        [HttpGet]
        [Route("getRequisicionesEstatus")]
        public IHttpActionResult GetRequisicionesEstatus(int estatus, Guid ReclutadorId)
        {
            try
            {
                var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                    .Where(e => e.Activo.Equals(true) && e.Estatus.Id.Equals(estatus))
                    .Select(e => new
                    {
                        Id = e.Id,
                        Folio = e.Folio,
                        VBtra = e.VBtra,
                        Estatus = e.Estatus.Descripcion,
                        EstatusId = e.EstatusId,
                        Reclutador = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Clave + " " + s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault(),
                        EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 42).Count(),
                        EnProcesoEC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 22).Count(),
                        EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 23).Count(),
                        contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count(),
                        ComentarioReclutador = db.ComentariosVacantes.OrderByDescending(f => f.fch_Creacion).Where(x => x.RequisicionId.Equals(e.Id) && x.Motivo.EstatusId.Equals(39)).Select(c => new {
                            id = c.Id,
                            folio = db.FolioIncidencia.Where(x => x.ComentarioId.Equals(c.Id)).Select(f => f.Folio).FirstOrDefault(),
                            fecha = c.fch_Creacion,
                            motivo = c.Motivo.Descripcion,
                            comentario = c.Comentario,
                            respuesta = String.IsNullOrEmpty(db.ComentariosVacantes.Where(x => x.RespuestaId.Equals(c.Id)).Select(r => r.fch_Creacion + " - " + db.Usuarios.Where(x => x.Id.Equals(r.ReclutadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault() + " - " + r.Comentario).FirstOrDefault()) ? "Doble click para editar" : db.ComentariosVacantes.Where(x => x.RespuestaId.Equals(c.Id)).Select(r => r.fch_Creacion + " - " + db.Usuarios.Where(x => x.Id.Equals(r.ReclutadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault() + " - " + r.Comentario).FirstOrDefault()
                        }).FirstOrDefault()
                    }).ToList();

                return Ok(vacantes);

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getRequiReclutador")]
        public IHttpActionResult GtRequiReclutador(Guid IdUsuario)
        {
            try
            {
                List<Guid> grp = new List<Guid>();

                var Grupos = db.GruposUsuarios
                    .Where(g => g.EntidadId.Equals(IdUsuario) & g.Grupo.Activo)
                           .Select(g => g.GrupoId)
                           .ToList();


                foreach (var grps in Grupos)
                {
                    grp = GetGrupo(grps, grp);
                }

                grp.Add(IdUsuario);

                var asig = db.AsignacionRequis
                    .OrderByDescending(e => e.Id)
                    .Where(a => grp.Distinct().Contains(a.GrpUsrId))
                    .Select(a => a.RequisicionId)
                    .Distinct()
                    .ToList();

                var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                    .Where(e => asig.Contains(e.Id))
                    .Where(e => e.Activo.Equals(true))
                    .Select(e => new
                    {
                        Id = e.Id,
                        VBtra = e.VBtra,
                        TipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento,
                        tipoReclutamientoId = e.TipoReclutamientoId,
                        ClaseReclutamiento = e.ClaseReclutamiento.clasesReclutamiento,
                        ClaseReclutamientoId = e.ClaseReclutamientoId,
                        SueldoMinimo = e.SueldoMinimo,
                        SueldoMaximo = e.SueldoMaximo,
                        fch_Creacion = e.fch_Creacion,
                        fch_Modificacion = e.fch_Modificacion,
                        fch_Cumplimiento = e.fch_Cumplimiento,
                        Estatus = e.Estatus.Descripcion,
                        EstatusId = e.EstatusId,
                        Prioridad = e.Prioridad.Descripcion,
                        PrioridadId = e.PrioridadId,
                        Cliente = e.Cliente.Nombrecomercial,
                        ClienteId = e.Cliente.Id,
                        Sucursal = db.Direcciones.Where(x => x.Id.Equals(e.DireccionId)).Select(d => d.Calle + " " + d.NumeroExterior + " C.P: " + d.CodigoPostal + " Col: " + d.Colonia.colonia).FirstOrDefault(),
                        Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        Folio = e.Folio,
                        DiasEnvio = e.DiasEnvio,
                        Confidencial = e.Confidencial,
                        //asignados = e.AsignacionRequi.Select(a => a.GrpUsrId).ToList(),
                        Asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id)).Select(x => x.GrpUsrId).ToList(),
                        Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) && p.StatusId.Equals(1)).Select(c => c.CandidatoId).Count(),
                        EnProceso = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40).Count(),
                        EnProcesoFR = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 21).Count(),
                        EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 23).Count(),
                        contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count(),
                        Solicita = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault(),
                        AreaExperiencia = e.Area.areaExperiencia,
                        Aprobador = e.Aprobador != null ? e.Aprobador : "",
                        ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select( c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList()
                    }).ToList();
                return Ok(vacantes);

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        [HttpGet]
        [Route("getRequiEstadisticos")]
        public IHttpActionResult GetRequiEstadisticos(Guid IdUsuario)
        {
            try
            {
                List<Guid> grp = new List<Guid>();

                var Grupos = db.GruposUsuarios
                    .Where(g => g.EntidadId.Equals(IdUsuario) & g.Grupo.Activo)
                           .Select(g => g.GrupoId)
                           .ToList();


                foreach (var grps in Grupos)
                {
                    grp = GetGrupo(grps, grp);
                }

                grp.Add(IdUsuario);

                var asig = db.AsignacionRequis
                    .OrderByDescending(e => e.Id)
                    .Where(a => grp.Distinct().Contains(a.GrpUsrId))
                    .Select(a => a.RequisicionId)
                    .Distinct()
                    .ToList();

                var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                    .Where(e => asig.Contains(e.Id))
                    .Where(e => e.Activo.Equals(true))
                    .Select(e => new
                    {
                        Id = e.Id,
                        VBtra = e.VBtra,
                        TipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento,
                        tipoReclutamientoId = e.TipoReclutamientoId,
                        ClaseReclutamiento = e.ClaseReclutamiento.clasesReclutamiento,
                        ClaseReclutamientoId = e.ClaseReclutamientoId,
                        SueldoMinimo = e.SueldoMinimo,
                        SueldoMaximo = e.SueldoMaximo,
                        fch_Creacion = e.fch_Creacion,
                        fch_Cumplimiento = e.fch_Cumplimiento,
                        Estatus = e.Estatus.Descripcion,
                        EstatusId = e.EstatusId,
                        Prioridad = e.Prioridad.Descripcion,
                        PrioridadId = e.PrioridadId,
                        Cliente = e.Cliente.Nombrecomercial,
                        ClienteId = e.Cliente.Id,
                        Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        Folio = e.Folio,
                        DiasEnvio = e.DiasEnvio,
                        Confidencial = e.Confidencial,
                        //asignados = e.AsignacionRequi.Select(a => a.GrpUsrId).ToList(),
                        Asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id)).Select(x => x.GrpUsrId).ToList(),
                        Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) && p.StatusId.Equals(1)).Select(c => c.CandidatoId).Except(db.ProcesoCandidatos.Where(xx => xx.RequisicionId.Equals(e.Id) && xx.EstatusId.Equals(27) || xx.EstatusId.Equals(40)).Select(cc => cc.CandidatoId)).Count(),
                        EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40).Count(),
                        EnProcesoFR = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 21).Count(),
                        EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 23).Count(),
                        contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count(),
                        Solicita = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault(),
                        AreaExperiencia = e.Area.areaExperiencia,
                        Aprobador = e.Aprobador != null ? e.Aprobador : "",
                    }).ToList();
                return Ok(vacantes);

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        [HttpGet]
        [Route("getConteoVacante")]
        public IHttpActionResult GetConteoVacante(Guid RequisicionId, Guid ClienteId)
        {
            try
            {
                var horarios = db.HorariosRequis.Where(x => x.RequisicionId.Equals(RequisicionId) && x.numeroVacantes > 0).Select(h => new
                {
                    Id = h.Id,
                    Vacantes = h.numeroVacantes,
                    Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(RequisicionId) && p.StatusId.Equals(1)).Select(c => c.CandidatoId).Except(db.ProcesoCandidatos.Where(xx => xx.RequisicionId.Equals(RequisicionId) && xx.EstatusId.Equals(27) || xx.EstatusId.Equals(40)).Select(cc => cc.CandidatoId)).Count(),
                    Apartados = db.ProcesoCandidatos.Where(p => p.HorarioId.Equals(h.Id) && p.EstatusId.Equals(12)).Count(),
                    Abandono = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 26).Count(),
                    Descartados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 27).Count(),
                    EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId != 27 && p.EstatusId != 40).Count(),
                    EnProcesoEnt = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 18).Count(),
                    EnProcesoFR = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 21).Count(),
                    EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 23).Count(),
                    contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 24).Count(),
                    Enviados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId.Equals(30) && p.EstatusId == 21).Count(),
                    EntCliente = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 22).Count(),
                    rechazados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 40).Count(),
                    porcentaje = h.numeroVacantes > 0 ? (db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 24).Count()) * 100 / h.numeroVacantes : 0,
                    horario = h.Nombre + " de " + h.deHora.Hour + " a " + h.aHora.Hour,
                    totalVacantes = db.HorariosRequis.Where(x => x.RequisicionId.Equals(RequisicionId) && x.numeroVacantes > 0).Sum(v => v.numeroVacantes)
                }).ToList();


                //var vacantes = db.Requisiciones   horario = h.aHora.Hour - h.deHora.Hour == 9 ? "Completo de " + h.deHora.Hour + " a " + h.aHora.Hour : h.deHora.Hour > 12 ? "Vespertino de " + h.deHora.Hour + " a " + h.aHora.Hour : "Matutino de " + h.deHora.Hour + " a " + h.aHora.Hour
                //    .Where(e => e.Id.Equals(RequisicionId))
                //    .Select(e => new
                //    {
                //        Id = e.Id,
                //        ClienteId = e.Cliente.Id,
                //        Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                //        Folio = e.Folio,
                //        Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) && p.StatusId.Equals(1)).Select(c => c.CandidatoId).Except(db.ProcesoCandidatos.Where(xx => xx.RequisicionId.Equals(e.Id) && xx.EstatusId.Equals(27) || xx.EstatusId.Equals(40)).Select(cc => cc.CandidatoId)).Count(),
                //        Apartados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 12).Count(),
                //        Abandono = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 26).Count(),
                //        Descartados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 27).Count(),
                //        EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40).Count(),
                //        EnProcesoEnt = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 18).Count(),
                //        EnProcesoFR = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 21).Count(),
                //        EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 23).Count(),
                //        contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count(),
                //        Enviados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && e.EstatusId.Equals(30) && p.EstatusId == 21).Count(),
                //        EntCliente = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 22).Count(),
                //        rechazados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 40).Count(),
                //        porcentaje = (db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count()) * 100 / (e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0),
                //        horario = db.HorariosRequis.Where(x => x.RequisicionId.Equals(e.Id)).Select(h => (h.aHora.Hour - h.deHora.Hour) == 9 ? "Completo de " + h.deHora.Hour + " a " + h.aHora.Hour : h.aHora.Hour > 12 ? "Vespertino de " + h.deHora.Hour + " a " + h.aHora.Hour : "Matutino de " + h.deHora.Hour + " a " + h.aHora.Hour)
                //    }).ToList();
                return Ok(horarios);

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        [HttpGet]
        [Route("getInformeVacantes")]
        public IHttpActionResult GetInformeVacantes(Guid reclutadorId)
        {
            try
            {
                List<Guid> grp = new List<Guid>();

                var Grupos = db.GruposUsuarios
                    .Where(g => g.EntidadId.Equals(reclutadorId) & g.Grupo.Activo)
                           .Select(g => g.GrupoId)
                           .ToList();


                foreach (var grps in Grupos)
                {
                    grp = GetGrupo(grps, grp);
                }

                grp.Add(reclutadorId);

                var asig = db.AsignacionRequis
                    .OrderByDescending(e => e.Id)
                    .Where(a => grp.Distinct().Contains(a.GrpUsrId))
                    .Select(a => a.RequisicionId)
                    .Distinct()
                    .ToList();

                var informe = db.Requisiciones.OrderByDescending(f => f.fch_Cumplimiento).Where(e => asig.Contains(e.Id))
                    .Where(e => e.Activo.Equals(true) && e.EstatusId != 9).Select(h => new
                    {
                        Id = h.Id,
                        Folio = h.Folio,
                        vBtra = h.VBtra,
                        Estatus = h.Estatus.Descripcion,
                        EstatusId = h.EstatusId,
                        cliente = h.Cliente.Nombrecomercial,
                        Vacantes = h.horariosRequi.Count() > 0 ? h.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                        Fch_limite = h.fch_Cumplimiento,
                        Postulados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId.Equals(10)).Select(c => c.CandidatoId).Distinct().Count(),
                        Abandono = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 26).Count(),
                        Descartados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 27).Count(),
                        EnProceso = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28).Count(),
                        entrevista = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 18).Count(),
                        //EnProcesoFR = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 21).Count(),
                        //EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 23).Count(),
                        contratados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count(),
                        Enviados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId.Equals(22)).Count(),
                        //EntCliente = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 22).Count(),
                        rechazados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 40).Count(),
                        porcentaje = h.horariosRequi.Count > 0 ? (db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count()) * 100 / h.horariosRequi.Sum(s => s.numeroVacantes) : 0,
                    }).ToList();

                return Ok(informe);

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        [HttpGet]
        [Route("getUltimoEstatus")]
        public IHttpActionResult GetUltimoEstatus(Guid RequisicionId)
        {
            try
            {
                var estatus = db.EstatusRequisiciones.OrderByDescending(x => x.fch_Modificacion).Where(x => x.RequisicionId.Equals(RequisicionId) && x.EstatusId != 39).Select(e => new
                {
                    e.EstatusId,
                    e.Estatus.Descripcion
                }).FirstOrDefault();

                return Ok(estatus);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getDireccionRequisicon")]
        public IHttpActionResult GetDireccionRequisicon(Guid Id)
        {
            try
            {
                var direccion = db.Direcciones.Where(x => x.Id.Equals(Id))
                    .Select(d => new {
                        TipoDireccion = d.TipoDireccion.tipoDireccion,
                        Pais = d.Pais.pais,
                        Estado = d.Estado.estado,
                        Municipio = d.Municipio.municipio,
                        Colonia = d.Colonia.colonia,
                        Calle = d.Calle,
                        NumeroExterior = d.NumeroExterior,
                        NumeroInterior = d.NumeroInterior != null ? d.NumeroInterior : "S/N",
                        Activo = d.Activo,
                        Principal = d.esPrincipal,
                        RutasCamion = db.RutasPerfil
                                        .Where(r => r.DireccionId.Equals(d.Id))
                                        .Select(r => new {
                                            Ruta = r.Ruta,
                                            Via = r.Via
                                        }).ToList()
                    })
                    .ToList();
                return Ok(direccion);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("getRutasCamion")]
        public IHttpActionResult GetRutasCamion(Guid Id)
        {
            try
            {
                var rutasCamion = db.RutasPerfil
                    .Where(r => r.DireccionId.Equals(Id))
                    .Select(x => new
                    {
                        Id = x.Id,
                        DireccionId = x.DireccionId,
                        Ruta = x.Ruta,
                        Via = x.Via
                    }).ToList();
                return Ok(rutasCamion);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("addRutaCamion")]
        public IHttpActionResult AddRutasCamion(RutaCamionDto ruta)
        {
            try
            {
                var rutaCamion = new RutasPerfil();
                rutaCamion.DireccionId = ruta.DireccionId;
                rutaCamion.Ruta = ruta.Ruta;
                rutaCamion.Via = ruta.Via;
                rutaCamion.UsuarioAlta = ruta.Usuario;
                db.RutasPerfil.Add(rutaCamion);
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("updateRutaCamion")]
        public IHttpActionResult UpdateRutasCamion(RutaCamionDto ruta)
        {
            try
            {
                var rutaCamion = db.RutasPerfil.Find(ruta.Id);
                db.Entry(rutaCamion).State = EntityState.Modified;
                rutaCamion.Ruta = ruta.Ruta;
                rutaCamion.Via = ruta.Via;
                rutaCamion.UsuarioMod = ruta.Usuario;
                rutaCamion.fch_Modificacion = DateTime.Now;
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("deleteRutaCamion")]
        public IHttpActionResult DeleteRutasCamion(RutaCamionDto ruta)
        {
            try
            {
                RutasPerfil rutaCamion = (RutasPerfil)db.RutasPerfil.Find(ruta.Id);
                db.RutasPerfil.Remove(rutaCamion);
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("upadateVacantes")]
        public IHttpActionResult UpdateVacantes(HorariosRequi horario)
        {
            try
            {
                if(horario.numeroVacantes == 0)
                {
                    var vacante = db.HorariosRequis
                                    .Where(h => h.RequisicionId.Equals(horario.RequisicionId) && h.Id != horario.Id)
                                    .Select(h => new
                                    {
                                        vacantes = h.numeroVacantes
                                    }).ToList();
                    var suma = vacante.Count() > 0 ? vacante.Sum(s => s.vacantes) : 0;

                    if(suma == 0)
                    {
                        return Ok(HttpStatusCode.NoContent);
                    }
                    else
                    {
                        var hr = db.HorariosRequis.Find(horario.Id);
                        db.Entry(hr).State = EntityState.Modified;
                        hr.numeroVacantes = horario.numeroVacantes;
                        hr.UsuarioMod = horario.Usuario;
                        hr.fch_Modificacion = DateTime.Now;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    }

                }else
                {
                    var hr = db.HorariosRequis.Find(horario.Id);
                    db.Entry(hr).State = EntityState.Modified;
                    hr.numeroVacantes = horario.numeroVacantes;
                    hr.UsuarioMod = horario.Usuario;
                    hr.fch_Modificacion = DateTime.Now;
                    db.SaveChanges();
                    return Ok(HttpStatusCode.OK);
                }
            }
            catch (Exception)
            {
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getHorariosRequisicion")]
        public IHttpActionResult GetHorariosRequisicion(Guid Id)
        {
            try
            {
                var horarios = db.HorariosRequis.Where(x => x.RequisicionId.Equals(Id)).ToList();
                return Ok(horarios);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getHorariosRequiConteo")]
        public IHttpActionResult GetHorariosRequiConteo(Guid requisicionId)
        {
            try
            {
                var horarios = db.HorariosRequis.Where(x => x.RequisicionId.Equals(requisicionId) && x.numeroVacantes > 0).Select(h => new
                {
                    id = h.Id,
                    nombre = h.Nombre + " de " + h.deHora.Hour + " a " + h.aHora.Hour,
                    deHora = h.deHora.Hour > 12 ? h.deHora.Hour + ":00 pm" : h.deHora.Hour + ":00 am",
                    aHora = h.aHora.Hour > 12 ? h.deHora.Hour + ":00 " : h.deHora.Hour + ":00 pm",
                    vacantes = h.numeroVacantes == db.ProcesoCandidatos.Where(x => x.HorarioId.Equals(h.Id) && x.EstatusId.Equals(24)).Count() ? true : false
                }).ToList();

                return Ok(horarios);
            }
            catch (Exception ex)
            {
                 return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("updateRequisiciones")]
        public IHttpActionResult UpdateRequi(RequisicionDto requi)
        {
            db.Database.Log = Console.Write;
            using (DbContextTransaction beginTran = db.Database.BeginTransaction())
            {
                try
                {
                    var requisicion = db.Requisiciones.Find(requi.Id);
                    db.Entry(requisicion).State = EntityState.Modified;

                    requisicion.fch_Cumplimiento = requi.fch_Cumplimiento;
                    requisicion.PrioridadId = requi.PrioridadId;
                    requisicion.Confidencial = requi.Confidencial;
                    if (requi.Confidencial && requisicion.EstatusId == 7)
                    {
                        requisicion.EstatusId = 6;
                    }
                    else
                    {
                        db.Entry(requisicion).Property(x => x.EstatusId).IsModified = false;
                        //requisicion.EstatusId = requi.EstatusId;
                    }

                    requisicion.fch_Modificacion = DateTime.Now;
                    requisicion.UsuarioMod = requi.Usuario;
                    if(requi.AsignacionRequi.Count() > 1)
                        requisicion.Asignada = true;
                    else
                        requisicion.Asignada = false;
                    AlterAsignacionRequi(requi.AsignacionRequi, requi.Id, requi.Folio, requi.Usuario, requisicion.VBtra);
                    db.SaveChanges();

                    Int64 Folio = requisicion.Folio;
                    //Creacion de Trazabalidad par ala requisición.
                    Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                    //Isertar el registro de la rastreabilidad. 
                    rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.Usuario, 3);

                    beginTran.Commit();

                    return Ok(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    beginTran.Rollback();
                    Console.Write(ex.Message);
                    return NotFound();
                }
            }


        }

        [HttpPost]
        [Route("deleteRequisiciones")]
        public IHttpActionResult DeleteRequi(RequisicionDeleteDto requi)
        {
            try
            {
                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi.Id)).ToList();

                var requisicion = db.Requisiciones.Find(requi.Id);
                db.Entry(requisicion).State = EntityState.Modified;
                requisicion.Activo = false;
                requisicion.UsuarioMod = requi.UsuarioMod;
                requisicion.fch_Modificacion = DateTime.Now;
                requisicion.EstatusId = 9;

                db.AsignacionRequis.RemoveRange(asignados);



                Int64 Folio = requisicion.Folio;
                string VBra = requisicion.VBtra;
                Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                //Isertar el registro de la rastreabilidad. 
                rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.UsuarioMod, 4);

                SendEmail.ConstructEmail(asignados, null, "RD", Folio, string.Empty, VBra);

                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch
            {
                return Ok(HttpStatusCode.NotAcceptable);
            }
        }

        [HttpPost]
        [Route("cancelRequisiciones")]
        public IHttpActionResult CancelRequi(RequisicionDeleteDto requi)
        {
            try
            {
                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi.Id)).ToList();

                var requisicion = db.Requisiciones.Find(requi.Id);
                db.Entry(requisicion).State = EntityState.Modified;
                requisicion.EstatusId = 8;
                requisicion.Aprobada = false;
                requisicion.Aprobador = string.Empty;
                requisicion.AprobadorId = new Guid("00000000-0000-0000-0000-000000000000");
                requisicion.UsuarioMod = requi.UsuarioMod;
                requisicion.fch_Modificacion = DateTime.Now;

                db.AsignacionRequis.RemoveRange(asignados);



                Int64 Folio = requisicion.Folio;
                string VBra = requisicion.VBtra;

                Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                //Isertar el registro de la rastreabilidad. 
                rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.UsuarioMod, 6);

                SendEmail.ConstructEmail(asignados, null, "RU", Folio, string.Empty, VBra);

                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.NotAcceptable);
            }
        }

        [HttpPost]
        [Route("reActivarRequisiciones")]
        public IHttpActionResult ReActivar(RequisicionDeleteDto requi)
        {
            try
            {
                var requisicion = db.Requisiciones.Find(requi.Id);
                db.Entry(requisicion).State = EntityState.Modified;
                requisicion.EstatusId = 5;
                requisicion.UsuarioMod = requi.UsuarioMod;
                requisicion.fch_Modificacion = DateTime.Now;


                Int64 Folio = requisicion.Folio;
                Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                //Isertar el registro de la rastreabilidad. 
                rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.UsuarioMod, 3);

                db.SaveChanges();


                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.NotAcceptable);
            }
        }

        [HttpPost]
        [Route("asignacionRequisiciones")]
        public IHttpActionResult AsginarRequi(AsignarVacanteReclutador requi)
        {
            db.Database.Log = Console.Write;
            using (DbContextTransaction beginTran = db.Database.BeginTransaction())
            {
                try
                {
                    var requisicion = db.Requisiciones.Find(requi.Id);
                    db.Entry(requisicion).State = EntityState.Modified;
                    requisicion.fch_Cumplimiento = requi.fch_Cumplimiento;
                    if (requisicion.EstatusId == 4)
                    {
                        requisicion.EstatusId = 6;
                        requisicion.Aprobador = requi.Usuario;
                        requisicion.AprobadorId = requi.AprobadorId;
                        requisicion.Aprobada = true;
                        requisicion.fch_Aprobacion = DateTime.Now;
                        
                    }
                    requisicion.DiasEnvio = requi.DiasEnvio;
                    requisicion.fch_Modificacion = DateTime.Now;
                    requisicion.UsuarioMod = requi.Usuario;
                    if (requi.AsignacionRequi.ToList().Count() > 1)
                        requisicion.Asignada = true;
                    else
                        requisicion.Asignada = false;
                    db.SaveChanges();
                    AlterAsignacionRequi(requi.AsignacionRequi, requi.Id, requisicion.Folio, requi.Usuario, requisicion.VBtra);
                    db.SaveChanges();
                    Int64 Folio = requisicion.Folio;
                    //Creacion de Trazabalidad par ala requisición.
                    Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                    //Isertar el registro de la rastreabilidad. 
                    rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.Usuario, 5);


                    beginTran.Commit();

                    return Ok(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    beginTran.Rollback();
                    Console.Write(ex.Message);
                    return NotFound();
                }
            }


        }

        private void Save()
        {
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    // Update the values of the entity that failed to save from the store 
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }

        public void AlterAsignacionRequi(List<AsignacionRequi> asignaciones, Guid RequiId, Int64 Folio, string Usuario, string VBra)
        {
            var user = db.Usuarios.Where(x => x.Usuario.Equals(Usuario)).Select(x =>
               x.Nombre + " " + x.ApellidoPaterno + " " + x.ApellidoMaterno
            ).FirstOrDefault();

            List<AsignacionRequi> NotChange = new List<AsignacionRequi>();
            List<AsignacionRequi> CheckExcept = new List<AsignacionRequi>();
            List<AsignacionRequi> AddElmt = new List<AsignacionRequi>();
            List<AsignacionRequi> Delete = new List<AsignacionRequi>();
            var asg = db.AsignacionRequis
                .Where(x => x.RequisicionId.Equals(RequiId))
                .ToList();
            if (asg.Count() > 0)
            {
                for (int i = 0; i < asg.Count(); i++)
                {
                    if (asignaciones.Count() > 0)
                    {
                        for (int x = 0; x < asignaciones.Count(); x++)
                        {
                            if (asignaciones[x].GrpUsrId.Equals(asg[i].GrpUsrId))
                            {
                                NotChange.Add(asignaciones[x]);
                                CheckExcept.Add(asg[i]);

                            }
                            if (asignaciones[x].GrpUsrId != asg[i].GrpUsrId)
                            {
                                AddElmt.Add(asignaciones[x]);

                            }
                        }
                    }
                }

                var filterAdd = AddElmt.Except(NotChange).ToList();
                var delet = asg.Except(CheckExcept).ToList();

                if (delet.Count() > 0)
                {
                    db.AsignacionRequis.RemoveRange(delet);
                    SendEmail.ConstructEmail(delet, NotChange, "D", Folio, user, VBra);
                }
                if (filterAdd.Count() > 0)
                {
                    db.AsignacionRequis.AddRange(filterAdd);
                    SendEmail.ConstructEmail(filterAdd, NotChange, "C", Folio, user, VBra);
                }
            }
            else
            {
                db.AsignacionRequis.AddRange(asignaciones);
                SendEmail.ConstructEmail(asignaciones, NotChange, "C", Folio, user, VBra);
            }

        }

        public List<Guid> GetGrupo(Guid grupo, List<Guid> listaIds)
        {
            if (!listaIds.Contains(grupo))
            {
                listaIds.Add(grupo);
                var listadoNuevo = db.GruposUsuarios
                    .Where(g => g.EntidadId.Equals(grupo) & g.Grupo.Activo)
                           .Select(g => g.GrupoId)
                           .ToList();
                foreach (Guid g in listadoNuevo)
                {
                    var gp = db.GruposUsuarios
                        .Where(x => x.EntidadId.Equals(g))
                        .Select(x => x.GrupoId)
                        .ToList();
                    foreach (Guid gr in gp)
                    {
                        GetGrupo(gr, listaIds);
                    }
                }
            }
            return listaIds;
        }
    }
}
