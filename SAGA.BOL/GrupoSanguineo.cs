using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class GrupoSanguineo
    {
        [Key]
        public int Id { get; set; }
        public string Grupo { get; set; }
        public bool Activo { get; set; }
    }
}
