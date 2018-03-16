using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class DAMFO_290
    {
        [Key]
        public Guid Id { get; set; }
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
        public DateTime fch_Creacion { get; set; }
        public DateTime? fch_Modificacion { get; set; }
        public bool FlexibilidadHorario { get; set; }
        public int JornadaLaboralId { get; set; }
        public int TipoModalidadId { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid? UsuarioMod { get; set; } 

        public virtual Cliente Cliente { get; set; }
        public virtual TipoReclutamiento TipoReclutamiento { get; set; }
        public virtual ClaseReclutamiento ClaseReclutamiento { get; set; }
        public virtual Genero Genero { get; set; }
        public virtual EstadoCivil EstadoCivil { get; set; }
        public virtual Area Area { get; set; }
        public virtual DiaSemana DiaCorte { get; set; }
        public virtual TipodeNomina TipoNomina { get; set; }
        public virtual DiaSemana DiaPago { get; set; }
        public virtual PeriodoPago PeriodoPago { get; set; }
        public virtual TipoContrato ContratoInicial { get; set; }
        public virtual TiempoContrato TiempoContrato { get; set; }
        public virtual JornadaLaboral JornadaLaboral { get; set; }
        public virtual TipoModalidad TipoModalidad { get; set; }

        public virtual ICollection<EscolaridadesPerfil> escolardadesPerfil { get; set; }
        public virtual ICollection<AptitudesPerfil> aptitudesPerfil { get; set; }
        public virtual ICollection<HorarioPerfil> horariosPerfil { get; set; }
        public virtual ICollection<ActividadesPerfil> actividadesPerfil { get; set; }
        public virtual ICollection<ObservacionesPerfil> observacionesPerfil { get; set; }
        public virtual ICollection<PsicometriasDamsa> psicometriasDamsa { get; set; }
        public virtual ICollection<PsicometriasCliente> psicometriasCliente { get; set; }
        public virtual ICollection<BeneficiosPerfil> beneficiosPerfil { get; set; }
        public virtual ICollection<DocumentosCliente> documentosCliente { get; set; }
        public virtual ICollection<ProcesoPerfil> procesoPerfil { get; set; }
        public virtual ICollection<PrestacionesClientePerfil> prestacionesCliente { get; set; }
        public virtual ICollection<CompetenciaAreaPerfil> competenciasAreaPerfil { get; set; }
        public virtual ICollection<CompetenciaCardinalPerfil> competenciasCardinalPerfil { get; set; }
        public virtual ICollection<CompetenciaGerencialPerfil> competetenciasGerencialPerfil { get; set; }

        public DAMFO_290()
        {
            this.Id = Guid.NewGuid();
        }

    }
}
