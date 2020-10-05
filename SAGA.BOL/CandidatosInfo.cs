using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class CandidatosInfo
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }

        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string CURP { get; set; }
        public string RFC { get; set; }
        [Display(Name = "Número de Seguro Social")]
        public string NSS { get; set; }

        public int? PaisNacimientoId { get; set; }
        public virtual Pais paisNacimiento { get; set; }

        public int? EstadoNacimientoId { get; set; }
        public virtual Estado estadoNacimiento { get; set; }

        public int? MunicipioNacimientoId { get; set; }
        public virtual Municipio municipioNacimiento { get; set; }

        public byte GeneroId { get; set; }
        public virtual Genero Genero { get; set; }

        public Guid ReclutadorId { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioMod { get; set; }

    }
}
