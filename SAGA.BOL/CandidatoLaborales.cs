using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class CandidatoLaborales
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatoInfoId { get; set; }
        public virtual CandidatosInfo CandidatoInfo { get; set; }

        public DateTime FechaIngreso { get; set; }
        public string ClaveTurno { get; set; }
        public string ClaveSucursal { get; set; }
        public string NoCuenta { get; set; }
        public string Departamento { get; set; }
        public DateTime FechaFormaPago { get; set; } // cuando se entregó tarjeta
        public string Puesto { get; set; }
        public string ClaveJefe { get; set; }
        public string ClaveExt { get; set; }
        public string SoporteFacturacion { get; set; }
        public decimal SueldoMensual { get; set; }
        public decimal SueldoDiario { get; set; }
        public decimal SueldoIntegrado { get; set; }

        public int BancoId { get; set; }
        public virtual CatalogoBancos Banco { get; set; }

        public int MotivoId { get; set; }
        public virtual MotivosContratacion Motivo { get; set; }

        public int FormaPagoId { get; set; }
        public virtual FormaPago FormaPago { get; set; }

    }
}
