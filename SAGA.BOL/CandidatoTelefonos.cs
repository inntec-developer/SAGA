using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    class CandidatoTelefonos
    {
        [Key]
        public Guid Id { get; set; }
        public string ClavePais { get; set; }
        public String ClaveLada { get; set; }
        public String Extension { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }

        public byte TipoTelefonoId { get; set; }
        public virtual TipoTelefono TipoTelefono { get; set; }
    }
}
