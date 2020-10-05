using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class TipodeNomina
    {
        [Key]
        public int Id { get; set; }
        public string Clave { get; set; }
        public string tipoDeNomina { get; set; }
        public string Descripcion { get; set; }
        public byte Tipo { get; set; } // 1 ventas 2 firmas
        public bool activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }
    }
}
