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
            List<ResumenDto> totales = new List<ResumenDto>();
            try
            {
                var val = db.Subordinados.Where(x => x.LiderId.Equals(gg)).Count();

                if (val > 0)
                {
                    var gerentes = db.Subordinados.Where(x => x.LiderId.Equals(gg)).Select(u => new { usuarioId = u.UsuarioId, tipo = u.Usuario.TipoUsuarioId }).ToList();

                    foreach (var c in gerentes)
                    {
                        var coords = db.Subordinados.Where(x => x.LiderId.Equals(c.usuarioId)).Select(u => u.UsuarioId).ToList();

                        foreach (var row in coords)
                        {
                            var reclutadores = db.Subordinados.Where(x => !x.UsuarioId.Equals(row) && x.LiderId.Equals(row)).Select(u => u.UsuarioId).ToList();
                            uids = GetSub(reclutadores, uids);

                            uids.Add(row);

                            var resum = new {
                                reclutadorId = row,
                                nombre = db.Usuarios.Where(x => x.Id.Equals(row)).Select(ng => ng.Nombre + " " + ng.ApellidoPaterno + " " + ng.ApellidoMaterno).FirstOrDefault(),
                                clave = db.Usuarios.Where(x => x.Id.Equals(row)).Select(cl => cl.Clave).FirstOrDefault(),
                                tipoUsuario = db.Usuarios.Where(x => x.Id.Equals(row)).Select(tu => tu.TipoUsuarioId).FirstOrDefault(),
                                resumen = db.AsignacionRequis.Where(x => uids.Contains(x.GrpUsrId)).GroupBy(g => g.GrpUsrId)
                                .Select(r => new ResumenDto
                                {
                                    reclutadorId = r.Key,
                                    nombre = db.Usuarios.Where(x => x.Id.Equals(r.Key)).Select(ng => ng.Nombre + " " + ng.ApellidoPaterno + " " + ng.ApellidoMaterno).FirstOrDefault(),
                                    clave = db.Usuarios.Where(x => x.Id.Equals(r.Key)).Select(cl => cl.Clave).FirstOrDefault(),
                                    tipoUsuario = db.Usuarios.Where(x => x.Id.Equals(r.Key)).Select(tu => tu.TipoUsuarioId).FirstOrDefault(),
                                    requis = r.Select(res => new RequisDtos {
                                        requisicionId = res.Requisicion.Id,
                                        folio = res.Requisicion.Folio,
                                        vBtra = res.Requisicion.VBtra,
                                        vacantes = res.Requisicion.horariosRequi.Count() > 0 ? res.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                                        contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Requisicion.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(res.GrpUsrId)).Select(cs => db.CandidatosInfo.Where(xx => xx.CandidatoId.Equals(cs.CandidatoId) && xx.ReclutadorId.Equals(cs.ReclutadorId))).Count(),
                                    }).ToList(),
                                    totalCub = r.Select(sum => new
                                    {
                                        cont = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(sum.Requisicion.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(r.Key)).Select(cand => cand.CandidatoId).Count(),
                                    }).Sum(x => x.cont),
                                    totalPos = r.Select(sum => new
                                    {
                                        cont = sum.Requisicion.horariosRequi.Count() > 0 ? sum.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                                    }).Sum(x => x.cont),

                                    totalFal = r.Select(sum => new
                                                {
                                                    cont = sum.Requisicion.horariosRequi.Count() > 0 ? sum.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                                                }).Sum(x => x.cont) - r.Select(sum => new
                                                {
                                                    cont = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(sum.Requisicion.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(r.Key)).Select(cand => cand.CandidatoId).Count(),
                                                }).Sum(x => x.cont),
                                    totalCump = r.Select(sum => new
                                                {
                                                    cont = sum.Requisicion.horariosRequi.Count() > 0 ? sum.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                                                }).Sum(x => x.cont) > 0 ? r.Select(sum => new
                                                                            {
                                                                                cont = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(sum.Requisicion.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(r.Key)).Select(cand => cand.CandidatoId).Count(),
                                                                            }).Sum(x => x.cont) * 100 / r.Select(sum => new
                                                                            {
                                                                                cont = sum.Requisicion.horariosRequi.Count() > 0 ? sum.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                                                                            }).Sum(x => x.cont) : 0
                                }).ToList()
                        };

                            ResumenDto rd = new ResumenDto();

                            rd.reclutadorId = resum.reclutadorId;
                            rd.nombre = resum.nombre;
                            rd.clave = resum.clave;
                            rd.tipoUsuario = resum.tipoUsuario;
                            rd.resumen = resum.resumen;
                            rd.totalCub = resum.resumen.Sum(s => s.totalCub);
                            rd.totalPos = resum.resumen.Sum(s => s.totalPos);
                            rd.totalFal = resum.resumen.Sum(s => s.totalPos) - resum.resumen.Sum(s => s.totalCub);
                            rd.totalCump = resum.resumen.Sum(s => s.totalPos) > 0 ? resum.resumen.Sum(s => s.totalCub) * 100 / resum.resumen.Sum(s => s.totalPos) : 0;

                            totales.Add(rd);

                            uids.Clear();


                        }
                       

                        
                        //var resumenrequis = db.Usuarios.Where(x => x.Id.Equals(c.usuarioId)).Select(U => new
                        //{
                        //    reclutadorId = c.usuarioId,
                        //    nombre = db.Usuarios.Where(x => x.Id.Equals(c.usuarioId)).Select(ng => ng.Nombre + " " + ng.ApellidoPaterno + " " + ng.ApellidoMaterno).FirstOrDefault(),
                        //    resumencoord = db.Requisiciones.Where(x => requis.Distinct().Contains(x.Id)).Select(r => new
                        //    {
                        //        requisicionId = r.Id,
                        //        vBtra = r.VBtra,
                        //        folio = r.Folio,
                        //        contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(r.Id) && x.EstatusId.Equals(24)).Count(),
                        //        vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                        //        reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id)).GroupBy(g => g.GrpUsrId).Select(rs => new 
                        //        {
                        //            reclutadorId = rs.Key,
                        //            nombre = rs.Select( n => n.GrpUsr.Nombre + " " + n.GrpUsr.ApellidoPaterno + " " + n.GrpUsr.ApellidoMaterno).FirstOrDefault(),
                        //            resumen = rs.Select( res => new
                        //            {
                        //                requisicionId = res.Requisicion.Id,
                        //                folio = res.Requisicion.Folio,
                        //                vBtra = res.Requisicion.VBtra,
                        //                vacantes = res.Requisicion.horariosRequi.Count() > 0 ? res.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                        //                contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(r.Id) && x.EstatusId.Equals(24) && !x.Requisicion.AprobadorId.Equals(res.GrpUsrId) && x.ReclutadorId.Equals(res.GrpUsrId)).Select(cs => db.CandidatosInfo.Where(xx => xx.CandidatoId.Equals(cs.CandidatoId) && xx.ReclutadorId.Equals(cs.ReclutadorId))).Count(),
                        //            })
                        //        }).ToList()
                        //    }).ToList()
                        //}).ToList();

                  

                        //var totalCub = 0;
                        //var totalPos = 0;
                        //foreach (var row in resumenrequis)
                        //{
                        //    foreach(var r in row.resumencoord)
                        //    {
                        //        totalCub = totalCub + r.contratados;
                        //        totalPos = totalPos + r.vacantes;

                        //    }

                        //}
                      
                        //var totalFal = totalPos - totalCub;
                        //var totalCump = totalCub * 100 / totalPos;

                        //var totalCubSub = Convert.ToInt16(resumenrequis.Select(x => x.resumencoord.Select(xx => xx.reclutadores.Sum(s => s.contratados))));
                        //var totalPosSub = Convert.ToInt16(resumenrequis.Select(x => x.resumencoord.Select(xx => xx.reclutadores.Sum(s => s.vacantes))));
                        //var totalFalSub = Convert.ToInt16(totalPosSub) - Convert.ToInt16(totalCubSub);
                        //var totalCumpSub = Convert.ToInt16(totalCubSub) * 100 / Convert.ToInt16(totalPosSub);

                        
                       

                        //var aux = resumenrequis.Select(rr => new ResumenDto
                        //{
                        //    reclutadorId = rr.reclutadorId,
                        //    nombre = rr.nombre,
                        //    resumencoord = rr.resumencoord.Select(coord => new ResumenDto
                        //    {
                        //        requisicionId = coord.requisicionId,
                        //        vBtra = coord.vBtra,
                        //        folio = coord.folio,
                        //        contratados = coord.contratados,
                        //        vacantes = coord.vacantes,
                        //        reclutadores = coord.reclutadores.Select( rrr => new ResumenDto
                        //        {
                        //            reclutadorId = rrr.reclutadorId,
                        //            nombre = rrr.nombre,
                        //            requisicionId = rrr.requisicionId,
                        //            vBtra = rrr.vBtra,
                        //            folio = rrr.folio,
                        //            contratados = rrr.contratados,
                        //            vacantes = rrr.vacantes,
                        //            totalCub = totalCubSub,
                        //            totalPos = totalPosSub,
                        //            totalFal = totalFalSub,
                        //            totalCump = totalPosSub > 0 ? totalCubSub * 100 / totalPosSub : 0

                        //        }).ToList()
                        //    }).ToList(),

                            //totalCub = totalCub,
                            //totalPos = totalPos,
                            //totalFal = totalFal,
                            //totalCump = totalPos > 0 ? totalCub * 100 / totalPos : 0,
                         
                    //}).ToList();

                        //foreach (var r in aux)
                        //{

                         
                        //    totales.Add(r);
                        //}

                    }
                    return Ok(totales);
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
