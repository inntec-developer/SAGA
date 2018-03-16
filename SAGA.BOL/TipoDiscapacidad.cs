using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class TipoDiscapacidad
    {
        [Key]
        public int Id { get; set; }
        public string  tipoDiscapacidad { get; set; }
    }
}