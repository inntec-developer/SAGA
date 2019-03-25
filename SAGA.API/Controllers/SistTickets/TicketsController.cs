using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using System.Data.Entity;
using System.Data.SqlClient;
using SAGA.API.Dtos;
namespace SAGA.API.Controllers
{
    [RoutePrefix("api/SistTickets")]
    public class TicketsController : ApiController
    {
        private SAGADBContext db;
        Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");
        public TicketsController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("InsertTicketRecl")]
        public IHttpActionResult InsertTicketRecl(Guid Ticket, Guid ReclutadorId, int ModuloId)
        {
            try
            {
                PostulateVacantController obj = new PostulateVacantController();
                ProcesoDto dto = new ProcesoDto();
                Ticket t = new Ticket();
                TicketReclutador tr = new TicketReclutador();

                tr.ReclutadorId = ReclutadorId;
                tr.TicketId = Ticket;
                tr.fch_Atencion = DateTime.Now;
                tr.fch_Final = DateTime.Now;

                db.TicketsReclutador.Add(tr);
                db.SaveChanges();

                dto.ReclutadorId = ReclutadorId;

                t = db.Tickets.Find(Ticket);

                db.Entry(t).State = EntityState.Modified;
                t.Estatus = 2;
                t.ModuloId = ModuloId;

                db.SaveChanges();

                dto.candidatoId = t.CandidatoId;

                var horario = db.HorariosRequis.Where(x => x.RequisicionId.Equals(t.RequisicionId)).Select(H => H.Id).FirstOrDefault();

                dto.horarioId = horario;
                dto.requisicionId = t.RequisicionId;
                dto.estatusId = 18; //entrevista reclutamiento

                obj.UpdateStatus(dto);
 
                return Ok(HttpStatusCode.OK);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("ticketConCita")]
        public IHttpActionResult TicketConCita(int folio)
        {
            try
            {
                var cita = db.CalendarioCandidato.Where(x => x.Folio.Equals(folio)).Select(T => new
                {
                    requisicionId = T.RequisicionId,
                    candidatoId = T.CandidatoId
                }).FirstOrDefault();

                if (cita != null)
                {
                    Ticket ticket = new Ticket();
                    ticket.CandidatoId = cita.candidatoId;
                    ticket.RequisicionId = cita.requisicionId;
                    ticket.MovimientoId = 1;
                    ticket.ModuloId = 2;
                    ticket.Estatus = 1;

                    var num = db.Tickets.Where(x => x.RequisicionId.Equals(cita.requisicionId) && x.MovimientoId.Equals(1)).Count();

                    var f = db.Requisiciones.Where(x => x.Id.Equals(cita.requisicionId)).Select(ff => ff.Folio).FirstOrDefault().ToString();

                    ticket.Numero = "CC-" + f.Substring(f.Length - 4, 4) + '-'  + num.ToString().PadLeft(3, '0');

                    db.Tickets.Add(ticket);

                    db.SaveChanges();

                    return Ok(ticket.Numero);
                }
                else
                {
                    return Ok("sincita");
                }
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("ticketSinCita")]
        public IHttpActionResult TicketSinCita(Guid requisicionId)
        {
            try
            {
                Ticket ticket = new Ticket();
                ticket.CandidatoId = new Guid("F66DA23E-9D69-E811-80E1-9E274155325E");
                ticket.RequisicionId = requisicionId;
                ticket.MovimientoId = 2;
                ticket.ModuloId = 2;
                ticket.Estatus = 1;

                var num = db.Tickets.Where(x => x.RequisicionId.Equals(requisicionId) && x.MovimientoId.Equals(2)).Count();

                var folio = db.Requisiciones.Where(x => x.Id.Equals(requisicionId)).Select(f => f.Folio).FirstOrDefault().ToString();

                ticket.Numero = "SC-" + folio.Substring(folio.Length - 4, 4) + '-' + num.ToString().PadLeft(3, '0');
             
                db.Tickets.Add(ticket);

                db.SaveChanges();

                return Ok(ticket.Numero);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("updateRequiTicket")]
        public IHttpActionResult UpdateRequiTicket(Guid ticketId, Guid requisicionId)
        {
            try
            {
                var t = db.Tickets.Find(ticketId);
                db.Entry(t).Property(x => x.RequisicionId).IsModified = true;

                t.RequisicionId = requisicionId;
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("updateStatus")]
        public IHttpActionResult UpdateStatus(Guid ticketId, int estatus, int moduloId)
        {
            try
            {
                var id = db.TicketsReclutador.Where(x => ticketId.Equals(ticketId)).Select(ID => ID.Id).FirstOrDefault();
                var TR = db.TicketsReclutador.Find(id);

                db.Entry(TR).Property(x => x.fch_Final).IsModified = true;

                TR.fch_Final = DateTime.Now;

                db.SaveChanges();

                var t = db.Tickets.Find(ticketId);

                db.Entry(t).State = EntityState.Modified;

                t.Estatus = estatus;
                t.ModuloId = moduloId;
                
                db.SaveChanges();

            
                return Ok(HttpStatusCode.OK);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpGet]
        [Route("getFilaTickets")]
        public IHttpActionResult GetFilaTickets(int estatus, Guid reclutadorId)
        {
            try
            {
                if (estatus == 3)
                {

                    var fila = db.Tickets.OrderBy(f => f.fch_Creacion).Where(x => x.Estatus.Equals(estatus)).Select(t => new
                    {
                        ticketId = t.Id,
                        ticket = t.Numero,
                        candidatoId = t.CandidatoId,
                        requisicionId = t.RequisicionId,
                        movimientoId = t.MovimientoId,
                        moduloId = t.ModuloId,
                        fch_Creacion = t.fch_Creacion,
                        fch_cita = db.CalendarioCandidato.Where(x => x.CandidatoId.Equals(t.CandidatoId)).Count() > 0 ? db.CalendarioCandidato.Where(x => x.CandidatoId.Equals(t.CandidatoId)).Select(f => f.Fecha).FirstOrDefault() : DateTime.Now,
                        tiempo = (DateTime.Now.Minute - t.fch_Creacion.Minute)
                    }).ToList();

                    db.SaveChanges();

                    return Ok(fila);
                }
                else
                {
                    var requis = db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(reclutadorId)).Select(r => r.RequisicionId).ToList();

                    var fila = db.Tickets.OrderBy(f => f.fch_Creacion).Where(x => x.Estatus.Equals(estatus) && requis.Contains(x.RequisicionId)).Select(t => new
                    {
                        ticketId = t.Id,
                        ticket = t.Numero,
                        candidatoId = t.CandidatoId,
                        requisicionId = t.RequisicionId,
                        movimientoId = t.MovimientoId,
                        moduloId = t.ModuloId,
                        fch_Creacion = t.fch_Creacion,
                        fch_cita = db.CalendarioCandidato.Where(x => x.CandidatoId.Equals(t.CandidatoId)).Count() > 0 ? db.CalendarioCandidato.Where(x => x.CandidatoId.Equals(t.CandidatoId)).Select(f => f.Fecha).FirstOrDefault() : DateTime.Now,
                        tiempo = (DateTime.Now.Minute - t.fch_Creacion.Minute)
                    }).ToList();

                    return Ok(fila);

                }
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getTicketEnAtencion")]
        public IHttpActionResult GetTicketEnAtencion()
        {
            try
            {
                var ticket = db.Tickets.OrderByDescending(f => f.fch_Creacion).Where(x => x.Estatus.Equals(2)).Select(t => new
                {
                    ticketId = t.Id,
                    ticket = t.Numero,
                    candidatoId = t.CandidatoId,
                    requisicionId = t.RequisicionId,
                    movimientoId = t.MovimientoId,
                    modulo = t.Modulo.Modulo,
                    moduloId = t.ModuloId
                }).ToList();

                return Ok(ticket);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getTicketPrioridad")]
        public IHttpActionResult GetTicketPrioridad(Guid reclutadorId, int ModuloId)
        {

            try
            {
                var requisiciones = db.CalendarioCandidato.OrderByDescending(f => f.Fecha).Where(x => (x.Fecha.Minute - DateTime.Now.Minute) >= -15 && (x.Fecha.Minute - DateTime.Now.Minute) <= 15).Select(c => c.RequisicionId).ToList();

                var requiReclutador = db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(reclutadorId) && requisiciones.Contains(x.RequisicionId)).Select(r => r.RequisicionId).ToList();

                //var requiReclutador = db.Requisiciones.Where(x => requisiciones.Contains(x.Id) && x.PropietarioId.Equals(reclutadorId)).Select(r => r.Id).ToList();

                var concita = db.Tickets.OrderBy(f => f.fch_Creacion).Where(x => x.Estatus.Equals(1) && requiReclutador.Contains(x.RequisicionId) && x.MovimientoId.Equals(1))
                .Select(t => new
                {
                    ticketId = t.Id,
                    ticket = t.Numero,
                    candidatoId = t.CandidatoId,
                    movimientoId = t.MovimientoId,
                    tiempoCita = t.fch_Creacion.Minute - DateTime.Now.Minute
                }).FirstOrDefault();

                if (concita == null)
                {
                    var requis = db.Requisiciones.Where(x => x.PropietarioId.Equals(reclutadorId)).Select(r => r.Id).ToList();

                    var sincita = db.Tickets.OrderBy(f => f.fch_Creacion).Where(x => x.Estatus.Equals(1) && requis.Contains(x.RequisicionId)).Select(t => new
                    {
                        ticketId = t.Id,
                        ticket = t.Numero,
                        candidatoId = t.CandidatoId,
                        requisicionId = t.RequisicionId,
                        movimientoId = t.MovimientoId,
                        tiempoCita = t.fch_Creacion.Minute - DateTime.Now.Minute
                    }).FirstOrDefault();

                    if (sincita == null)
                    {
                        var ticket = db.Tickets.OrderBy(f => f.fch_Creacion).Where(x => x.Estatus.Equals(1)).Select(t => new
                        {
                            ticketId = t.Id,
                            ticket = t.Numero,
                            candidatoId = t.CandidatoId,
                            requisicionId = t.RequisicionId,
                            movimientoId = t.MovimientoId,
                            tiempoCita = t.fch_Creacion.Minute - DateTime.Now.Minute
                        }).FirstOrDefault();

                        this.InsertTicketRecl(ticket.ticketId, reclutadorId, ModuloId);
                        return Ok(ticket.ticketId);
                    }
                    else
                    {
                        this.InsertTicketRecl(sincita.ticketId, reclutadorId, ModuloId);
                        return Ok(sincita.ticketId);
                    }
                }
                else
                {
                    this.InsertTicketRecl(concita.ticketId, reclutadorId, ModuloId);
                    return Ok(concita.ticketId);
                }


                //    var result = this.GetTicketsReclutador(ticket.ticketId, reclutadorId);



            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("getTicketsReclutador")]
        public IHttpActionResult GetTicketsReclutador(Guid Ticket, Guid ReclutadorId)
        {
            try
            {
                Guid aux = new Guid("00000000-0000-0000-0000-000000000000");

                int tiempo;

                if (db.TicketsReclutador.Count() > 0)
                {
                    tiempo = (int)(from TR in db.TicketsReclutador
                                   select new { TR.fch_Atencion, TR.fch_Final }).ToArray().Average(x => (x.fch_Final - x.fch_Atencion).TotalMinutes);
                }
                else
                {
                    tiempo = 0;
                }
                var ticket = db.TicketsReclutador.Where(x => x.ReclutadorId.Equals(ReclutadorId) && x.TicketId.Equals(Ticket)).Select(T => new
                {
                    ticketId = T.TicketId,
                    requisicionId = T.Ticket.RequisicionId,
                    numero = T.Ticket.Numero,
                    estado = T.Ticket.Estatus,
                    atendidos = db.TicketsReclutador.Where(x => x.ReclutadorId.Equals(ReclutadorId) && x.Ticket.Estatus.Equals(3) && x.fch_Atencion <= DateTime.Now).Count(),
                    fechaAtencion = T.fch_Atencion,
                    fechaFinal = T.fch_Final,
                    tiempo = tiempo,
                    //  tiempo = db.TicketsReclutador.Where(x => x.ReclutadorId.Equals(ReclutadorId)).Select(TR=>(TR.fch_Final -TR.fch_Atencion).TotalMinutes),
                    //tiempo = from TR in db.TicketsReclutador group TR by TR.fch_Atencion.Day into G select new { Day = G.Key, Minutes =  G.Average(TR => (TR.fch_Final - TR.fch_Atencion).TotalMinutes) > 0 ? G.Average(TR => (TR.fch_Final - TR.fch_Atencion).TotalMinutes) : 0 },
                    candidato = new
                    {
                        candidatoId = T.Ticket.CandidatoId,
                        curp = T.Ticket.Candidato.CURP,
                        nombre = T.Ticket.Candidato.Nombre + " " + T.Ticket.Candidato.ApellidoPaterno + " " + T.Ticket.Candidato.ApellidoPaterno,
                        dirNacimiento = T.Ticket.Candidato.municipioNacimiento.municipio + " " + T.Ticket.Candidato.estadoNacimiento.estado,
                        fechaNac = T.Ticket.Candidato.FechaNacimiento,
                        edad = DateTime.Now.Year - T.Ticket.Candidato.FechaNacimiento.Value.Year >= 0 ? DateTime.Now.Year - T.Ticket.Candidato.FechaNacimiento.Value.Year : 0,
                        email = T.Ticket.Candidato.emails.Select(m => m.email).FirstOrDefault(),
                        estatusId = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.Ticket.CandidatoId)).Count() > 0 ? db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.Ticket.CandidatoId)).Select(E => E.EstatusId).FirstOrDefault() : 27,
                        estatus = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(T.Ticket.CandidatoId)).Count() > 0 ? db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.Ticket.CandidatoId)).Select(E => E.Estatus.Descripcion).FirstOrDefault() : "Disponible",
                        requisicionId = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.Ticket.CandidatoId)).Count() > 0 ? db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.Ticket.CandidatoId)).Select(r => r.RequisicionId).FirstOrDefault() : aux
                    }

                }).ToList();

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("getTicketsExamen")]
        public IHttpActionResult GetTicketsExamen(Guid Ticket)
        {
            try
            {
                Guid aux = new Guid("00000000-0000-0000-0000-000000000000");

                var ticket = db.Tickets.Where(x => x.Id.Equals(Ticket)).Select(T => new
                {
                    ticketId = T.Id,
                    requisicionId = T.RequisicionId,
                    vBtra = T.Requisicion.VBtra,
                    folio = T.Requisicion.Folio,
                    numero = T.Numero,
                    estado = T.Estatus,
                    psicometria = db.PsicometriasDamsaRequis.Where(x => x.RequisicionId.Equals(T.RequisicionId) && x.PsicometriaId > 0).Count() > 0 ? true : false,
                    candidato = new
                    {
                        candidatoId = T.CandidatoId,
                        curp = T.Candidato.CURP,
                        nombre = T.Candidato.Nombre + " " + T.Candidato.ApellidoPaterno + " " + T.Candidato.ApellidoPaterno,
                        dirNacimiento = T.Candidato.municipioNacimiento.municipio + " " + T.Candidato.estadoNacimiento.estado,
                        fechaNac = T.Candidato.FechaNacimiento,
                        edad = DateTime.Now.Year - T.Candidato.FechaNacimiento.Value.Year >= 0 ? DateTime.Now.Year - T.Candidato.FechaNacimiento.Value.Year : 0,
                        email = T.Candidato.emails.Select(m => m.email).FirstOrDefault(),
                        estatusId = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.CandidatoId)).Count() > 0 ? db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.CandidatoId)).Select(E => E.EstatusId).FirstOrDefault() : 27,
                        estatus = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(T.CandidatoId)).Count() > 0 ? db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.CandidatoId)).Select(E => E.Estatus.Descripcion).FirstOrDefault() : "Disponible",
                        requisicionId = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.CandidatoId)).Count() > 0 ? db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(T.CandidatoId)).Select(r => r.RequisicionId).FirstOrDefault() : aux,
                        tecnicos = db.ExamenCandidato.Where(x => x.CandidatoId.Equals(T.CandidatoId) && x.RequisicionId.Equals(T.RequisicionId)).Count() > 0 ? db.ExamenCandidato.Where(x => x.CandidatoId.Equals(T.CandidatoId) && x.RequisicionId.Equals(T.RequisicionId)).Select(R => R.Resultado).FirstOrDefault() : 9999
                    }

                }).ToList();

                Ticket t = new Ticket();

                t = db.Tickets.Find(Ticket);

                db.Entry(t).State = EntityState.Modified;
                t.Estatus = 5;

                db.SaveChanges();

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getPostulaciones")]
        public IHttpActionResult GetPostulaciones(Guid candidatoId)
        {
            try
            {
                var requisicion = db.Tickets
                .Where(e => e.CandidatoId.Equals(candidatoId) && e.Requisicion.Activo.Equals(true))
                .Select(e => new
                {
                    Id = e.RequisicionId,
                    vBtra = e.Requisicion.VBtra,
                    TipoReclutamiento = e.Requisicion.TipoReclutamiento.tipoReclutamiento,
                    tipoReclutamientoId = e.Requisicion.TipoReclutamientoId,
                    ClaseReclutamiento = e.Requisicion.ClaseReclutamiento.clasesReclutamiento,
                    ClaseReclutamientoId = e.Requisicion.ClaseReclutamientoId,
                    SueldoMinimo = e.Requisicion.SueldoMinimo,
                    SueldoMaximo = e.Requisicion.SueldoMaximo,
                    fch_Creacion = e.Requisicion.fch_Creacion,
                    fch_Modificacion = e.Requisicion.fch_Modificacion,
                    fch_Cumplimiento = e.Requisicion.fch_Cumplimiento,
                    Estatus = e.Requisicion.Estatus.Descripcion,
                    EstatusId = e.Requisicion.EstatusId,
                    Prioridad = e.Requisicion.Prioridad.Descripcion,
                    PrioridadId = e.Requisicion.PrioridadId,
                    Cliente = e.Requisicion.Cliente.Nombrecomercial,
                    GiroEmpresa = e.Requisicion.Cliente.GiroEmpresas.giroEmpresa,
                    ActividadEmpresa = e.Requisicion.Cliente.ActividadEmpresas.actividadEmpresa,
                    Vacantes = e.Requisicion.horariosRequi.Count() > 0 ? e.Requisicion.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                    Folio = e.Requisicion.Folio,
                    DiasEnvio = e.Requisicion.DiasEnvio,
                    Confidencial = e.Requisicion.Confidencial,
                    Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) & p.StatusId.Equals(1)).Count(),
                    EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Count(),
                    Propietario = db.Usuarios.Where(x => x.Id.Equals(e.Requisicion.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                    reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.RequisicionId) ).Select(a => new
                    {
                        reclutadorId = a.GrpUsrId,
                        nombre = db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault()
                    }).Distinct().ToList(),
                    ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList(),
                    examenId = db.RequiExamen.Where(x => x.RequisicionId.Equals(e.RequisicionId)).Count() > 0 ? db.RequiExamen.Where(x => x.RequisicionId.Equals(e.RequisicionId)).Select(ex => ex.ExamenId).FirstOrDefault() : 0
                }).OrderByDescending(x => x.Folio).ToList();
                //&& !x.GrpUsrId.Equals(e.Requisicion.AprobadorId)
                return Ok(requisicion);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("liberarCandidato")]
        public IHttpActionResult LiberarCandidatos(Guid requisicionId, Guid candidatoId)
        {
            Guid aux = new Guid("00000000-0000-0000-0000-000000000000");
            try
            {
                var id = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(x => x.CandidatoId.Equals(candidatoId) && x.RequisicionId.Equals(requisicionId)).Select(x => x.Id).FirstOrDefault();
                var c = db.ProcesoCandidatos.Find(id);

                db.Entry(c).Property(x => x.EstatusId).IsModified = true;
                c.EstatusId = 27;

                db.SaveChanges();

                var ids = db.Postulaciones.Where(x => x.RequisicionId.Equals(requisicionId) && x.CandidatoId.Equals(candidatoId)).Select(x => x.Id).FirstOrDefault();
                if (ids != aux)
                {
                    var cc = db.Postulaciones.Find(ids);

                    db.Entry(cc).Property(x => x.StatusId).IsModified = true;
                    cc.StatusId = 1;

                    db.SaveChanges();
                }

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getVacantes")]
        public IHttpActionResult GetVacantes()
        {
            try
            {
                List<int> estatus = new List<int> { 34, 35, 36, 36 };
                var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                    .Where(e => e.Activo && !estatus.Contains(e.EstatusId))
                    .Select(e => new
                    {
                        Id = e.Id,
                        //Folio = e.Folio,
                        //Cliente = e.Cliente.Nombrecomercial,
                        //ClienteId = e.Cliente.Id,
                        //estado = e.Cliente.direcciones.Select(x => x.Municipio.municipio + " " + x.Estado.estado + " " + x.Estado.Pais.pais).FirstOrDefault(),
                        //domicilio_trabajo = e.Direccion.Calle + " " + e.Direccion.NumeroExterior + " " + e.Direccion.Colonia.colonia + " " + e.Direccion.Municipio.municipio + " " + e.Direccion.Estado.estado,
                        //Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        VBtra = e.VBtra,
                        requisitos = e.DAMFO290.escolardadesPerfil.Select(esc => esc.Escolaridad.gradoEstudio),
                        Actividades = e.DAMFO290.actividadesPerfil.Select(a => a.Actividades),
                        aptitudes = e.DAMFO290.aptitudesPerfil.Select(ap => ap.Aptitud.aptitud),
                        experiencia = e.Experiencia
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
        [Route("getModulos")]
        public IHttpActionResult GetModulos()
        {
            try
            {
                var modulos = db.ModulosReclutamiento.Select(M => new {
                    M.Modulo,
                    M.Id, 
                    M.TipoModulo
                });

                return Ok(modulos);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpPost]
        [Route("setExamen")]
        public IHttpActionResult SetExamen(ExamenCandidato objeto)
        {
            try
            {
                Guid aux = new Guid("00000000-0000-0000-0000-000000000000");
                var e = db.ExamenCandidato.Where(x => x.CandidatoId.Equals(objeto.CandidatoId) && x.RequisicionId.Equals(objeto.RequisicionId)).Select(id => id.Id).FirstOrDefault();

                if(e == aux)
                {
                    objeto.fch_Creacion = DateTime.Now;
                    objeto.fch_Modificacion = DateTime.Now;

                    db.ExamenCandidato.Add(objeto);

                    db.SaveChanges();

                    var ind = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(objeto.CandidatoId) && x.RequisicionId.Equals(objeto.RequisicionId)).Select(c => c.Id).FirstOrDefault();
                    var C = db.ProcesoCandidatos.Find(ind);

                    db.Entry(C).Property(x => x.EstatusId).IsModified = true;
                    db.Entry(C).Property(x => x.Fch_Modificacion).IsModified = true;


                    if (db.InformeRequisiciones.Where(x => x.CandidatoId.Equals(objeto.CandidatoId) && x.RequisicionId.Equals(objeto.RequisicionId) && x.EstatusId.Equals(18)).Count() > 0)
                    {
                        C.EstatusId = 13;
                        C.Fch_Modificacion = DateTime.Now;

                        db.SaveChanges();
                    }
                    else
                    {
                        C.EstatusId = 18;
                        C.Fch_Modificacion = DateTime.Now;

                        db.SaveChanges();

                        C.EstatusId = 13;
                        C.Fch_Modificacion = DateTime.Now;

                        db.SaveChanges();

                    }
                }

                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
            
        }
        [HttpGet]
        [Route("setEstatusCandidato")]
        public IHttpActionResult SetEstatus(Guid candidatoId, Guid requisicionId, int estatusId)
        {
            try
            {
                Guid aux = new Guid("00000000-0000-0000-0000-000000000000");

                var ind = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(candidatoId) && x.RequisicionId.Equals(requisicionId)).Select(c => c.Id).FirstOrDefault();
                var C = db.ProcesoCandidatos.Find(ind);

                db.Entry(C).Property(x => x.EstatusId).IsModified = true;
                db.Entry(C).Property(x => x.Fch_Modificacion).IsModified = true;

                if (db.InformeRequisiciones.Where(x => x.CandidatoId.Equals(candidatoId) && x.RequisicionId.Equals(requisicionId) && x.EstatusId.Equals(estatusId)).Count() == 0)
                {
                        C.EstatusId = 13;
                        C.Fch_Modificacion = DateTime.Now;

                        db.SaveChanges();
                }
                 
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }


    }
}
