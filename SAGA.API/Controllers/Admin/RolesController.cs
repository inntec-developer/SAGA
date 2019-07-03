using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using System.Data.Entity;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/admin")]
    public class RolesController : ApiController
    {
 
        private SAGADBContext db;
        public RolesController()
        {
            db = new SAGADBContext();
        }

        [HttpPost]
        [Route("agregarRol")]
        public IHttpActionResult AgregarRol(List<PrivilegiosDtos> privilegios)
        {
            try
            {
                Roles obj = new Roles();

                obj.Rol = privilegios.Select(n => n.Nombre).FirstOrDefault();
                obj.Activo = true;

                db.Roles.Add(obj);
                db.SaveChanges();

                foreach (PrivilegiosDtos ru in privilegios)
                {
                    if (ru.Create || ru.Read || ru.Update || ru.Delete || ru.Especial)
                    {
                        Privilegio o = new Privilegio();
                        o.RolId = obj.Id;
                        o.EstructuraId = ru.EstructuraId;
                        o.Create = ru.Create;
                        o.Read = ru.Read;
                        o.Update = ru.Update;
                        o.Delete = ru.Delete;
                        o.Especial = ru.Especial;
                        
                        db.Privilegios.Add(o);
                        db.SaveChanges();
                    }

                }

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
            

        }

        [HttpPost]
        [Route("updateRoles")]
        [Authorize]
        public IHttpActionResult updateRoles(Roles listJson)
        {
            string msj = "Actualizó";
            try
            {
                var r = db.Roles.Find(listJson.Id);

                db.Entry(r).State = EntityState.Modified;

                r.Rol = listJson.Rol;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.Message;
            }

            return Ok(msj);
        }

        [HttpGet]
        [Route("deleteRoles")]
        public IHttpActionResult deleteRoles(int id)
        {
            try
            {
                var r = db.Roles.Find(id);

                db.Entry(r).State = EntityState.Modified;
                r.Activo = false;
              
                db.SaveChanges();


                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }
        [HttpPost]
        [Route("deleteUserRol")]
        [Authorize]
        public IHttpActionResult DeleteUserRol(RolEntidad indices)
        {
            string msj = "Borró usuario";

            try
            {
                var idGU = db.RolEntidades
                     .Where(x => x.EntidadId.Equals(indices.EntidadId))
                     .Where(x => x.RolId.Equals(indices.RolId))
                     .Select(x => x.Id).FirstOrDefault();

                var dts = db.RolEntidades.Find(idGU);

                db.Entry(dts).State = EntityState.Deleted;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.Message;
            }

            return Ok(msj);
        }

        [HttpPost]
        [Route("addGroupRol")]
        [Authorize]
        public IHttpActionResult AddGroupRol(List<RolEntidad> listJson)
        {
            string msj = "Los datos se agregaron con éxito";

            try
            {
                List<RolEntidad> obj = new List<RolEntidad>();

                foreach (RolEntidad re in listJson)
                {
                    db.RolEntidades.Add(re);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msj = ex.Message;
            }
            return Ok(msj);
        }

        [HttpGet]
        [Route("getEstructuraRoles")]
        [Authorize]
        public IHttpActionResult GetEstructuraRoles(int rol)
        {
            var privilegiosRoles = db.Privilegios.Where(x => x.RolId.Equals(rol) & x.Rol.Activo.Equals(true) & x.Estructura.TipoEstructuraId <= 4)
            .Select(P => new PrivilegiosDtos()
            {
                RolId = P.RolId,
                Nombre = P.Estructura.Nombre,
                Create = P.Create,
                Read = P.Read,
                Update = P.Update,
                Delete = P.Delete,
                Especial = P.Especial,
                IdPadre = P.Estructura.IdPadre,
                EstructuraId = P.Estructura.Id,
                Accion = P.Estructura.Accion,
                Icono = P.Estructura.Icono,
                TipoEstructuraId = P.Estructura.TipoEstructuraId,
                Orden = P.Estructura.Orden
            }).OrderBy(o => o.Orden).ToList();


            //var dtos = db.Privilegios.GroupBy( g => new { g.Rol, g.RolId, g.EstructuraId, g.Estructura.Nombre, g.Estructura.Orden, g.Create, g.Read, g.Update, g.Delete, g.Especial }).Select(P => new
            //   {
            //       RolId = db.Roles.Where(x => x.Id.Equals(P.Key.RolId) && x.Activo).Select(R => R.Id).FirstOrDefault(),
            //       Rol = db.Roles.Where(x => x.Id.Equals(P.Key.RolId) && x.Activo).Select(R => R.Rol).FirstOrDefault(),
            //       EstructuraId = db.Estructuras.Where(x => x.Id.Equals(P.Key.EstructuraId)).Select(E => E.Id).FirstOrDefault(),
            //       Nombre = db.Estructuras.Where(x => x.Id.Equals(P.Key.EstructuraId)).Select(E => E.Nombre).FirstOrDefault(),
            //       create = P.Key.Create,
            //       read = P.Key.Read,
            //       update = P.Key.Update,
            //       delete = P.Key.Delete,
            //       especial = P.Key.Especial,
            //       orden = P.Key.Orden,
            //       TipoEstructuraId = db.Estructuras.Where(x => x.Id.Equals(P.Key.EstructuraId)).Select(E => E.TipoEstructuraId).FirstOrDefault(),
            //}).OrderBy(o => o.orden ).ToList();



            return Ok(privilegiosRoles);

        }

        public ICollection<PrivilegiosDtos> GetChild( int id)
        {
            return db.Privilegios
                    .Where(x => x.Estructura.IdPadre == id)
                    .Select(P => new PrivilegiosDtos
                    {
                        RolId = P.RolId,
                        Nombre = P.Estructura.Nombre,
                        Create = P.Create,
                        Read = P.Read,
                        Update = P.Update,
                        Delete = P.Delete,
                        Especial = P.Especial,
                        IdPadre = P.Estructura.IdPadre,
                        EstructuraId = P.Estructura.Id,
                        Accion = P.Estructura.Accion,
                        Icono = P.Estructura.Icono,
                        TipoEstructuraId = P.Estructura.TipoEstructuraId,
                        Orden = P.Estructura.Orden,
                        Children = GetChild(P.EstructuraId)
                    }).OrderBy(o => o.Orden)
                    .ToList();
        }

    }
}
