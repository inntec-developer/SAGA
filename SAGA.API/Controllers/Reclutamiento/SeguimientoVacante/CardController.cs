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
        public IHttpActionResult GetDatosCard(Guid ClientId)
        {
            var dts = db.Clientes.Where(x => x.Id.Equals(ClientId)).Select(C => new
            {
                NombreComercial = C.Nombrecomercial,
                RazonSocial = C.RazonSocial,
                Telefonos = C.telefonos.Select(t => new
                {
                    Tipo = t.TipoTelefono.Tipo,
                    Telefono = t.telefono,
                    Ext = t.Extension,
                    Activo = t.Activo,
                    Principal = t.esPrincipal
                }),
                Direccion = C.direcciones.Select(d => new
                {
                    Tipo = d.TipoDireccion.tipoDireccion,
                    Pais = d.Pais.pais,
                    Estado = d.Estado.estado,
                    Municipio = d.Municipio.municipio,
                    Colonia = d.Colonia.colonia,
                    Calle = d.Calle,
                    NoExt = d.NumeroExterior,
                    NoInt = d.NumeroInterior,
                    CP = d.CodigoPostal,
                    Activo = d.Activo,
                    Principal = d.esPrincipal
                }),
                Contactos = C.Contactos.Select(p => new {
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
                Asignados = (from R in db.Requisiciones
                         join AR in db.AsignacionRequis on R.Id equals AR.RequisicionId
                         join E in db.Entidad on AR.GrpUsrId equals E.Id
                         where R.ClienteId == ClientId
                             select new
                         {
                             Nombre = E.Nombre,
                             ApellidoPaterno = E.ApellidoPaterno,
                             ApellidoMaterno = E.ApellidoMaterno
                         }).Distinct()


        });

            //var mocos = (from R in db.Requisiciones
            //             join AR in db.AsignacionRequis on R.Id equals AR.RequisicionId
            //             join E in db.Entidad on AR.GrpUsrId equals E.Id
            //             where R.ClienteId == id
            //             select new
            //             {
            //                 nombre = E.Nombre
            //             }).Distinct();
           

            return Ok(dts);
        }
    }
}
