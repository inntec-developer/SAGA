using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class SoporteFacturacion
    {
        [Key]
        public int Id { get; set; }
        public string Clave { get; set; }
        public string Concepto { get; set; }
        public string NombreHoja { get; set; }
        public bool ServicioNomina { get; set; }
        public decimal MontoTope { get; set; }
        public string Observaciones { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; } 

        public Guid DepartamentoId { get; set; }
        public Departamento Departemento { get; set; }
        public int TipodeNominaId { get; set; }
        public TipodeNomina TipodeNomina { get; set; }
    }
}
