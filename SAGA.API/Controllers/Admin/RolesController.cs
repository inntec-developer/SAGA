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
        public IHttpActionResult AgregarRol(Roles listJson)
        {
            string mensaje = "Se agregó Rol";

            try
            {
                db.Roles.Add(listJson);
            
                db.SaveChanges();

                
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
                r.Activo = listJson.Activo;
                r.Create = listJson.Create;
                r.Update = listJson.Update;
                r.Read = listJson.Read;
                r.Rol = listJson.Rol;
                r.Delete = listJson.Delete;
                r.Especial = listJson.Especial;
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



    }
}
