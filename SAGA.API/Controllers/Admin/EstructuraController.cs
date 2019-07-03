using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using System.Data.Entity;
using System.Collections;

namespace SAGA.API.Controllers.Admin
{
    [RoutePrefix("api/admin")]
    public class EstructuraController : ApiController
    {
        private SAGADBContext db;
        public EstructuraController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("GetEstructura")]
        [Authorize]
        public IHttpActionResult GetEstructura()
        {
            List<PrivilegiosDtos> Tree = new List<PrivilegiosDtos>();

            Tree = db.Estructuras.Where(e => e.Id > 1 && e.Activo == true && e.TipoEstructuraId <= 4).Select(ee => new PrivilegiosDtos()
            {
                Id = ee.Id,
                IdPadre = ee.IdPadre,
                Nombre = ee.Nombre,
                TipoEstructuraId = ee.TipoEstructuraId,
                Orden = ee.Orden,
                Accion = ee.Accion,
                Descripcion = ee.Descripcion
            }).OrderBy(o => o.Orden).ToList();


            var nodes = Tree.Where(x => x.TipoEstructuraId.Equals(2)).Select(ee => new PrivilegiosDtos()
            {
                EstructuraId = ee.Id,
                IdPadre = ee.IdPadre,
                Nombre = ee.Nombre,
                Children = GetChild(Tree, ee.Id),
                TipoEstructuraId = ee.TipoEstructuraId,
                Orden = ee.Orden,
                Accion = ee.Accion,
                Descripcion = ee.Descripcion
            }).OrderBy(o=>o.Orden).ToList();

            return Ok(nodes);
        }

    
        public ICollection<PrivilegiosDtos> GetChild(List<PrivilegiosDtos> tree, int id)
        {
            return tree
                    .Where(c => c.IdPadre == id)
                    .Select(c => new PrivilegiosDtos
                    {
                        EstructuraId = c.Id,
                        Nombre = c.Nombre,
                        IdPadre = c.IdPadre,
                        Children = GetChild(tree, c.Id),
                        TipoEstructuraId = c.TipoEstructuraId,
                        Orden = c.Orden,
                        Accion = c.Accion,
                        Descripcion = c.Descripcion
                    }).OrderBy(o=>o.Orden)
                    .ToList();
        }


        [HttpGet]
        [Route("GetEstructura2")]
        public IHttpActionResult GetEstructura2()
        {
            List<PrivilegiosDtos> Tree = new List<PrivilegiosDtos>();

            Tree = db.Estructuras.Where(e => e.Id > 1).Select(ee => new PrivilegiosDtos()
            {
                Id = ee.Id,
                IdPadre = ee.IdPadre,
                Nombre = ee.Nombre,
                Orden = ee.Orden,
                TipoEstructuraId = ee.TipoEstructuraId
            }).OrderBy( o => o.Orden).ToList();



            return Ok(Tree);
        }

        //public List<PrivilegiosDtos> GetData(PrivilegiosDtos tree)
        //{
        //    List<PrivilegiosDtos> data = new List<PrivilegiosDtos>();
        //    PrivilegiosDtos mocos = new PrivilegiosDtos();

        //    mocos.EstructuraId = tree.Id;
        //    mocos.Nombre = tree.Nombre;
        //    mocos.IdPadre = tree.IdPadre;
        //    mocos.TipoEstructuraId = tree.TipoEstructuraId;



        //    data.Add(mocos);
        //    return data;

        //}


       

    }

  
}
