using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class RolEntidad
    {
        [Key]
        public int Id { get; set; }
        public Guid EntidadId { get; set; }
        public int RolId { get; set; }

        public virtual Roles Rol { get; set; }
        public virtual Entidad Entidad { get; set; }
    }
}
