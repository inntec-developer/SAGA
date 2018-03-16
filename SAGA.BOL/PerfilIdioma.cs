using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAGA.BOL
{
    public class PerfilIdioma
    {
        public Guid Id { get; set; }
        public int IdiomaId { get; set; }
        public virtual Idioma Idioma { get; set; }
        public byte NivelEscritoId { get; set; }
        public virtual Nivel nivelEscrito { get; set; }
        public byte NivelHabladoId { get; set; }
        public virtual Nivel nivelHablado { get; set; }

        public Guid PerfilCandidatoId { get; set; }
        public PerfilCandidato PerfilCandidato { get; set; }

        public PerfilIdioma()
        {
            this.Id = Guid.NewGuid();
        }
    }
}