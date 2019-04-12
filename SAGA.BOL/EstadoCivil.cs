using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class EstadoCivil
    {
        [Key]
        public byte Id { get; set; }
        public string estadoCivil { get; set; }
        public bool Activo { get; set; }
    }
}