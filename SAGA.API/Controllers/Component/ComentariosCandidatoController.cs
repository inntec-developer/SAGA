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

        //[HttpGet]
        //[Route("getComentarios")]
        //public IHttpActionResult GetComentarios(Guid Id)
        //{
        //    var comentarios = db.ComentariosEntrevistas
        //        .Where(x => x.CandidatoId.Equals(Id))
        //        .Select(x => new {
        //            Comentario = x.Comentario,
        //            Requisicion = db.Requisiciones
        //                            .Where(r => r.Id.Equals(x.RequisicionId)).
        //                            Select(r => new {
        //                                VBtra = r.VBtra,
        //                                Folio = r.Folio
        //                            }).FirstOrDefault(),
        //            fchComentario = x.fch_Creacion,
        //            Usuario = db.Usuarios.Where(u => u.Usuario.Equals(x.UsuarioAlta)).Select(u => new {
        //                Nombre = u.Nombre + " " + u.ApellidoPaterno,
        //                Foto = u.Foto
        //            }).FirstOrDefault()
        //        })
        //        .ToList().OrderBy(c => c.fchComentario);
        //    return Ok(comentarios);
        //}

        //[HttpPost]
        //[Route("addComentarios")]
        //public IHttpActionResult AddComentarios(ComentariosEntrevistaDto comentario)
        //{
        //    try
        //    {
        //        ComentarioEntrevista cm = new ComentarioEntrevista();
        //        cm.Comentario = comentario.Comentario;
        //        cm.CandidatoId = comentario.CandidatoId;
        //        cm.RequisicionId = comentario.RequisicionId;
        //        cm.UsuarioAlta = comentario.Usuario;
        //        cm.fch_Creacion = DateTime.Now;
        //        cm.fch_Creacion.ToUniversalTime();
        //        cm.UsuarioMod = comentario.Usuario;
        //        cm.fch_Modificacion = cm.fch_Creacion;
        //        //cm.ReclutadorId = comentario.UsuarioId;


        //        db.ComentariosEntrevistas.Add(cm);
        //        db.SaveChanges();

        //        return Ok(HttpStatusCode.OK);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(HttpStatusCode.NotFound);
        //    }
        //}
    }
}
