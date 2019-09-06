using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Utilerias
{
    public class GetSub
    {
        private SAGADBContext db;
        public GetSub()
        {
            db = new SAGADBContext();
        }

        public List<Guid> RecursividadSub(List<Guid> uid, List<Guid> listaIds)
        {
            foreach (var u in uid)
            {
                listaIds.Add(u);
                var listadoNuevo = db.Subordinados
                  .Where(g => g.LiderId.Equals(u))
                         .Select(g => g.UsuarioId)
                         .ToList();

                RecursividadSub(listadoNuevo, listaIds);

            }
            return listaIds;
        }
    }
}