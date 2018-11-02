using SAGA.API.Dtos;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
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
        public IHttpActionResult GetComentarios(Guid Id)
        {
            try
            {
                var comentarios = db.ComentariosVacantes
                    .Where(x => x.RequisicionId.Equals(Id))
                    .Select(x => new
                    {
                        Motivo = x.Motivo.Descripcion == "No aplica" ? "" : x.Motivo.Descripcion,
                        Comentario = x.Comentario,
                        Usuario = x.Reclutador.Nombre + " " + x.Reclutador.ApellidoPaterno,
                        Foto = x.Reclutador.Foto,
                        fchComentario = x.fch_Creacion
                    })
                    .ToList()
                    .OrderBy(x => x.fchComentario);
                return Ok(comentarios);
            }
            catch(Exception ex)
            {
                var msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("addComentariosVacante")]
        public IHttpActionResult AddComentarios(ComentariosRequisicionesDto comentario)
        {
            try
            {
                ComentarioVacante cm = new ComentarioVacante();
                cm.Comentario = comentario.Comentario.ToUpper().Trim();
                cm.RequisicionId = comentario.RequisicionId;
                cm.UsuarioAlta = comentario.UsuarioAlta;
                cm.ReclutadorId = comentario.ReclutadorId;
                cm.MotivoId = comentario.MotivoId;

                db.ComentariosVacantes.Add(cm);
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
    }
}
