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
            var damfo290 = db.DAMFO290.Select(df => new
            {
                df.Id,
                Cliente = db.Clientes.Where(c => c.Id == df.ClienteId).Select(a => new
                {
                    a.RazonSocial,
                    GiroEmpresas = db.GirosEmpresas.Where(ge => ge.Id.Equals(df.Cliente.GiroEmpresaId)).FirstOrDefault(),
                    ActividadEmpresas = db.ActividadesEmpresas.Where(ae => ae.Id.Equals(df.Cliente.ActividadEmpresaId)).FirstOrDefault()
                }).FirstOrDefault(),
                df.NombrePerfil,
                TipoReclutamiento = db.TiposReclutamientos.Where(tr => tr.Id.Equals(df.TipoReclutamientoId)).FirstOrDefault(),
                ClaseReclutamiento = db.ClasesReclutamientos.Where(cr => cr.Id.Equals(df.ClaseReclutamientoId)).FirstOrDefault(),
                df.SueldoMinimo,
                df.SueldoMaximo,
                df.fch_Creacion

            }).ToList();

            //DamfoDto.Damfo290Gral = (from damfo in db.DAMFO290
            //                         join cliente in db.Clientes on damfo.ClienteId equals cliente.Id
            //                         join giroEmpresa in db.GirosEmpresas on cliente.GiroEmpresaId equals giroEmpresa.Id
            //                         join actividadEmpresa in db.ActividadesEmpresas on giroEmpresa.Id equals actividadEmpresa.Id
            //                         join tipoReclutamiento in db.TiposReclutamientos on damfo.TipoReclutamientoId equals tipoReclutamiento.Id
            //                         join claseReclutamiento in db.ClasesReclutamientos on damfo.ClaseReclutamientoId equals claseReclutamiento.Id
            //                         select new Damfo290GralDto
            //                         {
            //                             Id = damfo.Id,
            //                             Cliente = cliente.RazonSocial,
            //                             NombrePerfil = damfo.NombrePerfil,
            //                             GiroEmpresa = giroEmpresa.giroEmpresa,
            //                             ActividadEmpresa = actividadEmpresa.actividadEmpresa,
            //                             TipoReclutamiento = tipoReclutamiento.tipoReclutamiento,
            //                             ClaseReclutamiento = claseReclutamiento.clasesReclutamiento,
            //                             SueldoMinimo = damfo.SueldoMinimo,
            //                             SueldoMaximo = damfo.SueldoMaximo,
            //                             fch_Creacion = damfo.fch_Creacion
            //                         }).ToList();


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
