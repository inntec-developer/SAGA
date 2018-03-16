using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Genero
    {
        [Key]
        public byte Id { get; set; }
        public string genero { get; set; }
    }
}