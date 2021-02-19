﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class PeriodoIncapacidad
    {
        public Guid Id { get; set; }
        public Guid CandidatosInfoId { get; set; }
        public string SerieFolio { get; set; }
        public int dias { get; set; }
        public DateTime fchIncio { get; set; }
        public DateTime fchFin { get; set; }
        public string Archivo { get; set; }
        public string Comentario { get; set; }
        public int TiposIncapacidadId { get; set; }
        public DateTime fchAlta { get; set; }
        public Guid UsuarioAltaId { get; set; }

        public TiposDiasEconomicos TiposIncapacidad { get; set; }
        public CandidatosInfo CandidatosInfo { get; set; }
        public Usuarios UsuarioAlta { get; set; }
    }
}
