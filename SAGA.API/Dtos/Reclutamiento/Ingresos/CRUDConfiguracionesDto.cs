using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Reclutamiento.Ingresos
{
    public class CRUDConfiguracionesDto
    {
        public byte Id { get; set; }
        public byte crud { get; set; }
        public string Catalogo { get; set; }
        public Guid Usuario { get; set; }
        public ConfigVacaciones VacacionesPpal { get; set; }
        public List<ConfigVacacionesDias> VacacionesDias { get; set; }
        public List<GrupoVacaciones> VacacionesRelacion { get; set; }
        public ConfigIncapacidades Incapacidades { get; set; }
        public List<ConfigIncapacidadesDias> IncapacidadesDias { get; set; }
        public List<GruposIncapacidad> IncapacidadesRelacion { get; set; }
        public ConfigDiasEconomicos DiasEconomicos { get; set; }
        public List<ConfigDiasEconomicosDias> DiasEconomicosDias { get; set; }
        public List<GruposDiasEconomicos> DiasEconomicosRel { get; set; }
        public ConfigGuardias Guardias { get; set; }
        public List<GruposGuardia> GuardiasRelacion { get; set; }
        public ConfigTiempoExtra TiempoExtra { get; set; }
        public List<GruposTiempoExtra> TiempoExtraRelacion { get; set; }
        public ConfigSuspensionNotas Suspensiones { get; set; }
        public List<ConfigSuspensionNotasDias> SuspensionesDias { get; set; }
        public List<GruposSuspension> SuspensionesRelacion { get; set; }
        public ConfigPrima PrimaDominical { get; set; }
        public List<GruposPrima> PrimaRelacion { get; set; }
        public ConfigTolerancia Tolerancia { get; set; }
        public List<ConfigToleranciaTiempo> ToleranciaTiempo { get; set; }
        public List<GruposTolerancia> ToleranciaRelacion { get; set; }
        public List<CandidatoHorario> CandidatosHorario { get; set; }
    }
}