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
        public DateTime FechaIngreso { get; set; }
        public string ClaveTurno { get; set; }
        public string NoCuenta { get; set; }
        public DateTime FechaFormaPago { get; set; } // cuando se entregó tarjeta
        public string ClaveJefe { get; set; }
        public string ClaveExt { get; set; }
        public decimal SueldoMensual { get; set; }
        public decimal SueldoDiario { get; set; }
        public decimal SueldoIntegrado { get; set; }

        public Guid CandidatoInfoId { get; set; }
        public virtual CandidatosInfo CandidatoInfo { get; set; }

        public int SucursalId { get; set; }
        public virtual Sucursales Sucursal { get; set; }

        public Guid DepartamentoId { get; set; }
        public virtual Departamento Departamento { get; set; }

        public int PuestoId { get; set; }
        public virtual Puesto Puesto { get; set; }

        public int SoporteFacturacionId { get; set; }
        public virtual SoporteFacturacion SoporteFacturacion { get; set; }

        public int BancoId { get; set; }
        public virtual CatalogoBancos Banco { get; set; }

        public int MotivoId { get; set; }
        public virtual MotivosContratacion Motivo { get; set; }

        public int FormaPagoId { get; set; }
        public virtual FormaPago FormaPago { get; set; }

    }
}
