﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class GiroEmpresaDto
    {
        public int Id { get; set; }
        public string giroEmpresa { get; set; }
        public bool activo { get; set; }
    }
}