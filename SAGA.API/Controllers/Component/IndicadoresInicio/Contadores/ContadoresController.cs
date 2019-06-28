using SAGA.API.Dtos;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/contadores")]
    [Authorize]
    public class ContadoresController : ApiController
    {

        private SAGADBContext db;
        private List<Guid> uids;

        public ContadoresController()
        {
            db = new SAGADBContext();
            uids = new List<Guid>();
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
        [Route("perfiles")]
        public IHttpActionResult GetPerfiles()
        {
            int perfiles = 0;
            try
            {
                perfiles = db.DAMFO290
                    .Where(d => d.Activo.Equals(true))
                    .Count();
                return Ok(perfiles);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("folios")]
        public IHttpActionResult GetFolios(UsuarioDto user)
        {
            try
            {
                int Folios = 0;

                int[] estatus = { 4, 5, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 44, 46 };
                if (user.TipoUsuarioId == 8 || user.TipoUsuarioId == 3 || user.TipoUsuarioId == 12 || user.TipoUsuarioId == 13 || user.TipoUsuarioId == 14)
                {
                    Folios = db.Requisiciones
                        .Where( r => estatus.Contains(r.EstatusId) && !r.Confidencial)
                        .Count();
                }
                else
                {
                    if(db.Subordinados.Count(x => x.LiderId.Equals(user.Id)) > 0)
                    {
                        var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(user.Id) && x.LiderId.Equals(user.Id))
                            .Select(u => u.UsuarioId)
                            .ToList();
                        uids = GetSub(ids, uids);
                    }

                    uids.Add(user.Id);

                    var requis = db.AsignacionRequis
                        .OrderByDescending(e => e.Id)
                        .Where(a => uids.Distinct().Contains(a.GrpUsrId))
                        .Select(a => a.RequisicionId)
                        .ToList();

                    Folios = db.Requisiciones
                        .Where(r => requis.Contains(r.Id) || r.PropietarioId.Equals(user.Id))
                        .Where(r => estatus.Contains(r.EstatusId))
                        .Count(); 
                }

                return Ok(Folios);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        [HttpPost]
        [Route("posiciones")]
        public IHttpActionResult GetPosiciones(UsuarioDto user)
        {
            try
            {
                int TotalVacantes = 0;

                int[] estatus = { 4, 5, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 44, 46 };
                if (user.TipoUsuarioId == 8 || user.TipoUsuarioId == 3 || user.TipoUsuarioId == 12 || user.TipoUsuarioId == 13 || user.TipoUsuarioId == 14)
                {
                    var vacantes = db.Requisiciones
                        .Where(r => estatus.Contains(r.EstatusId) && !r.Confidencial)
                        .Select(r => new
                        {
                            Vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        }).ToList();
                    foreach(var vtn in vacantes)
                    {
                        TotalVacantes = TotalVacantes + vtn.Vacantes;
                    }
                }
                else
                {
                    if (db.Subordinados.Count(x => x.LiderId.Equals(user.Id)) > 0)
                    {
                        var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(user.Id) && x.LiderId.Equals(user.Id))
                            .Select(u => u.UsuarioId)
                            .ToList();
                        uids = GetSub(ids, uids);
                    }

                    uids.Add(user.Id);

                    var requis = db.AsignacionRequis
                        .OrderByDescending(e => e.Id)
                        .Where(a => uids.Distinct().Contains(a.GrpUsrId))
                        .Select(a => a.RequisicionId)
                        .ToList();

                    var vacantes = db.Requisiciones
                        .Where(r => requis.Contains(r.Id) || r.PropietarioId.Equals(user.Id))
                        .Where(r => estatus.Contains(r.EstatusId))
                         .Select(r => new
                         {
                             Vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                         }).ToList();
                    foreach (var vtn in vacantes)
                    {
                        TotalVacantes = TotalVacantes + vtn.Vacantes;
                    }
                }

                return Ok(TotalVacantes);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }


        [HttpGet]
        [Route("candidatos")]
        public IHttpActionResult GetCandidatos()
        {
            try
            {
                var activos = db.AspNetUsers.Where(a => a.Activo == 0 && a.IdPersona != null).Select(a => a.IdPersona).ToList();
                var candidatos = db.PerfilCandidato
                    .Where(c => activos.Contains(c.CandidatoId))
                    .Count();
                return Ok(candidatos);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("posicionesActivas")]
        public IHttpActionResult GetPosicionesActivass(UsuarioDto user)
        {
            try
            {
                int TotalVacantes = 0;
                int TotalContratados = 0;

                int[] estatus = { 4, 5, 6, 7, 29, 30, 31, 32, 33, 38, 39, 43, 44, 46 };
                if (user.TipoUsuarioId == 8 || user.TipoUsuarioId == 3 || user.TipoUsuarioId == 12 || user.TipoUsuarioId == 13 || user.TipoUsuarioId == 14)
                {
                    var vacantes = db.Requisiciones
                        .Where(r => estatus.Contains(r.EstatusId) && !r.Confidencial)
                        .Select(r => new
                        {
                            Vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                            contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(r.Id) && p.EstatusId == 24).Count(),

                        }).ToList();
                    foreach (var vtn in vacantes)
                    {
                        TotalVacantes = TotalVacantes + vtn.Vacantes;
                        TotalContratados = TotalContratados + vtn.contratados;
                    }
                }
                else
                {
                    if (db.Subordinados.Count(x => x.LiderId.Equals(user.Id)) > 0)
                    {
                        var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(user.Id) && x.LiderId.Equals(user.Id))
                            .Select(u => u.UsuarioId)
                            .ToList();
                        uids = GetSub(ids, uids);
                    }

                    uids.Add(user.Id);

                    var requis = db.AsignacionRequis
                        .OrderByDescending(e => e.Id)
                        .Where(a => uids.Distinct().Contains(a.GrpUsrId))
                        .Select(a => a.RequisicionId)
                        .ToList();

                    var vacantes = db.Requisiciones
                        .Where(r => requis.Contains(r.Id) || r.PropietarioId.Equals(user.Id))
                        .Where(r => estatus.Contains(r.EstatusId))
                         .Select(r => new
                         {
                             Vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                             contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(r.Id) && p.EstatusId == 24).Count(),

                         }).ToList();
                    foreach (var vtn in vacantes)
                    {
                        TotalVacantes = TotalVacantes + vtn.Vacantes;
                        TotalContratados = TotalContratados + vtn.contratados;
                    }
                }
                var obj = new
                {
                    Vacantes = TotalVacantes - TotalContratados,
                    Contratados = TotalContratados
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
        [Route("candidatosEstatus")]
        public IHttpActionResult GetCandidatosEstatus()
        {
            int Postulado = 0;
            int Proceso = 0;
            int Entrevista = 0;
            int Finalista = 0;
            int ProcesoFinalizado = 0;
            int NR = 0;
            int Revision = 0;
            int Disponible = 0;

            var activos = db.AspNetUsers.Where(a => a.Activo == 0 && a.IdPersona != null).Select(a => a.IdPersona).ToList();

            var postuladosBolsa = db.Database
                     //.GroupBy(p => p.CandidatoId)
                     .SqlQuery<Guid>("SELECT CandidatoId FROM Btra.Postulaciones WHERE StatusId = 1 AND CandidatoId NOT IN (SELECT CandidatoId FROM [Recl].[ProcesoCandidatos])  GROUP BY CandidatoId HAVING COUNT(*) > 0")
                    .ToList().Count();

            var estatusCandidatos = db.PerfilCandidato
                    .Where(c => activos.Contains(c.CandidatoId))
                    .Select(c => new
                    {
                        CandidatoId = c.CandidatoId,
                        Estatus = db.ProcesoCandidatos
                        .OrderByDescending(p => p.Fch_Modificacion)
                        .Where(p => p.CandidatoId.Equals(c.CandidatoId))
                        .Select(p => new
                        {
                            estatusId = p.Estatus.Id
                        })
                        .FirstOrDefault()
                    }).ToList();


            foreach (var x in estatusCandidatos)
            {
                if (x.Estatus != null)
                {
                    var estatusId = x.Estatus.estatusId;
                    if (estatusId == 12 || estatusId == 10)
                        Postulado++;
                    if (estatusId == 13 || estatusId == 14 || estatusId == 15)
                        Proceso++;
                    if (estatusId == 16 || estatusId == 17 || estatusId == 18)
                        Entrevista++;
                    if (estatusId == 21 || estatusId == 22 || estatusId == 23)
                        Finalista++;
                    if (estatusId == 24 || estatusId == 26 || estatusId == 27 || estatusId == 40)
                        ProcesoFinalizado++;
                    if (estatusId == 42)
                        Revision++;
                    if (estatusId == 28)
                        NR++;
                }
                else
                {
                    Disponible++;
                }

            }

            var obj = new
            {
                Postulado = Postulado + postuladosBolsa,
                Proceso = Proceso,
                Entrevista = Entrevista,
                Finalista = Finalista,
                ProcesoFinalizado = ProcesoFinalizado,
                Revision = Revision,
                NR = NR,
                Disponible = Disponible
            };

            return Ok(obj);
        }
    }
}