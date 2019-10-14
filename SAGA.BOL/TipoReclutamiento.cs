using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public partial class TipoReclutamiento
    {
        public TipoReclutamiento(){}
        [Key]
        public int Id { get; set; }
        public string tipoReclutamiento { get; set; }
        public bool Activo { get; set; }
    }
}