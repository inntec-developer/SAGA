using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class DocumentosCliente
    {
        [Key]
        public Guid Id { get; set; }
        public string Documento { get; set; }
        public Guid DAMFO290Id { get; set; }

        public virtual DAMFO_290 DAMFO290 { get; set; }
        public DocumentosCliente()
        {
            this.Id = Guid.NewGuid();
        }
    }
}