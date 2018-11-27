using SAGA.API.Dtos;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Component
{
    [RoutePrefix("api/Candidatos")]
    public class ComentariosCandidatoController : ApiController
    {
        private SAGADBContext db;

        public ComentariosCandidatoController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getComentarios")]
        public IHttpActionResult GetComentarios(Guid Id)
        {
            var comentarios = db.ComentariosEntrevistas
                .Where(x => x.CandidatoId.Equals(Id))
                .Select(x => new
                {
                    Motivo = x.Motivo.Id == 7 ? "" : x.Motivo.Descripcion,
                    Comentario = x.Comentario,
                    UsuarioAlta = x.UsuarioAlta,
                    Requisicion = db.Requisiciones
                                    .Where(r => r.Id.Equals(x.RequisicionId)).
                                    Select(r => new
                                    {
                                        VBtra = r.VBtra,
                                        Folio = r.Folio
                                    }).FirstOrDefault(),
                    fchComentario = x.fch_Creacion,
                    Usuario = db.Usuarios.Where(u => u.Id.Equals(x.ReclutadorId)).Select(u => new
                    {
                        Nombre = u.Nombre + " " + u.ApellidoPaterno,
                        Foto = u.Foto == null ? "utilerias/img/user/default.jpg" : u.Foto
                    }).FirstOrDefault()
                })
                .ToList().OrderBy(c => c.fchComentario);
            return Ok(comentarios);
        }

        [HttpPost]
        [Route("addComentarios")]
        public IHttpActionResult AddComentarios(ComentariosEntrevistaDto comentario)
        {
            try
            {
                ComentarioEntrevista cm = new ComentarioEntrevista();
                cm.Comentario = comentario.Comentario.ToUpper().Trim();
                cm.CandidatoId = comentario.CandidatoId;
                cm.RequisicionId = comentario.RequisicionId;
                cm.UsuarioAlta = comentario.Usuario;
                cm.ReclutadorId = comentario.UsuarioId;
                cm.fch_Creacion = DateTime.Now;
                cm.fch_Creacion.ToUniversalTime();
                cm.MotivoId = 7; //por mientras
                //cm.ReclutadorId = comentario.UsuarioId;


                db.ComentariosEntrevistas.Add(cm);
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("addComentariosNR")]
        public IHttpActionResult AddComentariosNR(ComentariosEntrevistaDto comentario)
        {
            //FoliosIncidenciasController obj = new FoliosIncidenciasController();
            FolioIncidenciasCandidatos obj = new FolioIncidenciasCandidatos();
            var aux = new Guid("00000000-0000-0000-0000-000000000000");

            var dbTrans = db.Database.BeginTransaction();
            
                try
                {
                    ComentarioEntrevista cm = new ComentarioEntrevista();
                    cm.Comentario = comentario.Comentario.ToUpper().Trim();
                    cm.CandidatoId = comentario.CandidatoId;
                    cm.RequisicionId = comentario.RequisicionId;
                    cm.UsuarioAlta = comentario.Usuario;
                    cm.ReclutadorId = comentario.UsuarioId;
                    cm.fch_Creacion = DateTime.Now;
                    cm.fch_Creacion.ToUniversalTime();
                    cm.MotivoId = comentario.MotivoId; //por mientras

                    db.ComentariosEntrevistas.Add(cm);
                    db.SaveChanges();

                  //fecha = db.ComentariosEntrevistas.Where(x => x.Id.Equals(comentarioId)).Select(f => f.fch_Creacion).FirstOrDefault();

                    var c = db.FoliosIncidendiasCandidatos.Where(x => x.EstatusId.Equals(28)).Count();

                    string folio = cm.fch_Creacion.Year.ToString() + cm.fch_Creacion.Month.ToString() + cm.fch_Creacion.Day.ToString().PadLeft(2, '0') + (c + 1).ToString().PadLeft(4, '0');

                    obj.EstatusId = 28;
                    obj.Folio = folio;
                    obj.ComentarioId = cm.Id;

                    db.FoliosIncidendiasCandidatos.Add(obj);

                    db.SaveChanges();

                    //if (obj.GenerarFolioNR(28, cm.Id, cm.fch_Creacion))
                    //{
                    //    //var idC = db.Postulaciones.Where(x => x.CandidatoId.Equals(comentario.CandidatoId)).Select(id => id.Id).FirstOrDefault();

                        //if(idC != aux )
                        //{
                        //    var postulado = db.Postulaciones.Find(idC);
                        //    db.Entry(postulado).State = System.Data.Entity.EntityState.Modified;
                        //    postulado.StatusId = 5; //proceso finalizado

                        //    db.SaveChanges();
                        //}

                        //var idproceso = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(comentario.CandidatoId)).Select(idp => idp.Id).FirstOrDefault();
                        //var proceso = db.ProcesoCandidatos.Find(idproceso);
                        //db.Entry(proceso).State = System.Data.Entity.EntityState.Modified;
                        //proceso.EstatusId = 42;

                        //db.SaveChanges();

                        dbTrans.Commit();
                        return Ok(HttpStatusCode.OK);

                    //}
                    //else
                    //{
                    //    dbTrans.Rollback();
                    //    return Ok(HttpStatusCode.NotFound);
                    //}

                    
                }
                catch (Exception ex)
                {
                    dbTrans.Rollback();
                    return Ok(HttpStatusCode.NotFound);
                }
            
        }

        [HttpPost]
        [Route("addRespuesta")]
        public IHttpActionResult AddRespuesta(ComentariosEntrevistaDto comentario)
        {
            try
            {
                ComentarioEntrevista cm = new ComentarioEntrevista();
                cm.Comentario = comentario.Comentario.ToUpper().Trim();
                cm.CandidatoId = comentario.CandidatoId;
                cm.RequisicionId = comentario.RequisicionId;
                cm.UsuarioAlta = comentario.Usuario;
                cm.ReclutadorId = comentario.UsuarioId;
                cm.fch_Creacion = DateTime.Now;
                cm.fch_Creacion.ToUniversalTime();
                cm.MotivoId = comentario.MotivoId; //por mientras
                cm.RespuestaId = comentario.RespuestaId;

                db.ComentariosEntrevistas.Add(cm);
                db.SaveChanges();

                if (comentario.estatusId != 28)
                {
                    var idc = db.PerfilCandidato.Where(x => x.CandidatoId.Equals(comentario.CandidatoId)).Select(c => c.Id).FirstOrDefault();
                    var pc = db.PerfilCandidato.Find(idc);

                    db.Entry(pc).State = System.Data.Entity.EntityState.Modified;
                    pc.Estatus = 41;

                    db.SaveChanges();

                    var idcc = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(comentario.CandidatoId) && x.RequisicionId.Equals(comentario.RequisicionId)).Select(c => c.Id).FirstOrDefault();
                    var cc = db.ProcesoCandidatos.Find(idcc);

                    db.Entry(cc).State = System.Data.Entity.EntityState.Modified;
                    cc.EstatusId = 27;

                    db.SaveChanges();

                }
                else if(comentario.estatusId == 28)
                {
                    var idc = db.PerfilCandidato.Where(x => x.CandidatoId.Equals(comentario.CandidatoId)).Select(c => c.Id).FirstOrDefault();
                    var pc = db.PerfilCandidato.Find(idc);

                    db.Entry(pc).State = System.Data.Entity.EntityState.Modified;
                    pc.Estatus = 28;

                    db.SaveChanges();

                    var idcc = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(comentario.CandidatoId) && x.RequisicionId.Equals(comentario.RequisicionId)).Select(c => c.Id).FirstOrDefault();
                    var cc = db.ProcesoCandidatos.Find(idcc);

                    db.Entry(cc).State = System.Data.Entity.EntityState.Modified;
                    cc.EstatusId = 28;

                    db.SaveChanges();
                }

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.NotFound);
            }
        }
        [HttpGet]
        [Route("getFoliosIncidencias")]
        public IHttpActionResult GetFoliosIncidencias(int estatus)
        {
            Guid aux = new Guid("00000000-0000-0000-0000-000000000000");
            try
            {
                //var folio = db.FoliosIncidendiasCandidatos.Where(x => x.EstatusId.Equals(estatus) && db.ProcesoCandidatos.Where(xx => xx.CandidatoId.Equals(x.Comentario.CandidatoId) && xx.RequisicionId.Equals(x.Comentario.RequisicionId)).Select(cc => cc.EstatusId).FirstOrDefault() == 42).Select(inf => new
                //{
                //    respuesta = inf.Comentario.RespuestaId,
                //    comentarioId = inf.ComentarioId,
                //    candidatoId = inf.Comentario.CandidatoId,
                //    requisicionId = inf.Comentario.RequisicionId,
                //    folio = db.Requisiciones.Where(r => r.Id.Equals(inf.Comentario.RequisicionId)).Select(rr => rr.Folio).FirstOrDefault(),
                //    reclutador = db.Usuarios.Where(x => x.Id.Equals(inf.Comentario.ReclutadorId)).Select(p => p.Clave + " " + p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno).FirstOrDefault(),
                //    motivo = inf.Comentario.Motivo.Descripcion,
                //    motivoId = inf.Comentario.MotivoId,
                //    fecha = inf.Comentario.fch_Creacion,
                //    candidato = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(inf.Comentario.CandidatoId)).Select(p => p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno).FirstOrDefault(),
                //    direccion = inf.Comentario.Candidato.direcciones.FirstOrDefault().Calle + " " + inf.Comentario.Candidato.direcciones.FirstOrDefault().NumeroExterior + " col. " + inf.Comentario.Candidato.direcciones.FirstOrDefault().Colonia.colonia + " CP." + inf.Comentario.Candidato.direcciones.FirstOrDefault().Colonia.CP + " Tel. " + inf.Comentario.Candidato.telefonos.FirstOrDefault().telefono,
                //    estatus = inf.Comentario.Motivo.Estatus.Descripcion,
                //    comentario = db.ComentariosEntrevistas.OrderByDescending(f => f.fch_Creacion).Where(x => x.Id.Equals(inf.ComentarioId) && x.RespuestaId.Equals(aux)).Select(c => new
                //    {
                //        resp = c.RespuestaId,
                //        comentario = c.Comentario,
                //        //respuesta = db.ComentariosEntrevistas.Where(x => x.RespuestaId.Equals(inf.ComentarioId)).Select(cc => cc.Comentario).ToString()
                //    }).FirstOrDefault()

                //}).OrderByDescending(f => f.fecha).ToList();

          
                var folio = db.ComentariosEntrevistas.Where(x => x.Motivo.EstatusId.Equals(estatus) && db.ProcesoCandidatos.Where(xx => xx.CandidatoId.Equals(x.CandidatoId) && xx.RequisicionId.Equals(x.RequisicionId)).Select(cc => cc.EstatusId).FirstOrDefault() == 42).Select(inf => new
                {
                    comentarioId = inf.Id,
                    candidatoId = inf.CandidatoId,
                    requisicionId = inf.RequisicionId,
                    folio = db.Requisiciones.Where(x => x.Id.Equals(inf.RequisicionId)).Select(r => r.Folio).FirstOrDefault(),
                    reclutador = db.Usuarios.Where(x => x.Id.Equals(inf.ReclutadorId)).Select(p => p.Clave + " " + p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno).FirstOrDefault(),
                    motivo = inf.Motivo.Descripcion,
                    motivoId = inf.Motivo.Id,
                    fecha = inf.fch_Creacion,
                    candidato = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(inf.CandidatoId)).Select(p => p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno).FirstOrDefault(),
                    direccion = inf.Candidato.direcciones.FirstOrDefault().Calle + " " + inf.Candidato.direcciones.FirstOrDefault().NumeroExterior + " col. " + inf.Candidato.direcciones.FirstOrDefault().Colonia.colonia + " CP." + inf.Candidato.direcciones.FirstOrDefault().Colonia.CP + " Tel. " + inf.Candidato.telefonos.FirstOrDefault().telefono,
                    estatus = inf.Motivo.Estatus.Descripcion,
                    comentario = inf.Comentario,
                    respuesta = ""
                }).OrderByDescending(f => f.fecha).GroupBy(g => g.candidatoId).Select(result => result.FirstOrDefault()).ToList();

               
                return Ok(folio);
            }
            catch(Exception ex)
            {

                return Ok(HttpStatusCode.BadRequest);
            }
            
        }


    }
}
