using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class CompetenciaAreaRequi
    {
        [Key]
        public Guid Id { get; set; }
        public int CompetenciaId { get; set; }
        public string Nivel { get; set; }
        public Guid RequisicionId { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public virtual Requisicion Requisicion { get; set; }
        public virtual CompetenciaArea Competencia { get; set; }

        public CompetenciaAreaRequi()
        {
            this.Id = Guid.NewGuid();
        }
    }
}