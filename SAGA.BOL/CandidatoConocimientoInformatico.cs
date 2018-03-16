using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class CandidatoConocimientoInformatico
    {
        public Guid Id { get; set; }
        [Display(Name ="Conocimiento Informático")]
        public Guid ConocimientoInformaticoId { get; set; }
        public ConocimientoInformatico ConocimientoInformatico { get; set; }
        [Display(Name ="Nivel")]
        public int PorcentageId { get; set; }
        public Porcentage Porcentage { get; set; }

        public Guid CandidatoId { get; set; }

        public CandidatoConocimientoInformatico()
        {
            this.Id = Guid.NewGuid();
        }
    }
    
}
