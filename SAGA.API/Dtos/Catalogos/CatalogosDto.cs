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
        #endregion
        #region Reclutamiento
        #endregion
        #region Ventas
        #endregion
    }
}