﻿using System;
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
        //public string Foto { get; set; }
        public string Clave { get; set; }
        public Byte TipoUsuarioId { get; set; }
        public string Tipo { get; set; }
        public string Sucursal { get; set; }
        public Guid LiderId { get; set; }
        public string Lider { get; set; }
        public Guid DepartamentoId { get; set; }
        public string Departamento { get; set; }
        public List<PrivilegiosDtos> Privilegios { get; set; }
        public bool Activo { get; set; }
        public int UnidadNegocioId { get; set; }
        public List<string> Roles { get; set; }
    }
}