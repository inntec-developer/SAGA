using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class EstadoEstudio
    {
        [Key]
        public int Id { get; set; }
        public string estadoEstudio { get; set; }
    }
}