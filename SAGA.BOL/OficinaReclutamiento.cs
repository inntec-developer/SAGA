using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class OficinaReclutamiento : Entidad
    {
        public int TipoOficinaId { get; set; }
        public string IndicacionEspecial { get; set; }
        public int  Orden { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public bool Activo { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }
        public int UnidadNegocioId { get; set; }

        public UnidadNegocio UnidadNegocio { get; set; }
        public TipoOficina TipoOficina { get; set; }
    }
}
