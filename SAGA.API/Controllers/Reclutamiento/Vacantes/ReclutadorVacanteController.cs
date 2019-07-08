using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/VacantesEmail")]
    public class ReclutadorVacanteController : ApiController
    {
        private SAGADBContext db;

        public ReclutadorVacanteController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("ShowVacanteEmail")]
        [Authorize]
        public IHttpActionResult ShoeVacanteEmail(Int64 Folio)
        {
            try
            {
                var requisicion = db.Requisiciones
                    .Where(r => r.Folio.Equals(Folio))
                    .Select(r => new {
                        Id = r.Id,
                        Folio= r.Folio,
                        fch_Cumplimiento = r.fch_Cumplimiento,
                        fch_Creacion = r.fch_Creacion,
                        fch_Limite = r.fch_Limite,
                        Prioridad = r.Prioridad.Descripcion,
                        PrioridadId = r.PrioridadId,
                        Confidencial = r.Confidencial,
                        Estatus = r.Estatus.Descripcion,
                        EstatusId = r.EstatusId,
                        solicitante = db.Entidad.Where(x => x.Id.Equals(r.PropietarioId)).Select(S => S.Nombre + " " + S.ApellidoPaterno + " " + S.ApellidoMaterno).FirstOrDefault(),
                        coordinador = db.Entidad.Where(x => x.Id.Equals(r.AprobadorId)).Select(S => S.Nombre + " " + S.ApellidoPaterno + " " + S.ApellidoMaterno).FirstOrDefault(),
                        asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id)).Select(x => x.GrpUsrId).ToList(),
                        asignadosN = r.AsignacionRequi.Where(x => x.RequisicionId.Equals(r.Id) && !x.GrpUsrId.Equals(r.AprobadorId)).Select(x => new {
                            x.GrpUsr.Nombre,
                            x.GrpUsr.ApellidoMaterno,
                            x.GrpUsr.ApellidoPaterno
                        }),
                        vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        r.VBtra,
                        escolaridades = r.escolaridadesRequi.Select(es => new
                        {
                            gradoEstudio = es.Escolaridad.gradoEstudio,
                            estadoEstudio = es.EstadoEstudio.estadoEstudio
                        }).ToList(),
                        aptitudes = r.aptitudesRequi.Select(a => new
                        {
                            aptitud = a.Aptitud.aptitud
                        }).ToList(),
                        areaExperiencia = r.Area.areaExperiencia,
                        genero = r.Genero.genero,
                        edadMinima = r.EdadMinima,
                        edadMaxima = r.EdadMaxima,
                        estadoCivil = r.EstadoCivil.estadoCivil,
                        sueldoMinimo = r.SueldoMinimo,
                        sueldoMaximo = r.SueldoMaximo,
                        actividades = r.actividadesRequi.Select(ac => new {
                            actividades = ac.Actividades
                        }).ToList(),
                        experiencia = r.Experiencia,
                        prestacionesCliente = r.prestacionesClienteRequi.Select(pcr => new {
                            prestamo = pcr.Prestamo
                        }).ToList(),
                        observaciones = r.observacionesRequi.Select(ob => new {
                            observaciones = ob.Observaciones
                        }).ToList(),
                    })
                    .FirstOrDefault();

                return Ok(requisicion);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

    }
}
