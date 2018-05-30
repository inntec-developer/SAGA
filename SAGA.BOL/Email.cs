using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Email
    {
        public Guid Id { get; set; }  
        public string email { get; set; }
        public bool esPrincipal { get; set; }
        public Guid EntidadId { get; set; }
        public virtual Entidad Entidad{ get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public Email(string correo, Guid idPersona)
        {
            email = correo;
            EntidadId = idPersona;
            this.Id = Guid.NewGuid();

        }

        public Email()
        {
            this.Id = Guid.NewGuid();
        }
    }
}