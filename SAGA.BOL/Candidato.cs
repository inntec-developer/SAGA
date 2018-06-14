using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Candidato : Entidad
    {

        public int PaisNacimientoId { get; set; }
        public virtual Pais paisNacimiento { get; set; }


        public int? EstadoNacimientoId { get; set; }
        public virtual Estado estadoNacimiento { get; set; }

        public int? MunicipioNacimientoId { get; set; }
        public virtual Municipio municipioNacimiento { get; set; }

        public string CodigoPostal { get; set; }

        public byte GeneroId { get; set; }
        public virtual Genero Genero { get; set; }


        public byte? EstadoCivilId { get; set; }
        public virtual EstadoCivil EstadoCivil { get; set; }

        public bool esDiscapacitado { get; set; }
        public int? TipoDiscapacidadId { get; set; }
        public virtual TipoDiscapacidad TipoDiscapacidad { get; set; }

        public string OtraDiscapacidad { get; set; }

        public bool tieneLicenciaConducir { get; set; }
        public byte? TipoLicenciaId { get; set; }
        public virtual TipoLicencia TipoLicencia { get; set; }

        public bool tieneVehiculoPropio { get; set; }

        public bool puedeViajar { get; set; }
        public bool puedeRehubicarse { get; set; }

        public string CURP { get; set; }
        public string RFC { get; set; }
        [Display(Name ="Número de Seguro Social")]
        public string NSS { get; set; }

        public string ImgProfileUrl { get; set; }
        public DateTime fch_Creacion { get; set; }

        public Candidato()
        {
            direcciones = new List<Direccion>();
            telefonos = new List<Telefono>();
            emails = new List<Email>();
        }

    }

}