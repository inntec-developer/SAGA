using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class RolesDto
    {
        [Key]
        public int Id { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
    }
}