using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class UnidadNegocioEstados
    {
        [Key]
        public int Id { get; set; }
        public int estadoId { get; set; }
        public int unidadnegocioId { get; set; }

        public Estado Estado { get; set; }
        public UnidadNegocio UnidadNegocio { get; set; }
        
    }
}
