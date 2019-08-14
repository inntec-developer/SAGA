using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using AutoMapper;
using System.Data.Entity;
using System.Web;
using System.IO;
using System.Drawing;
using SAGA.API.Utilerias;
using System.Data.SqlClient;
using SAGA.API.Dtos.Equipos;

namespace SAGA.API.Controllers.Equipos
{
    [RoutePrefix("api/Equipos")]
    public class EquiposController : ApiController
    {
        private SAGADBContext db;
        public EquiposController()
        {
            db = new SAGADBContext();
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
            //if (!listaIds.Contains(uid))
            //{
            //    listaIds.Add(uid);
            //    var listadoNuevo = db.Subordinados
            //        .Where(g => g.LiderId.Equals(uid))
            //               .Select(g => g.UsuarioId)
            //               .ToList();
            //    foreach (Guid g in listadoNuevo)
            //    {
            //        var gp = db.Subordinados
            //            .Where(x => x.LiderId.Equals(g))
            //            .Select(x => x.UsuarioId)
            //            .ToList();
            //        foreach (Guid gr in gp)
            //        {
            //            GetSub(gr, listaIds);
            //        }
            //    }
            //}
            return listaIds;
        }

        [HttpGet]
        [Route("getRportGG")]
        public IHttpActionResult GetRportGG(Guid gg)
        {
            List<Guid> uids = new List<Guid>();
            List<Guid> requisids = new List<Guid>();
            try
            {
                var val = db.Subordinados.Where(x => x.LiderId.Equals(gg)).Count();

                if (val > 0)
                {
                    var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(gg) && x.LiderId.Equals(gg)).Select(u => new { usuarioId = u.UsuarioId, tipo = u.Usuario.TipoUsuarioId }).ToList();

                    foreach (var c in ids)
                    {
                        var id = db.Subordinados.Where(x => !x.UsuarioId.Equals(c.usuarioId) && x.LiderId.Equals(c.usuarioId)).Select(u => u.UsuarioId).ToList();

                        uids = GetSub(id, uids);

                        var requis = db.AsignacionRequis.Where(x => uids.Contains(x.GrpUsrId)).Select(r => r.RequisicionId).ToList();
                        var resumenrequis = db.Requisiciones.Where(x => requis.Distinct().Contains(x.Id)).Select(r => new
                        {
                            reclutadorId = c.usuarioId,
                            nombre = db.Usuarios.Where(x => x.Id.Equals(c.usuarioId)).Select(ng => ng.Nombre + " " + ng.ApellidoPaterno + " " + ng.ApellidoMaterno).FirstOrDefault(),
                            requisicionId = r.Id,
                            vBtra = r.VBtra,
                            folio = r.Folio,
                            contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(r.Id) && x.EstatusId.Equals(24)).Count(),
                            vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id)).Select(rs => new ResumenDto
                            {
                                requisicionId = rs.RequisicionId,
                                vacantes = rs.Requisicion.horariosRequi.Count() > 0 ? rs.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                                reclutadorId = rs.GrpUsrId,
                                nombre = rs.GrpUsr.Nombre + " " + rs.GrpUsr.ApellidoPaterno + " " + rs.GrpUsr.ApellidoMaterno,
                                contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(r.Id) && x.EstatusId.Equals(24)).Select(cs => db.CandidatosInfo.Where(x => x.CandidatoId.Equals(cs.CandidatoId) && x.ReclutadorId.Equals(rs.GrpUsrId))).Count(),
                            }).ToList()
                        }).ToList();

                        var totalCub = resumenrequis.Select(x => x.contratados).Sum();
                        var totalPos = resumenrequis.Select(x => x.vacantes).Sum();
                        var totalFal = totalPos - totalCub;
                        var totalCump = totalCub * 100 / totalPos;

                        var totalCubSub = resumenrequis.Select(x => x.reclutadores).Sum(s => s.Sum(ss => ss.contratados));
                        var totalPosSub = resumenrequis.Select(x => x.reclutadores).Sum(s => s.Sum(ss => ss.vacantes));
                        var totalFalSub = Convert.ToInt16(totalPosSub) - Convert.ToInt16(totalCubSub);
                        var totalCumpSub = Convert.ToInt16(totalCubSub) * 100 / Convert.ToInt16(totalPosSub);

                        //var totalPos = requis.Select(x => x.requisicionesId).ToList().Distinct().Select(xx => db.Requisiciones.Where(r => r.Id.Equals(xx)).Select(R => R.horariosRequi.Count() > 0 ? R.horariosRequi.Sum(v => v.numeroVacantes) : 0)).ToList();
                        //var totalCub = requis.Select(x => x.requisicionesId).ToList().Distinct().Select(xx => db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(xx) && p.EstatusId.Equals(24))).ToList();

                        //var mocos = new
                        //{
                        //    gerenteId = c.usuarioId,

                        //    sub = uids.Select(u => db.Usuarios.Where(x => x.Id.Equals(u)).Select(n => n.Nombre + "  " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()),

                        //    totalFal = totalPos.Sum(x => x.) - totalCub.Sum(),
                        //    cumplimiento = resumen.Select(x => x.contratados).Sum() * 100 / resumen.Select(x => x.Vacantes).Sum()
                        //};


                    }
                    return Ok("mocos");
                }
                else
                {
                    return Ok();
                }

              
            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
