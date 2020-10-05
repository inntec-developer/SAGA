using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FIRM_ConfigBitacora
    {
        [Key]
        public int Id { get; set; }
        public int SucursalesId { get; set; }
        public Sucursales Sucursales { get; set; }
        public int SoportesNominaId { get; set; }
        public FIRM_SoportesNomina SoportesNomina { get; set; }
        public int TipodeNominaId { get; set; }
        public TipodeNomina TipodeNomina { get; set; }
        public Guid Destinatario { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }
    }
}
