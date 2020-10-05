using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FIRM_SoporteSucursal
    {
        [Key]
        public int Id { get; set; }
        public int SucursalesId { get; set; }
        public Sucursales Sucursales { get; set; }
        public int SoportesNominaId { get; set; }
        public FIRM_SoportesNomina SoportesNomina { get; set; }
    }
}
