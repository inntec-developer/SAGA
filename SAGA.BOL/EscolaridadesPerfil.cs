using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class EscolaridadesPerfil
    {
        [Key]
        public Guid Id { get; set; }
        public int EscolaridadId { get; set; }
        public int EstadoEstudioId { get; set; }
        public Guid DAMFO290Id { get; set; }

        public DAMFO_290 DAMFO290 { get; set; }
        public virtual GradoEstudio Escolaridad { get; set; }
        public virtual EstadoEstudio EstadoEstudio { get; set; }

        public EscolaridadesPerfil()
        {
            this.Id = Guid.NewGuid();
        }
    }
}