using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public partial class HorarioPerfil
    {
        public  HorarioPerfil()
        {
            this.Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public byte deDiaId { get; set; }
        public byte aDiaId { get; set; }
        public string deHora { get; set; }
        public string aHora { get; set; }
        public byte numeroVacantes { get; set; }
        public string Especificaciones { get; set; }
        public Guid DAMFO290Id { get; set; }
        public bool Activo { get; set; }

        public virtual DAMFO_290 DAMFO290 { get; set; }
        public virtual DiaSemana deDia { get; set; }
        public virtual DiaSemana aDia { get; set; }
    }
}
