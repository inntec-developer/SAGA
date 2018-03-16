using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class CompetenciaArea
    {
        [Key]
        public int Id { get; set; }
        public string competenciaArea { get; set; }
    }
}
