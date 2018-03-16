using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Candidato : Persona
    {
     
        public int PaisNacimientoId { get; set; }
        public Pais paisNacimiento { get; set; }

       
        public int? EstadoNacimientoId { get; set; }
        public Estado estadoNacimiento { get; set; }

        public int? MunicipioNacimientoId { get; set; }
        public Municipio municipioNacimiento { get; set; }

        public string CodigoPostal { get; set; }

        public byte GeneroId { get; set; }
        public Genero Genero { get; set; }


        public byte? EstadoCivilId { get; set; }
        public EstadoCivil EstadoCivil { get; set; }

        public bool esDiscapacitado { get; set; }
        public int? TipoDiscapacidadId { get; set; }
        public TipoDiscapacidad TipoDiscapacidad { get; set; }

        public bool tieneLicenciaConducir { get; set; }
        public byte? TipoLicenciaId { get; set; }
        public TipoLicencia TipoLicencia { get; set; }

        public bool tieneVehiculoPropio { get; set; }

        public bool puedeViajar { get; set; }
        public bool puedeRehubicarse { get; set; }        
        
        public string CURP { get; set; }
        public string RFC { get; set; }
        [Display(Name ="Número de Seguro Social")]
        public string NSS { get; set; }
        
        public string ImgProfileUrl { get; set; }



        public Candidato()
        {
            direcciones = new List<Direccion>();
            telefonos = new List<Telefono>();
            emails = new List<Email>();
        }

    }

}