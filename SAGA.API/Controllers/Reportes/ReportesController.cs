
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
            int EstatusID = Convert.ToInt32(stus);
            DateTime FechaF = DateTime.Now;
            DateTime FechaI = DateTime.Now;
            try
            {
                FechaI = Convert.ToDateTime("2019-02-06");
                if (fini != null)
                {
                    FechaI = Convert.ToDateTime(fini);
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

            if (stus != "0")
            {
                datos = datos.Where(e => e.EstatusId == EstatusID).ToList();
            }

            if (clave != null)
            {
                datos = datos.Where(e => e.VBtra.ToLower().Contains(clave.ToLower())).ToList();
            }
            if (sol != "0")
            {
                datos = datos.Where(e=>e.Usuario == sol).ToList();
            }
            if (emp != "0")
            {
                datos = datos.Where(e => e.ClienteId == new Guid(emp)).ToList();
            }

            if (trcl != "0")
            {
                datos = datos.Where(e => e.TipoReclutamientoId == Convert.ToInt32(trcl)).ToList();
            }
            if (cor != "0")
            {
                datos = datos.Where(e => e.ClaseReclutamientoId == Convert.ToInt32(cor)).ToList();
            }

            if (recl != "0")
            {
                Guid idAs = new Guid(recl);
                var asigna = db.AsignacionRequis.Where(e => e.GrpUsrId == idAs).Select(e => e.RequisicionId).ToList();
                datos = datos.Where(e => asigna.Contains(e.Id)).ToList();
            }

            if (ofc != "0")
            {
               
                var negocio = db.OficinasReclutamiento.Where(e => e.UnidadNegocioId == Convert.ToInt32(ofc)).Select(e=>e.Id).ToList();
                var lista = db.Usuarios.Where(e => negocio.Contains(e.SucursalId)).Select(e=>e.Usuario).ToList();
                datos.Where(e => lista.Contains(e.Usuario)).ToList();
            }



            return Ok(datos);
        }


        [HttpGet]
        [Route("empresas")]
        public IHttpActionResult Empresas()
        {
            var datos = db.Clientes.Where(e=>e.Activo == true && e.RazonSocial != "" && e.RazonSocial != null).Select(e => new
            {
                e.RFC,
                e.Nombrecomercial,
                e.Id,
                e.RazonSocial
            }).Distinct().OrderBy(x=>x.RazonSocial).ToList();
            return Ok(datos);
        }


        [HttpGet]
        [Route("usuario")]
        public IHttpActionResult Usuario()
        {
            int[] Status = new[] { 1, 2, 3, 4, 5, 6 };
           
            var datos = db.Usuarios.Where(e => e.Activo == true && Status.Contains(e.TipoUsuarioId)).Select(e => new
            {
                e.Nombre,
                e.Id,
                e.Usuario
            }).OrderBy(x=>x.Nombre).ToList();
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
