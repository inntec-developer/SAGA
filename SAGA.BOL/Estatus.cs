using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Estatus
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
