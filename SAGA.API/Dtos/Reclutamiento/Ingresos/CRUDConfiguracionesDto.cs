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
        public Guid IdG { get; set; }
        public byte crud { get; set; }
        public string Catalogo { get; set; }
        public Guid Usuario { get; set; }
        public Guid ClienteId { get; set; }
        public ConfigVacaciones VacacionesPpal { get; set; }
        public List<ConfigVacacionesDias> VacacionesDias { get; set; }
        public List<EmpleadoVacaciones> VacacionesRelacion { get; set; }
        public ConfigIncapacidades Incapacidades { get; set; }
        public List<ConfigIncapacidadesDias> IncapacidadesDias { get; set; }
        public List<EmpleadoIncapacidad> IncapacidadesRelacion { get; set; }
        public ConfigDiasEconomicos DiasEconomicos { get; set; }
        public List<ConfigDiasEconomicosDias> DiasEconomicosDias { get; set; }
        public List<EmpleadoDiasEconomicos> DiasEconomicosRel { get; set; }
        public ConfigGuardias Guardias { get; set; }
        public List<EmpleadoGuardia> GuardiasRelacion { get; set; }
        public ConfigTiempoExtra TiempoExtra { get; set; }
        public List<EmpleadoTiempoExtra> TiempoExtraRelacion { get; set; }
        public ConfigSuspensionNotas Suspensiones { get; set; }
        public List<ConfigSuspensionNotasDias> SuspensionesDias { get; set; }
        public List<EmpleadoSuspension> SuspensionesRelacion { get; set; }
        public ConfigPrima PrimaDominical { get; set; }
        public List<EmpleadoPrima> PrimaRelacion { get; set; }
        public ConfigTolerancia Tolerancia { get; set; }
        public List<ConfigToleranciaTiempo> ToleranciaTiempo { get; set; }
        public List<EmpleadoTolerancia> ToleranciaRelacion { get; set; }
        public List<EmpleadoHorario> CandidatosHorario { get; set; }
        public List<AsignacionesIngresosDto> Asignacion { get; set; }
        public ConfigBono ConfigBono { get; set; }
    }

    public class AsignacionesIngresosDto
    {
        public int Id { get; set; }
        public Guid IdG { get; set; }
        public Guid EmpleadoId { get; set; }
        public bool Activo { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }
    }
}