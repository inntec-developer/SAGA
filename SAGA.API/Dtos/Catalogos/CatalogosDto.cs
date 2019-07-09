using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class CatalogosDto
    {
        public int opt { get; set; }
        public string Usuario { get; set; }
        #region Sistema
        public Catalogos Catalogos { get; set; }
        public List<Pais> Pais { get; set; }
        public List<EstadoDto> Estado { get; set; }
        public List<MunicipioDto> Municipio { get; set; }
        public List<ColoniasDto> Colonia { get; set; }
        public List<TpTelefonosDto> TpTelefono { get; set; }
        public List<TpUsuarioDto> TpUsuario { get; set; }
        public List<EstadoCivilDto> EstadoCivil { get; set; }
        public List<DepartamentosDto> Departamentos { get; set; }
        public List<AreaDto> Areas { get; set; }
        public List<EscolaridadesDto> Escolaridades { get; set; }
        public List<NivelDto> Nivel { get; set; }
        public List<MedioDto> Medio { get; set; }
        public List<IdiomaDto> Idioma { get; set; }
        public List<LogCatalogos> Log { get; set; }
        public List<DiscapacidadDto> Discapacidad { get; set; }
        public List<TipoLicenciaDto> TipoLicencia { get; set; }
        public List<TipoExamenDto> TipoExamen { get; set; }
        public List<GiroEmpresaDto> GiroEmpresa { get; set; }
        public List<TamanoEmpresaDto> TamanoEmpresa { get; set; }
        public List<TiposBasesDto> TiposBase { get; set; }
        public List<ActividadEmpresaDto> ActividadEmpresa { get; set; }
        public List<PerfilExpDto> PerfilExperiencia { get; set; }
        public List<AreaExpDto> AreaExperiencia { get; set; }
        public List<AptitudDto> Aptitud { get; set; }
        public List<AreaInteresDto> AreaInteres { get; set; }
        public List<JornadaLaboralDto> JornadaLaboral { get; set; }
        public List<TpModalidadDto> TipoModalidad { get; set; }
        public List<TiposPiscoDto> TipoPsicometria { get; set; }
        public List<PeriodoPagoDto> PeriodoPago { get; set; }
        public List<TpNominaDto> TipoNomina { get; set; }
        public List<DiasSemanaDto> DiasSemana { get; set; }
        public List<BeneficiosPerfilDto> BeneficioPerfil { get; set; }
        public List<TipoContratoDto> TipoContrato { get; set; }
        public List<TiemposContratoDto> TiemposContrato { get; set; }
        public List<DocDamsaDto> DocDamsa { get; set; }
        public List<PrestacionesdeLeyDto> PrestacionesLey { get; set; }
        public List<RolesDto> Roles { get; set; }
        #endregion
        #region Reclutamiento
        #endregion
        #region Ventas
        #endregion
    }
}