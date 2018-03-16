namespace SAGA.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "BTra.AcercaDeMi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        AcercaDeMi = c.String(maxLength: 400),
                        PuestoDeseado = c.String(maxLength: 40),
                        SalarioAceptable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalarioDeseado = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AreaExperienciaId = c.Int(nullable: false),
                        AreaInteresId = c.Int(),
                        PerfilExperienciaId = c.Int(),
                        SitioWeb = c.String(),
                        PerfilCandidatoId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BTra.AreasExperiencia", t => t.AreaExperienciaId, cascadeDelete: false)
                .ForeignKey("BTra.AreasInteres", t => t.AreaInteresId)
                .ForeignKey("BTra.PerfilCandidato", t => t.PerfilCandidatoId, cascadeDelete: true)
                .ForeignKey("BTra.PerfilExperiencia", t => t.PerfilExperienciaId)
                .Index(t => t.AreaExperienciaId)
                .Index(t => t.AreaInteresId)
                .Index(t => t.PerfilExperienciaId)
                .Index(t => t.PerfilCandidatoId);
            
            CreateTable(
                "BTra.AreasExperiencia",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        areaExperiencia = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BTra.AreasInteres",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        areaInteres = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BTra.PerfilCandidato",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CandidatoId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BTra.Candidatos", t => t.CandidatoId)
                .Index(t => t.CandidatoId);
            
            CreateTable(
                "sist.Personas",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Nombre = c.String(maxLength: 50),
                        ApellidoPaterno = c.String(maxLength: 50),
                        ApellidoMaterno = c.String(maxLength: 50),
                        FechaNacimiento = c.DateTime(storeType: "date"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "sist.Direcciones",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        TipoDireccionId = c.Int(nullable: false),
                        esMoral = c.Boolean(nullable: false),
                        Calle = c.String(maxLength: 100),
                        NumeroInterior = c.String(maxLength: 10),
                        NumeroExterior = c.String(maxLength: 10),
                        PaisId = c.Int(nullable: false),
                        EstadoId = c.Int(),
                        MunicipioId = c.Int(),
                        ColoniaId = c.Int(),
                        CodigoPostal = c.String(nullable: false, maxLength: 15),
                        esPrincipal = c.Boolean(nullable: false),
                        Activo = c.Boolean(nullable: false),
                        Referencia = c.String(maxLength: 500),
                        PersonaId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Colonias", t => t.ColoniaId)
                .ForeignKey("sist.Estados", t => t.EstadoId)
                .ForeignKey("sist.Municipios", t => t.MunicipioId)
                .ForeignKey("sist.Paises", t => t.PaisId, cascadeDelete: true)
                .ForeignKey("sist.Personas", t => t.PersonaId, cascadeDelete: true)
                .ForeignKey("sist.TiposDirecciones", t => t.TipoDireccionId, cascadeDelete: true)
                .Index(t => t.TipoDireccionId)
                .Index(t => t.PaisId)
                .Index(t => t.EstadoId)
                .Index(t => t.MunicipioId)
                .Index(t => t.ColoniaId)
                .Index(t => t.PersonaId);
            
            CreateTable(
                "sist.Colonias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        colonia = c.String(nullable: false, maxLength: 100),
                        CP = c.String(nullable: false, maxLength: 13),
                        TipoColonia = c.String(maxLength: 50),
                        MunicipioId = c.Int(nullable: false),
                        EstadoId = c.Int(nullable: false),
                        PaisId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Estados", t => t.EstadoId, cascadeDelete: true)
                .ForeignKey("sist.Municipios", t => t.MunicipioId, cascadeDelete: true)
                .ForeignKey("sist.Paises", t => t.PaisId, cascadeDelete: true)
                .Index(t => t.MunicipioId)
                .Index(t => t.EstadoId)
                .Index(t => t.PaisId);
            
            CreateTable(
                "sist.Estados",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        estado = c.String(nullable: false, maxLength: 100),
                        PaisId = c.Int(nullable: false),
                        Clave = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Paises", t => t.PaisId, cascadeDelete: false)
                .Index(t => t.PaisId);
            
            CreateTable(
                "sist.Paises",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        pais = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "sist.Municipios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        municipio = c.String(nullable: false, maxLength: 100),
                        EstadoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Estados", t => t.EstadoId, cascadeDelete: false)
                .Index(t => t.EstadoId);
            
            CreateTable(
                "sist.Emails",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        email = c.String(nullable: false, maxLength: 100),
                        esPrincipal = c.Boolean(nullable: false),
                        PersonaId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Personas", t => t.PersonaId, cascadeDelete: true)
                .Index(t => t.PersonaId);
            
            CreateTable(
                "sist.Telefonos",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ClavePais = c.String(nullable: false, maxLength: 5),
                        ClaveLada = c.String(maxLength: 5),
                        Extension = c.String(maxLength: 10),
                        telefono = c.String(nullable: false, maxLength: 15),
                        TipoTelefonoId = c.Byte(nullable: false),
                        Activo = c.Boolean(nullable: false),
                        esPrincipal = c.Boolean(nullable: false),
                        PersonaId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Personas", t => t.PersonaId, cascadeDelete: true)
                .ForeignKey("sist.TiposTelefonos", t => t.TipoTelefonoId, cascadeDelete: false)
                .Index(t => t.TipoTelefonoId)
                .Index(t => t.PersonaId);
            
            CreateTable(
                "sist.TiposTelefonos",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Tipo = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Vtas.ActividadEmpresas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GiroEmpresaId = c.Int(nullable: false),
                        actividadEmpresa = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.GiroEmpresas", t => t.GiroEmpresaId, cascadeDelete: true)
                .Index(t => t.GiroEmpresaId);
            
            CreateTable(
                "sist.GiroEmpresas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        giroEmpresa = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Vtas.Agencias",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ClienteId = c.Guid(nullable: false),
                        agencia = c.String(nullable: false, maxLength: 50),
                        DesdeCuendo = c.DateTime(nullable: false, storeType: "date"),
                        Empleado = c.Decimal(nullable: false, precision: 5, scale: 2),
                        Cobro = c.Decimal(nullable: false, precision: 5, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Vtas.Clientes", t => t.ClienteId)
                .Index(t => t.ClienteId);
            
            CreateTable(
                "Vtas.TamanosEmpresas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        tamanoEmpresa = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Vtas.TiposBases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        tipoBase = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Vtas.TiposEmpresas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        tipoEmpresa = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "sist.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rol = c.String(nullable: false, maxLength: 20),
                        ModuloId = c.Int(nullable: false),
                        Create = c.Boolean(),
                        Read = c.Boolean(),
                        Update = c.Boolean(),
                        Delete = c.Boolean(),
                        Activo = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Modulos", t => t.ModuloId, cascadeDelete: false)
                .Index(t => t.ModuloId);
            
            CreateTable(
                "sist.Modulos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 50),
                        Accion = c.String(),
                        Icono = c.String(),
                        Orden = c.Int(),
                        IdPadre = c.Int(),
                        Activo = c.Boolean(),
                        Ambito = c.String(maxLength: 30),
                        Clave = c.String(nullable: false, maxLength: 5, fixedLength: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "sist.TiposUsuarios",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Tipo = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "sist.TiposDirecciones",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        tipoDireccion = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "sist.EstadosCiviles",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        estadoCivil = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "sist.Generos",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        genero = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BTra.TiposDiscapacidades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        tipoDiscapacidad = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BTra.TiposLicencias",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        tipoLicencia = c.String(maxLength: 1),
                        Descripcion = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BTra.Certificaciones",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        certificacion = c.String(maxLength: 200),
                        AutoridadEmisora = c.String(maxLength: 200),
                        Licencia = c.String(maxLength: 100),
                        UrlCertificacion = c.String(maxLength: 500),
                        noVence = c.Boolean(),
                        YearInicioId = c.Int(),
                        MonthInicioId = c.Int(),
                        YearTerminoId = c.Int(),
                        MonthTerminoId = c.Int(),
                        PerfilCandidatoId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Months", t => t.MonthInicioId)
                .ForeignKey("sist.Months", t => t.MonthTerminoId)
                .ForeignKey("BTra.PerfilCandidato", t => t.PerfilCandidatoId, cascadeDelete: true)
                .ForeignKey("sist.Years", t => t.YearInicioId)
                .ForeignKey("sist.Years", t => t.YearTerminoId)
                .Index(t => t.YearInicioId)
                .Index(t => t.MonthInicioId)
                .Index(t => t.YearTerminoId)
                .Index(t => t.MonthTerminoId)
                .Index(t => t.PerfilCandidatoId);
            
            CreateTable(
                "sist.Months",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        month = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "sist.Years",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        year = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BTra.ConocimientosHabilidades",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Conocimiento = c.String(maxLength: 50),
                        Herramienta = c.String(),
                        InstitucionEducativaId = c.Guid(),
                        NivelId = c.Byte(),
                        PerfilCandidatoId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BTra.InstitucionesEducativas", t => t.InstitucionEducativaId)
                .ForeignKey("sist.Niveles", t => t.NivelId)
                .ForeignKey("BTra.PerfilCandidato", t => t.PerfilCandidatoId, cascadeDelete: true)
                .Index(t => t.InstitucionEducativaId)
                .Index(t => t.NivelId)
                .Index(t => t.PerfilCandidatoId);
            
            CreateTable(
                "BTra.InstitucionesEducativas",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        institucionEducativa = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "sist.Niveles",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        nivel = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BTra.Cursos",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        curso = c.String(maxLength: 200),
                        InstitucionEducativaId = c.Guid(nullable: false),
                        YearInicioId = c.Int(),
                        MonthInicioId = c.Int(),
                        YearTerminoId = c.Int(),
                        MonthTerminoId = c.Int(),
                        Horas = c.Int(),
                        PerfilCandidatoId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BTra.InstitucionesEducativas", t => t.InstitucionEducativaId, cascadeDelete: false)
                .ForeignKey("sist.Months", t => t.MonthInicioId)
                .ForeignKey("sist.Months", t => t.MonthTerminoId)
                .ForeignKey("BTra.PerfilCandidato", t => t.PerfilCandidatoId, cascadeDelete: true)
                .ForeignKey("sist.Years", t => t.YearInicioId)
                .ForeignKey("sist.Years", t => t.YearTerminoId)
                .Index(t => t.InstitucionEducativaId)
                .Index(t => t.YearInicioId)
                .Index(t => t.MonthInicioId)
                .Index(t => t.YearTerminoId)
                .Index(t => t.MonthTerminoId)
                .Index(t => t.PerfilCandidatoId);
            
            CreateTable(
                "BTra.ExperienciasProfesionales",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Empresa = c.String(maxLength: 100),
                        GiroEmpresaId = c.Int(nullable: false),
                        CargoAsignado = c.String(maxLength: 100),
                        AreaId = c.Int(nullable: false),
                        YearInicioId = c.Int(nullable: false),
                        MonthInicioId = c.Int(nullable: false),
                        YearTerminoId = c.Int(nullable: false),
                        MonthTerminoId = c.Int(nullable: false),
                        Salario = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TrabajoActual = c.Boolean(),
                        Descripcion = c.String(maxLength: 200),
                        PerfilCandidatoId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Areas", t => t.AreaId, cascadeDelete: false)
                .ForeignKey("sist.GiroEmpresas", t => t.GiroEmpresaId, cascadeDelete: false)
                .ForeignKey("sist.Months", t => t.MonthInicioId, cascadeDelete: false)
                .ForeignKey("sist.Months", t => t.MonthTerminoId, cascadeDelete: false)
                .ForeignKey("BTra.PerfilCandidato", t => t.PerfilCandidatoId, cascadeDelete: true)
                .ForeignKey("sist.Years", t => t.YearInicioId, cascadeDelete: false)
                .ForeignKey("sist.Years", t => t.YearTerminoId, cascadeDelete: false)
                .Index(t => t.GiroEmpresaId)
                .Index(t => t.AreaId)
                .Index(t => t.YearInicioId)
                .Index(t => t.MonthInicioId)
                .Index(t => t.YearTerminoId)
                .Index(t => t.MonthTerminoId)
                .Index(t => t.PerfilCandidatoId);
            
            CreateTable(
                "sist.Areas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BTra.Formaciones",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        InstitucionEducativaId = c.Guid(nullable: false),
                        GradoEstudioId = c.Int(nullable: false),
                        EstadoEstudioId = c.Int(nullable: false),
                        DocumentoValidadorId = c.Int(),
                        CarreraId = c.Guid(nullable: false),
                        YearInicioId = c.Int(),
                        MonthInicioId = c.Int(),
                        YearTerminoId = c.Int(),
                        MonthTerminoId = c.Int(),
                        PerfilCandidatoId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BTra.Carreras", t => t.CarreraId, cascadeDelete: false)
                .ForeignKey("BTra.DocumentosValidadores", t => t.DocumentoValidadorId)
                .ForeignKey("Recl.EstadosEstudios", t => t.EstadoEstudioId, cascadeDelete: false)
                .ForeignKey("sist.GradosEstudios", t => t.GradoEstudioId, cascadeDelete: false)
                .ForeignKey("BTra.InstitucionesEducativas", t => t.InstitucionEducativaId, cascadeDelete: false)
                .ForeignKey("sist.Months", t => t.MonthInicioId)
                .ForeignKey("sist.Months", t => t.MonthTerminoId)
                .ForeignKey("BTra.PerfilCandidato", t => t.PerfilCandidatoId, cascadeDelete: true)
                .ForeignKey("sist.Years", t => t.YearInicioId)
                .ForeignKey("sist.Years", t => t.YearTerminoId)
                .Index(t => t.InstitucionEducativaId)
                .Index(t => t.GradoEstudioId)
                .Index(t => t.EstadoEstudioId)
                .Index(t => t.DocumentoValidadorId)
                .Index(t => t.CarreraId)
                .Index(t => t.YearInicioId)
                .Index(t => t.MonthInicioId)
                .Index(t => t.YearTerminoId)
                .Index(t => t.MonthTerminoId)
                .Index(t => t.PerfilCandidatoId);
            
            CreateTable(
                "BTra.Carreras",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        carrera = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BTra.DocumentosValidadores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        documentoValidador = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.EstadosEstudios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        estadoEstudio = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "sist.GradosEstudios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        gradoEstudio = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "sist.PerfilIdiomas",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IdiomaId = c.Int(nullable: false),
                        NivelEscritoId = c.Byte(nullable: false),
                        NivelHabladoId = c.Byte(nullable: false),
                        PerfilCandidatoId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Idiomas", t => t.IdiomaId, cascadeDelete: false)
                .ForeignKey("sist.Niveles", t => t.NivelEscritoId, cascadeDelete: false)
                .ForeignKey("sist.Niveles", t => t.NivelHabladoId, cascadeDelete: false)
                .ForeignKey("BTra.PerfilCandidato", t => t.PerfilCandidatoId, cascadeDelete: true)
                .Index(t => t.IdiomaId)
                .Index(t => t.NivelEscritoId)
                .Index(t => t.NivelHabladoId)
                .Index(t => t.PerfilCandidatoId);
            
            CreateTable(
                "sist.Idiomas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        idioma = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BTra.PerfilExperiencia",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        perfilExperiencia = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.ActividadesPerfil",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Actividades = c.String(nullable: false, maxLength: 200),
                        DAMFO290Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.DAMFO_290",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ClienteId = c.Guid(nullable: false),
                        TipoReclutamientoId = c.Int(nullable: false),
                        ClaseReclutamientoId = c.Int(nullable: false),
                        NombrePerfil = c.String(maxLength: 100),
                        GeneroId = c.Byte(nullable: false),
                        EdadMinima = c.Int(nullable: false),
                        EdadMaxima = c.Int(nullable: false),
                        EstadoCivilId = c.Byte(nullable: false),
                        AreaId = c.Int(nullable: false),
                        Experiencia = c.String(nullable: false, maxLength: 500),
                        SueldoMinimo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SueldoMaximo = c.Decimal(nullable: false, precision: 18, scale: 3),
                        DiaCorteId = c.Byte(nullable: false),
                        TipoNominaId = c.Int(nullable: false),
                        DiaPagoId = c.Byte(nullable: false),
                        PeriodoPagoId = c.Int(nullable: false),
                        Especifique = c.String(),
                        ContratoInicialId = c.Int(nullable: false),
                        ContratoFinalId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Areas", t => t.AreaId, cascadeDelete: false)
                .ForeignKey("Recl.ClasesReclutamientos", t => t.ClaseReclutamientoId, cascadeDelete: false)
                .ForeignKey("Vtas.Clientes", t => t.ClienteId)
                .ForeignKey("Recl.TiposContratos", t => t.ContratoFinalId, cascadeDelete: false)
                .ForeignKey("Recl.TiposContratos", t => t.ContratoInicialId, cascadeDelete: false)
                .ForeignKey("Recl.DiasSemana", t => t.DiaCorteId, cascadeDelete: false)
                .ForeignKey("Recl.DiasSemana", t => t.DiaPagoId, cascadeDelete: false)
                .ForeignKey("sist.EstadosCiviles", t => t.EstadoCivilId, cascadeDelete: false)
                .ForeignKey("sist.Generos", t => t.GeneroId, cascadeDelete: false)
                .ForeignKey("Recl.PeriodosPagos", t => t.PeriodoPagoId, cascadeDelete: false)
                .ForeignKey("Recl.TiposNominas", t => t.TipoNominaId, cascadeDelete: false)
                .ForeignKey("Recl.TiposReclutamientos", t => t.TipoReclutamientoId, cascadeDelete: false)
                .Index(t => t.ClienteId)
                .Index(t => t.TipoReclutamientoId)
                .Index(t => t.ClaseReclutamientoId)
                .Index(t => t.GeneroId)
                .Index(t => t.EstadoCivilId)
                .Index(t => t.AreaId)
                .Index(t => t.DiaCorteId)
                .Index(t => t.TipoNominaId)
                .Index(t => t.DiaPagoId)
                .Index(t => t.PeriodoPagoId)
                .Index(t => t.ContratoInicialId)
                .Index(t => t.ContratoFinalId);
            
            CreateTable(
                "Recl.AptitudesPerfil",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        AptitudId = c.Int(nullable: false),
                        DAMFO290Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.Aptitudes", t => t.AptitudId, cascadeDelete: false)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .Index(t => t.AptitudId)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.Aptitudes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        aptitud = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.BeneficiosPerfil",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        TipoBeneficioId = c.Int(nullable: false),
                        Cantidad = c.Single(nullable: false),
                        Observaciones = c.String(maxLength: 500),
                        DAMFO290Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .ForeignKey("Recl.TiposBeneficios", t => t.TipoBeneficioId, cascadeDelete: false)
                .Index(t => t.TipoBeneficioId)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.TiposBeneficios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        tipoBeneficio = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.ClasesReclutamientos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        clasesReclutamiento = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.CompetenciaAreaPerfil",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CompetenciaId = c.Int(nullable: false),
                        Nivel = c.String(nullable: false, maxLength: 10),
                        DAMFO290Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.CompetenciasAreas", t => t.CompetenciaId, cascadeDelete: false)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .Index(t => t.CompetenciaId)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.CompetenciasAreas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        competenciaArea = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.CompetenciaCardinalPerfil",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CompetenciaId = c.Int(nullable: false),
                        Nivel = c.String(nullable: false, maxLength: 10),
                        DAMFO290Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.CompetenciasCardinales", t => t.CompetenciaId, cascadeDelete: false)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .Index(t => t.CompetenciaId)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.CompetenciasCardinales",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        competenciaCardinal = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.CompetenciaGerencialPerfil",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CompetenciaId = c.Int(nullable: false),
                        Nivel = c.String(nullable: false, maxLength: 10),
                        DAMFO290Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.CompetenciasGerenciales", t => t.CompetenciaId, cascadeDelete: false)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .Index(t => t.CompetenciaId)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.CompetenciasGerenciales",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        competenciaGerencial = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.TiposContratos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        tipoContrato = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.DiasSemana",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        diaSemana = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.DocumentosClientes",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Documento = c.String(nullable: false, maxLength: 100),
                        DAMFO290Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.EscolaridadesPerfil",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        EscolaridadId = c.Int(nullable: false),
                        EstadoEstudioId = c.Int(nullable: false),
                        DAMFO290Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .ForeignKey("sist.GradosEstudios", t => t.EscolaridadId, cascadeDelete: false)
                .ForeignKey("Recl.EstadosEstudios", t => t.EstadoEstudioId, cascadeDelete: false)
                .Index(t => t.EscolaridadId)
                .Index(t => t.EstadoEstudioId)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.HorariosPerfiles",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 100),
                        deDiaId = c.Byte(nullable: false),
                        aDiaId = c.Byte(nullable: false),
                        deHora = c.String(nullable: false, maxLength: 25),
                        aHora = c.String(nullable: false, maxLength: 25),
                        numeroVacantes = c.Byte(nullable: false),
                        Especificaciones = c.String(maxLength: 500),
                        DAMFO290Id = c.Guid(nullable: false),
                        Activo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.DiasSemana", t => t.aDiaId, cascadeDelete: false)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .ForeignKey("Recl.DiasSemana", t => t.deDiaId, cascadeDelete: false)
                .Index(t => t.deDiaId)
                .Index(t => t.aDiaId)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.ObservacionesPerfil",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Observaciones = c.String(nullable: false, maxLength: 100),
                        DAMFO290Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.PeriodosPagos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        periodoPago = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.PrestacionesClientePerfil",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Prestamo = c.String(nullable: false, maxLength: 100),
                        DAMFO290Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.ProcesoPerfil",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Proceso = c.String(nullable: false, maxLength: 100),
                        DAMFO290Id = c.Guid(nullable: false),
                        Orden = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.PsicometriasCliente",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Psicometria = c.String(maxLength: 50),
                        Descripcion = c.String(maxLength: 200),
                        DAMFO290Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.PsicometriasDamsa",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        PsicometriaId = c.Int(nullable: false),
                        DAMFO290Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: true)
                .ForeignKey("Recl.TiposPsicometrias", t => t.PsicometriaId, cascadeDelete: false)
                .Index(t => t.PsicometriaId)
                .Index(t => t.DAMFO290Id);
            
            CreateTable(
                "Recl.TiposPsicometrias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        tipoPsicometria = c.String(nullable: false, maxLength: 50),
                        descripcion = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.TiposNominas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        tipoDeNomina = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.TiposReclutamientos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        tipoReclutamiento = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Vtas.ActividadesRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Actividades = c.String(nullable: false, maxLength: 200),
                        RequisicionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .Index(t => t.RequisicionId);
            
            CreateTable(
                "Vtas.Requisiciones",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ClienteId = c.Guid(nullable: false),
                        TipoReclutamientoId = c.Int(nullable: false),
                        ClaseReclutamientoId = c.Int(nullable: false),
                        VBtra = c.String(maxLength: 100),
                        GeneroId = c.Byte(nullable: false),
                        EdadMinima = c.Int(nullable: false),
                        EdadMaxima = c.Int(nullable: false),
                        EstadoCivilId = c.Byte(nullable: false),
                        AreaId = c.Int(nullable: false),
                        Experiencia = c.String(nullable: false, maxLength: 500),
                        SueldoMinimo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SueldoMaximo = c.Decimal(nullable: false, precision: 18, scale: 3),
                        DiaCorteId = c.Byte(nullable: false),
                        TipoNominaId = c.Int(nullable: false),
                        DiaPagoId = c.Byte(nullable: false),
                        PeriodoPagoId = c.Int(nullable: false),
                        Especifique = c.String(),
                        ContratoInicialId = c.Int(nullable: false),
                        ContratoFinalId = c.Int(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaAprobacion = c.DateTime(nullable: false),
                        FechaCumplimiento = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        PropietarioId = c.Guid(nullable: false),
                        AprobadorId = c.Guid(nullable: false),
                        UsuarioMod = c.Guid(nullable: false),
                        PrioridadId = c.Int(nullable: false),
                        Aprobada = c.Boolean(),
                        Confidencial = c.Boolean(),
                        Asignada = c.Boolean(),
                        EstatusId = c.Int(nullable: false),
                        DAMFO290Id = c.Guid(nullable: false),
                        DireccionId = c.Guid(nullable: false),
                        HorarioId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Usuarios", t => t.AprobadorId)
                .ForeignKey("sist.Areas", t => t.AreaId, cascadeDelete: false)
                .ForeignKey("Recl.ClasesReclutamientos", t => t.ClaseReclutamientoId, cascadeDelete: false)
                .ForeignKey("Vtas.Clientes", t => t.ClienteId)
                .ForeignKey("Recl.TiposContratos", t => t.ContratoFinalId, cascadeDelete: false)
                .ForeignKey("Recl.TiposContratos", t => t.ContratoInicialId, cascadeDelete: false)
                .ForeignKey("Recl.DAMFO_290", t => t.DAMFO290Id, cascadeDelete: false)
                .ForeignKey("Recl.DiasSemana", t => t.DiaCorteId, cascadeDelete: false)
                .ForeignKey("Recl.DiasSemana", t => t.DiaPagoId, cascadeDelete: false)
                .ForeignKey("sist.Direcciones", t => t.DireccionId, cascadeDelete: false)
                .ForeignKey("sist.EstadosCiviles", t => t.EstadoCivilId, cascadeDelete: false)
                .ForeignKey("sist.Estatus", t => t.EstatusId, cascadeDelete: false)
                .ForeignKey("sist.Generos", t => t.GeneroId, cascadeDelete: false)
                .ForeignKey("Vtas.HorariosRequi", t => t.HorarioId, cascadeDelete: false)
                .ForeignKey("Recl.PeriodosPagos", t => t.PeriodoPagoId, cascadeDelete: false)
                .ForeignKey("sist.Prioridades", t => t.PrioridadId, cascadeDelete: false)
                .ForeignKey("sist.Usuarios", t => t.PropietarioId)
                .ForeignKey("Recl.TiposNominas", t => t.TipoNominaId, cascadeDelete: false)
                .ForeignKey("Recl.TiposReclutamientos", t => t.TipoReclutamientoId, cascadeDelete: false)
                .Index(t => t.ClienteId)
                .Index(t => t.TipoReclutamientoId)
                .Index(t => t.ClaseReclutamientoId)
                .Index(t => t.GeneroId)
                .Index(t => t.EstadoCivilId)
                .Index(t => t.AreaId)
                .Index(t => t.DiaCorteId)
                .Index(t => t.TipoNominaId)
                .Index(t => t.DiaPagoId)
                .Index(t => t.PeriodoPagoId)
                .Index(t => t.ContratoInicialId)
                .Index(t => t.ContratoFinalId)
                .Index(t => t.PropietarioId)
                .Index(t => t.AprobadorId)
                .Index(t => t.PrioridadId)
                .Index(t => t.EstatusId)
                .Index(t => t.DAMFO290Id)
                .Index(t => t.DireccionId)
                .Index(t => t.HorarioId);
            
            CreateTable(
                "Vtas.AptitudesRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        AptitudId = c.Int(nullable: false),
                        RequisicionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.Aptitudes", t => t.AptitudId, cascadeDelete: false)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .Index(t => t.AptitudId)
                .Index(t => t.RequisicionId);
            
            CreateTable(
                "Vtas.AsignacionesRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        RequisicionId = c.Guid(nullable: false),
                        GrpUsrId = c.Guid(nullable: false),
                        CRUD = c.String(maxLength: 5, fixedLength: true),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Personas", t => t.GrpUsrId, cascadeDelete: false)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .Index(t => t.RequisicionId)
                .Index(t => t.GrpUsrId);
            
            CreateTable(
                "Vtas.BeneficiosRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        TipoBeneficioId = c.Int(nullable: false),
                        Cantidad = c.Single(nullable: false),
                        Observaciones = c.String(maxLength: 500),
                        RequisicionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .ForeignKey("Recl.TiposBeneficios", t => t.TipoBeneficioId, cascadeDelete: false)
                .Index(t => t.TipoBeneficioId)
                .Index(t => t.RequisicionId);
            
            CreateTable(
                "Vtas.CompetenciasAreasRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CompetenciaId = c.Int(nullable: false),
                        Nivel = c.String(nullable: false, maxLength: 10),
                        RequisicionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.CompetenciasAreas", t => t.CompetenciaId, cascadeDelete: false)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .Index(t => t.CompetenciaId)
                .Index(t => t.RequisicionId);
            
            CreateTable(
                "Vtas.CompetenciasCardinalesRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CompetenciaId = c.Int(nullable: false),
                        Nivel = c.String(nullable: false, maxLength: 10),
                        RequisicionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.CompetenciasCardinales", t => t.CompetenciaId, cascadeDelete: false)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .Index(t => t.CompetenciaId)
                .Index(t => t.RequisicionId);
            
            CreateTable(
                "Vtas.CompetenciasGerencialesRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CompetenciaId = c.Int(nullable: false),
                        Nivel = c.String(nullable: false, maxLength: 10),
                        RequisicionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.CompetenciasGerenciales", t => t.CompetenciaId, cascadeDelete: false)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .Index(t => t.CompetenciaId)
                .Index(t => t.RequisicionId);
            
            CreateTable(
                "Vtas.DocumentosClienteRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Documento = c.String(nullable: false, maxLength: 100),
                        RequisicionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .Index(t => t.RequisicionId);
            
            CreateTable(
                "Vtas.EscolaridadesRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        EscolaridadId = c.Int(nullable: false),
                        EstadoEstudioId = c.Int(nullable: false),
                        RequisicionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.GradosEstudios", t => t.EscolaridadId, cascadeDelete: false)
                .ForeignKey("Recl.EstadosEstudios", t => t.EstadoEstudioId, cascadeDelete: false)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .Index(t => t.EscolaridadId)
                .Index(t => t.EstadoEstudioId)
                .Index(t => t.RequisicionId);
            
            CreateTable(
                "sist.Estatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(nullable: false, maxLength: 50),
                        ModuloId = c.Int(nullable: false),
                        Activo = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Modulos", t => t.ModuloId, cascadeDelete: true)
                .Index(t => t.ModuloId);
            
            CreateTable(
                "Vtas.HorariosRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 100),
                        deDiaId = c.Byte(nullable: false),
                        aDiaId = c.Byte(nullable: false),
                        deHora = c.String(nullable: false, maxLength: 25),
                        aHora = c.String(nullable: false, maxLength: 25),
                        numeroVacantes = c.Byte(nullable: false),
                        Especificaciones = c.String(nullable: false, maxLength: 500),
                        Activo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.DiasSemana", t => t.aDiaId, cascadeDelete: false)
                .ForeignKey("Recl.DiasSemana", t => t.deDiaId, cascadeDelete: false)
                .Index(t => t.deDiaId)
                .Index(t => t.aDiaId);
            
            CreateTable(
                "Vtas.ObservacionesRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Observaciones = c.String(nullable: false, maxLength: 100),
                        RequisicionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .Index(t => t.RequisicionId);
            
            CreateTable(
                "Vtas.PrestacionesClienteRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Prestamo = c.String(nullable: false, maxLength: 100),
                        RequisicionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .Index(t => t.RequisicionId);
            
            CreateTable(
                "sist.Prioridades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(nullable: false, maxLength: 50),
                        Activo = c.Boolean(),
                        ModuloId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Modulos", t => t.ModuloId, cascadeDelete: true)
                .Index(t => t.ModuloId);
            
            CreateTable(
                "Vtas.ProcesosRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Proceso = c.String(nullable: false, maxLength: 100),
                        RequisicionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .Index(t => t.RequisicionId);
            
            CreateTable(
                "Vtas.PsicometriasClienteRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Psicometria = c.String(maxLength: 50),
                        Descripcion = c.String(maxLength: 200),
                        RequisicionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .Index(t => t.RequisicionId);
            
            CreateTable(
                "Vtas.PsicometriasDamsaRequi",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        PsicometriaId = c.Int(nullable: false),
                        RequisicionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Recl.TiposPsicometrias", t => t.PsicometriaId, cascadeDelete: false)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .Index(t => t.PsicometriaId)
                .Index(t => t.RequisicionId);
            
            CreateTable(
                "sist.Cargoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.DiasObligatorios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        diaObligatorio = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.DocumentosDamsa",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        documentoDamsa = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BTra.FormasContacto",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CandidatoId = c.Guid(nullable: false),
                        CorreoElectronico = c.Boolean(nullable: false),
                        Celular = c.Boolean(nullable: false),
                        WhatsApp = c.Boolean(nullable: false),
                        TelLocal = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BTra.Candidatos", t => t.CandidatoId)
                .Index(t => t.CandidatoId);
            
            CreateTable(
                "BTra.FormulariosIniciales",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Paso = c.Int(nullable: false),
                        CandidatoId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BTra.Candidatos", t => t.CandidatoId)
                .Index(t => t.CandidatoId);
            
            CreateTable(
                "sist.Porcentages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        porcentage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "BTra.Postulaciones",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CandidatoId = c.Guid(nullable: false),
                        RequisicionId = c.Guid(nullable: false),
                        StatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("BTra.Candidatos", t => t.CandidatoId)
                .ForeignKey("Vtas.Requisiciones", t => t.RequisicionId, cascadeDelete: true)
                .ForeignKey("BTra.StatusPostulaciones", t => t.StatusId, cascadeDelete: false)
                .Index(t => t.CandidatoId)
                .Index(t => t.RequisicionId)
                .Index(t => t.StatusId);
            
            CreateTable(
                "BTra.StatusPostulaciones",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.PrestacionesLey",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        prestacionLey = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.RedesSociales",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        TipoRedSocialId = c.Byte(nullable: false),
                        redSocial = c.String(nullable: false, maxLength: 100),
                        PersonaId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Personas", t => t.PersonaId)
                .ForeignKey("BTra.TiposRedesSociales", t => t.TipoRedSocialId, cascadeDelete: true)
                .Index(t => t.TipoRedSocialId)
                .Index(t => t.PersonaId);
            
            CreateTable(
                "BTra.TiposRedesSociales",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        tipoRedSocial = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Recl.RutasPerfil",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        DireccionId = c.Guid(nullable: false),
                        Ruta = c.String(nullable: false, maxLength: 100),
                        Via = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Direcciones", t => t.DireccionId, cascadeDelete: true)
                .Index(t => t.DireccionId);
            
            CreateTable(
                "sist.UsuariosGrupos",
                c => new
                    {
                        Usuarios_Id = c.Guid(nullable: false),
                        Grupos_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Usuarios_Id, t.Grupos_Id })
                .ForeignKey("sist.Usuarios", t => t.Usuarios_Id)
                .ForeignKey("sist.Grupos", t => t.Grupos_Id)
                .Index(t => t.Usuarios_Id)
                .Index(t => t.Grupos_Id);
            
            CreateTable(
                "sist.RolesUsuarios",
                c => new
                    {
                        Roles_Id = c.Int(nullable: false),
                        Usuarios_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Roles_Id, t.Usuarios_Id })
                .ForeignKey("sist.Roles", t => t.Roles_Id, cascadeDelete: true)
                .ForeignKey("sist.Usuarios", t => t.Usuarios_Id, cascadeDelete: true)
                .Index(t => t.Roles_Id)
                .Index(t => t.Usuarios_Id);
            
            CreateTable(
                "BTra.Candidatos",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PaisNacimientoId = c.Int(nullable: false),
                        EstadoNacimientoId = c.Int(),
                        MunicipioNacimientoId = c.Int(),
                        CodigoPostal = c.String(),
                        GeneroId = c.Byte(nullable: false),
                        EstadoCivilId = c.Byte(),
                        esDiscapacitado = c.Boolean(nullable: false),
                        TipoDiscapacidadId = c.Int(),
                        tieneLicenciaConducir = c.Boolean(nullable: false),
                        TipoLicenciaId = c.Byte(),
                        tieneVehiculoPropio = c.Boolean(nullable: false),
                        puedeViajar = c.Boolean(nullable: false),
                        puedeRehubicarse = c.Boolean(nullable: false),
                        CURP = c.String(),
                        RFC = c.String(),
                        NSS = c.String(),
                        ImgProfileUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Personas", t => t.Id)
                .ForeignKey("sist.Paises", t => t.PaisNacimientoId, cascadeDelete: false)
                .ForeignKey("sist.Estados", t => t.EstadoNacimientoId)
                .ForeignKey("sist.Municipios", t => t.MunicipioNacimientoId)
                .ForeignKey("sist.Generos", t => t.GeneroId, cascadeDelete: false)
                .ForeignKey("sist.EstadosCiviles", t => t.EstadoCivilId)
                .ForeignKey("BTra.TiposDiscapacidades", t => t.TipoDiscapacidadId)
                .ForeignKey("BTra.TiposLicencias", t => t.TipoLicenciaId)
                .Index(t => t.Id)
                .Index(t => t.PaisNacimientoId)
                .Index(t => t.EstadoNacimientoId)
                .Index(t => t.MunicipioNacimientoId)
                .Index(t => t.GeneroId)
                .Index(t => t.EstadoCivilId)
                .Index(t => t.TipoDiscapacidadId)
                .Index(t => t.TipoLicenciaId);
            
            CreateTable(
                "Vtas.Clientes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RazonSocial = c.String(maxLength: 100),
                        Nombrecomercial = c.String(maxLength: 500),
                        RFC = c.String(maxLength: 15),
                        GiroEmpresaId = c.Int(nullable: false),
                        ActividadEmpresaId = c.Int(nullable: false),
                        TamanoEmpresaId = c.Int(nullable: false),
                        TipoEmpresaId = c.Int(nullable: false),
                        TipoBaseId = c.Int(nullable: false),
                        otraAgencia = c.Boolean(nullable: false),
                        esCliente = c.Boolean(nullable: false),
                        Clasificacion = c.String(nullable: false, maxLength: 10),
                        NumeroEmpleados = c.Int(nullable: false),
                        Activo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Personas", t => t.Id)
                .ForeignKey("sist.GiroEmpresas", t => t.GiroEmpresaId, cascadeDelete: false)
                .ForeignKey("Vtas.ActividadEmpresas", t => t.ActividadEmpresaId, cascadeDelete: false)
                .ForeignKey("Vtas.TamanosEmpresas", t => t.TamanoEmpresaId, cascadeDelete: false)
                .ForeignKey("Vtas.TiposEmpresas", t => t.TipoEmpresaId, cascadeDelete: false)
                .ForeignKey("Vtas.TiposBases", t => t.TipoBaseId, cascadeDelete: false)
                .Index(t => t.Id)
                .Index(t => t.GiroEmpresaId)
                .Index(t => t.ActividadEmpresaId)
                .Index(t => t.TamanoEmpresaId)
                .Index(t => t.TipoEmpresaId)
                .Index(t => t.TipoBaseId);
            
            CreateTable(
                "Vtas.Contactos",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Puesto = c.String(nullable: false, maxLength: 100),
                        ClienteId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Personas", t => t.Id)
                .ForeignKey("Vtas.Clientes", t => t.ClienteId)
                .Index(t => t.Id)
                .Index(t => t.ClienteId);
            
            CreateTable(
                "sist.Grupos",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Grupo = c.String(nullable: false, maxLength: 100),
                        Activo = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Personas", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "Vtas.Referenciados",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Puesto = c.String(nullable: false, maxLength: 100),
                        Clave = c.String(nullable: false, maxLength: 100),
                        ClienteId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Personas", t => t.Id)
                .ForeignKey("Vtas.Clientes", t => t.ClienteId)
                .Index(t => t.Id)
                .Index(t => t.ClienteId);
            
            CreateTable(
                "sist.Usuarios",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Usuario = c.String(nullable: false, maxLength: 20),
                        Password = c.String(nullable: false, maxLength: 40),
                        Activo = c.Boolean(nullable: false),
                        TipoUsuarioId = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("sist.Personas", t => t.Id)
                .ForeignKey("sist.TiposUsuarios", t => t.TipoUsuarioId, cascadeDelete: false)
                .Index(t => t.Id)
                .Index(t => t.TipoUsuarioId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("sist.Usuarios", "TipoUsuarioId", "sist.TiposUsuarios");
            DropForeignKey("sist.Usuarios", "Id", "sist.Personas");
            DropForeignKey("Vtas.Referenciados", "ClienteId", "Vtas.Clientes");
            DropForeignKey("Vtas.Referenciados", "Id", "sist.Personas");
            DropForeignKey("sist.Grupos", "Id", "sist.Personas");
            DropForeignKey("Vtas.Contactos", "ClienteId", "Vtas.Clientes");
            DropForeignKey("Vtas.Contactos", "Id", "sist.Personas");
            DropForeignKey("Vtas.Clientes", "TipoBaseId", "Vtas.TiposBases");
            DropForeignKey("Vtas.Clientes", "TipoEmpresaId", "Vtas.TiposEmpresas");
            DropForeignKey("Vtas.Clientes", "TamanoEmpresaId", "Vtas.TamanosEmpresas");
            DropForeignKey("Vtas.Clientes", "ActividadEmpresaId", "Vtas.ActividadEmpresas");
            DropForeignKey("Vtas.Clientes", "GiroEmpresaId", "sist.GiroEmpresas");
            DropForeignKey("Vtas.Clientes", "Id", "sist.Personas");
            DropForeignKey("BTra.Candidatos", "TipoLicenciaId", "BTra.TiposLicencias");
            DropForeignKey("BTra.Candidatos", "TipoDiscapacidadId", "BTra.TiposDiscapacidades");
            DropForeignKey("BTra.Candidatos", "EstadoCivilId", "sist.EstadosCiviles");
            DropForeignKey("BTra.Candidatos", "GeneroId", "sist.Generos");
            DropForeignKey("BTra.Candidatos", "MunicipioNacimientoId", "sist.Municipios");
            DropForeignKey("BTra.Candidatos", "EstadoNacimientoId", "sist.Estados");
            DropForeignKey("BTra.Candidatos", "PaisNacimientoId", "sist.Paises");
            DropForeignKey("BTra.Candidatos", "Id", "sist.Personas");
            DropForeignKey("Recl.RutasPerfil", "DireccionId", "sist.Direcciones");
            DropForeignKey("Recl.RedesSociales", "TipoRedSocialId", "BTra.TiposRedesSociales");
            DropForeignKey("Recl.RedesSociales", "PersonaId", "sist.Personas");
            DropForeignKey("BTra.Postulaciones", "StatusId", "BTra.StatusPostulaciones");
            DropForeignKey("BTra.Postulaciones", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("BTra.Postulaciones", "CandidatoId", "BTra.Candidatos");
            DropForeignKey("BTra.FormulariosIniciales", "CandidatoId", "BTra.Candidatos");
            DropForeignKey("BTra.FormasContacto", "CandidatoId", "BTra.Candidatos");
            DropForeignKey("Vtas.Requisiciones", "TipoReclutamientoId", "Recl.TiposReclutamientos");
            DropForeignKey("Vtas.Requisiciones", "TipoNominaId", "Recl.TiposNominas");
            DropForeignKey("Vtas.PsicometriasDamsaRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Vtas.PsicometriasDamsaRequi", "PsicometriaId", "Recl.TiposPsicometrias");
            DropForeignKey("Vtas.PsicometriasClienteRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Vtas.Requisiciones", "PropietarioId", "sist.Usuarios");
            DropForeignKey("Vtas.ProcesosRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Vtas.Requisiciones", "PrioridadId", "sist.Prioridades");
            DropForeignKey("sist.Prioridades", "ModuloId", "sist.Modulos");
            DropForeignKey("Vtas.PrestacionesClienteRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Vtas.Requisiciones", "PeriodoPagoId", "Recl.PeriodosPagos");
            DropForeignKey("Vtas.ObservacionesRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Vtas.Requisiciones", "HorarioId", "Vtas.HorariosRequi");
            DropForeignKey("Vtas.HorariosRequi", "deDiaId", "Recl.DiasSemana");
            DropForeignKey("Vtas.HorariosRequi", "aDiaId", "Recl.DiasSemana");
            DropForeignKey("Vtas.Requisiciones", "GeneroId", "sist.Generos");
            DropForeignKey("Vtas.Requisiciones", "EstatusId", "sist.Estatus");
            DropForeignKey("sist.Estatus", "ModuloId", "sist.Modulos");
            DropForeignKey("Vtas.Requisiciones", "EstadoCivilId", "sist.EstadosCiviles");
            DropForeignKey("Vtas.EscolaridadesRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Vtas.EscolaridadesRequi", "EstadoEstudioId", "Recl.EstadosEstudios");
            DropForeignKey("Vtas.EscolaridadesRequi", "EscolaridadId", "sist.GradosEstudios");
            DropForeignKey("Vtas.DocumentosClienteRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Vtas.Requisiciones", "DireccionId", "sist.Direcciones");
            DropForeignKey("Vtas.Requisiciones", "DiaPagoId", "Recl.DiasSemana");
            DropForeignKey("Vtas.Requisiciones", "DiaCorteId", "Recl.DiasSemana");
            DropForeignKey("Vtas.Requisiciones", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Vtas.Requisiciones", "ContratoInicialId", "Recl.TiposContratos");
            DropForeignKey("Vtas.Requisiciones", "ContratoFinalId", "Recl.TiposContratos");
            DropForeignKey("Vtas.CompetenciasGerencialesRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Vtas.CompetenciasGerencialesRequi", "CompetenciaId", "Recl.CompetenciasGerenciales");
            DropForeignKey("Vtas.CompetenciasCardinalesRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Vtas.CompetenciasCardinalesRequi", "CompetenciaId", "Recl.CompetenciasCardinales");
            DropForeignKey("Vtas.CompetenciasAreasRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Vtas.CompetenciasAreasRequi", "CompetenciaId", "Recl.CompetenciasAreas");
            DropForeignKey("Vtas.Requisiciones", "ClienteId", "Vtas.Clientes");
            DropForeignKey("Vtas.Requisiciones", "ClaseReclutamientoId", "Recl.ClasesReclutamientos");
            DropForeignKey("Vtas.BeneficiosRequi", "TipoBeneficioId", "Recl.TiposBeneficios");
            DropForeignKey("Vtas.BeneficiosRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Vtas.AsignacionesRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Vtas.AsignacionesRequi", "GrpUsrId", "sist.Personas");
            DropForeignKey("Vtas.Requisiciones", "AreaId", "sist.Areas");
            DropForeignKey("Vtas.AptitudesRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Vtas.AptitudesRequi", "AptitudId", "Recl.Aptitudes");
            DropForeignKey("Vtas.Requisiciones", "AprobadorId", "sist.Usuarios");
            DropForeignKey("Vtas.ActividadesRequi", "RequisicionId", "Vtas.Requisiciones");
            DropForeignKey("Recl.DAMFO_290", "TipoReclutamientoId", "Recl.TiposReclutamientos");
            DropForeignKey("Recl.DAMFO_290", "TipoNominaId", "Recl.TiposNominas");
            DropForeignKey("Recl.PsicometriasDamsa", "PsicometriaId", "Recl.TiposPsicometrias");
            DropForeignKey("Recl.PsicometriasDamsa", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Recl.PsicometriasCliente", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Recl.ProcesoPerfil", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Recl.PrestacionesClientePerfil", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Recl.DAMFO_290", "PeriodoPagoId", "Recl.PeriodosPagos");
            DropForeignKey("Recl.ObservacionesPerfil", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Recl.HorariosPerfiles", "deDiaId", "Recl.DiasSemana");
            DropForeignKey("Recl.HorariosPerfiles", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Recl.HorariosPerfiles", "aDiaId", "Recl.DiasSemana");
            DropForeignKey("Recl.DAMFO_290", "GeneroId", "sist.Generos");
            DropForeignKey("Recl.DAMFO_290", "EstadoCivilId", "sist.EstadosCiviles");
            DropForeignKey("Recl.EscolaridadesPerfil", "EstadoEstudioId", "Recl.EstadosEstudios");
            DropForeignKey("Recl.EscolaridadesPerfil", "EscolaridadId", "sist.GradosEstudios");
            DropForeignKey("Recl.EscolaridadesPerfil", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Recl.DocumentosClientes", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Recl.DAMFO_290", "DiaPagoId", "Recl.DiasSemana");
            DropForeignKey("Recl.DAMFO_290", "DiaCorteId", "Recl.DiasSemana");
            DropForeignKey("Recl.DAMFO_290", "ContratoInicialId", "Recl.TiposContratos");
            DropForeignKey("Recl.DAMFO_290", "ContratoFinalId", "Recl.TiposContratos");
            DropForeignKey("Recl.CompetenciaGerencialPerfil", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Recl.CompetenciaGerencialPerfil", "CompetenciaId", "Recl.CompetenciasGerenciales");
            DropForeignKey("Recl.CompetenciaCardinalPerfil", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Recl.CompetenciaCardinalPerfil", "CompetenciaId", "Recl.CompetenciasCardinales");
            DropForeignKey("Recl.CompetenciaAreaPerfil", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Recl.CompetenciaAreaPerfil", "CompetenciaId", "Recl.CompetenciasAreas");
            DropForeignKey("Recl.DAMFO_290", "ClienteId", "Vtas.Clientes");
            DropForeignKey("Recl.DAMFO_290", "ClaseReclutamientoId", "Recl.ClasesReclutamientos");
            DropForeignKey("Recl.BeneficiosPerfil", "TipoBeneficioId", "Recl.TiposBeneficios");
            DropForeignKey("Recl.BeneficiosPerfil", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Recl.DAMFO_290", "AreaId", "sist.Areas");
            DropForeignKey("Recl.AptitudesPerfil", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("Recl.AptitudesPerfil", "AptitudId", "Recl.Aptitudes");
            DropForeignKey("Recl.ActividadesPerfil", "DAMFO290Id", "Recl.DAMFO_290");
            DropForeignKey("BTra.AcercaDeMi", "PerfilExperienciaId", "BTra.PerfilExperiencia");
            DropForeignKey("sist.PerfilIdiomas", "PerfilCandidatoId", "BTra.PerfilCandidato");
            DropForeignKey("sist.PerfilIdiomas", "NivelHabladoId", "sist.Niveles");
            DropForeignKey("sist.PerfilIdiomas", "NivelEscritoId", "sist.Niveles");
            DropForeignKey("sist.PerfilIdiomas", "IdiomaId", "sist.Idiomas");
            DropForeignKey("BTra.Formaciones", "YearTerminoId", "sist.Years");
            DropForeignKey("BTra.Formaciones", "YearInicioId", "sist.Years");
            DropForeignKey("BTra.Formaciones", "PerfilCandidatoId", "BTra.PerfilCandidato");
            DropForeignKey("BTra.Formaciones", "MonthTerminoId", "sist.Months");
            DropForeignKey("BTra.Formaciones", "MonthInicioId", "sist.Months");
            DropForeignKey("BTra.Formaciones", "InstitucionEducativaId", "BTra.InstitucionesEducativas");
            DropForeignKey("BTra.Formaciones", "GradoEstudioId", "sist.GradosEstudios");
            DropForeignKey("BTra.Formaciones", "EstadoEstudioId", "Recl.EstadosEstudios");
            DropForeignKey("BTra.Formaciones", "DocumentoValidadorId", "BTra.DocumentosValidadores");
            DropForeignKey("BTra.Formaciones", "CarreraId", "BTra.Carreras");
            DropForeignKey("BTra.ExperienciasProfesionales", "YearTerminoId", "sist.Years");
            DropForeignKey("BTra.ExperienciasProfesionales", "YearInicioId", "sist.Years");
            DropForeignKey("BTra.ExperienciasProfesionales", "PerfilCandidatoId", "BTra.PerfilCandidato");
            DropForeignKey("BTra.ExperienciasProfesionales", "MonthTerminoId", "sist.Months");
            DropForeignKey("BTra.ExperienciasProfesionales", "MonthInicioId", "sist.Months");
            DropForeignKey("BTra.ExperienciasProfesionales", "GiroEmpresaId", "sist.GiroEmpresas");
            DropForeignKey("BTra.ExperienciasProfesionales", "AreaId", "sist.Areas");
            DropForeignKey("BTra.Cursos", "YearTerminoId", "sist.Years");
            DropForeignKey("BTra.Cursos", "YearInicioId", "sist.Years");
            DropForeignKey("BTra.Cursos", "PerfilCandidatoId", "BTra.PerfilCandidato");
            DropForeignKey("BTra.Cursos", "MonthTerminoId", "sist.Months");
            DropForeignKey("BTra.Cursos", "MonthInicioId", "sist.Months");
            DropForeignKey("BTra.Cursos", "InstitucionEducativaId", "BTra.InstitucionesEducativas");
            DropForeignKey("BTra.ConocimientosHabilidades", "PerfilCandidatoId", "BTra.PerfilCandidato");
            DropForeignKey("BTra.ConocimientosHabilidades", "NivelId", "sist.Niveles");
            DropForeignKey("BTra.ConocimientosHabilidades", "InstitucionEducativaId", "BTra.InstitucionesEducativas");
            DropForeignKey("BTra.Certificaciones", "YearTerminoId", "sist.Years");
            DropForeignKey("BTra.Certificaciones", "YearInicioId", "sist.Years");
            DropForeignKey("BTra.Certificaciones", "PerfilCandidatoId", "BTra.PerfilCandidato");
            DropForeignKey("BTra.Certificaciones", "MonthTerminoId", "sist.Months");
            DropForeignKey("BTra.Certificaciones", "MonthInicioId", "sist.Months");
            DropForeignKey("BTra.PerfilCandidato", "CandidatoId", "BTra.Candidatos");
            DropForeignKey("sist.Direcciones", "TipoDireccionId", "sist.TiposDirecciones");
            DropForeignKey("sist.RolesUsuarios", "Usuarios_Id", "sist.Usuarios");
            DropForeignKey("sist.RolesUsuarios", "Roles_Id", "sist.Roles");
            DropForeignKey("sist.Roles", "ModuloId", "sist.Modulos");
            DropForeignKey("sist.UsuariosGrupos", "Grupos_Id", "sist.Grupos");
            DropForeignKey("sist.UsuariosGrupos", "Usuarios_Id", "sist.Usuarios");
            DropForeignKey("Vtas.Agencias", "ClienteId", "Vtas.Clientes");
            DropForeignKey("Vtas.ActividadEmpresas", "GiroEmpresaId", "sist.GiroEmpresas");
            DropForeignKey("sist.Telefonos", "TipoTelefonoId", "sist.TiposTelefonos");
            DropForeignKey("sist.Telefonos", "PersonaId", "sist.Personas");
            DropForeignKey("sist.Emails", "PersonaId", "sist.Personas");
            DropForeignKey("sist.Direcciones", "PersonaId", "sist.Personas");
            DropForeignKey("sist.Direcciones", "PaisId", "sist.Paises");
            DropForeignKey("sist.Direcciones", "MunicipioId", "sist.Municipios");
            DropForeignKey("sist.Direcciones", "EstadoId", "sist.Estados");
            DropForeignKey("sist.Direcciones", "ColoniaId", "sist.Colonias");
            DropForeignKey("sist.Colonias", "PaisId", "sist.Paises");
            DropForeignKey("sist.Colonias", "MunicipioId", "sist.Municipios");
            DropForeignKey("sist.Municipios", "EstadoId", "sist.Estados");
            DropForeignKey("sist.Colonias", "EstadoId", "sist.Estados");
            DropForeignKey("sist.Estados", "PaisId", "sist.Paises");
            DropForeignKey("BTra.AcercaDeMi", "PerfilCandidatoId", "BTra.PerfilCandidato");
            DropForeignKey("BTra.AcercaDeMi", "AreaInteresId", "BTra.AreasInteres");
            DropForeignKey("BTra.AcercaDeMi", "AreaExperienciaId", "BTra.AreasExperiencia");
            DropIndex("sist.Usuarios", new[] { "TipoUsuarioId" });
            DropIndex("sist.Usuarios", new[] { "Id" });
            DropIndex("Vtas.Referenciados", new[] { "ClienteId" });
            DropIndex("Vtas.Referenciados", new[] { "Id" });
            DropIndex("sist.Grupos", new[] { "Id" });
            DropIndex("Vtas.Contactos", new[] { "ClienteId" });
            DropIndex("Vtas.Contactos", new[] { "Id" });
            DropIndex("Vtas.Clientes", new[] { "TipoBaseId" });
            DropIndex("Vtas.Clientes", new[] { "TipoEmpresaId" });
            DropIndex("Vtas.Clientes", new[] { "TamanoEmpresaId" });
            DropIndex("Vtas.Clientes", new[] { "ActividadEmpresaId" });
            DropIndex("Vtas.Clientes", new[] { "GiroEmpresaId" });
            DropIndex("Vtas.Clientes", new[] { "Id" });
            DropIndex("BTra.Candidatos", new[] { "TipoLicenciaId" });
            DropIndex("BTra.Candidatos", new[] { "TipoDiscapacidadId" });
            DropIndex("BTra.Candidatos", new[] { "EstadoCivilId" });
            DropIndex("BTra.Candidatos", new[] { "GeneroId" });
            DropIndex("BTra.Candidatos", new[] { "MunicipioNacimientoId" });
            DropIndex("BTra.Candidatos", new[] { "EstadoNacimientoId" });
            DropIndex("BTra.Candidatos", new[] { "PaisNacimientoId" });
            DropIndex("BTra.Candidatos", new[] { "Id" });
            DropIndex("sist.RolesUsuarios", new[] { "Usuarios_Id" });
            DropIndex("sist.RolesUsuarios", new[] { "Roles_Id" });
            DropIndex("sist.UsuariosGrupos", new[] { "Grupos_Id" });
            DropIndex("sist.UsuariosGrupos", new[] { "Usuarios_Id" });
            DropIndex("Recl.RutasPerfil", new[] { "DireccionId" });
            DropIndex("Recl.RedesSociales", new[] { "PersonaId" });
            DropIndex("Recl.RedesSociales", new[] { "TipoRedSocialId" });
            DropIndex("BTra.Postulaciones", new[] { "StatusId" });
            DropIndex("BTra.Postulaciones", new[] { "RequisicionId" });
            DropIndex("BTra.Postulaciones", new[] { "CandidatoId" });
            DropIndex("BTra.FormulariosIniciales", new[] { "CandidatoId" });
            DropIndex("BTra.FormasContacto", new[] { "CandidatoId" });
            DropIndex("Vtas.PsicometriasDamsaRequi", new[] { "RequisicionId" });
            DropIndex("Vtas.PsicometriasDamsaRequi", new[] { "PsicometriaId" });
            DropIndex("Vtas.PsicometriasClienteRequi", new[] { "RequisicionId" });
            DropIndex("Vtas.ProcesosRequi", new[] { "RequisicionId" });
            DropIndex("sist.Prioridades", new[] { "ModuloId" });
            DropIndex("Vtas.PrestacionesClienteRequi", new[] { "RequisicionId" });
            DropIndex("Vtas.ObservacionesRequi", new[] { "RequisicionId" });
            DropIndex("Vtas.HorariosRequi", new[] { "aDiaId" });
            DropIndex("Vtas.HorariosRequi", new[] { "deDiaId" });
            DropIndex("sist.Estatus", new[] { "ModuloId" });
            DropIndex("Vtas.EscolaridadesRequi", new[] { "RequisicionId" });
            DropIndex("Vtas.EscolaridadesRequi", new[] { "EstadoEstudioId" });
            DropIndex("Vtas.EscolaridadesRequi", new[] { "EscolaridadId" });
            DropIndex("Vtas.DocumentosClienteRequi", new[] { "RequisicionId" });
            DropIndex("Vtas.CompetenciasGerencialesRequi", new[] { "RequisicionId" });
            DropIndex("Vtas.CompetenciasGerencialesRequi", new[] { "CompetenciaId" });
            DropIndex("Vtas.CompetenciasCardinalesRequi", new[] { "RequisicionId" });
            DropIndex("Vtas.CompetenciasCardinalesRequi", new[] { "CompetenciaId" });
            DropIndex("Vtas.CompetenciasAreasRequi", new[] { "RequisicionId" });
            DropIndex("Vtas.CompetenciasAreasRequi", new[] { "CompetenciaId" });
            DropIndex("Vtas.BeneficiosRequi", new[] { "RequisicionId" });
            DropIndex("Vtas.BeneficiosRequi", new[] { "TipoBeneficioId" });
            DropIndex("Vtas.AsignacionesRequi", new[] { "GrpUsrId" });
            DropIndex("Vtas.AsignacionesRequi", new[] { "RequisicionId" });
            DropIndex("Vtas.AptitudesRequi", new[] { "RequisicionId" });
            DropIndex("Vtas.AptitudesRequi", new[] { "AptitudId" });
            DropIndex("Vtas.Requisiciones", new[] { "HorarioId" });
            DropIndex("Vtas.Requisiciones", new[] { "DireccionId" });
            DropIndex("Vtas.Requisiciones", new[] { "DAMFO290Id" });
            DropIndex("Vtas.Requisiciones", new[] { "EstatusId" });
            DropIndex("Vtas.Requisiciones", new[] { "PrioridadId" });
            DropIndex("Vtas.Requisiciones", new[] { "AprobadorId" });
            DropIndex("Vtas.Requisiciones", new[] { "PropietarioId" });
            DropIndex("Vtas.Requisiciones", new[] { "ContratoFinalId" });
            DropIndex("Vtas.Requisiciones", new[] { "ContratoInicialId" });
            DropIndex("Vtas.Requisiciones", new[] { "PeriodoPagoId" });
            DropIndex("Vtas.Requisiciones", new[] { "DiaPagoId" });
            DropIndex("Vtas.Requisiciones", new[] { "TipoNominaId" });
            DropIndex("Vtas.Requisiciones", new[] { "DiaCorteId" });
            DropIndex("Vtas.Requisiciones", new[] { "AreaId" });
            DropIndex("Vtas.Requisiciones", new[] { "EstadoCivilId" });
            DropIndex("Vtas.Requisiciones", new[] { "GeneroId" });
            DropIndex("Vtas.Requisiciones", new[] { "ClaseReclutamientoId" });
            DropIndex("Vtas.Requisiciones", new[] { "TipoReclutamientoId" });
            DropIndex("Vtas.Requisiciones", new[] { "ClienteId" });
            DropIndex("Vtas.ActividadesRequi", new[] { "RequisicionId" });
            DropIndex("Recl.PsicometriasDamsa", new[] { "DAMFO290Id" });
            DropIndex("Recl.PsicometriasDamsa", new[] { "PsicometriaId" });
            DropIndex("Recl.PsicometriasCliente", new[] { "DAMFO290Id" });
            DropIndex("Recl.ProcesoPerfil", new[] { "DAMFO290Id" });
            DropIndex("Recl.PrestacionesClientePerfil", new[] { "DAMFO290Id" });
            DropIndex("Recl.ObservacionesPerfil", new[] { "DAMFO290Id" });
            DropIndex("Recl.HorariosPerfiles", new[] { "DAMFO290Id" });
            DropIndex("Recl.HorariosPerfiles", new[] { "aDiaId" });
            DropIndex("Recl.HorariosPerfiles", new[] { "deDiaId" });
            DropIndex("Recl.EscolaridadesPerfil", new[] { "DAMFO290Id" });
            DropIndex("Recl.EscolaridadesPerfil", new[] { "EstadoEstudioId" });
            DropIndex("Recl.EscolaridadesPerfil", new[] { "EscolaridadId" });
            DropIndex("Recl.DocumentosClientes", new[] { "DAMFO290Id" });
            DropIndex("Recl.CompetenciaGerencialPerfil", new[] { "DAMFO290Id" });
            DropIndex("Recl.CompetenciaGerencialPerfil", new[] { "CompetenciaId" });
            DropIndex("Recl.CompetenciaCardinalPerfil", new[] { "DAMFO290Id" });
            DropIndex("Recl.CompetenciaCardinalPerfil", new[] { "CompetenciaId" });
            DropIndex("Recl.CompetenciaAreaPerfil", new[] { "DAMFO290Id" });
            DropIndex("Recl.CompetenciaAreaPerfil", new[] { "CompetenciaId" });
            DropIndex("Recl.BeneficiosPerfil", new[] { "DAMFO290Id" });
            DropIndex("Recl.BeneficiosPerfil", new[] { "TipoBeneficioId" });
            DropIndex("Recl.AptitudesPerfil", new[] { "DAMFO290Id" });
            DropIndex("Recl.AptitudesPerfil", new[] { "AptitudId" });
            DropIndex("Recl.DAMFO_290", new[] { "ContratoFinalId" });
            DropIndex("Recl.DAMFO_290", new[] { "ContratoInicialId" });
            DropIndex("Recl.DAMFO_290", new[] { "PeriodoPagoId" });
            DropIndex("Recl.DAMFO_290", new[] { "DiaPagoId" });
            DropIndex("Recl.DAMFO_290", new[] { "TipoNominaId" });
            DropIndex("Recl.DAMFO_290", new[] { "DiaCorteId" });
            DropIndex("Recl.DAMFO_290", new[] { "AreaId" });
            DropIndex("Recl.DAMFO_290", new[] { "EstadoCivilId" });
            DropIndex("Recl.DAMFO_290", new[] { "GeneroId" });
            DropIndex("Recl.DAMFO_290", new[] { "ClaseReclutamientoId" });
            DropIndex("Recl.DAMFO_290", new[] { "TipoReclutamientoId" });
            DropIndex("Recl.DAMFO_290", new[] { "ClienteId" });
            DropIndex("Recl.ActividadesPerfil", new[] { "DAMFO290Id" });
            DropIndex("sist.PerfilIdiomas", new[] { "PerfilCandidatoId" });
            DropIndex("sist.PerfilIdiomas", new[] { "NivelHabladoId" });
            DropIndex("sist.PerfilIdiomas", new[] { "NivelEscritoId" });
            DropIndex("sist.PerfilIdiomas", new[] { "IdiomaId" });
            DropIndex("BTra.Formaciones", new[] { "PerfilCandidatoId" });
            DropIndex("BTra.Formaciones", new[] { "MonthTerminoId" });
            DropIndex("BTra.Formaciones", new[] { "YearTerminoId" });
            DropIndex("BTra.Formaciones", new[] { "MonthInicioId" });
            DropIndex("BTra.Formaciones", new[] { "YearInicioId" });
            DropIndex("BTra.Formaciones", new[] { "CarreraId" });
            DropIndex("BTra.Formaciones", new[] { "DocumentoValidadorId" });
            DropIndex("BTra.Formaciones", new[] { "EstadoEstudioId" });
            DropIndex("BTra.Formaciones", new[] { "GradoEstudioId" });
            DropIndex("BTra.Formaciones", new[] { "InstitucionEducativaId" });
            DropIndex("BTra.ExperienciasProfesionales", new[] { "PerfilCandidatoId" });
            DropIndex("BTra.ExperienciasProfesionales", new[] { "MonthTerminoId" });
            DropIndex("BTra.ExperienciasProfesionales", new[] { "YearTerminoId" });
            DropIndex("BTra.ExperienciasProfesionales", new[] { "MonthInicioId" });
            DropIndex("BTra.ExperienciasProfesionales", new[] { "YearInicioId" });
            DropIndex("BTra.ExperienciasProfesionales", new[] { "AreaId" });
            DropIndex("BTra.ExperienciasProfesionales", new[] { "GiroEmpresaId" });
            DropIndex("BTra.Cursos", new[] { "PerfilCandidatoId" });
            DropIndex("BTra.Cursos", new[] { "MonthTerminoId" });
            DropIndex("BTra.Cursos", new[] { "YearTerminoId" });
            DropIndex("BTra.Cursos", new[] { "MonthInicioId" });
            DropIndex("BTra.Cursos", new[] { "YearInicioId" });
            DropIndex("BTra.Cursos", new[] { "InstitucionEducativaId" });
            DropIndex("BTra.ConocimientosHabilidades", new[] { "PerfilCandidatoId" });
            DropIndex("BTra.ConocimientosHabilidades", new[] { "NivelId" });
            DropIndex("BTra.ConocimientosHabilidades", new[] { "InstitucionEducativaId" });
            DropIndex("BTra.Certificaciones", new[] { "PerfilCandidatoId" });
            DropIndex("BTra.Certificaciones", new[] { "MonthTerminoId" });
            DropIndex("BTra.Certificaciones", new[] { "YearTerminoId" });
            DropIndex("BTra.Certificaciones", new[] { "MonthInicioId" });
            DropIndex("BTra.Certificaciones", new[] { "YearInicioId" });
            DropIndex("sist.Roles", new[] { "ModuloId" });
            DropIndex("Vtas.Agencias", new[] { "ClienteId" });
            DropIndex("Vtas.ActividadEmpresas", new[] { "GiroEmpresaId" });
            DropIndex("sist.Telefonos", new[] { "PersonaId" });
            DropIndex("sist.Telefonos", new[] { "TipoTelefonoId" });
            DropIndex("sist.Emails", new[] { "PersonaId" });
            DropIndex("sist.Municipios", new[] { "EstadoId" });
            DropIndex("sist.Estados", new[] { "PaisId" });
            DropIndex("sist.Colonias", new[] { "PaisId" });
            DropIndex("sist.Colonias", new[] { "EstadoId" });
            DropIndex("sist.Colonias", new[] { "MunicipioId" });
            DropIndex("sist.Direcciones", new[] { "PersonaId" });
            DropIndex("sist.Direcciones", new[] { "ColoniaId" });
            DropIndex("sist.Direcciones", new[] { "MunicipioId" });
            DropIndex("sist.Direcciones", new[] { "EstadoId" });
            DropIndex("sist.Direcciones", new[] { "PaisId" });
            DropIndex("sist.Direcciones", new[] { "TipoDireccionId" });
            DropIndex("BTra.PerfilCandidato", new[] { "CandidatoId" });
            DropIndex("BTra.AcercaDeMi", new[] { "PerfilCandidatoId" });
            DropIndex("BTra.AcercaDeMi", new[] { "PerfilExperienciaId" });
            DropIndex("BTra.AcercaDeMi", new[] { "AreaInteresId" });
            DropIndex("BTra.AcercaDeMi", new[] { "AreaExperienciaId" });
            DropTable("sist.Usuarios");
            DropTable("Vtas.Referenciados");
            DropTable("sist.Grupos");
            DropTable("Vtas.Contactos");
            DropTable("Vtas.Clientes");
            DropTable("BTra.Candidatos");
            DropTable("sist.RolesUsuarios");
            DropTable("sist.UsuariosGrupos");
            DropTable("Recl.RutasPerfil");
            DropTable("BTra.TiposRedesSociales");
            DropTable("Recl.RedesSociales");
            DropTable("Recl.PrestacionesLey");
            DropTable("BTra.StatusPostulaciones");
            DropTable("BTra.Postulaciones");
            DropTable("sist.Porcentages");
            DropTable("BTra.FormulariosIniciales");
            DropTable("BTra.FormasContacto");
            DropTable("Recl.DocumentosDamsa");
            DropTable("Recl.DiasObligatorios");
            DropTable("sist.Cargoes");
            DropTable("Vtas.PsicometriasDamsaRequi");
            DropTable("Vtas.PsicometriasClienteRequi");
            DropTable("Vtas.ProcesosRequi");
            DropTable("sist.Prioridades");
            DropTable("Vtas.PrestacionesClienteRequi");
            DropTable("Vtas.ObservacionesRequi");
            DropTable("Vtas.HorariosRequi");
            DropTable("sist.Estatus");
            DropTable("Vtas.EscolaridadesRequi");
            DropTable("Vtas.DocumentosClienteRequi");
            DropTable("Vtas.CompetenciasGerencialesRequi");
            DropTable("Vtas.CompetenciasCardinalesRequi");
            DropTable("Vtas.CompetenciasAreasRequi");
            DropTable("Vtas.BeneficiosRequi");
            DropTable("Vtas.AsignacionesRequi");
            DropTable("Vtas.AptitudesRequi");
            DropTable("Vtas.Requisiciones");
            DropTable("Vtas.ActividadesRequi");
            DropTable("Recl.TiposReclutamientos");
            DropTable("Recl.TiposNominas");
            DropTable("Recl.TiposPsicometrias");
            DropTable("Recl.PsicometriasDamsa");
            DropTable("Recl.PsicometriasCliente");
            DropTable("Recl.ProcesoPerfil");
            DropTable("Recl.PrestacionesClientePerfil");
            DropTable("Recl.PeriodosPagos");
            DropTable("Recl.ObservacionesPerfil");
            DropTable("Recl.HorariosPerfiles");
            DropTable("Recl.EscolaridadesPerfil");
            DropTable("Recl.DocumentosClientes");
            DropTable("Recl.DiasSemana");
            DropTable("Recl.TiposContratos");
            DropTable("Recl.CompetenciasGerenciales");
            DropTable("Recl.CompetenciaGerencialPerfil");
            DropTable("Recl.CompetenciasCardinales");
            DropTable("Recl.CompetenciaCardinalPerfil");
            DropTable("Recl.CompetenciasAreas");
            DropTable("Recl.CompetenciaAreaPerfil");
            DropTable("Recl.ClasesReclutamientos");
            DropTable("Recl.TiposBeneficios");
            DropTable("Recl.BeneficiosPerfil");
            DropTable("Recl.Aptitudes");
            DropTable("Recl.AptitudesPerfil");
            DropTable("Recl.DAMFO_290");
            DropTable("Recl.ActividadesPerfil");
            DropTable("BTra.PerfilExperiencia");
            DropTable("sist.Idiomas");
            DropTable("sist.PerfilIdiomas");
            DropTable("sist.GradosEstudios");
            DropTable("Recl.EstadosEstudios");
            DropTable("BTra.DocumentosValidadores");
            DropTable("BTra.Carreras");
            DropTable("BTra.Formaciones");
            DropTable("sist.Areas");
            DropTable("BTra.ExperienciasProfesionales");
            DropTable("BTra.Cursos");
            DropTable("sist.Niveles");
            DropTable("BTra.InstitucionesEducativas");
            DropTable("BTra.ConocimientosHabilidades");
            DropTable("sist.Years");
            DropTable("sist.Months");
            DropTable("BTra.Certificaciones");
            DropTable("BTra.TiposLicencias");
            DropTable("BTra.TiposDiscapacidades");
            DropTable("sist.Generos");
            DropTable("sist.EstadosCiviles");
            DropTable("sist.TiposDirecciones");
            DropTable("sist.TiposUsuarios");
            DropTable("sist.Modulos");
            DropTable("sist.Roles");
            DropTable("Vtas.TiposEmpresas");
            DropTable("Vtas.TiposBases");
            DropTable("Vtas.TamanosEmpresas");
            DropTable("Vtas.Agencias");
            DropTable("sist.GiroEmpresas");
            DropTable("Vtas.ActividadEmpresas");
            DropTable("sist.TiposTelefonos");
            DropTable("sist.Telefonos");
            DropTable("sist.Emails");
            DropTable("sist.Municipios");
            DropTable("sist.Paises");
            DropTable("sist.Estados");
            DropTable("sist.Colonias");
            DropTable("sist.Direcciones");
            DropTable("sist.Personas");
            DropTable("BTra.PerfilCandidato");
            DropTable("BTra.AreasInteres");
            DropTable("BTra.AreasExperiencia");
            DropTable("BTra.AcercaDeMi");
        }
    }
}
