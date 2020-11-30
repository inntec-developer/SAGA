using System;
using System.Collections.Generic;
using System.Linq;
using SAGA.DAL;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using SAGA.BOL;
using System.IO;
using SAGA.API.Utilerias;
using SAGA.API.Dtos.SistFirmas;
namespace SAGA.API.Controllers.SistFirmas
{
    [RoutePrefix("api/Firmas")]
    public class ConfigBitacoraController : ApiController
    {
        private SAGADBContext db;
        public ConfigBitacoraController()
        {
            db = new SAGADBContext();
        }
        [HttpPost]
        [Route("addBitacora")]
        [Authorize]
        public IHttpActionResult AddBitacora(BitacoraDto datos)
        {
            try
            {
                FIRM_ConfigBitacora bit = new FIRM_ConfigBitacora();

                bit = datos.ConfigBitacora;
                bit.fch_Creacion = DateTime.Now;
                bit.fch_Modificacion = DateTime.Now;

                db.FIRM_ConfigBitacora.Add(bit);
                db.SaveChanges();

                foreach (FIRM_FechasEstatus fe in datos.FechasEstatus)
                {
                    fe.ConfigBitacoraId = bit.Id;
                }


                db.FIRM_FechasEstatus.AddRange(datos.FechasEstatus);

                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }
        [HttpGet]
        [Route("getEstatusBitacora")]
        [Authorize]
        public IHttpActionResult GetEstatusBitacora()
        {
            try
            {
                var estatus = db.FIRM_EstatusBitacora.Where(x => x.Activo).Select(e => new
                {
                    e.Id,
                    e.Estatus
                });

                return Ok(estatus);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }
        [HttpGet]
        [Route("getEmpresas")]
        [Authorize]
        public IHttpActionResult GetEmpresas()
        {
            try
            {
                var clientes = db.Sucursales.Where(x => x.Activo).Select(c => new
                {
                    id = c.Id,
                    clave = c.Clave,
                    sucursal = c.Nombre,
                    empresa = c.Empresas.Nombre,
                    claveemp = c.Empresas.Clave,
                    registro_pat = c.RegistroPatronal.RP_Clave,
                    registro_imss = c.RegistroPatronal.RP_IMSS
                }).ToList();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }

        }
        [HttpGet]
        [Route("getSoporteNominas")]
        [Authorize]
        public IHttpActionResult GetSoporteNominas()
        {
            try
            {
                var soportes = db.FIRM_SoportesNomina.Where(x => x.Activo).Select(c => new
                {
                    c.Id,
                    c.Soporte
                }).ToList();
                return Ok(soportes);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }

        }
        [HttpGet]
        [Route("getTiposNomina")]
        [Authorize]
        public IHttpActionResult GetTiposNomina()
        {
            try
            {
                var tipos = db.TiposNominas.Where(x => x.Tipo.Equals(2)).Select(c => new
                {
                    c.Id,
                    c.Clave,
                    c.tipoDeNomina
                }).ToList();
                return Ok(tipos);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }

        }
    }
}
