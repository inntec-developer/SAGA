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
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public virtual DAMFO_290 DAMFO290 { get; set; }
        public DocumentosCliente()
        {
            this.Id = Guid.NewGuid();
        }
    }
}