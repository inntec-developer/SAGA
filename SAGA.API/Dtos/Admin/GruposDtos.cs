using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class GruposDtos
    {
        public Guid Id { get; set; }
        public bool Activo { get; set; }
        public string Foto { get; set; }
        public string Descripcion { get; set; }
        public string UsuarioAlta { get; set; }
        public string Nombre { get; set;  }
        public String FotoAux { get; set; }
        public byte TipoGrupoId { get; set; }
        public string TipoGrupo { get; set; }
        public ICollection<Grupos> Grupos { get; set; }

    }
}