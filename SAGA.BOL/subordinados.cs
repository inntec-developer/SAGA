using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Subordinados
    {
        [Key]
        public Guid Id { get; set; }
        public Guid LiderId { get; set; }
        public Guid UsuarioId { get; set; }

        public Usuarios Usuario { get; set; }
        public Usuarios Lider { get; set; }
    }
}
