using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Grupos : Persona
    {
        [Key]
        public string Grupo { get; set; }
        public bool Activo { get; set; }

        public virtual ICollection<Usuarios> Usuarios { get; set; }
    }
}
