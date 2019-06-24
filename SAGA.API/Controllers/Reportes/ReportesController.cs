
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

namespace SAGA.API.Controllers.Reportes
{
    [RoutePrefix("api/reporte")]
    public class ReportesController : ApiController
    {
        private SAGADBContext db = new SAGADBContext();


        [HttpGet]
        [Route("Informe")]
        public IHttpActionResult Informe(string clave, string ofc, string tipo, string fini,
           string ffin, string emp, string sol, string trcl, string cor, string stus, string recl, string usercor)
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
            var datos2 = db.Requisiciones.Where(e => e.fch_Creacion >= FechaI
            && e.fch_Creacion <= FechaF  && e.Confidencial == false).OrderByDescending(e=>e.fch_Creacion).ToList();

            if (tipo == "2" || tipo == "6")
            {
             datos2 = db.Requisiciones.Where(e => e.fch_Modificacion >= FechaI
             && e.fch_Modificacion <= FechaF && e.Confidencial == false).OrderByDescending(e => e.fch_Modificacion).ToList();
            }
            var requi = datos2.Select(e => e.Id).ToList();
      //      var aprobador = datos2.Select(e => new { e.AprobadorId, e.Id }).ToList();
            var nombreReclu = db.AsignacionRequis.Where(a => requi.Contains(a.RequisicionId)).Select(a => new { a.RequisicionId, a.GrpUsrId, Nombre = db.Usuarios.Where(e=>e.Id == a.GrpUsrId).FirstOrDefault().Nombre.ToUpper() +" "+ db.Usuarios.Where(e => e.Id == a.GrpUsrId).FirstOrDefault().ApellidoPaterno.ToUpper() + " " + db.Usuarios.Where(e => e.Id == a.GrpUsrId).FirstOrDefault().ApellidoMaterno.ToUpper() }).ToList();
           // var nombreReclu = db.Usuarios.Where(x => lago.Contains(x.Id)).Select(x => new { x.Nombre,x.Id }).ToList();
            //var cadenas = nombreReclu.Where(b => b.Id == new Guid("2217b0f2-5a6e-e811-80e1-9e274155325e")).Select(b => b.Nombre).ToList();
            //String.Join(String.Empty, cadenas.ToArray());

            var datos = datos2.Select(e => new
            {
                e.Id,
                e.Folio,
                VBtra = e.VBtra.ToUpper(),
                cubierta = db.ProcesoCandidatos.Where(x => x.RequisicionId == e.Id && x.EstatusId == 24).Select(a => a.CandidatoId).Distinct().ToList().Count,
                porcentaje = e.horariosRequi.Sum(s => s.numeroVacantes) > 0 ? (db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count()) * 100 / e.horariosRequi.Sum(s => s.numeroVacantes) : 0,
                e.fch_Creacion,
                e.fch_Modificacion,
                e.fch_Limite,
                empresa = e.Cliente.Nombrecomercial.ToUpper(),
                e.ClienteId,
                e.AprobadorId,
                cordinador2 = db.Usuarios.Where(x=>x.Usuario == e.Aprobador).ToList().Count > 0? db.Usuarios.Where(x => e.Aprobador.Contains(x.Usuario)).Select(x=>x.Nombre + " " + x.ApellidoPaterno + " " + x.ApellidoMaterno).FirstOrDefault().ToUpper() : "",
                nombreApellido = db.Usuarios.Where(x => x.Usuario == e.Propietario).FirstOrDefault().Nombre.ToUpper() + " " + db.Usuarios.Where(x => x.Usuario == e.Propietario).FirstOrDefault().ApellidoPaterno.ToUpper() + " " + db.Usuarios.Where(x => x.Usuario == e.Propietario).FirstOrDefault().ApellidoMaterno.ToUpper(),
                propietario = db.Usuarios.Where(x => x.Usuario == e.Propietario).FirstOrDefault().Nombre.ToUpper(),
                Usuario = db.Usuarios.Where(x => x.Usuario == e.Propietario).FirstOrDefault().Usuario,
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
                reclutadorTotal = db.AsignacionRequis.Where(a => a.RequisicionId == e.Id && a.GrpUsrId != e.AprobadorId).Count() == 0? "SIN ASIGNAR" : db.AsignacionRequis.Where(a => a.RequisicionId == e.Id && a.GrpUsrId != e.AprobadorId).Count().ToString(),
                nombreReclutado = String.Join(", ", nombreReclu.Where(b => b.RequisicionId == e.Id && b.GrpUsrId != e.AprobadorId).Select(b => b.Nombre).ToList().ToArray())
        }).ToList();
            

            if (stus != "0" && stus != null)
            {
                var obj = stus.Split(',');
                List<int> listaAreglo = new List<int>();
                for (int i = 0; i < obj.Count() - 1; i++)
                {
                    listaAreglo.Add(Convert.ToInt32(obj[i]));
                }
                var obb = listaAreglo.Where(e => e.Equals(0)).ToList();
                if (obb.Count == 0)
                {
                    datos = datos.Where(e => listaAreglo.Contains(e.EstatusId)).ToList();
                }
            }

            if (clave != null)
            {
                datos = datos.Where(e => e.VBtra.ToLower().Contains(clave.ToLower())).ToList();
            }

            if (sol != "0" && sol != null)
            {
                var obj = sol.Split(',');
                List<string> listaAreglo = new List<string>();
                for (int i = 0; i < obj.Count() - 1; i++)
                {
                    listaAreglo.Add(obj[i]);
                }
                var obb = listaAreglo.Where(e => e.Equals("0")).ToList();
                if (obb.Count == 0)
                {
                    datos = datos.Where(e => listaAreglo.Contains(e.Usuario)).ToList();
                }
            }

            if (emp != "0" && emp != "00000000-0000-0000-0000-000000000000," && emp != null)
            {
                var obj = emp.Split(',');
                List<Guid> listaAreglo = new List<Guid>();
                for (int i = 0; i < obj.Count() - 1; i++)
                {
                    listaAreglo.Add(new Guid(obj[i]));
                }
                var obb = listaAreglo.Where(e => e.Equals(new Guid("00000000-0000-0000-0000-000000000000"))).ToList();
                if (obb.Count == 0)
                {
                    datos = datos.Where(e => listaAreglo.Contains(e.ClienteId)).ToList();
                }
            }
            //if (cor != "0")
            //{
            //    datos = datos.Where(e => e.ClaseReclutamientoId == Convert.ToInt32(cor)).ToList();
            //}
            if (cor != "0" && cor != null)
            {
                var obj = cor.Split(',');
                List<int> listaAreglo = new List<int>();
                for (int i = 0; i < obj.Count() - 1; i++)
                {
                    listaAreglo.Add(Convert.ToInt32(obj[i]));
                }
                var obb = listaAreglo.Where(e => e.Equals("0")).ToList();
                if (obb.Count == 0)
                {
                    datos = datos.Where(e => listaAreglo.Contains(e.ClaseReclutamientoId)).ToList();
                }
            }

            if (usercor != "0" && usercor != null && usercor != "00000000-0000-0000-0000-000000000000,")
            {
                var obj = usercor.Split(',');
                List<Guid> listaAreglo = new List<Guid>();
                for (int i = 0; i < obj.Count() - 1; i++)
                {
                    listaAreglo.Add(new Guid(obj[i]));
                }
                var obb = listaAreglo.Where(e => e.Equals(new Guid("00000000-0000-0000-0000-000000000000"))).ToList();
                if (obb.Count == 0)
                {
                    datos = datos.Where(e => listaAreglo.Contains(e.AprobadorId)).ToList();
                }
            }

            if (trcl != "0" && trcl != null)
            {
                var obj = sol.Split(',');
                List<int> listaAreglo = new List<int>();
                for (int i = 0; i < obj.Count() - 1; i++)
                {
                    listaAreglo.Add(Convert.ToInt32(obj[i]));
                }
                var obb = listaAreglo.Where(e => e.Equals("0")).ToList();
                if (obb.Count == 0)
                {
                    datos = datos.Where(e => listaAreglo.Contains(e.TipoReclutamientoId)).ToList();
                }
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
                    var asigna = db.AsignacionRequis.Where(e => listaAreglo.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                    datos = datos.Where(e => asigna.Contains(e.Id)).ToList();
                }
            }

            if (ofc != "0" && ofc != "0," && ofc != null)
            {
                var obj = ofc.Split(',');
                List<int> listaAreglo = new List<int>();
                for (int i = 0; i < obj.Count() -1; i++)
                {
                    listaAreglo.Add(Convert.ToInt32(obj[i]));
                }

                var obb = listaAreglo.Where(e => e.Equals(0)).ToList();
                if (obb.Count == 0)
                {
                    // Estado = db.Direcciones.Where(x => x.EntidadId == db.Usuarios.Where(a => a.Usuario == e.Propietario).FirstOrDefault().SucursalId).FirstOrDefault().Estado.estado,
                    //int unidad = Convert.ToInt32(ofc);

                    var negocio = db.OficinasReclutamiento.Where(e => listaAreglo.Contains(e.UnidadNegocioId)).Select(e => e.Id).ToList();
                    var estado = db.Direcciones.Where(e => negocio.Contains(e.EntidadId)).Select(e => e.EstadoId).Distinct().ToList();
                    foreach (var item in listaAreglo)
                    {
                        string monterrey = "6,7,10,28,19,24";
                        string jalisco = "1,32,3,8,10,11,14,16,18,2,25,26";
                        string mexico = "4,5,9,13,15,22,27,30,20,12,31,23,21,29,17";
                        if (item == 1)
                        {
                            var array = jalisco.Split(',');
                            for (int i = 0; i < array.Length; i++)
                            {
                                estado.Add(Int32.Parse(array[i]));
                            }
                        }
                        if (item == 2)
                        {
                            var array = mexico.Split(',');
                            for (int i = 0; i < array.Length; i++)
                            {
                                estado.Add(Int32.Parse(array[i]));
                            }
                        }
                        if (item == 3)
                        {
                            var array = monterrey.Split(',');
                            for (int i = 0; i < array.Length; i++)
                            {
                                estado.Add(Int32.Parse(array[i]));
                            }
                        }
                    }
                    estado = estado.Distinct().ToList();
                    datos = datos.Where(e=>estado.Contains(e.EstadoId)).ToList();
                }
            }

            return Ok(datos);
        }


        [HttpGet]
        [Route("empresas")]
        public IHttpActionResult Empresas()
        {
            var fecha = DateTime.Now.AddDays(-15);
            var datos = db.Clientes.Where(e=>e.Activo == true && e.esCliente.Equals(true)).Select(e => new
            {
                e.RFC,
                e.Nombrecomercial,
                e.Id,
                e.RazonSocial,
                fechal = fecha
            }).Distinct().OrderBy(x=>x.RazonSocial).ToList();

            datos.Insert(0, new { RFC = "Todos", Nombrecomercial = "as", Id = new Guid("00000000-0000-0000-0000-000000000000"), RazonSocial = "Todas", fechal = fecha });
            return Ok(datos);
        }


        [HttpGet]
        [Route("usuario")]
        public IHttpActionResult Usuario(string cor)
        {
            
            int[] Status = new[] { 1, 2, 3, 5, 6 };

            if (cor == "1")
            {
                Status = new[] { 4 };
            }
            var datos = db.Usuarios.Where(e => e.Activo == true && Status.Contains(e.TipoUsuarioId)).Select(e => new
            {
                Nombre = e.Nombre + " "+ e.ApellidoPaterno,
                e.Id,
                e.Usuario
            }).OrderBy(x=>x.Nombre).ToList();
            datos.Insert(0, new { Nombre = "Todos", Id = new Guid("00000000-0000-0000-0000-000000000000") , Usuario = "0"});
            return Ok(datos);
        }


        [HttpGet]
        [Route("estatus")]
        public IHttpActionResult Estatus()
        {
            var datos = db.Estatus.Where(e => e.Activo == true && e.TipoMovimiento == 2 && e.Id != 5).Select(e => new
            {
                e.Descripcion,
                e.Id
            }).ToList();
           
           
           datos.Insert(0 ,new { Descripcion = "Todos", Id = 0 });
            return Ok(datos);
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
                e.UnidadNegocioId,
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
            var requi = db.Requisiciones.Where(e=>e.fch_Creacion >= FechaI && e.fch_Creacion <= FechaF).ToList();
           

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
                return Ok(datos);
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
            var recluta = candidatos.Select(e => e.GrpUsrId).Distinct().ToList();
       
            var vacantes = db.Requisiciones.Where(e => listaRequi.Contains(e.Id) && Status.Contains(e.EstatusId)).ToList();
            var datos = db.Usuarios.Where(e => recluta.Contains(e.Id)).ToList();
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
                    var obb = listaAreglo.Where(e => e.Equals("0")).ToList();
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
                    var listaPosicion = db.AsignacionRequis.Where(e => listaRequien.Contains(e.RequisicionId) && e.GrpUsrId == item.Id).Select(e=>e.RequisicionId).ToList();

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
                        obj.puntaje = PuntajeCalculo(item.Id, ListaCubierta);
                       
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
            int[] Status = new[] { 34, 35, 36, 37,47,48 };
            var listaRequi = db.InformeRequisiciones.Where(e => e.fch_Modificacion >= FechaI && e.fch_Modificacion <= FechaF).Select(e => e.RequisicionId).Distinct().ToList();
            listaRequi = db.Requisiciones.Where(e => listaRequi.Contains(e.Id)).Select(a => a.Id).ToList();
            var candidatos = db.AsignacionRequis.Where(e => listaRequi.Contains(e.RequisicionId)).ToList();
            var recluta = candidatos.Select(e => e.GrpUsrId).Distinct().ToList();
            var vacantes = db.Requisiciones.Where(e => listaRequi.Contains(e.Id)).ToList();
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
                    var obb = listaAreglo.Where(e => e.Equals("0")).ToList();
                    if (obb.Count == 0)
                    {
                        vacantes = vacantes.Where(e => listaAreglo.Contains(e.ClaseReclutamientoId)).ToList();
                    }
                }
                
                foreach (var item in datos)
                {
                    var obj = new proactividad();
                    var listaRequien = vacantes.Select(e => e.Id).ToList();
                    var ListaCubierta = db.ProcesoCandidatos.Where(e => e.ReclutadorId == item.Id && e.Fch_Modificacion >= FechaI && e.Fch_Modificacion <= FechaF && e.EstatusId == 24).ToList();
                    var listaPosicion = db.AsignacionRequis.Where(e => listaRequien.Contains(e.RequisicionId) && e.GrpUsrId == item.Id).Select(e => e.RequisicionId).ToList();

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
            int[] Status = new[] { 34, 35, 36, 37, 47, 48 };
            var Requisiciones = db.InformeRequisiciones.Where(e => e.fch_Modificacion >= FechaI && e.fch_Modificacion <= FechaF).ToList();

          //  var Requisiciones = db.AsignacionRequis.Where(e => e.fch_Modificacion >= FechaI && e.fch_Modificacion <= FechaF).ToList();
            var listaRequi = Requisiciones.Select(e => e.RequisicionId).Distinct().ToList();
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
                    var obb = listaAreglo.Where(e => e.Equals("0")).ToList();
                    if (obb.Count == 0)
                    {
                        vacantes = vacantes.Where(e => listaAreglo.Contains(e.ClaseReclutamientoId)).ToList();
                    }
                }

                foreach (var item in datos)
                {
                    var obj = new proactividad();
                    var listaRequien = vacantes.Select(e => e.Id).ToList();
                    var ListaCubierta = db.ProcesoCandidatos.Where(e => e.ReclutadorId == item.Id && e.Fch_Modificacion >= FechaI && e.Fch_Modificacion <= FechaF && e.EstatusId == 24).ToList();
                    var listaPosicion = db.AsignacionRequis.Where(e => listaRequien.Contains(e.RequisicionId) && e.GrpUsrId == item.Id).Select(e => e.RequisicionId).ToList();

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


        [HttpGet]
        [Route("candidatos")]
        public IHttpActionResult candidatos(string fini, string ffin, int edad, int avance, int genero, int estadoID)
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
            var candidato = db.Candidatos.Where(e => e.fch_Creacion >= FechaI && e.fch_Creacion <= FechaF).ToList();

             var datos = candidato.Select(e=> new {
                e.Id,
                e.CURP,
                e.RFC,
                estadoId = e.estadoNacimiento.Id,
                e.estadoNacimiento.estado,
                edad = DateTime.Now.Year - e.FechaNacimiento.Value.Year,
                e.Genero,
                e.GeneroId,
                avance = 0
            }).ToList();

            if(edad != 0)
            {
                datos.Where(e => e.edad > edad + 1 && e.edad < edad + 6).ToList();
            }

            if (genero != 0)
            {
                datos.Where(e => e.GeneroId == genero).ToList();
            }

            if (estadoID != 0)
            {
                datos.Where(e => e.estadoId == estadoID).ToList();
            }
            

            return Ok(datos);
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
   


        public int PuntajeCalculo(Guid id, List<ProcesoCandidato> lista)
        {
            int total = 0;
            var requi = lista.Where(e=>e.ReclutadorId == id).Select(e => e.RequisicionId).ToList();
            try
            {
                var datos = db.PonderacionRequisiciones.Where(e => requi.Contains(e.RequisicionId)).Select(e => new
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
