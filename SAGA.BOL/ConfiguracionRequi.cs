using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class ConfiguracionRequi
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        public Guid RequisicionId { get; set; }
        public int IdEstructura { get; set; }
        public string Campo { get; set; }
        public int R_D { get; set; }
        public bool Resumen { get; set; }
        public bool Detalle { get; set; }

        //public virtual Requisicion Requi { get; set; }
    }
}
