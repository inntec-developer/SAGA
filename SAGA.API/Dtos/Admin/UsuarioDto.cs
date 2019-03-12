using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class UsuarioDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Usuario { get; set; }
        public string Foto { get; set; }
        public string Clave { get; set; }
        public int TipoUsuarioId { get; set; }
        public string Tipo { get; set; }
        public string Sucursal { get; set; }
        public List<PrivilegiosDtos> Privilegios { get; set; }
    }
}