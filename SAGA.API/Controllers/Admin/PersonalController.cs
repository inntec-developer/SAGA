using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using AutoMapper;
using System.Data.Entity;

namespace SAGA.API.Controllers.Admin
{
    [RoutePrefix("api/admin")]
    public class PersonalController : ApiController
    {
        private SAGADBContext db;
        public PersonalController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("get")]
        public IHttpActionResult getDtosPersonal()
        {
            List<PersonasDtos> dts = new List<PersonasDtos>();

            //dts = db.Usuarios.Select(c => new PersonasDtos
            //{
            //    Id = c.Id,
            //    Clave = c.Clave,
            //    nombre = db.Personas.Where(p => p.Id.Equals(c.Id)).Select(x => x.Nombre).FirstOrDefault(),
            //    apellidoPaterno = db.Personas.Where(p => p.Id.Equals(c.Id)).Select(x => x.ApellidoPaterno).FirstOrDefault(),
            //    apellidoMaterno = db.Personas.Where(p => p.Id.Equals(c.Id)).Select(x => x.ApellidoMaterno).FirstOrDefault(),
            //    tipoUsuario = c.TipoUsuario.Tipo,
            //    Usuario = c.Usuario,
            //    Departamento = c.Departamento.Nombre,
            //    Email = db.Emails.Where(x => x.PersonaId.Equals(c.Id)).Select(e => new{
            //        Email = e.email,
            //        Ie.Id }).ToList()

            //}).ToList();

            var persona = db.Usuarios.Select(u => new
            {
                Id = u.Id,
                Foto = u.Foto,
                Clave = u.Clave,
                nombre = u.Nombre,
                apellidoPaterno = u.ApellidoPaterno,
                apellidoMaterno = u.ApellidoMaterno,
                tipoUsuario = u.TipoUsuario.Tipo,
                tipoUsuarioId = u.TipoUsuario.Id,
                Usuario = u.Usuario,
                Departamento = u.Departamento.Nombre,
                DepartamentoId = u.Departamento.Id,
                Email = db.Emails.Where(x => x.EntidadId.Equals(u.Id)).Select(e => new {
                    email = e.email
                }),
                grupos = db.GruposUsuarios.Where(gu => gu.EntidadId.Equals(u.Id)).Select(g => new
                {
                    Id = g.GrupoId,
                    Nombre = db.Grupos.Where(x => x.Id.Equals(g.GrupoId)).Select(x => x.Nombre)
                }),
              
                activo = u.Activo

            }).OrderBy(o => o.nombre).ToList();

            return Ok(persona);

        }

        [HttpGet]
        [Route("getEntidades")]
        public IHttpActionResult GetEntidades()
        {
            var persona = db.Entidad.Where(x => x.TipoEntidadId.Equals(1) || x.TipoEntidadId.Equals(4)).Select(u => new
            {
                Id = u.Id,
                Foto = String.IsNullOrEmpty(u.Foto) ? "http://localhost:4200/assets/img/user/01.jpg" : u.Foto,
                Clave = db.Usuarios.Where(x => x.Id.Equals(u.Id)).Select(c => c.Clave).FirstOrDefault(),
                nombre = u.Nombre,
                apellidoPaterno = string.IsNullOrEmpty(u.ApellidoPaterno) ? "" : u.ApellidoPaterno,
                apellidoMaterno = string.IsNullOrEmpty(u.ApellidoMaterno) ? "" : u.ApellidoMaterno,
                Usuario = db.Usuarios.Where(x => x.Id.Equals(u.Id)).Select(c => c.Usuario).FirstOrDefault(),
                Departamento = db.Usuarios.Where(x => x.Id.Equals(u.Id)).Select(c => c.Departamento.Nombre).FirstOrDefault(),
                Email = db.Emails.Where(x => x.EntidadId.Equals(u.Id)).Select(e => new {
                    email = e.email
                }),
                grupos = db.GruposUsuarios.Where(gu => gu.EntidadId.Equals(u.Id)).Select(g => new
                {
                    Id = g.GrupoId,
                    Nombre = db.Grupos.Where(x => x.Id.Equals(g.GrupoId)).Select(x => x.Nombre).FirstOrDefault()
                })

            }).OrderBy(o => o.nombre).ToList();

            return Ok(persona);

        }

        [HttpGet]
        [Route("getUsuarioByDepa")]
        public IHttpActionResult getDtosByDepa(Guid id)
        {
            List<PersonasDtos> dts = new List<PersonasDtos>();

            var persona = db.Usuarios.Where(a => a.DepartamentoId.Equals(id)).Select(u => new
            {
                Id = u.Id,
                Clave = u.Clave,
                nombre = db.Entidad.Where(p => p.Id.Equals(u.Id)).Select(p => p.Nombre),
                apellidoPaterno = db.Entidad.Where(p => p.Id.Equals(u.Id)).Select(p => p.ApellidoPaterno),
                apellidoMaterno = db.Entidad.Where(p => p.Id.Equals(u.Id)).Select(p => p.ApellidoMaterno),
                tipoUsuario = u.TipoUsuario.Tipo,
                Usuario = u.Usuario,
                Departamento = u.Departamento.Nombre,
                Email = db.Emails.Where(x => x.EntidadId.Equals(u.Id)).Select(e => new {
                    email = e.email
                }),
                activo = u.Activo

            }).ToList();


            //dts = db.Personas.Select(c => new PersonasDtos
            //{
            //    nombre = c.Nombre,
            //    apellidoPaterno = c.ApellidoPaterno,
            //    apellidoMaterno = c.ApellidoMaterno,
            //    TipoUriario = db.Usuarios.Where(x => x.Id.Equals(c.Id)).Select(x => x.TipoUsuario.Tipo).FirstOrDefault(),
            //    Usuario = db.Usuarios.Where(u => u.Id.Equals(c.Id)).Select(x => x.Usuario).FirstOrDefault(),
            //    Email = db.Emails.Where(e => e.PersonaId.Equals(c.Id)).Select(x => x.email).FirstOrDefault()
            //}).ToList();

            return Ok(persona);

        }

        [HttpGet]
        [Route("getUsuarioByGrupo")]
        public IHttpActionResult GetDtosByGrupo(Guid id)
        {
            try
            {
                var persona = db.GruposUsuarios.Where(x => x.GrupoId.Equals(id)).Select(u => new
                {
                    EntidadId = u.EntidadId,
                    Foto = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select( f => String.IsNullOrEmpty(f.Foto) ? "http://localhost:4200/assets/img/user/01.jpg" : f.Foto).FirstOrDefault(),
                    TipoEntidadId = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(n => n.TipoEntidadId).FirstOrDefault(),
                    nombre = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(n => n.Nombre).FirstOrDefault(),
                    apellidoPaterno = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(n => string.IsNullOrEmpty(n.ApellidoPaterno) ? "" : n.ApellidoPaterno).FirstOrDefault(),
                    apellidoMaterno = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(n => string.IsNullOrEmpty(n.ApellidoMaterno) ? "" : n.ApellidoMaterno).FirstOrDefault(),
                    Usuario = db.Usuarios.Where(x => x.Id.Equals(u.EntidadId)).Select(c => string.IsNullOrEmpty(c.Usuario) ? "" : c.Usuario).FirstOrDefault(),
                }).OrderBy(o => o.TipoEntidadId).ToList();

                return Ok(persona);
            }
            catch( Exception ex)
            {
                return Ok(ex);
            }

        }


        [HttpPost]
        [Route("addUsuario")]
        public IHttpActionResult AddUsuario(PersonasDtos listJson)
        {
            string msj = "Se agregó usuario";

            try
            {
                var usuario = Mapper.Map<PersonasDtos, Usuarios>(listJson);
                usuario.Clave = listJson.Clave;
                usuario.Usuario = listJson.Usuario;
                usuario.Nombre = listJson.nombre;
                usuario.ApellidoPaterno = listJson.apellidoPaterno;
                usuario.ApellidoMaterno = listJson.apellidoMaterno;
                usuario.emails = listJson.Email;
                usuario.DepartamentoId = listJson.DepartamentoId;
                usuario.UsuarioAlta = "INNTEC";
                usuario.TipoUsuarioId = 5;
                usuario.Password = listJson.Password;
                usuario.TipoEntidadId = 1;
                usuario.Foto = listJson.Foto;
                db.Usuarios.Add(usuario);
                db.SaveChanges();


            }
            catch (Exception ex)
            {
                msj = ex.Message;
            }

            return Ok(msj);
        }

        [HttpGet]
        [Route("udActivo")]
        public IHttpActionResult UdActivo(Guid id, bool v)
        {
            string msj = "Actualizó";
            try
            {

                var usuario = db.Usuarios.Find(id);

                db.Usuarios.Attach(usuario);
                usuario.Activo = v;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.Message;
            }

            return Ok(msj);
        }
        [HttpPost]
        [Route("updateUsuario")]
        public IHttpActionResult UpdateUsuario(PersonasDtos listJson)
        {
            string msj = "Actualizó usuario";

            try
            {
                var usuario = db.Usuarios.Find(listJson.Id);
                db.Entry(usuario).State = EntityState.Modified;

                usuario.Usuario = listJson.Usuario;
                usuario.Nombre = listJson.nombre;
                usuario.ApellidoPaterno = listJson.apellidoPaterno;
                usuario.ApellidoMaterno = listJson.apellidoMaterno;
                usuario.DepartamentoId = listJson.DepartamentoId;
                usuario.UsuarioAlta = "INNTEC";
                usuario.TipoUsuarioId = listJson.TipoUsuarioId;
                usuario.Foto = listJson.Foto;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.Message;
            }

            return Ok(msj);
        }

        [HttpGet]
        [Route("setUsers")]
        public IHttpActionResult SetUsers(string p, string e)
        {
            var query =
                   (from users in db.Usuarios
                    join email in db.Emails on users.Id equals email.EntidadId
                    where users.Password == p & email.email == e
                    select new { IdUser = users.Id }).ToList();
             
            if (query.Count() == 0)
                return Ok(0);
            else

                return Ok(query.FirstOrDefault());
     

        }

    }
}
