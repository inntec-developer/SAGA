using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Requisiciones")]
    public class RequisicionesController : ApiController
    {
        private SAGADBContext db;

        public RequisicionesController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getDamfos")]
        public IHttpActionResult Get()
        {
            Damfo290Dto DamfoDto = new Damfo290Dto();

            DamfoDto.Damfo290 = (from damfo in db.DAMFO290
                        join cliente in db.Clientes on damfo.ClienteId equals cliente.Id
                        join giroEmpresa in db.GirosEmpresas on cliente.GiroEmpresaId equals giroEmpresa.Id
                        join actividadEmpresa in db.ActividadesEmpresas on giroEmpresa.Id equals actividadEmpresa.Id
                        join tipoReclutamiento in db.TiposReclutamientos on damfo.TipoReclutamientoId equals tipoReclutamiento.Id
                        join claseReclutamiento in db.ClasesReclutamientos on damfo.ClaseReclutamientoId equals claseReclutamiento.Id
                        select new Damfo290GralDto {
                            Id = damfo.Id,
                            Cliente = cliente.RazonSocial,
                            NombrePerfil = damfo.NombrePerfil,
                            GiroEmpresa = giroEmpresa.giroEmpresa,
                            ActividadEmpresa = actividadEmpresa.actividadEmpresa,
                            TipoReclutamiento = tipoReclutamiento.tipoReclutamiento,
                            ClaseReclutamiento = claseReclutamiento.clasesReclutamiento,
                            SueldoMinimo = damfo.SueldoMinimo,
                            SueldoMaximo = damfo.SueldoMaximo,
                            fch_Creacion = damfo.fch_Creacion
                       }).ToList();


            return Ok(DamfoDto.Damfo290);
        }
    }
}
