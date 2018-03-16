using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Area
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}