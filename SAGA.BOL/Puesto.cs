using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Puesto
    {
        [Key]
        public int  Id { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public int CoordinacionId { get; set; }
        public bool Activo { get; set; }
        public bool BTRA { get; set; }
        public bool ERP { get; set; }

        public virtual ClaseReclutamiento Coordinacion { get; set; }

    }
}
