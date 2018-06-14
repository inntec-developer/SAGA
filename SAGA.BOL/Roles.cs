using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Roles
    {
        [Key]
        public int Id { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
    }
}
