﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class Sucursales
    {
        [Key]
        public int Id { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public string Comentario { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }

        public Guid EmpresasId { get; set; }
        public Empresas Empresas { get; set; }

        public int RegistroPatronalId { get; set; }
        public RegistroPatronal RegistroPatronal { get; set; }
    }
}
