using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FIRM_Compromiso
    {
        [Key]
        public Guid Id { get; set; }
        public string Actividad { get; set; }
        public string Responsable { get; set; }
        public DateTime Fecha { get; set; }
        public Guid Damfo022Id { get; set; }
        public FIRM_Damfo022 Damfo022 { get; set; }
    }
}
