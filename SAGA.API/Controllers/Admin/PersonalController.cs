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
                nombre = db.Personas.Where(p => p.Id.Equals(u.Id)).Select(p=>p.Nombre),
                apellidoPaterno = db.Personas.Where(p => p.Id.Equals(u.Id)).Select(p => p.ApellidoPaterno),
                apellidoMaterno = db.Personas.Where(p => p.Id.Equals(u.Id)).Select(p => p.ApellidoMaterno),
                tipoUsuario = u.TipoUsuario.Tipo,
                Usuarios = u.Usuario,
                Departamento = u.Departamento.Nombre,
                Email = db.Emails.Where(x => x.PersonaId.Equals(u.Id)).Select(e => new {
                    email = e.email
                })

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

            return Ok(dts);

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
    }
}
