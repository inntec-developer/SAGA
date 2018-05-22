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
                Clave = u.Clave,
                nombre = db.Personas.Where(p => p.Id.Equals(u.Id)).Select(p => p.Nombre),
                apellidoPaterno = db.Personas.Where(p => p.Id.Equals(u.Id)).Select(p => p.ApellidoPaterno),
                apellidoMaterno = db.Personas.Where(p => p.Id.Equals(u.Id)).Select(p => p.ApellidoMaterno),
                tipoUsuario = u.TipoUsuario.Tipo,
                Usuario = u.Usuario,
                Departamento = u.Departamento.Nombre,
                Email = db.Emails.Where(x => x.PersonaId.Equals(u.Id)).Select(e => new {
                    email = e.email
                }),
                grupos = db.GruposUsuarios.Where(gu => gu.UsuarioId.Equals(u.Id)).Select(g => new
                {
                    Id = g.GrupoId,
                    Nombre = db.Grupos.Where(x => x.Id.Equals(g.GrupoId)).Select(x => x.Nombre)
                }),


            activo = u.Activo

            }).ToList();


            //dts = db.Personas.Select(c => new PersonasDtos
            //{       Grupos = db.Personas.Where(p => p.Id.Equals(db.GruposUsuarios.Select(g => g.UsuarioId))).Select( gs => new
            //{
                //Grupo = db.Grupos.Where(x => x.Id.Equals(db.GruposUsuarios.Select(xx => xx.GrupoId))).Select(ng => ng.Nombre)
                //}),
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
        [Route("getUsuarioByDepa")]
        public IHttpActionResult getDtosByDepa(Guid id)
        {
            List<PersonasDtos> dts = new List<PersonasDtos>();

            var persona = db.Usuarios.Where(a => a.DepartamentoId.Equals(id)).Select(u => new
            {
                Id = u.Id,
                Clave = u.Clave,
                nombre = db.Personas.Where(p => p.Id.Equals(u.Id)).Select(p => p.Nombre),
                apellidoPaterno = db.Personas.Where(p => p.Id.Equals(u.Id)).Select(p => p.ApellidoPaterno),
                apellidoMaterno = db.Personas.Where(p => p.Id.Equals(u.Id)).Select(p => p.ApellidoMaterno),
                tipoUsuario = u.TipoUsuario.Tipo,
                Usuario = u.Usuario,
                Departamento = u.Departamento.Nombre,
                Email = db.Emails.Where(x => x.PersonaId.Equals(u.Id)).Select(e => new {
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

    }
}
