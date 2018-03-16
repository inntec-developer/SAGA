using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class TipoTelefono
    {
        [Key]
        public byte Id { get; set; }
        public string Tipo { get; set; } 
    }
}