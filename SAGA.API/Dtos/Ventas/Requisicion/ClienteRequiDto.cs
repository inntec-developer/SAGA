using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class ClienteRequiDto
    {
        public string Nombrecomercial { get; set; }
        public GiroEmpresa GiroEmpresas { get; set; }
        public ActividadEmpresa ActividadEmpresas { get; set; }
        public string RFC { get; set; }
    }

    public class ClienteCoincidenciaDto
    {
        public string Cliente { get; set; }
    }
}