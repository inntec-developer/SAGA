using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class OficioRequisicion
    {
        public Guid Id { get; set; }
        public string Oficio { get; set; }
        public Guid RequisicionId { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string Comentario { get; set; }
    }
}
