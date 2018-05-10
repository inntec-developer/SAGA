using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Estatus
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public int TipoMovimiento { get; set; }
        public int  Orden { get; set; }
    }
}
