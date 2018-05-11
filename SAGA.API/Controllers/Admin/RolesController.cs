using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos.Admin;

namespace SAGA.API.Controllers.Admin
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
     
    }
}
