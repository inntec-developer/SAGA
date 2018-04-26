using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Folio
    {
        [Key]
        public int Id { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public int Consecutivo { get; set; }
        public string TipoMovimiento { get; set; }
        public DateTime fch_Creacion { get; set; }

    }
}
