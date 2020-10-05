using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Equipos
{
    public class RequisicionesDto
    {
        public Guid clienteId { get; set; }
        public string cliente { get; set; }
        public string razon { get; set; }
        public Guid requisicionId { get; set; }
        public DateTime? fch_Modificacion { get; set; }
        public int estatusId { get; set; }
        public string estatus { get; set; }
        public Int64 folio { get; set; }
        public int cubiertas { get; set; }
        public int posiciones { get; set; }
    }
}