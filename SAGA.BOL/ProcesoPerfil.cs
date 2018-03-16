using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAGA.BOL
{
    public class ProcesoPerfil
    {
        [Key]
        public Guid Id { get; set; }
        public string Proceso { get; set; }
        public Guid DAMFO290Id { get; set; }
        public int Orden { get; set; }

        public virtual DAMFO_290 DAMFO290 { get; set; }

        public ProcesoPerfil()
        {
            this.Id = Guid.NewGuid();
        }
    }
}