using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.BOL;

namespace SAGA.API.Dtos
{
    public class FiltrosDto
    {
        public Guid IdCandidato { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public DateTime? fechaNacimiento { get; set; }
        public string curp { get; set; }
        public string rfc { get; set; }
        public string nss { get; set; }
        public int IdPais { get; set; }
        public int? IdEstado { get; set; }
        public int? IdMunicipio { get; set; }
        public int? IdColonia { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        public string cp { get; set; }
        public int? IdAreaExp { get; set; }
        public int? IdPerfil { get; set; }
        public int? IdGenero { get; set; }
        public int? IdPDiscapacidad { get; set; }
        public int? IdTipoLicencia { get; set; }
        public int? IdNvEstudios { get; set; }
        public int? IdIdiomas { get; set; }
        public decimal? Salario { get; set; }
        public int? Edad { get; set; }
        public bool Reubicacion { get; set; }
        public bool TpVehiculo { get; set; }
        public string AreaExp { get; set; }
        public string AreaInt { get; set; }
        public string Estatus { get; set; }
        public virtual List<int> Formaciones { get; set; }
        public virtual ICollection<PerfilIdioma> Idiomas { get; set; }
        public DateTime? fch_Creacion { get; set; }
    }
}