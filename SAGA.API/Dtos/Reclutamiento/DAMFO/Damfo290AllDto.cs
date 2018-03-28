using System;

namespace SAGA.API.Dtos
{
    public class Damfo290AllDto
    {
        public Guid ClienteId { get; set; }
        public int TipoReclutamientoId { get; set; }
        public int ClaseReclutamientoId { get; set; }
        public string NombrePerfil { get; set; }
        public byte GeneroId { get; set; }
        public int EdadMinima { get; set; }
        public int EdadMaxima { get; set; }
        public byte EstadoCivilId { get; set; }
        public int AreaId { get; set; }
        public string Experiencia { get; set; }
        public decimal SueldoMinimo { get; set; }
        public decimal SueldoMaximo { get; set; }
        public byte DiaCorteId { get; set; }
        public int TipoNominaId { get; set; }
        public byte DiaPagoId { get; set; }
        public int PeriodoPagoId { get; set; }
        public string Especifique { get; set; }
        public int ContratoInicialId { get; set; }
        public int TiempoContratoId { get; set; }
        public bool Activo { get; set; }
        public bool FlexibilidadHorario { get; set; }
        public int JornadaLaboralId { get; set; }
        public int TipoModalidadId { get; set; }
    }
}