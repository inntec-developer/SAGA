using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class TiempoContrato
    {
        [Key]
        public int Id { get; set; }
        public string Tiempo { get; set; }
        public int Orden { get; set; }
    }
}
