using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class LogsIngresos
    {
        public Guid Id { get; set; }
        public Guid ASPId { get; set; }
        public Guid EntidadId { get; set; }
        public Int32 EstructuraId { get; set; }
        public DateTime fch_Ingreso { get; set; }
    }
}
