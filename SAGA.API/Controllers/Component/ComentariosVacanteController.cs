using SAGA.API.Dtos;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/reclutamiento/seguimientovacante")]
    public class ComentariosVacanteController : ApiController
    {
        private SAGADBContext db;

        public ComentariosVacanteController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getComentariosVacante")]
        [Authorize]
        public IHttpActionResult GetComentarios(Guid Id)
        {
            try
            {
                var comentarios = db.ComentariosVacantes
                    .Where(x => x.RequisicionId.Equals(Id))
                    .Select(x => new
                    {
                        id = x.Id,
                        Motivo = x.Motivo.Descripcion == "No aplica" ? "" : x.Motivo.Descripcion,
                        Comentario = x.Comentario,
                        Usuario = x.Reclutador.Nombre + " " + x.Reclutador.ApellidoPaterno,
                        UsuarioId = x.Reclutador.Id,
                        Clave = x.Reclutador.Clave,
                        Foto = x.Reclutador.Foto,
                        fchComentario = x.fch_Creacion
                    })
                    .OrderBy(x => x.fchComentario).ToList();
                return Ok(comentarios);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("addComentariosVacante")]
        [Authorize]
        public IHttpActionResult AddComentariosVacante(ComentariosRequisicionesDto comentario)
        {
            FoliosIncidenciasController Ofi = new FoliosIncidenciasController();

            //var T = db.Database.BeginTransaction();
            try
            {
                ComentarioVacante cm = new ComentarioVacante();
                cm.Comentario = comentario.Comentario.ToUpper().Trim();
                cm.RequisicionId = comentario.RequisicionId;
                cm.UsuarioAlta = db.Usuarios.Where(x => x.Id.Equals(comentario.ReclutadorId)).Select(u => u.Usuario).FirstOrDefault();
                cm.ReclutadorId = comentario.ReclutadorId;
                cm.MotivoId = comentario.MotivoId;
                cm.RespuestaId = comentario.RespuestaId;

                db.ComentariosVacantes.Add(cm);
                db.SaveChanges();

                if (comentario.EstatusId.Equals(39)) //pausada
                {
                    Ofi.GenerarFolio(comentario.EstatusId, cm.Id);
                    Ofi.EnviarEmail(comentario.EstatusId, comentario.RequisicionId, comentario.ReclutadorId);
                }
                else if (comentario.EstatusId.Equals(20))
                {
                    if (comentario.Tipo == 3)
                    {
                        Ofi.TransferRequiReclutador(comentario.RequisicionId, comentario.UsuarioAux, comentario.UsuarioTransferId, comentario.ReclutadorId);
                    }
                    else if (comentario.Tipo == 4)
                    {
                        Ofi.TransferDamfo(comentario.RequisicionId, comentario.ReclutadorId, comentario.UsuarioTransferId);
                    }
                    else
                    {
                        Ofi.TransferRequi(comentario.RequisicionId, comentario.UsuarioTransferId, comentario.ReclutadorId, comentario.Tipo);
                    }
                }
                else if (!comentario.EstatusId.Equals(8) && !comentario.EstatusId.Equals(0))
                { // para des pausar
                    Ofi.EnviarEmail2(comentario.EstatusId, comentario.RequisicionId, comentario.ReclutadorId);
                }

                //T.Commit();
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                //T.Rollback();
                return Ok(HttpStatusCode.NotFound);
            }
        }
        [HttpPost]
        [Route("deleteComentariosVacante")]
        [Authorize]
        public IHttpActionResult DeleteComentariosVacante(ComentariosRequisicionesDto comentario)
        {
            try
            {
                var id = db.ComentariosVacantes.Find(comentario.Id);
                db.Entry(id).State = EntityState.Deleted;
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        [HttpPost]
        [Route("updateComentariosVacante")]
        [Authorize]
        public IHttpActionResult UpdateComentariosVacante(ComentariosRequisicionesDto comentario)
        {
            try
            {
                var id = db.ComentariosVacantes.Find(comentario.Id);
                db.Entry(id).Property(x => x.Comentario).IsModified = true;

                id.Comentario = comentario.Comentario;

                db.SaveChanges();

                return Ok(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
    }

}
