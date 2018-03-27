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
            DamfoDto.DamfoCmA = (from x in db.CompetenciaAreaPerfil
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoCmADto {
                                     CompetenciaId = x.CompetenciaId,
                                     Nivel = x.Nivel
                                 }).ToList();
            CompetenciaAreaRequi compArea = new CompetenciaAreaRequi();
            List<CompetenciaAreaRequi> compAreas = new List<CompetenciaAreaRequi>();
            foreach(DamfoCmADto cmA in DamfoDto.DamfoCmA){
                compArea.CompetenciaId = cmA.CompetenciaId;
                compArea.Nivel = cmA.Nivel;
                compAreas.Add(compArea);
            }
            #endregion
            #region Competencias Cardinal
            DamfoDto.DamfoCmC = (from x in db.CompetenciaCardinalPerfil
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoCmCDto{
                                     CompetenciaId = x.CompetenciaId,
                                     Nivel = x.Nivel
                                 }).ToList();
            CompetenciaCardinalRequi compCardinal = new CompetenciaCardinalRequi();
            List<CompetenciaCardinalRequi> compCardinales = new List<CompetenciaCardinalRequi>();
            foreach (DamfoCmCDto cmR in DamfoDto.DamfoCmC)
            {
                compCardinal.CompetenciaId = cmR.CompetenciaId;
                compCardinal.Nivel = cmR.Nivel;
                compCardinales.Add(compCardinal);
            }
            #endregion
            #region Competencias Gerencial
            DamfoDto.DamfoCmG = (from x in db.CompetenciaGerencialPerfil
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoCmGDto
                                 {
                                     CompetenciaId = x.CompetenciaId,
                                     Nivel = x.Nivel
                                 }).ToList();
            CompetenciaGerencialRequi compGerencial = new CompetenciaGerencialRequi();
            List<CompetenciaGerencialRequi> compGerenciales = new List<CompetenciaGerencialRequi>();
            foreach (DamfoCmGDto cmG in DamfoDto.DamfoCmG)
            {
                compGerencial.CompetenciaId = cmG.CompetenciaId;
                compGerencial.Nivel = cmG.Nivel;
                compGerenciales.Add(compGerencial);
            }
            #endregion
            #region Documentos Cliente
            DamfoDto.DamfoDoc = (from x in db.DocumentosClientes
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoDocDto
                                 {
                                     Documento = x.Documento
                                 }).ToList();
            DocumentosClienteRequi documento = new DocumentosClienteRequi();
            List<DocumentosClienteRequi> documentos = new List<DocumentosClienteRequi>();
            foreach (DamfoDocDto doc in DamfoDto.DamfoDoc)
            {
                documento.Documento = doc.Documento;
                documentos.Add(documento);
            }
            #endregion
            #region Horarios
            DamfoDto.DamfoHor = (from x in db.HorariosPerfiles
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoHorDto
                                 {
                                     Nombre = x.Nombre,
                                     deDiaId = x.deDiaId,
                                     aDiaId = x.aDiaId,
                                     deHora = x.deHora,
                                     aHora = x.aHora,
                                     numeroVacantes = x.numeroVacantes,
                                     Especificaciones = x.Especificaciones,
                                     Activo = x.Activo
                                 }).ToList();
            HorarioRequi horario = new HorarioRequi();
            List<HorarioRequi> horarios = new List<HorarioRequi>();
            foreach (DamfoHorDto hor in DamfoDto.DamfoHor)
            {
                horario.Nombre = hor.Nombre;
                horario.deDiaId = hor.deDiaId;
                horario.aDiaId = hor.aDiaId;
                horario.deHora = hor.deHora;
                horario.aHora = hor.aHora;
                horario.numeroVacantes = hor.numeroVacantes;
                hor.Especificaciones = hor.Especificaciones;
                hor.Activo = hor.Activo;
                horarios.Add(horario);
            }
            #endregion
            #region Observaciones
            DamfoDto.DamfoObs = (from x in db.ObservacionesPerfil
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoObsDto
                                 {
                                     Observaciones = x.Observaciones
                                 }).ToList();
            ObservacionesRequi observacion = new ObservacionesRequi();
            List<ObservacionesRequi> observaciones = new List<ObservacionesRequi>();
            foreach (DamfoObsDto obs in DamfoDto.DamfoObs)
            {
                observacion.Observaciones = obs.Observaciones;
                observaciones.Add(observacion);
            }
            #endregion
            #region Prestaciones
            DamfoDto.DamfoPre = (from x in db.PrestacionesClientePerfil
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoPreDto
                                 {
                                     Prestamo = x.Prestamo
                                 }).ToList();
            PrestacionesClienteRequi prestacion = new PrestacionesClienteRequi();
            List<PrestacionesClienteRequi> prestaciones = new List<PrestacionesClienteRequi>();
            foreach (DamfoPreDto pres in DamfoDto.DamfoPre)
            {
                prestacion.Prestamo = pres.Prestamo;
                prestaciones.Add(prestacion);
            }
            #endregion
            #region Proceso
            DamfoDto.DamfoPro = (from x in db.ProcesosPerfil
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoProDto
                                 {
                                     Proceso = x.Proceso,
                                     Orden = x.Orden
                                 }).ToList();
            ProcesoRequi proceso = new ProcesoRequi();
            List<ProcesoRequi> procesos = new List<ProcesoRequi>();
            foreach (DamfoProDto pro in DamfoDto.DamfoPro)
            {
                proceso.Proceso = pro.Proceso;
                proceso.Orden = pro.Orden;
                procesos.Add(proceso);
            }
            #endregion
            #region Psicometrias Cliente
            DamfoDto.DamfoPsC = (from x in db.PsicometriasCliente
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoPsCDto
                                 {
                                     Psicometria = x.Psicometria,
                                     Descripcion = x.Descripcion
                                 }).ToList();
            PsicometriasClienteRequi pscCliente = new PsicometriasClienteRequi();
            List<PsicometriasClienteRequi> pscClientes = new List<PsicometriasClienteRequi>();
            foreach (DamfoPsCDto psc in DamfoDto.DamfoPsC)
            {
                pscCliente.Psicometria = psc.Psicometria;
                pscCliente.Descripcion = psc.Descripcion;
                pscClientes.Add(pscCliente);
            }
            #endregion
            #region Psicometrias Damsa
            DamfoDto.DamfoPsD = (from x in db.PsicometriasDamsa
                                 where x.DAMFO290Id.Equals(Id)
                                 select new DamfoPsDDto
                                 {
                                     PsicometriaId = x.PsicometriaId,
                                 }).ToList();
            PsicometriasDamsaRequi pscDamsa = new PsicometriasDamsaRequi();
            List<PsicometriasDamsaRequi> pscDamsas = new List<PsicometriasDamsaRequi>();
            foreach (DamfoPsDDto psc in DamfoDto.DamfoPsD)
            {
                pscDamsa.PsicometriaId = psc.PsicometriaId;
                pscDamsas.Add(pscDamsa);
            }
            #endregion

            requisicion.escolaridadesRequi = escolaridades;
            requisicion.aptitudesRequi = aptitudes;
            requisicion.actividadesRequi = actividades;
            requisicion.beneficiosRequi = beneficios;
            requisicion.competenciasAreaRequi = compAreas;
            requisicion.competenciasCardinalRequi = compCardinales;
            requisicion.competetenciasGerencialRequi = compGerenciales;
            requisicion.documentosClienteRequi = documentos;
            requisicion.horariosPerfil = horarios;
            requisicion.observacionesRequi = observaciones;
            requisicion.prestacionesClienteRequi = prestaciones;
            requisicion.procesoRequi = procesos;
            requisicion.psicometriasClienteRequi = pscClientes;
            requisicion.psicometriasDamsaRequi = pscDamsas;

            //resuisicion.escolaridadesRequi =  esco;
            return Ok(requisicion);
        }
    }
}