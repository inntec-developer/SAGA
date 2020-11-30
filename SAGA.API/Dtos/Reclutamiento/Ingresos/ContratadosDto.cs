using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Reclutamiento.Ingresos
{
     public class ContratadosDto
    {
        public Guid id { get; set; }
        public Guid candidatoId { get; set; }
        public string nombre { get; set; }
        public string nombreCompleto { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string curp { get; set; }
        public string rfc { get; set; }
        public string nss { get; set; }
        public string Foto { get; set; }
        public DateTime fechaNacimiento { get; set; }

        public Guid requisicionId { get; set; }
        public string reclutador { get; set; }
        public int paisNacimiento { get; set; }
        public int estadoNacimiento { get; set; }
        public int municipioNacimiento { get; set; }
        public string localidad { get; set; }
        public int generoId { get; set; }
        public string genero { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public DateTime fch_Ingreso { get; set; }
        public string lada { get; set; }
        public string telefono { get; set; }
        public string direccion { get; set; }
        public string folio { get; set; }
        public string vbtra { get; set; }
        public Guid clienteId { get; set; }
        public string nombrecomercial { get; set; }
        public string razonSocial { get; set; }
    }
}