using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Examenes
{
    [RoutePrefix("api/examenes")]
    public class EntrevistasController : ApiController
    {
        private SAGADBContext db;
        public EntrevistasController()
        {
            db = new SAGADBContext();
        }
        [HttpGet]
        [Route("getExamenesEntrevista")]
        public IHttpActionResult GetExamenEntrevistas()
        {
            try
            {
                var examenes = db.Examenes.Where(x => (x.TipoExamenId.Equals(6) || x.TipoExamenId.Equals(7))).Select(E => new
                {
                    examenId = E.Id,
                    nombre = E.Nombre,
                    preguntas = db.Preguntas.Where(x => x.ExamenId.Equals(E.Id)).Select(r => new
                    {
                        pregunta = r.Pregunta
                    }),                    
                    tipo = E.TipoExamen.Nombre,
                    fch_Modificacion = DateTime.Now,
                    ale = db.Preguntas.Where(x => x.ExamenId.Equals(E.Id)).Count(),
                    num = db.Preguntas.Where(x => x.ExamenId.Equals(E.Id)).Count(),
                    usuario = "INNTECC"
                }).ToList();

                return Ok(examenes);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }
    }
}
