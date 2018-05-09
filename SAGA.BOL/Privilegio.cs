using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Privilegio
    {
        [key]
        public int Id { get; set; }
        public Guid IdGrupo_IdUsuario { get; set; }
        public int RolId { get; set; }
        public int Tipo { get; set; }

        public virtual Roles Rol { get; set; }
    }
}
