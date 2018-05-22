using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;

namespace SAGA.API.Controllers.Admin
{
    [RoutePrefix("api/admin")]
    public class PrivilegiosController : ApiController
    {
        private SAGADBContext db;
        public PrivilegiosController()
        {
            db = new SAGADBContext();
        }

        [HttpPost]
        [Route("agregarPrivilegio")]
        public IHttpActionResult AgregarPrivilegio(List<Privilegio> listJson)
        {
            string mensaje = "Se agregó Privilegio";
       
         
           
            try
            {
                foreach (Privilegio ru in listJson)
                {
                    db.Privilegios.Add(ru);
                }


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
