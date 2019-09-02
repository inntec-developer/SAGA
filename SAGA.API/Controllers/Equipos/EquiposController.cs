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
        public List<RequisDtos> AddRequis(List<ResumenDto> tree, List<RequisDtos> inicio, Guid id)
        {

            var mocos = tree.Where(x => x.liderId.Equals(id)).ToList();
            if (mocos.Count() > 0)
            {
                foreach (var nodo in mocos)
                {

                    inicio.AddRange(nodo.requis);
                    AddRequis(tree, inicio, nodo.reclutadorId);
                    // inicio.AddRange(nodo.requis);
                }
            }
            return inicio;
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
                totalCub = u.totalCub,
                totalPos = u.totalPos,
                totalPosAux = u.totalPosAux,
                requis = u.requisAux,
                foto = u.foto,
                resumen = GetRportGG2(tree, u.reclutadorId),

            }).ToList();

            return mocos;
        }

        public int sumTotalCub(List<ResumenDto> tree, int inicio, Guid id)
        {
            var mocos = tree.Where(x => x.liderId == id).ToList();
            if (mocos.Count() > 0)
            {
                foreach (var nodo in mocos)
                {
                    var suma = nodo.requis.OrderBy(o => o.requisicionId).Select(rs => new { rs.requisicionId, rs.cubiertas }).Distinct().Sum(s => s.cubiertas);
                    inicio += sumTotalCub(tree, suma, nodo.reclutadorId);
                }
            }
            return inicio;
        }
        public int sumTotalPos(List<ResumenDto> tree, int inicio, Guid id)
        {
            var mocos = tree.Where(x => x.liderId == id).ToList();
            if (mocos.Count() > 0)
            {
                foreach (var nodo in mocos)
                {
                    var suma = nodo.requis.OrderBy(o => o.requisicionId).Select(rs => new { rs.requisicionId, rs.vacantes }).Distinct().Sum(s => s.vacantes);
                    inicio += sumTotalPos(tree, suma, nodo.reclutadorId);
                }
            }
            return inicio;
        }

        [HttpGet]
        [Route("getRportGG")]
        public IHttpActionResult GetRportGG(Guid gg)
        {
            List<Guid> uids = new List<Guid>();
            int[] estatusId = new int[] { 8, 9, 34, 35, 36, 37, 47, 48 };
            try
            {
                var gerentes = db.Subordinados.Where(x => !x.UsuarioId.Equals(gg) && x.LiderId.Equals(gg)).Select(u => u.UsuarioId).ToList();

                if (gerentes.Count() > 0)
                {
                   
                    uids = GetSub(gerentes, uids);
                    uids.Add(gg);


                    var asignadas = db.AsignacionRequis
                      .OrderByDescending(e => e.Id)
                      .Where(a => uids.Distinct().Contains(a.GrpUsrId)
                          && !estatusId.Contains(a.Requisicion.EstatusId) && !a.Requisicion.Confidencial)
                      .Select(a => a.RequisicionId)
                      .Distinct()
                      .ToList();

                    var requis = db.Requisiciones
                    .Where(e => (uids.Contains(e.AprobadorId) || uids.Contains(e.PropietarioId))
                        && !estatusId.Contains(e.EstatusId) && !e.Confidencial)
                    .Select(a => a.Id).ToList();

                    var AllRequis = requis.Union(asignadas).Distinct().ToList();

                    var tree = db.Usuarios.Where(x => uids.Distinct().Contains(x.Id)).Select(r => new ResumenDto
                    {
                        liderId = db.Subordinados.Where(x => x.UsuarioId.Equals(r.Id)).Select(l => l.LiderId).FirstOrDefault(),
                        reclutadorId = r.Id,
                        nombre = r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno,
                        clave = r.Clave,
                        tipoUsuario = r.TipoUsuarioId,
                        foto = @"https://apierp.damsa.com.mx/img/" + r.Clave + ".jpg",
                        requis = db.AsignacionRequis.Where(x => AllRequis.Contains(x.RequisicionId) && (x.GrpUsrId.Equals(r.Id) || x.Requisicion.AprobadorId.Equals(r.Id) || x.Requisicion.PropietarioId.Equals(r.Id))).Select(sum => new RequisDtos
                        {
                            folio = sum.Requisicion.Folio,
                            requisicionId = sum.RequisicionId,
                            vBtra = sum.Requisicion.Estatus.Descripcion,
                            vacantes = sum.Requisicion.horariosRequi.Count() > 0 ? sum.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                            cubiertas = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(sum.RequisicionId) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(r.Id)).Count()
                        }).Union(db.Requisiciones.Where(xx => AllRequis.Contains(xx.Id) && (xx.AprobadorId.Equals(r.Id) || xx.PropietarioId.Equals(r.Id))).Select(rr => new RequisDtos
                        {
                            folio = rr.Folio,
                            requisicionId = rr.Id,
                            vBtra = rr.Estatus.Descripcion,
                            vacantes = rr.horariosRequi.Sum(s => s.numeroVacantes) > 0 ? rr.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                            cubiertas = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(rr.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(r.Id)).Count()
                        })).ToList(),
                        requisAux = db.AsignacionRequis.Where(x => AllRequis.Contains(x.RequisicionId) && (x.GrpUsrId.Equals(r.Id) || x.Requisicion.AprobadorId.Equals(r.Id) || x.Requisicion.PropietarioId.Equals(r.Id))).Select(sum => new RequisDtos
                        {
                            folio = sum.Requisicion.Folio,
                            requisicionId = sum.RequisicionId,
                            vBtra = sum.Requisicion.Estatus.Descripcion,
                            vacantes = sum.Requisicion.horariosRequi.Sum(s => s.numeroVacantes) > 0 ? sum.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                            cubiertas = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(sum.RequisicionId) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(r.Id)).Count()
                        }).Union(db.Requisiciones.Where(xx => AllRequis.Contains(xx.Id) && (xx.AprobadorId.Equals(r.Id) || xx.PropietarioId.Equals(r.Id))).Select(rr => new RequisDtos{
                            folio = rr.Folio,
                            requisicionId = rr.Id,
                            vBtra = rr.Estatus.Descripcion,
                            vacantes = rr.horariosRequi.Sum(s => s.numeroVacantes) > 0 ? rr.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                            cubiertas = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(rr.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(r.Id)).Count()
                        })).ToList(),
                        totalPosAux = db.AsignacionRequis.Where(x => AllRequis.Contains(x.RequisicionId) && (x.GrpUsrId.Equals(r.Id) || x.Requisicion.AprobadorId.Equals(r.Id) || x.Requisicion.PropietarioId.Equals(r.Id))).Count() > 0 ? db.AsignacionRequis.Where(x => AllRequis.Contains(x.RequisicionId) && (x.GrpUsrId.Equals(r.Id) || x.Requisicion.AprobadorId.Equals(r.Id) || x.Requisicion.PropietarioId.Equals(r.Id))).Select(sum => new RequisDtos
                        {
                            folio = sum.Requisicion.Folio,
                            requisicionId = sum.RequisicionId,
                            vBtra = sum.Requisicion.Estatus.Descripcion,
                            vacantes = sum.Requisicion.horariosRequi.Count() > 0 ? sum.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                            cubiertas = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(sum.RequisicionId) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(r.Id)).Count()
                        }).OrderBy(o => o.folio).Select(sss => new { requisicionId = sss.requisicionId, vacantes = sss.vacantes}).Distinct().Sum(s => s.vacantes) : 0,
                    }).ToList();
    
                    foreach(var nodo in tree)
                    {
                        nodo.requis = AddRequis(tree, nodo.requis, nodo.reclutadorId);
                        nodo.totalPos = nodo.requis.OrderBy(o => o.folio).Select(rr => new { folio = rr.folio, vacantes = rr.vacantes}).Distinct().Sum(s => s.vacantes);
                        nodo.totalCub = nodo.requis.OrderBy(o => o.requisicionId).Select(rr => new { requisicionId = rr.requisicionId, cubiertas = rr.cubiertas }).Distinct().Sum(s => s.cubiertas);
                    }
                 
                    var nodes = tree.Where(x => x.reclutadorId.Equals(gg)).Select(r => new
                    {
                        liderId = r.liderId,
                        reclutadorId = r.reclutadorId,
                        nombre = r.nombre,
                        clave = r.clave,
                        tipoUsuario = r.tipoUsuario,
                        totalCub = r.totalCub,
                        totalPos = r.totalPos,
                        totalPosAux = r.totalPosAux,
                        requis = r.requisAux,
                        foto = r.foto,
                        resumen = GetRportGG2(tree, r.reclutadorId),
                    }).ToList();


                    return Ok(nodes);
                }
                else
                {
                    return Ok(HttpStatusCode.ExpectationFailed);
                }


            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet]
        [Route("getRportTable")]
        public IHttpActionResult GetRportTable(Guid usuario, int orden)
        {
            //1 posiciones activas. 2 cubiertas. 3 faltantes. 4 cumplimiento
            List<Guid> uids = new List<Guid>();
            int[] estatusId = { 8, 9, 34, 35, 36, 37, 47, 48 };
            try
            {
                var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(usuario) && x.LiderId.Equals(usuario)).Select(u => u.UsuarioId).ToList();
                uids = GetSub(ids, uids);

                uids.Add(usuario);

                var asignadas = db.AsignacionRequis
                            .OrderByDescending(e => e.Id)
                            .Where(a => uids.Distinct().Contains(a.GrpUsrId)
                                && !estatusId.Contains(a.Requisicion.EstatusId) && !a.Requisicion.Confidencial)
                            .Select(a => a.RequisicionId)
                            .Distinct()
                            .ToList();

                var requis = db.Requisiciones
                          .Where(e => (uids.Contains(e.AprobadorId) || uids.Contains(e.PropietarioId))
                              && !estatusId.Contains(e.EstatusId) && !e.Confidencial)
                          .Select(a => a.Id).ToList();


                var AllRequis = requis.Union(asignadas).Distinct().ToList();

                var total = db.Requisiciones.Where(x => AllRequis.Contains(x.Id)).Select(res => new
                {
                    requisicionId = res.Id,
                    estatusId = res.EstatusId,
                    folio = res.Folio,
                    cliente = res.Cliente.Nombrecomercial,
                    vBtra = res.VBtra,
                    fch_Cumplimiento = res.fch_Cumplimiento,
                    vacantes = res.horariosRequi.Sum(s => s.numeroVacantes) > 0 ? res.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                    contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Id) && x.EstatusId.Equals(24)).Count(),
                    coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(res.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(res.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                    reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(res.Id) && !x.GrpUsrId.Equals(res.AprobadorId)).Select(a =>
                                   db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                   ),
                    faltantes = (res.horariosRequi.Count() > 0 ? res.horariosRequi.Sum(v => v.numeroVacantes) : 0) - db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Id) && x.EstatusId.Equals(24)).Count(),
                    cumplimiento = res.horariosRequi.Sum(s => s.numeroVacantes) > 0 ? db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Id) && x.EstatusId.Equals(24)).Count() * 100 / res.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                }).ToList();

                var mocos2 = total.Sum(s => s.vacantes);
                if (orden == 1)
                {
                    total = total.OrderByDescending(o => o.vacantes).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                }
                else if(orden == 2)
                {
                    total = total.OrderByDescending(o => o.contratados).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                }
                else if (orden == 3)
                {
                    total = total.OrderByDescending(o => o.faltantes).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                }
                else
                {
                    total = total.OrderByDescending(o => o.cumplimiento).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                }

                return Ok(total);
                 
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }

    }
}
