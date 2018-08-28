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
            var rol = db.Privilegios.Where(p => p.EstructuraId.Equals(130)).Select(x => x.RolId).ToList();
            var entidad = db.RolEntidades.Where(x => rol.Contains(x.RolId)).Select(x => x.EntidadId).ToList();

            var asignacion = db.Usuarios
                .Where(u => u.Activo.Equals(true) && u.TipoUsuarioId <= 5 && u.TipoUsuarioId > 0 && entidad.Contains(u.Id))
                .Select(x => new AsigancionDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre + " " + x.ApellidoPaterno + " " + x.ApellidoMaterno,
                    Descripcion = x.Usuario,
                    Email = db.Emails.Where(e => e.EntidadId.Equals(x.Id)).Select(e => e.email).FirstOrDefault(),
                    TipoEntidadId = x.TipoEntidadId
                }).OrderBy(u => u.Nombre)
                .Union(
                    db.Grupos
                    .OrderByDescending(g => g.Nombre)
                    .Where(g => g.Activo.Equals(true) && entidad.Contains(g.Id))
                    .Select(g => new AsigancionDto
                    {
                        Id = g.Id,
                        Nombre = g.Nombre + " " + g.ApellidoPaterno + " " + g.ApellidoMaterno,
                        Descripcion = g.Descripcion,
                        Email = db.Emails.Where(e => e.EntidadId.Equals(g.Id)).Select(e => e.email).FirstOrDefault(),
                        TipoEntidadId = g.TipoEntidadId
                    })
                ).OrderBy( g=> g.Nombre);
            return Ok(asignacion);
        }

        [HttpGet]
        [Route("getUserGroupL")]
        public IHttpActionResult GetUserGroupL()

        {
            var rol = db.Privilegios.Where(p => p.EstructuraId.Equals(130)).Select(x => x.RolId).ToList();
            var entidad = db.RolEntidades.Where(x => rol.Contains(x.RolId)).Select(x => x.EntidadId).ToList();

            var asignacion = db.Usuarios
                .Where(u => u.Activo.Equals(true) && u.TipoUsuarioId <= 6 && u.TipoUsuarioId > 0 && entidad.Contains(u.Id))
                .Select(x => new AsigancionDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre + " " + x.ApellidoPaterno + " " + x.ApellidoMaterno,
                    Descripcion = x.Usuario,
                    Email = db.Emails.Where(e => e.EntidadId.Equals(x.Id)).Select(e => e.email).FirstOrDefault(),
                    TipoEntidadId = x.TipoEntidadId
                }).OrderBy(u => u.Nombre)
                .Union(
                    db.Grupos
                    .OrderByDescending(g => g.Nombre)
                    .Where(g => g.Activo.Equals(true) && entidad.Contains(g.Id))
                    .Select(g => new AsigancionDto
                    {
                        Id = g.Id,
                        Nombre = g.Nombre + " " + g.ApellidoPaterno + " " + g.ApellidoMaterno,
                        Descripcion = g.Descripcion,
                        Email = db.Emails.Where(e => e.EntidadId.Equals(g.Id)).Select(e => e.email).FirstOrDefault(),
                        TipoEntidadId = g.TipoEntidadId
                    })
                ).OrderBy(g => g.Nombre);
            return Ok(asignacion);
        }
    }
}
