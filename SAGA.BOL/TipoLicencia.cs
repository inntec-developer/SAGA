
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class TipoLicencia
    {
        [Key]
        public byte Id { get; set; }
        public string tipoLicencia { get; set; }
        public string Descripcion { get; set; }
    }
}