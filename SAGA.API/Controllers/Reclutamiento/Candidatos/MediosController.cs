using SAGA.DAL;
using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.API.Dtos;
using AutoMapper;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace SAGA.API.Controllers.Reclutamiento.Candidatos
{
    [RoutePrefix("api/Candidatos")]
    public class MediosController : ApiController
    {
        private SAGADBContext db;

        public MediosController()
        {
            db = new SAGADBContext();
        }

        [HttpPost]
        [Route("insertMedios")]
        public IHttpActionResult InsertMedios(TiposMedios data)
        {
            try
            {
                db.TiposMedios.Add(data);
                db.SaveChanges();

                return Ok(HttpStatusCode.Accepted);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [Route("getMediosRecl")]
        public IHttpActionResult GetMediosRecl()
        {
            try
            {
                var medios = db.Medios.Where(x => x.Activo).Select(m => new
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    medios = db.TiposMedios.Where(x => x.MediosId.Equals(m.Id)).Select(tm => new
                    {
                        TipoMediosId = tm.Id,
                        TipoNombre = tm.Nombre
                    }).ToList()

                }).ToList();

                return Ok(medios);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

    }
}
