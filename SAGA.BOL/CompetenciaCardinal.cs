using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class CompetenciaCardinal
    {
        [Key]
        public int Id { get; set; }
        public string competenciaCardinal { get; set; }
    }
}
