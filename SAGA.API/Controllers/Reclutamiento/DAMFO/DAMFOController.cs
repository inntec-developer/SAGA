using SAGA.API.Dtos;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Damfo290")]
    public class DAMFOController : ApiController
    {
        private SAGADBContext db;
        Damfo290Dto DamfoDto;

        public DAMFOController()
        {
            db = new SAGADBContext();
            DamfoDto = new Damfo290Dto();
        }

        //api/Damfo290/getViewDamfos
        [HttpGet]
        [Route("getViewDamfos")]
        public IHttpActionResult Get()
        {
            var damfo290 = db.DAMFO290.Where(df => df.Activo).Select(df => new
            {
                Id = df.Id,
                Cliente = df.Cliente.Nombrecomercial,
                NombrePerfil = df.NombrePerfil,
                Vacantes = df.horariosPerfil.Count() > 0 ? df.horariosPerfil.Sum( h => h.numeroVacantes) : 0,
                SueldoMinimo = df.SueldoMinimo,
                SueldoMaximo = df.SueldoMaximo,
                TipoReclutamiento = df.TipoReclutamiento.tipoReclutamiento,
                ClaseReclutamiento = df.ClaseReclutamiento.clasesReclutamiento,
                fch_Creacion = df.fch_Creacion
            }).ToList();


            return Ok(damfo290);
        }
        //api/Damfo290/getById
        [HttpGet]
        [Route("getById")]
        public IHttpActionResult GetById(Guid Id)
        {
            try
            {
                var damfoGetById = db.DAMFO290.First(x => x.Id.Equals(Id));
                return Ok(damfoGetById);
            }
            catch (Exception ex)
            {
                string mssg = ex.Message;
                return Ok(mssg);
            }
        }
    }
}
