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
                    Motivo = x.Motivo.Descripcion == "No aplica" ? "" : x.Motivo.Descripcion,
                    Comentario = x.Comentario,
                    Requisicion = db.Requisiciones
                                    .Where(r => r.Id.Equals(x.RequisicionId)).
                                    Select(r => new
                                    {
                                        VBtra = r.VBtra,
                                        Folio = r.Folio
                                    }).FirstOrDefault(),
                    fchComentario = x.fch_Creacion,
                    Usuario = db.Usuarios.Where(u => u.Usuario.Equals(x.UsuarioAlta)).Select(u => new
                    {
                        Nombre = u.Nombre + " " + u.ApellidoPaterno,
                        Foto = u.Foto
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
                //cm.ReclutadorId = comentario.UsuarioId;

                db.ComentariosEntrevistas.Add(cm);
                db.SaveChanges();

                PerfilCandidato pc = new PerfilCandidato();
                pc.CandidatoId = comentario.CandidatoId;
                pc.Estatus = 28;

                db.PerfilCandidato.Add(pc);
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.NotFound);
            }
        }
    }
}
