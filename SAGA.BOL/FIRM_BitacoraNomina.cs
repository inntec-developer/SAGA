using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FIRM_BitacoraNomina
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid PropietarioId { get; set; }
        public bool Retardo { get; set; }
        public bool Porques { get; set; }

        public int EstatusNominaId { get; set; }
        public FIRM_EstatusNomina EstatusNomina { get; set; }
    }
}
