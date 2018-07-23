﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class TrazabilidadMes
    {
        [Key]
        public Guid Id { get; set; }
        public int TipoMovimientoId { get; set; }
        public Int64 Folio { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioAlta { get; set; }
        public Guid UsuarioId { get; set; }

        public TipoMovimiento TipoMovimiento { get; set; }
        public Usuarios Usuario { get; set; }
    }
}
