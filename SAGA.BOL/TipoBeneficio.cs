using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class TipoBeneficio
    {
        [Key]
        public int Id { get; set; }
        public string tipoBeneficio { get; set; }
    }
}
