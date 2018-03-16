using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class DocumentosClienteRequi
    {
        [Key]
        public Guid Id { get; set; }
        public string Documento { get; set; }
        public Guid RequisicionId { get; set; }
        public virtual Requisicion Requisicion { get; set; }

        public DocumentosClienteRequi()
        {
            this.Id = Guid.NewGuid();
        }
    }
}