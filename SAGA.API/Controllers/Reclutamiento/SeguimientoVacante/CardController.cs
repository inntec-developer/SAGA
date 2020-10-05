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

            try
            {
                var Asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(RequisicionId)).Select(A => A.GrpUsrId).ToList();

            
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
                    Contactos = C.Cliente.Contactos.Select(p => new
                    {
                        Puesto = p.Puesto,
                        Nombre = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
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
                        //data = db.GruposUsuarios.Where(x => x.GrupoId.Equals(E.Id)).Select(G => new
                        //{
                        //    Foto = G.Entidad.Foto,
                        //    Nombre = G.Entidad.Nombre,
                        //    ApellidoPaterno = G.Entidad.ApellidoPaterno,
                        //    ApellidoMaterno = G.Entidad.ApellidoMaterno
                        //})
                    }).ToList()

                }).ToList();


                return Ok(dts2);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
    }
}
