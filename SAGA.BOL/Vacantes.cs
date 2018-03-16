using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Vacante
    {

        public Guid Id { get; set; }
        public string VBtra { get; set; }
        public int EscolaridadId { get; set; }
        public string Experiencia { get; set; }
        public Guid DireccionId { get; set; }
        public int CategoriaId { get; set; }
        public Guid ClienteId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public byte NivelId { get; set; }
        public Guid HorarioId { get; set; }
        public string Actividades { get; set; }
        public int TipoContratoId { get; set; }

        public Guid DAMFO290Id { get; set; }

        public EstadoEstudio Escolaridad { get; set; }
        public Direccion Direccion { get; set; }
        public Area Categoria { get; set; }
        public Cliente Cliente { get; set; }
        public Nivel Nivel { get; set; }
        public HorarioPerfil Horario { get; set; }
        public TipoContrato Contrato { get; set; }

        public ICollection<AptitudesPerfil> Aptitudes { get; set; }
        public ICollection<BeneficiosPerfil> Beneficios { get; set; }
        public ICollection<PrestacionesClientePerfil> PrestacionesCliente { get; set; }



    }
}
