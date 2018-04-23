using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAGA.BOL
{
    public class CompetenciaCardinalPerfil
    {
        [Key]
        public Guid Id { get; set; }
        public int CompetenciaId { get; set; }
        public string Nivel { get; set; }
        public Guid DAMFO290Id { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public virtual DAMFO_290 DAMFO290 { get; set; }
        public virtual CompetenciaCardinal Competencia { get; set; }

        public CompetenciaCardinalPerfil()
        {
            this.Id = Guid.NewGuid();
        }
    }
}