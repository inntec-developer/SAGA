using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class CompetenciaCardinalRequi
    {
        [Key]
        public Guid Id { get; set; }
        public int CompetenciaId { get; set; }
        public string Nivel { get; set; }
        public Guid RequisicionId { get; set; }

        public virtual Requisicion Requisicion { get; set; }
        public virtual CompetenciaCardinal Competencia { get; set; }

        public CompetenciaCardinalRequi()
        {
            this.Id = Guid.NewGuid();
        }
    }
}