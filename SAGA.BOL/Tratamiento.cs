﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Tratamiento
    {
        [Key]
        public int Id { get; set; }
        public string tratamiento { get; set; }
    }
}
