﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using System.Data.Entity;
using System.Data.SqlClient;
namespace SAGA.API.Controllers
{
    [RoutePrefix("api/SistTickets")]
    public class TicketsController : ApiController
    {
        private SAGADBContext db;

        public TicketsController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("InsertTicketRecl")]
        public IHttpActionResult InsertTicketRecl(Guid Ticket, Guid ReclutadorId)
        {
            try
            {
                Ticket t = new Ticket();
                TicketReclutador tr = new TicketReclutador();

                tr.ReclutadorId = ReclutadorId;
                tr.TicketId = Ticket;
                tr.fch_Atencion = DateTime.Now;
                tr.fch_Final = DateTime.Now;

                db.TicketsReclutador.Add(tr);
                db.SaveChanges();

                t = db.Tickets.Find(Ticket);

                db.Entry(t).State = EntityState.Modified;

                t.Estatus = 2;

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
        public IHttpActionResult UpdateStatus(Guid ticketId)
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

                t.Estatus = 3;

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
        public IHttpActionResult GetFilaTickets()
        {
            try
            {
                var tiempo = (from TR in db.Tickets
                              select new { TR.fch_Creacion, DateTime.Now }).ToArray().Select(x => (DateTime.Now - x.fch_Creacion).TotalMinutes);


                var fila = db.Tickets.OrderByDescending(f => f.fch_Creacion).Where(x => x.Estatus.Equals(1)).Select(t => new
                {
                    ticketId = t.Id,
                    ticket = t.Numero,
                    candidatoId = t.CandidatoId,
                    movimientoId = t.MovimientoId,
                    fch_Creacion = t.fch_Creacion,
                    tiempo = (DateTime.Now.Hour - t.fch_Creacion.Hour) * 60
                }).ToList();

                return Ok(fila);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getTicketPrioridad")]
        public IHttpActionResult GetTicketPrioridad(Guid reclutadorId)
        {

            try
            {

                var ticket = db.Tickets.OrderBy(f => f.fch_Creacion).Where(x => x.Estatus.Equals(1)).Select(t => new
                {
                    ticketId = t.Id,
                    ticket = t.Numero,
                    candidatoId = t.CandidatoId,
                    movimientoId = t.MovimientoId,
                }).FirstOrDefault();

                this.InsertTicketRecl(ticket.ticketId, reclutadorId);

            //    var result = this.GetTicketsReclutador(ticket.ticketId, reclutadorId);

                return Ok(ticket.ticketId);

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
                TimeSpan d = new TimeSpan(1, 0, 0, 0);

                var tiempo = (from TR in db.TicketsReclutador
                              select new { TR.fch_Atencion, TR.fch_Final }).ToArray().Average(x => (x.fch_Final - x.fch_Atencion).TotalMinutes);

                var ticket = db.TicketsReclutador.Where(x => x.ReclutadorId.Equals(ReclutadorId) && x.TicketId.Equals(Ticket) ).Select(T => new
                {
                    ticketId = T.TicketId,
                    numero = T.Ticket.Numero,
                    estado = T.Ticket.Estatus,
                    atendidos = db.TicketsReclutador.Where(x => x.ReclutadorId.Equals(ReclutadorId) && x.Ticket.Estatus.Equals(3) && x.fch_Atencion <= DateTime.Now).Count(),
                    fechaAtencion = T.fch_Atencion,
                    fechaFinal = T.fch_Final,
                    tiempo = tiempo,
                  //  tiempo = db.TicketsReclutador.Where(x => x.ReclutadorId.Equals(ReclutadorId)).Select(TR=>(TR.fch_Final -TR.fch_Atencion).TotalMinutes),
                    //tiempo = from TR in db.TicketsReclutador group TR by TR.fch_Atencion.Day into G select new { Day = G.Key, Minutes =  G.Average(TR => (TR.fch_Final - TR.fch_Atencion).TotalMinutes) > 0 ? G.Average(TR => (TR.fch_Final - TR.fch_Atencion).TotalMinutes) : 0 },
                    candidato = new {
                        candidatoId = T.Ticket.CandidatoId,
                        curp = T.Ticket.Candidato.CURP, nombre = T.Ticket.Candidato.Nombre + " " + T.Ticket.Candidato.ApellidoPaterno + " " + T.Ticket.Candidato.ApellidoPaterno,
                        dirNacimiento = T.Ticket.Candidato.municipioNacimiento.municipio + " " + T.Ticket.Candidato.estadoNacimiento.estado,
                        fechaNac = T.Ticket.Candidato.FechaNacimiento,
                        edad = DateTime.Now.Year - T.Ticket.Candidato.FechaNacimiento.Value.Year >= 0 ? DateTime.Now.Year - T.Ticket.Candidato.FechaNacimiento.Value.Year : 0,
                        email = T.Ticket.Candidato.emails.Select(m => m.email).FirstOrDefault(),
                    }
                      
                }).ToList();

                return Ok(ticket);
            }
            catch(Exception ex )
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
                var requisicion = db.Postulaciones
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
                    reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.Requisicion.AprobadorId)).Select(a => new
                    {
                        reclutador = db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault()
                    }).Distinct().ToList(),
                    ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList()

                }).OrderByDescending(x => x.Folio).ToList();

                return Ok(requisicion);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
            
   }
}
