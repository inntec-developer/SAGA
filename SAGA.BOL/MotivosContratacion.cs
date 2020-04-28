using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class MotivosContratacion
    {
        [Key]
        public int Id { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
