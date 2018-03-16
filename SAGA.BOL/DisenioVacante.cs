using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class DisenioVacante
    {
        public int Id { get; set; }
        public Guid RequisiconId { get; set; }
        public bool ClienteId { get; set; }
        public bool TipoReclutamientoId { get; set; }
        public bool ClaseReclutamientoId { get; set; }
        public bool NombrePerfil { get; set; }
        public bool GeneroId { get; set; }
        public bool EdadMinima { get; set; }
        public bool EdadMaxima { get; set; }
        public bool EstadoCivilId { get; set; }
        public bool NivelId { get; set; }
        public bool AreaId { get; set; }
        public bool Experiencia { get; set; }
        public bool SueldoMinimo { get; set; }
        public bool SueldoMaximo { get; set; }
        public bool DiaCorteId { get; set; }
        public bool TipoNominaId { get; set; }
        public bool DiaPagoId { get; set; }
        public bool PeriodoPagoId { get; set; }
        public bool Especifique { get; set; }
        public bool ContratoInicialId { get; set; }
        public bool ContratoFinalId { get; set; }

        // Colletions
        public bool EscolaridadesPerfil  { get; set; }
        public bool AptitudesPerfil  { get; set; }
        public bool HorarioPerfil  { get; set; }
        public bool ActividadesPerfil  { get; set; }
        public bool BeneficiosPerfil  { get; set; }
        public bool PrestacionesClientePerfil  { get; set; }
    }
}
