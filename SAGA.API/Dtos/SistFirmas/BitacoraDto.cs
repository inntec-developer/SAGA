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
        public FIRM_ConfigBitacora ConfigBitacora { get; set; }
        public List<FIRM_FechasEstatus> FechasEstatus { get; set; }
        public List<FIRM_EstatusEmails> EstatusEmails { get; set; }
        public List<XLSToJson> Configuracion { get; set; }
    }
    //public class diashoras
    //{
    //    public int dia { get; set; }
    //    public DateTime hora { get; set; }
    //}
    public class XLSToJson
    {
        public FIRM_ConfigBitacora Empresa { get; set; }
        public List<FIRM_FechasEstatus> FechasEstatus { get; set; }
        public List<FIRM_EstatusEmails> EstatusEmails { get; set; }

    }
}