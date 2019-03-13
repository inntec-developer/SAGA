using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/PreguntasFrecuente")]
    public class PreguntasFrecuenteController : ApiController
    {
        private SAGADBContext db = new SAGADBContext();
        

        [HttpGet]
        [Route("preguntas")]
        public IHttpActionResult preguntas()
        {
            try
            {
                var datos = db.PreguntasFrecuentes.OrderByDescending(e=>e.Id).ToList();
              //  var datos = db.PreguntasFrecuentes.Select(e => new { e.Id, e.Pregunta, e.Respuesta, e.Activo }).ToList();
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
                var datos = db.PreguntasFrecuentes.Where(e=>e.Id == iden).FirstOrDefault();
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