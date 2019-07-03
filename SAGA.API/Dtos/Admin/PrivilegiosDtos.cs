using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.BOL;
using SAGA.DAL;

namespace SAGA.API.Dtos
{
    public class PrivilegiosDtos
    {
        public int Id { get; set; }
        public int IdPadre { get; set; }
        public int RolId { get; set; }
        public int EstructuraId { get; set; }
        public string Nombre { get; set; }
        public int TipoEstructuraId { get; set; }
        public string TipoEstructura { get; set; }
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool Especial { get; set; }
        public string Accion { get; set; }
        public string Icono { get; set; }
        public int Orden { get; set; }
        public ICollection<PrivilegiosDtos> Children { get; set; }
        public string Descripcion { get; set; }
    }
}