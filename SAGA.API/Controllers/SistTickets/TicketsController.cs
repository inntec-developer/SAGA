using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using System.Data.Entity;

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
        [Route("getFilaTickets")]
        public IHttpActionResult GetFilaTickets()
        {
            try
            {
                var fila = db.Tickets.OrderByDescending(f => f.fch_Creacion).Where(x => x.Estatus.Equals(1)).Select(t => new
                {
                    ticketId = t.Id,
                    ticket = t.Numero,
                    candidatoId = t.CandidatoId,
                    movimientoId = t.MovimientoId,
                    tiempo = DateTime.Now.Minute - t.fch_Creacion.Minute >= 0 ? DateTime.Now.Minute - t.fch_Creacion.Minute : 0
                });

                return Ok(fila);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getTicketPrioridad")]
        public IHttpActionResult GetTicketPrioridad()
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

                return Ok(ticket);

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
                var ticket = db.TicketsReclutador.Where(x => x.ReclutadorId.Equals(ReclutadorId) && x.TicketId.Equals(Ticket) && x.Ticket.Estatus.Equals(2)).Select(T => new
                {
                    numero = T.Ticket.Numero,
                    estado = T.Ticket.Estatus,
                    atendidos = db.TicketsReclutador.Where(x => x.ReclutadorId.Equals(ReclutadorId) && x.Ticket.Estatus.Equals(3) && x.fch_Atencion >= DateTime.Now).Count(),
                    candidato = new { nombre = T.Ticket.Candidato.Nombre + " " + T.Ticket.Candidato.ApellidoPaterno + " " + T.Ticket.Candidato.ApellidoPaterno,
                        dirNacimiento = T.Ticket.Candidato.municipioNacimiento.municipio + " " + T.Ticket.Candidato.estadoNacimiento.estado,
                        fechaNac = T.Ticket.Candidato.FechaNacimiento,
                        edad = DateTime.Now.Year - T.Ticket.Candidato.FechaNacimiento.Value.Year >= 0 ? DateTime.Now.Year - T.Ticket.Candidato.FechaNacimiento.Value.Year : 0 }
                }).ToList();

                return Ok(ticket);
            }
            catch(Exception ex )
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }
            
   }
}
