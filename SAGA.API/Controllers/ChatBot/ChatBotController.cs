using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text.RegularExpressions;
namespace SAGA.API.Controllers.ChatBot
{
    [RoutePrefix("api/ChatBot")]
    public class ChatBotController : ApiController
    {
        private SAGADBContext db = new SAGADBContext();

        [HttpGet]
        [Route("sucursales")]
        public IHttpActionResult Sucursales(string filtro)
        {
            try
            {
                var municipioId = db.Municipios.Where(xx => xx.municipio.Equals(filtro)).Select(id => id.Id).FirstOrDefault();
                var result = db.Direcciones.Where(x => x.MunicipioId == municipioId && x.TipoDireccionId.Equals(3)).Select(S =>
                S.Calle + " " + S.NumeroExterior + " " + S.Colonia.colonia + " " + S.CodigoPostal + " " + S.Municipio.municipio
                ).ToArray();

                return Ok(result);
             
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
    }
}
