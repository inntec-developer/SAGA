using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public abstract class Persona
    {
        [Key]
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }
        public virtual ICollection<Direccion> direcciones { get; set; }
        public virtual ICollection<Telefono> telefonos { get; set; }
        public virtual ICollection<Email> emails { get; set; }
        public Persona()
        {
            this.Id = Guid.NewGuid();
        }
        

    }
}