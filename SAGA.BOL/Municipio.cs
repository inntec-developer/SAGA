using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Municipio
    {
        [Key]
        public int Id { get; set; }
        public string municipio { get; set; }
        public int EstadoId { get; set; }
        public Estado Estado { get; set; }
    }
}