using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class RequisicionDeleteDto
    {
        public Guid Id { get; set; }
        public string UsuarioMod { get; set; }
    }

    public class CancelRequiDto
    {
        public Int64 Folio { get; set; }
        public string VBtra { get; set; }
        public int EstatusId { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }
        public bool Publicado { get; set; }
    }
}