using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Pais
    {
        [key]
        public int Id { get; set; }
        public string pais { get; set; }
    }
}