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
                int id = 0;
                var nom = privilegios.Select(n => n.Nombre).FirstOrDefault();
                obj.Rol = nom;

                db.Roles.Add(obj);
                id = db.SaveChanges();

                foreach (PrivilegiosDtos ru in privilegios)
                {
                    Privilegio o = new Privilegio();
                    o.RolId = id;
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



    }
}
