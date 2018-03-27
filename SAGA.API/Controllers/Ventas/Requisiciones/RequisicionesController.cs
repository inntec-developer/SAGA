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
            #region Escolaridades 
            DamfoDto.DamfoEsc = (from x in db.EscolaridadesPerfil
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoEscDto
                                 {
                                     EscolaridadId = x.EscolaridadId,
                                     EstadoEstudioId = x.EstadoEstudioId
                                 }).ToList();
            EscolaridadesRequi escolaridad = new EscolaridadesRequi();
            List<EscolaridadesRequi> escolaridades = new List<EscolaridadesRequi>();
            foreach (DamfoEscDto esc in DamfoDto.DamfoEsc)
            {
                escolaridad.EscolaridadId = esc.EscolaridadId;
                escolaridad.EstadoEstudioId = esc.EstadoEstudioId;
                escolaridades.Add(escolaridad);
            }
            #endregion
            #region Aptitudes 
            DamfoDto.DamfoApt = (from x in db.AptitudesPerfil
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoAptDto
                                 {
                                     AptitudId = x.AptitudId,
                                 }).ToList();
            AptitudesRequi aptitud = new AptitudesRequi();
            List<AptitudesRequi> aptitudes = new List<AptitudesRequi>();
            foreach (DamfoAptDto apt in DamfoDto.DamfoApt)
            {
                aptitud.AptitudId = apt.AptitudId;
                aptitudes.Add(aptitud);
            }
            #endregion
            #region Actividades 
            DamfoDto.DamfoAct = (from x in db.ActividadesPerfil
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoActDto
                                 {
                                     Actividades = x.Actividades,
                                 }).ToList();
            ActividadesRequi actividad = new ActividadesRequi();
            List<ActividadesRequi> actividades = new List<ActividadesRequi>();
            foreach (DamfoActDto act in DamfoDto.DamfoAct)
            {
                actividad.Actividades = act.Actividades;
                actividades.Add(actividad);
            }
            #endregion
            #region Beneficios 
            DamfoDto.DamfoBen = (from x in db.BeneficiosPerfil
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoBenDto
                                 {
                                     Cantidad = x.Cantidad,
                                     Observaciones = x.Observaciones,
                                     TipoBeneficioId = x.TipoBeneficioId
                                 }).ToList();
            BeneficiosRequi beneficio = new BeneficiosRequi();
            List<BeneficiosRequi> beneficios = new List<BeneficiosRequi>();
            foreach (DamfoBenDto ben in DamfoDto.DamfoBen)
            {
                beneficio.Cantidad = ben.Cantidad;
                beneficio.TipoBeneficioId = ben.TipoBeneficioId;
                beneficio.Observaciones = ben.Observaciones;
                beneficios.Add(beneficio);
            }
            #endregion

            #region Competencias Areas
            )
            #endregion

            #region Competencias Region
            
            #endregion


            requisicion.escolaridadesRequi = escolaridades;
            requisicion.aptitudesRequi = aptitudes;
            requisicion.actividadesRequi = actividades;
            requisicion.beneficiosRequi = beneficios;

            //resuisicion.escolaridadesRequi =  esco;
            return Ok(requisicion);
        }
    }
}