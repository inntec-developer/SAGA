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
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public virtual Direccion Direccion { get; set; }

        public RutasPerfil()
        {
            this.Id = Guid.NewGuid();
        }
    }
}