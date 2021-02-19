using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Ventas.DirectorioEmpresarial
{
    [RoutePrefix("api/Directorio")]
    public class DirectorioEmpresarialController : ApiController
    {
        private SAGADBContext db;

        public DirectorioEmpresarialController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getProspectos")]
        
        public IHttpActionResult GetProspectos()
        {
            try
            {
                var prospecto = db.Clientes
                    .Where(c => c.esCliente.Equals(false) && c.Activo.Equals(true))
                    .OrderByDescending(p => p.fch_Creacion)
                    .Select(x => new
                    {
                        x.Id,
                        x.Clave,
                        x.Nombrecomercial,
                        x.GiroEmpresas.giroEmpresa,
                        x.ActividadEmpresas.actividadEmpresa,
                        x.TamanoEmpresas.tamanoEmpresa,
                        x.Clasificacion,
                        x.TipoEmpresas.tipoEmpresa,
                        x.NumeroEmpleados

                    })
                    .ToList();
                return Ok(prospecto);
            }
            catch(Exception ex) { string msg = ex.Message; return Ok(HttpStatusCode.NotFound); }
        }

        [HttpGet]
        [Route("getClientes")]
        
        public IHttpActionResult GetClientes()
        {
            try
            {
                var prospecto = db.Clientes
                    .Where(c => c.esCliente.Equals(true) && c.Activo.Equals(true))
                    .OrderByDescending( c => c.fch_Creacion)
                    .Select(x => new {
                        x.Id,
                        x.RazonSocial,
                        x.Clave,
                        x.Nombrecomercial,
                        x.RFC,
                        x.GiroEmpresas.giroEmpresa,
                        x.ActividadEmpresas.actividadEmpresa,
                        x.TamanoEmpresas.tamanoEmpresa,
                        x.Clasificacion,
                        x.TipoEmpresas.tipoEmpresa,
                        x.NumeroEmpleados
                    })
                    .ToList();
                return Ok(prospecto);
            }
            catch (Exception ex) { string msg = ex.Message; return Ok(HttpStatusCode.NotFound); }
        }

    }
}
