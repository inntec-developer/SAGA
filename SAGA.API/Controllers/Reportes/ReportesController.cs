
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

            var datos = db.Requisiciones.Where(e=>e.fch_Creacion >= FechaI 
            && e.fch_Creacion <= FechaF).Select(e => new
            {
                e.Id,
                e.Folio,
                e.VBtra,
                porcentaje = e.horariosRequi.Sum(s => s.numeroVacantes) > 0 ? (db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count()) * 100 / e.horariosRequi.Sum(s => s.numeroVacantes) : 0,
                e.fch_Creacion,
                e.fch_Limite,
                empresa = e.Cliente.Nombrecomercial,
                e.ClienteId,
                propietario = db.Usuarios.Where(x => x.Usuario == e.Propietario).FirstOrDefault().Nombre,
                Usuario = db.Usuarios.Where(x => x.Usuario == e.Propietario).FirstOrDefault().Usuario,
                numero = e.horariosRequi.Where(x=>x.Activo == true).Count(),
                e.EstatusId,
                e.TipoReclutamientoId,
                e.ClaseReclutamientoId,
                estatus = e.Estatus.Descripcion,
                fch_Modificacion = db.EstatusRequisiciones.Where(x=>x.RequisicionId == e.Id).FirstOrDefault().fch_Modificacion
              
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

            if (trcl != "0")
            {
                datos = datos.Where(e => e.TipoReclutamientoId == Convert.ToInt32(trcl)).ToList();
            }
            if (cor != "0")
            {
                datos = datos.Where(e => e.ClaseReclutamientoId == Convert.ToInt32(cor)).ToList();
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
                    //int unidad = Convert.ToInt32(ofc);
                    var negocio = db.OficinasReclutamiento.Where(e => listaAreglo.Contains(e.UnidadNegocioId)).Select(e => e.Id).ToList();
                    var lista = db.Usuarios.Where(e => negocio.Contains(e.SucursalId)).Select(e => e.Usuario).ToList();
                    datos.Where(e => lista.Contains(e.Usuario)).ToList();
                }
            }



            return Ok(datos);
        }


        [HttpGet]
        [Route("empresas")]
        public IHttpActionResult Empresas()
        {
            var datos = db.Clientes.Where(e=>e.Activo == true && e.esCliente.Equals(true)).Select(e => new
            {
                e.RFC,
                e.Nombrecomercial,
                e.Id,
                e.RazonSocial
            }).Distinct().OrderBy(x=>x.RazonSocial).ToList();

            datos.Insert(0, new { RFC = "Todos los Usuarios", Nombrecomercial = "as", Id = new Guid("00000000-0000-0000-0000-000000000000"), RazonSocial = "Todas las empresas" });
            return Ok(datos);
        }


        [HttpGet]
        [Route("usuario")]
        public IHttpActionResult Usuario()
        {
            int[] Status = new[] { 1, 2, 3, 4, 5, 6 };
           
            var datos = db.Usuarios.Where(e => e.Activo == true && Status.Contains(e.TipoUsuarioId)).Select(e => new
            {
                Nombre = e.Nombre + " "+ e.ApellidoPaterno,
                e.Id,
                e.Usuario
            }).OrderBy(x=>x.Nombre).ToList();
            datos.Insert(0, new { Nombre = "Todos los Usuarios", Id = new Guid("00000000-0000-0000-0000-000000000000") , Usuario = "0"});
            return Ok(datos);
        }


        [HttpGet]
        [Route("estatus")]
        public IHttpActionResult Estatus()
        {
            var datos = db.Estatus.Where(e => e.Activo == true && e.TipoMovimiento == 2).Select(e => new
            {
                e.Descripcion,
                e.Id
            }).ToList();
           
           
           datos.Insert(0 ,new { Descripcion = "Todos los estatus", Id = 0 });
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
