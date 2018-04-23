using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Usuarios : Persona
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
        public bool Activo { get; set; }
        public byte TipoUsuarioId { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public virtual TipoUsuario TipoUsuario { get; set; }

        public virtual ICollection<Roles> Roles { get; set; }
        public virtual ICollection<Grupos> Grupos { get; set; }

    }
}
