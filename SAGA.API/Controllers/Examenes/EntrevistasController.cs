using SAGA.BOL;
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
                    ale = db.ConfigEntrevistas.Where(x => x.examenId.Equals(E.Id)).Select(a => a.numPreguntas).FirstOrDefault(),
                    num = db.Preguntas.Where(x => x.ExamenId.Equals(E.Id)).Count(),
                    usuario = String.IsNullOrEmpty(db.ConfigEntrevistas.Where(x => x.examenId.Equals(E.Id)).Select(u => u.Usuario.Usuario).FirstOrDefault()) ? "SIN REGISTRO" : db.ConfigEntrevistas.Where(x => x.examenId.Equals(E.Id)).Select(u => u.Usuario.Usuario).FirstOrDefault()
                }).ToList();

                return Ok(examenes);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }
        [HttpPost]
        [Route("updateAlea")]
        public IHttpActionResult updateAlea(ConfigEntrevista resultado)
        {
            try
            {

                var idx = db.ConfigEntrevistas.Where(x => x.examenId.Equals(resultado.examenId)).Select(i => i.Id).FirstOrDefault();
                if(idx != 0)
                {
                    var id = db.ConfigEntrevistas.Find(idx);
                    db.Entry(id).State = System.Data.Entity.EntityState.Modified;

                    id.numPreguntas = resultado.numPreguntas;
                    id.usuarioId = resultado.usuarioId;
                    id.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();

                }
                else
                {
                    
                    db.ConfigEntrevistas.Add(resultado);
                    db.SaveChanges();
                }
                
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getEntrevista")]
        public IHttpActionResult GetEntrevista()
        {
            try
            {
                var examenesId = db.Examenes.Where(x => x.TipoExamenId.Equals(6)).Select(e => e.Id);
                var preguntas = db.Preguntas.Where(x => examenesId.Contains(x.ExamenId) && x.Activo.Equals(1)).GroupBy(g => g.ExamenId)
                    .Select(E => new
                    {
                        examenId = E.Key,
                        alea = db.ConfigEntrevistas.Where(x => x.examenId.Equals(E.Key)).Select(n => n.numPreguntas).FirstOrDefault(),
                        preguntas = E.Select(p => new
                        {
                            preguntaId = p.Id,
                            pregunta = p.Pregunta,
                        }).OrderBy(o => Guid.NewGuid())
                   
                     }).ToList();

                var entrevista = preguntas.Select(m => new
                {
                    preguntas = m.preguntas.Take(m.alea),
                    examenId = m.examenId
                });
                return Ok(entrevista);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }
    }
}
