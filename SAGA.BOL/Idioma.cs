using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAGA.BOL
{
    public class Idioma
    {
        public int Id { get; set; } 
        [Display(Name ="Idioma")]
        public string idioma { get; set; }
    }
}