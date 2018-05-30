﻿using System;

namespace SAGA.BOL
{
    public class Referenciado : Entidad
    {
        public string Puesto { get; set; }
        public string Clave { get; set; }
        public Guid ClienteId { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }
        public virtual Cliente Cliente { get; set; }
    }
}