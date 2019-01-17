using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;

using SAGA.API.Dtos.Examenes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/examenes")]
    public class ExamenesController : ApiController
    {
        private SAGADBContext db;
        public ExamenesController()
        {
            db = new SAGADBContext();
        }

        [HttpPost]
        [Route("insertExamen")]
        public IHttpActionResult InsertExamen(List<ExamenDto> Objeto)
        {
            Examenes E = new Examenes();
            Preguntas P = new Preguntas();
            Respuestas R = new Respuestas();
         
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    E.TipoExamenId = Objeto[0].TipoExamen.Id;
                    E.Nombre = Objeto[0].TipoExamen.Nombre;
                    E.Descripcion = "SIN ASIGNAR";

                    db.Examenes.Add(E);
                    db.SaveChanges();

                    int idE = E.Id;

                    foreach (var obj in Objeto)
                    {
                        P.ExamenId = idE;
                        P.Pregunta = obj.Pregunta;
                        P.Tipo = obj.Tipo;
                        P.Activo = 1;

                        db.Preguntas.Add(P);
                        db.SaveChanges();

                        int idP = P.Id;
                   
                        if (obj.Tipo > 1)
                        {
                            foreach (RespuestaDto r in obj.Respuestas)
                            {
                                R.PreguntaId = idP;
                                R.Respuesta = r.resp;
                                R.Validacion = r.value;

                                db.Respuestas.Add(R);
                                db.SaveChanges();

                                R = new Respuestas();
                            }
                        }

                        P = new Preguntas();
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

        [HttpPost]
        [Route("insertRelacion")]
        public IHttpActionResult InsertRelacion(List<RequiExamen> requiexamen)
        {
            try
            {
                RequiExamen RE = new RequiExamen();

                foreach (RequiExamen re in requiexamen)
                {
                    RE.ExamenId = re.ExamenId;
                    RE.RequisicionId = re.RequisicionId;

                    db.RequiExamen.Add(RE);
                    db.SaveChanges();

                    RE = new RequiExamen();
                }

                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);

            }
        }

        [HttpGet]
        [Route("getCatalogo")]
        public IHttpActionResult CatalogoExamenes()
        {
            var examenes = db.TipoExamen.ToList();
            return Ok(examenes);
        }

        [HttpGet]
        [Route("getExamenes")]
        public IHttpActionResult GetExamenes(int tipoexamenId)
        {
            var examenes = db.Examenes.Where(x => x.TipoExamenId.Equals(tipoexamenId)).ToList();
            return Ok(examenes);
        }

        [HttpGet]
        [Route("getExamen")]
        public IHttpActionResult GetExamen(int examenId)
        {
            var examenes = db.Preguntas.Where(x => x.ExamenId.Equals(examenId) && x.Activo.Equals(1)).Select(E => new {
                pregunta = E.Pregunta,
                respuestas = db.Respuestas.Where(x => x.PreguntaId.Equals(E.Id)).Select(R => new
                {
                    resp = string.IsNullOrEmpty(R.Respuesta) ? "Pregunta Abierta" : R.Respuesta,
                    value = R.Validacion

                }), 
                tipo = E.Tipo
            }).ToList();

            return Ok(examenes);
        }
    }
}
