using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class ProcesoDto
    {
        public Guid candidatoId { get; set; }
        public int estatusId { get; set; }
        public Guid requisicionId { get; set; }
        public string vacante { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public Guid horarioId { get; set; }
        public string horario { get; set; }
        public int tipoMediosId { get; set; }
        public Guid departamentoId { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string nombreCandidato { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public string curp { get; set; }
        public string rfc { get; set; }
        public string nss { get; set; }
        public int paisNacimientoId { get; set; }
        public int estadoNacimientoId { get; set; }
        public int municipioNacimientoId { get; set; }
        public byte generoId { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid ReclutadorId { get; set; }


    }
}