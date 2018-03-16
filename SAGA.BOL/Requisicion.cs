using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Requisicion
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public int TipoReclutamientoId { get; set; }
        public int ClaseReclutamientoId { get; set; }
        public string VBtra { get; set; }
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
        public DateTime? fch_Creacion { get; set; }
        public DateTime? fch_Aprobacion { get; set; }
        public DateTime? fch_Cumplimiento { get; set; }
        public DateTime? fch_Modificacion { get; set; }
        public Guid PropietarioId { get; set; }
        public Guid AprobadorId { get; set; }
        public Guid UsuarioMod { get; set; }
        public int PrioridadId { get; set; }
        public Boolean Aprobada { get; set; }
        public Boolean Confidencial { get; set; }
        public Boolean Asignada { get; set; }
        public int EstatusId { get; set; }
        public Guid DAMFO290Id { get; set; }
        public Guid DireccionId { get; set; }
        public bool FlexibilidadHorario { get; set; }
        public int JornadaLaboralId { get; set; }
        public int TipoModalidadId { get; set; }
        public bool Activo { get; set; }


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
        public virtual TipoContrato ContratoFinal { get; set; }
        public virtual TiempoContrato TiempoContrato { get; set; }
        public virtual JornadaLaboral JornadaLaboral { get; set; }
        public virtual TipoModalidad TipoModalidad { get; set; }
        // Extras para tabla de requisicion.
        public virtual Usuarios Propietario { get; set; }
        public virtual Usuarios Aprobador { get; set; }
        public virtual Prioridad Prioridad { get; set; }
        public virtual Estatus Estatus { get; set; }
        public virtual DAMFO_290 DAMFO290 { get; set; }
        public virtual Direccion Direccion { get; set; }
        

        public virtual ICollection<EscolaridadesRequi> escolaridadesRequi { get; set; }
        public virtual ICollection<AptitudesRequi> aptitudesRequi { get; set; }
        public virtual ICollection<ActividadesRequi> actividadesRequi { get; set; }
        public virtual ICollection<ObservacionesRequi> observacionesRequi { get; set; }
        public virtual ICollection<PsicometriasDamsaRequi> psicometriasDamsaRequi { get; set; }
        public virtual ICollection<PsicometriasClienteRequi> psicometriasClienteRequi { get; set; }
        public virtual ICollection<BeneficiosRequi> beneficiosRequi { get; set; }
        public virtual ICollection<DocumentosClienteRequi> documentosClienteRequi { get; set; }
        public virtual ICollection<ProcesoRequi> procesoRequi { get; set; }
        public virtual ICollection<PrestacionesClienteRequi> prestacionesClienteRequi { get; set; }
        public virtual ICollection<CompetenciaAreaRequi> competenciasAreaRequi { get; set; }
        public virtual ICollection<CompetenciaCardinalRequi> competenciasCardinalRequi { get; set; }
        public virtual ICollection<CompetenciaGerencialRequi> competetenciasGerencialRequi { get; set; }
        public virtual ICollection<AsignacionRequi> AsignacionRequi { get; set; }


        public Requisicion()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
