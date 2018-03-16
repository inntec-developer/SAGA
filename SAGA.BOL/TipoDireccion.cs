using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public partial class TipoDireccion
    {
        public TipoDireccion(){}
        [Key]
        public int Id { get; set; }
        public string tipoDireccion { get; set; }

    }
}