﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class ReporteGeneralDto
    {
        public Guid Id { get; set; }
        public Int64 Folio { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Cumplimiento { get; set; }
        public string RazonSocial { get; set; }
        public string Nombrecomercial { get; set; }
        public string estado { get; set; }
        public string VBtra { get; set; }
        public decimal SueldoMaximo { get; set; }
        public string tipoReclutamiento { get; set; }
        public string clasesReclutamiento { get; set; }
        public int vacantes { get; set; }
        public int contratados { get; set; }
        public int faltante { get; set; }
        public int porcentaje { get; set; }
        public int enProcesoEC { get; set; }
        public int enProcesoFC { get; set; }
        public int diasTrans { get; set; }
        public string domicilio_trabajo { get; set; }
        public string solicita { get; set; }
        public string coordinador { get; set; }
        public Guid PropietarioId { get; set; }
        public Guid AprobadorId { get; set; }
        public List<EstatusRequiDto> Estatus { get; set; }
        public List<string> comentarios_solicitante { get; set; }
        public List<string> comentarios_coord { get; set; }
        public List<CR> comentarios_reclutador { get; set; }
    }

    public class CR
    {
        public string reclutador { get; set; }
        public List<comentariosRecl> comentario { get; set; }
    }

    public class comentariosRecl
    {
        public string comentario { get; set; }
        public DateTime fch_Creacion { get; set; }
    }
}