using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class GrupoUsuarios
    {
        [key]
        public int Id { get; set; }
        public Guid GrupoId { get; set; }
        public Guid UsuarioId { get; set; }

        public virtual Usuarios Usuario { get; set; }
        public virtual Grupos Grupo { get; set; }
    }
}
