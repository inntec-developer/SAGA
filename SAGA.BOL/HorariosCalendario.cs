using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class HorariosCalendario
    {
        [Key]
        public int Id { get; set; }
        public DateTime Horario { get; set; }
        public string Descripcion { get; set; }
        public Boolean Activo { get; set; }
        public int Orden { get; set; }

    }
}
