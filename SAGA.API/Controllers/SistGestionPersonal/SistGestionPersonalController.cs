using SAGA.API.Utilerias;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.SistGestionPersonal
{
    [RoutePrefix("api/gestionpersonal")]
    public class SistGestionPersonalController : ApiController
    {
        private SAGADBContext db;
        public SistGestionPersonalController()
        {
            db = new SAGADBContext();
        }
        //[HttpGet]
        //[Route("getHorarios")]
        //public IHttpActionResult GetHorarios()
        //{
        //    try
        //    {
        //        var horarios = db.HorariosIngresos.Where(x => x.Activo)

        //    }
        //    catch(Exception ex)
        //    {
        //        APISAGALog log = new APISAGALog();
        //        log.WriteError("db.Horarios - " + ex.Message + " InnerException - " + ex.InnerException.Message);
        //        return Ok(HttpStatusCode.BadRequest);
        //    }

        //}
    }
}
