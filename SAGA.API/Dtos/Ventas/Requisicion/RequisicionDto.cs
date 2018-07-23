﻿using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class RequisicionDto
    {
        public Guid Id { get; set; }
        public Int64 Folio { get; set; }
        public DateTime fch_Cumplimiento { get; set; }
        public int PrioridadId { get; set; }
        public int EstatusId { get; set; }
        public bool Confidencial { get; set; }
        public string Usuario { get; set; }
        public virtual List<AsignacionRequi> AsignacionRequi { get; set; }

    }
}