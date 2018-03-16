using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class RedSocial
    {
        public RedSocial()
        {
            this.Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public byte TipoRedSocialId { get; set; }
        public string redSocial { get; set; }
        public Guid? PersonaId { get; set; }
        public virtual TipoRedSocial TipoRedSocial {get; set;}
        public virtual Persona Personas { get; set; }
    }
}
