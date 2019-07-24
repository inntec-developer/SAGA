using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Version
{
    [RoutePrefix("api/Vertion")]
    public class CheckVertionController : ApiController
    {
        private SAGADBContext db;
        private bool Actualizado;

        public CheckVertionController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("Check")]
        public IHttpActionResult CheckVertion(string version)
        {
            try
            {
               
                var check = db.VertionSistem.Where(x => x.Version == version && x.Liberada == true)
                    .OrderByDescending(x => x.Id)
                    .Take(1)
                    .Count();
                if(check == 1)
                {
                    Actualizado = true;
                }
                else
                {
                    Actualizado = false;
                }
                return Ok(Actualizado);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
    }
}
