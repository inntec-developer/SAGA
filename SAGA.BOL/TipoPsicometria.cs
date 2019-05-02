using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class TipoPsicometria
    {
        public TipoPsicometria(){}

        [Key]
        public int Id { get; set; }
        public string tipoPsicometria { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
    }
}
