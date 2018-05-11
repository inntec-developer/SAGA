using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;

namespace SAGA.API.Controllers.Admin
{
    [RoutePrefix("api/admin")]
    public class GruposController : ApiController
    {
        private SAGADBContext db;
        public GruposController()
        {
            db = new SAGADBContext();
        }

        [HttpPost]
        [Route("agregarGrupo")]
        public IHttpActionResult AgregarRol(Grupos listJson)
        {
            string mensaje = "Se agregó Grupo";
            listJson.UsuarioAlta = "INNTEC";
            try
            {
                db.Grupos.Add(listJson);

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
