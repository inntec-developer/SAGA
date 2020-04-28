using SAGA.DAL;
using SAGA.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Ventas.PrefilReclutamiento
{
    [RoutePrefix("api/PerfilReclutamiento")]
    public class TransferDamfo290Controller : ApiController
    {
        private SAGADBContext db;
        public TransferDamfo290Controller()
        {
            db = new SAGADBContext();
        }
        [HttpGet]
        [Route("getSubordinados")]
        [Authorize]
        public IHttpActionResult GetSubordinados(Guid lider)
        {
            try
            {
                List<Guid> uids = new List<Guid>();
                Utilerias.GetSub obj = new Utilerias.GetSub();
                var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(lider) && x.LiderId.Equals(lider)).Select(u => u.UsuarioId).ToList();

                uids = obj.RecursividadSub(ids, uids);

                var usuarios = db.Usuarios.Where(x => uids.Contains(x.Id)).Select(n => new {
                    usuarioId = n.Id,
                    nombre = n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno
                });
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }
    }
}
