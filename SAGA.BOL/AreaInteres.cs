using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class AreaInteres
    {
        [Key]
        public int Id { get; set; }
        public string areaInteres { get; set; }
    }
}