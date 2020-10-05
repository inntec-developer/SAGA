using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Grupos
    {
        [Key]
        public int Id { get; set; }
        public string Clave { get; set; }
        public bool Activo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public Guid UsuarioMod { get; set; }
        public DateTime fch_Modificacion { get; set; }
    }
}
