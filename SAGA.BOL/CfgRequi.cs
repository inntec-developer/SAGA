using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class CfgRequi
    {
        [Key]
        public int Id { get; set; }
        public int ConfigMovId { get; set; }
        public int R_D { get; set; }
        public bool R { get; set; }
        public bool D { get; set; }

        public virtual ConfiguracionMovs ConfigMov { get; set; }
    }
}
