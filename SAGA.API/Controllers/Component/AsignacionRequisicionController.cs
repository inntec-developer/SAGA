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
    [Authorize]
    public class AsignacionRequisicionController : ApiController
    {
        private SAGADBContext db;

        public AsignacionRequisicionController()
        {
            db = new SAGADBContext();
        }
        //api/AsignacionRequi/getUserGroup VENTAS
        [HttpGet]
        [Route("getUserGroup")]
        public IHttpActionResult GetUserGroup()
        {
            var rol = db.Privilegios.Where(p => p.EstructuraId.Equals(130)).Select(x => x.RolId).ToList();
            var entidad = db.RolEntidades.Where(x => rol.Contains(x.RolId)).Select(x => x.EntidadId).ToList();

            var asignacion = db.Grupos.OrderBy(x => x.Nombre)
                .Where(g => g.Activo.Equals(true) && entidad.Contains(g.Id))
                .Where(g => (g.TipoGrupoId > 3 && g.TipoGrupoId <= 5) || g.TipoGrupoId == 10)
                .Select(g => new
                {
                    id = g.Id,
                    nombre = g.Nombre,
                    usuarios = db.Usuarios
                                .Where(u => (db.GruposUsuarios
                                            .Where(x => x.GrupoId.Equals(g.Id))
                                            .Where(x => x.Entidad.TipoEntidadId.Equals(1))
                                            .Select(x => x.EntidadId)
                                            .ToList()).Contains(u.Id) )
                                .Where(u =>  u.Activo.Equals(true))
                                .Where(u => (u.TipoUsuarioId > 2 && u.TipoUsuarioId <= 5) || u.TipoUsuarioId == 11)
                                .Select(u => new UsuariosDto
                                {
                                    Id = u.Id,
                                    Nombre = u.Nombre + " " + u.ApellidoPaterno,
                                    Usuario = u.Usuario,
                                    Email = db.Emails.Where(e => e.EntidadId.Equals(u.Id)).Select(e => e.email).FirstOrDefault(),
                                    TipoUsuario = u.TipoUsuario.Tipo
                                }).ToList()
                }).ToList();
            return Ok(asignacion);
        }

        [HttpGet]
        [Route("getUserGroupL")]
        public IHttpActionResult GetUserGroupL()

        {
            int[] tipoUsuario = { };
            var rol = db.Privilegios.Where(p => p.EstructuraId.Equals(130)).Select(x => x.RolId).ToList();
            var entidad = db.RolEntidades.Where(x => rol.Contains(x.RolId)).Select(x => x.EntidadId).ToList();

            var asignacion = db.Grupos.OrderBy(x => x.Nombre)
               .Where(g => g.Activo.Equals(true) && (g.TipoGrupoId == 11 || g.TipoGrupoId == 5) && entidad.Contains(g.Id))
               .Select(g => new
               {
                   id = g.Id,
                   nombre = g.Nombre,
                   usuarios = db.Usuarios
                               .Where(u => (db.GruposUsuarios
                                           .Where(x => x.GrupoId.Equals(g.Id))
                                           .Where(x => x.Entidad.TipoEntidadId.Equals(1))
                                           .Select(x => x.EntidadId)
                                           .ToList()).Contains(u.Id))
                                           .Where(u => u.Activo.Equals(true))
                                .Where(u => (u.TipoUsuarioId > 3 && u.TipoUsuarioId <= 6) || u.TipoUsuarioId.Equals(11))
                               .Select(u => new UsuariosDto
                               {
                                   Id = u.Id,
                                   Nombre = u.Nombre + " " + u.ApellidoPaterno,
                                   Usuario = u.Usuario,
                                   Email = db.Emails.Where(e => e.EntidadId.Equals(u.Id)).Select(e => e.email).FirstOrDefault(),
                                   TipoUsuario = u.TipoUsuario.Tipo
                               }).ToList()
               }).ToList();
            return Ok(asignacion);
        }

        [HttpGet]
        [Route("getAsignados")]
        public IHttpActionResult GetAsignados(Guid requisicionId)
        {
            try
            {
                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requisicionId) & !x.GrpUsrId.Equals(x.Requisicion.AprobadorId)).Select(A => new
                {
                    reclutadorId = A.GrpUsrId,
                    nombre = db.Usuarios.Where(x => x.Id.Equals(A.GrpUsrId)).Select(U => U.Nombre + " " + U.ApellidoPaterno + " " + U.ApellidoMaterno).FirstOrDefault()
                }).ToList();
                return Ok(asignados);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);

            }
        }
    }
}
