using SAGA.API.Utilerias;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Equipos
{
    [RoutePrefix("api/Equipos")]
    public class ClientesController : ApiController
    {
        private SAGADBContext db;
        public ClientesController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getRportClientes")]
        public IHttpActionResult GetRportClientes(Guid usuarioId)
        {
            List<Guid> uids = new List<Guid>();
            int[] estatusId = { 8, 9, 34, 35, 36, 37, 47, 48 };
            int?[] unidadNegocio = { 0 };

            GetSub obj = new GetSub();
           
            try
            {
                var tipo = db.Usuarios.Where(x => x.Id.Equals(usuarioId)).Select(t => t.TipoUsuarioId).FirstOrDefault();

                if (tipo == 14 || tipo == 8 || tipo == 13)
                {
                    var reclutadores = db.AsignacionRequis.Where(x => !estatusId.Contains(x.Requisicion.EstatusId)).Select(u => new
                    {
                        clienteId = u.Requisicion.ClienteId,
                        reclutadorId = u.GrpUsrId,
                        tipoUsuario = db.Usuarios.Where(x => x.Id.Equals(u.GrpUsrId)).Select(n => n.TipoUsuarioId).FirstOrDefault(),
                        nombre = db.Usuarios.Where(x => x.Id.Equals(u.GrpUsrId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                        foto = @"https://apierp.damsa.com.mx/img/" + db.Usuarios.Where(x => x.Id.Equals(u.GrpUsrId)).Select(n => n.Clave).FirstOrDefault() + ".jpg",
                    }).Union(db.Requisiciones.Where(x => !estatusId.Contains(x.EstatusId)).Select(u => new
                    {
                        clienteId = u.ClienteId,
                        reclutadorId = u.AprobadorId,
                        tipoUsuario = db.Usuarios.Where(x => x.Id.Equals(u.AprobadorId)).Select(n => n.TipoUsuarioId).FirstOrDefault(),
                        nombre = db.Usuarios.Where(x => x.Id.Equals(u.AprobadorId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                        foto = @"https://apierp.damsa.com.mx/img/" + db.Usuarios.Where(x => x.Id.Equals(u.AprobadorId)).Select(n => n.Clave).FirstOrDefault() + ".jpg",
                    })).Union(db.Requisiciones.Where(x => !estatusId.Contains(x.EstatusId)).Select(u => new
                    {
                        clienteId = u.ClienteId,
                        reclutadorId = u.PropietarioId,
                        tipoUsuario = db.Usuarios.Where(x => x.Id.Equals(u.PropietarioId)).Select(n => n.TipoUsuarioId).FirstOrDefault(),
                        nombre = db.Usuarios.Where(x => x.Id.Equals(u.PropietarioId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                        foto = @"https://apierp.damsa.com.mx/img/" + db.Usuarios.Where(x => x.Id.Equals(u.AprobadorId)).Select(n => n.Clave).FirstOrDefault() + ".jpg",
                    })).ToList().Distinct();

                    var requisTodas = db.Requisiciones.Where(x => !estatusId.Contains(x.EstatusId) && !x.Confidencial).Select(r => new
                    {
                        clienteId = r.ClienteId,
                        cliente = r.Cliente.Nombrecomercial,
                        requisicionId = r.Id,
                        folio = r.Folio,
                        cubiertas = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(r.Id) && x.EstatusId.Equals(24)).Count(),
                        posiciones = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                                
                    }).ToList();
                    reclutadores.Distinct();
                    var clientes = requisTodas.GroupBy(g => g.clienteId).Select(d => new {
                        clienteId = d.Key,
                        cliente = d.Where(x => x.clienteId.Equals(d.Key)).Select(c => c.cliente).FirstOrDefault(),
                        reclutadores = reclutadores.Where(x => x.clienteId.Equals(d.Key)).Distinct(),
                        requisiciones = d.Select(x => new
                        {
                            requisicionId = x.requisicionId,
                            folio = x.folio,
                            cubiertas = x.cubiertas,
                            posiciones = x.posiciones
                        }),
                        totalPos = d.Select(x => new
                        {
                            requisicionId = x.requisicionId,
                            pos = x.posiciones
                        }).OrderBy(o => o.requisicionId).Select(xx => new { xx.requisicionId, xx.pos }).Distinct().Sum(s => s.pos),
                        totalCub = d.Select(x => new
                        {
                            requisicionId = x.requisicionId,
                            cub = x.cubiertas
                        }).OrderBy(o => o.requisicionId).Select(xx => new { xx.requisicionId, xx.cub }).Distinct().Sum(s => s.cub),
                    }).OrderByDescending(o => o.totalPos).ToList();

                    return Ok(clientes);
                }
                else if(tipo == 12)
                {
                    var UnidadNegocio = db.Usuarios
                  .Where(u => u.Id.Equals(usuarioId)).Select(u => new { u.Sucursal.UnidadNegocioId, u.Sucursal.UnidadNegocio.Descripcion }).FirstOrDefault();

                    switch (UnidadNegocio.UnidadNegocioId)
                    {
                        case 3: //mty
                            int?[] mty = { 6, 7, 10, 19, 28, 24 };
                            unidadNegocio = mty;
                            break;
                        case 1:
                            int?[] gdl = { 1, 3, 8, 11, 14, 16, 18, 2, 25, 26, 32 };
                            unidadNegocio = gdl;
                            break;
                        case 2:
                            int?[] mx = { 4, 5, 9, 12, 13, 15, 17, 20, 21, 22, 23, 27, 29, 30, 31 };
                            unidadNegocio = mx;
                            break;
                        default:
                            break;
                    }

                    var reclutadores = db.AsignacionRequis.Where(x => !estatusId.Contains(x.Requisicion.EstatusId) && unidadNegocio.Contains(x.Requisicion.Direccion.EstadoId)).Select(u => new
                    {
                        clienteId = u.Requisicion.ClienteId,
                        reclutadorId = u.GrpUsrId,
                        tipoUsuario = db.Usuarios.Where(x => x.Id.Equals(u.GrpUsrId)).Select(n => n.TipoUsuarioId).FirstOrDefault(),
                        nombre = db.Usuarios.Where(x => x.Id.Equals(u.GrpUsrId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                        foto = @"https://apierp.damsa.com.mx/img/" + db.Usuarios.Where(x => x.Id.Equals(u.GrpUsrId)).Select(n => n.Clave).FirstOrDefault() + ".jpg",
                    }).Union(db.Requisiciones.Where(x => !estatusId.Contains(x.EstatusId) && unidadNegocio.Contains(x.Direccion.EstadoId)).Select(u => new
                    {
                        clienteId = u.ClienteId,
                        reclutadorId = u.AprobadorId,
                        tipoUsuario = db.Usuarios.Where(x => x.Id.Equals(u.AprobadorId)).Select(n => n.TipoUsuarioId).FirstOrDefault(),
                        nombre = db.Usuarios.Where(x => x.Id.Equals(u.AprobadorId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                        foto = @"https://apierp.damsa.com.mx/img/" + db.Usuarios.Where(x => x.Id.Equals(u.AprobadorId)).Select(n => n.Clave).FirstOrDefault() + ".jpg",
                    })).Union(db.Requisiciones.Where(x => !estatusId.Contains(x.EstatusId)).Select(u => new
                    {
                        clienteId = u.ClienteId,
                        reclutadorId = u.PropietarioId,
                        tipoUsuario = db.Usuarios.Where(x => x.Id.Equals(u.PropietarioId)).Select(n => n.TipoUsuarioId).FirstOrDefault(),
                        nombre = db.Usuarios.Where(x => x.Id.Equals(u.PropietarioId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                        foto = @"https://apierp.damsa.com.mx/img/" + db.Usuarios.Where(x => x.Id.Equals(u.PropietarioId)).Select(n => n.Clave).FirstOrDefault() + ".jpg",
                    })).ToList().Distinct();

                    reclutadores.Distinct();

                    var requisTodas = db.Requisiciones.Where(x => !estatusId.Contains(x.EstatusId) && unidadNegocio.Contains(x.Direccion.EstadoId)).Select(r => new
                    {
                        clienteId = r.ClienteId,
                        cliente = r.Cliente.Nombrecomercial,
                        requisicionId = r.Id,
                        folio = r.Folio,
                        cubiertas = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(r.Id) && x.EstatusId.Equals(24)).Count(),
                        posiciones = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                    }).ToList();


                    var clientes = requisTodas.GroupBy(g => g.clienteId).Select(d => new {
                        clienteId = d.Key,
                        cliente = d.Where(x => x.clienteId.Equals(d.Key)).Select(c => c.cliente).FirstOrDefault(),
                        reclutadores = reclutadores.Where(x => x.clienteId.Equals(d.Key)).ToList().Distinct(),
                        requisiciones = d.Select(x => new
                        {
                            requisicionId = x.requisicionId,
                            folio = x.folio,
                            cubiertas = x.cubiertas,
                            posiciones = x.posiciones
                        }),
                        totalPos = d.Select(x => new
                        {
                            requisicionId = x.requisicionId,
                            pos = x.posiciones
                        }).OrderBy(o => o.requisicionId).Select(xx => new { xx.requisicionId, xx.pos }).Distinct().Sum(s => s.pos),
                        totalCub = d.Select(x => new
                        {
                            requisicionId = x.requisicionId,
                            cub = x.cubiertas
                        }).OrderBy(o => o.requisicionId).Select(xx => new { xx.requisicionId, xx.cub }).Distinct().Sum(s => s.cub),
                    }).OrderByDescending(o => o.totalPos).ToList();

                    return Ok(clientes);
                }
                else
                {
                    var gerentes = db.Subordinados.Where(x => !x.UsuarioId.Equals(usuarioId) && x.LiderId.Equals(usuarioId)).Select(u => u.UsuarioId).ToList();

                    uids = obj.RecursividadSub(gerentes, uids);
                    uids.Add(usuarioId);

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

                    var AllRequis = requis.Union(asignadas);
                 
                    var reclutadores = db.AsignacionRequis.Where(x => AllRequis.Distinct().Contains(x.RequisicionId) && uids.Distinct().Contains(x.GrpUsrId)).Select(u => new
                    {
                        clienteId = u.Requisicion.ClienteId,
                        reclutadorId = u.GrpUsrId,
                        tipoUsuario = db.Usuarios.Where(x => x.Id.Equals(u.GrpUsrId)).Select(n => n.TipoUsuarioId).FirstOrDefault(),
                        nombre = db.Usuarios.Where(x => x.Id.Equals(u.GrpUsrId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                        foto = @"https://apierp.damsa.com.mx/img/" + db.Usuarios.Where(x => x.Id.Equals(u.GrpUsrId)).Select(n => n.Clave).FirstOrDefault() + ".jpg",
                    }).Union(
                              db.Requisiciones.Where(x => AllRequis.Distinct().Contains(x.Id) && (uids.Distinct().Contains(x.AprobadorId))).Select(u => new
                              {
                                  clienteId = u.ClienteId,
                                  reclutadorId = u.AprobadorId,
                                  tipoUsuario = db.Usuarios.Where(x => x.Id.Equals(u.AprobadorId)).Select(n => n.TipoUsuarioId).FirstOrDefault(),
                                  nombre = db.Usuarios.Where(x => x.Id.Equals(u.AprobadorId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                                  foto = @"https://apierp.damsa.com.mx/img/" + db.Usuarios.Where(x => x.Id.Equals(u.AprobadorId)).Select(n => n.Clave).FirstOrDefault() + ".jpg",
                              })).Distinct().Union(db.Requisiciones.Where(x => AllRequis.Distinct().Contains(x.Id) && (uids.Distinct().Contains(x.PropietarioId))).Select(u => new
                              {
                                  clienteId = u.ClienteId,
                                  reclutadorId = u.PropietarioId,
                                  tipoUsuario = db.Usuarios.Where(x => x.Id.Equals(u.PropietarioId)).Select(n => n.TipoUsuarioId).FirstOrDefault(),
                                  nombre = db.Usuarios.Where(x => x.Id.Equals(u.PropietarioId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                                  foto = @"https://apierp.damsa.com.mx/img/" + db.Usuarios.Where(x => x.Id.Equals(u.PropietarioId)).Select(n => n.Clave).FirstOrDefault() + ".jpg",
                              })).ToList().Distinct();

                    reclutadores.ToList().Distinct();

                    var requisTodas = db.Requisiciones.Where(x => AllRequis.Distinct().Contains(x.Id)).Select(r => new
                    {
                        clienteId = r.ClienteId,
                        cliente = r.Cliente.Nombrecomercial,
                        requisicionId = r.Id,
                        folio = r.Folio,
                        cubiertas = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(r.Id) && x.EstatusId.Equals(24)).Count(),
                        posiciones = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                      
                    }).ToList();


                    var clientes = requisTodas.GroupBy(g => g.clienteId).Select(d => new {
                        clienteId = d.Key,
                        cliente = d.Where(x => x.clienteId.Equals(d.Key)).Select(c => c.cliente).FirstOrDefault(),
                        reclutadores = reclutadores.Where(x => x.clienteId.Equals(d.Key)).ToList().Distinct(),
                        requisiciones = d.Select(x => new
                        {
                            requisicionId = x.requisicionId,
                            folio = x.folio,
                            cubiertas = x.cubiertas,
                            posiciones = x.posiciones
                        }),
                        totalPos = d.Select(x => new
                        {
                            requisicionId = x.requisicionId,
                            pos = x.posiciones
                        }).OrderBy(o => o.requisicionId).Select(xx => new { xx.requisicionId, xx.pos }).Distinct().Sum(s => s.pos),
                        totalCub = d.Select(x => new
                        {
                            requisicionId = x.requisicionId,
                            cub = x.cubiertas
                        }).OrderBy(o => o.requisicionId).Select(xx => new { xx.requisicionId, xx.cub }).Distinct().Sum(s => s.cub),
                    }).OrderByDescending(o => o.totalPos).ToList();

                    return Ok(clientes);
                }

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet]
        [Route("getInformeClientes")]
        // Seccion que olicita el trakin vacante. 
        public IHttpActionResult GetInformeClientes(Guid cc)
        {
            try
            {

                var informe = db.Requisiciones.OrderByDescending(f => f.fch_Cumplimiento).Where(e => e.ClienteId.Equals(cc))
                    .Where(e => e.Activo.Equals(true) && e.EstatusId != 9 && !e.Confidencial).Select(h => new
                    {
                        Id = h.Id,
                        Folio = h.Folio,
                        vBtra = h.VBtra,
                        Estatus = h.Estatus.Descripcion,
                        EstatusId = h.EstatusId,
                        cliente = h.Cliente.Nombrecomercial,
                        Vacantes = h.horariosRequi.Count() > 0 ? h.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                        Fch_limite = h.fch_Cumplimiento,
                        Postulados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId.Equals(10)).Select(c => c.CandidatoId).Distinct().Count(),
                        Abandono = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 26).Count(),
                        Descartados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 27).Count(),
                        EnProceso = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28).Count(),
                        entrevista = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 18).Count(),
                        //EnProcesoFR = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 21).Count(),
                        //EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 23).Count(),
                        contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count(),
                        Enviados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId.Equals(21)).Count(),
                        //EntCliente = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 22).Count(),
                        rechazados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 40).Count(),
                        porcentaje = (db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count()) > 0 ? (db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count()) * 100 / h.horariosRequi.Sum(s => s.numeroVacantes) : 0,
                    }).ToList();

                return Ok(informe);

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        [HttpGet]
        [Route("getRportTableClientes")]
        public IHttpActionResult GetRportTableClientes(Guid usuarioId, int orden)
        {
            GetSub obj = new GetSub();
            int?[] unidadNegocio = { 0 };
            //1 posiciones activas. 2 cubiertas. 3 faltantes. 4 cumplimiento
            List<Guid> uids = new List<Guid>();
            int[] estatusId = { 8, 9, 34, 35, 36, 37, 47, 48 };
            try
            {
                var tipo = db.Usuarios.Where(x => x.Id.Equals(usuarioId)).Select(t => t.TipoUsuarioId).FirstOrDefault();

                if (tipo == 14 || tipo == 8 || tipo == 13)
                {
                    var clientes = db.Requisiciones.Select(x => x.ClienteId).Distinct();

                    var total = db.Requisiciones.Where(x => clientes.Contains(x.ClienteId) && !estatusId.Contains(x.EstatusId) && !x.Confidencial).Select(res => new
                    {
                        requisicionId = res.Id,
                        estatusId = res.EstatusId,
                        folio = res.Folio,
                        cliente = res.Cliente.Nombrecomercial,
                        vBtra = res.VBtra,
                        fch_Cumplimiento = res.fch_Cumplimiento,
                        vacantes = res.horariosRequi.Count() > 0 ? res.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                        contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Id) && x.EstatusId.Equals(24)).Count(),
                        coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(res.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(res.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                        reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(res.Id) && !x.GrpUsrId.Equals(res.AprobadorId)).Select(a =>
                                       db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                       ),
                        faltantes = (res.horariosRequi.Count() > 0 ? res.horariosRequi.Sum(v => v.numeroVacantes) : 0) - db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Id) && x.EstatusId.Equals(24)).Count(),
                        cumplimiento = res.horariosRequi.Sum(h => h.numeroVacantes) > 0 ? db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Id) && x.EstatusId.Equals(24)).Count() * 100 / res.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                    }).ToList();


                    if (orden == 1)
                    {
                        total = total.OrderByDescending(o => o.vacantes).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                    }
                    else if (orden == 2)
                    {
                        total = total.OrderBy(o => o.contratados).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                    }
                    else if (orden == 3)
                    {
                        total = total.OrderByDescending(o => o.faltantes).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                    }
                    else
                    {
                        total = total.OrderBy(o => o.cumplimiento).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                    }
                    return Ok(total);
                }
                else if (tipo == 12)
                {
                    var UnidadNegocio = db.Usuarios
                  .Where(u => u.Id.Equals(usuarioId)).Select(u => new { u.Sucursal.UnidadNegocioId, u.Sucursal.UnidadNegocio.Descripcion }).FirstOrDefault();

                    switch (UnidadNegocio.UnidadNegocioId)
                    {
                        case 3: //mty
                            int?[] mty = { 6, 7, 10, 19, 28, 24 };
                            unidadNegocio = mty;
                            break;
                        case 1:
                            int?[] gdl = { 1, 3, 8, 11, 14, 16, 18, 2, 25, 26, 32 };
                            unidadNegocio = gdl;
                            break;
                        case 2:
                            int?[] mx = { 4, 5, 9, 12, 13, 15, 17, 20, 21, 22, 23, 27, 29, 30, 31 };
                            unidadNegocio = mx;
                            break;
                        default:
                            break;
                    }
                    var clientes = db.Requisiciones.Where(x => unidadNegocio.Contains(x.Direccion.EstadoId)).Select(c => c.ClienteId).Distinct();

                    var total = db.Requisiciones.Where(x => clientes.Contains(x.ClienteId) && !estatusId.Contains(x.EstatusId) && !x.Confidencial).Select(res => new
                    {
                        requisicionId = res.Id,
                        estatusId = res.EstatusId,
                        folio = res.Folio,
                        cliente = res.Cliente.Nombrecomercial,
                        vBtra = res.VBtra,
                        fch_Cumplimiento = res.fch_Cumplimiento,
                        vacantes = res.horariosRequi.Count() > 0 ? res.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                        contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Id) && x.EstatusId.Equals(24)).Count(),
                        coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(res.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(res.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                        reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(res.Id) && !x.GrpUsrId.Equals(res.AprobadorId)).Select(a =>
                                       db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                       ),
                        faltantes = (res.horariosRequi.Count() > 0 ? res.horariosRequi.Sum(v => v.numeroVacantes) : 0) - db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Id) && x.EstatusId.Equals(24)).Count(),
                        cumplimiento = res.horariosRequi.Sum( h=> h.numeroVacantes ) > 0 ? db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Id) && x.EstatusId.Equals(24)).Count() * 100 / res.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                    }).ToList();


                    if (orden == 1)
                    {
                        total = total.OrderByDescending(o => o.vacantes).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                    }
                    else if (orden == 2)
                    {
                        total = total.OrderBy(o => o.contratados).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                    }
                    else if (orden == 3)
                    {
                        total = total.OrderByDescending(o => o.faltantes).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                    }
                    else
                    {
                        total = total.OrderBy(o => o.cumplimiento).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                    }
                    return Ok(total);


                }
                else
                {
                    var gerentes = db.Subordinados.Where(x => !x.UsuarioId.Equals(usuarioId) && x.LiderId.Equals(usuarioId)).Select(u => u.UsuarioId).ToList();

                    uids = obj.RecursividadSub(gerentes, uids);
                    uids.Add(usuarioId);

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

                    var AllRequis = requis.Union(asignadas);

                    var clientes = db.Requisiciones.Where(x => AllRequis.Contains(x.Id)).Select(c => c.ClienteId).Distinct();

                    var total = db.Requisiciones.Where(x => clientes.Contains(x.ClienteId) && !estatusId.Contains(x.EstatusId) && !x.Confidencial).Select(res => new
                    {
                        requisicionId = res.Id,
                        estatusId = res.EstatusId,
                        folio = res.Folio,
                        cliente = res.Cliente.Nombrecomercial,
                        vBtra = res.VBtra,
                        fch_Cumplimiento = res.fch_Cumplimiento,
                        vacantes = res.horariosRequi.Count() > 0 ? res.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                        contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Id) && x.EstatusId.Equals(24)).Count(),
                        coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(res.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(res.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                        reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(res.Id) && !x.GrpUsrId.Equals(res.AprobadorId)).Select(a =>
                                       db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                       ),
                        faltantes = (res.horariosRequi.Count() > 0 ? res.horariosRequi.Sum(v => v.numeroVacantes) : 0) - db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Id) && x.EstatusId.Equals(24)).Count(),
                        cumplimiento = res.horariosRequi.Sum(h => h.numeroVacantes) > 0 ? db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(res.Id) && x.EstatusId.Equals(24)).Count() * 100 / res.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                    }).ToList();


                    if (orden == 1)
                    {
                        total = total.OrderByDescending(o => o.vacantes).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                    }
                    else if (orden == 2)
                    {
                        total = total.OrderBy(o => o.contratados).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                    }
                    else if (orden == 3)
                    {
                        total = total.OrderByDescending(o => o.faltantes).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                    }
                    else
                    {
                        total = total.OrderBy(o => o.cumplimiento).ThenBy(oo => oo.fch_Cumplimiento).ToList();
                    }
                    return Ok(total);
                }
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
    }
}