using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Pais
    {
        [Key]
        public int Id { get; set; }
        public string pais { get; set; }
    }
}