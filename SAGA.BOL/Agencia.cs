using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public partial class Agencia
    {
        public Agencia()
        {
            this.Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public string agencia { get; set; }
        public DateTime DesdeCuendo { get; set; }
        public decimal Empleado { get; set; }
        public decimal Cobro { get; set; }

        public virtual Cliente Cliente { get; set; }

    }
}
