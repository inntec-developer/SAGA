using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class RequiExamen
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RequisicionId { get; set; }
        public int ExamenId { get; set; }

        public Requisicion Requisicion { get; set; }
        public Examenes Examen { get; set; }

    }
}
