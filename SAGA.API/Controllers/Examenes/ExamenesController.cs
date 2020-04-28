using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using System.IO;
using SAGA.API.Dtos.Examenes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.Configuration;

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

        public void GuardarImagen(string nombre , string file, string type)
        {
            string x = file.Replace("data:" + type + ";base64,", "");
            byte[] imageBytes = Convert.FromBase64String(x);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);

            string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/Examenes/" + nombre);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            image.Save(fullPath);
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
                List<string> rutas = new List<string>();
                try
                {
                    E.TipoExamenId = Objeto[0].TipoExamen.Id;
                    E.Nombre = Objeto[0].TipoExamen.Nombre;
                    E.Descripcion = Objeto[0].Descripcion;
                    E.UsuarioId = Objeto[0].usuarioId;
                    E.fch_Modificacion = DateTime.Now;
                    
                    db.Examenes.Add(E);
                    db.SaveChanges();

                    int idE = E.Id;

                    foreach (var obj in Objeto)
                    {
                        P.ExamenId = idE;
                        P.Pregunta = obj.Pregunta.Pregunta;
                        P.Tipo = obj.Pregunta.Tipo;
                        P.Activo = 1;

                        db.Preguntas.Add(P);
                        db.SaveChanges();

                        int idP = P.Id;

                        if(obj.Pregunta.file != "")
                        {
                            var nom = "Preguntas/_" + idP.ToString() + "_" + obj.Pregunta.name;
                            this.GuardarImagen(nom, obj.Pregunta.file, obj.Pregunta.type);
                          
                        }
                        if(obj.Pregunta.Tipo >= 2)
                        {
                            foreach (RespuestaDto r in obj.Respuestas)
                            {
                                R.PreguntaId = idP;
                                R.Respuesta = r.resp;
                                R.Validacion = r.value;

                                db.Respuestas.Add(R);
                                db.SaveChanges();

                                if (r.file != "")
                                {
                                    var nom = "Respuestas/_" + R.Id.ToString() + "_" + r.name;
                                    this.GuardarImagen(nom, r.file, r.type);
                                }

                                R = new Respuestas();
                            }

                        }
                        else if (obj.Pregunta.Tipo == 1)
                        {
                            R.PreguntaId = idP;
                            R.Respuesta = "Es pregunta abierta";
                            R.Validacion = 0;

                            db.Respuestas.Add(R);
                            db.SaveChanges();

                            //if (obj.Pregunta.file != "")
                            //{
                            //    var nom = "Preguntas/_" + idP.ToString() + "_" + obj.Pregunta.name;
                            //    this.GuardarImagen(nom, obj.Pregunta.file, obj.Pregunta.type);

                            //}

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
            catch (Exception ex)
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
            catch (Exception ex)
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
        public IHttpActionResult GetExamenes()
        {
            var examenes = db.Examenes.Select(e => new
            {
                id = e.Id,
                tipo = e.TipoExamen.Nombre,
                examen = e.Nombre,
                descripcion = e.Descripcion,
                fch_Creacion = DateTime.Now,
                usuario = "INNTEC",
                numPreguntas = db.Preguntas.Where(x => x.ExamenId.Equals(e.Id)).Count()
            }).OrderBy(o => o.tipo);
            return Ok(examenes);
        }
        [HttpGet]
        [Route("getExamenes")]
        public IHttpActionResult GetExamenes(int tipoexamenId)
        {
            try
            {
                var examenes = db.Examenes.Where(x => x.TipoExamenId.Equals(tipoexamenId)).ToList();
                return Ok(examenes);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        public string GetImage(string ruta, string nom)
        {
            string fullPath;


            try
            {
                fullPath = System.Web.Hosting.HostingEnvironment.MapPath(ruta);
   
                string[] fileEntries = Directory.GetFiles(fullPath, nom + "*.*");

                var type = Path.GetExtension(fileEntries[0]);
                var fileName = Path.GetFileName(fileEntries[0]);

                FileStream fs = new FileStream(fileEntries[0], FileMode.Open, FileAccess.Read);
                    byte[] bimage = new byte[fs.Length];
                    fs.Read(bimage, 0, Convert.ToInt32(fs.Length));
                    fs.Close();

                string img = "data:" + type + ";base64," + Convert.ToBase64String(bimage);
                return ruta + fileName;
            }
            catch(Exception ex)
            {
                return "";
            }

        }

        [HttpGet]
        [Route("getExamen")]
        public IHttpActionResult GetExamen(int examenId)
        {
            try
            {
                var examenes = db.Preguntas.Where(x => x.ExamenId.Equals(examenId) && x.Activo.Equals(1)).Select(E => new
                {
                    examenId = E.ExamenId,
                    nombre = E.Examen.Nombre,
                    preguntaId = E.Id,
                    pregunta = E.Pregunta,
                    respuestas = db.Respuestas.Where(x => x.PreguntaId.Equals(E.Id)).Select(R => new
                    {
                        respuestaId = R.Id,
                        resp = string.IsNullOrEmpty(R.Respuesta) ? "Pregunta Abierta" : R.Respuesta,
                        value = R.Validacion,

                    }).OrderByDescending(o => o.value).ToList(),
                    tipo = E.Tipo,

                }).ToList();

                var mocos = examenes.Select(E => new
                {
                    examenId = E.examenId,
                    nombre = E.nombre,
                    preguntaId = E.preguntaId,
                    pregunta = E.pregunta,
                    file = GetImage("/utilerias/img/Examenes/Preguntas/", "_" + E.preguntaId + "_"),
                    respuestas = E.respuestas.Select(r => new {
                        respuestaId = r.respuestaId,
                        resp = r.resp,
                        value = r.value,
                        file = GetImage("/utilerias/img/Examenes/Respuestas/", "_" + r.respuestaId + "_"),
                    }),
                    tipo = E.tipo,
                });

                return Ok(mocos);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
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
                    examenId = E.ExamenId,
                    vBtra = db.Requisiciones.Where(x => x.Id.Equals(requisicionId)).Select(R => R.VBtra).FirstOrDefault(),
                    folio = db.Requisiciones.Where(x => x.Id.Equals(requisicionId)).Select(R => R.Folio).FirstOrDefault(),
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


                var mocos = examenes.Select(E => new
                {
                    examenId = E.examenId,
                    vBtra = E.vBtra,
                    folio = E.folio,
                    nombre = E.nombre,
                    preguntaId = E.preguntaId,
                    pregunta = E.pregunta,
                    file = GetImage("/utilerias/img/Examenes/Preguntas/", "_" + E.preguntaId + "_"),
                    respuestas = E.respuestas,
                    tipo = E.tipo,
                });

                return Ok(mocos);
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

                foreach (var rc in resultado)
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
            catch (Exception ex)
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
                var resultados = db.RequiExamen.OrderByDescending(x => x.Requisicion.Folio).Select(R => new
                {
                    requisicionId = R.Id,
                    folio = R.Requisicion.Folio,
                    cliente = R.Requisicion.Cliente.Nombrecomercial,
                    vBtra = R.Requisicion.VBtra,
                    candidatos = db.ExamenCandidato.OrderByDescending(o => o.fch_Modificacion).Where(x => x.RequisicionId.Equals(R.RequisicionId)).Select(C => new
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
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("examenesMedicos")]
        public IHttpActionResult ExamenesMedicos()
        {
            try
            {
                List<Guid> clientes = new List<Guid>();
                clientes.Add(new Guid("2BAEA5F0-7B54-E811-80E0-9E274155325E"));
                clientes.Add(new Guid("2DAEA5F0-7B54-E811-80E0-9E274155325E"));
                clientes.Add(new Guid("6D9898FC-7B54-E811-80E0-9E274155325E"));
                clientes.Add(new Guid("6F9898FC-7B54-E811-80E0-9E274155325E"));
                clientes.Add(new Guid("9ACB8014-7C54-E811-80E0-9E274155325E"));
                clientes.Add(new Guid("DB654F3E-7C54-E811-80E0-9E274155325E"));

                var candidatosFac = db.MedicosCandidato.Select(c => c.CandidatoId).ToList();
                var damfo = db.CostosDamfo290.Where(x => x.TipoCostos.Costos.Descripcion.Equals("MEDICOS")).Select(r => r.DAMFO290Id).ToList();
                var requis = db.Requisiciones.OrderBy(o => o.fch_Creacion).Where(x => damfo.Contains(x.DAMFO290Id) && x.Activo).GroupBy(g => g.ClienteId).Select(R => new {
                    clienteId = R.Key,
                    cliente = db.Clientes.Where(x => x.Id.Equals(R.Key)).Select(C => C.Nombrecomercial).FirstOrDefault(),
                    razon = db.Clientes.Where(x => x.Id.Equals(R.Key)).Select(C => C.RazonSocial).FirstOrDefault(),
                    examenes = R.Select(e => 
                        db.CostosDamfo290.Where(x => x.DAMFO290Id.Equals(e.DAMFO290Id)).Select(ee => new
                        {
                            id = ee.Id,
                            examen = ee.TipoCostos.Descripcion,
                            costo = ee.Costo
                        })
                    ).ToList(),
                    candidatos = R.Where(x => db.ProcesoCandidatos.Where(xx => xx.RequisicionId.Equals(x.Id) && !candidatosFac.Contains(xx.CandidatoId)).Count() > 0).GroupBy(g => g.Id).Select(r => new {
                        requisicionId = r.Key,
                        folio = r.Select( f => f.Folio).FirstOrDefault(),
                        vBtra = r.Select( v => v.VBtra).FirstOrDefault(),
                        candidatos = db.ProcesoCandidatos.Where(x => x.RequisicionId.Equals(r.Key) && !candidatosFac.Contains(x.CandidatoId)).Select(CC => new
                        {
                            candidatoId = CC.CandidatoId,
                            nombre = db.Candidatos.Where(x => x.Id.Equals(CC.CandidatoId)).Select(N => N.Nombre + " " + N.ApellidoPaterno + " " + N.ApellidoMaterno).FirstOrDefault(),
                        }).OrderBy(o => o.nombre)
                    } ).ToList()

                }).ToList();

                //var resultado = db.ProcesoCandidatos.Where(x => requis.Contains(x.RequisicionId)).GroupBy(g => g.RequisicionId)
                //    .Select(C => new
                //    {
                //        clienteId = db.Requisiciones.Where(x => x.Id.Equals(C.Key)).Select(cc => cc.ClienteId),
                //        cliente = db.Clientes.Where(x => x.Id.Equals(db.Requisiciones.Where(x => x.Id.Equals(C.Key)).Select(cc => cc.ClienteId))).Select(n => n.Nombrecomercial).FirstOrDefault(),
                //        razonsocial = db.Clientes.Where(x => x.Id.Equals(C.Key)).Select(n => n.RazonSocial).FirstOrDefault(),
                //        candidatos = C.Select(cc => db.Candidatos.Where(x => x.Id.Equals(cc.CandidatoId)).Select(nc => nc.Nombre + " " + nc.ApellidoPaterno + " " + nc.ApellidoMaterno).FirstOrDefault())
                //    });

                return Ok(requis);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpPost]
        [Route("insertResultMedico")]
        public IHttpActionResult InsertResultMedico(List<MedicoCandidato> resultado)
        {
            try
            {
                MedicoCandidato RC = new MedicoCandidato();
                List<Guid> requis = new List<Guid>();
                TipoExamenDto aux = new TipoExamenDto();
                foreach (var rc in resultado)
                {
                    RC.CandidatoId = rc.CandidatoId;
                    RC.RequisicionId = rc.RequisicionId;
                    RC.Resultado = rc.Resultado;
                    RC.Facturado = rc.Facturado;
                    RC.fch_Modificacion = DateTime.Now;

                    db.MedicosCandidato.Add(RC);
                    //   db.SaveChanges();
                    requis.Add(rc.RequisicionId);
                    RC = new MedicoCandidato();
                }
         //      List<Guid> damfos = db.Requisiciones.Where(x => requis.Distinct().Contains(x.Id)).Select(d => d.DAMFO290Id).Distinct().ToList();

           
                this.EnviarEmailMedicos(requis, resultado.Count());
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getExamenCandidato")]
        public IHttpActionResult GetExamenCandidato(Guid candidatoId)
        {
            var tecnicos = db.ExamenCandidato.OrderByDescending(o => o.fch_Modificacion).Where(x => x.CandidatoId.Equals(candidatoId)).Select(R => new
            {
                requisicionId = R.RequisicionId,
                folio = R.Requisicion.Folio,
                cliente = R.Requisicion.Cliente.Nombrecomercial,
                vBtra = R.Requisicion.VBtra,
                candidatoId = R.CandidatoId,
                examen = R.Examen.Nombre,
                resultado = R.Resultado
            }).ToList();

            var psicometricos = db.PsicometriaCandidato.OrderByDescending(o => o.fch_Resultado).Where(x => x.CandidatoId.Equals(candidatoId)).Select(P => new
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

        public IHttpActionResult EnviarEmailMedicos(List<Guid> requis, int candidatos)
        {
            try
            {
                Guid requi = requis[0];

                var requisiciones = db.Requisiciones.Where(x => x.Id.Equals(requi) && x.Activo).Select(R => new {
                    clienteId = R.ClienteId,
                    cliente = R.Cliente.Nombrecomercial,
                    razon = R.Cliente.RazonSocial,
                    estado = R.Cliente.direcciones.Select(x => x.Estado.estado).FirstOrDefault(),
                    examenes = db.CostosDamfo290.Where(x => x.DAMFO290Id.Equals(R.DAMFO290Id)).Select(e => new {
                        examen = e.TipoCostos.Descripcion,
                        costo = e.Costo
                    }).ToList()
                }).FirstOrDefault();
      
                string email = "bmorales@damsa.com.mx";
                string body = "";
                // email = "idelatorre@damsa.com.mx";
                if (email != "")
                {
                    string from = "noreply@damsa.com.mx";
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "SAGA Inn");
                    m.Subject = "Solicitud de Facturación REACTIVOS MEDICOS";

                    m.To.Add(email);

                    body = string.Format("<p>Por este medio se les informa que se requiere facturar #Reactivos M&eacute;dicos para el cliente {0}</p>", requisiciones.cliente);
                        body = body + string.Format("<br/><p>Raz&oacute;n Social: {0}</p>", requisiciones.razon);
                        body = body + string.Format("<p>Estado: {0}</p><br/>", requisiciones.estado);
                        foreach (var rr in requisiciones.examenes)
                        {
                            body = body + string.Format("<p>Tipo Examen: {0}</p>", rr.examen);
                            body = body + string.Format("<p>Costo Reactivo: ${0} </p>", rr.costo);

                        }
                    body = body + string.Format("<p>N&uacute;mero Reactivos: {0}</p>", requis.Count());
                    body = body + string.Format("<p>Monto a Facturar: ${0} </p><br/><br/>", requisiciones.examenes.Sum(s => s.costo) * requis.Count() );
                
                    body = body + "<br/><br/><br/><p>Favor de corroborar esta informaci&oacute;n y dar el seguimiento correspondiente.</p>";
                    body = body + "<br/><br/><p>Me despido de usted agradeciendo su atenci&oacute;n y enviandole un cordial saludo.</p>";

                    m.Body = body;
                    m.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                    smtp.Send(m);

                }

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
    }

    
}
