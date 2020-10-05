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

                var check = db.VertionSistem.Where(x => x.Liberada == true)
                    .OrderByDescending(x => x.Id)
                    .Select(x => new
                    {
                        version = x.Version
                    })
                    .FirstOrDefault();
            //    var check = db.VertionSistem
            //.OrderByDescending(x => x.Id)
            //.Select(x => new
            //{
            //    version = x.Version
            //})
            //.FirstOrDefault();
                if (check.version == version)
                {
                    Actualizado = true;
                }
                else
                {
                    Actualizado = false;
                }

                var obj = new
                {
                    Actualizado = Actualizado,
                    Version = check.version
                };
                return Ok(obj);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
    }
}
