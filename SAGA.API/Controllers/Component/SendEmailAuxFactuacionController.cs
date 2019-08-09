using SAGA.API.Utilerias;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Component
{
    [RoutePrefix("api/Correo")]
    public class SendEmailAuxFactuacionController : ApiController
    {
        private SAGADBContext db;
        private SendEmails SendEmail;

        public SendEmailAuxFactuacionController()
        {
            db = new SAGADBContext();
            SendEmail = new SendEmails();
        }

        [HttpGet]
        [Route("enviarCorreFactPuro")]
        public IHttpActionResult SenEmailFacturacio(Int64 Folio)
        {
            try
            {
                var IdRequisicion = db.Requisiciones
                    .Where(r => r.Folio.Equals(Folio))
                    .Select(r => r.Id)
                    .FirstOrDefault();
                SendEmail.SendEmailRequisPurasHafacturar(IdRequisicion);
                return Ok(HttpStatusCode.Accepted);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("checkFolioPuro")]
        public IHttpActionResult CheckFolioPuro(Int64 Folio)
        {
            try
            {
                var tipoReclutamiento = db.Requisiciones
                    .Where(r => r.Folio.Equals(Folio))
                    .Select(r => r.TipoReclutamientoId)
                    .FirstOrDefault();
                if (tipoReclutamiento == 1 && tipoReclutamiento != null)
                {
                    return Ok(HttpStatusCode.Accepted);
                }
                else
                {
                    return Ok(HttpStatusCode.NoContent);
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
    }
}
