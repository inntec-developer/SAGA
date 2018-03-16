using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAGA.BOL
{
    public class BeneficiosPerfil
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public int TipoBeneficioId { get; set; }
        public float Cantidad { get; set; }
        public string Observaciones { get; set; }
        public Guid DAMFO290Id { get; set; }

        public virtual DAMFO_290 DAMFO290 { get; set; }
        public virtual TipoBeneficio TipoBeneficio { get; set; }

        public BeneficiosPerfil()
        {
            this.Id = Guid.NewGuid();
        }
    }
}