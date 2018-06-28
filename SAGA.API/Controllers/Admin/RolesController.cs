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
            string mensaje = "Se agregó Rol";
            //int id = 0;
            try
            {
                Roles obj = new Roles();

                obj.Rol = privilegios.Select(n => n.Nombre).FirstOrDefault();

                db.Roles.Add(obj);
                db.SaveChanges();

                foreach (PrivilegiosDtos ru in privilegios)
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
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }
            return Ok(mensaje);

        }

        [HttpPost]
        [Route("updateRoles")]
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

        [HttpPost]
        [Route("deleteRoles")]
        public IHttpActionResult deleteRoles(Roles listJson)
        {
            string msj = "Borró";
            try
            {
                var r = db.Roles.Find(listJson.Id);

                db.Entry(r).State = EntityState.Modified;
                r.Activo = false;

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
        public IHttpActionResult AddGroupRol(List<RolEntidad> listJson)
        {
            string msj = "Agrego";

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
        public IHttpActionResult GetEstructuraRoles()
        {
           
             var dtos = db.Privilegios.GroupBy( g => new { g.Rol, g.RolId, g.EstructuraId, g.Estructura.Nombre, g.Create, g.Read, g.Update, g.Delete, g.Especial }).Select(P => new
                {
                    RolId = db.Roles.Where(x => x.Id.Equals(P.Key.RolId)).Select(R => R.Id).FirstOrDefault(),
                    Rol = db.Roles.Where(x => x.Id.Equals(P.Key.RolId)).Select(R => R.Rol).FirstOrDefault(),
                    EstructuraId = db.Estructuras.Where(x => x.Id.Equals(P.Key.EstructuraId)).Select(E => E.Id).FirstOrDefault(),
                    Nombre = db.Estructuras.Where(x => x.Id.Equals(P.Key.EstructuraId)).Select(E => E.Nombre).FirstOrDefault(),
                    create = P.Key.Create,
                    read = P.Key.Read,
                    update = P.Key.Update,
                    delete = P.Key.Delete,
                    especial = P.Key.Especial
                }).OrderBy(o => new { o.Rol, o.EstructuraId }).ToList();

          

            return Ok(dtos);
        }
    }
}
