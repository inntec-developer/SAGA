using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Roles
    {
        [Key]
        public int Id { get; set; }
        public string Rol { get; set; }
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool Activo { get; set; }

        public virtual ICollection<Usuarios> Usuarios { get; set; }
    }
}
