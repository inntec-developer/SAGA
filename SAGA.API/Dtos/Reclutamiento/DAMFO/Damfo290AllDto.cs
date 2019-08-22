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

    public class EscoPerfilDto
    {
        public Guid Id { get; set; }
        public int EscolaridadId { get; set; }
        public int EstadoEstudioId { get; set; }
        public Guid Damfo290Id { get; set; }
        public string Usuario { get; set; }
    }

    public class BenPerfilDto
    {
        public Guid Id { get; set; }
        public int TipoBeneficioId { get; set; }
        public float Cantidad { get; set; }
        public string Observaciones { get; set; }
        public Guid DAMFO290Id { get; set; }
        public string Usuario { get; set; }
        public string Action { get; set; }
    }

    public class HrsPerfilDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public byte deDiaId { get; set; }
        public byte aDiaId { get; set; }
        public DateTime deHora { get; set; }
        public DateTime aHora { get; set; }
        public byte numeroVacantes { get; set; }
        public string Especificaciones { get; set; }
        public bool Activo { get; set; }
        public Guid DAMFO290Id { get; set; }
        public string Usuario { get; set; }
        public string Action { get; set; }
    }
    public class ActPerfilDto
    {
        public Guid Id { get; set; }
        public string  Actividad { get; set; }
        public Guid DAMFO290Id { get; set; }
        public string Usuario { get; set; }
        public string Action { get; set; }
    }
    public class ObsPerfilDto
    {
        public Guid Id { get; set; }
        public string Observaciones { get; set; }
        public Guid DAMFO290Id { get; set; }
        public string Usuario { get; set; }
        public string Action { get; set; }
    }
    public class PstDamsaDto
    {
        public Guid Id { get; set; }
        public int PsicometriaId { get; set; }
        public Guid DAMFO290Id { get; set; }
        public string Usuario { get; set; }
        public string Action { get; set; }
    }

    public class PstClienteDto
    {
        public Guid Id { get; set; }
        public string Psicometria { get; set; }
        public string Descripcion { get; set; }
        public Guid DAMFO290Id { get; set; }
        public string Usuario { get; set; }
        public string Action { get; set; }
    }

    public class DocClienteDto
    {
        public Guid Id { get; set; }
        public string Documento { get; set; }
        public Guid DAMFO290Id { get; set; }
        public string Usuario { get; set; }
        public string Action { get; set; }
    }
    public class ProcesoPerfilDto
    {
        public Guid Id { get; set; }
        public string Proceso { get; set; }
        public int Orden { get; set; }
        public Guid DAMFO290Id { get; set; }
        public string Usuario { get; set; }
        public string Action { get; set; }
    }

    public class PrestacionPerfilDto
    {
        public Guid Id { get; set; }
        public string Prestacion { get; set; }
        public Guid DAMFO290Id { get; set; }
        public string Usuario { get; set; }
        public string Action { get; set; }
    }

    public class CompeteciasPerfilDto
    {
        public Guid Id { get; set; }
        public int CompetenciaId { get; set; }
        public string Nivel { get; set; }
        public Guid DAMFO290Id { get; set; }
        public string Usuario { get; set; }
        public string Action { get; set; }
    }
}