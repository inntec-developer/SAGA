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
			//Database.SetInitializer(new MigrateDatabaseToLatestVersion<SAGADBContext, Migrations.Configuration>());
			//this.Configuration.LazyLoadingEnabled = true;
			//this.Configuration.ProxyCreationEnabled = true;
		}

		public DbSet<Usuarios> Usuarios { get; set; }
		public DbSet<Roles> Roles { get; set; }
		public DbSet<Grupos> Grupos { get; set; }
		public DbSet<Estatus> Estatus { get; set; }
		public DbSet<Prioridad> Prioridades { get; set; }

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
		#endregion

		#region PROSPECTOS/CLIENTES (Vtas)
		public DbSet<Agencia> Agencias { get; set; }
		public DbSet<Cliente> Clientes { get; set; }
		public DbSet<ConfiguracionRequi> ConfiguracionRequis { get; set; }
		public DbSet<Contacto> Contactos { get; set; }
		public DbSet<RedSocial> RedesSociales { get; set; }
		public DbSet<Referenciado> Referenciados { get; set; }
		public DbSet<TamanoEmpresa> TamanoEmpresas { get; set; }
		public DbSet<TipoBase> TiposBases { get; set; }
		#endregion

		#region DAMFO 290 (Recl)
		public DbSet<ActividadesPerfil> ActividadesPerfil { get; set; }
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
		#endregion

		#region Sist

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
		public DbSet<Persona> Personas { get; set; }
		public DbSet<Telefono> Telefonos { get; set; }
		public DbSet<TipoTelefono> TiposTelefonos { get; set; }
		public DbSet<TipoDireccion> TiposDirecciones { get; set; }
		public DbSet<TipoEstructura> TipoEstructuras { get; set; }
		public DbSet<TipoRedSocial> TiposRedesSociales { get; set; }
        public DbSet<TipoAccion> TiposAcciones { get; set; }
        public DbSet<TipoMovimiento> TiposMovimientos { get; set; }
        public DbSet<TrazabilidadMes> TrazabilidadesMes { get; set; }
        public DbSet<RastreabilidadMes> RastreabilidadMes { get; set; }
        #endregion

        #region REQUISICIONES (Vtas)
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
        public DbSet<HorariosDireccionesRequi> HorariosDoreccionesRequi { get; set; }
		#endregion

		/*
		 * Loging
		 */
		//public DbSet<AspNetUsers> AspNetUsers { get; set; }


		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			//modelBuilder.Entity<PersonasMap>().Property(p => p.Email).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute() { IsUnique = true }));

			modelBuilder.HasDefaultSchema("sist");

			//modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
			//modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
			//modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });

			#region Administracion_Sistema_Sist
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
            modelBuilder.Configurations.Add(new TrazabilidadMesMap().ToTable("TrazabilidadMesMap"));
            modelBuilder.Configurations.Add(new RastreabilidadMesMap().ToTable("RastreabilidadMes"));
			//modelBuilder.Entity<AspNetUsers>().ToTable("AspNetUsers");
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
			#endregion

			#region Reclutamiento_Recl
			modelBuilder.Configurations.Add(new ActividadesPerfilMap().ToTable("ActividadesPerfil", "Recl"));
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
			modelBuilder.Configurations.Add(new PsicometriasDamsaMap().ToTable("PsicometriasDamsa", "Recl"));
			modelBuilder.Configurations.Add(new PsicometriasClienteMap().ToTable("PsicometriasCliente", "Recl"));
			modelBuilder.Configurations.Add(new ProcesoPerfilMap().ToTable("ProcesoPerfil", "Recl"));
			modelBuilder.Configurations.Add(new RedSocialMap().ToTable("RedesSociales", "Recl"));
			modelBuilder.Configurations.Add(new RutasPerfilMap().ToTable("RutasPerfil", "Recl"));

			//modelBuilder.Configurations.Add(new VacantesMap().ToTable("Vacantes", "Recl"));


			#endregion

			#region Ventas_Vtas
			modelBuilder.Configurations.Add(new ConfiguracionRequiMap().ToTable("ConfiguracionRequi", "Vtas"));
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
			#endregion

			#region ServicioCliente_SCte


			#endregion

			#region Herencia_de_persona
			modelBuilder.Configurations.Add(new PersonaMap());
			modelBuilder.Configurations.Add(new CandidatoMap());
			modelBuilder.Configurations.Add(new ContactoMap());
			modelBuilder.Configurations.Add(new ReferenciadoMap());
			modelBuilder.Configurations.Add(new ClienteMap());
			#endregion

		}

		#region "Mapeo Sist"
		public class AreaMap : EntityTypeConfiguration<Area>
		{
			public AreaMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Nombre).HasMaxLength(50).IsRequired();
			}
		}
		public class PersonaMap : EntityTypeConfiguration<Persona>
		{
			public PersonaMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Nombre).HasMaxLength(50);
				Property(x => x.ApellidoMaterno).HasMaxLength(50);
				Property(x => x.ApellidoPaterno).HasMaxLength(50);
				Property(x => x.FechaNacimiento).HasColumnType("date").IsOptional();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
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
				Property(x => x.UsuarioAlta).HasMaxLength(30).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();

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
				Property(x => x.diaSemana).HasMaxLength(15).IsRequired();
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
				Property(x => x.NumeroInterior).HasMaxLength(10);
				Property(x => x.PaisId).IsRequired();
				Property(x => x.EstadoId);
				Property(x => x.MunicipioId);
				Property(x => x.ColoniaId);
				Property(x => x.CodigoPostal).HasMaxLength(15).IsRequired();
			}
		}
		public class EmailMap : EntityTypeConfiguration<Email>
		{
			public EmailMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.email).HasMaxLength(100).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class EstadoCivilMap : EntityTypeConfiguration<EstadoCivil>
		{
			public EstadoCivilMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.estadoCivil).HasMaxLength(20).IsRequired();
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
			}
		}
		public class EstatusMap : EntityTypeConfiguration<Estatus>
		{
			public EstatusMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Descripcion).HasMaxLength(50).IsRequired();
				Property(x => x.Activo).IsOptional();
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
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Grupo).HasMaxLength(100).IsRequired().IsUnicode();
				Property(x => x.Activo).IsOptional();
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
			}
		}
		public class MunicipioMap : EntityTypeConfiguration<Municipio>
		{
			public MunicipioMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.municipio).HasMaxLength(100).IsRequired();
				Property(x => x.EstadoId).IsRequired();
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
				Property(x => x.Rol).HasMaxLength(20).IsRequired().IsUnicode();
				Property(x => x.Create).IsOptional();
				Property(x => x.Read).IsOptional();
				Property(x => x.Update).IsOptional();
				Property(x => x.Delete).IsOptional();
				Property(x => x.Activo).IsOptional();
			}
		}
		public class TamanoEmpresaMap : EntityTypeConfiguration<TamanoEmpresa>
		{
			public TamanoEmpresaMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.tamanoEmpresa).HasMaxLength(30).IsRequired();
			}
		}
		public class TelefonoMap : EntityTypeConfiguration<Telefono>
		{
			public TelefonoMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.ClavePais).HasMaxLength(5).IsRequired();
				Property(x => x.ClaveLada).HasMaxLength(5);
				Property(x => x.telefono).HasMaxLength(15).IsRequired();
				Property(x => x.Extension).HasMaxLength(10);
				Property(x => x.esPrincipal).IsRequired();
				Property(x => x.TipoTelefonoId).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
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
			}
		}
		public class TipoBaseMap : EntityTypeConfiguration<TipoBase>
		{
			public TipoBaseMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.tipoBase).HasMaxLength(50).IsRequired();
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
				Property(x => x.Usuario).HasMaxLength(20).IsRequired().IsUnicode();
				Property(x => x.Password).HasMaxLength(40).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();

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
				Property(e => e.Clave).HasMaxLength(5).IsRequired();
				Property(e => e.Menu).IsRequired();
				Property(e => e.Confidencial).IsRequired();
				Property(e => e.Inclusivo).IsRequired();
				Property(e => e.DescripcionInclusivo).HasMaxLength(150);
				Property(e => e.Activo).IsRequired();
				Property(e => e.Icono).HasMaxLength(100);
				Property(e => e.Accion).HasMaxLength(500);
				Property(e => e.PermisoC).HasMaxLength(1000);
				Property(e => e.PermisoR).HasMaxLength(1000);
				Property(e => e.PermisoU).HasMaxLength(1000);
				Property(e => e.PermisoD).HasMaxLength(1000);

			}
		}
        public class TipoMovimientoMap : EntityTypeConfiguration<TipoMovimiento>
        {
            public TipoMovimientoMap()
            {
                HasKey(x => x.Id);
                Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Clave).HasMaxLength(6).IsRequired().IsUnicode(true);
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
                Property(x => x.Clave).HasMaxLength(2).IsRequired().IsUnicode(true);
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
                Property(x => x.fch_Creacion).HasColumnType("DateTime").IsRequired();
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
                Property(x => x.fch_Modificacion).HasColumnType("DateTime").IsRequired();
                Property(x => x.UsuarioMod).HasMaxLength(50).IsRequired();
                Property(x => x.TipoAccionId).IsRequired();
                Property(x => x.Descripcion).HasMaxLength(50).IsRequired();
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
			}
		}
		public class AreaInteresMap : EntityTypeConfiguration<AreaInteres>
		{
			public AreaInteresMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.areaInteres).HasMaxLength(200);
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
				Property(x => x.CarreraId).IsRequired();
				Property(x => x.YearInicioId).IsOptional();
				Property(x => x.MonthInicioId).IsOptional();
				Property(x => x.YearTerminoId).IsOptional();
				Property(x => x.MonthTerminoId).IsOptional();

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
			}
		}
		public class PerfilCandidatoMap : EntityTypeConfiguration<PerfilCandidato>
		{
			public PerfilCandidatoMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.CandidatoId).IsRequired();
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
		#endregion

		#region "Mapeo Vtas"
		#region Prospectos / Clientes
		public class ActividadEmpMap : EntityTypeConfiguration<ActividadEmpresa>
		{
			public ActividadEmpMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.actividadEmpresa).HasMaxLength(50).IsRequired();
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
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
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
				Property(x => x.Puesto).HasMaxLength(100).IsRequired(); Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class ClaseReclutamientoMap : EntityTypeConfiguration<ClaseReclutamiento>
		{
			public ClaseReclutamientoMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.clasesReclutamiento).HasMaxLength(50).IsRequired();
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
				Property(x => x.fch_Creacion).HasColumnType("DateTime").IsOptional();
				Property(x => x.fch_Aprobacion).HasColumnType("DateTime").IsOptional();
				Property(x => x.fch_Cumplimiento).HasColumnType("DateTime").IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("DateTime").IsOptional();
				Property(x => x.Propietario).HasMaxLength(15).IsOptional();
				Property(x => x.Aprobador).HasMaxLength(15).IsOptional();
				Property(x => x.UsuarioMod).HasMaxLength(15).IsOptional();
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
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class AptitudesRequiMap : EntityTypeConfiguration<AptitudesRequi>
		{
			public AptitudesRequiMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.AptitudId).IsRequired();
				Property(x => x.RequisicionId).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
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
				Property(x => x.deHora).HasMaxLength(25).IsRequired();
				Property(x => x.aHora).HasMaxLength(25).IsRequired();
				Property(x => x.numeroVacantes).HasColumnType("tinyint").IsRequired();
				Property(x => x.Especificaciones).HasMaxLength(500).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class ActividadesRequilMap : EntityTypeConfiguration<ActividadesRequi>
		{
			public ActividadesRequilMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Actividades).HasMaxLength(200).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class ObservacionesRequiMap : EntityTypeConfiguration<ObservacionesRequi>
		{
			public ObservacionesRequiMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Observaciones).HasMaxLength(100).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class PsicometriasDamsaRequiMap : EntityTypeConfiguration<PsicometriasDamsaRequi>
		{
			public PsicometriasDamsaRequiMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.PsicometriaId).IsRequired();
				Property(x => x.RequisicionId).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
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
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
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
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}

		}
		public class DocumentosClienteRequiMap : EntityTypeConfiguration<DocumentosClienteRequi>
		{
			public DocumentosClienteRequiMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Documento).HasMaxLength(100).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class ProcesoRequiMap : EntityTypeConfiguration<ProcesoRequi>
		{
			public ProcesoRequiMap()
			{

				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Proceso).HasMaxLength(100).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class PrestacionesClienteRequiMap : EntityTypeConfiguration<PrestacionesClienteRequi>
		{
			public PrestacionesClienteRequiMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Prestamo).HasMaxLength(100).IsRequired();
				Property(x => x.RequisicionId).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class CompetenciasAreasRequiMap : EntityTypeConfiguration<CompetenciaAreaRequi>
		{
			public CompetenciasAreasRequiMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.CompetenciaId).IsRequired();
				Property(x => x.Nivel).HasMaxLength(10).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class CompetenciaCardinalRequilMap : EntityTypeConfiguration<CompetenciaCardinalRequi>
		{
			public CompetenciaCardinalRequilMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.CompetenciaId).IsRequired();
				Property(x => x.Nivel).HasMaxLength(10).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}

		}
		public class CompetenciaGerencialRequiMap : EntityTypeConfiguration<CompetenciaGerencialRequi>
		{
			public CompetenciaGerencialRequiMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.CompetenciaId).IsRequired();
				Property(x => x.Nivel).HasMaxLength(10).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class AsignacionRequiMap : EntityTypeConfiguration<AsignacionRequi>
		{
			public AsignacionRequiMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.RequisicionId).IsRequired();
				Property(x => x.GrpUsrId).IsRequired();
				Property(x => x.CRUD).HasMaxLength(5).IsFixedLength();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class ConfiguracionRequiMap : EntityTypeConfiguration<ConfiguracionRequi>
		{
			public ConfiguracionRequiMap()
			{
				HasKey(x => x.id);
				Property(e => e.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(e => e.IdRequi).IsRequired();
				Property(e => e.IdEstructura).IsRequired();
				Property(e => e.Campo).HasMaxLength(500).IsRequired();
				Property(e => e.R_D).IsRequired();
				Property(e => e.Resumen).IsRequired();
				Property(e => e.Detalle).IsRequired();
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
        #endregion
        #endregion

        #region "Mapeo Recl(DAMFO)"
        public class ActividadesPerfilMap : EntityTypeConfiguration<ActividadesPerfil>
		{
			public ActividadesPerfilMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Actividades).HasMaxLength(200).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class AptitudesPerfilMap : EntityTypeConfiguration<AptitudesPerfil>
		{
			public AptitudesPerfilMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.AptitudId).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class AptitudMap : EntityTypeConfiguration<Aptitud>
		{
			public AptitudMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.aptitud).HasMaxLength(50).IsRequired();
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
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}

		}
		public class CompetenciaAreaPerfilMap : EntityTypeConfiguration<CompetenciaAreaPerfil>
		{
			public CompetenciaAreaPerfilMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.CompetenciaId).IsRequired();
				Property(x => x.Nivel).HasMaxLength(10).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class CompetenciaCardinalPerfilMap : EntityTypeConfiguration<CompetenciaCardinalPerfil>
		{
			public CompetenciaCardinalPerfilMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.CompetenciaId).IsRequired();
				Property(x => x.Nivel).HasMaxLength(10).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}

		}
		public class CompetenciaGerencialPerfilMap : EntityTypeConfiguration<CompetenciaGerencialPerfil>
		{
			public CompetenciaGerencialPerfilMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.CompetenciaId).IsRequired();
				Property(x => x.Nivel).HasMaxLength(10).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
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
				Property(x => x.fch_Creacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsRequired();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").IsOptional();
				Property(x => x.FlexibilidadHorario).IsRequired();
				Property(x => x.JornadaLaboralId).IsOptional();
				Property(x => x.TipoModalidadId).IsOptional();
				Property(x => x.UsuarioAlta).HasMaxLength(15).IsOptional();
				Property(x => x.UsuarioMod).HasMaxLength(15).IsOptional();
			}
		}
		public class DocumentosDamsaMap : EntityTypeConfiguration<DocumentosDamsa>
		{
			public DocumentosDamsaMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.documentoDamsa).HasMaxLength(100).IsRequired();
			}
		}
		public class DocumentosClienteMap : EntityTypeConfiguration<DocumentosCliente>
		{
			public DocumentosClienteMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Documento).HasMaxLength(100).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
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
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}

		}
		public class RedSocialMap : EntityTypeConfiguration<RedSocial>
		{
			public RedSocialMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.redSocial).HasMaxLength(100).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
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
				Property(x => x.deHora).HasMaxLength(25).IsRequired();
				Property(x => x.aHora).HasMaxLength(25).IsRequired();
				Property(x => x.numeroVacantes).HasColumnType("tinyint").IsRequired();
				Property(x => x.Especificaciones).HasMaxLength(500);
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class ObservacionesPerfilMap : EntityTypeConfiguration<ObservacionesPerfil>
		{
			public ObservacionesPerfilMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Observaciones).HasMaxLength(100).IsRequired();
				Property(x => x.DAMFO290Id).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class PeriodoPagoMap : EntityTypeConfiguration<PeriodoPago>
		{
			public PeriodoPagoMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.periodoPago).HasMaxLength(50).IsRequired();
			}
		}
		public class PrestacionesClientePerfilMap : EntityTypeConfiguration<PrestacionesClientePerfil>
		{
			public PrestacionesClientePerfilMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Prestamo).HasMaxLength(100).IsRequired();
				Property(x => x.DAMFO290Id).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class PrestacionesLeyMap : EntityTypeConfiguration<PrestacionLey>
		{
			public PrestacionesLeyMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.prestacionLey).HasMaxLength(50).IsRequired();
			}
		}
		public class ProcesoPerfilMap : EntityTypeConfiguration<ProcesoPerfil>
		{
			public ProcesoPerfilMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.Proceso).HasMaxLength(100).IsRequired();
				Property(x => x.Orden).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();

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
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class PsicometriasDamsaMap : EntityTypeConfiguration<PsicometriasDamsa>
		{
			public PsicometriasDamsaMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.PsicometriaId).IsRequired();
				Property(x => x.DAMFO290Id).IsRequired();
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
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
				Property(x => x.UsuarioAlta).IsOptional();
				Property(x => x.UsuarioMod).IsOptional();
				Property(x => x.fch_Creacion).IsOptional().HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
				Property(x => x.fch_Modificacion).HasColumnType("datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed).IsOptional();
			}
		}
		public class TipoReclutamientoMap : EntityTypeConfiguration<TipoReclutamiento>
		{
			public TipoReclutamientoMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.tipoReclutamiento).HasMaxLength(20).IsRequired();
			}
		}
		public class TipoPsicometriaMap : EntityTypeConfiguration<TipoPsicometria>
		{
			public TipoPsicometriaMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.tipoPsicometria).HasMaxLength(50).IsRequired();
				Property(x => x.descripcion).HasMaxLength(50).IsRequired();
			}
		}
		public class TipoBeneficioMap : EntityTypeConfiguration<TipoBeneficio>
		{
			public TipoBeneficioMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.tipoBeneficio).HasMaxLength(50).IsRequired();
			}
		}
		public class TipodeNominaMap : EntityTypeConfiguration<TipodeNomina>
		{
			public TipodeNominaMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.tipoDeNomina).HasMaxLength(50).IsRequired();
			}
		}
		public class TipoContratoMap : EntityTypeConfiguration<TipoContrato>
		{
			public TipoContratoMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.tipoContrato).HasMaxLength(50).IsRequired();
			}
		}
		public class ProcesoCandidatoMap : EntityTypeConfiguration<ProcesoCandidato>
		{
			public ProcesoCandidatoMap()
			{
				HasKey(x => x.Id); Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
				Property(x => x.CandidatoId).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new[] { new IndexAttribute("Index") { IsUnique = true } }));
				Property(x => x.RequisicionId).IsRequired();
				Property(x => x.Reclutador).IsRequired();
				Property(x => x.Estatus).IsRequired();
				Property(x => x.TpContrato).IsOptional();
				Property(x => x.Fch_Creacion).IsRequired().HasColumnType("Datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
				Property(x => x.Fch_Modificacion).IsOptional().HasColumnType("Datetime").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
			}
		}

		#endregion
	}
}