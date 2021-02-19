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
        [HttpGet]
        [Route("getUserByType")]
        public IHttpActionResult GetUserByType(string type, string dep)
        {
            try
            {
                var entidad = db.Usuarios.Where(x => type.Contains(x.TipoUsuario.Tipo.ToLower())
                    && dep.Contains(x.Departamento.Clave.ToLower()) && x.Activo).Select(u => new UsuariosDto
                    {
                        Id = u.Id,
                        Nombre = u.Nombre + " " + u.ApellidoPaterno,
                        Usuario = u.Usuario,
                        Email = db.Emails.Where(e => e.EntidadId.Equals(u.Id)).Select(e => e.email).FirstOrDefault(),
                        TipoUsuario = u.TipoUsuario.Tipo
                    }).ToList();

                if(entidad.Count() == 0 )
                {
                    return Ok(HttpStatusCode.NoContent);
                }
                return Ok(entidad);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
        //api/AsignacionRequi/getUserGroup VENTAS
        [HttpGet]
        [Route("getUserGroup")]
        public IHttpActionResult GetUserGroup()
        {
            var rol = db.Privilegios.Where(p => p.EstructuraId.Equals(130)).Select(x => x.RolId).ToList();
            var entidad = db.RolEntidades.Where(x => rol.Contains(x.RolId)).Select(x => x.EntidadId).ToList();

            var asignacion = db.Usuarios
                                  .Where(u => u.Activo.Equals(true))
                                  .Where(u => (u.TipoUsuarioId > 3 && u.TipoUsuarioId <= 5) || u.TipoUsuarioId == 10)
                                  .Where(u => (u.Departamento.Clave == "RECL" || u.Departamento.Clave == "RCMP" || u.Departamento.Clave == "RECM"))
                                  .Select(u => new UsuariosDto
                                  {
                                      Id = u.Id,
                                      Nombre = u.Nombre + " " + u.ApellidoPaterno,
                                      Usuario = u.Usuario,
                                      Email = db.Emails.Where(e => e.EntidadId.Equals(u.Id)).Select(e => e.email).FirstOrDefault(),
                                      TipoUsuario = u.TipoUsuario.Tipo
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

            var asignacion = db.Usuarios
                                .Where(u => u.Activo.Equals(true))
                                .Where(u => (u.TipoUsuarioId == 11 || u.TipoUsuarioId == 5) || u.TipoUsuarioId.Equals(11))
                                .Where(u => (u.Departamento.Clave == "RECL" || u.Departamento.Clave == "RCMP" || u.Departamento.Clave == "RECM"))
                                .Select(u => new UsuariosDto
                                {
                                    Id = u.Id,
                                    Nombre = u.Nombre + " " + u.ApellidoPaterno,
                                    Usuario = u.Usuario,
                                    Email = db.Emails.Where(e => e.EntidadId.Equals(u.Id)).Select(e => e.email).FirstOrDefault(),
                                    TipoUsuario = u.TipoUsuario.Tipo
                                }).ToList();
            return Ok(asignacion);
        }

        [HttpGet]
        [Route("getAsignados")]
        public IHttpActionResult GetAsignados(Guid requisicionId)
        {
            try
            {
                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requisicionId) & x.Tipo.Equals(2)).Select(A => new
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
