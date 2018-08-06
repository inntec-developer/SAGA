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

        public List<Guid> GetGrupo(Guid grupo, List<Guid> listaIds)
        {

            var idG = db.GruposUsuarios
                    .Where(x => x.EntidadId == grupo)
                    .Select(g => g.GrupoId).ToList();

            if (idG.Count > 0)
            {
                listaIds.Add(idG.FirstOrDefault());
                return GetGrupo(idG.FirstOrDefault(), listaIds);
            }

            return listaIds;
        }


        [HttpGet]
        [Route("getprivilegios")]
        public IHttpActionResult GetPrivilegios2(Guid idUser)
        {
           
            List<PrivilegiosDtos> privilegios = new List<PrivilegiosDtos>();

            var Grupos = db.GruposUsuarios // Obtenemos los Ids de las celulas o grupos a los que pertenece.
                           .Where(g => g.EntidadId.Equals(idUser) & g.Grupo.Activo)
                           .Select(g => g.GrupoId)
                           .ToList();

            //falta el for para los demas grupos
            
            var mocos = GetGrupo(Grupos.FirstOrDefault(), Grupos);

            Grupos.Add(idUser);

            var roles = db.RolEntidades
                        .Where(x => Grupos.Contains(x.EntidadId))
                        .Select(r => r.RolId
                        ).ToList();

            var privilegiosRoles = db.Privilegios.Where(x => roles.Contains(x.RolId) & x.Rol.Activo)
                   .Select(P => new
                   {
                       RolId = P.RolId,
                       Rol = P.Rol.Rol,
                       Nombre = P.Estructura.Nombre,
                       Create = P.Create,
                       Read = P.Read,
                       Update = P.Update,
                       Delete = P.Delete,
                       Especial = P.Especial,
                       IdPadre = P.Estructura.IdPadre,
                       EstructuraId = P.Estructura.Id,
                       Accion = P.Estructura.Accion,
                       Icono = P.Estructura.Icono,
                       TipoEstructuraId = P.Estructura.TipoEstructuraId,
                       Orden = P.Estructura.Orden
                   }).OrderBy(o => o.Orden).ToList();

            foreach (var registro in
               privilegiosRoles.GroupBy(g => g.EstructuraId).Select((v, i) => new { Indice = i, Valor = v }) // Agrupar por el indice repetido. Valor tiene mis registros repetidos
               .Select(x => new {

                   IdPadre = x.Valor.Select(c => c.IdPadre).FirstOrDefault(),
                   EstructuraId = x.Valor.Key,
                   Nombre = x.Valor.Select(c => c.Nombre).FirstOrDefault(),
                   Create = x.Valor.Where(c => c.Create.Equals(true)).Select(c => c.Create).FirstOrDefault(),
                   Read = x.Valor.Where(c => c.Read.Equals(true)).Select(c => c.Read).FirstOrDefault(),
                   Update = x.Valor.Where(c => c.Update.Equals(true)).Select(c => c.Update).FirstOrDefault(),
                   Delete = x.Valor.Where(c => c.Delete.Equals(true)).Select(c => c.Delete).FirstOrDefault(),
                   Especial = x.Valor.Where(c => c.Especial.Equals(true)).Select(c => c.Especial).FirstOrDefault(),
                   RolId = x.Valor.Select(c => c.RolId).FirstOrDefault(),
                   Rol = x.Valor.Select(c => c.Rol).FirstOrDefault(),
                   Link = x.Valor.Select(c => c.Accion).FirstOrDefault(),
                   Icon = x.Valor.Select(c => c.Icono).FirstOrDefault(),
                   TipoEstructuraId = x.Valor.Select(c => c.TipoEstructuraId).FirstOrDefault(),
                   Orden = x.Valor.Select(c => c.Orden).FirstOrDefault()
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
                        Nombre = registro.Nombre.ToString(), //nombre de la estructura
                        TipoEstructuraId = registro.TipoEstructuraId,
                        Accion = registro.Link,
                        Icono = registro.Icon,
                        Orden = registro.Orden

                    });
            }

            return Ok(privilegios);


        }
        public List<PrivilegiosDtos> GetPrivilegios(Guid idUser)
        {

            List<PrivilegiosDtos> privilegios = new List<PrivilegiosDtos>();
         
            var Grupos = db.GruposUsuarios // Obtenemos los Ids de las celulas o grupos a los que pertenece.
                           .Where(g => g.EntidadId.Equals(idUser) & g.Grupo.Activo.Equals(true))
                           .Select(g => g.GrupoId)
                           .ToList();

            var mocos = GetGrupo(Grupos.FirstOrDefault(), Grupos);

            Grupos.Add(idUser);

            var roles = db.RolEntidades
                       .Where(x => Grupos.Contains(x.EntidadId) & x.Rol.Activo.Equals(true))
                       .Select(r => r.RolId
                       ).ToList();

            var privilegiosRoles = db.Privilegios.Where(x => roles.Contains(x.RolId))
                   .Select(P => new
                   {
                       RolId = P.RolId,
                       Rol = P.Rol.Rol,
                       Nombre = P.Estructura.Nombre,
                       Create = P.Create,
                       Read = P.Read,
                       Update = P.Update,
                       Delete = P.Delete,
                       Especial = P.Especial,
                       IdPadre = P.Estructura.IdPadre,
                       EstructuraId = P.Estructura.Id,
                       Accion = P.Estructura.Accion,
                       Icono = P.Estructura.Icono,
                       TipoEstructuraId = P.Estructura.TipoEstructuraId,
                       Orden = P.Estructura.Orden
                   }).OrderBy(o => o.Orden).ToList();

            foreach (var registro in
                privilegiosRoles.GroupBy(g => g.EstructuraId).Select((v, i) => new { Indice = i, Valor = v }) // Agrupar por el indice repetido. Valor tiene mis registros repetidos
                .Select(x => new {

                    IdPadre = x.Valor.Select(c => c.IdPadre).FirstOrDefault(),
                    EstructuraId = x.Valor.Key,
                    Nombre = x.Valor.Select(c => c.Nombre).FirstOrDefault(),
                    Create = x.Valor.Where(c => c.Create.Equals(true)).Select(c => c.Create).FirstOrDefault(),
                    Read = x.Valor.Where(c => c.Read.Equals(true)).Select(c => c.Read).FirstOrDefault(),
                    Update = x.Valor.Where(c => c.Update.Equals(true)).Select(c => c.Update).FirstOrDefault(),
                    Delete = x.Valor.Where(c => c.Delete.Equals(true)).Select(c => c.Delete).FirstOrDefault(),
                    Especial = x.Valor.Where(c => c.Especial.Equals(true)).Select(c => c.Especial).FirstOrDefault(),
                    RolId = x.Valor.Select(c => c.RolId).FirstOrDefault(),
                    Rol = x.Valor.Select(c => c.Rol).FirstOrDefault(),
                    Link = x.Valor.Select(c => c.Accion).FirstOrDefault(),
                    Icon = x.Valor.Select(c => c.Icono).FirstOrDefault(),
                    TipoEstructuraId = x.Valor.Select(c => c.TipoEstructuraId).FirstOrDefault(),
                    Orden = x.Valor.Select(c => c.Orden).FirstOrDefault()
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
                        Nombre = registro.Nombre, //nombre de la estructura
                        TipoEstructuraId = registro.TipoEstructuraId,
                        Accion = registro.Link,
                        Icono = registro.Icon,
                        Orden = registro.Orden

                    });
            }

            return privilegios;
        }

 
        //[HttpGet]
        //[Route("getprivilegiosDios")]
        public List<PrivilegiosDtos> GetPrivilegiosDios()
        {
            List<PrivilegiosDtos> privilegiosDios = new List<PrivilegiosDtos>();

            privilegiosDios = db.Estructuras.Where(x => x.Id.Equals(2) || ( x.Id >= 117 && x.Id <= 127 ))
                   .Select(P => new PrivilegiosDtos
                   {
                       Nombre = P.Nombre,
                       Create = true,
                       Read = true,
                       Update = true,
                       Delete = true,
                       Especial = true,
                       IdPadre = P.IdPadre,
                       EstructuraId = P.Id,
                       Accion = P.Accion,
                       Icono = P.Icono,
                       TipoEstructuraId = P.TipoEstructuraId,
                       Orden = P.Orden
                   }).OrderBy(o => o.Orden).ToList();

            return (privilegiosDios);
        }
        [HttpPost]
        [Route("modificarPrivilegios")]
        public IHttpActionResult ModificarPrivilegios(List<PrivilegiosDtos> listJson)
        {
            try
            {

                foreach (PrivilegiosDtos ru in listJson)
                {
                    var r = db.Privilegios.Where(x => x.RolId.Equals(ru.RolId) && x.EstructuraId.Equals(ru.EstructuraId)).FirstOrDefault();

                    db.Entry(r).State = EntityState.Modified;

                    r.Create = ru.Create;
                    r.Read = ru.Read;
                    r.Update = ru.Update;
                    r.Delete = ru.Delete;
                    r.Especial = ru.Especial;

                    db.SaveChanges();
                }
                return Ok(HttpStatusCode.Created);
            }
            catch( Exception ex )
            {
                return Ok(HttpStatusCode.NotModified);
            }
            
        }

        [HttpPost]
        [Route("agregarSeccion")]
        public IHttpActionResult AgregarSeccion(List<PrivilegiosDtos> privilegios)
        {
            string mensaje = "Se agregó Rol";
            //int id = 0;
            try
            {
                var rolId = privilegios.Select(n => n.RolId).FirstOrDefault();

                var r = db.Privilegios.Where(x => x.RolId.Equals(rolId)).FirstOrDefault();

                db.Entry(r).State = EntityState.Added;

                foreach (PrivilegiosDtos ru in privilegios)
                {
                    if (ru.Create || ru.Read || ru.Update || ru.Delete || ru.Especial)
                    {
                        Privilegio o = new Privilegio();
                        o.RolId = rolId;
                        o.EstructuraId = ru.EstructuraId;
                        o.Create = ru.Create;
                        o.Read = ru.Read;
                        o.Update = ru.Update;
                        o.Delete = ru.Delete;
                        o.Especial = ru.Especial;

                        db.Privilegios.Add(o);
                        db.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }
            return Ok(mensaje);

        }

    }
}
