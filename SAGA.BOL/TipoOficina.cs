using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class TipoOficina
    {  
        [Key]
        public int Id { get; set; }
        public string tipoOficina { get; set; }
        public string Icono { get; set; }
    }
}