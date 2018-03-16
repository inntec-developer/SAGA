using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Email
    {
        public Guid Id { get; set; }  
        public string email { get; set; }
        public bool esPrincipal { get; set; }
        public Guid PersonaId { get; set; }
        public virtual Persona Persona{ get; set; }

        public Email(string correo, Guid idPersona)
        {
            email = correo;
            PersonaId = idPersona;
            this.Id = Guid.NewGuid();

        }

        public Email()
        {
            this.Id = Guid.NewGuid();
        }
    }
}