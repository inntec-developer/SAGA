using SAGA.BOL;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace SAGA.DAL
{
    public class SAGADBContext : DbContext
    {
        public SAGADBContext() : base("SAGADB")
        {
           // Database.SetInitializer<SAGADBContext>(null);
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SAGADBContext, Migrations.Configuration>());
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }

        #region Btra
        public DbSet<AboutMe> AcercaDeMi { get; set; }
        public DbSet<ActividadEmpresa> ActividadesEmpresas { get; set; }
        public DbSet<AreaExperiencia> AreasExperiencia { get; set; }
        public DbSet<AreaInteres> AreasInteres { get; set; }
        public DbSet<Candidato> Candidatos { get; set; }
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<ConocimientoOHabilidad> Conocimientos { get; set; }
        public DbSet<Certificacion> Certificaciones { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<DocumentoValidador> DocumentosValidadores { get; set; }
        public DbSet<ExperienciaProfesional> ExperienciasProfesionales { get; set; }
        public DbSet<FormaContacto> FormasContacto { get; set; }
        public DbSet<FormulariosIniciales> FormulariosIniciales { get; set; }
        public DbSet<InstitucionEducativa> InstitucionesEducativas { get; set; }
        public DbSet<Nivel> Niveles { get; set; }
        public DbSet<PerfilCandidato> PerfilCandidato { get; set; }
        public DbSet<PerfilExperiencia> PerfilExperiencia { get; set; }
        public DbSet<Formacion> Formaciones { get; set; }
        public DbSet<Idioma> Idiomas { get; set; }
        public DbSet<Month> Months { get; set; }
        public DbSet<PerfilIdioma> PerfilesIdiomas { get; set; }
        public DbSet<Porcentage> Porcentages { get; set; }
        public DbSet<Postulacion> Postulaciones { get; set; }
        public DbSet<StatusPostulacion> StatusPostulaciones { get; set; }
        public DbSet<TipoDiscapacidad> TiposDiscapacidades { get; set; }
        public DbSet<TipoLicencia> TiposLicencias { get; set; }
        public DbSet<Year> Years { get; set; }
        public DbSet<Frecuencias> Frecuencias { get; set; }
        public DbSet<Alertashdr> Alertas { get; set; }
        public DbSet<Alertasdtl> Alertasdtl { get; set; }
        public DbSet<DpTpDiscapacidad> DpTpDiscapacidad { get; set; }
        public DbSet<MiCVUpload> MiCVUpload { get; set; }
        public DbSet<HorariosCalendario> HorariosCalendario { get; set; }
        public DbSet<CalendarioCandidato> CalendarioCandidato { get; set; }
        public DbSet<AvancePerfil> AvancePerfil { get; set; }
        #endregion

        #region Recl
        public DbSet<ActividadesPerfil> ActividadesPerfil { get; set; }
        public DbSet<ActividadesPerfiles> ActividadesPerfiles { get; set; }
        public DbSet<AptitudesPerfil> AptitudesPerfil { get; set; }
        public DbSet<Aptitud> Aptitudes { get; set; }
        public DbSet<BeneficiosPerfil> BeneficiosPerfil { get; set; }
        public DbSet<ClaseReclutamiento> ClasesReclutamientos { get; set; }
        public DbSet<CompetenciaArea> CompetenciasAreas { get; set; }
        public DbSet<CompetenciaCardinal> CompetenciasCardinales { get; set; }
        public DbSet<CompetenciaGarencial> CompetenciasGerenciales { get; set; }
        public DbSet<CompetenciaAreaPerfil> CompetenciaAreaPerfil { get; set; }
        public DbSet<CompetenciaCardinalPerfil> CompetenciaCardinalPerfil { get; set; }
        public DbSet<CompetenciaGerencialPerfil> CompetenciaGerencialPerfil { get; set; }
        public DbSet<DiaObligatorio> DiasOblicatorios { get; set; }
        public DbSet<DiaSemana> DiasSemanas { get; set; }
        public DbSet<DocumentosCliente> DocumentosClientes { get; set; }
        public DbSet<DocumentosDamsa> DocumentosDamsa { get; set; }
        public DbSet<DAMFO_290> DAMFO290 { get; set; }
        public DbSet<EscolaridadesPerfil> EscolaridadesPerfil { get; set; }
        public DbSet<HorarioPerfil> HorariosPerfiles { get; set; }
        public DbSet<JornadaLaboral> JornadasLaborales { get; set; }
        public DbSet<ObservacionesPerfil> ObservacionesPerfil { get; set; }
        public DbSet<PrestacionesClientePerfil> PrestacionesClientePerfil { get; set; }
        public DbSet<PeriodoPago> PeriodosPagos { get; set; }
        public DbSet<PrestacionLey> PrestacionesLey { get; set; }
        public DbSet<ProcesoCandidato> ProcesoCandidatos { get; set; }
        public DbSet<ProcesoCampo> ProcesoCampo { get; set; }
        public DbSet<ProcesoPerfil> ProcesosPerfil { get; set; }
        public DbSet<PsicometriasCliente> PsicometriasCliente { get; set; }
        public DbSet<PsicometriasDamsa> PsicometriasDamsa { get; set; }
        public DbSet<RutasPerfil> RutasPerfil { get; set; }
        public DbSet<TiempoContrato> TiemposContratos { get; set; }
        public DbSet<TipoBeneficio> TiposBeneficios { get; set; }
        public DbSet<TipoContrato> TiposContrato { get; set; }
        public DbSet<TipoEmpresa> TiposEmpresas { get; set; }
        public DbSet<TipoModalidad> TiposModalidades { get; set; }
        public DbSet<TipodeNomina> TiposNominas { get; set; }
        public DbSet<TipoPsicometria> TiposPsicometrias { get; set; }
        public DbSet<TipoReclutamiento> TiposReclutamientos { get; set; }
        public DbSet<CfgRequi> CfgRequi { get; set; }
        public DbSet<ComentarioEntrevista> ComentariosEntrevistas { get; set; }
        public DbSet<ComentarioVacante> ComentariosVacantes { get; set; }
        public DbSet<CandidatoLiberado> CandidatosLiberados { get; set; }
        public DbSet<Medios> Medios { get; set; }
        public DbSet<TiposMedios> TiposMedios { get; set; }
        public DbSet<CandidatosInfo> CandidatosInfo { get; set; }
        public DbSet<FolioIncidenciasCandidatos> FoliosIncidendiasCandidatos { get; set; }
        public DbSet<OficioRequisicion> OficiosRequisicion { get; set; }
        public DbSet<PonderacionRequisiciones> PonderacionRequisiciones { get; set; }
        public DbSet<HistoricoTransDamfo> HistoricoTransDamfo { get; set; }
        public DbSet<DocumentosCandidato> DocumentosCandidato { get; set; }
        public DbSet<PerfilesDamfo> PerfilesDamfo { get; set; }
        public DbSet<PerfilDamfoRel> PerfilDamfoRel { get; set; }

       
        #endregion

        #region  ASIG
        public DbSet<PeriodoCompensaciones> PeriodoCompensaciones { get; set; }
        public DbSet<PeriodoHorasExtras> PeriodoHorasExtras { get; set; }
        public DbSet<PeriodoReconocimiento> PeriodoReconocimiento { get; set; }
        public DbSet<PeriodoMemo> PeriodoMemo { get; set; }
        public DbSet<PeriodoActa> PeriodoActa { get; set; }
        public DbSet<PeriodoSuspension> PeriodoSuspension { get; set; }
        public DbSet<PeriodoGuardia> PeriodoGuardia { get; set; }
        public DbSet<PeriodoPermisos> PeriodoPermisos { get; set; }
        public DbSet<PeriodoIncapacidad> PeriodoIncapacidad { get; set; }
        public DbSet<PeriodoDE> PeriodoDE { get; set; }
        public DbSet<PeriodoVacaciones> PeriodoVacaciones { get; set; }
        public DbSet<PeriodoBonos> PeriodoBonos { get; set; }
        #endregion

        #region GePe Gestion Personal
        // Captura
        public DbSet<CandidatosGenerales> CandidatoGenerales { get; set; }
        public DbSet<CandidatosExtras> CandidatoExtras { get; set; }
        public DbSet<CandidatoLaborales> CandidatoLaborales { get; set; }
        public DbSet<Gafetes> Gafetes { get; set; }
        public DbSet<ValidacionCURPRFC> ValidacionCURPRFC { get; set; }
        public DbSet<BiometricosFP> BiometricosFP { get; set; }

        public DbSet<JustificacionTrabajo> JustificacionTrabajo { get; set; }
        public DbSet<Sucursales> Sucursales { get; set; }
        public DbSet<RegistroPatronal> RegistroPatronal { get; set; }
        public DbSet<PuestosCliente> PuestosCliente { get; set; }
        public DbSet<EstatusLaboral> EstatusLaboral { get; set; }
        public DbSet<SoporteFacturacion> SoporteFacturacion { get; set; }
        public DbSet<SoporteSucursales> SoporteSucursales { get; set; }
        public DbSet<SoportePuestos> SoportePuestos { get; set; }
        public DbSet<SoporteDptoIngresos> SoporteDptosIngresos { get; set; }
        public DbSet<EmpleadosSoporte> EmpleadosSoporte { get; set; }
        public DbSet<DiasFestivos> DiasFestivos { get; set; }
        public DbSet<HorariosIngresos> HorariosIngresos { get; set; }
        public DbSet<DiasHorasIngresos> DiasHorasIngresos { get; set; }
        public DbSet<DiasHorasEspecial> DiasHorasEspecial { get; set; }
        public DbSet<TurnosHorarios> TurnosHorarios { get; set; }
        public DbSet<TiposBono> TiposBono { get; set; }
        public DbSet<TipoPeriodos> TipoPeriodos { get; set; }

        ///ingresos
        
        public DbSet<DptosIngresos> DptosIngresos { get; set; }
        public DbSet<CatalogoClientes> CatalogoClientes { get; set; }
        public DbSet<PuestosIngresos> PuestosIngresos { get; set; }
        public DbSet<CatalogoBancos> CatalogoBancos { get; set; }
        public DbSet<EmpresaBancos> EmpresaBancos { get; set; }
        public DbSet<MotivosContratacion> MotivosContratacion { get; set; }
        public DbSet<GrupoSanguineo> GrupoSanguineo { get; set; }
        public DbSet<FormaPago> FormaPago { get; set; }
        public DbSet<TipoDocumentos> TiposDocumentos { get; set; }
        public DbSet<Documentos> Documentos { get; set; }

        ///modulo configuraciones
        public DbSet<TiposConfiguraciones> TiposConfiguraciones { get; set; }
        public DbSet<TiempoAntiguedad> TiempoAntiguedad { get; set; }
        public DbSet<ConfigVacaciones> ConfigVacaciones { get; set; }
        public DbSet<ConfigVacacionesDias> ConfigVacacionesDias { get; set; }
        public DbSet<EmpleadoVacaciones> GrupoVacaciones { get; set; }
        public DbSet<ConfigIncapacidades> ConfigIncapacidades { get; set; }
        public DbSet<ConfigIncapacidadesDias> ConfigIncapacidadesDias { get; set; }
        public DbSet<TiposIncapacidad> TiposIncapacidad { get; set; }
        public DbSet<EmpleadoIncapacidad> EmpleadoIncapacidad { get; set; }
        public DbSet<ConfigTiempoExtra> ConfigTiempoExtra { get; set; }
        public DbSet<EmpleadoTiempoExtra> EmpleadoTiempoExtra { get; set; }
        public DbSet<ConfigSuspensionNotas> ConfigSuspensionNotas { get; set; }
        public DbSet<ConfigSuspensionNotasDias> ConfigSuspensionNotasDias { get; set; }
        public DbSet<EmpleadoSuspension> EmpleadoSuspension { get; set; }
        public DbSet<ConfigGuardias> ConfigGuardias { get; set; }
        public DbSet<EmpleadoGuardia> EmpleadoGuardia { get; set; }
        public DbSet<TiposDiasEconomicos> TiposDiasEconomicos { get; set; }
        public DbSet<ConfigDiasEconomicos> ConfigDiasEconomicos { get; set; }
        public DbSet<ConfigDiasEconomicosDias> ConfigDiasEconomicosDias { get; set; }
        public DbSet<EmpleadoDiasEconomicos> EmpleadoDiasEconomicos { get; set; }
        public DbSet<ConfigPrima> ConfigPrima { get; set; }
        public DbSet<EmpleadoPrima> EmpleadoPrima { get; set; }
        public DbSet<ConfigTolerancia> ConfigTolerancia { get; set; }
        public DbSet<ConfigToleranciaTiempo> ConfigToleranciaTiempo { get; set; }
        public DbSet<EmpleadoTolerancia> EmpleadoTolerancia { get; set; }
        public DbSet<EmpleadoHorario> EmpleadoHorario { get; set; }
        public DbSet<GrupoEmpleados> GrupoEmpleados { get; set; }
        public DbSet<ConfigBono> ConfigBono { get; set; }
        public DbSet<EmpleadoBono> EmpleadoBono { get; set; }

        // checador
        public DbSet<Jornada> Jornada { get; set; }
        #endregion
        #region Sist
        public DbSet<RelacionClientesSistemas> RelacionClientesSistemas { get; set; }
        public DbSet<Ambito> Ambitos { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<Colonia> Colonias { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<EstadoCivil> EstadosCiviles { get; set; }
        public DbSet<EstadoEstudio> EstadosEstudios { get; set; }
        public DbSet<Estructura> Estructuras { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<GradoEstudio> GradosEstudios { get; set; }
        public DbSet<GiroEmpresa> GirosEmpresas { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Entidad> Entidad { get; set; }
        public DbSet<Privilegio> Privilegios { get; set; }
        public DbSet<Telefono> Telefonos { get; set; }
        public DbSet<TipoTelefono> TiposTelefonos { get; set; }
        public DbSet<TipoDireccion> TiposDirecciones { get; set; }
        public DbSet<TipoEstructura> TipoEstructuras { get; set; }
        public DbSet<TipoRedSocial> TiposRedesSociales { get; set; }
        public DbSet<TipoAccion> TiposAcciones { get; set; }
        public DbSet<TipoMovimiento> TiposMovimientos { get; set; }
        public DbSet<TrazabilidadMes> TrazabilidadesMes { get; set; }
        public DbSet<RastreabilidadMes> RastreabilidadMes { get; set; }
        public DbSet<Folio> Folios { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<TipoEntidad> TiposEntidades { get; set; }
        public DbSet<Tratamiento> Tratamientos { get; set; }
        public DbSet<PostNombre> PostNombres { get; set; }
        public DbSet<RolEntidad> RolEntidades { get; set; }
        public DbSet<ConfiguracionMovs> ConfiguracionesMov { get; set; }
        public DbSet<LogsIngresos> LogsIngresos { get; set; }
        public DbSet<MotivoLiberacion> MotivosLiberacion { get; set; }
        public DbSet<FolioIncidencia> FolioIncidencia { get; set; }
        public DbSet<Puesto> Puestos { get; set; }
        public DbSet<CalendarioEvent> CalendarioEvent { get; set; }
        public DbSet<TipoActividadReclutador> TipoActividadReclutador { get; set; }
        public DbSet<AlertasStm> AlertasStm { get; set; }
        public DbSet<TipoAlerta> TiposAlertas { get; set; }
        public DbSet<OficinaReclutamiento> OficinasReclutamiento { get; set; }
        public DbSet<UnidadNegocio> UnidadesNegocios { get; set; }
        public DbSet<UnidadNegocioEstados> UnidadNegocioEstados { get; set; } 
        public DbSet<Catalogos> Catalogos { get; set; }
        public DbSet<LogCatalogos> LogCatalogos { get; set; }
        public DbSet<Transferencias> Transferencias { get; set; }
        public DbSet<TiposTransferencias> TiposTransferencias { get; set; }
        public DbSet<TipoExamenMedico> TiposExamenMedico { get; set; }
        public DbSet<VertionSistem> VertionSistem { get; set; }
        public DbSet<TitulosArte> TitulosArte { get; set; }
        public DbSet<ArteRequi> ArteRequi { get; set; }


        ///modulo admin 
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Grupos> Grupos { get; set; }
        public DbSet<Estatus> Estatus { get; set; }
        public DbSet<Prioridad> Prioridades { get; set; }
        public DbSet<Subordinados> Subordinados { get; set; }

        /// modulo para examenes
        public DbSet<Examenes> Examenes { get; set; }
        public DbSet<Entrevista> Entrevistas { get; set; }
        public DbSet<TipoExamen> TipoExamen { get; set; }
        public DbSet<Preguntas> Preguntas { get; set; }
        public DbSet<Respuestas> Respuestas { get; set; }
        public DbSet<RequiExamen> RequiExamen { get; set; }
        public DbSet<ExamenCandidato> ExamenCandidato { get; set; }
        public DbSet<ResultadosCandidato> resultadocandidato { get; set; }
        public DbSet<RequiClaves> RequiClaves { get; set; }
        public DbSet<PsicometriaCandidato> PsicometriaCandidato { get; set; }
        public DbSet<MedicoCandidato> MedicosCandidato { get; set; }
        public DbSet<ConfigEntrevista> ConfigEntrevistas { get; set; }
        // Preguntas Frecuentes
        public DbSet<PreguntasFrecuente> PreguntasFrecuentes { get; set; }

        #endregion

        #region FIRMAS
        public DbSet<Empresas> Empresas { get; set; }
        public DbSet<FIRM_RP_Empresas> FIRM_RPEmpresas { get; set; }
        public DbSet<FIRM_EstatusBitacora> FIRM_EstatusBitacora { get; set; }
        public DbSet<FIRM_SoportesNomina> FIRM_SoportesNomina { get; set; }
        public DbSet<FIRM_SoporteSucursal> FIRM_SoporteSucursal { get; set; }
        public DbSet<FIRM_ConfigBitacora> FIRM_ConfigBitacora { get; set; }
        public DbSet<FIRM_Bitacora> FIRM_Bitacora { get; set; }
        public DbSet<FIRM_RP> FIRM_RP { get; set; }
        public DbSet<FIRM_Ishikawa> FIRM_Ishikawa { get; set; }
        public DbSet<FIRM_CausaEfecto> FIRM_CausaEfecto { get; set; }
        public DbSet<FIRM_Porques> FIRM_Porques { get; set; }
        public DbSet<FIRM_Compromiso> FIRM_Compromiso { get; set; }
        public DbSet<FIRM_Damfo022> FIRM_Damfo022 { get; set; }
        public DbSet<FIRM_FechasEstatus> FIRM_FechasEstatus { get; set; }
        public DbSet<FIRM_EstatusNomina> FIRM_EstatusNomina { get; set; }
        public DbSet<FIRM_BitacoraNomina> FIRM_BitacoraNomina { get; set; }
        public DbSet<FIRM_EstatusEmails> FIRM_EstatusEmails { get; set; }
        #endregion

        #region Vtas
        public DbSet<Requisicion> Requisiciones { get; set; }
        public DbSet<EscolaridadesRequi> EscolaridadesRequis { get; set; }
        public DbSet<AptitudesRequi> AptitudesRequis { get; set; }
        public DbSet<HorarioRequi> HorariosRequis { get; set; }
        public DbSet<ActividadesRequi> ActividadesRequis { get; set; }
        public DbSet<ObservacionesRequi> ObservacionesRequis { get; set; }
        public DbSet<PsicometriasDamsaRequi> PsicometriasDamsaRequis { get; set; }
        public DbSet<PsicometriasClienteRequi> PsicometriasClienteRequis { get; set; }
        public DbSet<BeneficiosRequi> BeneficiosRequis { get; set; }
        public DbSet<DocumentosClienteRequi> DocumentosClienteRequis { get; set; }
        public DbSet<ProcesoRequi> ProcesoRequis { get; set; }
        public DbSet<PrestacionesClienteRequi> PrestacionesClienteRequis { get; set; }
        public DbSet<CompetenciaAreaRequi> CompetenciasAreaRequis { get; set; }
        public DbSet<CompetenciaCardinalRequi> CompetenciasCardinalRequis { get; set; }
        public DbSet<CompetenciaGerencialRequi> CompetetenciasGerencialRequis { get; set; }
        public DbSet<AsignacionRequi> AsignacionRequis { get; set; }
        public DbSet<TipoUsuario> TiposUsuarios { get; set; }
        public DbSet<HorariosDireccionesRequi> HorariosDireccionesRequi { get; set; }
        public DbSet<InformeRequisicion> InformeRequisiciones { get; set; }
        public DbSet<InformeCandidatos> InformeCandidatos { get; set; }
        public DbSet<EstatusRequisiciones> EstatusRequisiciones { get; set; }
        public DbSet<FacturacionPuro> FacturacionPuro { get; set; }

        public DbSet<Agencia> Agencias { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ConfiguracionRequi> ConfiguracionRequis { get; set; }
        public DbSet<Contacto> Contactos { get; set; }
        public DbSet<RedSocial> RedesSociales { get; set; }
        public DbSet<Referenciado> Referenciados { get; set; }
        public DbSet<TamanoEmpresa> TamanoEmpresas { get; set; }
        public DbSet<TipoBase> TiposBases { get; set; }
        //public DbSet<ExamenMedicoCliente> ExamenesMedicosCliente { get; set; }

        // Relacion de Direccion - Emails - Teledonos - Contactos. Para clientes
        public DbSet<DireccionTelefono> DireccionesTelefonos { get; set; }
        public DbSet<DireccionEmail> DireccionesEmails { get; set; }
        public DbSet<DireccionContacto> DireccionesContactos { get; set; }

        // Costos
        public DbSet<Costos> Costos { get; set; }
        public DbSet<TipoCostos> TipoCostos { get; set; }
        public DbSet<CostosDamfo290> CostosDamfo290 { get; set; }
        #endregion

        #region SistTurnos
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketReclutador> TicketsReclutador { get; set; }
        public DbSet<ModulosReclutamiento> ModulosReclutamiento { get; set; }
        public DbSet<HistoricoTicket> HistoricosTickets { get; set; }

        #endregion



        /*
		 * Loging
		 */
        public DbSet<AspNetUsers> AspNetUsers { get; set; }
        #region Mapeo GEPE
        // checador
        public class JornadaMap : EntityTypeConfiguration<Jornada>
        {
            public JornadaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoInfoId).IsRequired();
                Property(x => x.Fecha).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.Dia).IsRequired();
                Property(x => x.Hora).HasColumnType("datetime").IsRequired();
                Property(x => x.Tipo).IsRequired();
            }
        }
        #region Mapeo ASIG
        public class PeriodoBonoMap : EntityTypeConfiguration<PeriodoBonos>
        {
            public PeriodoBonoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.fchIncio).HasColumnType("datetime").IsRequired();
                Property(x => x.fchFin).HasColumnType("datetime").IsRequired();
                Property(x => x.ConfigBonoId).IsRequired();
                Property(x => x.Porcentaje).IsRequired();
                Property(x => x.Comentario).IsRequired().HasMaxLength(300);
                Property(x => x.fchAlta).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioAltaId).IsRequired();
            }
        }
        public class PeriodoCompensacionesMap : EntityTypeConfiguration<PeriodoCompensaciones>
        {
            public PeriodoCompensacionesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.Fecha).HasColumnType("datetime").IsRequired();
                Property(x => x.Tipo).IsRequired();
                Property(x => x.Comentario).IsRequired().HasMaxLength(300);
                Property(x => x.fchAlta).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioAltaId).IsRequired();
            }
        }
        public class PeriodoHorasExtrasMap : EntityTypeConfiguration<PeriodoHorasExtras>
        {
            public PeriodoHorasExtrasMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.fchIncio).HasColumnType("datetime").IsRequired();
                Property(x => x.fchFin).HasColumnType("datetime").IsRequired();
                Property(x => x.Tiempo).IsRequired();
                Property(x => x.Comentario).IsRequired().HasMaxLength(300);
                Property(x => x.fchAlta).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioAltaId).IsRequired();
            }
        }
        public class PeriodoReconocimientoMap : EntityTypeConfiguration<PeriodoReconocimiento>
        {
            public PeriodoReconocimientoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.fchIncio).HasColumnType("datetime").IsRequired();
                Property(x => x.fchFin).HasColumnType("datetime").IsRequired();
                Property(x => x.Comentario).IsRequired().HasMaxLength(300);
                Property(x => x.fchAlta).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioAltaId).IsRequired();
            }
        }
        public class PeriodoMemoMap : EntityTypeConfiguration<PeriodoMemo>
        {
            public PeriodoMemoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.fchIncio).HasColumnType("datetime").IsRequired();
                Property(x => x.fchFin).HasColumnType("datetime").IsRequired();
                Property(x => x.Comentario).IsRequired().HasMaxLength(300);
                Property(x => x.Faltas).IsRequired();
                Property(x => x.Retardos).IsRequired();
                Property(x => x.fchAlta).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioAltaId).IsRequired();
            }
        }
        public class PeriodoActaMap : EntityTypeConfiguration<PeriodoActa>
        {
            public PeriodoActaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.fchIncio).HasColumnType("datetime").IsRequired();
                Property(x => x.fchFin).HasColumnType("datetime").IsRequired();
                Property(x => x.Comentario).IsRequired().HasMaxLength(300);
                Property(x => x.Faltas).IsRequired();
                Property(x => x.fchAlta).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioAltaId).IsRequired();
            }
        }
        public class PeriodoSuspensionMap : EntityTypeConfiguration<PeriodoSuspension>
        {
            public PeriodoSuspensionMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.fchIncio).HasColumnType("datetime").IsRequired();
                Property(x => x.fchFin).HasColumnType("datetime").IsRequired();
                Property(x => x.Comentario).IsRequired().HasMaxLength(300);
                Property(x => x.dias).IsRequired();
                Property(x => x.Tipo).IsRequired();
                Property(x => x.fchAlta).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioAltaId).IsRequired();
            }
        }
        public class PeriodoGuardiaMap : EntityTypeConfiguration<PeriodoGuardia>
        {
            public PeriodoGuardiaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.Fecha).HasColumnType("datetime").IsRequired();
                Property(x => x.Comentario).IsRequired().HasMaxLength(300);
                Property(x => x.CubridorId).IsRequired();
                Property(x => x.fchAlta).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioAltaId).IsRequired();
            }
        }
        public class PeriodoPermisosMap : EntityTypeConfiguration<PeriodoPermisos>
        {
            public PeriodoPermisosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.fchIncio).HasColumnType("datetime").IsRequired();
                Property(x => x.fchFin).HasColumnType("datetime").IsRequired();
                Property(x => x.dias).IsRequired();
                Property(x => x.Comentario).IsRequired().HasMaxLength(300);
                Property(x => x.Sueldo).IsRequired();
                Property(x => x.TipoJustificacionId).IsRequired();
                Property(x => x.Tipo).IsRequired();
                Property(x => x.fchAlta).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioAltaId).IsRequired();
            }
        }
        public class PeriodoIncapacidadMap : EntityTypeConfiguration<PeriodoIncapacidad>
        {
            public PeriodoIncapacidadMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.fchIncio).HasColumnType("datetime").IsRequired();
                Property(x => x.fchFin).HasColumnType("datetime").IsRequired();
                Property(x => x.dias).IsRequired();
                Property(x => x.Comentario).IsRequired().HasMaxLength(300);
                Property(x => x.SerieFolio).HasMaxLength(100).IsRequired();
                Property(x => x.Archivo).HasMaxLength(100).IsRequired();
                Property(x => x.TiposIncapacidadId).IsRequired();

                Property(x => x.fchAlta).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioAltaId).IsRequired();
            }
        }
        public class PeriodoDEMap : EntityTypeConfiguration<PeriodoDE>
        {
            public PeriodoDEMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.fchIncio).HasColumnType("datetime").IsRequired();
                Property(x => x.fchFin).HasColumnType("datetime").IsRequired();
                Property(x => x.dias).IsRequired();
                Property(x => x.Comentario).IsRequired().HasMaxLength(300);
                Property(x => x.TiposDiasEconomicosId).IsRequired();

                Property(x => x.fchAlta).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioAltaId).IsRequired();
            }
        }
        public class PeriodoVacacionesMap : EntityTypeConfiguration<PeriodoVacaciones>
        {
            public PeriodoVacacionesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.fchIncio).HasColumnType("datetime").IsRequired();
                Property(x => x.fchFin).HasColumnType("datetime").IsRequired();
                Property(x => x.dias).IsRequired();
                Property(x => x.Comentario).IsRequired().HasMaxLength(300);

                Property(x => x.fchAlta).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioAltaId).IsRequired();
            }
        }
        #endregion

        #region Mapeo Ingresos
        public class CatalogoClientesMap : EntityTypeConfiguration<CatalogoClientes>
        {
            public CatalogoClientesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CatalogosId).IsRequired();
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.Observaciones).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fchAlta).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fchModificacion).HasColumnType("datetime").IsRequired();
            }
        }
     
        public class DptosIngresosMap : EntityTypeConfiguration<DptosIngresos>
        {
            public DptosIngresosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.Clave).HasMaxLength(20).IsRequired();
                Property(x => x.Observaciones).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class TiposConfiguracionesMap : EntityTypeConfiguration<TiposConfiguraciones>
        {
            public TiposConfiguracionesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                Property(x => x.Descripcion).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }
        }
        public class TiempoAntiguedadMap : EntityTypeConfiguration<TiempoAntiguedad>
        {
            public TiempoAntiguedadMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Tiempo).IsRequired();
                Property(x => x.DiasLey).IsRequired();
                Property(x => x.Descripcion).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }
        }
        public class ConfigVacacionesMap : EntityTypeConfiguration<ConfigVacaciones>
        {
            public ConfigVacacionesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                Property(x => x.DiasExpiran).IsRequired();
                Property(x => x.DiasContinuos).IsRequired();
                Property(x => x.DiasIncremento).IsRequired();
                Property(x => x.Porcentaje).IsRequired();
                Property(x => x.Observaciones).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class ConfigVacacionesDiasMap : EntityTypeConfiguration<ConfigVacacionesDias>
        {
            public ConfigVacacionesDiasMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.TiempoAntiguedadId).IsRequired();
                Property(x => x.Dias).IsRequired();
                Property(x => x.ConfigVacacionesId).IsRequired();

            }
        }
        public class EmpleadoVacacionesMap : EntityTypeConfiguration<EmpleadoVacaciones>
        {
            public EmpleadoVacacionesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ConfigVacacionesId).IsRequired();
                Property(x => x.empleadoId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }
        }
        public class ConfigIncapacidadesMap : EntityTypeConfiguration<ConfigIncapacidades>
        {
            public ConfigIncapacidadesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                Property(x => x.Comentarios).IsRequired().HasMaxLength(200);
                Property(x => x.Activo).IsRequired();
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class ConfigIncapacidadesDiasMap : EntityTypeConfiguration<ConfigIncapacidadesDias>
        {
            public ConfigIncapacidadesDiasMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Dias).IsRequired();
                Property(x => x.Porcentaje).IsRequired();
                Property(x => x.TiposIncapacidadId).IsRequired();
                Property(x => x.ConfigIncapacidadesId).IsRequired();
            }
        }
        public class TiposIncapacidadMap : EntityTypeConfiguration<TiposIncapacidad>
        {
            public TiposIncapacidadMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                Property(x => x.Comentarios).IsRequired().HasMaxLength(200);
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class EmpleadoIncapacidadMap : EntityTypeConfiguration<EmpleadoIncapacidad>
        {
            public EmpleadoIncapacidadMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ConfigIncapacidadesId).IsRequired();
                Property(x => x.empleadoId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }
        }
        public class ConfigTiempoExtraMap : EntityTypeConfiguration<ConfigTiempoExtra>
        {
            public ConfigTiempoExtraMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Redondeo).IsRequired();
                Property(x => x.TE_Media).IsRequired();
                Property(x => x.TE_Hora).IsRequired();
                Property(x => x.TE_Dobles).IsRequired();
                Property(x => x.TE_Total).IsRequired();
                Property(x => x.Comentarios).IsRequired().HasMaxLength(200);
                Property(x => x.Activo).IsRequired();
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }
        }
        public class EmpleadoTiempoExtraMap : EntityTypeConfiguration<EmpleadoTiempoExtra>
        {
            public EmpleadoTiempoExtraMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ConfigTiempoExtraId).IsRequired();
                Property(x => x.empleadoId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class ConfigSuspensionNotasMap : EntityTypeConfiguration<ConfigSuspensionNotas>
        {
            public ConfigSuspensionNotasMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                Property(x => x.Comentarios).IsRequired().HasMaxLength(200);
                Property(x => x.Activo).IsRequired();
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class ConfigSuspensionNotasDiasMap : EntityTypeConfiguration<ConfigSuspensionNotasDias>
        {
            public ConfigSuspensionNotasDiasMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Dias).IsRequired();
                Property(x => x.Retardos).IsRequired();
                Property(x => x.Tipo).IsRequired();
                Property(x => x.ConfigSuspensionNotasId).IsRequired();
            }
        }
        public class EmpleadoSuspensionMap : EntityTypeConfiguration<EmpleadoSuspension>
        {
            public EmpleadoSuspensionMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ConfigSuspensionNotasId).IsRequired();
                Property(x => x.empleadoId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class ConfigGuardiasMap : EntityTypeConfiguration<ConfigGuardias>
        {
            public ConfigGuardiasMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.NoGuardias).IsRequired();
                Property(x => x.Consecutivas).IsRequired();
                Property(x => x.Comentarios).IsRequired().HasMaxLength(200);
                Property(x => x.Activo).IsRequired();
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }

        }
        public class EmpleadoGuardiaMap : EntityTypeConfiguration<EmpleadoGuardia>
        {
            public EmpleadoGuardiaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ConfigGuardiasId).IsRequired();
                Property(x => x.empleadoId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class TiposDiasEconomicosMap : EntityTypeConfiguration<TiposDiasEconomicos>
        {
            public TiposDiasEconomicosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                Property(x => x.Comentarios).IsRequired().HasMaxLength(200);
                Property(x => x.Activo).IsRequired();
                Property(x => x.Orden).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }

        }
        public class ConfigDiasEconomicosMap : EntityTypeConfiguration<ConfigDiasEconomicos>
        {
            public ConfigDiasEconomicosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                Property(x => x.Comentarios).IsRequired().HasMaxLength(200);
                Property(x => x.DiasConSueldo).IsRequired();
                Property(x => x.DiasSinSueldo).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }

        }
        public class ConfigDiasEconomicosDiasMap : EntityTypeConfiguration<ConfigDiasEconomicosDias>
        {
            public ConfigDiasEconomicosDiasMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Dias).IsRequired();
                Property(x => x.TiposDiasEconomicosId).IsRequired();
                Property(x => x.ConfigDiasEconomicosId).IsRequired();
            }

        }
        public class EmpleadoDiasEconomicosMap : EntityTypeConfiguration<EmpleadoDiasEconomicos>
        {
            public EmpleadoDiasEconomicosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ConfigDiasEconomicosId).IsRequired();
                Property(x => x.empleadoId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class ConfigPrimaMap : EntityTypeConfiguration<ConfigPrima>
        {
            public ConfigPrimaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Horas).IsRequired();
                Property(x => x.Observaciones).IsRequired().HasMaxLength(200);
                Property(x => x.Activo).IsRequired();
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }

        }
        public class EmpleadoPrimaMap : EntityTypeConfiguration<EmpleadoPrima>
        {
            public EmpleadoPrimaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ConfigPrimaId).IsRequired();
                Property(x => x.empleadoId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class ConfigToleranciaMap : EntityTypeConfiguration<ConfigTolerancia>
        {
            public ConfigToleranciaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                Property(x => x.Observaciones).IsRequired().HasMaxLength(200);
                Property(x => x.Activo).IsRequired();
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }

        }
        public class ConfigToleranciaTiempoMap : EntityTypeConfiguration<ConfigToleranciaTiempo>
        {
            public ConfigToleranciaTiempoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Tiempo).IsRequired();
                Property(x => x.Tipo).IsRequired();
                Property(x => x.ConfigToleranciaId).IsRequired();

            }

        }
        public class EmpleadoToleranciaMap : EntityTypeConfiguration<EmpleadoTolerancia>
        {
            public EmpleadoToleranciaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ConfigToleranciaId).IsRequired();
                Property(x => x.empleadoId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class EmpleadoHorarioMap : EntityTypeConfiguration<EmpleadoHorario>
        {
            public EmpleadoHorarioMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.HorariosIngresosId).IsRequired();
                Property(x => x.empleadoId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class GrupoEmpleadosMap : EntityTypeConfiguration<GrupoEmpleados>
        {
            public GrupoEmpleadosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.GrupoId).IsRequired();
                Property(x => x.EmpleadoId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class BiometricosFPMap : EntityTypeConfiguration<BiometricosFP>
        {
            public BiometricosFPMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.FingerPrint);
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class ConfigBonoMap : EntityTypeConfiguration<ConfigBono>
        {
            public ConfigBonoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.TiposBonoId).IsRequired();
                Property(x => x.PeriodosId).IsRequired();
                Property(x => x.Comentarios).IsRequired().HasMaxLength(300);
                Property(x => x.Activo).IsRequired();
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class EmpleadoBonoMap : EntityTypeConfiguration<EmpleadoBono>
        {
            public EmpleadoBonoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ConfigBonoId).IsRequired();
                Property(x => x.empleadoId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        #endregion

        #endregion


        #region "Mapeo Sist"
        public class TitulosArteMap : EntityTypeConfiguration<TitulosArte>
        {
            public TitulosArteMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(200).IsRequired();
                Property(x => x.Descripcion).HasMaxLength(400).IsRequired();
                Property(x => x.Orden).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class ArteRequiMap : EntityTypeConfiguration<ArteRequi>
        {
            public ArteRequiMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.TitulosArteId).IsRequired();
                Property(x => x.BG).HasMaxLength(200).IsRequired();
                Property(x => x.Contenido).HasMaxLength(500).IsRequired();
                Property(x => x.Ruta).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }

        public class TiposBonoMap : EntityTypeConfiguration<TiposBono>
        {
            public TiposBonoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.Comentario).HasMaxLength(300).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class TipoPeriodosMap : EntityTypeConfiguration<TipoPeriodos>
        {
            public TipoPeriodosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.Comentarios).HasMaxLength(300).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.Dias).IsRequired();
                Property(x => x.Meses).IsRequired();
                Property(x => x.Orden).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class AreaMap : EntityTypeConfiguration<Area>
        {
            public AreaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.Clave).HasMaxLength(20).IsRequired();
                Property(x => x.Observaciones).HasMaxLength(200).IsRequired();
                Property(x => x.Orden).IsOptional();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class JustificacionTrabajoMap : EntityTypeConfiguration<JustificacionTrabajo>
        {
            public JustificacionTrabajoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Descripcion).HasMaxLength(100).IsRequired();
                Property(x => x.Comentario).HasMaxLength(300).IsRequired();
                Property(x => x.Clave).HasMaxLength(20).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class SucursalesMap : EntityTypeConfiguration<Sucursales>
        {
            public SucursalesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(200).IsRequired();
                Property(x => x.Comentario).HasMaxLength(300).IsRequired();
                Property(x => x.Clave).HasMaxLength(30).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.RegistroPatronalId).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class RegistroPatronalMap : EntityTypeConfiguration<RegistroPatronal>
        {
            public RegistroPatronalMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RP_Clave).HasMaxLength(100).IsRequired();
                Property(x => x.RP_IMSS).HasMaxLength(100).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class PuestosIngresosMap : EntityTypeConfiguration<PuestosIngresos>
        {
            public PuestosIngresosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Clave).HasMaxLength(50).IsRequired();
                Property(x => x.Descripcion).HasMaxLength(200).IsRequired();
                Property(x => x.Nombre).HasMaxLength(200).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }
        }
        public class PuestosClienteMap : EntityTypeConfiguration<PuestosCliente>
        {
            public PuestosClienteMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.PuestosIngresosId).IsRequired();
            }
        }
        public class EstatusLaboralMap : EntityTypeConfiguration<EstatusLaboral>
        {
            public EstatusLaboralMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Estatus).HasMaxLength(100).IsRequired();
                Property(x => x.Comentario).HasMaxLength(300).IsRequired();
                Property(x => x.Clave).HasMaxLength(20).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class DiasFestivosMap : EntityTypeConfiguration<DiasFestivos>
        {
            public DiasFestivosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Descripcion).HasMaxLength(200).IsRequired();
                Property(x => x.Fecha).IsRequired();
                Property(x => x.Anio).IsRequired();
                Property(x => x.MesNombre).HasMaxLength(100).IsRequired();
                Property(x => x.MesNum).IsRequired();
                Property(x => x.DiaSemanaNombre).HasMaxLength(100).IsRequired();
                Property(x => x.DiaSemanaNum).IsRequired();
                Property(x => x.Comentario).HasMaxLength(200).IsRequired();
                Property(x => x.Clave).HasMaxLength(20).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.Tipo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class HorariosIngresosMap : EntityTypeConfiguration<HorariosIngresos>
        {
            public HorariosIngresosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.Especificaciones).HasMaxLength(200).IsRequired();
                Property(x => x.Clave).HasMaxLength(20).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.TurnosHorariosId).IsRequired();
                Property(x => x.HorasTotales).IsRequired();
                Property(x => x.HorasDescanso).IsRequired();
                Property(x => x.HorasComida).IsRequired();
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class TurnosHorariosMap : EntityTypeConfiguration<TurnosHorarios>
        {
            public TurnosHorariosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(50).IsRequired();
                Property(x => x.Descripcion).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }
        }
        public class DiasHorasIngresosMap : EntityTypeConfiguration<DiasHorasIngresos>
        {
            public DiasHorasIngresosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Dia).IsRequired();
                Property(x => x.DeHora).HasColumnType("datetime").IsRequired();
                Property(x => x.AHora).HasColumnType("datetime").IsRequired();
                Property(x => x.Tipo).IsRequired();
                Property(x => x.LimiteEntrada).IsRequired();
                Property(x => x.LimiteComida1).IsRequired();
                Property(x => x.LimiteComida2).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.HorariosIngresosId).IsRequired();
            }
        }
        public class DiasHorasEspecialMap : EntityTypeConfiguration<DiasHorasEspecial>
        {
            public DiasHorasEspecialMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.fchInicio).HasColumnType("datetime").IsRequired();
                Property(x => x.fchFin).HasColumnType("datetime").IsRequired();
                Property(x => x.DeHora).HasColumnType("datetime").IsRequired();
                Property(x => x.AHora).HasColumnType("datetime").IsRequired();
                Property(x => x.Tipo).IsRequired();
                Property(x => x.LimiteEntrada).IsRequired();
                Property(x => x.LimiteComida1).IsRequired();
                Property(x => x.LimiteComida2).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.HorariosIngresosId).IsRequired();
            }
        }
        public class SoporteFacturacionMap : EntityTypeConfiguration<SoporteFacturacion>
        {
            public SoporteFacturacionMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Clave).HasMaxLength(50).IsRequired();
                Property(x => x.Concepto).HasMaxLength(200).IsRequired();
                Property(x => x.NombreHoja).HasMaxLength(50).IsRequired();
                Property(x => x.ServicioNomina).IsRequired();
                Property(x => x.MontoTope).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.Observaciones).IsRequired().HasMaxLength(200);
                Property(x => x.TipodeNominaId).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class EmpleadosSoporteMap : EntityTypeConfiguration<EmpleadosSoporte>
        {
            public EmpleadosSoporteMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.SoporteFacturacionId).IsRequired();
                Property(x => x.Porcentaje).IsRequired();
            }
        }
        public class SoporteSucursalesMap : EntityTypeConfiguration<SoporteSucursales>
        {
            public SoporteSucursalesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.sucursalesId).IsRequired();
                Property(x => x.SoporteFacturacionId).IsRequired();
            }
        }
        public class SoportePuestosMap : EntityTypeConfiguration<SoportePuestos>
        {
            public SoportePuestosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.puestoId).IsRequired();
                Property(x => x.SoporteFacturacionId).IsRequired();
            }
        }
        public class SoporteDptoIngresosMap : EntityTypeConfiguration<SoporteDptoIngresos>
        {
            public SoporteDptoIngresosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.DptosIngresosId).IsRequired();
                Property(x => x.SoporteFacturacionId).IsRequired();
            }
        }
        public class EntidadMap : EntityTypeConfiguration<Entidad>
        {
            public EntidadMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(50);
                Property(x => x.ApellidoMaterno).HasMaxLength(50);
                Property(x => x.ApellidoPaterno).HasMaxLength(50);
                Property(x => x.FechaNacimiento).HasColumnType("date").IsOptional();
                Property(x => x.Foto).IsOptional();
                Property(x => x.TipoEntidadId).IsOptional();

            }
        }
        public class ClienteMap : EntityTypeConfiguration<Cliente>
        {
            public ClienteMap()
            {
                ToTable("Clientes", "Vtas");
                Property(x => x.RazonSocial).HasMaxLength(100);
                Property(x => x.Nombrecomercial).HasMaxLength(500);
                Property(x => x.RFC).HasMaxLength(15);
                Property(x => x.Clasificacion).HasMaxLength(10).IsRequired();
                Property(x => x.NumeroEmpleados).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
                Property(x => x.Clave).HasMaxLength(50).IsRequired();
                //Property(x => x.Clave_Patronal).HasMaxLength(20).IsRequired();
                //Property(x => x.Registro_Patronal_IMSS).HasMaxLength(50).IsRequired();

        }
        }
        public class ColoniasMap : EntityTypeConfiguration<Colonia>
        {
            public ColoniasMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.colonia).HasMaxLength(100).IsRequired();
                Property(x => x.CP).HasMaxLength(13).IsRequired();
                Property(x => x.TipoColonia).HasMaxLength(50);
                Property(x => x.MunicipioId).IsRequired();
                Property(x => x.Activo).IsOptional();
            }
        }
        public class CompetenciasCardinalesMap : EntityTypeConfiguration<CompetenciaCardinal>
        {
            public CompetenciasCardinalesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.competenciaCardinal).HasMaxLength(100).IsRequired();
            }
        }
        public class CompetenciasAreasMap : EntityTypeConfiguration<CompetenciaArea>
        {
            public CompetenciasAreasMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.competenciaArea).HasMaxLength(100).IsRequired();
            }
        }
        public class CompeteciasGerencialesMap : EntityTypeConfiguration<CompetenciaGarencial>
        {
            public CompeteciasGerencialesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.competenciaGerencial).HasMaxLength(100).IsRequired();
            }
        }
        public class DiaSemanaMap : EntityTypeConfiguration<DiaSemana>
        {
            public DiaSemanaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.diaSemana).HasMaxLength(50).IsRequired();
                Property(x => x.tipo).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class DireccionMap : EntityTypeConfiguration<Direccion>
        {
            public DireccionMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.TipoDireccionId).IsRequired();
                Property(x => x.Calle).HasMaxLength(100);
                Property(x => x.Referencia).HasMaxLength(500);
                Property(x => x.NumeroExterior).HasMaxLength(10);
                Property(x => x.NumeroInterior).HasMaxLength(30);
                Property(x => x.PaisId).IsRequired();
                Property(x => x.EstadoId).IsRequired();
                Property(x => x.MunicipioId).IsOptional();
                Property(x => x.ColoniaId).IsOptional();
                Property(x => x.CodigoPostal).HasMaxLength(15).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class EmailMap : EntityTypeConfiguration<Email>
        {
            public EmailMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.email).HasMaxLength(100).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class EstadoCivilMap : EntityTypeConfiguration<EstadoCivil>
        {
            public EstadoCivilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.estadoCivil).HasMaxLength(20).IsRequired();
                Property(x => x.Activo).IsOptional();
            }
        }
        public class EstadoEstudioMap : EntityTypeConfiguration<EstadoEstudio>
        {
            public EstadoEstudioMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.estadoEstudio).HasMaxLength(15).IsRequired();
            }
        }
        public class EstadoMap : EntityTypeConfiguration<Estado>
        {
            public EstadoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.estado).HasMaxLength(100).IsRequired();
                Property(x => x.PaisId).IsRequired();
                Property(x => x.Activo).IsOptional();
                Property(x => x.Clave).IsOptional();
                Property(x => x.Latitud).IsOptional();
                Property(x => x.Longitud).IsOptional();
            }
        }
        public class EstatusMap : EntityTypeConfiguration<Estatus>
        {
            public EstatusMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Descripcion).HasMaxLength(50).IsRequired();
                Property(x => x.Activo).IsOptional();
                Property(x => x.TipoMovimiento).IsRequired();
                Property(x => x.Orden).IsOptional();
            }
        }
        public class GeneroMap : EntityTypeConfiguration<Genero>
        {
            public GeneroMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.genero).HasMaxLength(15).IsRequired();
            }
        }
        public class GiroEmpresaMap : EntityTypeConfiguration<GiroEmpresa>
        {
            public GiroEmpresaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.giroEmpresa).HasMaxLength(15).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class GradoEstudioMap : EntityTypeConfiguration<GradoEstudio>
        {
            public GradoEstudioMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.gradoEstudio).HasMaxLength(15).IsRequired();
            }
        }
        public class GruposMap : EntityTypeConfiguration<Grupos>
        {
            public GruposMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Clave).HasMaxLength(20).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
                Property(x => x.Descripcion).HasMaxLength(200);
            }
        }
        public class SubordinadosMap : EntityTypeConfiguration<Subordinados>
        {
            public SubordinadosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.LiderId).IsRequired();
                Property(x => x.UsuarioId).IsRequired();
            }
        }
        public class JornadaLaboralMap : EntityTypeConfiguration<JornadaLaboral>
        {
            public JornadaLaboralMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Jornada).HasMaxLength(50).IsRequired();
                Property(x => x.Orden).IsRequired();
                Property(x => x.VariosHorarios).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class MunicipioMap : EntityTypeConfiguration<Municipio>
        {
            public MunicipioMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.municipio).HasMaxLength(100).IsRequired();
                Property(x => x.EstadoId).IsRequired();
                Property(x => x.Activo).IsOptional();
            }
        }
        public class NivelMap : EntityTypeConfiguration<Nivel>
        {
            public NivelMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.nivel).HasMaxLength(50).IsRequired();
            }
        }
        public class PaisMap : EntityTypeConfiguration<Pais>
        {
            public PaisMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.pais).HasMaxLength(50).IsRequired();
                Property(x => x.Activo).IsOptional();
            }
        }
        public class PrioridadMap : EntityTypeConfiguration<Prioridad>
        {
            public PrioridadMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Descripcion).HasMaxLength(50).IsRequired();
                Property(x => x.Activo).IsOptional();
            }
        }
        public class RolesMap : EntityTypeConfiguration<Roles>
        {
            public RolesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Rol).HasMaxLength(100).IsRequired().IsUnicode();
            }
        }
        public class TamanoEmpresaMap : EntityTypeConfiguration<TamanoEmpresa>
        {
            public TamanoEmpresaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tamanoEmpresa).HasMaxLength(30).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class TelefonoMap : EntityTypeConfiguration<Telefono>
        {
            public TelefonoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ClavePais).HasMaxLength(5).IsRequired();
                Property(x => x.ClaveLada).HasMaxLength(5);
                Property(x => x.telefono).HasMaxLength(20).IsRequired();
                Property(x => x.Extension).HasMaxLength(10);
                Property(x => x.esPrincipal).IsRequired();
                Property(x => x.TipoTelefonoId).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class TiempoContratoMap : EntityTypeConfiguration<TiempoContrato>
        {
            public TiempoContratoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Tiempo).HasMaxLength(50).IsRequired();
                Property(x => x.Orden).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class TipoBaseMap : EntityTypeConfiguration<TipoBase>
        {
            public TipoBaseMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tipoBase).HasMaxLength(50).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class TipoDireccionMap : EntityTypeConfiguration<TipoDireccion>
        {
            public TipoDireccionMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tipoDireccion).HasMaxLength(50).IsRequired().IsUnicode();
            }
        }
        public class TipoEmpresaMap : EntityTypeConfiguration<TipoEmpresa>
        {
            public TipoEmpresaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tipoEmpresa).HasMaxLength(20).IsRequired();
            }
        }
        public class TipoTelefonoMap : EntityTypeConfiguration<TipoTelefono>
        {
            public TipoTelefonoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Tipo).HasMaxLength(15).IsRequired();
                Property(x => x.Activo).IsOptional();
            }
        }
        public class TipoModalidadMap : EntityTypeConfiguration<TipoModalidad>
        {
            public TipoModalidadMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Modalidad).HasMaxLength(50).IsRequired();
                Property(x => x.Orden).IsRequired();
            }
        }
        public class TipoUsuarioMap : EntityTypeConfiguration<TipoUsuario>
        {
            public TipoUsuarioMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Tipo).HasMaxLength(30).IsRequired();
            }
        }
        public class UsuariosMap : EntityTypeConfiguration<Usuarios>
        {
            public UsuariosMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Usuario).HasMaxLength(30).IsRequired().IsUnicode();
                Property(x => x.Password).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
                Property(x => x.Foto).HasMaxLength(200);
                Property(x => x.Clave).HasMaxLength(50);
                Property(x => x.DepartamentoId).IsOptional();
                Property(x => x.SucursalId).IsRequired();

                //HasMany(x => x.Grupos)
                //    .WithMany(x => x.Usuarios)
                //    .Map(mu =>
                //    {
                //        // UsrGrupos
                //        mu.MapLeftKey("IdUsuario");
                //        mu.MapRightKey("IdGrupo");
                //        mu.ToTable("UsrGrupos");
                //    });
            }
        }
        public class TipoEstructuraMap : EntityTypeConfiguration<TipoEstructura>
        {
            public TipoEstructuraMap()
            {
                HasKey(e => e.Id);
                Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(e => e.Nombre).HasMaxLength(25).IsRequired();
            }
        }
        public class AmbitoMap : EntityTypeConfiguration<Ambito>
        {
            public AmbitoMap()
            {
                HasKey(e => e.Id);
                Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(e => e.Nombre).HasMaxLength(5).IsRequired();
            }
        }
        public class EstructuraMap : EntityTypeConfiguration<Estructura>
        {
            public EstructuraMap()
            {
                HasKey(e => e.Id);
                Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(e => e.IdPadre);
                Property(e => e.TipoEstructuraId).IsRequired();
                Property(e => e.Nombre).HasMaxLength(100);
                Property(e => e.Descripcion).HasMaxLength(150);
                Property(e => e.Orden).IsRequired();
                Property(e => e.AmbitoId).IsRequired();
                Property(e => e.Menu).IsRequired();
                Property(e => e.Confidencial).IsRequired();
                //   Property(e => e.Inclusivo).IsRequired();
                Property(e => e.Activo).IsRequired();
                Property(e => e.Icono).HasMaxLength(100);
                Property(e => e.Accion).HasMaxLength(500);
                Property(e => e.TipoEstructuraId).IsRequired();
            }
        }
        public class TipoMovimientoMap : EntityTypeConfiguration<TipoMovimiento>
        {
            public TipoMovimientoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Clave).HasMaxLength(8).IsRequired().IsUnicode(true);
                Property(x => x.Descripcion).HasMaxLength(50).IsRequired();
                Property(x => x.Orden).IsRequired();
            }
        }
        public class TipoAccionMap : EntityTypeConfiguration<TipoAccion>
        {
            public TipoAccionMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Clave).HasMaxLength(8).IsRequired().IsUnicode(true);
                Property(x => x.Descripcion).HasMaxLength(50).IsRequired();
                Property(x => x.Orden).IsRequired();
            }
        }
        public class TrazabilidadMesMap : EntityTypeConfiguration<TrazabilidadMes>
        {
            public TrazabilidadMesMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.TipoMovimientoId).IsRequired();
                Property(x => x.Folio).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.UsuarioAlta).HasMaxLength(50).IsRequired();
                Property(x => x.UsuarioId).IsRequired();
            }
        }
        public class RastreabilidadMesMap : EntityTypeConfiguration<RastreabilidadMes>
        {
            public RastreabilidadMesMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.TrazabilidadMesId).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.UsuarioMod).HasMaxLength(50).IsRequired();
                Property(x => x.TipoAccionId).IsRequired();
                Property(x => x.Descripcion).HasMaxLength(50).IsRequired();
            }
        }
        public class FoliosMap : EntityTypeConfiguration<Folio>
        {
            public FoliosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Anio).IsRequired();
                Property(x => x.Mes).IsRequired();
                Property(x => x.Consecutivo).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
            }
        }
        public class PrivilegiosMap : EntityTypeConfiguration<Privilegio>
        {
            public PrivilegiosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.EstructuraId).IsRequired();
                Property(x => x.RolId).IsRequired();
                Property(x => x.Create).IsRequired();
                Property(x => x.Read).IsRequired();
                Property(x => x.Read).IsRequired();
                Property(x => x.Delete).IsRequired();
            }
        }
        public class TipoEntidadMap : EntityTypeConfiguration<TipoEntidad>
        {
            public TipoEntidadMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tipoEntidad).HasMaxLength(50);
                Property(x => x.Activo).IsRequired();
            }
        }

        public class TratamientoMap : EntityTypeConfiguration<Tratamiento>
        {
            public TratamientoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tratamiento).HasMaxLength(50);
            }
        }

        public class PostNombreMap : EntityTypeConfiguration<PostNombre>
        {
            public PostNombreMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            }
        }

        public class OficinaReclutamientoMap : EntityTypeConfiguration<OficinaReclutamiento>
        {
            public OficinaReclutamientoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.TipoOficinaId).IsRequired();
                Property(x => x.IndicacionEspecial).HasMaxLength(50).IsOptional();
                Property(x => x.Orden).IsRequired();
                Property(x => x.Latitud).HasMaxLength(25).IsRequired();
                Property(x => x.Longitud).HasMaxLength(25).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
                Property(x => x.estadoId).IsRequired();
            }
        }

        public class UnidadNegocioEstadosMap : EntityTypeConfiguration<UnidadNegocioEstados>
        {
            public UnidadNegocioEstadosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.estadoId).IsRequired();
                Property(x => x.unidadnegocioId).IsRequired();

            }
        }

        public class TipoOficinaMap : EntityTypeConfiguration<TipoOficina>
        {
            public TipoOficinaMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tipoOficina).HasMaxLength(25).IsRequired();
                Property(x => x.Icono).HasMaxLength(255).IsOptional();

            }
        }

        public class RolEntidadesMap : EntityTypeConfiguration<RolEntidad>
        {
            public RolEntidadesMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.EntidadId).IsRequired();
                Property(x => x.RolId).IsRequired();
            }
        }

        public class ConfiguracionMovsMap : EntityTypeConfiguration<ConfiguracionMovs>
        {
            public ConfiguracionMovsMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.EstructuraId).IsRequired();
                Property(x => x.esPublicable).IsRequired();
                Property(x => x.esEditable).IsRequired();
                Property(x => x.nuevaEtiqueta).HasMaxLength(150).IsOptional();
                Property(x => x.nuevoValor).HasMaxLength(150).IsOptional();
            }
        }

        public class LogsIngresosMap : EntityTypeConfiguration<LogsIngresos>
        {
            public LogsIngresosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ASPId).IsOptional();
                Property(x => x.EntidadId).IsOptional();
                Property(x => x.EstructuraId).IsRequired();
                Property(x => x.fch_Ingreso).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
            }
        }

        public class MotivoLiberacioMap : EntityTypeConfiguration<MotivoLiberacion>
        {
            public MotivoLiberacioMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Descripcion).HasMaxLength(100).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.EstatusId).IsOptional();
            }
        }

        public class FolioIncidenciaMap : EntityTypeConfiguration<FolioIncidencia>
        {
            public FolioIncidenciaMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Folio).HasMaxLength(20).IsRequired();
                Property(x => x.ComentarioId).IsRequired();
            }
        }

        public class PuestoMap : EntityTypeConfiguration<Puesto>
        {
            public PuestoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Clave).HasMaxLength(20).IsRequired();
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.CoordinacionId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.BTRA).IsRequired();
                Property(x => x.ERP).IsRequired();
            }
        }
      
        public class CalendarioEventMap : EntityTypeConfiguration<CalendarioEvent>
        {
            public CalendarioEventMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.EntidadId).IsRequired();
                Property(x => x.Title).HasMaxLength(100).IsRequired();
                Property(x => x.Message).HasMaxLength(500).IsOptional();
                Property(x => x.Start).IsRequired();
                Property(x => x.End).IsOptional();
                Property(x => x.AllDay).IsOptional();
                Property(x => x.backgroundColor).HasMaxLength(50).IsRequired();
                Property(x => x.borderColor).HasMaxLength(50).IsRequired();
                Property(x => x.TipoActividadId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
            }
        }

        public class TipoActividadReclutadorMap : EntityTypeConfiguration<TipoActividadReclutador>
        {
            public TipoActividadReclutadorMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Actividad).HasMaxLength(100).IsRequired();
                Property(x => x.Activo).IsRequired();
            }
        }

        public class AlertasStmMap : EntityTypeConfiguration<AlertasStm>
        {
            public AlertasStmMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.EntidadId).IsRequired();
                Property(x => x.Alert).HasMaxLength(500).IsRequired();
                Property(x => x.Icon).HasMaxLength(30).IsRequired();
                Property(x => x.Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.Activo).IsRequired();
            }
        }

        public class TipoAlertaMap : EntityTypeConfiguration<TipoAlerta>
        {
            public TipoAlertaMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Tipo).HasMaxLength(50).IsRequired();
                Property(x => x.Activo).IsRequired();
            }
        }

        #region Examenes
        //Modulo para examenes

        public class ExamenesMap : EntityTypeConfiguration<Examenes>
        {
            public ExamenesMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.Descripcion).HasMaxLength(600).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.TipoExamenId).IsRequired();
                Property(x => x.Orden).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.UsuarioId).IsRequired();
                Property(x => x.fch_Modificacion).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();

            }
        }
        public class TipoExamenesMap : EntityTypeConfiguration<TipoExamen>
        {
            public TipoExamenesMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.Descripcion).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.UsuarioId).IsRequired();
                Property(x => x.fch_Modificacion).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
            }
        }
        public class PreguntasMap : EntityTypeConfiguration<Preguntas>
        {
            public PreguntasMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Pregunta).HasMaxLength(500).IsRequired();
                Property(x => x.Tipo).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.Orden).IsRequired();
                Property(x => x.ExamenId).IsRequired();
            }
        }

        public class RespuestasMap : EntityTypeConfiguration<Respuestas>
        {
            public RespuestasMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Respuesta).HasMaxLength(500).IsRequired();
                Property(x => x.Validacion).IsRequired();
                Property(x => x.PreguntaId).IsRequired();
                Property(x => x.Orden).IsRequired();
            }
        }

        public class RequiExamenMap : EntityTypeConfiguration<RequiExamen>
        {
            public RequiExamenMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.ExamenId).IsRequired();
            }
        }

        public class ExamenCandidatoMap : EntityTypeConfiguration<ExamenCandidato>
        {
            public ExamenCandidatoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ExamenId).IsRequired();
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.Resultado).IsOptional();
                Property(x => x.fch_Creacion).IsRequired();
                Property(x => x.fch_Modificacion).IsRequired();
            }
        }

        public class ResultadosCandidatoMap : EntityTypeConfiguration<ResultadosCandidato>
        {
            public ResultadosCandidatoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.PreguntaId).IsRequired();
                Property(x => x.Value).IsRequired();
                Property(x => x.RespuestaId).IsRequired();
            }
        }

        public class RequiClavesMap : EntityTypeConfiguration<RequiClaves>
        {
            public RequiClavesMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.Clave).IsRequired().IsUnicode();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioId).IsOptional();
            }
        }

        public class PsicometriaCandidatosMap : EntityTypeConfiguration<PsicometriaCandidato>
        {
            public PsicometriaCandidatosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.RequiClaveId).IsRequired();
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.Resultado).IsRequired().HasMaxLength(100);
                Property(x => x.fch_Creacion).IsRequired();
                Property(x => x.fch_Resultado).IsRequired();
                Property(x => x.UsuarioId).IsOptional();
            }
        }

        public class EntrevistasMap : EntityTypeConfiguration<Entrevista>
        {
            public EntrevistasMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(200).IsRequired();
                Property(x => x.Descripcion).HasMaxLength(600).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.UsuarioId).IsRequired();
                Property(x => x.fch_Modificacion).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
            }
        }
        public class ConfigEntrevistaMap : EntityTypeConfiguration<ConfigEntrevista>
        {
            public ConfigEntrevistaMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.EntrevistaId).IsRequired();
                Property(x => x.PreguntaId).IsRequired();
                Property(x => x.Orden).IsRequired();
            }
        }

        public class RelacionClientesSistemasMap : EntityTypeConfiguration<RelacionClientesSistemas>
        {
            public RelacionClientesSistemasMap()
            {
                Property(x => x.Id).IsRequired();
                Property(x => x.Clave_Empresa).HasMaxLength(8).IsRequired();
                Property(x => x.Clave_Razon).HasMaxLength(8).IsOptional();
                Property(x => x.Clave_Unica).HasMaxLength(50).IsOptional();
                Property(x => x.Usuario).HasMaxLength(30).IsOptional();

            }
        }
        #endregion

        //Modulo de preguntas frecuentes
        public class PreguntasFrecuentesMap : EntityTypeConfiguration<PreguntasFrecuente>
        {
            public PreguntasFrecuentesMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Pregunta).HasMaxLength(500).IsRequired();
                Property(x => x.Respuesta).HasMaxLength(500).IsRequired();
                Property(x => x.Activo).IsOptional();
                //   Property(x => x.).IsOptional();
            }
        }

        public class UnidadesNegociosMap : EntityTypeConfiguration<UnidadNegocio>
        {
            public UnidadesNegociosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
                Property(x => x.Activo).IsRequired();
                Property(x => x.Clave).IsRequired().HasMaxLength(5);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).IsRequired();
                Property(x => x.usuarioAlta).IsRequired();
                Property(x => x.usuarioMod).IsRequired();
            }
        }

        public class CatalogosMap : EntityTypeConfiguration<Catalogos>
        {
            public CatalogosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                Property(x => x.Descripcion).IsRequired().HasMaxLength(255);
                Property(x => x.EstructuraId).IsRequired();
                Property(x => x.Activo).IsRequired();
            }
        }

        public class LogCatalogosMap : EntityTypeConfiguration<LogCatalogos>
        {
            public LogCatalogosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CatalogoId).IsRequired();
                Property(x => x.Campo).IsRequired().HasMaxLength(20);
                Property(x => x.FechaAct).IsRequired();
                Property(x => x.TpMov).IsRequired().HasMaxLength(5); ;
                Property(x => x.Usuario).IsRequired().HasMaxLength(50); ;
            }
        }

        public class TransferenciasMap : EntityTypeConfiguration<Transferencias>
        {
            public TransferenciasMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.antId).IsRequired();
                Property(x => x.actId).IsRequired();
                Property(x => x.requisicionId).IsRequired();
                Property(x => x.tipoTransferenciaId).IsRequired();
                Property(x => x.fch_Modificacion).IsRequired();
            }
        }

        public class TiposTransferenciasMap : EntityTypeConfiguration<TiposTransferencias>
        {
            public TiposTransferenciasMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.TipoTransf).HasMaxLength(100).IsRequired();
                Property(x => x.Activo).IsRequired();
            }
        }

        public class TipoExamenMedicoMap : EntityTypeConfiguration<TipoExamenMedico>
        {
            public TipoExamenMedicoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Descripcion).HasMaxLength(100).IsRequired();
                Property(x => x.Activo).IsRequired();
            }
        }
        public class MedicoCandidatoMap : EntityTypeConfiguration<MedicoCandidato>
        {
            public MedicoCandidatoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.RequisicionId).IsOptional();
                Property(x => x.Facturado).IsRequired();
                Property(x => x.Resultado).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }

        public class VertionSistemMap : EntityTypeConfiguration<VertionSistem>
        {
            public VertionSistemMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Version).HasMaxLength(50).IsRequired();
                Property(x => x.Descripcion).IsOptional();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.Liberada).IsRequired();
            }
        }

        public class CatalogoBancosMap : EntityTypeConfiguration<CatalogoBancos>
        {
            public CatalogoBancosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Clave).HasMaxLength(20).IsRequired();
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.RazonSocial).HasMaxLength(200).IsRequired();
                Property(x => x.Descripcion).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }
        }

        public class EmpresaBancosMap : EntityTypeConfiguration<EmpresaBancos>
        {
            public EmpresaBancosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.BancoId).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }

        public class MotivosContratacionMap : EntityTypeConfiguration<MotivosContratacion>
        {
            public MotivosContratacionMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Clave).IsRequired().HasMaxLength(20);
                Property(x => x.Descripcion).HasMaxLength(100).IsRequired();
                Property(x => x.Observaciones).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }

        public class GrupoSanguineoMap : EntityTypeConfiguration<GrupoSanguineo>
        {
            public GrupoSanguineoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Grupo).IsRequired().HasMaxLength(5);
                Property(x => x.Activo).IsRequired();
            }
        }

        public class FormaPagoMap : EntityTypeConfiguration<FormaPago>
        {
            public FormaPagoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Clave).HasMaxLength(20).IsRequired();
                Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
                Property(x => x.Comentario).IsRequired().HasMaxLength(200);
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }

        public class TiposDocumentosMap : EntityTypeConfiguration<TipoDocumentos>
        {
            public TiposDocumentosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Descripcion).HasMaxLength(100).IsRequired();
                Property(x => x.Observaciones).HasMaxLength(300).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.usuarioId).IsRequired();
                Property(x => x.fch_Modificacion).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
            }
        }
        public class DocumentosMap : EntityTypeConfiguration<Documentos>
        {
            public DocumentosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Descripcion).HasMaxLength(100).IsRequired();
                Property(x => x.Observaciones).HasMaxLength(300).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.usuarioId).IsRequired();
                Property(x => x.TipoDocumentoId).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
            }
        }
        #endregion

        #region "Mapeo BTra"
        public class CandidatoMap : EntityTypeConfiguration<Candidato>
        {
            public CandidatoMap()
            {
                ToTable("Candidatos", "BTra");
                Property(x => x.PaisNacimientoId);
                Property(x => x.EstadoNacimientoId);
                Property(x => x.MunicipioNacimientoId);
                Property(x => x.fch_Creacion).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
                Property(x => x.OtraDiscapacidad).HasMaxLength(100).IsOptional();

            }
        }
        public class AboutMeMap : EntityTypeConfiguration<AboutMe>
        {
            public AboutMeMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.PerfilCandidatoId).IsRequired();
                Property(x => x.AcercaDeMi).HasMaxLength(400);
                Property(x => x.PuestoDeseado).HasMaxLength(40);
                Property(x => x.SalarioAceptable).HasPrecision(18, 2).IsRequired();
                Property(x => x.SalarioDeseado).HasPrecision(18, 2).IsRequired();
                Property(x => x.AreaExperienciaId).IsRequired();
                Property(x => x.AreaInteresId).IsOptional();
                Property(x => x.PerfilExperienciaId).IsOptional();


            }
        }
        public class AreaExperienciaMap : EntityTypeConfiguration<AreaExperiencia>
        {
            public AreaExperienciaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.areaExperiencia).HasMaxLength(200);
                Property(x => x.Icono).HasMaxLength(50).IsOptional();
            }
        }
        public class AreaInteresMap : EntityTypeConfiguration<AreaInteres>
        {
            public AreaInteresMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.areaInteres).HasMaxLength(200);
                Property(x => x.AreaExperienciaId).IsRequired();
            }
        }
        public class CarreraMap : EntityTypeConfiguration<Carrera>
        {
            public CarreraMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.carrera).HasMaxLength(250);
            }
        }
        public class CertificacionMap : EntityTypeConfiguration<Certificacion>
        {
            public CertificacionMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.certificacion).HasMaxLength(200);
                Property(x => x.AutoridadEmisora).HasMaxLength(200);
                Property(x => x.Licencia).HasMaxLength(100);
                Property(x => x.UrlCertificacion).HasMaxLength(500);
                Property(x => x.noVence).IsOptional();
                Property(x => x.YearInicioId).IsOptional();
                Property(x => x.MonthInicioId).IsOptional();
                Property(x => x.YearTerminoId).IsOptional();
                Property(x => x.MonthTerminoId).IsOptional();
                Property(x => x.PerfilCandidatoId).IsRequired();
                Property(x => x.Actual).IsOptional();
            }
        }
        public class ConocimientoOHabilidadMap : EntityTypeConfiguration<ConocimientoOHabilidad>
        {
            public ConocimientoOHabilidadMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Conocimiento).HasMaxLength(250);
                Property(x => x.Conocimiento).HasMaxLength(50);
                Property(x => x.InstitucionEducativaId).IsOptional();
                Property(x => x.NivelId).IsOptional();
                Property(x => x.PerfilCandidatoId).IsRequired();
            }
        }
        public class CursoMap : EntityTypeConfiguration<Curso>
        {
            public CursoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.curso).HasMaxLength(200);
                Property(x => x.InstitucionEducativaId).IsRequired();
                Property(x => x.YearInicioId).IsOptional();
                Property(x => x.MonthInicioId).IsOptional();
                Property(x => x.YearTerminoId).IsOptional();
                Property(x => x.MonthTerminoId).IsOptional();
                Property(x => x.Horas).IsOptional();
                Property(x => x.PerfilCandidatoId).IsRequired();
                Property(x => x.Actual).IsOptional();

            }
        }
        public class DocumentoValidadorMap : EntityTypeConfiguration<DocumentoValidador>
        {
            public DocumentoValidadorMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.documentoValidador).HasMaxLength(100);
            }
        }
        public class FormulariosInicialesMap : EntityTypeConfiguration<FormulariosIniciales>
        {
            public FormulariosInicialesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Paso);
            }
        }
        public class FormacionMap : EntityTypeConfiguration<Formacion>
        {
            public FormacionMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.InstitucionEducativaId).IsRequired();
                Property(x => x.GradoEstudioId).IsRequired();
                Property(x => x.EstadoEstudioId).IsRequired();
                Property(x => x.DocumentoValidadorId).IsOptional();
                Property(x => x.CarreraId).IsOptional();
                Property(x => x.YearInicioId).IsOptional();
                Property(x => x.MonthInicioId).IsOptional();
                Property(x => x.YearTerminoId).IsOptional();
                Property(x => x.MonthTerminoId).IsOptional();
                Property(x => x.Actual).IsOptional();
            }
        }
        public class FormaContactoMap : EntityTypeConfiguration<FormaContacto>
        {
            public FormaContactoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.CorreoElectronico);
                Property(x => x.Celular).IsRequired();
                Property(x => x.WhatsApp).IsRequired();
                Property(x => x.TelLocal).IsRequired();

            }
        }
        public class ExperienciaProfesionalMap : EntityTypeConfiguration<ExperienciaProfesional>
        {
            public ExperienciaProfesionalMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Empresa).HasMaxLength(100);
                Property(x => x.GiroEmpresaId).IsRequired();
                Property(x => x.CargoAsignado).HasMaxLength(100);
                Property(x => x.AreaId).IsRequired();
                Property(x => x.YearInicioId).IsRequired();
                Property(x => x.MonthInicioId).IsRequired();
                Property(x => x.YearTerminoId).IsRequired();
                Property(x => x.MonthTerminoId).IsRequired();
                Property(x => x.Salario).HasPrecision(18, 2).IsRequired();
                Property(x => x.TrabajoActual).IsOptional();
                Property(x => x.Descripcion).HasMaxLength(500).IsOptional();
                Property(x => x.PerfilCandidatoId).IsRequired();
            }
        }
        public class InstitucionEducativaMap : EntityTypeConfiguration<InstitucionEducativa>
        {
            public InstitucionEducativaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.institucionEducativa).HasMaxLength(250);
            }
        }
        public class PerfilExperienciaMap : EntityTypeConfiguration<PerfilExperiencia>
        {
            public PerfilExperienciaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.perfilExperiencia).HasMaxLength(200);
                Property(x => x.activo).IsRequired();
            }
        }
        public class PerfilCandidatoMap : EntityTypeConfiguration<PerfilCandidato>
        {
            public PerfilCandidatoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.Estatus).IsOptional();
            }
        }
        public class PostulacionMap : EntityTypeConfiguration<Postulacion>
        {
            public PostulacionMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.StatusId).IsRequired();
                Property(X => X.fch_Postulacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();


            }
        }
        public class StatusPostulacionMap : EntityTypeConfiguration<StatusPostulacion>
        {
            public StatusPostulacionMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Status).HasMaxLength(50);
            }
        }
        public class TipoDiscapacidadMap : EntityTypeConfiguration<TipoDiscapacidad>
        {
            public TipoDiscapacidadMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tipoDiscapacidad).HasMaxLength(50);
            }
        }
        public class TipoLicenciaMap : EntityTypeConfiguration<TipoLicencia>
        {
            public TipoLicenciaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tipoLicencia).HasMaxLength(1);
                Property(x => x.Descripcion).HasMaxLength(250);
            }
        }
        public class TipoRedSocialMap : EntityTypeConfiguration<TipoRedSocial>
        {
            public TipoRedSocialMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tipoRedSocial).HasMaxLength(50);
            }
        }
        public class PerfilIdimoasMap : EntityTypeConfiguration<PerfilIdioma>
        {
            public PerfilIdimoasMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.PerfilCandidatoId).IsRequired();
                Property(x => x.IdiomaId).IsRequired();
                Property(x => x.NivelEscritoId).IsOptional();
                Property(x => x.NivelHabladoId).IsOptional();

            }
        }
        public class DepartamentoMap : EntityTypeConfiguration<Departamento>
        {
            public DepartamentoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.AreaId).IsRequired();
                Property(x => x.Clave).HasMaxLength(20).IsRequired();
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.Observaciones).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class FrecuenciasMap : EntityTypeConfiguration<Frecuencias>
        {
            public FrecuenciasMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Frecuencia).IsRequired();
                Property(x => x.Activo).IsOptional();
            }
        }
        public class AlertasMap : EntityTypeConfiguration<Alertashdr>
        {
            public AlertasMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.FrecuenciaId).IsRequired();
                Property(x => x.CorreoTelefono).HasMaxLength(100).IsRequired();
                Property(x => x.Fch_UltimaEjecucion).HasColumnType("datetime").IsRequired();
                Property(x => x.Activo).IsOptional();
            }
        }
        public class AlertasdtlMap : EntityTypeConfiguration<Alertasdtl>
        {
            public AlertasdtlMap()
            {
                HasKey(x => x.Id);
                Property(x => x.alertaId).IsRequired();
                Property(x => x.areaexperiencia).IsRequired();
            }
        }

        public class HorariosCalendarioMap : EntityTypeConfiguration<HorariosCalendario>
        {
            public HorariosCalendarioMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Horario).HasColumnType("datetime").IsRequired();
                Property(x => x.Descripcion).IsOptional();
                Property(x => x.Activo).IsRequired();
                Property(x => x.Orden).HasColumnType("int").IsRequired();
            }
        }

        public class CalendarioCandidatoMap : EntityTypeConfiguration<CalendarioCandidato>
        {
            public CalendarioCandidatoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.UbicacionId).IsRequired();
                Property(x => x.Fecha).HasColumnType("datetime");
                Property(x => x.Estatus).HasColumnType("int").IsRequired();
                Property(x => x.Folio).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RequisicionId).IsRequired();
            }
        }

        public class AvancePerfilMap : EntityTypeConfiguration<AvancePerfil>
        {
            public AvancePerfilMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.PerfilCandidatoId).IsRequired();
                Property(x => x.Avance).IsOptional();
            }
        }


        #endregion

        #region "Mapeo Vtas"
        #region Prospectos / Clientes
        public class ActividadEmpMap : EntityTypeConfiguration<ActividadEmpresa>
        {
            public ActividadEmpMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.actividadEmpresa).HasMaxLength(300).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class AgenciaMap : EntityTypeConfiguration<Agencia>
        {
            public AgenciaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.agencia).HasMaxLength(50).IsRequired();
                Property(x => x.DesdeCuendo).HasColumnType("date");
                Property(x => x.Empleado).HasPrecision(5, 2).IsRequired();
                Property(x => x.Cobro).HasPrecision(5, 2).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class ReferenciadoMap : EntityTypeConfiguration<Referenciado>
        {
            public ReferenciadoMap()
            {
                ToTable("Referenciados", "Vtas");
                Property(x => x.Puesto).HasMaxLength(100).IsRequired();
                Property(x => x.Clave).HasMaxLength(100).IsRequired();
            }
        }
        public class ContactoMap : EntityTypeConfiguration<Contacto>
        {
            public ContactoMap()
            {
                ToTable("Contactos", "Vtas");
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Puesto).HasMaxLength(100).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(50);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(50);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
                Property(x => x.InfoAdicional).HasMaxLength(250).IsOptional();
            }
        }
        public class ClaseReclutamientoMap : EntityTypeConfiguration<ClaseReclutamiento>
        {
            public ClaseReclutamientoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.clasesReclutamiento).HasMaxLength(50).IsRequired();
                Property(x => x.Activo).IsRequired();
            }
        }

        public class DpTpDiscapacidadMap : EntityTypeConfiguration<DpTpDiscapacidad>
        {
            public DpTpDiscapacidadMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.tipoDiscapacidadId).IsRequired();
            }
        }

        public class FacturacionPuroMap : EntityTypeConfiguration<FacturacionPuro>
        {
            public FacturacionPuroMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.Porcentaje).IsRequired();
                Property(x => x.Monto).HasPrecision(18, 4).IsRequired();
                Property(x => x.PerContratado).IsRequired();
                Property(x => x.MontoContratado).HasPrecision(18, 4).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }

        public class DireccionTelefonoMap : EntityTypeConfiguration<DireccionTelefono>
        {
            public DireccionTelefonoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.DireccionId).IsRequired();
                Property(x => x.TelefonoId).IsRequired();
            }
        }

        public class DireccionEmailMap : EntityTypeConfiguration<DireccionEmail>
        {
            public DireccionEmailMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.DireccionId).IsRequired();
                Property(x => x.EmailId).IsRequired();
            }
        }

        public class DireccionContactoMap : EntityTypeConfiguration<DireccionContacto>
        {
            public DireccionContactoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.DireccionId).IsRequired();
                Property(x => x.ContactoId).IsRequired();
            }
        }

        public class CostosMap : EntityTypeConfiguration<Costos>
        {
            public CostosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Descripcion).HasMaxLength(100).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.Fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.Fch_Modificacion).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioCreacion).IsOptional();
                Property(x => x.UsuarioModificacion).IsOptional();
            }
        }
        public class TipoCostosMap : EntityTypeConfiguration<TipoCostos>
        {
            public TipoCostosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Descripcion).HasMaxLength(100).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.CostosId).IsRequired();
                Property(x => x.Fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.Fch_Modificacion).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioCreacion).IsOptional();
                Property(x => x.UsuarioModificacion).IsOptional();

            }
        }
        public class CostosDamfo290Map : EntityTypeConfiguration<CostosDamfo290>
        {
            public CostosDamfo290Map()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.TipoCostosId).IsRequired();
                Property(x => x.Costo).HasPrecision(18, 4).IsRequired();
                Property(x => x.DAMFO290Id).IsRequired();
            }
        }
        public class ExamenMedicoClienteMap : EntityTypeConfiguration<ExamenMedicoCliente>
        {
            public ExamenMedicoClienteMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.TipoExamenMedicoId).IsRequired();
                Property(x => x.Costo).HasPrecision(18, 4).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }

        #endregion

        #region Requisiciones
        public class RequisicionMap : EntityTypeConfiguration<Requisicion>
        {
            public RequisicionMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.TipoNominaId).IsRequired();
                Property(x => x.ClaseReclutamientoId).IsRequired();
                Property(x => x.VBtra).HasMaxLength(100);
                Property(x => x.GeneroId).IsRequired();
                Property(x => x.EdadMinima).IsRequired();
                Property(x => x.EdadMaxima).IsRequired();
                Property(x => x.EstadoCivilId).IsRequired();
                Property(x => x.AreaId).IsRequired();
                Property(x => x.Experiencia).HasMaxLength(500).IsRequired();
                Property(x => x.SueldoMinimo).HasPrecision(18, 2).IsRequired();
                Property(x => x.SueldoMaximo).HasPrecision(18, 3).IsRequired();
                Property(x => x.DiaCorteId).IsRequired();
                Property(x => x.TipoNominaId).IsRequired();
                Property(x => x.DiaPagoId).IsRequired();
                Property(x => x.PeriodoPagoId).IsRequired();
                Property(x => x.Especifique).IsOptional();
                Property(x => x.ContratoInicialId).IsRequired();
                Property(x => x.TiempoContratoId).IsOptional();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Aprobacion).HasColumnType("DateTime").IsOptional();
                Property(x => x.fch_Cumplimiento).HasColumnType("DateTime").IsOptional();
                Property(x => x.fch_Modificacion).HasColumnType("DateTime").IsOptional();
                Property(x => x.Propietario).HasMaxLength(30).IsOptional();
                Property(x => x.Aprobador).HasMaxLength(30).IsOptional();
                Property(x => x.UsuarioMod).HasMaxLength(30).IsOptional();
                Property(x => x.PrioridadId).IsOptional();
                Property(x => x.Aprobada).IsOptional();
                Property(x => x.Confidencial).IsOptional();
                Property(x => x.Asignada).IsOptional();
                Property(x => x.EstatusId).IsOptional();
                Property(x => x.DAMFO290Id).IsRequired();
                Property(x => x.DireccionId).IsOptional();
                Property(x => x.FlexibilidadHorario).IsRequired();
                Property(x => x.JornadaLaboralId).IsOptional();
                Property(x => x.TipoModalidadId).IsOptional();
                Property(x => x.Activo).IsRequired();
                Property(x => x.Folio).IsRequired();
                Property(x => x.DiasEnvio).IsOptional();
                Property(x => x.PropietarioId).IsOptional();
                Property(x => x.AprobadorId).IsOptional();
                Property(x => x.Publicado).IsOptional();
            }
        }
        public class EscolaridadesRequiMap : EntityTypeConfiguration<EscolaridadesRequi>
        {
            public EscolaridadesRequiMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.EscolaridadId).IsRequired();
                Property(x => x.EstadoEstudioId).IsRequired();
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class AptitudesRequiMap : EntityTypeConfiguration<AptitudesRequi>
        {
            public AptitudesRequiMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.AptitudId).IsRequired();
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class HorarioRequiMap : EntityTypeConfiguration<HorarioRequi>
        {
            public HorarioRequiMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.deDiaId).IsRequired();
                Property(x => x.aDiaId).IsRequired();
                Property(x => x.deHora).IsRequired();
                Property(x => x.aHora).IsRequired();
                Property(x => x.numeroVacantes).HasColumnType("tinyint").IsRequired();
                Property(x => x.Especificaciones).HasMaxLength(500).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class ActividadesRequilMap : EntityTypeConfiguration<ActividadesRequi>
        {
            public ActividadesRequilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Actividades).HasMaxLength(200).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class ObservacionesRequiMap : EntityTypeConfiguration<ObservacionesRequi>
        {
            public ObservacionesRequiMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Observaciones).HasMaxLength(100).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class PsicometriasDamsaRequiMap : EntityTypeConfiguration<PsicometriasDamsaRequi>
        {
            public PsicometriasDamsaRequiMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.PsicometriaId).IsRequired();
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class PsicometriasClienteRequiMap : EntityTypeConfiguration<PsicometriasClienteRequi>
        {
            public PsicometriasClienteRequiMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Psicometria).HasMaxLength(50);
                Property(x => x.Descripcion).HasMaxLength(200);
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class BeneficiosRequiMap : EntityTypeConfiguration<BeneficiosRequi>
        {
            public BeneficiosRequiMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.TipoBeneficioId).IsRequired();
                Property(x => x.Observaciones).HasMaxLength(500);
                Property(x => x.Cantidad).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }

        }
        public class DocumentosClienteRequiMap : EntityTypeConfiguration<DocumentosClienteRequi>
        {
            public DocumentosClienteRequiMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Documento).HasMaxLength(100).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class ProcesoRequiMap : EntityTypeConfiguration<ProcesoRequi>
        {
            public ProcesoRequiMap()
            {

                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Proceso).HasMaxLength(100).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class PrestacionesClienteRequiMap : EntityTypeConfiguration<PrestacionesClienteRequi>
        {
            public PrestacionesClienteRequiMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Prestamo).HasMaxLength(100).IsRequired();
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class CompetenciasAreasRequiMap : EntityTypeConfiguration<CompetenciaAreaRequi>
        {
            public CompetenciasAreasRequiMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CompetenciaId).IsRequired();
                Property(x => x.Nivel).HasMaxLength(10).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class CompetenciaCardinalRequilMap : EntityTypeConfiguration<CompetenciaCardinalRequi>
        {
            public CompetenciaCardinalRequilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CompetenciaId).IsRequired();
                Property(x => x.Nivel).HasMaxLength(10).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }

        }
        public class CompetenciaGerencialRequiMap : EntityTypeConfiguration<CompetenciaGerencialRequi>
        {
            public CompetenciaGerencialRequiMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CompetenciaId).IsRequired();
                Property(x => x.Nivel).HasMaxLength(10).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class AsignacionRequiMap : EntityTypeConfiguration<AsignacionRequi>
        {
            public AsignacionRequiMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.GrpUsrId).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
                Property(x => x.Tipo).IsRequired();
            }
        }
        public class ConfiguracionRequiMap : EntityTypeConfiguration<ConfiguracionRequi>
        {
            public ConfiguracionRequiMap()
            {
                HasKey(x => x.id);
                Property(e => e.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(e => e.RequisicionId).IsRequired();
                Property(e => e.IdEstructura).IsRequired();
                Property(e => e.Campo).HasMaxLength(500).IsRequired();
                Property(e => e.R_D).IsRequired();
                Property(e => e.Resumen).IsRequired();
                Property(e => e.Detalle).IsRequired();
                Property(e => e.UsuarioId).IsRequired();
                Property(e => e.Fch_Modificacion).IsRequired();
            }
        }
        public class HorariosDireccionesRequiMap : EntityTypeConfiguration<HorariosDireccionesRequi>
        {
            public HorariosDireccionesRequiMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.HorariosId).IsRequired();
                Property(x => x.DireccionesId).IsRequired();
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.Vacantes).HasColumnType("tinyint").IsRequired();
            }
        }
        public class InformeRequisicionesMap : EntityTypeConfiguration<InformeRequisicion>
        {
            public InformeRequisicionesMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.EstatusId).IsRequired();
                Property(x => x.ReclutadorId);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class InformeCandidatosMap : EntityTypeConfiguration<InformeCandidatos>
        {
            public InformeCandidatosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.tipoMovimientoId).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
            }
        }
        public class EstatusRequisicionesMap : EntityTypeConfiguration<EstatusRequisiciones>
        {
            public EstatusRequisicionesMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.PropietarioId).IsRequired();
                Property(x => x.EstatusId).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
                Property(x => x.UsuarioMod).HasMaxLength(50).IsRequired();
            }
        }
        #endregion
        #endregion

        #region Mapeo Banco

        public class TicketsMap : EntityTypeConfiguration<Ticket>
        {
            public TicketsMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.ModuloId).IsRequired();
                Property(x => x.Numero).HasMaxLength(50).IsRequired();
                Property(x => x.MovimientoId).IsRequired();
                Property(x => x.Estatus).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
            }
        }

        public class TicketsReclutadorMap : EntityTypeConfiguration<TicketReclutador>
        {
            public TicketsReclutadorMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ReclutadorId).IsRequired();
                Property(x => x.TicketId).IsRequired();
                Property(x => x.fch_Atencion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Final).HasColumnType("datetime").IsRequired();
            }
        }

        public class ModulosReclutamientoMap : EntityTypeConfiguration<ModulosReclutamiento>
        {
            public ModulosReclutamientoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.SucursalId).IsRequired();
                Property(x => x.Modulo).HasMaxLength(50).IsRequired();
                Property(x => x.TipoModulo).IsRequired();
                Property(x => x.Descripcion).HasMaxLength(150).IsRequired();
            }
        }

        public class HistoricoTicketMap : EntityTypeConfiguration<HistoricoTicket>
        {
            public HistoricoTicketMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.ReclutadorId).IsOptional();
                Property(x => x.CandidatoId).IsOptional();
                Property(x => x.Numero).IsRequired().HasMaxLength(20);
                Property(x => x.MovimientoId).IsRequired();
                Property(x => x.Estatus).IsRequired();
                Property(x => x.ModuloId).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        #endregion

        #region "Mapeo Recl"
        public class ActividadesPerfilMap : EntityTypeConfiguration<ActividadesPerfil>
        {
            public ActividadesPerfilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Actividades).HasMaxLength(200).IsOptional();
                Property(x => x.ActividadesPerfilesId).IsOptional();
            }
        }
        public class ActividadesPerfilesMap : EntityTypeConfiguration<ActividadesPerfiles>
        {
            public ActividadesPerfilesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Actividades).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.PerfilesDamfoId).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class AptitudesPerfilMap : EntityTypeConfiguration<AptitudesPerfil>
        {
            public AptitudesPerfilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.AptitudId).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class PerfilesDamfoMap : EntityTypeConfiguration<PerfilesDamfo>
        {
            public PerfilesDamfoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Perfil).HasMaxLength(100).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.EntidadId).IsRequired();
            }
        }
        public class PerfilDamfoRelMap : EntityTypeConfiguration<PerfilDamfoRel>
        {
            public PerfilDamfoRelMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.damfo_290Id).IsRequired();
                Property(x => x.ActividadesPerfilesId).IsRequired();
            }

        }

        public class AptitudMap : EntityTypeConfiguration<Aptitud>
        {
            public AptitudMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.aptitud).HasMaxLength(50).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class BeneficiosPerfilMap : EntityTypeConfiguration<BeneficiosPerfil>
        {
            public BeneficiosPerfilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.TipoBeneficioId).IsRequired();
                Property(x => x.Observaciones).HasMaxLength(500);
                Property(x => x.Cantidad).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }

        }
        public class CompetenciaAreaPerfilMap : EntityTypeConfiguration<CompetenciaAreaPerfil>
        {
            public CompetenciaAreaPerfilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CompetenciaId).IsRequired();
                Property(x => x.Nivel).HasMaxLength(10).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class CompetenciaCardinalPerfilMap : EntityTypeConfiguration<CompetenciaCardinalPerfil>
        {
            public CompetenciaCardinalPerfilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CompetenciaId).IsRequired();
                Property(x => x.Nivel).HasMaxLength(10).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }

        }
        public class CompetenciaGerencialPerfilMap : EntityTypeConfiguration<CompetenciaGerencialPerfil>
        {
            public CompetenciaGerencialPerfilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CompetenciaId).IsRequired();
                Property(x => x.Nivel).HasMaxLength(10).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class DAMFO_290Map : EntityTypeConfiguration<DAMFO_290>
        {
            public DAMFO_290Map()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ClienteId).IsRequired();
                Property(x => x.TipoNominaId).IsRequired();
                Property(x => x.ClaseReclutamientoId).IsRequired();
                Property(x => x.NombrePerfil).HasMaxLength(100);
                Property(x => x.GeneroId).IsRequired();
                Property(x => x.EdadMaxima).IsRequired();
                Property(x => x.EstadoCivilId).IsRequired();
                Property(x => x.Experiencia).HasMaxLength(500).IsRequired();
                Property(x => x.SueldoMinimo).HasPrecision(18, 2).IsRequired();
                Property(x => x.SueldoMaximo).HasPrecision(18, 3).IsRequired();
                Property(x => x.DiaCorteId).IsRequired();
                Property(x => x.TipoNominaId).IsRequired();
                Property(x => x.DiaPagoId).IsRequired();
                Property(x => x.PeriodoPagoId).IsRequired();
                Property(x => x.ContratoInicialId).IsRequired();
                Property(x => x.TiempoContratoId).IsOptional();
                Property(x => x.Activo).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired().IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
                Property(x => x.FlexibilidadHorario).IsRequired();
                Property(x => x.JornadaLaboralId).IsOptional();
                Property(x => x.TipoModalidadId).IsOptional();
                Property(x => x.UsuarioAlta).HasMaxLength(30).IsOptional();
                Property(x => x.UsuarioMod).HasMaxLength(30).IsOptional();
                Property(x => x.Arte).HasMaxLength(50).IsRequired();
            }
        }
        public class DocumentosDamsaMap : EntityTypeConfiguration<DocumentosDamsa>
        {
            public DocumentosDamsaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.documentoDamsa).HasMaxLength(100).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class DocumentosClienteMap : EntityTypeConfiguration<DocumentosCliente>
        {
            public DocumentosClienteMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Documento).HasMaxLength(100).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class DiaObligatorioMap : EntityTypeConfiguration<DiaObligatorio>
        {
            public DiaObligatorioMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.diaObligatorio).HasMaxLength(100).IsRequired();
            }
        }
        public class EscolaridadesPerfilMap : EntityTypeConfiguration<EscolaridadesPerfil>
        {
            public EscolaridadesPerfilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.EscolaridadId).IsRequired();
                Property(x => x.DAMFO290Id).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }

        }
        public class RedSocialMap : EntityTypeConfiguration<RedSocial>
        {
            public RedSocialMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.redSocial).HasMaxLength(100).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class HorarioPerfilMap : EntityTypeConfiguration<HorarioPerfil>
        {
            public HorarioPerfilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.deDiaId).IsRequired();
                Property(x => x.aDiaId).IsRequired();
                Property(x => x.deHora).IsRequired();
                Property(x => x.aHora).IsRequired();
                Property(x => x.numeroVacantes).HasColumnType("tinyint").IsRequired();
                Property(x => x.Especificaciones).HasMaxLength(500);
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class ObservacionesPerfilMap : EntityTypeConfiguration<ObservacionesPerfil>
        {
            public ObservacionesPerfilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Observaciones).HasMaxLength(100).IsRequired();
                Property(x => x.DAMFO290Id).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class PeriodoPagoMap : EntityTypeConfiguration<PeriodoPago>
        {
            public PeriodoPagoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.periodoPago).HasMaxLength(50).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class PrestacionesClientePerfilMap : EntityTypeConfiguration<PrestacionesClientePerfil>
        {
            public PrestacionesClientePerfilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Prestamo).HasMaxLength(100).IsRequired();
                Property(x => x.DAMFO290Id).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class PrestacionesLeyMap : EntityTypeConfiguration<PrestacionLey>
        {
            public PrestacionesLeyMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.prestacionLey).HasMaxLength(50).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class ProcesoPerfilMap : EntityTypeConfiguration<ProcesoPerfil>
        {
            public ProcesoPerfilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Proceso).HasMaxLength(100).IsRequired();
                Property(x => x.Orden).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();

            }
        }
        public class PsicometriasClienteMap : EntityTypeConfiguration<PsicometriasCliente>
        {
            public PsicometriasClienteMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Psicometria).HasMaxLength(50);
                Property(x => x.Descripcion).HasMaxLength(200);
                Property(x => x.DAMFO290Id).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class PsicometriasDamsaMap : EntityTypeConfiguration<PsicometriasDamsa>
        {
            public PsicometriasDamsaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.PsicometriaId).IsRequired();
                Property(x => x.DAMFO290Id).IsRequired();
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class RutasPerfilMap : EntityTypeConfiguration<RutasPerfil>
        {
            public RutasPerfilMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.DireccionId).IsRequired();
                Property(x => x.Ruta).HasMaxLength(100).IsRequired();
                Property(x => x.Via).HasMaxLength(100);
                Property(x => x.UsuarioAlta).IsOptional().HasMaxLength(30);
                Property(x => x.UsuarioMod).IsOptional().HasMaxLength(30);
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
            }
        }
        public class TipoReclutamientoMap : EntityTypeConfiguration<TipoReclutamiento>
        {
            public TipoReclutamientoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tipoReclutamiento).HasMaxLength(50).IsRequired();
                Property(x => x.Activo).IsRequired();
            }
        }
        public class TipoPsicometriaMap : EntityTypeConfiguration<TipoPsicometria>
        {
            public TipoPsicometriaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tipoPsicometria).HasMaxLength(50).IsRequired();
                Property(x => x.descripcion).HasMaxLength(50).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class TipoBeneficioMap : EntityTypeConfiguration<TipoBeneficio>
        {
            public TipoBeneficioMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tipoBeneficio).HasMaxLength(50).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class TipodeNominaMap : EntityTypeConfiguration<TipodeNomina>
        {
            public TipodeNominaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tipoDeNomina).HasMaxLength(100).IsRequired();
                Property(x => x.Clave).HasMaxLength(50).IsRequired();
                Property(x => x.Descripcion).HasMaxLength(200).IsRequired();
                Property(x => x.Tipo).IsRequired();
                Property(x => x.activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class TipoContratoMap : EntityTypeConfiguration<TipoContrato>
        {
            public TipoContratoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.tipoContrato).HasMaxLength(50).IsRequired();
                Property(x => x.periodoPrueba).IsRequired();
                Property(x => x.activo).IsRequired();
            }
        }
        public class ProcesoCandidatoMap : EntityTypeConfiguration<ProcesoCandidato>
        {
            public ProcesoCandidatoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.Folio).IsRequired();
                Property(x => x.ReclutadorId).IsRequired();
                Property(x => x.Reclutador).IsRequired().HasMaxLength(100);
                Property(x => x.EstatusId).IsRequired();
                Property(x => x.TpContrato).IsOptional();
                Property(x => x.HorarioId).IsOptional();
                Property(x => x.TipoMediosId).IsRequired();
                Property(x => x.DepartamentoId).IsRequired();
                Property(x => x.Fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.Fch_Modificacion).IsOptional().HasColumnType("Datetime");
            }
        }
        public class ProcesoCampoMap : EntityTypeConfiguration<ProcesoCampo>
        {
            public ProcesoCampoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.UsuarioId).IsRequired();
                Property(x => x.ReclutadorId).IsRequired();
                Property(x => x.Fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
            }
        }
        public class CfgRequiMap : EntityTypeConfiguration<CfgRequi>
        {
            public CfgRequiMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ConfigMovId).IsRequired();
                Property(x => x.R_D).IsRequired();
                Property(x => x.R).IsRequired();
                Property(x => x.D).IsRequired();
            }
        }
        public class ComentarioEntrevistaMap : EntityTypeConfiguration<ComentarioEntrevista>
        {
            public ComentarioEntrevistaMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RespuestaId).IsOptional();
                Property(x => x.Comentario).HasMaxLength(500).IsRequired();
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.RequisicionId).IsOptional();
                Property(x => x.UsuarioAlta).HasMaxLength(30).IsRequired();
                Property(x => x.ReclutadorId).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
                Property(x => x.MotivoId).IsOptional();
            }
        }
        public class ComentarioVacanteMap : EntityTypeConfiguration<ComentarioVacante>
        {
            public ComentarioVacanteMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RespuestaId).IsOptional();
                Property(x => x.Comentario).HasMaxLength(500).IsRequired();
                Property(x => x.RequisicionId).IsOptional();
                Property(x => x.UsuarioAlta).HasMaxLength(30).IsRequired();
                Property(x => x.ReclutadorId).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
                Property(x => x.MotivoId).IsOptional();
            }
        }

        public class MiCVUploadMap : EntityTypeConfiguration<MiCVUpload>
        {
            public MiCVUploadMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.UrlCV).IsRequired();
            }
        }

        public class CandidatoLiberadoMap : EntityTypeConfiguration<CandidatoLiberado>
        {
            public CandidatoLiberadoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.ReclutadorId).IsRequired();
                Property(x => x.MotivoId).IsRequired();
                Property(x => x.Comentario).HasMaxLength(500).IsRequired();
                Property(x => x.fch_Liberacion).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
            }
        }

        public class MediosMap : EntityTypeConfiguration<Medios>
        {
            public MediosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(50).IsRequired();
                Property(x => x.Activo).IsRequired();
            }
        }

        public class TiposMediosMap : EntityTypeConfiguration<TiposMedios>
        {
            public TiposMediosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.MediosId).IsRequired();
                Property(x => x.Activo).IsRequired();
            }
        }

        public class CandidatosInfoMap : EntityTypeConfiguration<CandidatosInfo>
        {
            public CandidatosInfoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.CURP).HasMaxLength(30).IsRequired();
                Property(x => x.Nombre).HasMaxLength(50).IsRequired();
                Property(x => x.ApellidoMaterno).HasMaxLength(50).IsRequired();
                Property(x => x.ApellidoPaterno).HasMaxLength(50).IsRequired();
                Property(x => x.FechaNacimiento).HasColumnType("date").IsRequired();
                Property(x => x.PaisNacimientoId).IsRequired();
                Property(x => x.EstadoNacimientoId).IsRequired();
                Property(x => x.MunicipioNacimientoId).IsRequired();
                Property(x => x.RFC).IsRequired();
                Property(x => x.NSS).IsRequired();
                Property(x => x.GeneroId).IsRequired();
                Property(x => x.fch_Creacion).IsRequired().HasColumnType("Datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
                Property(x => x.fch_Modificacion).IsRequired().HasColumnType("Datetime");
                Property(x => x.UsuarioMod).IsRequired();
            }
        }

        public class FoliosIncidenciasCandidatosMap : EntityTypeConfiguration<FolioIncidenciasCandidatos>
        {
            public FoliosIncidenciasCandidatosMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.ComentarioId).IsRequired();
                Property(x => x.Folio).IsRequired();
                Property(x => x.EstatusId).IsRequired();
            }
        }

        public class OficioRequisicionMap : EntityTypeConfiguration<OficioRequisicion>
        {
            public OficioRequisicionMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Oficio).IsRequired().HasMaxLength(100);
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.Comentario).HasMaxLength(500).IsOptional();
                Property(x => x.fch_Creacion).HasColumnType("DATETIME").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            }
        }

        public class PonderacionRequisicionesMap : EntityTypeConfiguration<PonderacionRequisiciones>
        {
            public PonderacionRequisicionesMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Ponderacion).IsRequired();
                Property(x => x.RequisicionId).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("DATETIME").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
                Property(x => x.fch_Creacion).HasColumnType("DATETIME").IsOptional();
            }
        }
        public class HistoricoTransDamfoMap : EntityTypeConfiguration<HistoricoTransDamfo>
        {
            public HistoricoTransDamfoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.DAMFO290Id).IsRequired();
                Property(x => x.PropietarioId).IsRequired();
                Property(x => x.TransferidoId).IsRequired();
                Property(x => x.fch_Alta).HasColumnType("DATETIME").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            }
        }

        // captura 
        public class CandidatoGeneralesMap : EntityTypeConfiguration<CandidatosGenerales>
        {
            public CandidatoGeneralesMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoInfoId).IsRequired();
                Property(x => x.PaisId).IsRequired();
                Property(x => x.EstadoId).IsRequired();
                Property(x => x.MunicipioId).IsRequired();
                Property(x => x.ColoniaId).IsRequired();
                Property(x => x.Calle).IsRequired().HasMaxLength(400);
                Property(x => x.CodigoPostal).IsRequired().HasMaxLength(10);
                Property(x => x.NumeroExterior).IsRequired().HasMaxLength(10);
                Property(x => x.NumeroInterior).IsRequired().HasMaxLength(10);
                Property(x => x.EstadoCivilId).IsRequired();
                Property(x => x.GrupoSanguineoId).IsRequired();
                Property(x => x.ImgUrl).IsRequired().HasMaxLength(100);
                Property(x => x.email).IsRequired().HasMaxLength(100);
            }
        }

        public class CandidatoExtrasMap : EntityTypeConfiguration<CandidatosExtras>
        {
            public CandidatoExtrasMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoInfoId).IsRequired();
                Property(x => x.Conyuge).IsRequired().HasMaxLength(200);
                Property(x => x.NomPadre).IsRequired().HasMaxLength(200);
                Property(x => x.NomMadre).IsRequired().HasMaxLength(200);
                Property(x => x.NomBeneficiario).IsRequired().HasMaxLength(200);
                Property(x => x.Nacionalidad).IsRequired().HasMaxLength(100);
                Property(x => x.Observaciones).IsRequired().HasMaxLength(500);
            }
        }

        public class CandidatoLaboralesMap : EntityTypeConfiguration<CandidatoLaborales>
        {
            public CandidatoLaboralesMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoInfoId).IsRequired();
                Property(x => x.FechaIngreso).IsRequired();
                Property(x => x.FormaPagoId).IsRequired();
                Property(x => x.ClaveTurno).IsRequired().HasMaxLength(20);
                Property(x => x.BancoId).IsRequired();
                Property(x => x.NoCuenta).IsRequired().HasMaxLength(20);
                Property(x => x.FechaFormaPago).IsRequired();
                Property(x => x.PuestosIngresosId).IsRequired();
                Property(x => x.ClaveJefe).IsRequired().HasMaxLength(20);
                Property(x => x.ClaveExt).IsRequired().HasMaxLength(20);
                Property(x => x.MotivoId).IsRequired();
                Property(x => x.SueldoMensual).IsRequired();
                Property(x => x.SueldoDiario).IsRequired();
                Property(x => x.SueldoIntegrado).IsRequired();
            }
        }

        public class GafetesMap : EntityTypeConfiguration<Gafetes>
        {
            public GafetesMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatoId).IsRequired();
                Property(x => x.Clave).HasMaxLength(20).IsRequired();
                Property(x => x.Codigo).HasMaxLength(50).IsRequired();
                Property(x => x.fch_Ingreso).IsRequired();
                Property(x => x.UsuarioId).IsRequired();
                Property(x => x.Activo).IsRequired();
            }
        }
        public class ValidacionCURPRFCMap : EntityTypeConfiguration<ValidacionCURPRFC>
        {
            public ValidacionCURPRFCMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.CandidatosInfoId).IsRequired();
                Property(x => x.CURP).IsRequired();
                Property(x => x.RFC).IsRequired();
                Property(x => x.fch_Modificacion).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
            }
        }
        public class DocumentosCandidatoMap : EntityTypeConfiguration<DocumentosCandidato>
        {
            public DocumentosCandidatoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Ruta).HasMaxLength(200).IsRequired();
                Property(x => x.usuarioId).IsRequired();
                Property(x => x.documentoId).IsRequired();
                Property(x => x.candidatoId).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).IsRequired();
                Property(x => x.usuarioMod).IsRequired();
            }
        }
        #endregion

        #region Mapeo FIRM
        public class EmpresasMap : EntityTypeConfiguration<Empresas>
        {
            public EmpresasMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                Property(x => x.Clave).HasMaxLength(20).IsRequired();
                Property(x => x.Observaciones).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class FIRM_EstatusEmailsMap : EntityTypeConfiguration<FIRM_EstatusEmails>
        {
            public FIRM_EstatusEmailsMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.EstatusBitacoraId).IsRequired();
                Property(x => x.ConfigBitacoraId).IsRequired();
                Property(x => x.Email).HasMaxLength(200).IsRequired();
            }
        }
        public class FIRM_BitacoraNominaMap : EntityTypeConfiguration<FIRM_BitacoraNomina>
        {
            public FIRM_BitacoraNominaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Fecha).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.PropietarioId).IsRequired();
                Property(x => x.EstatusBitacoraId).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
                Property(x => x.Retardo).IsRequired();
                Property(x => x.Porques).IsRequired();
            }
        }
        public class FIRM_EstatusNominaMap : EntityTypeConfiguration<FIRM_EstatusNomina>
        {
            public FIRM_EstatusNominaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Estatus).IsRequired().HasMaxLength(100);
                Property(x => x.Observaciones).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class FIRM_EstatusBitacoraMap : EntityTypeConfiguration<FIRM_EstatusBitacora>
        {
            public FIRM_EstatusBitacoraMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Estatus).IsRequired().HasMaxLength(100);
                Property(x => x.Observaciones).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.Tipo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class FIRM_SoportesNominaMap : EntityTypeConfiguration<FIRM_SoportesNomina>
        {
            public FIRM_SoportesNominaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Soporte).IsRequired().HasMaxLength(100);
                Property(x => x.Observaciones).HasMaxLength(200).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }
        }
        public class FIRM_SoporteSucursalMap : EntityTypeConfiguration<FIRM_SoporteSucursal>
        {
            public FIRM_SoporteSucursalMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.SoportesNominaId).IsRequired();
                Property(x => x.SucursalesId).IsRequired();
            }
        }
        public class FIRM_ConfigBitacoraMap : EntityTypeConfiguration<FIRM_ConfigBitacora>
        {
            public FIRM_ConfigBitacoraMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.TipodeNominaId).IsRequired();
                Property(x => x.EmpresasId).IsRequired();
                Property(x => x.SoportesNominaId).IsRequired();
                Property(x => x.Destinatario).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }
        }
        public class FIRM_RPMap : EntityTypeConfiguration<FIRM_RP>
        {
            public FIRM_RPMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.RP_Clave).IsRequired().HasMaxLength(20);
                Property(x => x.RP_Base).IsRequired().HasMaxLength(200);
                Property(x => x.RP_IMSS).IsRequired().HasMaxLength(100);
                Property(x => x.Activo).IsRequired();
                Property(x => x.Observaciones).IsRequired().HasMaxLength(300);
                Property(x => x.UsuarioAlta).IsRequired();
                Property(x => x.UsuarioMod).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();

            }
        }
        public class FIRM_RPEmpresasMap : EntityTypeConfiguration<FIRM_RP_Empresas>
        {
            public FIRM_RPEmpresasMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.EmpresasId).IsRequired();
                Property(x => x.FIRM_RPId).IsRequired();
            }
        }
        public class FIRM_BitacoraMap : EntityTypeConfiguration<FIRM_Bitacora>
        {
            public FIRM_BitacoraMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.FechasEstatusId).IsRequired();
                Property(x => x.FilePath).IsRequired().HasMaxLength(200);
                Property(x => x.Activo).IsRequired();
                Property(x => x.PropietarioId).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
                Property(x => x.Retardo).IsRequired();
                Property(x => x.Porques).IsRequired();

            }
        }
        public class FIRM_FechasEstatusMap : EntityTypeConfiguration<FIRM_FechasEstatus>
        {
            public FIRM_FechasEstatusMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Year).IsRequired();
                Property(x => x.Month).IsRequired();
                Property(x => x.Day).IsRequired();
                Property(x => x.WeekDay).IsRequired();
                Property(x => x.EstatusBitacoraId).IsRequired();
                Property(x => x.ConfigBitacoraId).IsRequired();
                Property(x => x.Hour).HasColumnType("datetime").IsRequired();
                Property(x => x.Fecha).IsRequired();

            }
        }
        public class FIRM_Damfo022Map : EntityTypeConfiguration<FIRM_Damfo022>
        {
            public FIRM_Damfo022Map()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Folio).IsRequired().HasMaxLength(13);
                Property(x => x.Fecha).IsRequired();
                Property(x => x.Descripcion).IsRequired().HasMaxLength(500);
                Property(x => x.Problema).IsRequired().HasMaxLength(200);
                Property(x => x.Causa_Raiz).IsRequired().HasMaxLength(200);
                Property(x => x.SolucionTmp).IsRequired().HasMaxLength(500);
                Property(x => x.Solucion).IsRequired().HasMaxLength(500);
                Property(x => x.Estatus).IsRequired();
                Property(x => x.Activo).IsRequired();
                Property(x => x.fch_Creacion).HasColumnType("datetime").IsRequired();
                Property(x => x.fch_Modificacion).HasColumnType("datetime").IsRequired();
            }
        }
        public class FIRM_IshikawaMap : EntityTypeConfiguration<FIRM_Ishikawa>
        {
            public FIRM_IshikawaMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Causa).HasMaxLength(100).IsRequired();
                Property(x => x.Activo).IsRequired();
            }
        }
        public class FIRM_PorquesMap : EntityTypeConfiguration<FIRM_Porques>
        {
            public FIRM_PorquesMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Damfo022Id).IsRequired();
                Property(x => x.Porque).IsRequired().HasMaxLength(300);
            }
        }
        public class FIRM_CausaEfectoMap : EntityTypeConfiguration<FIRM_CausaEfecto>
        {
            public FIRM_CausaEfectoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Damfo022Id).IsRequired();
                Property(x => x.IshikawaId).IsRequired();
            }
        }
        public class FIRM_CompromisoMap : EntityTypeConfiguration<FIRM_Compromiso>
        {
            public FIRM_CompromisoMap()
            {
                HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Actividad).IsRequired().HasMaxLength(300);
                Property(x => x.Responsable).HasMaxLength(100).IsRequired();
                Property(x => x.Fecha).IsRequired();
                Property(x => x.Damfo022Id).IsRequired();
            }
        }
        #endregion
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<PersonasMap>().Property(p => p.Email).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsUnique = true }));

            modelBuilder.HasDefaultSchema("Sist");

            //modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            //modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            //modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });

            #region Administracion_Sistema_Sist

            modelBuilder.Configurations.Add(new RelacionClientesSistemasMap().ToTable("RelacionClientesSistemas", "dbo"));
            modelBuilder.Configurations.Add(new EstructuraMap().ToTable("Estructuras"));
            modelBuilder.Configurations.Add(new TipoEstructuraMap().ToTable("TiposEstructuras"));
            modelBuilder.Configurations.Add(new AmbitoMap().ToTable("Ambitos"));
            modelBuilder.Configurations.Add(new AreaMap().ToTable("Areas"));
            modelBuilder.Configurations.Add(new ColoniasMap().ToTable("Colonias"));
            modelBuilder.Configurations.Add(new CompetenciasAreasMap().ToTable("CompetenciasAreas"));
            modelBuilder.Configurations.Add(new CompetenciasCardinalesMap().ToTable("CompetenciasCardinales"));
            modelBuilder.Configurations.Add(new CompeteciasGerencialesMap().ToTable("CompetenciasGerenciales"));
            modelBuilder.Configurations.Add(new DireccionMap().ToTable("Direcciones"));
            modelBuilder.Configurations.Add(new EstadoMap().ToTable("Estados"));
            modelBuilder.Configurations.Add(new EstatusMap().ToTable("Estatus"));
            modelBuilder.Configurations.Add(new EmailMap().ToTable("Emails"));
            modelBuilder.Configurations.Add(new EstadoCivilMap().ToTable("EstadosCiviles"));
            modelBuilder.Configurations.Add(new EstadoEstudioMap().ToTable("EstadosEstudios"));
            modelBuilder.Configurations.Add(new GeneroMap().ToTable("Generos"));
            modelBuilder.Configurations.Add(new GradoEstudioMap().ToTable("GradosEstudios"));
            modelBuilder.Configurations.Add(new GruposMap().ToTable("Grupos"));
            modelBuilder.Configurations.Add(new GiroEmpresaMap().ToTable("GiroEmpresas"));
            modelBuilder.Configurations.Add(new JornadaLaboralMap().ToTable("JornadasLaborales"));
            modelBuilder.Configurations.Add(new MunicipioMap().ToTable("Municipios"));
            modelBuilder.Configurations.Add(new NivelMap().ToTable("Niveles"));
            modelBuilder.Configurations.Add(new PaisMap().ToTable("Paises"));
            modelBuilder.Configurations.Add(new PrioridadMap().ToTable("Prioridades"));
            modelBuilder.Configurations.Add(new RedSocialMap().ToTable("RedesSociales"));
            modelBuilder.Configurations.Add(new RolesMap().ToTable("Roles"));
            modelBuilder.Configurations.Add(new TamanoEmpresaMap().ToTable("TamanosEmpresas"));
            modelBuilder.Configurations.Add(new TelefonoMap().ToTable("Telefonos"));
            modelBuilder.Configurations.Add(new TiempoContratoMap().ToTable("TiemposContratos"));
            modelBuilder.Configurations.Add(new TipoBaseMap().ToTable("TiposBases"));
            modelBuilder.Configurations.Add(new TipoDireccionMap().ToTable("TiposDirecciones"));
            modelBuilder.Configurations.Add(new TipoEmpresaMap().ToTable("TiposEmpresas"));
            modelBuilder.Configurations.Add(new TipoTelefonoMap().ToTable("TiposTelefonos"));
            modelBuilder.Configurations.Add(new TipoUsuarioMap().ToTable("TiposUsuarios"));
            modelBuilder.Configurations.Add(new TipoBeneficioMap().ToTable("TiposBeneficios"));
            modelBuilder.Configurations.Add(new TipoContratoMap().ToTable("TiposContratos"));
            modelBuilder.Configurations.Add(new TipoModalidadMap().ToTable("TiposModalidades"));
            modelBuilder.Configurations.Add(new TipodeNominaMap().ToTable("TiposNominas"));
            modelBuilder.Configurations.Add(new TipoPsicometriaMap().ToTable("TiposPsicometrias"));
            modelBuilder.Configurations.Add(new TipoReclutamientoMap().ToTable("TiposReclutamientos"));
            modelBuilder.Configurations.Add(new UsuariosMap().ToTable("Usuarios"));
            modelBuilder.Configurations.Add(new TipoAccionMap().ToTable("TiposAcciones"));
            modelBuilder.Configurations.Add(new TipoMovimientoMap().ToTable("TiposMovimientos"));
            modelBuilder.Configurations.Add(new TrazabilidadMesMap().ToTable("TrazabilidadMes"));
            modelBuilder.Configurations.Add(new RastreabilidadMesMap().ToTable("RastreabilidadMes"));
            modelBuilder.Configurations.Add(new FoliosMap().ToTable("Folios"));
            modelBuilder.Configurations.Add(new PrivilegiosMap().ToTable("Privilegios"));
            modelBuilder.Configurations.Add(new DepartamentoMap().ToTable("Departamentos"));
            modelBuilder.Configurations.Add(new TipoEntidadMap().ToTable("TiposEntidades"));
            modelBuilder.Configurations.Add(new TratamientoMap().ToTable("Tratamientos"));
            modelBuilder.Configurations.Add(new OficinaReclutamientoMap().ToTable("OficinasReclutamiento"));
            modelBuilder.Configurations.Add(new TipoOficinaMap().ToTable("TiposOficinas"));
            modelBuilder.Configurations.Add(new RolEntidadesMap().ToTable("RolEntidades"));
            modelBuilder.Configurations.Add(new ConfiguracionMovsMap().ToTable("ConfiguracionesMovs"));
            modelBuilder.Configurations.Add(new LogsIngresosMap().ToTable("LogsIngresos"));
            modelBuilder.Configurations.Add(new MotivoLiberacioMap().ToTable("MotivosLiberaciones"));
            modelBuilder.Configurations.Add(new FolioIncidenciaMap().ToTable("FolioIncidencias"));
            modelBuilder.Configurations.Add(new PuestoMap().ToTable("Puestos"));
            modelBuilder.Configurations.Add(new PuestosIngresosMap().ToTable("PuestosIngresos"));
            modelBuilder.Configurations.Add(new CalendarioEventMap().ToTable("CalendarioEvent"));
            modelBuilder.Configurations.Add(new TipoActividadReclutadorMap().ToTable("TipoActividadReclutador"));
            modelBuilder.Configurations.Add(new AlertasStmMap().ToTable("AlertasStm"));
            modelBuilder.Configurations.Add(new TipoAlertaMap().ToTable("TiposAlertas"));
            modelBuilder.Configurations.Add(new SubordinadosMap().ToTable("Subordinados"));
            modelBuilder.Configurations.Add(new UnidadesNegociosMap().ToTable("UnidadesNegocios"));
            modelBuilder.Configurations.Add(new TipoExamenMedicoMap().ToTable("TiposExamenesMedicos"));
            modelBuilder.Configurations.Add(new TitulosArteMap().ToTable("TitulosArte"));
            modelBuilder.Configurations.Add(new ArteRequiMap().ToTable("ArteRequi"));
          
            //Catalogos
            modelBuilder.Configurations.Add(new CatalogosMap().ToTable("Catalogos"));
            modelBuilder.Configurations.Add(new LogCatalogosMap().ToTable("LogCatalogos"));
            modelBuilder.Configurations.Add(new CatalogoBancosMap().ToTable("CatalogoBancos"));
            modelBuilder.Configurations.Add(new EmpresaBancosMap().ToTable("EmpresaBancosMap"));
            modelBuilder.Configurations.Add(new MotivosContratacionMap().ToTable("MotivosContratacion"));
            modelBuilder.Configurations.Add(new GrupoSanguineoMap().ToTable("GrupoSanguineo"));
            modelBuilder.Configurations.Add(new FormaPagoMap().ToTable("FormaPago"));
            modelBuilder.Configurations.Add(new TiposDocumentosMap().ToTable("TiposDocumentos"));
            modelBuilder.Configurations.Add(new DocumentosMap().ToTable("Documentos"));
            modelBuilder.Configurations.Add(new TiposBonoMap().ToTable("TiposBono"));
            modelBuilder.Configurations.Add(new TipoPeriodosMap().ToTable("TipoPeriodos"));
            //Modulo para examenes
            modelBuilder.Configurations.Add(new ExamenesMap().ToTable("Examenes"));
            modelBuilder.Configurations.Add(new TipoExamenesMap().ToTable("TipoExamen"));
            modelBuilder.Configurations.Add(new PreguntasMap().ToTable("Preguntas"));
            modelBuilder.Configurations.Add(new RespuestasMap().ToTable("Respuestas"));
            modelBuilder.Configurations.Add(new RequiExamenMap().ToTable("RequiExamen"));
            modelBuilder.Configurations.Add(new ExamenCandidatoMap().ToTable("ExamenCandidato"));
            modelBuilder.Configurations.Add(new ResultadosCandidatoMap().ToTable("ResultadosCandidato"));
            modelBuilder.Configurations.Add(new RequiClavesMap().ToTable("RequiClaves"));
            modelBuilder.Configurations.Add(new PsicometriaCandidatosMap().ToTable("PsicometriaCandidatos"));
            modelBuilder.Configurations.Add(new MedicoCandidatoMap().ToTable("MedicoCandidato"));
            modelBuilder.Configurations.Add(new ConfigEntrevistaMap().ToTable("ConfigEntrevistas"));


            //Preguntas Frecuentes 
            modelBuilder.Configurations.Add(new PreguntasFrecuentesMap().ToTable("PreguntasFrecuentes"));

            modelBuilder.Configurations.Add(new TransferenciasMap().ToTable("Transferencias"));
            modelBuilder.Configurations.Add(new TiposTransferenciasMap().ToTable("TiposTransferencias"));

            modelBuilder.Entity<AspNetUsers>().ToTable("AspNetUsers");

            //Tabla de Versiones del Sistema.
            modelBuilder.Configurations.Add(new VertionSistemMap().ToTable("VertionSistem"));
            #endregion

            #region BolsaTrabajo_BTra
            modelBuilder.Configurations.Add(new AboutMeMap().ToTable("AcercaDeMi", "BTra"));
            modelBuilder.Configurations.Add(new AreaExperienciaMap().ToTable("AreasExperiencia", "BTra"));
            modelBuilder.Configurations.Add(new AreaInteresMap().ToTable("AreasInteres", "BTra"));
            modelBuilder.Configurations.Add(new CarreraMap().ToTable("Carreras", "BTra"));
            modelBuilder.Configurations.Add(new CertificacionMap().ToTable("Certificaciones", "BTra"));
            modelBuilder.Configurations.Add(new ConocimientoOHabilidadMap().ToTable("ConocimientosHabilidades", "BTra"));
            modelBuilder.Configurations.Add(new CursoMap().ToTable("Cursos", "BTra"));
            modelBuilder.Configurations.Add(new DocumentoValidadorMap().ToTable("DocumentosValidadores", "BTra"));
            modelBuilder.Configurations.Add(new FormulariosInicialesMap().ToTable("FormulariosIniciales", "BTra"));
            modelBuilder.Configurations.Add(new FormacionMap().ToTable("Formaciones", "BTra"));
            modelBuilder.Configurations.Add(new FormaContactoMap().ToTable("FormasContacto", "BTra"));
            modelBuilder.Configurations.Add(new ExperienciaProfesionalMap().ToTable("ExperienciasProfesionales", "BTra"));
            modelBuilder.Configurations.Add(new InstitucionEducativaMap().ToTable("InstitucionesEducativas", "BTra"));
            modelBuilder.Configurations.Add(new PerfilCandidatoMap().ToTable("PerfilCandidato", "BTra"));
            modelBuilder.Configurations.Add(new PerfilExperienciaMap().ToTable("PerfilExperiencia", "BTra"));
            modelBuilder.Configurations.Add(new PostulacionMap().ToTable("Postulaciones", "BTra"));
            modelBuilder.Configurations.Add(new StatusPostulacionMap().ToTable("StatusPostulaciones", "BTra"));
            modelBuilder.Configurations.Add(new TipoDiscapacidadMap().ToTable("TiposDiscapacidades", "BTra"));
            modelBuilder.Configurations.Add(new TipoLicenciaMap().ToTable("TiposLicencias", "BTra"));
            modelBuilder.Configurations.Add(new TipoRedSocialMap().ToTable("TiposRedesSociales", "BTra"));
            modelBuilder.Configurations.Add(new PerfilIdimoasMap().ToTable("PerfilIdiomas", "BTra"));
            modelBuilder.Configurations.Add(new FrecuenciasMap().ToTable("Frecuencias", "BTra"));
            modelBuilder.Configurations.Add(new AlertasdtlMap().ToTable("Alertasdtl", "BTra"));
            modelBuilder.Configurations.Add(new DpTpDiscapacidadMap().ToTable("DpTpDiscapacidad", "BTra"));
            modelBuilder.Configurations.Add(new MiCVUploadMap().ToTable("MiCVUpload", "BTra"));
            modelBuilder.Configurations.Add(new HorariosCalendarioMap().ToTable("HorariosCalendario", "BTra"));
            modelBuilder.Configurations.Add(new CalendarioCandidatoMap().ToTable("CalendarioCandidato", "BTra"));
            modelBuilder.Configurations.Add(new AvancePerfilMap().ToTable("AvancePerfil", "BTra"));
            #endregion

            #region Reclutamiento_Recl
            modelBuilder.Configurations.Add(new ConfiguracionRequiMap().ToTable("ConfiguracionRequi", "Recl"));
            modelBuilder.Configurations.Add(new ActividadesPerfilMap().ToTable("ActividadesPerfil", "Recl"));
            modelBuilder.Configurations.Add(new ActividadesPerfilesMap().ToTable("ActividadesPerfiles", "Recl"));
            modelBuilder.Configurations.Add(new PerfilesDamfoMap().ToTable("PerfilesDamfo", "Recl"));
            modelBuilder.Configurations.Add(new PerfilDamfoRelMap().ToTable("PerfilDamfoRel", "Recl"));
            modelBuilder.Configurations.Add(new AptitudMap().ToTable("Aptitudes", "Recl"));
            modelBuilder.Configurations.Add(new AptitudesPerfilMap().ToTable("AptitudesPerfil", "Recl"));
            modelBuilder.Configurations.Add(new BeneficiosPerfilMap().ToTable("BeneficiosPerfil", "Recl"));
            modelBuilder.Configurations.Add(new CompetenciaAreaPerfilMap().ToTable("CompetenciaAreaPerfil", "Recl"));
            modelBuilder.Configurations.Add(new CompetenciaCardinalPerfilMap().ToTable("CompetenciaCardinalPerfil", "Recl"));
            modelBuilder.Configurations.Add(new CompetenciaGerencialPerfilMap().ToTable("CompetenciaGerencialPerfil", "Recl"));
            modelBuilder.Configurations.Add(new ClaseReclutamientoMap().ToTable("ClasesReclutamientos", "Recl"));
            modelBuilder.Configurations.Add(new DAMFO_290Map().ToTable("DAMFO_290", "Recl"));
            modelBuilder.Configurations.Add(new DiaObligatorioMap().ToTable("DiasObligatorios", "Recl"));
            modelBuilder.Configurations.Add(new DiaSemanaMap().ToTable("DiasSemana", "Recl"));
            modelBuilder.Configurations.Add(new DocumentosDamsaMap().ToTable("DocumentosDamsa", "Recl"));
            modelBuilder.Configurations.Add(new DocumentosClienteMap().ToTable("DocumentosClientes", "Recl"));
            modelBuilder.Configurations.Add(new EscolaridadesPerfilMap().ToTable("EscolaridadesPerfil", "Recl"));
            modelBuilder.Configurations.Add(new HorarioPerfilMap().ToTable("HorariosPerfiles", "Recl"));
            modelBuilder.Configurations.Add(new ObservacionesPerfilMap().ToTable("ObservacionesPerfil", "Recl"));
            modelBuilder.Configurations.Add(new PeriodoPagoMap().ToTable("PeriodosPagos", "Recl"));
            modelBuilder.Configurations.Add(new PrestacionesLeyMap().ToTable("PrestacionesLey", "Recl"));
            modelBuilder.Configurations.Add(new PrestacionesClientePerfilMap().ToTable("PrestacionesClientePerfil", "Recl"));
            modelBuilder.Configurations.Add(new ProcesoCandidatoMap().ToTable("ProcesoCandidatos", "Recl"));
            modelBuilder.Configurations.Add(new ProcesoCampoMap().ToTable("ProcesoCampo", "Recl"));
            modelBuilder.Configurations.Add(new PsicometriasDamsaMap().ToTable("PsicometriasDamsa", "Recl"));
            modelBuilder.Configurations.Add(new PsicometriasClienteMap().ToTable("PsicometriasCliente", "Recl"));
            modelBuilder.Configurations.Add(new ProcesoPerfilMap().ToTable("ProcesoPerfil", "Recl"));
            modelBuilder.Configurations.Add(new RutasPerfilMap().ToTable("RutasPerfil", "Recl"));
            modelBuilder.Configurations.Add(new CfgRequiMap().ToTable("CfgRequi", "Recl"));
            modelBuilder.Configurations.Add(new ComentarioEntrevistaMap().ToTable("ComentariosEntrevistas", "Recl"));
            modelBuilder.Configurations.Add(new ComentarioVacanteMap().ToTable("ComentariosVacantes", "Recl"));
            modelBuilder.Configurations.Add(new CandidatoLiberadoMap().ToTable("CandidatosLiberados", "Recl"));
            modelBuilder.Configurations.Add(new MediosMap().ToTable("Medios", "Recl"));
            modelBuilder.Configurations.Add(new TiposMediosMap().ToTable("TiposMedios", "Recl"));
            modelBuilder.Configurations.Add(new CandidatosInfoMap().ToTable("CandidatosInfo", "Recl"));
            modelBuilder.Configurations.Add(new FoliosIncidenciasCandidatosMap().ToTable("FoliosIncidenciasCandidatos", "Recl"));
            modelBuilder.Configurations.Add(new OficioRequisicionMap().ToTable("OficiosRequisicion", "Recl"));
            modelBuilder.Configurations.Add(new PonderacionRequisicionesMap().ToTable("PonderacionRequisiciones", "Recl"));
            modelBuilder.Configurations.Add(new HistoricoTransDamfoMap().ToTable("HistoricoTransDamfo", "Recl"));
            modelBuilder.Configurations.Add(new CandidatoGeneralesMap().ToTable("CandidatoGenerales", "Recl"));
            modelBuilder.Configurations.Add(new CandidatoExtrasMap().ToTable("CandidatoExtras", "Recl"));
            modelBuilder.Configurations.Add(new CandidatoLaboralesMap().ToTable("CandidatoLaborales", "Recl"));
            modelBuilder.Configurations.Add(new DocumentosCandidatoMap().ToTable("DocumentosCandidato", "Recl"));
            modelBuilder.Configurations.Add(new GafetesMap().ToTable("Gafetes", "Recl"));
            modelBuilder.Configurations.Add(new ValidacionCURPRFCMap().ToTable("ValidacionCURPRFC", "Recl"));
            #endregion

            #region Ventas_Vtas			
            modelBuilder.Configurations.Add(new ActividadEmpMap().ToTable("ActividadEmpresas", "Vtas"));
            modelBuilder.Configurations.Add(new ActividadesRequilMap().ToTable("ActividadesRequi", "Vtas"));
            modelBuilder.Configurations.Add(new AgenciaMap().ToTable("Agencias", "Vtas"));
            modelBuilder.Configurations.Add(new AptitudesRequiMap().ToTable("AptitudesRequi", "Vtas"));
            modelBuilder.Configurations.Add(new AsignacionRequiMap().ToTable("AsignacionesRequi", "Vtas"));
            modelBuilder.Configurations.Add(new BeneficiosRequiMap().ToTable("BeneficiosRequi", "Vtas"));
            modelBuilder.Configurations.Add(new CompetenciasAreasRequiMap().ToTable("CompetenciasAreasRequi", "Vtas"));
            modelBuilder.Configurations.Add(new CompetenciaCardinalRequilMap().ToTable("CompetenciasCardinalesRequi", "Vtas"));
            modelBuilder.Configurations.Add(new CompetenciaGerencialRequiMap().ToTable("CompetenciasGerencialesRequi", "Vtas"));
            modelBuilder.Configurations.Add(new DocumentosClienteRequiMap().ToTable("DocumentosClienteRequi", "Vtas"));
            modelBuilder.Configurations.Add(new EscolaridadesRequiMap().ToTable("EscolaridadesRequi", "Vtas"));
            modelBuilder.Configurations.Add(new HorarioRequiMap().ToTable("HorariosRequi", "Vtas"));
            modelBuilder.Configurations.Add(new HorariosDireccionesRequiMap().ToTable("HorariosDireccionesRequi", "Vtas"));
            modelBuilder.Configurations.Add(new ObservacionesRequiMap().ToTable("ObservacionesRequi", "Vtas"));
            modelBuilder.Configurations.Add(new ProcesoRequiMap().ToTable("ProcesosRequi", "Vtas"));
            modelBuilder.Configurations.Add(new PrestacionesClienteRequiMap().ToTable("PrestacionesClienteRequi", "Vtas"));
            modelBuilder.Configurations.Add(new PsicometriasDamsaRequiMap().ToTable("PsicometriasDamsaRequi", "Vtas"));
            modelBuilder.Configurations.Add(new PsicometriasClienteRequiMap().ToTable("PsicometriasClienteRequi", "Vtas"));
            modelBuilder.Configurations.Add(new RequisicionMap().ToTable("Requisiciones", "Vtas"));
            modelBuilder.Configurations.Add(new InformeRequisicionesMap().ToTable("InformeRequisicion", "Vtas"));
            modelBuilder.Configurations.Add(new InformeCandidatosMap().ToTable("InformeCandidatos", "Recl"));
            modelBuilder.Configurations.Add(new EstatusRequisicionesMap().ToTable("EstatusRequisiciones", "Vtas"));
            modelBuilder.Configurations.Add(new FacturacionPuroMap().ToTable("FacturacionPuro", "Vtas"));
            modelBuilder.Configurations.Add(new DireccionTelefonoMap().ToTable("DireccionesTelefonos", "Vtas"));
            modelBuilder.Configurations.Add(new DireccionEmailMap().ToTable("DireccionesEmails", "Vtas"));
            modelBuilder.Configurations.Add(new DireccionContactoMap().ToTable("DireccionesContactos", "Vtas"));
            modelBuilder.Configurations.Add(new CostosMap().ToTable("Costos", "Vtas"));
            modelBuilder.Configurations.Add(new TipoCostosMap().ToTable("TipoCostos", "Vtas"));
            modelBuilder.Configurations.Add(new CostosDamfo290Map().ToTable("CostosDamfo290", "Vtas"));
            modelBuilder.Configurations.Add(new ExamenMedicoClienteMap().ToTable("ExamenesMedicosCliente", "Vtas"));
            modelBuilder.Configurations.Add(new EntrevistasMap().ToTable("Entrevistas", "Sist"));
            #endregion

            #region Gestion de Personal GePe
            modelBuilder.Configurations.Add(new JornadaMap().ToTable("Jornada", "GePe"));
            #region ASIGNACIONES
            modelBuilder.Configurations.Add(new PeriodoActaMap().ToTable("PeriodoActa", "GePe"));
            modelBuilder.Configurations.Add(new PeriodoCompensacionesMap().ToTable("PeriodoCompensaciones", "GePe"));
            modelBuilder.Configurations.Add(new PeriodoHorasExtrasMap().ToTable("PeriodoHorasExtras", "GePe"));
            modelBuilder.Configurations.Add(new PeriodoReconocimientoMap().ToTable("PeriodoReconocimiento", "GePe"));
            modelBuilder.Configurations.Add(new PeriodoMemoMap().ToTable("PeriodoMemo", "GePe"));
            modelBuilder.Configurations.Add(new PeriodoSuspensionMap().ToTable("PeriodoSuspension", "GePe"));
            modelBuilder.Configurations.Add(new PeriodoGuardiaMap().ToTable("PeriodoGuardia", "GePe"));
            modelBuilder.Configurations.Add(new PeriodoPermisosMap().ToTable("PeriodoPermisos", "GePe"));
            modelBuilder.Configurations.Add(new PeriodoIncapacidadMap().ToTable("PeriodoIncapacidad", "GePe"));
            modelBuilder.Configurations.Add(new PeriodoDEMap().ToTable("PeriodoDE", "GePe"));
            modelBuilder.Configurations.Add(new PeriodoVacacionesMap().ToTable("PeriodoVacaciones", "GePe"));
            modelBuilder.Configurations.Add(new PeriodoBonoMap().ToTable("PeriodoBonos", "GePe"));
            #endregion

            #region Sist ingresos
            modelBuilder.Configurations.Add(new SoporteFacturacionMap().ToTable("SoporteFacturacion", "GePe"));
            modelBuilder.Configurations.Add(new EmpleadosSoporteMap().ToTable("EmpleadosSoporte", "GePe"));
            modelBuilder.Configurations.Add(new SoporteSucursalesMap().ToTable("SoporteSucursales", "GePe"));
            modelBuilder.Configurations.Add(new SoportePuestosMap().ToTable("SoportePuestos", "GePe"));
            modelBuilder.Configurations.Add(new SoporteDptoIngresosMap().ToTable("SoporteDptoIngresos", "GePe"));
            modelBuilder.Configurations.Add(new EstatusLaboralMap().ToTable("EstatusLaboral", "GePe"));
            modelBuilder.Configurations.Add(new PuestosClienteMap().ToTable("PuestosCliente", "GePe"));
            modelBuilder.Configurations.Add(new SucursalesMap().ToTable("Sucursales", "GePe"));
            modelBuilder.Configurations.Add(new RegistroPatronalMap().ToTable("RegistroPatronal", "GePe"));
            modelBuilder.Configurations.Add(new JustificacionTrabajoMap().ToTable("JustificacionTrabajo", "GePe"));
            modelBuilder.Configurations.Add(new DiasFestivosMap().ToTable("DiasFestivos", "GePe"));
            modelBuilder.Configurations.Add(new HorariosIngresosMap().ToTable("HorariosIngresos", "GePe"));
            modelBuilder.Configurations.Add(new DiasHorasIngresosMap().ToTable("DiasHorasIngresos", "GePe"));
            modelBuilder.Configurations.Add(new DiasHorasEspecialMap().ToTable("DiasHorasEspecial", "GePe"));
            modelBuilder.Configurations.Add(new TurnosHorariosMap().ToTable("TurnosHorarios", "GePe"));
            
            modelBuilder.Configurations.Add(new DptosIngresosMap().ToTable("DptosIngresos", "GePe"));
            modelBuilder.Configurations.Add(new BiometricosFPMap().ToTable("BiometricosFP", "GePe"));
            modelBuilder.Configurations.Add(new CatalogoClientesMap().ToTable("CatalogoClientes", "GePe"));

            //configuraciones 
            modelBuilder.Configurations.Add(new TiposConfiguracionesMap().ToTable("TiposConfiguraciones", "GePe"));
            modelBuilder.Configurations.Add(new TiempoAntiguedadMap().ToTable("TiempoAntiguedad", "GePe"));
            modelBuilder.Configurations.Add(new ConfigVacacionesMap().ToTable("ConfigVacaciones", "GePe"));
            modelBuilder.Configurations.Add(new ConfigVacacionesDiasMap().ToTable("ConfigVacacionesDias", "GePe"));
            modelBuilder.Configurations.Add(new EmpleadoVacacionesMap().ToTable("EmpleadoVacaciones", "GePe"));
            modelBuilder.Configurations.Add(new ConfigIncapacidadesMap().ToTable("ConfigIncapacidades", "GePe"));
            modelBuilder.Configurations.Add(new ConfigIncapacidadesDiasMap().ToTable("ConfigIncapacidadesDias", "GePe"));
            modelBuilder.Configurations.Add(new TiposIncapacidadMap().ToTable("TiposIncapacidad", "GePe"));
            modelBuilder.Configurations.Add(new EmpleadoIncapacidadMap().ToTable("EmpleadoIncapacidad", "GePe"));
            modelBuilder.Configurations.Add(new ConfigTiempoExtraMap().ToTable("ConfigTiempoExtra", "GePe"));
            modelBuilder.Configurations.Add(new EmpleadoTiempoExtraMap().ToTable("EmpleadoTiempoExtra", "GePe"));
            modelBuilder.Configurations.Add(new ConfigSuspensionNotasMap().ToTable("ConfigSuspensionNotas", "GePe"));
            modelBuilder.Configurations.Add(new ConfigSuspensionNotasDiasMap().ToTable("ConfigSuspensionNotasDias", "GePe"));
            modelBuilder.Configurations.Add(new EmpleadoSuspensionMap().ToTable("EmpleadoSuspension", "GePe"));
            modelBuilder.Configurations.Add(new ConfigGuardiasMap().ToTable("ConfigGuardias", "GePe"));
            modelBuilder.Configurations.Add(new EmpleadoGuardiaMap().ToTable("EmpleadoGuardia", "GePe"));
            modelBuilder.Configurations.Add(new TiposDiasEconomicosMap().ToTable("TiposDiasEconomicos", "GePe"));
            modelBuilder.Configurations.Add(new ConfigDiasEconomicosMap().ToTable("ConfigDiasEconomicos", "GePe"));
            modelBuilder.Configurations.Add(new ConfigDiasEconomicosDiasMap().ToTable("ConfigDiasEconomicosDias", "GePe"));
            modelBuilder.Configurations.Add(new EmpleadoDiasEconomicosMap().ToTable("EmpleadoDiasEconomicos", "GePe"));
            modelBuilder.Configurations.Add(new ConfigPrimaMap().ToTable("ConfigPrima", "GePe"));
            modelBuilder.Configurations.Add(new EmpleadoPrimaMap().ToTable("EmpleadoPrima", "GePe"));
            modelBuilder.Configurations.Add(new ConfigToleranciaMap().ToTable("ConfigTolerancia", "GePe"));
            modelBuilder.Configurations.Add(new ConfigToleranciaTiempoMap().ToTable("ConfigToleranciaTiempo", "GePe"));
            modelBuilder.Configurations.Add(new EmpleadoToleranciaMap().ToTable("EmpleadoTolerancia", "GePe"));
            modelBuilder.Configurations.Add(new EmpleadoHorarioMap().ToTable("EmpleadoHorario", "GePe"));
            modelBuilder.Configurations.Add(new GrupoEmpleadosMap().ToTable("GrupoEmpleados", "GePe"));
            modelBuilder.Configurations.Add(new ConfigBonoMap().ToTable("ConfigBono", "GePe"));
            modelBuilder.Configurations.Add(new EmpleadoBonoMap().ToTable("EmpleadoBono", "GePe"));
            #endregion
            #endregion

            #region Firm Firmas
            modelBuilder.Configurations.Add(new EmpresasMap().ToTable("Empresas", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_EstatusBitacoraMap().ToTable("EstatusBitacora", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_SoportesNominaMap().ToTable("SoportesNomina", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_SoporteSucursalMap().ToTable("SoporteSucursal", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_ConfigBitacoraMap().ToTable("ConfigBitacora", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_BitacoraMap().ToTable("Bitacora", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_RPMap().ToTable("RP_Firmas", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_RPEmpresasMap().ToTable("RPEmpresas", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_Damfo022Map().ToTable("Damfo022", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_IshikawaMap().ToTable("Ishikawa", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_PorquesMap().ToTable("Porques", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_CausaEfectoMap().ToTable("CausaEfecto", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_CompromisoMap().ToTable("Compromisos", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_FechasEstatusMap().ToTable("FechasEstatus", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_EstatusNominaMap().ToTable("EstatusNomina", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_BitacoraNominaMap().ToTable("BitacoraNomina", "Firm"));
            modelBuilder.Configurations.Add(new FIRM_EstatusEmailsMap().ToTable("EstatusEmails", "Firm"));
            #endregion

            #region Banco_sist
            modelBuilder.Configurations.Add(new TicketsMap().ToTable("Tickets", "Sist"));
            modelBuilder.Configurations.Add(new TicketsReclutadorMap().ToTable("TicketsReclutador", "Sist"));
            modelBuilder.Configurations.Add(new ModulosReclutamientoMap().ToTable("ModulosReclutamiento", "Recl"));
            modelBuilder.Configurations.Add(new HistoricoTicketMap().ToTable("HistoricosTickets", "Sist"));

            #endregion

            #region ServicioCliente_SCte


            #endregion

            #region Herencia_de_Entidad
            modelBuilder.Configurations.Add(new EntidadMap().ToTable("Entidades"));
            modelBuilder.Configurations.Add(new CandidatoMap());
            modelBuilder.Configurations.Add(new ContactoMap());
            modelBuilder.Configurations.Add(new ReferenciadoMap());
            modelBuilder.Configurations.Add(new ClienteMap());
            #endregion

        }
    }
}