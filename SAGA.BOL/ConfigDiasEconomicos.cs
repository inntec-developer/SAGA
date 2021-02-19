﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class ConfigDiasEconomicos
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Comentarios { get; set; }
        public byte DiasConSueldo { get; set; }
        public byte DiasSinSueldo { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }

        public Guid ClienteId { get; set; }
        public Cliente Cliente { get; set; }
    }
}
