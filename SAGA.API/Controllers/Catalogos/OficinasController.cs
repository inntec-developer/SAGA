using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.BOL;
using SAGA.DAL;

namespace SAGA.API.Controllers.Catalogos
{
    [RoutePrefix("api/Oficina")]
    public class OficinasController : ApiController
    {
        private SAGADBContext db = new SAGADBContext();

        [HttpGet]
        [Route("oficina")]
        public IHttpActionResult oficina()
        {
            try
            {
                string cadena = "1,2,3,4,5";
                var array = cadena.Split(',');
                List<int> tipos = new List<int>();
                for (int i = 0; i < array.Length; i++)
                {
                    tipos.Add(Int32.Parse(array[i]));
                }

                var datos = db.OficinasReclutamiento.Where(e => tipos.Contains(e.TipoOficinaId)).Select(
                     caja => new
                     {
                        caja.Id,
                        nombre = db.Entidad.Where(e => e.Id == caja.Id).Select(e => e.Nombre).FirstOrDefault(),
                        caja.Latitud,
                        caja.Longitud,
                        caja.TipoOficina.tipoOficina,
                        Estado = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().Estado.estado,
                        Municipio = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().Municipio.municipio,
                        Colonia = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().Colonia.colonia,
                        Calle = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().Calle,
                        NumeroExt = db.Direcciones.Where(e => e.EntidadId == caja.Id).FirstOrDefault().NumeroExterior,
                        Telefono = db.Telefonos.Where(e => e.EntidadId == caja.Id && e.esPrincipal == true).FirstOrDefault().telefono,
                        Extension = db.Telefonos.Where(e => e.EntidadId == caja.Id && e.esPrincipal == true).FirstOrDefault().Extension,
                        Correo = db.Emails.Where(e => e.EntidadId == caja.Id && e.esPrincipal == true).FirstOrDefault().email,
                     }).OrderBy(e => e.nombre).ToList();

              
                return Ok(datos);
            }
            catch (Exception ex)
            {
                var mensaje = ex;
            }

            return Ok("");
        }

        [HttpGet]
        [Route("add")]
        public IHttpActionResult agregar(string pregunta, string repuesta)
        {
            try
            {
                var datos = db.PreguntasFrecuentes.ToList();
                var pren = new PreguntasFrecuente();
                //     pren.Id = datos.Count + 1;
                pren.Pregunta = pregunta;
                pren.Respuesta = repuesta;
                pren.Activo = true;
                db.PreguntasFrecuentes.Add(pren);
                db.SaveChanges();

                return Ok("Éxito al agregar la pregunta");
            }
            catch (Exception ex)
            {
                var mensaje = ex;
            }
            return Ok("Algo paso, intentelo mas tarde");
        }

        [HttpGet]
        [Route("alter")]
        public IHttpActionResult editar(string id, string pregunta, string repuesta, string activo)
        {

            try
            {
                int iden = Convert.ToInt32(id);
                var datos = db.PreguntasFrecuentes.Where(e => e.Id == iden).FirstOrDefault();
                datos.Pregunta = pregunta;
                datos.Respuesta = repuesta;
                datos.Activo = Convert.ToBoolean(activo);
                db.SaveChanges();

                return Ok("Éxito al guardar cambios");
            }
            catch (Exception ex)
            {
                var mensaje = ex;
            }
            return Ok("Algo paso, intentelo mas tarde");
        }

        [HttpGet]
        [Route("delete")]
        public IHttpActionResult eliminar(string id)
        {
            try
            {
                int iden = Convert.ToInt32(id);
                var datos = db.PreguntasFrecuentes.Where(e => e.Id == iden).FirstOrDefault();
                db.PreguntasFrecuentes.Remove(datos);
                db.SaveChanges();
                return Ok("éxito al eliminar la pregunta");
            }
            catch (Exception ex)
            {
                var mensaje = ex;
            }
            return Ok("Algo paso intentelo mas tarde");
        }



    }
}
