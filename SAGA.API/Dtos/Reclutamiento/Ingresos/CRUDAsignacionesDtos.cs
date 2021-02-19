using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.BOL;

namespace SAGA.API.Dtos.Reclutamiento.Ingresos
{
    public class CRUDAsignacionesDtos
    {
        public byte crud { get; set; }
        public String Catalogo { get; set; }
        public PeriodoVacaciones PeriodoVacaciones { get; set; }
        public PeriodoDE PeriodoDE { get; set; }
        public PeriodoIncapacidad PeriodoIncapacidad { get; set; }
        public PeriodoPermisos PeriodoPermisos { get; set; }
        public PeriodoGuardia PeriodoGuardia { get; set; }
        public PeriodoSuspension PeriodoSuspension { get; set; }
        public List<PeriodoSuspension> PeriodoSuspensionList { get; set; }
        public PeriodoActa PeriodoActa { get; set; }
        public List<PeriodoActa> PeriodoActaList { get; set; }
        public PeriodoHorasExtras PeriodoTiempoExtra { get; set; }
        public List<PeriodoHorasExtras> PeriodoTiempoExtraList { get; set; }
        public PeriodoBonos PeriodosBonos { get; set; }
        public List<PeriodoBonos> PeriodoBonosList { get; set; }
        public PeriodoCompensaciones PeriodoCompensaciones { get; set; }
        public List<PeriodoCompensaciones> PeriodoCompensacionesList { get; set; }
    }
}