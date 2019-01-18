using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Examenes
{
    public class ExamenDto
    {
        //public int ExamenId { get; set; }
        //public string Examen { get; set; }
        //public string TipoExamen { get; set; }
        //public int TipoExamenId { get; set; }
        //public int PreguntaId { get; set; }
        public string Pregunta { get; set; }
        //public Guid RespuestaId { get; set; }
        //public string Respuesta { get; set; }
       // public int Validacion { get; set; }
       public int Tipo { get; set; } //tipo pregunta abierta o seleccion
        public List<RespuestaDto> Respuestas { get; set; }
        public TipoExamenDto TipoExamen { get; set; }


    }

    public class RespuestaDto
    {
        public string resp { get; set; }
        public int value { get; set; }

    }

    public class RequiExamenDto
    {
        public Guid RequisicionId { get; set; }
        public int ExamenId { get; set; }
    }
}