using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public abstract class Entidad
    {
        [Key]
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Foto { get; set; }
        public int TipoEntidadId { get; set; }

        public TipoEntidad TipoEntidad { get; set; }

        public virtual ICollection<Direccion> direcciones { get; set; }
        public virtual ICollection<Telefono> telefonos { get; set; }
        public virtual ICollection<Email> emails { get; set; }
        public Entidad()
        {
            this.Id = Guid.NewGuid();
        }
        

    }
}