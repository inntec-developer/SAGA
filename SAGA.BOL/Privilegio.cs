using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Privilegio
    {
        [Key]
        public int Id { get; set; }
        public int EstructuraId { get; set; }
        public int RolId { get; set; }
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool Especial { get; set; }

        public virtual Roles Rol { get; set; }
        public virtual Estructura Estructura { get; set; }
    }
}
