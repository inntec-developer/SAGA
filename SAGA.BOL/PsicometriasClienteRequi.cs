using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class PsicometriasClienteRequi
    {
        [Key]
        public Guid Id { get; set; }
        public string Psicometria { get; set; }
        public string Descripcion { get; set; }
        public Guid RequisicionId { get; set; }

        public virtual Requisicion Requisicion { get; set; }

        public PsicometriasClienteRequi()
        {
            this.Id = Guid.NewGuid();
        }
    }
}