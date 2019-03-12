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
using System.Data.Entity;

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
         
            SAGA.BOL.Examenes E = new SAGA.BOL.Examenes();
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
                        else if(obj.Tipo == 1)
                        {
                            R.PreguntaId = idP;
                            R.Respuesta = "Es pregunta abierta";
                            R.Validacion = 0;

                            db.Respuestas.Add(R);
                            db.SaveChanges();

                            R = new Respuestas();
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
        public IHttpActionResult InsertRelacion(List<RequiExamen> relacion)
        {
            try
            {
                RequiExamen RE = new RequiExamen();

                foreach (var re in relacion)
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
        [HttpPost]
        [Route("actualizarResultado")]
        public IHttpActionResult ActualizarResultado(ExamenCandidato resultado)
        {
            try
            {
                var id = db.ExamenCandidato.Where(x => x.CandidatoId.Equals(resultado.CandidatoId) & x.RequisicionId.Equals(resultado.RequisicionId) & x.ExamenId.Equals(resultado.ExamenId)).Select(R => R.Id).FirstOrDefault();

                var candidato = db.ExamenCandidato.Find(id);
                db.Entry(candidato).State = EntityState.Modified;
                candidato.Resultado = resultado.Resultado;

                db.SaveChanges();
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
            try
            {
                var examenes = db.Preguntas.Where(x => x.ExamenId.Equals(examenId) && x.Activo.Equals(1)).Select(E => new
                {
                    nombre = E.Examen.Nombre,
                    preguntaId = E.Id,
                    pregunta = E.Pregunta,
                    respuestas = db.Respuestas.Where(x => x.PreguntaId.Equals(E.Id)).Select(R => new
                    {
                        respuestaId = R.Id,
                        resp = string.IsNullOrEmpty(R.Respuesta) ? "Pregunta Abierta" : R.Respuesta,
                        value = R.Validacion,
                    }).OrderBy(o => Guid.NewGuid()).ToList(),
                    tipo = E.Tipo,
                   
                }).ToList();

                return Ok(examenes);
            }
            catch(Exception ex)
            {
                return Ok();
            }
            
        }

        [HttpGet]
        [Route("getExamenRequi")]
        public IHttpActionResult GetExamenRequi(Guid requisicionId)
        {
            try
            {
                var examenId = db.RequiExamen.Where(x => x.RequisicionId.Equals(requisicionId)).Select(id => id.ExamenId).FirstOrDefault();

                var examenes = db.Preguntas.Where(x => x.ExamenId.Equals(examenId) && x.Activo.Equals(1)).Select(E => new
                {
                    nombre = E.Examen.Nombre,
                    preguntaId = E.Id,
                    pregunta = E.Pregunta,
                    respuestas = db.Respuestas.Where(x => x.PreguntaId.Equals(E.Id)).Select(R => new
                    {
                        resp = string.IsNullOrEmpty(R.Respuesta) ? "Pregunta Abierta" : R.Respuesta,
                        value = R.Validacion,
                    }).OrderBy(o => Guid.NewGuid()).ToList(),
                    tipo = E.Tipo,

                }).ToList();

                return Ok(examenes);
            }
            catch (Exception ex)
            {
                return Ok();
            }

        }
        [HttpGet]
        [Route("getRequiEstatus")]
        public IHttpActionResult GetRequiEstatus(int estatus)
        {
            try
            {
                List<int> estatusList = new List<int> { 8, 34, 35, 36, 36 };

                var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                    .Where(e => e.Activo.Equals(true) && !estatusList.Contains(e.EstatusId))
                    .Select(e => new
                    {
                        Id = e.Id,
                        Folio = e.Folio,
                        VBtra = e.VBtra,
                        Cliente = e.Cliente.Nombrecomercial,
                        Solicita = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault(),
                        fch_Creacion = e.fch_Creacion,
                        Estatus = e.Estatus.Descripcion,
                        EstatusId = e.EstatusId,
                        Examen = db.RequiExamen.Where(x => x.RequisicionId.Equals(e.Id)).Count() > 0 ? true : false,
                        Reclutador = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Clave + " " + s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault(),
                        postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) && p.StatusId.Equals(1)).Select(c => new {
                            candidatoId = c.CandidatoId,
                            curp = c.Candidato.CURP,
                            nombre = c.Candidato.Nombre, 
                            apellidoPaterno = c.Candidato.ApellidoPaterno,
                            apellidoMaterno = c.Candidato.ApellidoMaterno
                        }).ToList()
            }).ToList();

                return Ok(vacantes);

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }


        [HttpPost]
        [Route("insertRespCandidato")]
        public IHttpActionResult InsertRespCandidato(List<ResultadosCandidato> resultado)
        {
            try
            {
                ResultadosCandidato RC = new ResultadosCandidato();

                foreach(var rc in resultado)
                {
                    RC.CandidatoId = rc.CandidatoId;
                    RC.PreguntaId = rc.PreguntaId;
                    RC.RespuestaId = rc.RespuestaId;
                    RC.Value = rc.Value;

                    db.resultadocandidato.Add(RC);
                    db.SaveChanges();

                    RC = new ResultadosCandidato();
                }

                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        
        }

        [HttpGet]
        [Route("getCandidatos")]
        public IHttpActionResult GetCandidatos()
        {
            try
            {
                var resultados = db.RequiExamen.Select(R => new
                {
                    requisicionId = R.Id,
                    folio = R.Requisicion.Folio,
                    cliente = R.Requisicion.Cliente.Nombrecomercial,
                    vBtra = R.Requisicion.VBtra,
                    candidatos = db.ExamenCandidato.Where(x => x.RequisicionId.Equals(R.RequisicionId)).Select(C => new
                    {
                        C.CandidatoId,
                        C.RequisicionId,
                        curp = C.Candidato.CURP,
                        rfc = C.Candidato.RFC,
                        nombre = C.Candidato.Nombre + " " + C.Candidato.ApellidoPaterno + " " + C.Candidato.ApellidoMaterno,
                        usuario = db.Usuarios.Where(x => x.Id.Equals(C.Requisicion.PropietarioId)).Select(s => s.Clave + " " + s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault(),
                        fecha = C.fch_Creacion,
                        resultado = C.Resultado
                    }).ToList()
                });
                return Ok(resultados);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }


        [HttpGet]
        [Route("getExamenCandidato")]
        public IHttpActionResult GetExamenCandidato(Guid candidatoId)
        {
            var tecnicos = db.ExamenCandidato.Where(x => x.CandidatoId.Equals(candidatoId)).Select(R => new
            {
                requisicionId = R.Id,
                folio = R.Requisicion.Folio,
                cliente = R.Requisicion.Cliente.Nombrecomercial,
                vBtra = R.Requisicion.VBtra,
                candidatoId = R.CandidatoId,
                examen = R.Examen.Nombre,
                resultado = R.Resultado
            }).ToList();

            var psicometricos = db.PsicometriaCandidato.Where(x => x.CandidatoId.Equals(candidatoId)).Select(P => new
            {
                requisicionId = P.RequisicionId,
                folio = P.Requisicion.Folio,
                vBtra = P.Requisicion.VBtra,
                clave = P.RequiClave.Clave,
                resultado = P.Resultado

            }).ToList();

            var resultado = new List<object> { tecnicos.ToList(), psicometricos.ToList() };
          
            return Ok(resultado);
        }
        [HttpGet]
        [Route("getRespCandidatos")]
        public IHttpActionResult GetRespCandidatos(Guid CandidatoId, Guid RequisicionId)
        {
            var resultado = db.ExamenCandidato.Where(x => x.CandidatoId.Equals(CandidatoId) & x.RequisicionId.Equals(RequisicionId)).Select(E => new
            {

                ExamenId = E.ExamenId,
                Tipo = E.Examen.TipoExamen.Nombre,
                Nombre = E.Examen.Nombre,
                Resultado = E.Resultado,
                Respuestas = db.Preguntas.Where(x => x.ExamenId.Equals(E.ExamenId)).Select(R => new
                {
                    pregunta = R.Pregunta,
                    tipop = R.Tipo,
                    respuesta = db.resultadocandidato.Where(x => x.PreguntaId.Equals(R.Id) && x.CandidatoId.Equals(CandidatoId)).Select(res => res.Pregunta.Tipo == 1 ? res.Value : res.Respuesta.Respuesta).FirstOrDefault(),
                    value = db.resultadocandidato.Where(x => x.PreguntaId.Equals(R.Id) && x.CandidatoId.Equals(CandidatoId)).Select(res => res.Value).FirstOrDefault(),
                    respCorrecta = db.Respuestas.Where(x => x.PreguntaId.Equals(R.Id) && x.Validacion.Equals(1)).Select(rc => rc.Respuesta)

                    //{
                    //    id = new Guid(res.Value),
                    //    resp = db.Respuestas.Where(x => x.Id.Equals(new Guid(res.Value))).Select(rrr => rrr.Respuesta)

                    //})
                    //respuesta = db.resultadocandidato.Where(x => x.PreguntaId.Equals(R.Id)).Select(rr => new
                    //{
                    //    resp = db.Respuestas.Where(x => x.Id.Equals(rr.Value) && x.PreguntaId.Equals(R.Id)).Select(rrr => rrr.Respuesta),
                    //    value = db.Respuestas.Where(x => x.Id.Equals(rr.Value)).Select(rrr => rrr.Validacion)
                    //}).ToList()
                }).ToList()

            }).ToList();
            return Ok(resultado);
        }
    }
}
