using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class RutasPerfil
    {
        [Key]
        public Guid Id { get; set; }
        public Guid DireccionId { get; set; }
        public string Ruta { get; set; }
        public string Via { get; set; }

        public virtual Direccion Direccion { get; set; }

        public RutasPerfil()
        {
            this.Id = Guid.NewGuid();
        }
    }
}