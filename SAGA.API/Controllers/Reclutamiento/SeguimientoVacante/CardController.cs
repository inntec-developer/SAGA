using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/reclutamiento/SeguimientoVacante")]
    public class CardController : ApiController
    {
        private SAGADBContext db;
        public CardController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getCard")]
        public IHttpActionResult GetDatosCard(Guid ClientId, Guid RequisicionId)
        {
            //PrivilegiosController obj = new PrivilegiosController();
            //List<Guid> ids = new List<Guid>();
            //var Asignados = (from R in db.Requisiciones
            //                 join AR in db.AsignacionRequis on R.Id equals AR.RequisicionId
            //                 join E in db.Entidad on AR.GrpUsrId equals E.Id
            //                 where R.Id.Equals(RequisicionId)
            //                 select new
            //                 {
            //                     Id = E.Id,
            //                     tipo = E.TipoEntidadId
            //                 }).ToList();

            var Asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(RequisicionId)).Select(A => A.GrpUsrId).ToList();

            //foreach (var id in Asignados)
            //{
            //    if(id.tipo == 4)
            //    {
            //        obj.GetGrupo(id.Id, ids);
            //    }
            //    else
            //    {
            //        ids.Add(id.Id);
            //    }
            //}

            //ids.Distinct();

            var dts2 = db.Requisiciones.Where(x => x.Id.Equals(RequisicionId)).Select(C => new
            {
                NombreComercial = C.Cliente.Nombrecomercial,
                RazonSocial = C.Cliente.RazonSocial,
                Telefonos = C.Cliente.telefonos.Where(x => x.Activo.Equals(true)).Select(t => new
                {
                    Tipo = t.TipoTelefono.Tipo,
                    Telefono = t.telefono,
                    Exts = t.Extension,
                    Activo = t.Activo ? "Activo" : "",
                    Principal = t.esPrincipal
                }),
                Direccion = new
                {
                    Tipo = C.Direccion.TipoDireccion.tipoDireccion,
                    Pais = C.Direccion.Pais.pais,
                    Estado = C.Direccion.Estado.estado,
                    Municipio = C.Direccion.Municipio.municipio,
                    Colonia = C.Direccion.Colonia.colonia,
                    Calle = C.Direccion.Calle,
                    NoExt = C.Direccion.NumeroExterior,
                    NoInt = C.Direccion.NumeroInterior,
                    CP = C.Direccion.CodigoPostal,
                    Activo = C.Direccion.Activo,
                    Principal = C.Direccion.esPrincipal
                },
                Contactos = C.Cliente.Contactos.Select(p => new {
                    Puesto = p.Puesto,
                    Nombre = p.Nombre,
                    ApellidoPaterno = p.ApellidoPaterno,
                    ApellidoMaterno = p.ApellidoMaterno,
                    Tipo = p.TipoEntidad.tipoEntidad,
                    Telefonos = p.telefonos.Select(t => new
                    {
                        ext = t.Extension,
                        Telefono = t.telefono
                    }),
                    Emails = p.emails.Select(e => new
                    {
                        email = e.email
                    })
                }),
                Asignados = db.Entidad.Where(x => Asignados.Contains(x.Id)).Select(E => new
                {
                    EntidadId = E.Id,
                    Nombre = E.Nombre,
                    ApellidoPaterno = E.ApellidoPaterno,
                    ApellidoMaterno = E.ApellidoMaterno,
                    Foto = E.Foto,
                    data = db.GruposUsuarios.Where(x => x.GrupoId.Equals(E.Id)).Select(G => new
                    {
                        Foto = G.Entidad.Foto,
                        Nombre = G.Entidad.Nombre,
                        ApellidoPaterno = G.Entidad.ApellidoPaterno,
                        ApellidoMaterno = G.Entidad.ApellidoMaterno
                    })
                }).ToList()

            }).ToList();
            //var dts = db.Clientes.Where(x => x.Id.Equals(ClientId)).Select(C => new
            //{
            //    NombreComercial = C.Nombrecomercial,
            //    RazonSocial = C.RazonSocial,
            //    Telefonos = C.telefonos.Where(x => x.Activo.Equals(true)).Select(t => new
            //    {
            //        Tipo = t.TipoTelefono.Tipo,
            //        Telefono = t.telefono,
            //        Exts = C.telefonos.Where(xx => xx.telefono.Equals(t.telefono)).Select(ex => new
            //        {
            //            ext = ex.Extension
            //        }),
            //        Activo = t.Activo ? "Activo" : "",
            //        Principal = t.esPrincipal
            //    }),
            //    Direccion = C.direcciones.Select(d => new
            //    {
            //        Tipo = d.TipoDireccion.tipoDireccion,
            //        Pais = d.Pais.pais,
            //        Estado = d.Estado.estado,
            //        Municipio = d.Municipio.municipio,
            //        Colonia = d.Colonia.colonia,
            //        Calle = d.Calle,
            //        NoExt = d.NumeroExterior,
            //        NoInt = d.NumeroInterior,
            //        CP = d.CodigoPostal,
            //        Activo = d.Activo,
            //        Principal = d.esPrincipal
            //    }),
            //    Contactos = C.Contactos.Select(p => new {
            //        Puesto = p.Puesto,
            //        Nombre = p.Nombre,
            //        ApellidoPaterno = p.ApellidoPaterno,
            //        ApellidoMaterno = p.ApellidoMaterno,
            //        Tipo = p.TipoEntidad.tipoEntidad,
            //        Telefonos = p.telefonos.Select(t => new
            //        {
            //            ext = t.Extension,
            //            Telefono = t.telefono
            //        }),
            //        Emails = p.emails.Select(e => new
            //        {
            //            email = e.email
            //        })
            //    }),
            //    Asignados = db.Entidad.Where(x => ids.Contains(x.Id)).Select(E => new
            //    { 
            //        EntidadId = E.Id,
            //        Nombre = E.Nombre,
            //        ApellidoPaterno = E.ApellidoPaterno,
            //        ApellidoMaterno = E.ApellidoMaterno,
            //        Foto = E.Foto,
            //        data = db.GruposUsuarios.Where(x => x.GrupoId.Equals(E.Id)).Select(G => new {
            //            Foto = G.Entidad.Foto,
            //            Nombre = G.Entidad.Nombre,
            //            ApellidoPaterno = G.Entidad.ApellidoPaterno,
            //            ApellidoMaterno = G.Entidad.ApellidoMaterno
            //        })
            //     })
            //    }).ToList();

            return Ok(dts2);
        }
    }
}
