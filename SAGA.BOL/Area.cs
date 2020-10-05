using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Area
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public string Observaciones { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioMod { get; set; }
    }
}