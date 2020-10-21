using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.SistFirmas
{
    public class BitacoraDto
    {
        public FIRM_Bitacora Bitacora { get; set; }
        public int EstatusBitacoraId { get; set; }
        public int ConfigBitacoraId { get; set; }
    }
}