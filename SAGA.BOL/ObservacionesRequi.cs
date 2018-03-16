using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class ObservacionesRequi
    {
        [Key]
        public Guid Id { get; set; }
        public string Observaciones { get; set; }
        public Guid RequisicionId { get; set; }

        public virtual Requisicion Requisicion { get; set; }

        public ObservacionesRequi()
        {
            this.Id = Guid.NewGuid();
        }
    }
}