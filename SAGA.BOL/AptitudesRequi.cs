using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class AptitudesRequi
    {
        [Key]
        public Guid Id { get; set; }
        public int AptitudId { get; set; }
        public Guid RequisicionId { get; set; }

        public Requisicion Requisicion { get; set; }
        public virtual Aptitud Aptitud { get; set; }

        public AptitudesRequi()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
