using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAGA.BOL
{
    public class PsicometriasCliente
    {
        [Key]
        public Guid Id { get; set; }
        public string Psicometria { get; set; }
        public string Descripcion { get; set; }
        public Guid DAMFO290Id { get; set; }

        public virtual DAMFO_290 DAMFO290 { get; set; }

        public PsicometriasCliente()
        {
            this.Id = Guid.NewGuid();
        }
    }
}