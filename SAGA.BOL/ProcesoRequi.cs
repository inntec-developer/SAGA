using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class ProcesoRequi
    {
        [Key]
        public Guid Id { get; set; }
        public string Proceso { get; set; }
        public Guid RequisicionId { get; set; }

        public virtual Requisicion Requisicion { get; set; }

        public ProcesoRequi()
        {
            this.Id = Guid.NewGuid();
        }
    }
}