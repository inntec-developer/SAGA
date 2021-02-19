using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class InfoGeneralDto
    {
        public Guid Id { get; set; }
        public string RazonSocial { get; set; }
        public string Clave { get; set; }
        public string NombreComercial { get; set; }
        public string RFC { get; set; }
        public int GiroEmpresa { get; set; }
        public int ActividadEmpresa { get; set; }
        public int TamanoEmpresa { get; set; }
        public int TipoEmpresa { get; set; }
        public int TipoBase{ get; set; }
        public string Clasificacion { get; set; }
        public int NumeroEmpleados { get; set; }
        public string Usuario { get; set; }
    }
}