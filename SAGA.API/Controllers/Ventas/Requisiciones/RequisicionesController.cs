using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Requisiciones")]
    public class RequisicionesController : ApiController
    {
        private SAGADBContext db;
        Damfo290Dto DamfoDto;
        Requisicion requisicion;

        public RequisicionesController()
        {
            db = new SAGADBContext();
            DamfoDto = new Damfo290Dto();
            requisicion = new Requisicion();
        }

        [HttpGet]
        [Route("getDamfos")]
        public IHttpActionResult Get()
        {


            DamfoDto.Damfo290Gral = (from damfo in db.DAMFO290
                                 join cliente in db.Clientes on damfo.ClienteId equals cliente.Id
                                 join giroEmpresa in db.GirosEmpresas on cliente.GiroEmpresaId equals giroEmpresa.Id
                                 join actividadEmpresa in db.ActividadesEmpresas on giroEmpresa.Id equals actividadEmpresa.Id
                                 join tipoReclutamiento in db.TiposReclutamientos on damfo.TipoReclutamientoId equals tipoReclutamiento.Id
                                 join claseReclutamiento in db.ClasesReclutamientos on damfo.ClaseReclutamientoId equals claseReclutamiento.Id
                                 select new Damfo290GralDto
                                 {
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


            return Ok(DamfoDto.Damfo290Gral);
        }


        //api/Requisiciones/clonDamfo
        [HttpGet]
        [Route("clonDamfo")]
        public IHttpActionResult Clon(Guid Id)
        {
            try
            {
                var damfoId = new SqlParameter("@Id", Id);
                var requi = db.Database.SqlQuery<Requisicion>("exec createRequisicion @Id", damfoId).SingleOrDefault();
                Guid RequisicionId = requi.Id;
                return Ok(RequisicionId);
            }
            catch(Exception ex)
            {
                string messg = ex.Message;
                return Ok(messg);
            }
            
        }

        private void Save()
        {
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    // Update the values of the entity that failed to save from the store 
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }
    }
}