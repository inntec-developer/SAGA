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
            FoliosIncidenciasController obj = new FoliosIncidenciasController();
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

                var idc = db.PerfilCandidato.Where(x => x.CandidatoId.Equals(comentario.CandidatoId)).Select(c => c.Id).FirstOrDefault();
                var pc = db.PerfilCandidato.Find(idc);

                db.Entry(pc).State = System.Data.Entity.EntityState.Modified;
                pc.Estatus = 28;

                db.SaveChanges();

                obj.GenerarFolioNR(28, cm.Id);

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
            try
            {
                var folio = db.ComentariosEntrevistas.Where(x => x.Motivo.EstatusId.Equals(estatus)).Select(inf => new
                {
                    candidatoId = inf.CandidatoId,
                    folio = db.FoliosIncidendiasCandidatos.Where(x => x.ComentarioId.Equals(inf.Id)).Select(d => d.Folio),
                    reclutador = db.Usuarios.Where(x => x.Id.Equals(inf.ReclutadorId)).Select(p => p.Clave + " " + p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno),
                    motivo = inf.Motivo.Descripcion,
                    fecha = inf.fch_Creacion,
                    candidato = db.CandidatosInfo.Where(x => x.CandidatoId.Equals(inf.CandidatoId)).Select(p => p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno),
                    direccion = inf.Candidato.direcciones,
                    estatus = inf.Motivo.Estatus.Descripcion, 
                    comentario = inf.Comentario,
                    respuesta = db.ComentariosEntrevistas.Where(x => x.RespuestaId.Equals(inf.Id)).Select( c => c.Comentario )
                });

                return Ok(folio);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
            
        }


    }
}
