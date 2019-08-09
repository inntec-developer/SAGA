using SAGA.API.Dtos;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Ventas.PrefilReclutamiento
{
    [RoutePrefix("api/PerfilReclutamiento")]
    //[Authorize]
    public class PerfilReclutamientoController : ApiController
    {
        private SAGADBContext db;

        public PerfilReclutamientoController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getCliente")]
        public IHttpActionResult GetClientes(string busqueda)
        {
            try
            {
                var clientes = db.Clientes
                    .Where(c => c.RazonSocial.Contains(busqueda) || c.Nombrecomercial.Contains(busqueda) || c.RFC.Contains(busqueda))
                    .Where(c => c.Activo.Equals(true) && c.esCliente.Equals(true))
                    .Select(c => new
                    {
                        c.Id,
                        c.RazonSocial,
                        c.Nombrecomercial,
                        c.RFC,
                        c.GiroEmpresas.giroEmpresa,
                        c.ActividadEmpresas.actividadEmpresa                        
                    })
                    .ToList();

                return Ok(clientes);
            }
            catch(Exception ex)
            {
                string mesg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getClienteId")]
        public IHttpActionResult GetClienteId(Guid PerfilId)
        {
            try
            {
                var clientes = db.DAMFO290
                    .Where(d => d.Id.Equals(PerfilId))
                    .Select(d => new
                    {
                        Id= d.Id,
                        ClienteId = d.ClienteId,
                        RazonSocial = d.Cliente.RazonSocial,
                        Nombrecomercial = d.Cliente.Nombrecomercial,
                        RFC = d.Cliente.RFC,
                        GiroEmpresa = d.Cliente.GiroEmpresas.giroEmpresa,
                        ActividadEmpresa = d.Cliente.ActividadEmpresas.actividadEmpresa,
                        Tipo = d.TipoReclutamientoId,
                        Clase = d.ClaseReclutamientoId,
                    })
                    .FirstOrDefault();

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                string mesg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getInfoCliente")]
        public IHttpActionResult GetInformacionCliente(Guid ClienteId)
        {
            try
            {
                var info = db.Clientes.
                    Where(c => c.Id.Equals(ClienteId))
                    .Select(c => new
                    {
                        direcciones = c.direcciones.Select(d => new
                        {
                            id = d.Id,
                            tipoDireccion = d.TipoDireccion.tipoDireccion,
                            pais = d.Pais.pais,
                            estado = d.Estado.estado,
                            municipio = d.Municipio.municipio,
                            colonia = d.Colonia.colonia,
                            calle = d.Calle,
                            numeroExterior = d.NumeroExterior,
                            numeroInterior = d.NumeroInterior,
                            codigoPostal = d.CodigoPostal,
                            activo = d.Activo,
                            esPrincipal = d.esPrincipal,
                        }).ToList(),
                        telefonos = db.Telefonos
                                    .Where(t => t.EntidadId == ClienteId)
                                    .Select(t => new {
                                        Calle = db.DireccionesTelefonos
                                            .Where(dt => dt.TelefonoId.Equals(t.Id)).FirstOrDefault() != null ?
                                            db.DireccionesTelefonos
                                            .Where(dt => dt.TelefonoId.Equals(t.Id))
                                            .Select(dt => dt.Direccion.Calle + " No. " + dt.Direccion.NumeroExterior + " C.P. " + dt.Direccion.CodigoPostal)
                                            .FirstOrDefault() : "Sin Registro",
                                        tipo = t.TipoTelefono.Tipo,
                                        clavePais = t.ClavePais,
                                        claveLada = t.ClaveLada,
                                        telefono = t.telefono,
                                        extension = t.Extension,
                                        activo = t.Activo,
                                        esPrincipal = t.esPrincipal
                                    }).ToList(),
                        contactos = db.Contactos
                                    .Where(cn => cn.ClienteId == ClienteId)
                                    .Select(cn => new
                                    {
                                        Calle = db.DireccionesContactos
                                                    .Where(dc => dc.ContactoId.Equals(cn.Id)).FirstOrDefault() != null ? db.DireccionesContactos
                                                    .Where(dc => dc.ContactoId.Equals(cn.Id))
                                                    .Select(dc => dc.Direccion.Calle + " No. " + dc.Direccion.NumeroExterior + " C.P. " + dc.Direccion.CodigoPostal)
                                                    .FirstOrDefault() : "Sin Registro",
                                        nombre = cn.Nombre,
                                        apellidoPaterno = cn.ApellidoPaterno,
                                        apellidoMaterno = cn.ApellidoMaterno,
                                        puesto = cn.Puesto,
                                        infoAdicional = cn.InfoAdicional,
                                        telefonos = db.Telefonos
                                            .Where(t => t.EntidadId == cn.Id)
                                            .Select(t => new {
                                                tipo = t.TipoTelefono.Tipo,
                                                clavePais = t.ClavePais,
                                                claveLada = t.ClaveLada,
                                                telefono = t.telefono,
                                                extension = t.Extension
                                            })
                                            .ToList(),
                                        Email = db.Emails
                                            .Where(e => e.EntidadId == cn.Id)
                                            .Select(e => new { email = e.email })
                                            .ToList(),
                                    }).ToList(),

                    
                    }).FirstOrDefault();

                return Ok(info);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getInfoPerfil")]
        public IHttpActionResult GetInfoPerfil(Guid PerfilId)
        {
            try
            {
                var perfil = db.DAMFO290
                .Where(x => x.Id.Equals(PerfilId))
                .Select(x => new
                {
                    x.NombrePerfil,
                    x.GeneroId,
                    x.EdadMinima,
                    x.EdadMaxima,
                    x.EstadoCivilId,
                    escolaridades = x.escolardadesPerfil
                        .Select(e => new
                        {
                            e.Id,
                            e.EscolaridadId,
                            nivelId = e.EstadoEstudioId,
                            Escolaridad = e.Escolaridad.gradoEstudio,
                            Nivel = e.EstadoEstudio.estadoEstudio
                        }).ToList(),
                    x.AreaId,
                    x.ContratoInicialId,
                    x.TiempoContratoId,
                    aptitudes = x.aptitudesPerfil
                        .Select(a => new
                        {
                            Id = a.AptitudId
                        }).ToList(),
                    x.Experiencia,
                    x.SueldoMinimo,
                    x.SueldoMaximo,
                    x.DiaCorteId,
                    x.TipoNominaId,
                    x.DiaPagoId,
                    x.PeriodoPagoId
                })
                .FirstOrDefault();
                return Ok(perfil);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }





        #region Escolaridadades
        [HttpPost]
        [Route("addEscolaridad")]
        public IHttpActionResult AddEscolaridades(EscoPerfilDto esco)
        {
            try
            {
                var escolaridad = new EscolaridadesPerfil();
                escolaridad.EscolaridadId = esco.EscolaridadId;
                escolaridad.EstadoEstudioId = esco.EstadoEstudioId;
                escolaridad.DAMFO290Id = esco.Damfo290Id;
                escolaridad.UsuarioAlta = esco.Usuario;
                db.EscolaridadesPerfil.Add(escolaridad);
                db.SaveChanges();
                var escoID = db.EscolaridadesPerfil
                    .Where(e => e.DAMFO290Id.Equals(esco.Damfo290Id))
                    .OrderByDescending(e => e.fch_Creacion)
                    .Select(e => e.Id)
                    .Take(1)
                    .FirstOrDefault();
                return Ok(escoID);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        #endregion
    }
}
