﻿
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
           string ffin, string emp, string sol, string trcl, string cor, string stus, string recl)
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
            && e.fch_Creacion <= FechaF && e.EstatusId != 9 && e.Confidencial == false).OrderByDescending(e=>e.fch_Creacion).ToList();

            if (tipo == "2" || tipo == "6")
            {
             datos2 = db.Requisiciones.Where(e => e.fch_Modificacion >= FechaI
             && e.fch_Modificacion <= FechaF && e.EstatusId != 9 && e.Confidencial == false).OrderByDescending(e => e.fch_Modificacion).ToList();
            }
            var requi = datos2.Select(e => e.Id).ToList();
            var nombreReclu = db.AsignacionRequis.Where(a => requi.Contains(a.RequisicionId)).Select(a => new { a.RequisicionId, Nombre = db.Usuarios.Where(e=>e.Id == a.GrpUsrId).FirstOrDefault().Nombre +" "+ db.Usuarios.Where(e => e.Id == a.GrpUsrId).FirstOrDefault().ApellidoPaterno }).ToList();
           // var nombreReclu = db.Usuarios.Where(x => lago.Contains(x.Id)).Select(x => new { x.Nombre,x.Id }).ToList();
            //var cadenas = nombreReclu.Where(b => b.Id == new Guid("2217b0f2-5a6e-e811-80e1-9e274155325e")).Select(b => b.Nombre).ToList();
            //String.Join(String.Empty, cadenas.ToArray());

            var datos = datos2.Select(e => new
            {
                e.Id,
                e.Folio,
                e.VBtra,
                cubierta = db.ProcesoCandidatos.Where(x => x.RequisicionId == e.Id && x.EstatusId == 24).Select(a => a.CandidatoId).Distinct().ToList().Count,
                porcentaje = e.horariosRequi.Sum(s => s.numeroVacantes) > 0 ? (db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count()) * 100 / e.horariosRequi.Sum(s => s.numeroVacantes) : 0,
                e.fch_Creacion,
                e.fch_Modificacion,
                e.fch_Limite,
                empresa = e.Cliente.Nombrecomercial,
                e.ClienteId,
                cordinador2 = db.Usuarios.Where(x=>x.Usuario == e.Aprobador).ToList().Count > 0? db.Usuarios.Where(x => e.Aprobador.Contains(x.Usuario)).Select(x=>x.Nombre + " " + x.ApellidoPaterno).FirstOrDefault() : "",
                nombreApellido = db.Usuarios.Where(x => x.Usuario == e.Propietario).FirstOrDefault().Nombre + " " + db.Usuarios.Where(x => x.Usuario == e.Propietario).FirstOrDefault().ApellidoPaterno,
                propietario = db.Usuarios.Where(x => x.Usuario == e.Propietario).FirstOrDefault().Nombre,
                Usuario = db.Usuarios.Where(x => x.Usuario == e.Propietario).FirstOrDefault().Usuario,
                Estado = e.Direccion.Estado.estado,
                e.Direccion.EstadoId,
                numero = e.horariosRequi.Sum(a => a.numeroVacantes),
                e.EstatusId,
                e.TipoReclutamientoId,
                e.TipoReclutamiento.tipoReclutamiento,
                e.ClaseReclutamiento.clasesReclutamiento,
                e.ClaseReclutamientoId,
                e.fch_Cumplimiento,
                estatus = e.Estatus.Descripcion,
                reclutadorTotal = db.AsignacionRequis.Where(a => a.RequisicionId == e.Id).Count() == 0? "SIN ASIGNAR" : db.AsignacionRequis.Where(a => a.RequisicionId == e.Id).Count().ToString(),
                nombreReclutado = String.Join(", ", nombreReclu.Where(b => b.RequisicionId == e.Id).Select(b => b.Nombre).ToList().ToArray())
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
                var obj = sol.Split(',');
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
        public IHttpActionResult Usuario()
        {
           
            int[] Status = new[] { 1, 2, 3, 5, 6 };

            //if (cor == "1")
            //{
            //    Status = new[] { 4 };
            //}

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
            var datos = db.Estatus.Where(e => e.Activo == true && e.TipoMovimiento == 2 && e.Id != 9 && e.Id != 5).Select(e => new
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
    }
}
