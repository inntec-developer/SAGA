using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAGA.BOL
{
    public class PsicometriasDamsa
    {
        [Key]
        public Guid Id { get; set; }
        public int PsicometriaId { get; set; }
        public Guid DAMFO290Id { get; set; }

        public virtual DAMFO_290 DAMFO290 { get; set; }
        public virtual TipoPsicometria Psicometria { get; set; }

        public PsicometriasDamsa()
        {
            this.Id = Guid.NewGuid();
        }
    }
}