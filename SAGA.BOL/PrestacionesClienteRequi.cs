using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class PrestacionesClienteRequi
    {
        [Key]
        public Guid Id { get; set; }
        public string Prestamo { get; set; }
        public Guid RequisicionId { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public virtual Requisicion Requisicion { get; set; }

        public PrestacionesClienteRequi()
        {
            this.Id = Guid.NewGuid();
        }
    }
}