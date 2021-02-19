using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class DiasHorasIngresos
    {
        [Key]
        public Guid Id { get; set; }
        public byte Dia { get; set; }
        public DateTime DeHora { get; set; }
        public DateTime AHora { get; set; }
        public bool Activo { get; set; }
        public byte Tipo { get; set; } //laboral 1 comida 2 descanso 3
        public byte LimiteEntrada { get; set; }
        public byte LimiteComida1 { get; set; }
        public byte LimiteComida2 { get; set; }
        public Guid HorariosIngresosId { get; set; }

        public HorariosIngresos HorariosIngresos { get; set; }
    }
}
