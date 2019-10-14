using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public partial class ClaseReclutamiento
    {
        public ClaseReclutamiento(){}
        [Key]
        public int Id { get; set; }
        public string clasesReclutamiento { get; set; }
        public bool Activo { get; set; }
    }
}