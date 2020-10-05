using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.SistFirmas
{
    public class Damfo022Dto
    {
        public Guid Id { get; set; }
        public string Folio { get; set; }
        public DateTime Fecha { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }
        public string Descripcion { get; set; }
        public string Problema { get; set; }
        public string Causa_Raiz { get; set; }
        public string SolucionTmp { get; set; }
        public string Solucion { get; set; }
        public bool Estatus { get; set; }
        public bool Activo { get; set; }
        public FIRM_Damfo022 Damfo022 { get; set; }
        public List<FIRM_Porques> Porques { get; set; }
        public List<FIRM_CausaEfecto> CausasEfecto { get; set; }
        public List<FIRM_Compromiso> Compromisos { get; set; }
    }
}