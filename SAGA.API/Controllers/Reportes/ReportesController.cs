
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAGA.API.Dtos.Reporte;
using SAGA.API.Dtos;
using SAGA.API.Utilerias;

namespace SAGA.API.Controllers.Reportes
{
    [RoutePrefix("api/reporte")]
    public class ReportesController : ApiController
    {
        private SAGADBContext db = new SAGADBContext();

        [HttpPost]
        [Route("Informe")]
        [Authorize]
        public IHttpActionResult Informe(ReportesDto source)
        {
            int[] estatusId = new int[] { 8, 9, 34, 35, 36, 37, 47, 48 };
            DateTime FechaF = DateTime.Now;
            DateTime FechaI = DateTime.Now;
            PrivilegiosController objP = new PrivilegiosController();
            var permisos = objP.GetPrivilegios(source.usuario);
            var paginationMetadata = new PaginationDto
            {
                pageSize = source.rowIndex[1],
                currentPage = source.rowIndex[0],
                totalPages = 0
            };
            bool confidencial = permisos.Where(x => x.Nombre.Equals("Confidencial") && x.TipoEstructuraId.Equals(8)).Select(c => c.Read).FirstOrDefault();
            try
            {
                if (source.fini != null)
                {
                    FechaI = Convert.ToDateTime(source.fini);
                }
                if (source.ffin != null)
                {
                    FechaF = Convert.ToDateTime(source.ffin);
                }

                FechaF = FechaF.AddDays(1);

                var datos = db.Requisiciones.Where(e => e.fch_Creacion >= FechaI && e.fch_Creacion <= FechaF).OrderByDescending(o => o.fch_Creacion).Select(e => new
                {
                    e.Id,
                    e.Confidencial,
                    e.fch_Modificacion,
                    e.ClienteId,
                    e.AprobadorId,
                    e.PropietarioId,
                    e.Direccion.EstadoId,
                    numero = e.horariosRequi.Sum(a => a.numeroVacantes),
                    e.EstatusId,
                    e.TipoReclutamientoId,
                    e.ClaseReclutamientoId
                }).ToList();
                // se tiene que omptimizar esta parte
                //if (source.clave != null)
                //{
                //    datos = datos.Where(e => e.VBtra.ToLower().Contains(source.clave.ToLower())).ToList();
                //}
                if (source.tipo == 2 || source.tipo == 6)
                {
                    datos.OrderByDescending(o => o.fch_Modificacion);
                }
                if (source.stus.Count() > 0 && source.stus != null)
                {
                    datos = datos.Where(e => source.stus.Contains(e.EstatusId)).ToList();
                }
                if (source.sol.Count() > 0)
                {
                    datos = datos.Where(e => source.sol.Contains(e.PropietarioId)).ToList();
                }
                if (source.coord.Count() > 0)
                {
                    datos = datos.Where(e => source.coord.Contains(e.ClaseReclutamientoId)).ToList();
                }
                if (source.trcl.Count() > 0)
                {
                    datos = datos.Where(e => source.trcl.Contains(e.TipoReclutamientoId)).ToList();
                }
                if (source.emp.Count() > 0)
                {
                    datos = datos.Where(e => source.emp.Contains(e.ClienteId)).ToList();
                }

                Guid id = source.usuario;
                int usertipo = db.Usuarios.Where(e => e.Id == id).Select(e => e.TipoUsuarioId).FirstOrDefault();
                if (usertipo == 11)
                {
                    var requiID = datos.Select(x => x.Id).ToList();
                    var asigna = db.AsignacionRequis.Where(e => requiID.Contains(e.RequisicionId)).ToList();
                    var requien = asigna.Where(e => e.GrpUsrId == id).Select(x => x.RequisicionId).ToList();
                    datos = datos.Where(e => e.AprobadorId == id || requien.Contains(e.Id)).ToList();
                }
                else
                {
                    if (source.usercoor.Count() > 0)
                    {

                        var asigna = db.AsignacionRequis.Where(e => source.usercoor.Contains(e.GrpUsrId) && e.Tipo.Equals(1)).Select(e => e.RequisicionId).Distinct().ToList();
                        datos = datos.Where(e => asigna.Contains(e.Id)).ToList();
                    }
                }
                if (source.recl.Count() > 0)
                {
                    var asigna = db.AsignacionRequis.Where(e => source.recl.Contains(e.GrpUsrId) && e.Tipo.Equals(2)).Select(e => e.RequisicionId).ToList();
                    datos = datos.Where(e => asigna.Contains(e.Id)).ToList();
                }
                if (source.ofc.Count() > 0)
                {
                    var estados = db.UnidadNegocioEstados.Where(x => source.ofc.Contains(x.unidadnegocioId)).Select(suc => suc.estadoId).ToList().Distinct();
                    datos = datos.Where(e => estados.Contains(e.EstadoId)).ToList();
                }

                if (!confidencial)
                {
                    datos = datos.Where(x => !x.Confidencial).ToList();
                }

                var totalFolios = datos.Count();
                var posActivas = datos.Where(x => !estatusId.Contains(x.EstatusId)).Select(r => r.numero).ToList().Sum();
                List<Guid> requis = new List<Guid>();

                if(paginationMetadata.currentPage > 0)
                {
                    requis = datos.Select(r => r.Id).Skip(paginationMetadata.pageSize * (paginationMetadata.currentPage - 1))
                   .Take(paginationMetadata.pageSize).ToList();
                } else
                {
                    requis = datos.Select(r => r.Id).ToList();
                }
                
                var result = db.Requisiciones.Where(x => requis.Contains(x.Id)).Select(e => new
                {
                    e.Confidencial,
                    e.Id,
                    e.Folio,
                    VBtra = e.VBtra.ToUpper(),
                    cubierta = db.ProcesoCandidatos.Where(x => x.EstatusId == 24 && x.RequisicionId == e.Id).Select(a => a.CandidatoId).Distinct().ToList().Count(),
                    porcentaje = e.horariosRequi.Sum(s => s.numeroVacantes) > 0 ? (db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count()) * 100 / e.horariosRequi.Sum(s => s.numeroVacantes) : 0,
                    e.fch_Creacion,
                    e.fch_Modificacion,
                    e.fch_Limite,
                    empresa = e.Cliente.Nombrecomercial.ToUpper(),
                    e.ClienteId,
                    e.AprobadorId,
                    coordinador2 = e.AsignacionRequi.Where(x => x.Tipo.Equals(1)).Select(c => db.Usuarios.Where(x => x.Id.Equals(c.GrpUsrId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()).FirstOrDefault(),
                    nombreApellido = db.Usuarios.Where(x => x.Id == e.PropietarioId).Select(n => n.Nombre.ToUpper() + " " + n.ApellidoPaterno.ToUpper() + " " + n.ApellidoMaterno.ToUpper()).FirstOrDefault(),
                    propietario = db.Usuarios.Where(x => x.Id == e.PropietarioId).FirstOrDefault().Nombre.ToUpper(),
                    e.PropietarioId,
                    Usuario = db.Usuarios.Where(x => x.Id == e.PropietarioId).FirstOrDefault().Usuario,
                    Estado = e.Direccion.Estado.estado.ToUpper(),
                    e.Direccion.EstadoId,
                    numero = e.horariosRequi.Sum(a => a.numeroVacantes),
                    e.EstatusId,
                    e.TipoReclutamientoId,
                    tipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento.ToUpper(),
                    clasesReclutamiento = e.ClaseReclutamiento.clasesReclutamiento.ToUpper(),
                    e.ClaseReclutamientoId,
                    e.fch_Cumplimiento,
                    estatus = e.Estatus.Descripcion.ToUpper(),
                   // reclutadorTotal = db.AsignacionRequis.Where(a => a.RequisicionId == e.Id && a.GrpUsrId != e.AprobadorId).Count() == 0 ? "SIN ASIGNAR" : db.AsignacionRequis.Where(a => a.RequisicionId == e.Id && a.GrpUsrId != e.AprobadorId).Count().ToString(),
                    nombreReclutado = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && x.Tipo.Equals(2)).Select(a =>
                        db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                      ).ToList(),
                    candiTotal = db.ProcesoCandidatos.Where(a => a.RequisicionId.Equals(e.Id)).Count(),
                    cubiertos = db.CandidatosInfo.Where(x => db.InformeRequisiciones.Where(xx => xx.EstatusId == 24 && xx.RequisicionId == e.Id).Select(a => a.CandidatoId).Distinct().ToList().Contains(x.CandidatoId))
                                        .Select(ci => new {
                                            candidato = ci.Nombre + " " + ci.ApellidoPaterno + " " + ci.ApellidoMaterno,
                                            reclutador = db.Usuarios.Where(x => x.Id.Equals(ci.ReclutadorId)).Select(nr => nr.Clave + " " + nr.Nombre + " " + nr.ApellidoPaterno + " " + nr.ApellidoMaterno).FirstOrDefault()
                                        }).ToList(),
                    nombreCandidato = db.ProcesoCandidatos.Where(a => a.RequisicionId.Equals(e.Id)).Select(b => db.Candidatos.Where(xc => xc.Id.Equals(b.CandidatoId)).Select(c => c.Nombre + " " + c.ApellidoPaterno + " " + c.ApellidoMaterno)).ToList(),
                    coemtaTotal = db.ComentariosVacantes.Where(x => x.RequisicionId == e.Id).Count(),
                    listaComentario = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c =>
                          c.fch_Creacion + " " + c.Comentario.ToUpper()).ToList()

                }).ToList();


                return Ok(new
                {
                    result,
                    posActivas,
                    totalFolios,
                    paginationMetadata
                });
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("empresas")]
        public IHttpActionResult Empresas()
        {
            var fecha = DateTime.Now.AddDays(-15);
            var clientes = db.Requisiciones.Select(c => c.ClienteId).ToList().Distinct();
            var datos = db.Clientes.Where(e => clientes.Contains(e.Id) && e.Activo == true && e.esCliente == true).Select(e => new
            {
                e.RFC,
                e.Nombrecomercial,
                e.Id,
                e.RazonSocial,
                fechal = fecha
            }).Distinct().OrderBy(x=>x.RazonSocial).ToList();
            //if (bandera == "1")
            //{
            //    datos.Insert(0, new { RFC = "Todos", Nombrecomercial = "as", Id = new Guid("00000000-0000-0000-0000-000000000000"), RazonSocial = "Todas", fechal = fecha });
            //}
           
            return Ok(datos);
        }


        [HttpGet]
        [Route("usuario")]
        public IHttpActionResult Usuario(string cor)
        {
            
            int[] Tipo = new[] { 11 };

            //Cordinadores
            if (cor == "1")
            {
                var reclutamiento = new Guid("67B220CE-F9D2-E811-80EB-9E274155325E"); 
                Guid[] depa = new[] { reclutamiento };
                var lider = db.Usuarios.Where(e => e.TipoUsuarioId == 5 && depa.Contains(e.DepartamentoId)).Select(e => new
                {
                    Nombre = e.Nombre + " " + e.ApellidoPaterno + " " + e.ApellidoMaterno,
                    e.Id,
                    e.Usuario
                }).OrderBy(x => x.Nombre).ToList();

                var datos2 = db.Usuarios.Where(e => e.Activo == true && e.TipoUsuarioId == 4).Select(e => new
                {
                    Nombre = e.Nombre + " " + e.ApellidoPaterno + " " + e.ApellidoMaterno,
                    e.Id,
                    e.Usuario
                }).OrderBy(x => x.Nombre).ToList();

                foreach (var item in lider)
                {
                    datos2.Insert(0, new { Nombre = item.Nombre, Id = item.Id, Usuario = item.Usuario });
                }
                datos2 = datos2.OrderBy(e => e.Nombre).ToList();
               // datos2.Insert(0, new { Nombre = "Todos", Id = new Guid("00000000-0000-0000-0000-000000000000"), Usuario = "0" });
                return Ok(datos2);
            }
            //Solicitantes
            if (cor == "2")
            {
                var ventas = new Guid("924B7E74-8857-E811-80E1-9E274155325E");
                var adPersonal = new Guid("9F4B7E74-8857-E811-80E1-9E274155325E");
                Guid[] depa = new[] { ventas, adPersonal };
                var lider = db.Usuarios.Where(e => e.TipoUsuarioId == 5 && depa.Contains(e.DepartamentoId)).Select(e => new
                {
                    Nombre = e.Nombre + " " + e.ApellidoPaterno + " " + e.ApellidoMaterno,
                    e.Id,
                    e.Usuario
                }).OrderBy(x => x.Nombre).ToList();

                var datos2 = db.Usuarios.Where(e => e.Activo == true && e.TipoUsuarioId == 10).Select(e => new
                {
                    Nombre = e.Nombre + " " + e.ApellidoPaterno + " " + e.ApellidoMaterno, 
                    e.Id,
                    e.Usuario
                }).OrderBy(x => x.Nombre).ToList();

                foreach (var item in lider)
                {
                    datos2.Insert(0, new { Nombre = item.Nombre, Id = item.Id, Usuario = item.Usuario });
                }
                datos2 = datos2.OrderBy(e => e.Nombre).ToList();
                //datos2.Insert(0, new { Nombre = "Todos", Id = new Guid("00000000-0000-0000-0000-000000000000"), Usuario = "0" });
                return Ok(datos2);
            }


            var datos = db.Usuarios.Where(e => e.Activo == true && Tipo.Contains(e.TipoUsuarioId)).Select(e => new
            {
                Nombre = e.Nombre + " "+ e.ApellidoPaterno + " " + e.ApellidoMaterno,
                e.Id,
                e.Usuario
            }).OrderBy(x=>x.Nombre).ToList();
           // datos.Insert(0, new { Nombre = "Todos", Id = new Guid("00000000-0000-0000-0000-000000000000") , Usuario = "0"});
            return Ok(datos);
        }


        [HttpGet]
        [Route("estatus")]
        public IHttpActionResult Estatus(string bandera)
        {
            int[] ActivosList = new[] { 4, 6, 7, 29, 30, 31,32,33,38,39 };
            int[] CubiertosList = new[] { 34, 35, 36, 37, 47, 48 };
            int[] OtrosList = new[] { 8, 9, 43, 44, 45, 46 };

            if (bandera == "2")
            {
                
                var dato = new
                {
                    activos = db.Estatus.Where(x => ActivosList.Contains(x.Id)).Select(x => new { x.Descripcion, x.Id }).ToList(),
                    cubiertos = db.Estatus.Where(x => CubiertosList.Contains(x.Id)).Select(x => new { x.Descripcion, x.Id }).ToList(),
                    otros = db.Estatus.Where(x => OtrosList.Contains(x.Id)).Select(x => new { x.Descripcion, x.Id }).ToList(),
                };
                
                return Ok(dato);
            }

            //var datos = new
            //{
            //    activos = db.Estatus.Where(x => ActivosList.Contains(x.Id)).Select(x => new { x.Descripcion, x.Id }).ToList(),
            //    cubiertos = db.Estatus.Where(x => CubiertosList.Contains(x.Id)).Select(x => new { x.Descripcion, x.Id }).ToList(),
            //    otros = db.Estatus.Where(x => OtrosList.Contains(x.Id)).Select(x => new { x.Descripcion, x.Id }).ToList()
            //};

            var datos = db.Estatus.Where(e => e.Activo == true && e.TipoMovimiento == 2 && e.Id != 5).Select(e => new
            {
                e.Descripcion,
                e.Id,
            }).ToList();

            if (bandera == "1")
            {
                var datos2 = db.Estatus.Where(e => e.Activo == true && e.TipoMovimiento == 19 && e.Id != 5).Select(e => new
                {
                    e.Descripcion,
                    e.Id,
                }).ToList();
                datos2.Insert(0, new { Descripcion = "Todos", Id = 0 });
                return Ok(datos2);
            }
            datos.Insert(0, new { Descripcion = "Todos", Id = 0 });
            return Ok(datos);
        }

        [HttpGet]
        [Route("estatusDiasTrans")]
        public IHttpActionResult estatusDiasTrans()
        {
            try
            {
                var datos = db.Database.SqlQuery<PausadasDto>("dbo.sp_estatusDiasTrans").ToList();
                return Ok(datos);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
            
        }

        [HttpGet]
        [Route("oficinas")]
        public IHttpActionResult Oficinas()
        {
            int[] tipos = new[] { 1, 2, 3, 4, 5};
            var datos = db.OficinasReclutamiento.Where(e => tipos.Contains(e.TipoOficinaId)).Select(e=> new
            {
                e.Orden,
                e.Id,
                UnidadNegocioId = db.UnidadNegocioEstados.Where(x => x.estadoId.Equals(e.estadoId)).Select(un => un.unidadnegocioId).FirstOrDefault(),
                oficina = db.Entidad.Where(x=>x.Id == e.Id).FirstOrDefault().Nombre
            }).OrderBy(e => e.Orden).ToList();
            
            return Ok(datos);
        }
       
        [HttpGet]
        [Route("coordinacion")]
        public IHttpActionResult coordinacion(string fini, string ffin, string stus)
        {
            DateTime FechaF = DateTime.Now;
            DateTime FechaI = DateTime.Now;
            try
            {
                if (fini != null)
                {
                    FechaI = Convert.ToDateTime(fini);
                }
                if (ffin != null)
                {
                    FechaF = Convert.ToDateTime(ffin);
                }
            }
            catch (Exception)
            {

            }
            FechaF = FechaF.AddDays(1);
            var requi = db.Requisiciones.Where(e=>e.fch_Modificacion >= FechaI && e.fch_Modificacion < FechaF).ToList();
           

            var masivo = requi.Where(e=>e.ClaseReclutamientoId == 3).Select(e=> new {
                e.Id,
                e.EstatusId
            }).ToList();

            var operativo = requi.Where(e => e.ClaseReclutamientoId == 2).Select(e => new {
                e.Id,
                e.EstatusId
            }).ToList();

            var ezpeciali = requi.Where(e => e.ClaseReclutamientoId == 1).Select(e => new {
                e.Id,
                e.EstatusId
            }).ToList();

            try
            {
                var datos2 = db.Estatus.Where(e => e.Activo == true && e.TipoMovimiento == 2 && e.Id != 5).Select(e => new
                {
                    e.Id,
                    e.Descripcion
                }).ToList();

                var datos3 = new List<proactividad>();
                foreach (var item in datos2)
                {
                    var obj = new proactividad();
                    obj.vacantes = item.Id;
                    obj.nombre = item.Descripcion;
                    obj.numeropos = masivo.Where(x => x.EstatusId == item.Id).ToList().Count();
                    obj.porcentaje = operativo.Where(x => x.EstatusId == item.Id).ToList().Count();
                    obj.puntaje = ezpeciali.Where(x => x.EstatusId == item.Id).ToList().Count();
                    datos3.Add(obj);
                }

                var datos = datos3.Select(e => new {
                    Id = e.vacantes,
                    Descripcion = e.nombre,
                    masivo = e.numeropos,
                    operativo = e.porcentaje,
                    ezpecial = e.puntaje,
                }).ToList();

                if (stus != "0" && stus != null)
                {
                    var obj = stus.Split(',');
                    List<int> listaAreglo = new List<int>();
                    for (int i = 0; i < obj.Count() ; i++)
                    {
                        listaAreglo.Add(Convert.ToInt32(obj[i]));
                    }
                    var obb = listaAreglo.Where(e => e.Equals(0)).ToList();
                    if (obb.Count == 0)
                    {
                        datos = datos.Where(e => listaAreglo.Contains(e.Id)).ToList();
                    }
                }
                var requien = datos.Select(e => e.Id).ToList();
                var datos4 = datos.Select(e => new {
                    Id = e.Id,
                    Descripcion = e.Descripcion,
                    masivo = e.masivo,
                    operativo = e.operativo,
                    ezpecial = e.ezpecial,
                    totalmas = masivo.Where(x => requien.Contains(x.EstatusId)).ToList().Count(),
                    totalope = operativo.Where(x => requien.Contains(x.EstatusId)).ToList().Count(),
                    totalesp = ezpeciali.Where(x => requien.Contains(x.EstatusId)).ToList().Count(),
                    total = masivo.Where(x => requien.Contains(x.EstatusId)).ToList().Count() + operativo.Where(x => requien.Contains(x.EstatusId)).ToList().Count()
                    + ezpeciali.Where(x => requien.Contains(x.EstatusId)).ToList().Count()
                }).ToList();
                return Ok(datos4);
            }
            catch (Exception ex)
            {
               var mensaje =  ex.Message;
               
            }

           
            //if (cor != "0" && cor != null)
            //{
            //    var obj = cor.Split(',');
            //    List<int> listaAreglo = new List<int>();
            //    for (int i = 0; i < obj.Count() - 1; i++)
            //    {
            //        listaAreglo.Add(Convert.ToInt32(obj[i]));
            //    }
            //    var obb = listaAreglo.Where(e => e.Equals("0")).ToList();
            //    if (obb.Count == 0)
            //    {
            //        datos = datos.Where(e => listaAreglo.Contains(e.ClaseReclutamientoId)).ToList();
            //    }
            //}

            return Ok("El servidor no responde");
        }


        [HttpGet]
        [Route("actividad")]
        public IHttpActionResult actividad(string fini,string ffin,string recl, string cor)
        {
            DateTime FechaF = DateTime.Now;
            DateTime FechaI = DateTime.Now;
            try
            {
                if (fini != null)
                {
                    FechaI = Convert.ToDateTime(fini);
                }
                if (ffin != null)
                {
                    FechaF = Convert.ToDateTime(ffin);
                }
            }
            catch (Exception)
            {

            }
            FechaF = FechaF.AddDays(1);
            int[] Status = new[] { 34, 35, 36, 37,47,48 };
        //    var listaRequi = db.InformeRequisiciones.Where(e => e.fch_Modificacion >= FechaI && e.fch_Modificacion <= FechaF ).Select(e => e.RequisicionId).Distinct().ToList();
         //   listaRequi = db.Requisiciones.Where(e => listaRequi.Contains(e.Id) && Status.Contains(e.EstatusId)).Select(a=>a.Id).ToList();
          var listaRequi = db.EstatusRequisiciones.Where(e => e.fch_Modificacion >= FechaI && e.fch_Modificacion < FechaF && Status.Contains(e.EstatusId)).Select(e=>e.RequisicionId).ToList().Distinct();
        
          var candidatos = db.AsignacionRequis.Where(e => listaRequi.Contains(e.RequisicionId)).ToList();
            var recluta = candidatos.Select(e => new { e.GrpUsrId }).Distinct().ToList();
            var vacantes = db.Requisiciones.Where(e => listaRequi.Contains(e.Id) && Status.Contains(e.EstatusId)).ToList();
            var aprovador = vacantes.Select(e => e.AprobadorId).ToList();
            recluta = recluta.Where(e => !aprovador.Contains(e.GrpUsrId)).ToList();
            var recluta2 = recluta.Select(e => e.GrpUsrId).ToList();
            var datos = db.Usuarios.Where(e => recluta2.Contains(e.Id)).ToList();
            //var datos = db.Usuarios.Where(e => recluta.Contains(e.Id)).Select(e => new {
            //    e.Id,
            //    vacantes = db.HorariosRequis.Where(x => candidatos.Where(a => a.ReclutadorId == e.Id).Select(a => a.RequisicionId).Contains(x.RequisicionId)).Sum(x => x.numeroVacantes),
            //    cubiertas = db.HorariosRequis.Where(x => candidatos.Where(a => a.ReclutadorId == e.Id && a.EstatusId == 34).Select(a => a.RequisicionId).Contains(x.RequisicionId)).Sum(x => x.numeroVacantes),
            //    puntaje = PuntajeCalculo(e.Id, candidatos)
            //}).ToList();
            try
            {

                List<proactividad> ProActi = new List<proactividad>();
                if (cor != "0" && cor != null)
                {
                    var obj = cor.Split(',');
                    List<int> listaAreglo = new List<int>();
                    for (int i = 0; i < obj.Count() - 1; i++)
                    {
                        listaAreglo.Add(Convert.ToInt32(obj[i]));
                    }
                    var obb = listaAreglo.Where(e => e.Equals(0)).ToList();
                    if (obb.Count == 0)
                    {
                        vacantes = vacantes.Where(e => listaAreglo.Contains(e.ClaseReclutamientoId)).ToList();
                       
                    }
                }
                foreach (var item in datos)
                {
                    var obj = new proactividad();
                    var listaRequien = vacantes.Select(e => e.Id).ToList();
                    var ListaCubierta = db.ProcesoCandidatos.Where(e => e.ReclutadorId == item.Id && e.EstatusId == 24 && listaRequien.Contains(e.RequisicionId)).ToList();
                    var Cubiertaid = ListaCubierta.Select(e => e.RequisicionId).Distinct().ToList();
                   

                    obj.vacantes = db.AsignacionRequis.Where(e => listaRequien.Contains(e.RequisicionId) && e.GrpUsrId == item.Id).ToList().Count();
                    try
                    {
                        obj.numeropos = db.HorariosRequis.Where(x => Cubiertaid.Contains(x.RequisicionId)).Sum(x => x.numeroVacantes);
                    }
                    catch (Exception)
                    {
                        obj.numeropos = 0;
                    }

                    try
                    {
                        obj.cubiertas = ListaCubierta.Count();
                        obj.puntaje = new Calculo().PuntajeCalculo(item.Id, ListaCubierta);
                       
                    }
                    catch (Exception)
                    {
                        obj.cubiertas = 0;
                    }
                    obj.nombre = item.Nombre + " " + item.ApellidoPaterno + " " + item.ApellidoMaterno;
                    obj.id = item.Id;
                    
                    ProActi.Add(obj);
                }

                if (recl != "0" && recl != "00000000-0000-0000-0000-000000000000," && recl != null)
                {
                    var obj = recl.Split(',');
                    List<Guid> listaAreglo = new List<Guid>();
                    for (int i = 0; i < obj.Count() - 1; i++)
                    {
                        listaAreglo.Add(new Guid(obj[i]));
                    }
                    var obb = listaAreglo.Where(e => e.Equals(new Guid("00000000-0000-0000-0000-000000000000"))).ToList();
                    if (obb.Count == 0)
                    {
                        //  var asigna = db.AsignacionRequis.Where(e => listaAreglo.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                        ProActi = ProActi.Where(e => listaAreglo.Contains(e.id)).ToList();
                    }
                }

                return Ok(ProActi);
            }
            catch (Exception eror)
            {
                var algo = eror;
            }
            //int entrevTotal = db.HorariosRequis.Where(e => asigna.Contains(e.RequisicionId)).Sum(e => e.numeroVacantes);
            //decimal entrevi = db.InformeRequisiciones.Where(e => asigna.Contains(e.RequisicionId) && e.EstatusId == 18).Select(a => a.CandidatoId).Distinct().ToList().Count;
            // datos.Insert(0, new { Descripcion = "Todos", Id = 0 });
            return Ok("Por el momento el servidor no responde");
        }


        [HttpGet]
        [Route("detallerecluta")]
        public IHttpActionResult detallerecluta(string fini, string ffin, string recl, string cor)
        {
            DateTime FechaF = DateTime.Now;
            DateTime FechaI = DateTime.Now;
            try
            {
                if (fini != null)
                {
                    FechaI = Convert.ToDateTime(fini);
                }
                if (ffin != null)
                {
                    FechaF = Convert.ToDateTime(ffin);
                }
            }
            catch (Exception)
            {

            }
            FechaF = FechaF.AddDays(1);
            int[] Status = new[] { 33,34, 35, 36, 37,47,48 };

            var info = db.CandidatosInfo.Where(e => e.fch_Modificacion >= FechaI && e.fch_Modificacion < FechaF).Select(e=>e.ReclutadorId).Distinct().ToList();
            var candidatos = db.AsignacionRequis.Where(e => info.Contains(e.GrpUsrId)).ToList();
            var CandiRequi = candidatos.Select(e => e.RequisicionId).ToList();
            var listaRequi = db.EstatusRequisiciones.Where(e => CandiRequi.Contains(e.RequisicionId) && Status.Contains(e.EstatusId)).Select(e => e.RequisicionId).ToList().Distinct();
            
            var recluta = candidatos.Select(e => new { e.GrpUsrId }).Distinct().ToList();
            var vacantes = db.Requisiciones.Where(e => listaRequi.Contains(e.Id) && Status.Contains(e.EstatusId)).ToList();
            var aprovador = vacantes.Select(e => e.AprobadorId).ToList();
            recluta = recluta.Where(e => !aprovador.Contains(e.GrpUsrId)).ToList();
            var recluta2 = recluta.Select(e => e.GrpUsrId).ToList();
            var datos = db.Usuarios.Where(e => recluta2.Contains(e.Id)).ToList();
            try
            {

                List<proactividad> ProActi = new List<proactividad>();
                if (cor != "0" && cor != null)
                {
                    var obj = cor.Split(',');
                    List<int> listaAreglo = new List<int>();
                    for (int i = 0; i < obj.Count() - 1; i++)
                    {
                        listaAreglo.Add(Convert.ToInt32(obj[i]));
                    }
                    var obb = listaAreglo.Where(e => e.Equals(0)).ToList();
                    if (obb.Count == 0)
                    {
                        vacantes = vacantes.Where(e => listaAreglo.Contains(e.ClaseReclutamientoId)).ToList();
                    }
                }
                
                foreach (var item in datos)
                {
                    var obj = new proactividad();
                    var listaRequien = vacantes.Select(e => e.Id).ToList();
                    var ListaCubierta = db.ProcesoCandidatos.Where(e => e.ReclutadorId == item.Id && e.EstatusId == 24 && listaRequien.Contains(e.RequisicionId)).ToList();
                    var listaPosicion = candidatos.Where(e => e.GrpUsrId == item.Id).Select(e => e.RequisicionId).ToList();

                    //var obj = new proactividad();
                    //var listaRequien = vacantes.Select(e => e.Id).ToList();
                    //var ListaCubierta = db.ProcesoCandidatos.Where(e => e.ReclutadorId == item.Id && e.Fch_Modificacion >= FechaI && e.Fch_Modificacion <= FechaF && e.EstatusId == 24).ToList();
                    //var listaPosicion = db.AsignacionRequis.Where(e => listaRequien.Contains(e.RequisicionId) && e.GrpUsrId == item.Id).Select(e => e.RequisicionId).ToList();

                    obj.vacantes = db.AsignacionRequis.Where(e => listaRequien.Contains(e.RequisicionId) && e.GrpUsrId == item.Id).ToList().Count();
                    try
                    {
                        obj.numeropos = db.HorariosRequis.Where(x => listaPosicion.Contains(x.RequisicionId)).Sum(x => x.numeroVacantes);
                    }
                    catch (Exception)
                    {
                        obj.numeropos = 0;
                    }

                    try
                    {
                        obj.cubiertas = ListaCubierta.Count();
                        decimal operacion = 0;
                        if (obj.cubiertas > 0)
                        {
                           operacion = (Convert.ToDecimal(obj.cubiertas) / obj.numeropos) * (100m);
                        }
                        
                        obj.porcentaje = Convert.ToInt32(operacion);
                    }
                    catch (Exception)
                    {
                        obj.cubiertas = 0;
                    }
                    obj.nombre = item.Nombre + " " + item.ApellidoPaterno + " " + item.ApellidoMaterno;
                    obj.id = item.Id;
                    ProActi.Add(obj);
                }

                if (recl != "0" && recl != "00000000-0000-0000-0000-000000000000," && recl != null)
                {
                    var obj = recl.Split(',');
                    List<Guid> listaAreglo = new List<Guid>();
                    for (int i = 0; i < obj.Count() - 1; i++)
                    {
                        listaAreglo.Add(new Guid(obj[i]));
                    }
                    var obb = listaAreglo.Where(e => e.Equals(new Guid("00000000-0000-0000-0000-000000000000"))).ToList();
                    if (obb.Count == 0)
                    {
                        ProActi = ProActi.Where(e => listaAreglo.Contains(e.id)).ToList();
                    }
                }
                return Ok(ProActi);
            }
            catch (Exception eror)
            {
                var algo = eror;
            }
            return Ok("Por el momento el servidor no responde");
        }

        [HttpGet]
        [Route("detallecordina")]
        public IHttpActionResult detallecordina(string fini, string ffin, string aprob, string cor)
        {
            DateTime FechaF = DateTime.Now;
            DateTime FechaI = DateTime.Now;
            try
            {
                if (fini != null)
                {
                    FechaI = Convert.ToDateTime(fini);
                }
                if (ffin != null)
                {
                    FechaF = Convert.ToDateTime(ffin);
                }
            }
            catch (Exception)
            {

            }
            FechaF = FechaF.AddDays(1);
            int[] Status = new[] { 33,34, 35, 36, 37, 47, 48 };
            var Requisiciones = db.Requisiciones.Where(e => e.fch_Modificacion >= FechaI && e.fch_Modificacion < FechaF).ToList();

          //  var Requisiciones = db.AsignacionRequis.Where(e => e.fch_Modificacion >= FechaI && e.fch_Modificacion <= FechaF).ToList();
            var listaRequi = Requisiciones.Select(e => e.Id).Distinct().ToList();
            var vacantes = db.Requisiciones.Where(e => listaRequi.Contains(e.Id)).ToList();
            var recluta = vacantes.Where(e=>e.AprobadorId != new Guid("00000000-0000-0000-0000-000000000000")).Select(e=>e.AprobadorId).ToList();
            var datos = db.Usuarios.Where(e => recluta.Contains(e.Id)).ToList();
            try
            {

                List<proactividad> ProActi = new List<proactividad>();
                if (cor != "0" && cor != null)
                {
                    var obj = cor.Split(',');
                    List<int> listaAreglo = new List<int>();
                    for (int i = 0; i < obj.Count() - 1; i++)
                    {
                        listaAreglo.Add(Convert.ToInt32(obj[i]));
                    }
                    var obb = listaAreglo.Where(e => e.Equals(0)).ToList();
                    if (obb.Count == 0)
                    {
                        vacantes = vacantes.Where(e => listaAreglo.Contains(e.ClaseReclutamientoId)).ToList();
                    }
                }

                foreach (var item in datos)
                {

                    var ListaUsuario = new List<Guid>();
                    ListaUsuario.Add(item.Id);
                    var arbol = db.Subordinados.Where(e => e.LiderId == item.Id).ToList();
                    if (arbol.Count > 0)
                    {
                        ListaUsuario.AddRange(arbol.Select(e => e.UsuarioId));
                        foreach (var item2 in arbol)
                        {
                            var hijos = db.Subordinados.Where(e => e.LiderId == item2.UsuarioId).ToList();
                            if (hijos.Count > 0)
                            {
                                ListaUsuario.AddRange(hijos.Select(e => e.UsuarioId));
                            }
                        }
                    }
                    var obj = new proactividad();
                    var listaRequien = vacantes.Where(e => e.AprobadorId == item.Id).Select(e => e.Id).ToList();
                    var ListaCubierta = db.ProcesoCandidatos.Where(e => ListaUsuario.Contains(e.ReclutadorId) && listaRequien.Contains(e.RequisicionId) && e.EstatusId == 24).ToList();
                   // var listaPosicion = db.HorariosRequis.Where(e => listaRequien.Contains(e.RequisicionId)).Sum(e => e.numeroVacantes); //db.AsignacionRequis.Where(e => listaRequien.Contains(e.RequisicionId) && ListaUsuario.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();

                    obj.vacantes = db.AsignacionRequis.Where(e => listaRequien.Contains(e.RequisicionId) && e.GrpUsrId == item.Id).ToList().Count();
                    try
                    {
                        obj.numeropos = db.HorariosRequis.Where(x => listaRequien.Contains(x.RequisicionId)).Sum(x => x.numeroVacantes);
                    }
                    catch (Exception)
                    {
                        obj.numeropos = 0;
                    }

                    try
                    {
                        obj.cubiertas = ListaCubierta.Count();
                        decimal operacion = 0;
                        if (obj.cubiertas > 0)
                        {
                            operacion = (Convert.ToDecimal(obj.cubiertas) / obj.numeropos) * (100m);
                        }

                        obj.porcentaje = Convert.ToInt32(operacion);
                    }
                    catch (Exception)
                    {
                        obj.cubiertas = 0;
                    }
                    obj.nombre = item.Nombre + " " + item.ApellidoPaterno + " " + item.ApellidoMaterno;
                    obj.id = item.Id;
                    ProActi.Add(obj);
                }

                if (aprob != "0" && aprob != "00000000-0000-0000-0000-000000000000," && aprob != null)
                {
                    var obj = aprob.Split(',');
                    List<Guid> listaAreglo = new List<Guid>();
                    for (int i = 0; i < obj.Count() - 1; i++)
                    {
                        listaAreglo.Add(new Guid(obj[i]));
                    }
                    var obb = listaAreglo.Where(e => e.Equals(new Guid("00000000-0000-0000-0000-000000000000"))).ToList();
                    if (obb.Count == 0)
                    {
                        ProActi = ProActi.Where(e => listaAreglo.Contains(e.id)).ToList();
                    }
                }
                return Ok(ProActi);
            }
            catch (Exception eror)
            {
                var algo = eror;
            }
            return Ok("Por el momento el servidor no responde");
        }


        [HttpPost]
        [Route("candidatos")]
        [Authorize]
        public IHttpActionResult candidatos(ReportesDto source)
        {
            DateTime FechaF = DateTime.Now;
            DateTime FechaI = DateTime.Now;
            int Edad = Convert.ToInt32(source.edad);
            int Genero = Convert.ToInt32(source.genero);
            try
            {
                if (source.fini != null)
                {
                    FechaI = Convert.ToDateTime(source.fini);
                }
                if (source.ffin != null)
                {
                    FechaF = Convert.ToDateTime(source.ffin);
                }
         
            FechaF = FechaF.AddDays(1);
            //var candidato = db.Candidatos.Where(e => e.fch_Creacion >= FechaI && e.fch_Creacion < FechaF).ToList();
            //var listcandi = candidato.Select(e => e.Id).ToList();
            //var entidad = db.Entidad.Where(e => listcandi.Contains(e.Id)).ToList();

            //var proceso = candidato.Select(e => new { e.Id, estatuid = db.ProcesoCandidatos.Where(x=>x.CandidatoId == e.Id).ToList().Count < 1? 0: db.ProcesoCandidatos.Where(x => x.CandidatoId == e.Id).FirstOrDefault().EstatusId }).ToList();
            //var consul = proceso.Select(e => new {
            //    e.Id,
            //    e.estatuid,
            //    nombre = e.estatuid > 0? db.Estatus.Where(x=>x.Id == e.estatuid).FirstOrDefault().Descripcion : "DISPONIBLE"
            //}).ToList();

            var datos = db.Candidatos.Where(x => x.fch_Creacion >= FechaI && x.fch_Creacion < FechaF).OrderByDescending(o => o.fch_Creacion).Select(e => new
            {
                e.Id,
                nombre = e.Nombre + " " + e.ApellidoPaterno + " " + e.ApellidoMaterno,
                curp = string.IsNullOrEmpty(e.CURP) ? "S/R" : e.CURP,
                rfc = string.IsNullOrEmpty(e.RFC) ? "S/R" : e.RFC,
                estadoId = e.estadoNacimiento.Id,
                e.estadoNacimiento.estado,
                edad = e.FechaNacimiento == null ? 0 : DateTime.Now.Year - e.FechaNacimiento.Value.Year,
                e.Genero.genero,
                e.GeneroId,
                //estatusId = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(e.Id)).OrderByDescending(o => o.Fch_Modificacion).Select(ee => ee.Estatus.Id).FirstOrDefault(),
                //estatus = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(e.Id)).OrderByDescending(o => o.Fch_Modificacion).Select(ee => ee.Estatus.Descripcion).FirstOrDefault(),
                estatusId = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(e.Id)).OrderByDescending(o => o.Fch_Modificacion).Select(ee => ee.Estatus.Id).FirstOrDefault(),
                estatus = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(e.Id)).OrderByDescending(o => o.Fch_Modificacion).Select(ee => ee.Estatus.Descripcion).FirstOrDefault() == null ? "SIN PROCESO" : db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(e.Id)).OrderByDescending(o => o.Fch_Modificacion).Select(ee => ee.Estatus.Descripcion).FirstOrDefault(),
                avance = db.AvancePerfil.Where(x => x.PerfilCandidatoId.Equals(e.Id)).Count() == 0 ? 10 :
                db.AvancePerfil.Where(x => x.PerfilCandidatoId.Equals(e.Id)).Select(a => a.Avance).FirstOrDefault(),
                fecha = e.fch_Creacion
            }).ToList().Select((row, index) => new {
                rowIndex = index,
                row.Id,
                row.nombre,
                row.rfc,
                row.curp,
                row.estadoId,
                row.estado,
                row.edad,
                row.genero,
                row.GeneroId,
                row.estatusId,
                row.estatus,
                row.avance,
                row.fecha
            }).ToList();

            if(Edad != 0 && datos.Count() > 0)
            {
               datos = datos.Where(e => e.edad > Edad -1 && e.edad < Edad + 5).ToList();
            }

            if (Genero != 0 && datos.Count() > 0)
            {
               datos = datos.Where(e => e.GeneroId == Genero).ToList();
            }

            if (source.estadoId.Count() > 0 && datos.Count() > 0)
            {
                datos = datos.Where(e => source.estadoId.Contains(e.estadoId)).ToList();
            }

            if (source.stus.Count() > 0 && datos.Count() > 0)
            {
                datos = datos.Where(e => source.stus.Contains(e.estatusId)).ToList();
            }
            var countObj = datos.Count();

            if( countObj > 0 )
                {
                    var contratados = datos.Where(x => x.estatusId.Equals(24)).Count();
                    var seguimiento = datos.Where(x => !x.estadoId.Equals(0)).Count() > 0 ?
                        datos.Where(x => !x.estadoId.Equals(0)).Count() * 100 / datos.Count() : 0;
                    var rowi = datos[source.rowIndex[0]].rowIndex;
                    if (source.rowIndex[1] > countObj)
                    {
                        source.rowIndex[1] = countObj - 1;
                    }
                    var rowe = datos[source.rowIndex[1]].rowIndex;
                    var candidatos = datos.Where(e => e.rowIndex >= rowi && e.rowIndex <= rowe).ToList();
                    var obj = new
                    {
                        totales = new { total = countObj, contratados, seguimiento },
                        candidatos
                    };
                    //var result = db.Candidatos.Where(x => candidatos.Contains(x.Id)).Select(e => new
                    //{
                    //    e.Id,
                    //    total = countObj,
                    //    contratados = contratados,
                    //    nombre = e.Nombre + " " + e.ApellidoPaterno + " " + e.ApellidoMaterno,
                    //    curp = string.IsNullOrEmpty(e.CURP) ? "S/R" : e.CURP,
                    //    rfc = string.IsNullOrEmpty(e.RFC) ? "S/R" : e.RFC,
                    //    estadoId = e.estadoNacimiento.Id,
                    //    e.estadoNacimiento.estado,
                    //    edad = e.FechaNacimiento == null ? 0 : DateTime.Now.Year - e.FechaNacimiento.Value.Year,
                    //    e.Genero.genero,
                    //    e.GeneroId,
                    //    estatusId = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(e.Id)).OrderByDescending(o => o.Fch_Modificacion).Select(ee => ee.Estatus.Id).FirstOrDefault(),
                    //    estatus = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(e.Id)).OrderByDescending(o => o.Fch_Modificacion).Select(ee => ee.Estatus.Descripcion).FirstOrDefault() == null ? "SIN PROCESO" : db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(e.Id)).OrderByDescending(o => o.Fch_Modificacion).Select(ee => ee.Estatus.Descripcion).FirstOrDefault(),
                    //    avance = db.AvancePerfil.Where(x => x.PerfilCandidatoId == e.Id).ToList().Count == 0 ? 0 : db.AvancePerfil.Where(x => x.PerfilCandidatoId == e.Id).FirstOrDefault().Avance
                    //});

                    return Ok(obj);
                }
            else
                {
                    return Ok(datos);
                }
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }
        }

        [HttpGet]
        [Route("vacante")]
        public IHttpActionResult vacante(string cliente, string coordina)
        {
           
            int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39 };
            
            try
            {
                Guid clienteID = new Guid(cliente);
                var requi = db.Requisiciones.Where(e => EstatusList.Contains(e.EstatusId) && e.Activo == true && e.ClienteId == clienteID).ToList();
                string nombrecliente = db.Clientes.Where(e => e.Id == clienteID).FirstOrDefault().Nombrecomercial;
                var requiID = requi.Select(e => e.Id).ToList();
                var ListaCubierta = db.ProcesoCandidatos.Where(e => requiID.Contains(e.RequisicionId) && e.EstatusId == 24).ToList();
                var datos = requi.Select(e=> new {
                    perfil = e.VBtra,
                    numeropos = e.horariosRequi.Sum(s => s.numeroVacantes),
                    cubierta = db.ProcesoCandidatos.Where(x => x.RequisicionId == e.Id && x.EstatusId == 24).Count(),
                    faltantes = db.HorariosRequis.Where(x => x.RequisicionId == e.Id).Sum(x => x.numeroVacantes) - db.ProcesoCandidatos.Where(x => x.RequisicionId == e.Id && x.EstatusId == 24).Count(),
                    porsentaje = e.horariosRequi.Sum(s => s.numeroVacantes) > 0 ? (db.ProcesoCandidatos.Where(p => p.RequisicionId == e.Id && p.EstatusId == 24).Count()) * 100 / e.horariosRequi.Sum(s => s.numeroVacantes) : 0,
                    cordinacion = e.ClaseReclutamiento.clasesReclutamiento,
                    e.ClaseReclutamientoId,
                    vacantenombre = nombrecliente
                }).OrderByDescending(e=>e.numeropos).ToList();
                if (coordina != "0" && coordina != null)
                {
                    var obj = coordina.Split(',');
                    List<int> listaAreglo = new List<int>();
                    for (int i = 0; i < obj.Count() - 1; i++)
                    {
                        listaAreglo.Add(Convert.ToInt32(obj[i]));
                    }
                    var obb = listaAreglo.Where(e => e.Equals(0)).ToList();
                    if (obb.Count == 0)
                    {
                        datos = datos.Where(e => listaAreglo.Contains(e.ClaseReclutamientoId)).ToList();
                    }
                }
                return Ok(datos);
            }
            catch (Exception ex)
            {
                var mensaje = ex.Message;
                var datos = db.Requisiciones.Where(e => e.VBtra == "error provocado").ToList();
                return Ok(datos);
            }
          
        }


        [HttpGet]
        [Route("clientes")]
        public IHttpActionResult Clientes(string fini, string ffin,string bandera)
        {

            int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39 };
            
            try
            {
                if (bandera == "3")
                {
                    var clientedetalle = db.Clientes.Where(e => e.Activo == true && e.esCliente == true).ToList();
                    var clienteactivo = db.Requisiciones.Select(e => e.ClienteId).Distinct().ToList();
                    var clientedetalle2 = clientedetalle.Where(e => !clienteactivo.Contains(e.Id)).ToList();
                    var telefono = db.Telefonos.ToList();
                    var email = db.Emails.ToList();
                    var datos = clientedetalle2.Select(e => new
                    {
                        e.Id,
                        nombre = e.Nombrecomercial,
                        razon = e.RazonSocial,
                        rfc = e.RFC,
                        telefono = telefono.Where(x=>x.esPrincipal == true && x.EntidadId == e.Id).Count() == 0? "": telefono.Where(x => x.esPrincipal == true && x.EntidadId == e.Id).FirstOrDefault().ClaveLada + telefono.Where(x => x.esPrincipal == true && x.EntidadId == e.Id).FirstOrDefault().telefono,
                        email = email.Where(x=>x.esPrincipal == true && x.EntidadId == e.Id).Count() == 0? "" : email.Where(x => x.esPrincipal == true && x.EntidadId == e.Id).FirstOrDefault().email
                    }).ToList();
                    return Ok(datos);
                }
                if (bandera == "4")
                {
                    var rangofecha = DateTime.Now.AddMonths(-3);
                    var clientedetalle = db.Clientes.Where(e => e.Activo == true && e.esCliente == true).ToList();
                    var clientepasado = db.Requisiciones.Where(e=>e.fch_Creacion < rangofecha).Select(e => new { e.ClienteId }).Distinct().ToList();
                    var clientepresente = db.Requisiciones.Where(e => e.fch_Creacion > rangofecha).Select(e => e.ClienteId).Distinct().ToList();
                    var listaCliente = clientepasado.Where(e => !clientepresente.Contains(e.ClienteId)).Select(e=>e.ClienteId).ToList();
                    clientedetalle = clientedetalle.Where(e => listaCliente.Contains(e.Id)).ToList();
                    var telefono = db.Telefonos.ToList();
                    var email = db.Emails.ToList();
                    var datos = clientedetalle.Select(e => new
                    {
                        e.Id,
                        nombre = e.Nombrecomercial,
                        razon = e.RazonSocial,
                        rfc = e.RFC,
                        telefono = telefono.Where(x => x.esPrincipal == true && x.EntidadId == e.Id).Count() == 0 ? "" : telefono.Where(x => x.esPrincipal == true && x.EntidadId == e.Id).FirstOrDefault().ClaveLada + telefono.Where(x => x.esPrincipal == true && x.EntidadId == e.Id).FirstOrDefault().telefono,
                        email = email.Where(x => x.esPrincipal == true && x.EntidadId == e.Id).Count() == 0 ? "" : email.Where(x => x.esPrincipal == true && x.EntidadId == e.Id).FirstOrDefault().email
                    }).ToList();
                    return Ok(datos);
                }

                DateTime FechaF = DateTime.Now;
                DateTime FechaI = DateTime.Now;

                if (fini != null)
                {
                    FechaI = Convert.ToDateTime(fini);
                }
                if (ffin != null)
                {
                    FechaF = Convert.ToDateTime(ffin);
                }
                FechaF = FechaF.AddDays(1);

                var cliente = db.Clientes.Where(e=>e.Activo == true && e.esCliente == true).ToList();
                var requis = db.Requisiciones.Where(e=>EstatusList.Contains(e.EstatusId) && e.fch_Creacion >= FechaI && e.fch_Creacion < FechaF).ToList();
                int pos = 0;
                int cub = 0;
                List<proactividad> lista = new List<proactividad>();
                foreach (var item in cliente)
                {
                    var ob = new proactividad();
                    pos = 0;
                    cub = 0;
                    var requien = requis.Where(x => x.ClienteId == item.Id).Select(x => x.Id).ToList();
                    if (requien.Count > 0)
                    {
                        pos = db.HorariosRequis.Where(a => requien.Contains(a.RequisicionId)).Sum(x => x.numeroVacantes);
                        cub = db.ProcesoCandidatos.Where(x => requien.Contains(x.RequisicionId) && x.EstatusId == 24).Distinct().Count();
                    }
                    
                    ob.nombre = item.Nombrecomercial;
                    ob.numeropos = pos;
                    ob.cubiertas = cub;
                    ob.puntaje = 0;
                    ob.porcentaje = 0;
                    if (pos > 0)
                    {
                        ob.puntaje = pos - cub;
                        ob.porcentaje = (cub * 100) / pos;
                    }
                    if (bandera == "2")
                    {
                        if (pos > 0)
                        {
                            lista.Add(ob);
                        }
                    }
                    else
                    {
                        lista.Add(ob);
                    }
                   
                }
                lista = lista.OrderByDescending(e => e.numeropos).ToList();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                var mensaje = ex.Message;
                var datos = db.Requisiciones.Where(e => e.VBtra == "error provocado").ToList();
                return Ok(datos);
            }

        }


        [HttpGet]
        [Route("consultavacante")]
        public IHttpActionResult ConsultaVacante(string busquedad)
        {
            int[] EstatusList = new[] { 4,8,9, 34,35,36,37,43,44,45, 46,47,48};

            var vct = (from vacante in db.Requisiciones
                       where !EstatusList.Contains(vacante.EstatusId) && vacante.VBtra.ToLower().StartsWith(busquedad.Trim().ToLower())
                            || !EstatusList.Contains(vacante.EstatusId) && vacante.VBtra.ToLower().Contains(busquedad.Trim().ToLower())
                            || !EstatusList.Contains(vacante.EstatusId) && vacante.Folio.ToString().ToLower().Contains(busquedad.Trim().ToLower())
                       select vacante
                                       ).ToList();

            var requis = vct.Where(e => e.Activo == true && e.Confidencial == false).ToList();
            var requien = requis.Select(e => e.Id).ToList();
            var asig = db.AsignacionRequis.Where(e=> requien.Contains(e.RequisicionId) && e.Tipo.Equals(2)).ToList();
            var grupu = asig.Select(e => e.GrpUsrId).ToList();
           
            var user = db.Usuarios.Where(e => grupu.Contains(e.Id)).Select(e =>new  {
                nombre = e.Nombre + " " + e.ApellidoPaterno + " " + e.ApellidoMaterno,
                e.Id,
                email = e.emails.FirstOrDefault().email,
                e.SucursalId,
                sucursal = e.Sucursal.Nombre
                }).ToList();
            var sucur = user.Select(e => e.SucursalId).ToList();
            var oficina = db.Usuarios.Where(e => sucur.Contains(e.Id)).ToList();

            var datos = requis.Select(e => new {
                e.Folio,
                e.VBtra,
                reclutador = asig.Where(x => x.RequisicionId == e.Id).Count() > 0? asig.Where(x=>x.RequisicionId == e.Id).Select(x=>
                new { nombre = user.Where(a=>a.Id == x.GrpUsrId).FirstOrDefault().nombre}).FirstOrDefault().nombre: "SIN ASIGNAR",

                email = asig.Where(x => x.RequisicionId == e.Id).Count() > 0 ? asig.Where(x => x.RequisicionId == e.Id).Select(x =>
                   new { nombre = user.Where(a => a.Id == x.GrpUsrId).FirstOrDefault().email }).FirstOrDefault().nombre : "SIN ASIGNAR",

                sucursal = asig.Where(x => x.RequisicionId == e.Id).Count() > 0 ? asig.Where(x => x.RequisicionId == e.Id).Select(x =>
                   new { nombre = user.Where(a => a.Id == x.GrpUsrId).FirstOrDefault().sucursal }).FirstOrDefault().nombre : "SIN ASIGNAR",

            }).ToList();
                
 
                
                return Ok(datos);
        }

        [HttpGet]
        [Route("mapafolios")]
        public IHttpActionResult MapaFolios()
        {
            try
            {
                var inicio = Convert.ToDateTime("2019/" + DateTime.Now.Month.ToString() + "/01");
                //&& (e.fch_Creacion >= inicio && e.fch_Creacion <= DateTime.Now
                int[] EstatusList = new[] { 4, 6, 7, 29, 30, 31, 32, 33, 38, 39 };
                int[] EstatusCub = new[] { 34, 35, 36, 37, 47, 48 };
                int[] estatusId = new int[] { 8, 9, 34, 35, 36, 37, 47, 48};

             
                var requi = db.Requisiciones.Where(e => e.Direccion.Pais.Id.Equals(42) 
                && !estatusId.Contains(e.EstatusId)).GroupBy(g => g.Direccion.EstadoId).Select(k => new
                {
                    estado = k.Select(est => est.Direccion.Estado.estado).FirstOrDefault(),
                    estadoId = k.Key,
                    folios = k.Select(x => x.Id).Count(),
                    foliosActivos = k.Where(x => EstatusList.Contains(x.EstatusId)).Count(),
                    fch_Creacion = k.Select(x => x.fch_Creacion),
                    vacantes = k.Select(r => new {
                        vacantes = db.HorariosRequis.Where(x => x.RequisicionId.Equals(r.Id)).Sum(s => s.numeroVacantes)
                        }).Sum(s => s.vacantes),
                    cubiertas = k.Select(r => new {
                        mocos = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(r.Id) && x.EstatusId.Equals(24)).Count()
                    }).Sum(s => s.mocos),
                    nueva = k.Where(x => x.EstatusId.Equals(4)).Count(),
                    aprobada = k.Where(x => x.EstatusId.Equals(6)).Count(),
                    disenada = k.Where(x => x.EstatusId.Equals(7)).Count(),
                    busqueda = k.Where(x => x.EstatusId.Equals(29)).Count(),
                    envio = k.Where(x => x.EstatusId.Equals(30)).Count(),
                    nuevabusq = k.Where(x => x.EstatusId.Equals(31)).Count(),
                    socioeconomico = k.Where(x => x.EstatusId.Equals(32)).Count(),
                    espera = k.Where(x => x.EstatusId.Equals(33)).Count(),
                    garantia = k.Where(x => x.EstatusId.Equals(38)).Count(),
                    pausada = k.Where(x => x.EstatusId.Equals(39)).Count(),
                    latitude = k.Select(geo => geo.Direccion.Estado.Latitud).FirstOrDefault(),
                    longitude = k.Select(geo => geo.Direccion.Estado.Longitud).FirstOrDefault(),
                }).OrderBy(o => o.estado).ToList();

               return Ok(requi);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("avanceReclutadorCliente")]
        [Authorize]
        public IHttpActionResult AvanceReclutadorCliente(string fecha)
        {
            int[] estatusId = new int[] { 8, 9, 34, 35, 36, 37, 47, 48 };
            try
            {
                DateTime f = Convert.ToDateTime(fecha);
                DateTime ff = f.AddDays(1);
                //var report = db.AsignacionRequis.Where(x => x.Tipo.Equals(2) && !estatusId.Contains(x.Requisicion.EstatusId)).GroupBy(g => g.GrpUsrId).Select(r => new
                //{
                //    id = r.Key,
                //    nombre = db.Usuarios.Where(x => x.Id.Equals(r.Key)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                //    total = r.Select(x => x.Requisicion.Id).Count(),
                //    resumen = r.GroupBy(gg => gg.Requisicion.ClienteId).Select(s => new
                //    {
                //        clienteId = s.Key,
                //        nombrecomercial = s.Select(c => c.Requisicion.Cliente.Nombrecomercial).FirstOrDefault(),
                //        RazonSocial = s.Select(c => c.Requisicion.Cliente.RazonSocial).FirstOrDefault(),
                //        vacantes = s.Select(c => c.Requisicion.horariosRequi.Sum(h => h.numeroVacantes)).Sum(),
                //        enProceso = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(s.Select(xxx => xxx.Requisicion.Id).FirstOrDefault())
                //        && x.ReclutadorId.Equals(r.Key) && !x.Estatus.Descripcion.ToLower().Equals("cubierto") && !x.Estatus.Descripcion.ToLower().Equals("contratado")
                //        && !x.Estatus.Descripcion.ToLower().Equals("liberado")).Select(c => c.CandidatoId).Count(),
                //        contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(s.Select(xxx => xxx.Requisicion.Id).FirstOrDefault()) && x.ReclutadorId.Equals(r.Key)
                //        && x.Estatus.Descripcion.ToLower().Equals("cubierto")).Select(c => c.CandidatoId).Count()
                //    }),
                //    totalProceso = r.GroupBy(gg => gg.Requisicion.ClienteId).Select(s => new
                //    {
                //        enProceso = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(s.Select(xxx => xxx.Requisicion.Id).FirstOrDefault()) && x.ReclutadorId.Equals(r.Key)
                //        && !x.Estatus.Descripcion.ToLower().Equals("cubierto")  && !x.Estatus.Descripcion.ToLower().Equals("contratado")
                //        && !x.Estatus.Descripcion.ToLower().Equals("liberado")).Select(c => c.CandidatoId).Count(),
                //    }).Sum(c => c.enProceso),
                //    totalCubiertos = r.GroupBy(gg => gg.Requisicion.ClienteId).Select(s => new
                //    {
                //        contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(s.Select(xxx => xxx.Requisicion.Id).FirstOrDefault()) && x.ReclutadorId.Equals(r.Key)
                //        && x.Estatus.Descripcion.ToLower().Equals("cubierto")).Select(c => c.CandidatoId).Count()
                //    }).Sum(c => c.contratados),
                //}).ToList();


                var report = db.AsignacionRequis.Where(x => x.Tipo.Equals(2) && !estatusId.Contains(x.Requisicion.EstatusId)).GroupBy(g => g.GrpUsrId).Select(r => new
                {
                    id = r.Key,
                    nombre = db.Usuarios.Where(x => x.Id.Equals(r.Key)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                    total = r.Select(x => x.Requisicion.Id).Count(),
                    resumen = r.GroupBy(gg => gg.Requisicion.ClienteId).Select(s => new
                    {
                        clienteId = s.Key,
                        nombrecomercial = s.Select(c => c.Requisicion.Cliente.Nombrecomercial).FirstOrDefault(),
                        RazonSocial = s.Select(c => c.Requisicion.Cliente.RazonSocial).FirstOrDefault(),
                        vacantes = s.Select(c => c.Requisicion.horariosRequi.Sum(h => h.numeroVacantes)).Sum(),
                        enProceso = s.Select(c => new
                        {
                            candidatos = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(c.RequisicionId)
                                         && x.ReclutadorId.Equals(r.Key) && x.Fch_Modificacion >= f && x.Fch_Modificacion < ff && !x.Estatus.Descripcion.ToLower().Equals("liberado")
                                         && !x.Estatus.Descripcion.ToLower().Equals("cubierto") && !x.Estatus.Descripcion.ToLower().Equals("contratado"))
                                         .Select(cc => cc.CandidatoId).Count()
                        }).Sum(sp => sp.candidatos),
                        contratados = s.Select(c => new
                        {
                            candidatos = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(c.RequisicionId) && x.ReclutadorId.Equals(r.Key)
                                                                          && x.Fch_Modificacion >= f && x.Fch_Modificacion < ff && x.Estatus.Descripcion.ToLower().Equals("cubierto"))
                                                                          .Select(cc => cc.CandidatoId).Count()
                        }).Sum(sc => sc.candidatos)
                    }),
                    totalProceso = r.GroupBy(gg => gg.Requisicion.ClienteId).Select(s => new
                    {
                        enProceso = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(s.Select(xxx => xxx.Requisicion.Id).FirstOrDefault()) && x.ReclutadorId.Equals(r.Key)
                        && !x.Estatus.Descripcion.ToLower().Equals("cubierto") && x.Fch_Modificacion >= f && x.Fch_Modificacion < ff
                        && !x.Estatus.Descripcion.ToLower().Equals("liberado")
                        && !x.Estatus.Descripcion.ToLower().Equals("contratado")).Select(c => c.CandidatoId).Count(),
                    }).Sum(c => c.enProceso),
                    totalCubiertos = r.GroupBy(gg => gg.Requisicion.ClienteId).Select(s => new
                    {
                        contratados = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(s.Select(xxx => xxx.Requisicion.Id).FirstOrDefault()) && x.ReclutadorId.Equals(r.Key)
                        && x.Fch_Modificacion >= f && x.Fch_Modificacion < ff
                        && x.Estatus.Descripcion.ToLower().Equals("cubierto")).Select(c => c.CandidatoId).Count()
                    }).Sum(c => c.contratados),
                }).ToList();

                var mocos = db.AsignacionRequis.Where(x => x.Tipo.Equals(2) && !estatusId.Contains(x.Requisicion.EstatusId)).Select(r => r.GrpUsrId).Distinct().ToList();
                var report2 = db.ProcesoCandidatos.Where(x => mocos.Contains(x.ReclutadorId)).GroupBy(g => g.ReclutadorId)
                     .Select(c => new
                     {
                         nombre = db.Usuarios.Where(x => x.Id.Equals(c.Key)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                         citados = c.GroupBy(gr => gr.RequisicionId).Select( rr => new {
                             candidatos = db.InformeRequisiciones.Where(x => x.RequisicionId.Equals(rr.Key)
                        && x.fch_Modificacion >= f && x.fch_Modificacion < ff && x.Estatus.Descripcion.ToLower().Equals("cita reclutamiento")).Count()
                         }).Sum(s => s.candidatos),
                         enviados = c.GroupBy(gr => gr.RequisicionId).Select(rr => new {
                             candidatos = db.InformeRequisiciones.Where(x => x.RequisicionId.Equals(rr.Key)
                        && x.fch_Modificacion >= f && x.fch_Modificacion < ff && x.Estatus.Descripcion.ToLower().Equals("entrevista cliente")).Count()
                         }).Sum(s => s.candidatos),
                         cubiertos = c.GroupBy(gr => gr.RequisicionId).Select(rr => new {
                             candidatos = db.InformeRequisiciones.Where(x => x.RequisicionId.Equals(rr.Key)
                        && x.fch_Modificacion >= f && x.fch_Modificacion < ff && x.Estatus.Descripcion.ToLower().Equals("cubierto")).Count()
                         }).Sum(s => s.candidatos),
                     }).ToList();
                     
                
                return Ok(new { report, report2 } );
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }

        public class proactividad
        {
            public string nombre { get; set; }
            public int vacantes { get; set; }
            public int cubiertas { get; set; }
            public int puntaje { get; set; }
            public int numeropos { get; set; }
            public int porcentaje { get; set; }
            public Guid id { get; set; }
        }

        public class Calculo
        {
            public int PuntajeCalculo(Guid id, List<ProcesoCandidato> lista)
            {
                using (SAGADBContext dbs = new SAGADBContext())
                {
                    int total = 0;
                    var requi = lista.Where(e => e.ReclutadorId == id).Select(e => e.RequisicionId).ToList();
                    try
                    {
                        var datos = dbs.PonderacionRequisiciones.Where(e => requi.Contains(e.RequisicionId)).Select(e => new
                        {
                            e.RequisicionId,
                            e.Ponderacion
                        }).ToList();
                        var ponderacion = datos.Select(e => new
                        {
                            puntos = lista.Where(a => a.RequisicionId == e.RequisicionId).ToList().Count() * e.Ponderacion
                        }).ToList();
                        total = ponderacion.Sum(e => e.puntos);
                    }
                    catch (Exception)
                    {
                        total = 0;
                    }
                    return total;
                }
            }
        }

    }
}
