using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class PsicometriasDamsaRequi
    {
        [Key]
        public Guid Id { get; set; }
        public int PsicometriaId { get; set; }
        public Guid RequisicionId { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public virtual Requisicion Requisicion { get; set; }
        public virtual TipoPsicometria Psicometria { get; set; }

        public PsicometriasDamsaRequi()
        {
            this.Id = Guid.NewGuid();
        }
    }
}