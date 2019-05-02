using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class PeriodoPago
    {
        [Key]
        public int Id { get; set; }
        public string periodoPago { get; set; }
        public bool activo { get; set; }
    }
}
