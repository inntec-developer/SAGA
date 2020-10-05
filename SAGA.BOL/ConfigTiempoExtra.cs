using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class ConfigTiempoExtra
    {
        [Key]
        public int Id { get; set; }
        public bool Redondeo { get; set; }
        public byte TE_Total { get; set; } // horas totales a la semana
        public int TE_Media { get; set; } // minutos a partir de los cuales se considera media extra
        public int TE_Hora { get; set; } // minutos a partir de los cuales se considera hora extra
        public byte TE_Dobles { get; set; } // horas pago doble en un dia
        public byte TE_Triple { get; set; } //horas triples a pagar en un dia
        public string Comentarios { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }

        //Artículo 66.- Podrá también prolongarse la jornada de trabajo por circunstancias extraordinarias, sin exceder nunca de tres horas diarias ni de tres veces en una semana.

        //Nota: Siempre las primeras 2 horas serán consideradas como dobles para su pago, en base al salario diario y la tercera como triple para su pago en base al salario diario;
        //es decir, en 3 horas extras dos se pagan como dobles y una como triple.La ley dice que no se pueden trabajar mas de 9 horas extras a la semana y no mas de 3 por día.


    }
}
