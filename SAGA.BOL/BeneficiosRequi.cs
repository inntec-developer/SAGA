using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class BeneficiosRequi
    {
        [Key]
        public Guid Id { get; set; }
        public int TipoBeneficioId { get; set; }
        public float Cantidad { get; set; }
        public string Observaciones { get; set; }
        public Guid RequisicionId { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public virtual Requisicion Requisicion { get; set; }
        public virtual TipoBeneficio TipoBeneficio { get; set; }

        public BeneficiosRequi()
        {
            this.Id = Guid.NewGuid();
        }
    }
}