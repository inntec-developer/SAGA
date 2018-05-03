using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos.Admin;

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
        public IHttpActionResult GetDtosPersonal()
        {
            List<PersonasDtos> dts = new List<PersonasDtos>();

            dts = db.Usuarios.Where(x => x.Activo.Equals(true)).Select(c => new PersonasDtos
            {
                Id= c.Id,
                nombre = db.Personas.Where(p => p.Id.Equals(c.Id)).Select(x => x.Nombre).FirstOrDefault(),
                apellidoPaterno = db.Personas.Where(p => p.Id.Equals(c.Id)).Select(x => x.ApellidoPaterno).FirstOrDefault(),
                apellidoMaterno = db.Personas.Where(p => p.Id.Equals(c.Id)).Select(x => x.ApellidoMaterno).FirstOrDefault(),
                TipoUriario = c.TipoUsuario.Tipo,
                Usuario = c.Usuario,
                Email = db.Emails.Where(e => e.PersonaId.Equals(c.Id)).Select(x => x.email).FirstOrDefault()

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


    }
}
