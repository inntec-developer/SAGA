using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Utilerias
{
    public class BusinessDay
    {
        public DateTime bussines (DateTime fecha_Creacion, int DiasAdicionales )
        {
            Int16 count = 1;
            Int16 NumeroDias = 0;

            while(count < DiasAdicionales)
            {
                NumeroDias = Convert.ToInt16(fecha_Creacion.DayOfWeek);
                while(NumeroDias == 1 || NumeroDias == 7)
                {
                    DiasAdicionales++;
                    NumeroDias++;
                }
                count++;
            }
            fecha_Creacion.AddDays(DiasAdicionales);
            return fecha_Creacion;
        }
    }
}