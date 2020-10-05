﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Equipos
{
    public class ResumenDto
    {
        public Guid liderId { get; set; }
        public Guid reclutadorId { get; set; }
        public string nombre { get; set; }
        public string clave { get; set; }
        public int tipoUsuario { get; set; }
        public string tipo { get; set; }
        public int totalCub { get; set; }
        public int totalPos { get; set; }
        public int totalPosAux { get; set; }
        public string foto { get; set; }
        public List<RequisDtos> requis { get; set; }
        public List<RequisDtos> requisAux { get; set; }
        public List<ResumenDto> resumen { get; set; }
    }

    public class RequisDtos
    {
        public Guid requisicionId { get; set; }
        public int vacantes { get; set; }
        public string vBtra { get; set; }
        public long folio { get; set; }
        public int cubiertas { get; set; }
    }

}