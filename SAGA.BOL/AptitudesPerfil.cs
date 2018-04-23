using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAGA.BOL
{
    public class AptitudesPerfil
    {
        [Key]
        public Guid Id { get; set; }
        public int AptitudId { get; set; }
        public Guid DAMFO290Id { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public DAMFO_290 DAMFO290 { get; set; }
        public virtual Aptitud Aptitud { get; set; }

        public AptitudesPerfil()
        {
            this.Id = Guid.NewGuid();
        }
    }
}