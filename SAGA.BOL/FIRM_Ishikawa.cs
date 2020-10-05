using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FIRM_Ishikawa
    {
        [Key]
        public byte Id { get; set; }
        public string Causa { get; set; }
        public bool Activo { get; set; }
    }
}
