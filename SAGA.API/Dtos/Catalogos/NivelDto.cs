﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class NivelDto
    {
        public byte Id { get; set; }
        public string nivel { get; set; }
    }

    public class EstadoEstudioDto
    {
        public int Id { get; set; }
        public string Nivel { get; set; }
    }

}