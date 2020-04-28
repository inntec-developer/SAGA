using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace SAGA.API.Dtos.Examenes
{
    public class ExamenDto
    {
        //public int ExamenId { get; set; }
        //public string Examen { get; set; }
        //public string TipoExamen { get; set; }
        //public int TipoExamenId { get; set; }
        //public int PreguntaId { get; set; }
        public PreguntaDto Pregunta { get; set; }
        public Guid usuarioId { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public string Descripcion { get; set; }
        public List<RespuestaDto> Respuestas { get; set; }
        public TipoExamenDto TipoExamen { get; set; }


    }

    public class PreguntaDto
    {
        public int preguntaId { get; set; }
        public string Pregunta { get; set; }
        public int Tipo { get; set; }
        public int Orden { get; set; }
        public string file { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class RespuestaDto
    {
        public string resp { get; set; }
        public int value { get; set; }
        public string file { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class RequiExamenDto
    {
        public Guid RequisicionId { get; set; }
        public int ExamenId { get; set; }
    }
}