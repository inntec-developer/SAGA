using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class CatalogosDto
    {
        public int opt { get; set; }
        #region Sistema
        public Catalogos Catalogos { get; set; }
        public List<Pais> Pais { get; set; }
        public List<EstadoDto> Estado { get; set; }
        public List<MunicipioDto> Municipio { get; set; }
        public List<ColoniasDto> Colonia { get; set; }
        public List<TpTelefonosDto> TpTelefono { get; set; }
        public List<TpUsuarioDto> TpUsuario { get; set; }
        public List<EstadoCivilDto> EstadoCivil { get; set; }
        public List<DepartamentosDto> Departamentos { get; set; }
        public List<AreaDto> Areas { get; set; }
        public List<EscolaridadesDto> Escolaridades { get; set; }
        public List<NivelDto> Nivel { get; set; }
        public List<MedioDto> Medio { get; set; }
        public List<IdiomaDto> Idioma { get; set; }
        #endregion
        #region Reclutamiento
        #endregion
        #region Ventas
        #endregion
    }
}