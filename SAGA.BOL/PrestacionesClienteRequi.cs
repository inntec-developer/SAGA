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

        public virtual Requisicion Requisicion { get; set; }

        public PrestacionesClienteRequi()
        {
            this.Id = Guid.NewGuid();
        }
    }
}