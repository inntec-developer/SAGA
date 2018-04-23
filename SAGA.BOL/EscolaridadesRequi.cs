using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class EscolaridadesRequi
    {
        [Key]
        public Guid Id { get; set; }
        public int EscolaridadId { get; set; }
        public int EstadoEstudioId { get; set; }
        public Guid RequisicionId { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public Requisicion Requisicion { get; set; }
        public virtual GradoEstudio Escolaridad { get; set; }
        public virtual EstadoEstudio EstadoEstudio { get; set; }

        public EscolaridadesRequi()
        {
            this.Id = Guid.NewGuid();
        }

    }
}
