using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class GradoEstudio
    {
        [Key]
        public int Id { get; set; }
        public string gradoEstudio { get; set; }
    }
}