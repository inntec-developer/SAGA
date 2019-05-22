using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class TiposTransferencias
    {
        [Key]
        public int Id { get; set; }
        public string TipoTransf { get; set; }
        public byte Activo { get; set; }
    }
}
