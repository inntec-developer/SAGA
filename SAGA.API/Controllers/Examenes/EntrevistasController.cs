using SAGA.API.Dtos.Examenes;
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

        [HttpPost]
        [Route("insertEntrevista")]
        public IHttpActionResult InsertExamen(EntrevistaDto Objeto)
        {
            SAGA.BOL.Entrevista E = new SAGA.BOL.Entrevista();
            ConfigEntrevista CE = new ConfigEntrevista();
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    E.Nombre = Objeto.Nombre;
                    E.Descripcion = Objeto.Descripcion;
                    E.UsuarioId = Objeto.usuarioId;
                    E.fch_Modificacion = DateTime.Now;
                    E.Activo = true;
                    db.Entrevistas.Add(E);
                    db.SaveChanges();

                    int idE = E.Id;

                    foreach (var obj in Objeto.Preguntas)
                    {
                        CE.EntrevistaId = idE;
                        CE.PreguntaId = obj.preguntaId;
                        CE.Orden = obj.Orden;
                        CE.Activo = true;

                        db.ConfigEntrevistas.Add(CE);
                        db.SaveChanges();
                    }
                    dbContextTransaction.Commit();

                    return Ok(HttpStatusCode.OK);

                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Ok(HttpStatusCode.ExpectationFailed);
                }

            }
        }

        [HttpGet]
        [Route("getExamenesEntrevista")]
        public IHttpActionResult GetExamenEntrevistas()
        {
            try
            {
                var entrevistas = db.Entrevistas.Where(x => x.Activo).Select(E => new
                {
                    entrevistaId = E.Id,
                    nombre = E.Nombre,
                    descripcion = E.Descripcion,
                    preguntas = db.ConfigEntrevistas.Where(x => x.EntrevistaId.Equals(E.Id)).Select(p => new
                    {
                        preguntaId = p.PreguntaId,
                        pregunta = p.Pregunta.Pregunta,
                        orden = p.Orden,
                        respuestas = db.Respuestas.Where(x => x.PreguntaId.Equals(p.PreguntaId)).Select(r => new
                        {
                            respuestaId = r.Id,
                            respuesta = r.Respuesta,
                            value = r.Validacion
                        })
                    }),
                    fch_Creacion = E.fch_Creacion,
                    usuario = E.Usuario.Nombre + " " + E.Usuario.ApellidoPaterno + " " + E.Usuario.ApellidoMaterno,
                    num = db.ConfigEntrevistas.Where(x => x.EntrevistaId.Equals(E.Id)).Select(p => p.PreguntaId).Count()
                });

                return Ok(entrevistas);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }
        //[HttpPost]
        //[Route("updateAlea")]
        //public IHttpActionResult updateAlea(ConfigEntrevista resultado)
        //{
        //    try
        //    {

        //        var idx = db.ConfigEntrevistas.Where(x => x.examenId.Equals(resultado.examenId)).Select(i => i.Id).FirstOrDefault();
        //        if(idx != 0)
        //        {
        //            var id = db.ConfigEntrevistas.Find(idx);
        //            db.Entry(id).State = System.Data.Entity.EntityState.Modified;

        //            id.numPreguntas = resultado.numPreguntas;
        //            id.usuarioId = resultado.usuarioId;
        //            id.fch_Modificacion = DateTime.Now;

        //            db.SaveChanges();

        //        }
        //        else
        //        {
                    
        //            db.ConfigEntrevistas.Add(resultado);
        //            db.SaveChanges();
        //        }
                
        //        return Ok(HttpStatusCode.OK);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(HttpStatusCode.ExpectationFailed);
        //    }

        //}

        //[HttpGet]
        //[Route("getEntrevista")]
        //public IHttpActionResult GetEntrevista()
        //{
        //    try
        //    {
        //        var examenesId = db.Examenes.Where(x => x.TipoExamenId.Equals(6)).Select(e => e.Id);
        //        var preguntas = db.Preguntas.Where(x => examenesId.Contains(x.ExamenId) && x.Activo.Equals(1)).GroupBy(g => g.ExamenId)
        //            .Select(E => new
        //            {
        //                examenId = E.Key,
        //                alea = db.ConfigEntrevistas.Where(x => x.examenId.Equals(E.Key)).Select(n => n.numPreguntas).FirstOrDefault(),
        //                preguntas = E.Select(p => new
        //                {
        //                    preguntaId = p.Id,
        //                    pregunta = p.Pregunta,
        //                }).OrderBy(o => Guid.NewGuid())
                   
        //             }).ToList();

        //        var entrevista = preguntas.Select(m => new
        //        {
        //            preguntas = m.preguntas.Take(m.alea),
        //            examenId = m.examenId
        //        });
        //        return Ok(entrevista);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(HttpStatusCode.ExpectationFailed);
        //    }

        //}
    }
}
