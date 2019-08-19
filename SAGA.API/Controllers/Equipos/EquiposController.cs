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
            return listaIds;
        }

        public List<ResumenDto> GetRportGG2(List<ResumenDto> tree, Guid gg)
        {
            var mocos = tree.Where(x => x.liderId == gg).Select(u => new ResumenDto
            {
                liderId = u.liderId,
                reclutadorId = u.reclutadorId,
                nombre = u.nombre,
                clave = u.clave,
                tipoUsuario = u.tipoUsuario,
                requis = u.requis,
                totalCub = u.totalCub,
                totalPos = u.totalPos,
                resumen = GetRportGG2(tree,u.reclutadorId),

            }).ToList();

            return mocos;
        }

        public int sumrec(List<ResumenDto> tree, List<int> totCub)
        {

            foreach (var c in tree)
            {
                totCub.Add(c.totalCub);
  
                if (c.resumen.Count > 0)
                {
                    c.totalCub = sumrec(c.resumen, totCub);
                }
            }


            return totCub.Sum();
        }
        public int sumTotalPos(List<ResumenDto> tree, List<int> totPos)
        {

            foreach (var c in tree)
            {
                totPos.Add(c.totalPos);
                if (c.resumen.Count > 0)
                {
                    c.totalPos = sumTotalPos(c.resumen, totPos);
                }
            }


            return totPos.Sum();
        }

        [HttpGet]
        [Route("getRportGG")]
        public IHttpActionResult GetRportGG(Guid gg)
        {

            List<Guid> uids = new List<Guid>();
            List<ResumenDto> totales = new List<ResumenDto>();
            try
            {
               
        var val = db.Subordinados.Where(x => x.LiderId.Equals(gg)).Count();

                if (val > 0)
                {
                    var gerentes = db.Subordinados.Where(x => !x.UsuarioId.Equals(gg) && x.LiderId.Equals(gg)).Select(u => u.UsuarioId).ToList();

                    uids = GetSub(gerentes, uids);
                    uids.Add(gg);

                    var tree = db.Usuarios.Where(x => uids.Distinct().Contains(x.Id)).Select(r => new ResumenDto
                    {
                        liderId = db.Subordinados.Where(x => x.UsuarioId.Equals(r.Id)).Select(l=> l.LiderId).FirstOrDefault(),
                        reclutadorId = r.Id,
                        nombre = r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno,
                        clave = r.Clave,
                        tipoUsuario = r.TipoUsuarioId,
                       
                        requis = db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(r.Id)).Select(res => new RequisDtos
                        {
                            requisicionId = res.Requisicion.Id,
                            folio = res.Requisicion.Folio,
                            vBtra = res.Requisicion.VBtra,
                            vacantes = res.Requisicion.horariosRequi.Count() > 0 ? res.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                            contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Requisicion.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(res.GrpUsrId)).Select(cs => db.CandidatosInfo.Where(xx => xx.CandidatoId.Equals(cs.CandidatoId) && xx.ReclutadorId.Equals(cs.ReclutadorId))).Count(),
                        }).ToList(),

                        totalCub = db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(r.Id)).Count() > 0 ? db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(r.Id)).Select(sum => new
                        {
                            cont = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(sum.Requisicion.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(sum.GrpUsrId)).Select(cand => db.CandidatosInfo.Where(xx => xx.CandidatoId.Equals(cand.CandidatoId) && xx.ReclutadorId.Equals(cand.ReclutadorId))).Count(),
                        }).Sum(x => x.cont) : 0,

                        totalPos = db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(r.Id)).Count() > 0 ? db.AsignacionRequis.Where(x => x.GrpUsrId.Equals(r.Id)).Select(sum => new
                        {
                            vac = sum.Requisicion.horariosRequi.Count() > 0 ? sum.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                        }).Sum(x => x.vac) : 0

                    }).ToList();


                    var nodes = tree.Where(x => x.tipoUsuario.Equals(14)).Select(r => new ResumenDto
                    {
                        reclutadorId = r.reclutadorId,
                        nombre = r.nombre,
                        clave = r.clave,
                        tipoUsuario = r.tipoUsuario,
                        totalCub = r.totalCub,
                        totalPos = r.totalPos,
                        resumen = GetRportGG2(tree, r.reclutadorId),        
                    }).ToList();

                
                    var resumen = nodes.Select(rr => new {
                        reclutadorId = rr.reclutadorId,
                        nombre = rr.nombre,
                        clave = rr.clave,
                        tipoUsuario = rr.tipoUsuario,
                        resumen = rr.resumen,
                        totalCub = sumrec(nodes, new List<int>()),
                        totalPos = sumTotalPos(nodes, new List<int>())
   
                    });
                    uids.Clear();

                    //foreach (var c in gerentes)
                    //{
                    //    var coords = db.Subordinados.Where(x => x.LiderId.Equals(c)).Select(u => u.UsuarioId).ToList();

                    //    foreach (var row in coords)
                    //    {
                    //        var reclutadores = db.Subordinados.Where(x => !x.UsuarioId.Equals(row) && x.LiderId.Equals(row)).Select(u => u.UsuarioId).ToList();
                    //        uids = GetSub(reclutadores, uids);

                    //        uids.Add(row);

                         
                    //            var resum = new {
                    //            reclutadorId = row,
                    //            nombre = db.Usuarios.Where(x => x.Id.Equals(row)).Select(ng => ng.Nombre + " " + ng.ApellidoPaterno + " " + ng.ApellidoMaterno).FirstOrDefault(),
                    //            clave = db.Usuarios.Where(x => x.Id.Equals(row)).Select(cl => cl.Clave).FirstOrDefault(),
                    //            tipoUsuario = db.Usuarios.Where(x => x.Id.Equals(row)).Select(tu => tu.TipoUsuarioId).FirstOrDefault(),
                    //            resumen = db.AsignacionRequis.Where(x => uids.Contains(x.GrpUsrId)).GroupBy(g => g.GrpUsrId)
                    //            .Select(r => new ResumenDto
                    //            {
                    //                reclutadorId = r.Key,
                    //                nombre = db.Usuarios.Where(x => x.Id.Equals(r.Key)).Select(ng => ng.Nombre + " " + ng.ApellidoPaterno + " " + ng.ApellidoMaterno).FirstOrDefault(),
                    //                clave = db.Usuarios.Where(x => x.Id.Equals(r.Key)).Select(cl => cl.Clave).FirstOrDefault(),
                    //                tipoUsuario = db.Usuarios.Where(x => x.Id.Equals(r.Key)).Select(tu => tu.TipoUsuarioId).FirstOrDefault(),
                    //                requis = r.Select(res => new RequisDtos {
                    //                    requisicionId = res.Requisicion.Id,
                    //                    folio = res.Requisicion.Folio,
                    //                    vBtra = res.Requisicion.VBtra,
                    //                    vacantes = res.Requisicion.horariosRequi.Count() > 0 ? res.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                    //                    contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Requisicion.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(res.GrpUsrId)).Select(cs => db.CandidatosInfo.Where(xx => xx.CandidatoId.Equals(cs.CandidatoId) && xx.ReclutadorId.Equals(cs.ReclutadorId))).Count(),
                    //                }).ToList(),
                    //                totalCub = r.Select(sum => new
                    //                {
                    //                    cont = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(sum.Requisicion.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(r.Key)).Select(cand => cand.CandidatoId).Count(),
                    //                }).Sum(x => x.cont),
                    //                totalPos = r.Select(sum => new
                    //                {
                    //                    cont = sum.Requisicion.horariosRequi.Count() > 0 ? sum.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                    //                }).Sum(x => x.cont),

                    //                totalFal = r.Select(sum => new
                    //                            {
                    //                                cont = sum.Requisicion.horariosRequi.Count() > 0 ? sum.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                    //                            }).Sum(x => x.cont) - r.Select(sum => new
                    //                            {
                    //                                cont = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(sum.Requisicion.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(r.Key)).Select(cand => cand.CandidatoId).Count(),
                    //                            }).Sum(x => x.cont),
                    //                totalCump = r.Select(sum => new
                    //                            {
                    //                                cont = sum.Requisicion.horariosRequi.Count() > 0 ? sum.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                    //                            }).Sum(x => x.cont) > 0 ? r.Select(sum => new
                    //                                                        {
                    //                                                            cont = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(sum.Requisicion.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(r.Key)).Select(cand => cand.CandidatoId).Count(),
                    //                                                        }).Sum(x => x.cont) * 100 / r.Select(sum => new
                    //                                                        {
                    //                                                            cont = sum.Requisicion.horariosRequi.Count() > 0 ? sum.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                    //                                                        }).Sum(x => x.cont) : 0
                    //            }).ToList()
                    //    };

                    //        ResumenDto rd = new ResumenDto();

                    //        rd.reclutadorId = resum.reclutadorId;
                    //        rd.nombre = resum.nombre;
                    //        rd.clave = resum.clave;
                    //        rd.tipoUsuario = resum.tipoUsuario;
                    //        rd.resumen = resum.resumen;
                    //        rd.totalCub = resum.resumen.Sum(s => s.totalCub);
                    //        rd.totalPos = resum.resumen.Sum(s => s.totalPos);
                    //        rd.totalFal = resum.resumen.Sum(s => s.totalPos) - resum.resumen.Sum(s => s.totalCub);
                    //        rd.totalCump = resum.resumen.Sum(s => s.totalPos) > 0 ? resum.resumen.Sum(s => s.totalCub) * 100 / resum.resumen.Sum(s => s.totalPos) : 0;

                    //        totales.Add(rd);

                    //        uids.Clear();


                    //    }
                       

                    //}
                    return Ok(resumen);
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
