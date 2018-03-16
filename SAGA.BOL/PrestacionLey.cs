using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class PrestacionLey
    {
        [Key]
        public int Id { get; set; }
        public string  prestacionLey { get; set; }
    }
}
