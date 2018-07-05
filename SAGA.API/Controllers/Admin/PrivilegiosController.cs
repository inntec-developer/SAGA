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

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/admin")]
    public class PrivilegiosController : ApiController
    {
        private SAGADBContext db;
        public PrivilegiosController()
        {
            db = new SAGADBContext();
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
                Children = GetChild(Tree, ee.Id),
                TipoEstructuraId = ee.TipoEstructuraId
            }).ToList();

            return Ok(nodes);
        }

        public ICollection<PrivilegiosDtos> GetChild(List<PrivilegiosDtos> tree, int id)
        {
            return  tree
                    .Where(c => c.IdPadre == id && c.TipoEstructuraId < 7)
                    .Select(c => new PrivilegiosDtos 
                    {
                        EstructuraId = c.Id,
                        Nombre = c.Nombre,
                        IdPadre = c.IdPadre,
                        Children = GetChild(tree, c.Id),
                        TipoEstructuraId = c.TipoEstructuraId
                    })
                    .ToList();
        }

        [HttpGet]
        [Route("getprivilegios")]
        public IHttpActionResult GetPrivilegios(Guid idUser)
        {
            List<PrivilegiosDtos> privilegios = new List<PrivilegiosDtos>();

            // me da los privilegios de roles en donde se encuentra el usuario 
            // me falta que tambien me saque los datos de usuarios sin grupos
            var query = ( from E in db.Entidad
                        join RE in db.RolEntidades on E.Id equals RE.EntidadId
                        join G in db.Grupos on E.Id equals G.Id
                        join GU in db.GruposUsuarios on G.Id equals GU.GrupoId
                        join P in db.Privilegios on RE.RolId equals P.RolId
                        join ES in db.Estructuras on P.EstructuraId equals ES.Id
                        where GU.EntidadId == idUser
                          select new { idPadre = ES.IdPadre, EstructuraId = ES.Id, TipoEstructuraId = ES.TipoEstructuraId, nombre = ES.Nombre, link= ES.Accion, icon = ES.Icono, RolId = RE.RolId, Rol = RE.Rol.Rol, Create = P.Create, Read = P.Read, Update = P.Update, Delete = P.Delete, Especial = P.Especial }).ToList();

            foreach (var registro in
                query.GroupBy(g => g.EstructuraId).Select((v, i) => new { Indice = i, Valor = v }) // Agrupar por el indice repetido. Valor tiene mis registros repetidos
                .Select(x => new {
                    
                    IdPadre = x.Valor.Select(c => c.idPadre).FirstOrDefault(),
                    EstructuraId = x.Valor.Key,
                    Create = x.Valor.Where(c => c.Create.Equals(true)).Select(c => c.Create).FirstOrDefault(),
                    Read = x.Valor.Where(c => c.Read.Equals(true)).Select(c => c.Read).FirstOrDefault(),
                    Update = x.Valor.Where(c => c.Update.Equals(true)).Select(c => c.Update).FirstOrDefault(),
                    Delete = x.Valor.Where(c => c.Delete.Equals(true)).Select(c => c.Delete).FirstOrDefault(),
                    Especial = x.Valor.Where(c => c.Especial.Equals(true)).Select(c => c.Especial).FirstOrDefault(),
                    RolId = x.Valor.Select(c => c.RolId).FirstOrDefault(),
                    Rol = x.Valor.Select(c => c.nombre).FirstOrDefault(),
                    Link = x.Valor.Select(c => c.link).FirstOrDefault(),
                    Icon = x.Valor.Select(c => c.icon).FirstOrDefault(),
                    TipoEstructuraId = x.Valor.Select(c=> c.TipoEstructuraId).FirstOrDefault()
                }))
            {
                privilegios.Add(
                    new PrivilegiosDtos
                    {
                        IdPadre = registro.IdPadre,
                        EstructuraId = registro.EstructuraId,
                        Create = registro.Create,
                        Read = registro.Read,
                        Update = registro.Update,
                        Delete = registro.Delete, 
                        Especial = registro.Especial,
                        RolId = registro.RolId,
                        Nombre = registro.Rol, //nombre de la estructura
                        TipoEstructuraId = registro.TipoEstructuraId, 
                        Accion = registro.Link,
                        Icono = registro.Icon
                        
                    });



            }

            return Ok(privilegios);
        }
        public List<PrivilegiosDtos> GetPrivilegios2(Guid idUser)
        {
            List<PrivilegiosDtos> privilegios = new List<PrivilegiosDtos>();

            // me da los privilegios de roles en donde se encuentra el usuario 
            // me falta que tambien me saque los datos de usuarios sin grupos
            var query = (from E in db.Entidad
                         join RE in db.RolEntidades on E.Id equals RE.EntidadId
                         join G in db.Grupos on E.Id equals G.Id
                         join GU in db.GruposUsuarios on G.Id equals GU.GrupoId
                         join P in db.Privilegios on RE.RolId equals P.RolId
                         join ES in db.Estructuras on P.EstructuraId equals ES.Id
                         where GU.EntidadId == idUser
                         select new { idPadre = ES.IdPadre, EstructuraId = ES.Id, TipoEstructuraId = ES.TipoEstructuraId, nombre = ES.Nombre, link = ES.Accion, icon = ES.Icono, RolId = RE.RolId, Rol = RE.Rol.Rol, Create = P.Create, Read = P.Read, Update = P.Update, Delete = P.Delete, Especial = P.Especial }).ToList();

            foreach (var registro in
                query.GroupBy(g => g.EstructuraId).Select((v, i) => new { Indice = i, Valor = v }) // Agrupar por el indice repetido. Valor tiene mis registros repetidos
                .Select(x => new {

                    IdPadre = x.Valor.Select(c => c.idPadre).FirstOrDefault(),
                    EstructuraId = x.Valor.Key,
                    Create = x.Valor.Where(c => c.Create.Equals(true)).Select(c => c.Create).FirstOrDefault(),
                    Read = x.Valor.Where(c => c.Read.Equals(true)).Select(c => c.Read).FirstOrDefault(),
                    Update = x.Valor.Where(c => c.Update.Equals(true)).Select(c => c.Update).FirstOrDefault(),
                    Delete = x.Valor.Where(c => c.Delete.Equals(true)).Select(c => c.Delete).FirstOrDefault(),
                    Especial = x.Valor.Where(c => c.Especial.Equals(true)).Select(c => c.Especial).FirstOrDefault(),
                    RolId = x.Valor.Select(c => c.RolId).FirstOrDefault(),
                    Rol = x.Valor.Select(c => c.nombre).FirstOrDefault(),
                    Link = x.Valor.Select(c => c.link).FirstOrDefault(),
                    Icon = x.Valor.Select(c => c.icon).FirstOrDefault(),
                    TipoEstructuraId = x.Valor.Select(c => c.TipoEstructuraId).FirstOrDefault()
                }))
            {
                privilegios.Add(
                    new PrivilegiosDtos
                    {
                        IdPadre = registro.IdPadre,
                        EstructuraId = registro.EstructuraId,
                        Create = registro.Create,
                        Read = registro.Read,
                        Update = registro.Update,
                        Delete = registro.Delete,
                        Especial = registro.Especial,
                        RolId = registro.RolId,
                        Nombre = registro.Rol, //nombre de la estructura
                        TipoEstructuraId = registro.TipoEstructuraId,
                        Accion = registro.Link,
                        Icono = registro.Icon

                    });



            }

            return privilegios;
        }

        [HttpPost]
        [Route("modificarPrivilegios")]
        public IHttpActionResult ModificarPrivilegios(PrivilegiosDtos listJson)
        {
            try
            {
                var r = db.Privilegios.Where(x => x.RolId.Equals(listJson.RolId) && x.EstructuraId.Equals(listJson.EstructuraId)).FirstOrDefault();

                db.Entry(r).State = EntityState.Modified;

                r.Create = listJson.Create;
                r.Read = listJson.Read;
                r.Update = listJson.Update;
                r.Delete = listJson.Delete;
                r.Especial = listJson.Especial;

                db.SaveChanges();

                return Ok(r);
            }
            catch( Exception ex )
            {
                return Ok(ex.Message);
            }
            
        }

    }
}
