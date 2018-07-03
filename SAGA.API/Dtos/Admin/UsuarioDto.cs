using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class UsuarioDto
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Usuario { get; set; }
        public List<PrivilegiosDtos> Privilegios { get; set; }
    }
}