using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FormaPago
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }

    }
}
