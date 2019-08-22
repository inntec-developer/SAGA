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
                totalCub = u.totalCub,
                totalPos = u.totalPos,
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
                    nodo.totalCub = nodo.totalCub + sumTotalCub(tree, nodo.totalCub, nodo.reclutadorId);
                }
            }
            return mocos.Sum(x => x.totalCub);
        }
        public int sumTotalPos(List<ResumenDto> tree, int inicio, Guid id)
        {
            var mocos = tree.Where(x => x.liderId == id).ToList();
            if (mocos.Count() > 0)
            {
                foreach (var nodo in mocos)
                {
                    nodo.totalPos = nodo.totalPos + sumTotalPos(tree, nodo.totalPos, nodo.reclutadorId);
                }
            }
            return mocos.Sum(x => x.totalPos);

        }

        [HttpGet]
        [Route("getRportGG")]
        public IHttpActionResult GetRportGG(Guid gg)
        {
            List<Guid> uids = new List<Guid>();
            int[] estatusId = { 8, 9, 34, 35, 36, 37, 47, 48 };
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
                        liderId = db.Subordinados.Where(x => x.UsuarioId.Equals(r.Id)).Select(l => l.LiderId).FirstOrDefault(),
                        reclutadorId = r.Id,
                        nombre = r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno,
                        clave = r.Clave,
                        tipoUsuario = r.TipoUsuarioId,
                        foto = @"https://apierp.damsa.com.mx/img/" + r.Clave + ".jpg",
                   
                        totalCub = db.AsignacionRequis.Where(x => !estatusId.Contains(x.Requisicion.EstatusId) && x.GrpUsrId.Equals(r.Id)).Count() > 0 ? db.AsignacionRequis.Where(x => !estatusId.Contains(x.Requisicion.EstatusId) && x.GrpUsrId.Equals(r.Id)).Select(sum => new
                        {
                            cont = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(sum.Requisicion.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(sum.GrpUsrId)).Select(cand => db.CandidatosInfo.Where(xx => xx.CandidatoId.Equals(cand.CandidatoId) && xx.ReclutadorId.Equals(cand.ReclutadorId))).Count(),
                        }).Sum(x => x.cont) : 0,

                        totalPos = db.AsignacionRequis.Where(x => !estatusId.Contains(x.Requisicion.EstatusId) && x.GrpUsrId.Equals(r.Id)).Count() > 0 ? db.AsignacionRequis.Where(x => !estatusId.Contains(x.Requisicion.EstatusId) && x.GrpUsrId.Equals(r.Id)).Select(sum => new
                        {
                            vac = sum.Requisicion.horariosRequi.Count() > 0 ? sum.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                        }).Sum(x => x.vac) : 0

                    }).ToList();

                    var nodesSum = tree.Where(x => x.reclutadorId.Equals(gg)).Select(r => new ResumenDto
                    {
                        reclutadorId = r.reclutadorId,
                        nombre = r.nombre,
                        clave = r.clave,
                        tipoUsuario = r.tipoUsuario,
                        totalCub = sumTotalCub(tree, r.totalCub, r.reclutadorId),
                        totalPos = sumTotalPos(tree, r.totalPos, r.reclutadorId),
                        //resumen = GetRportGG2(tree, r.reclutadorId),
                    }).ToList();

                    var nodes = tree.Where(x => x.reclutadorId.Equals(gg)).Select(r => new ResumenDto
                    {
                        liderId = r.liderId,
                        reclutadorId = r.reclutadorId,
                        nombre = r.nombre,
                        clave = r.clave,
                        tipoUsuario = r.tipoUsuario,
                        totalCub = nodesSum[0].totalCub,
                        totalPos = nodesSum[0].totalPos,
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

                var requis = db.AsignacionRequis.Where(x => uids.Distinct().Contains(x.GrpUsrId) && !estatusId.Contains(x.Requisicion.EstatusId)).Select(res => new
                {
                    requisicionId = res.Requisicion.Id,
                    estatusId = res.Requisicion.EstatusId,
                    folio = res.Requisicion.Folio,
                    cliente = res.Requisicion.Cliente.Nombrecomercial,
                    vBtra = res.Requisicion.VBtra,
                    fch_Cumplimiento = res.Requisicion.fch_Cumplimiento,
                    vacantes = res.Requisicion.horariosRequi.Count() > 0 ? res.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                    contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Requisicion.Id) && x.EstatusId.Equals(24)).Count(),
                    coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(res.Requisicion.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(res.Requisicion.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                    reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(res.RequisicionId) && !x.GrpUsrId.Equals(res.Requisicion.AprobadorId)).Select(a =>
                                   db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                   ),
                    faltantes = (res.Requisicion.horariosRequi.Count() > 0 ? res.Requisicion.horariosRequi.Sum(v => v.numeroVacantes) : 0) - (db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Requisicion.Id) && x.EstatusId.Equals(24) && x.ReclutadorId.Equals(res.GrpUsrId)).Select(cs => db.CandidatosInfo.Where(xx => xx.CandidatoId.Equals(cs.CandidatoId) && xx.ReclutadorId.Equals(cs.ReclutadorId))).Count()),
                    cumplimiento = res.Requisicion.horariosRequi.Count() > 0 ? db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Requisicion.Id) && x.EstatusId.Equals(24)).Count() * 100 / res.Requisicion.horariosRequi.Count() : 0,
                }).ToList();


                if (orden == 1)
                {
                    requis = requis.OrderByDescending(o => o.vacantes).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                }
                else if(orden == 2)
                {
                    requis = requis.OrderByDescending(o => o.contratados).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                }
                else if (orden == 4)
                {
                    requis = requis.OrderByDescending(o => o.faltantes).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                }
                else
                {
                    requis = requis.OrderByDescending(o => o.cumplimiento).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                }

                return Ok(requis);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }

    }
}
