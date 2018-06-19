using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;

namespace SAGA.API.Controllers.Admin
{
    [RoutePrefix("api/admin")]
    public class PrivilegiosController : ApiController
    {
        private SAGADBContext db;
        public PrivilegiosController()
        {
            db = new SAGADBContext();
        }

        [HttpPost]
        [Route("agregarPrivilegio")]
        public IHttpActionResult AgregarPrivilegio(List<Privilegio> listJson)
        {
            string mensaje = "Se agregó Privilegio";

            try
            {
                foreach (Privilegio ru in listJson)
                {

                    db.Privilegios.Add(ru);
                }


                //db.SaveChanges();


            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return Ok(mensaje);
        }

        [HttpGet]
        [Route("GetEstructura")]
        public IHttpActionResult GetEstructura()
        {
            List<PrivilegiosDtos> Tree = new List<PrivilegiosDtos>();

            Tree = db.Estructuras.Where(e => e.Id > 1 ).Select(ee => new PrivilegiosDtos()
            {
                Id = ee.Id,
                IdPadre = ee.IdPadre,
                Nombre = ee.Nombre,
                TipoEstructuraId = ee.TipoEstructuraId
            }).ToList();

          
            var nodes = Tree.Where(x => x.TipoEstructuraId.Equals(2)).Select(ee => new PrivilegiosDtos()
            {
                EstructuraId = ee.Id,
                IdPadre = ee.IdPadre,
                Nombre = ee.Nombre,
                Children = GetChild(Tree, ee.Id)
            }).ToList();

            return Ok(nodes);
        }

        public ICollection<PrivilegiosDtos> GetChild(List<PrivilegiosDtos> tree, int id)
        {
            return  tree
                    .Where(c => c.IdPadre == id)
                    .Select(c => new PrivilegiosDtos 
                    {
                        EstructuraId = c.Id,
                        Nombre = c.Nombre,
                        IdPadre = c.IdPadre,
                        Children = GetChild(tree, c.Id)
                    })
                    .ToList();
        }


    }
}
