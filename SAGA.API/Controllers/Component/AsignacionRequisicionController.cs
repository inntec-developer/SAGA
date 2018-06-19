using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Component
{
    [RoutePrefix("api/AsignacionRequi")]
    public class AsignacionRequisicionController : ApiController
    {
        private SAGADBContext db;

        public AsignacionRequisicionController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getUserGroup")]
        public IHttpActionResult GetUserGroup()

        {
            var asignacion = db.Usuarios
                .OrderBy(u => u.Nombre)
                .Where(u => u.Activo.Equals(true))
                .Select(x => new AsigancionDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre + " " + x.ApellidoMaterno,
                    Descripcion = x.Usuario,
                    Email = db.Emails.Where(e => e.EntidadId.Equals(x.Id)).Select(e => e.email).FirstOrDefault(),
                    TipoEntidadId = x.TipoEntidadId
                })
                .Union(
                    db.Grupos
                    .OrderByDescending(g => g.Nombre)
                    .Where(g => g.Activo.Equals(true))
                    .Select(g => new AsigancionDto
                    {
                        Id = g.Id,
                        Nombre = g.Nombre + " " + g.ApellidoMaterno,
                        Descripcion = g.Descripcion,
                        Email = db.Emails.Where(e => e.EntidadId.Equals(g.Id)).Select(e => e.email).FirstOrDefault(),
                        TipoEntidadId = g.TipoEntidadId
                    })
                ).OrderByDescending( g=> g.TipoEntidadId);


            //var usuarios = db.Usuarios
            //    .Where(u => u.Activo.Equals(true))
            //    .Select(x => x.Nombre)
            //    .ToList();


            //var grupos = db.Grupos
            //    .Where(g => g.Activo.Equals(true))
            //    .Select(g => g.Nombre)
            //    .ToList();

            

            return Ok(asignacion);
        }


    }
}
