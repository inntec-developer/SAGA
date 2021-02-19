using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class ConfigVacaciones
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int DiasExpiran { get; set; }
        public byte DiasContinuos { get; set; }
        public byte DiasIncremento { get; set; }
        public decimal Porcentaje { get; set; }
        public string Observaciones { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }

        public Guid ClienteId { get; set; }
        public Cliente Cliente { get; set; }

    }

    //    Artículo 76.- Los trabajadores que tengan más de un año de servicios disfrutarán de un período 
    //anual de vacaciones pagadas, que en ningún caso podrá ser inferior a seis días laborables, y 
    //que aumentará en dos días laborables, hasta llegar a doce, por cada año subsecuente de servicios.
//Después del cuarto año, el período de vacaciones aumentará en dos días por cada cinco de servicios.

}
