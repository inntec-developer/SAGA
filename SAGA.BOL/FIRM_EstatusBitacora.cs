﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FIRM_EstatusBitacora
    {
        [Key]
        public int Id { get; set; }
        public string Estatus { get; set; }
        public string Observaciones { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }

        //public int ConfigBitacoraId { get; set; }
        //public FIRM_ConfigBitacora ConfigBitacora { get; set; }


    }
}
