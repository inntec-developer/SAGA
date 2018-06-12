using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SAGA.API.Utilerias
{
    public class Rastreabilidad
    {
        SAGADBContext db;

        public Rastreabilidad()
        {
            db = new SAGADBContext();
        }


        public void RastreabilidadInsert(Guid trazabilidadId, string Usuario, int TipoAccion)
        {
            object[] _params = {
                    new SqlParameter("@TRAZABILIDADID", trazabilidadId),
                    new SqlParameter("@USUARIO", Usuario.ToUpper()),
                    new SqlParameter("@TIPOACCIONID", TipoAccion)
                };
            db.Database.SqlQuery<RastreabilidadMes>("exec dbo.Rastreabilidad @TRAZABILIDADID, @USUARIO, @TIPOACCIONID", _params).Single();
        }
    }
}