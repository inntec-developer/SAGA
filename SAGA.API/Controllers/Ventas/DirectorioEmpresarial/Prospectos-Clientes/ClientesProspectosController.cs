using AutoMapper;
using SAGA.API.Dtos;
using SAGA.DAL;
using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;

namespace SAGA.API.Controllers.Ventas.DirectorioEmpresarial.Prospectos_Clientes
{
    [RoutePrefix("api/Directorio")]
    public class ClientesProspectosController : ApiController
    {
        
        private SAGADBContext db;
        
        public ClientesProspectosController()
        {
            db = new SAGADBContext();
        }

        [Route("addProspecto")]
        [HttpPost]
        
        public IHttpActionResult AddProspecto(ProspectoDto prospecto)
        {
            using (DbContextTransaction beginTran = db.Database.BeginTransaction())
            {
                try
                {
                    var cliente = Mapper.Map<ProspectoDto, Cliente>(prospecto);
                    cliente.Nombrecomercial = prospecto.Nombrecomercial.ToUpper();
                    cliente.GiroEmpresaId = prospecto.GiroEmpresaId;
                    cliente.ActividadEmpresaId = prospecto.ActividadEmpresaId;
                    cliente.NumeroEmpleados = prospecto.NumeroEmpleados;
                    cliente.TamanoEmpresaId = prospecto.TamanoEmpresaId;
                    cliente.TipoEmpresaId = prospecto.TipoEmpresaId;
                    cliente.TipoBaseId = prospecto.TipoBaseId;
                    cliente.Clasificacion = prospecto.Clasificacion;
                    cliente.TipoEntidadId = 6;
                    cliente.Activo = true;
                    cliente.esCliente = false;
                    cliente.otraAgencia = false;
                    cliente.direcciones = prospecto.Direcciones;
                    cliente.telefonos = prospecto.Telefonos;
                    cliente.emails = prospecto.Emails;
                    cliente.Contactos = prospecto.Contactos;
                    cliente.Nombre = string.Empty;
                    cliente.ApellidoMaterno = string.Empty;
                    cliente.ApellidoPaterno = string.Empty;
                    cliente.fch_Modificacion = DateTime.Now;
                    cliente.UsuarioAlta = prospecto.Usuario;
                    cliente.UsuarioMod = prospecto.Usuario;
                    db.Clientes.Add(cliente);
                    db.SaveChanges();
                    Guid IdCliente = db.Clientes.OrderByDescending(x => x.fch_Creacion).Take(1).Select(x => x.Id).FirstOrDefault();
                    
                    List<Direccion> direcciones = db.Direcciones.Where(x => x.EntidadId.Equals(IdCliente)).ToList();
                    List<Telefono> telefonos = db.Telefonos.Where(x => x.EntidadId.Equals(IdCliente)).ToList();
                    List<Email> emails = db.Emails.Where(x => x.EntidadId.Equals(IdCliente)).ToList();
                    List<Contacto> contactos = db.Contactos.Where(x => x.ClienteId.Equals(IdCliente)).ToList();

                    foreach (Direccion d in direcciones)
                    {
                        //Recorrer los Telefonos
                        foreach (Telefono t in telefonos)
                        {
                            foreach (var dt in prospecto.DireccionTelefono)
                            {
                                if (d.Calle.Equals(dt.Calle)
                                    && d.NumeroExterior.Equals(dt.NumeroExterior)
                                    && d.NumeroInterior.Equals(dt.NumeroInterior)
                                    && d.CodigoPostal.Equals(dt.CodigoPostal)
                                    && t.telefono.Equals(dt.Telefono)
                                    && t.Extension.Equals(dt.Extension))
                                {
                                    DireccionTelefono de = new DireccionTelefono();
                                    de.DireccionId = d.Id;
                                    de.TelefonoId = t.Id;
                                    db.DireccionesTelefonos.Add(de);
                                    db.SaveChanges();
                                }
                            }
                        }
                        //Recorere los Emails
                        foreach (Email e in emails)
                        {
                            foreach (var em in prospecto.DireccionEmail)
                            {
                                if (d.Calle.Equals(em.Calle)
                                    && d.NumeroExterior.Equals(em.NumeroExterior)
                                    && d.NumeroInterior.Equals(em.NumeroInterior)
                                    && d.CodigoPostal.Equals(em.CodigoPostal)
                                    && e.email.Equals(em.Email))
                                {
                                    DireccionEmail de = new DireccionEmail();
                                    de.DireccionId = d.Id;
                                    de.EmailId = e.Id;
                                    db.DireccionesEmails.Add(de);
                                    db.SaveChanges();
                                }
                            }
                        }
                            
                        //Recorere los Emails
                        foreach (Contacto c in contactos)
                        {
                            foreach (var co in prospecto.DireccionContacto)
                            {
                                if (d.Calle.Equals(co.Calle)
                                    && d.NumeroExterior.Equals(co.NumeroExterior)
                                    && d.NumeroInterior.Equals(co.NumeroInterior)
                                    && d.CodigoPostal.Equals(co.CodigoPostal)
                                    && c.Nombre.Equals(co.Nombre)
                                    && c.ApellidoPaterno.Equals(co.ApellidoPaterno)
                                    && c.ApellidoMaterno.Equals(co.ApellidoMaterno)
                                    && c.Puesto.Equals(co.Puesto))
                                {
                                    DireccionContacto dc = new DireccionContacto();
                                    dc.DireccionId = d.Id;
                                    dc.ContactoId = c.Id;
                                    db.DireccionesContactos.Add(dc);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    beginTran.Commit();
                    return Ok(cliente.Id);
                }
                catch (Exception ex)
                {
                    beginTran.Rollback();
                    string msg = ex.Message;
                    return Ok(HttpStatusCode.NotModified);
                }
            }
        }

        [Route("hacerCliente")]
        [HttpPost]
        
        public IHttpActionResult HacerCliente(HacerClienteDto cliente)
        {
            try
            {
                var prospecto = db.Clientes.Find(cliente.Id);
                db.Entry(prospecto).State = EntityState.Modified;
                prospecto.RazonSocial = cliente.Razonsocial.ToUpper();
                prospecto.RFC = cliente.RFC.ToUpper();
                prospecto.esCliente = true;
                prospecto.UsuarioMod = cliente.Usuario;
                prospecto.fch_Modificacion = DateTime.Now;
                db.SaveChanges();


                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotAcceptable);
            }
        }

        [Route("EditInfoGeneral")]
        [HttpPost]
        
        public IHttpActionResult EditInforGeneral(InfoGeneralDto info)
        {
            try
            {
                var cliente = db.Clientes.Find(info.Id);
                db.Entry(cliente).State = EntityState.Modified;
                cliente.RazonSocial = info.RazonSocial.ToUpper();
                cliente.RFC = info.RFC.ToUpper();
                cliente.Nombrecomercial = info.NombreComercial.ToUpper();
                cliente.TamanoEmpresaId = info.TamanoEmpresa;
                cliente.NumeroEmpleados = info.NumeroEmpleados;
                cliente.GiroEmpresaId = info.GiroEmpresa;
                cliente.ActividadEmpresaId = info.ActividadEmpresa;
                cliente.TipoEmpresaId = info.TipoEmpresa;
                cliente.TipoBaseId = info.TipoBase;
                cliente.Clasificacion = info.Clasificacion;
                cliente.UsuarioMod = info.Usuario;
                cliente.fch_Modificacion = DateTime.Now;
                db.SaveChanges();
                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        #region Direccion
        [Route("AddDireccionCliente")]
        [HttpPost]
        
        public IHttpActionResult AddDireccionCliente(DireccionClienteDto info)
        {
            try
            {
                var direccion = Mapper.Map<DireccionClienteDto, Direccion>(info);
                direccion.TipoDireccionId = info.TipoDireccionId;
                direccion.Calle = info.Calle;
                direccion.NumeroExterior = info.NumeroExterior;
                direccion.NumeroInterior = info.NumeroInterior;
                direccion.PaisId = info.PaisId;
                direccion.EstadoId = info.EstadoId;
                direccion.MunicipioId = info.MunicipioId;
                direccion.ColoniaId = info.ColoniaId;
                direccion.CodigoPostal = info.CodigoPostal;
                direccion.esPrincipal = info.esPrincipal;
                direccion.Activo = info.Activo;
                direccion.Referencia = info.Referencia;
                direccion.EntidadId = info.EntidadId;
                direccion.UsuarioAlta = info.Usuario;
                direccion.UsuarioMod = info.Usuario;
                direccion.fch_Modificacion = DateTime.Now;
                db.Direcciones.Add(direccion);
                db.SaveChanges();

                var Id = db.Direcciones.OrderByDescending(d => d.fch_Creacion).Where(d => d.EntidadId.Equals(info.EntidadId)).Select(d => d.Id).Take(1).FirstOrDefault();

                return Ok(Id);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("EditDireccionCliente")]
        [HttpPost]
        
        public IHttpActionResult EditDireccionCliente(DireccionClienteDto info)
        {
            try
            {
                var direccion = db.Direcciones.Find(info.Id);
                db.Entry(direccion).State = EntityState.Modified;
                direccion.TipoDireccionId = info.TipoDireccionId;
                direccion.Calle = info.Calle;
                direccion.NumeroExterior = info.NumeroExterior;
                direccion.NumeroInterior = info.NumeroInterior;
                direccion.PaisId = info.PaisId;
                direccion.EstadoId = info.EstadoId;
                direccion.MunicipioId = info.MunicipioId;
                direccion.ColoniaId = info.ColoniaId;
                direccion.CodigoPostal = info.CodigoPostal;
                direccion.esPrincipal = info.esPrincipal;
                direccion.Activo = info.Activo;
                direccion.Referencia = info.Referencia;
                direccion.UsuarioMod = info.Usuario;
                direccion.fch_Modificacion = DateTime.Now;
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("DeleteDireccionCliente")]
        [HttpGet]
        
        public IHttpActionResult DeleteDireccionCliente(Guid DireccionId)
        {
            try
            {
                var direccion = db.Direcciones.Find(DireccionId);
                db.Entry(direccion).State = EntityState.Deleted;

                var dt = db.DireccionesTelefonos.Where(x => x.DireccionId.Equals(DireccionId)).ToList();
                //var telefonosId = dt.Select(x => x.TelefonoId).ToList();
                //var telefonos = db.Telefonos.Where(t => telefonosId.Contains(t.Id)).ToList();
                //db.Telefonos.RemoveRange(telefonos);
                db.DireccionesTelefonos.RemoveRange(dt);

                var de = db.DireccionesEmails.Where(x => x.DireccionId.Equals(DireccionId)).ToList();
                //var emailsId = de.Select(e => e.EmailId).ToList();
                //var emails = db.Emails.Where(e => emailsId.Contains(e.Id)).ToList();
                //db.Emails.RemoveRange(emails);
                db.DireccionesEmails.RemoveRange(de);

                var dc = db.DireccionesContactos.Where(x => x.DireccionId.Equals(DireccionId)).ToList();
                //var contactosId = dc.Select(c => c.ContactoId).ToList();
                //var contactos = db.Contactos.Where(c => contactosId.Contains(c.Id)).ToList();
                //db.Contactos.RemoveRange(contactos);
                db.DireccionesContactos.RemoveRange(dc);

                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        #endregion

        [Route("AddTelefonoCliente")]
        [HttpPost]
        
        public IHttpActionResult AddTelefonoCliente(TelefonoClienteDto info)
        {
            try
            {
                var telefono = Mapper.Map<TelefonoClienteDto, Telefono>(info);
                telefono.ClavePais = info.ClavePais;
                telefono.ClaveLada = info.ClaveLada;
                telefono.Extension = info.Extension;
                telefono.telefono = info.telefono;
                telefono.TipoTelefonoId = info.TipoTelefonoId;
                telefono.esPrincipal = info.esPrincipal;
                telefono.Activo = info.Activo;
                telefono.EntidadId = info.EntidadId;
                telefono.UsuarioAlta = info.Usuario;
                telefono.UsuarioMod = info.Usuario;
                telefono.fch_Modificacion = DateTime.Now;
                db.Telefonos.Add(telefono);
                db.SaveChanges();


                var Id = db.Telefonos.OrderByDescending(d => d.fch_Creacion).Where(d => d.EntidadId.Equals(info.EntidadId)).Select(d => d.Id).Take(1).FirstOrDefault();

                var direccionTelefono = new DireccionTelefono();
                direccionTelefono.DireccionId = info.DireccionId;
                direccionTelefono.TelefonoId = Id;
                db.DireccionesTelefonos.Add(direccionTelefono);
                db.SaveChanges();

                var IdDT = db.DireccionesTelefonos
                    .Where(dt => dt.DireccionId.Equals(info.DireccionId))
                    .Where(dt => dt.TelefonoId.Equals(Id))
                    .Select(dt => dt.Id)
                    .FirstOrDefault();

                Guid[] objeto = { IdDT, Id };

                return Ok(objeto);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("EditTelefonoCliente")]
        [HttpPost]
        
        public IHttpActionResult EditTelefonoCliente(TelefonoClienteDto info)
        {
            try
            {
                var telefono = db.Telefonos.Find(info.Id);
                db.Entry(telefono).State = EntityState.Modified;
                telefono.ClavePais = info.ClavePais;
                telefono.ClaveLada = info.ClaveLada;
                telefono.Extension = info.Extension;
                telefono.telefono = info.telefono;
                telefono.TipoTelefonoId = info.TipoTelefonoId;
                telefono.esPrincipal = info.esPrincipal;
                telefono.Activo = info.Activo;
                telefono.UsuarioMod = info.Usuario;
                telefono.fch_Modificacion = DateTime.Now;

                var direccionTelefono = db.DireccionesTelefonos.Find(info.IdDT);
                if(direccionTelefono != null)
                {
                    db.Entry(direccionTelefono).State = EntityState.Modified;
                    direccionTelefono.DireccionId = info.DireccionId;
                    direccionTelefono.TelefonoId = info.Id;
                }
                else
                {
                    var AddDireccionTelefono = new DireccionTelefono();
                    AddDireccionTelefono.DireccionId = info.DireccionId;
                    AddDireccionTelefono.TelefonoId = info.Id;
                    db.DireccionesTelefonos.Add(AddDireccionTelefono);
                    db.SaveChanges();
                }
                
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        [Route("DeleteTelefonoCliente")]
        [HttpGet]
        
        public IHttpActionResult DeleteTelefonoCliente(Guid TelefonoId)
        {
            try
            {
                var telefeno = db.Telefonos.Find(TelefonoId);
                db.Entry(telefeno).State = EntityState.Deleted;
                db.SaveChanges();
                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        [Route("AddEmailCliente")]
        [HttpPost]
        
        public IHttpActionResult AddEmailCliente(EmailClienteDto info)
        {
            try
            {
                var email = Mapper.Map<EmailClienteDto, Email>(info);
                email.email = info.email;
                email.EntidadId = info.EntidadId;
                email.UsuarioAlta = info.Usuario;
                email.UsuarioMod = info.Usuario;
                email.fch_Modificacion = DateTime.Now;
                email.esPrincipal = false;
                db.Emails.Add(email);
                db.SaveChanges();

                var Id = db.Emails.OrderByDescending(d => d.fch_Creacion).Where(d => d.EntidadId.Equals(info.EntidadId)).Select(d => d.Id).Take(1).FirstOrDefault();

                var direcionEmail = new DireccionEmail();
                direcionEmail.DireccionId = info.DireccionId;
                direcionEmail.EmailId = Id;
                db.DireccionesEmails.Add(direcionEmail);
                db.SaveChanges();

                var IdDE = db.DireccionesEmails
                    .Where(d => d.DireccionId.Equals(info.DireccionId))
                    .Where(d => d.EmailId.Equals(Id))
                    .Select(d => d.Id)
                    .FirstOrDefault();

                Guid[] objeto = { IdDE, Id };

                return Ok(objeto);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("EditEmailCliente")]
        [HttpPost]
        
        public IHttpActionResult EditEmailCliente(EmailClienteDto info)
        {
            try
            {
                var email = db.Emails.Find(info.Id);
                db.Entry(email).State = EntityState.Modified;
                email.email = info.email;
                email.EntidadId = info.EntidadId;
                email.UsuarioMod = info.Usuario;
                email.fch_Modificacion = DateTime.Now;

                var DirecionEmail = db.DireccionesEmails.Find(info.IdDE);
                if(DirecionEmail != null)
                { 
                    db.Entry(DirecionEmail).State = EntityState.Modified;
                    DirecionEmail.DireccionId = info.DireccionId;
                    DirecionEmail.EmailId = info.Id;
                }
                else
                {
                    var AddDirecionEmail = new DireccionEmail();
                    AddDirecionEmail.DireccionId = info.DireccionId;
                    AddDirecionEmail.EmailId = info.Id;
                    db.DireccionesEmails.Add(AddDirecionEmail);
                }

                db.SaveChanges();
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("DeleteEmailCliente")]
        [HttpGet]
        
        public IHttpActionResult DeleteEmailCliente(Guid EmailId)
        {
            try
            {
                var email = db.Emails.Find(EmailId);
                db.Entry(email).State = EntityState.Deleted;
                db.SaveChanges();
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("AddContactoCliente")]
        [HttpPost]
        
        public IHttpActionResult AddContactosCliente(ContactoClienteDto info)
        {
            try
            {
                var contacto = Mapper.Map<ContactoClienteDto, Contacto>(info);
                contacto.Nombre = info.Nombre;
                contacto.ApellidoPaterno = info.ApellidoPaterno;
                contacto.ApellidoMaterno = info.ApellidoMaterno;
                contacto.TipoEntidadId = 3;
                contacto.Puesto = info.Puesto;
                contacto.InfoAdicional = info.InfoAdicional;
                contacto.ClienteId = info.ClienteId;
                contacto.emails = info.emails;
                contacto.telefonos = info.telefonos;
                contacto.UsuarioAlta = info.Usuario;
                contacto.UsuarioMod = info.Usuario;
                contacto.fch_Modificacion = DateTime.Now;
                db.Contactos.Add(contacto);
                db.SaveChanges();

                var Id = db.Contactos.OrderByDescending(d => d.fch_Creacion).Where(d => d.ClienteId.Equals(info.ClienteId)).Select(d => d.Id).Take(1).FirstOrDefault();

                var direcionContaco = new DireccionContacto();
                direcionContaco.DireccionId = info.DireccionId;
                direcionContaco.ContactoId = Id;
                db.DireccionesContactos.Add(direcionContaco);
                db.SaveChanges();

                var IdDCn = db.DireccionesContactos
                    .Where(d => d.DireccionId.Equals(info.DireccionId))
                    .Where(d => d.ContactoId.Equals(Id))
                    .Select(d => d.Id)
                    .FirstOrDefault();

                var Tel = db.Telefonos
                            .Where(t => t.EntidadId.Equals(Id))
                            .Select(t => new {
                                id = t.Id,
                                clavePais = t.ClavePais,
                                claveLada = t.ClaveLada,
                                telefono = t.telefono,
                                extension = t.Extension,
                                tipoTelefono = t.TipoTelefono.Tipo,
                                TipoTelefonoId = t.TipoTelefonoId,
                            })
                            .ToList();
                
                var Email = db.Emails
                            .Where(t => t.EntidadId.Equals(Id))
                            .Select(t => new {
                                id = t.Id,
                                email = t.email,
                            })
                            .ToList();
                var obj = new
                {
                    IdDCn = IdDCn,
                    Id = Id,
                    telefonos = Tel,
                    emails = Email
                };

                return Ok(obj);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("EditContactoCliente")]
        [HttpPost]
        
        public IHttpActionResult EditContactosCliente(ContactoClienteDto info)
        {
            try
            {
                var contacto = db.Contactos.Find(info.Id);
                db.Entry(contacto).State = EntityState.Modified;
                contacto.Nombre = info.Nombre;
                contacto.ApellidoPaterno = info.ApellidoPaterno;
                contacto.ApellidoMaterno = info.ApellidoMaterno;
                contacto.Puesto = info.Puesto;
                contacto.InfoAdicional = info.InfoAdicional;
                contacto.UsuarioMod = info.Usuario;
                contacto.fch_Modificacion = DateTime.Now;

                var DireccionContacto = db.DireccionesContactos.Find(info.IdDCn);
                if (DireccionContacto != null)
                {
                    if(DireccionContacto.DireccionId != info.DireccionId)
                    {
                        db.Entry(DireccionContacto).State = EntityState.Modified;
                        DireccionContacto.DireccionId = info.DireccionId;
                        DireccionContacto.ContactoId = info.Id;
                    }
                }
                else
                {
                    var AddDireccionContactol = new DireccionContacto();
                    AddDireccionContactol.DireccionId = info.DireccionId;
                    AddDireccionContactol.ContactoId = info.Id;
                    db.DireccionesContactos.Add(AddDireccionContactol);
                }

                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("DeleteContactoCliente")]
        [HttpGet]
        
        public IHttpActionResult DeleteContactosCliente(Guid ContactoId)
        {
            try
            {
                var contacto = db.Contactos.Find(ContactoId);
                db.Entry(contacto).State = EntityState.Deleted;
                db.SaveChanges();
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("CRUDTelefonContacto")]
        [HttpPost]
        
        public IHttpActionResult CRUDTelefonoContacto(ContactoTelefonoDto telefono)
        {
            try
            {
                switch (telefono.Action)
                {
                    case "C":
                        Telefono tel = new Telefono();
                        tel.ClavePais = telefono.ClavePais;
                        tel.ClaveLada = telefono.ClaveLada;
                        tel.Extension = telefono.Extension;
                        tel.telefono = telefono.telefono;
                        tel.TipoTelefonoId = telefono.TipoTelefonoId;
                        tel.Activo = telefono.Activo;
                        tel.esPrincipal = telefono.esPrincipal;
                        tel.EntidadId = telefono.EntidadId;
                        tel.UsuarioAlta = telefono.Usuario;
                        tel.UsuarioMod = telefono.Usuario;
                        tel.fch_Modificacion = DateTime.Now;
                        db.Telefonos.Add(tel);
                        db.SaveChanges();
                        var IdTelefono = db.Telefonos
                            .Where(t => t.EntidadId.Equals(telefono.EntidadId)
                                        && t.telefono.Equals(telefono.telefono)
                                        && t.Extension.Equals(telefono.Extension))
                            .Select(x => x.Id).FirstOrDefault();
                        var obj = new { id = IdTelefono.ToString() };

                        return Ok(obj);

                    case "U":
                        var Telefono = db.Telefonos.Find(telefono.Id);
                        db.Entry(Telefono).State = EntityState.Modified;
                        Telefono.TipoTelefonoId = telefono.TipoTelefonoId;
                        Telefono.ClavePais = telefono.ClavePais;
                        Telefono.ClaveLada = telefono.ClaveLada;
                        Telefono.Extension = telefono.Extension;
                        Telefono.telefono = telefono.telefono;
                        Telefono.fch_Modificacion = DateTime.Now;
                        Telefono.UsuarioMod = telefono.Usuario;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    case "D":
                        var delete = db.Telefonos.Find(telefono.Id);
                        db.Entry(delete).State = EntityState.Deleted;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    default:
                        return Ok(HttpStatusCode.NotFound);
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("CRUDContactoCorreo")]
        [HttpPost]
        
        public IHttpActionResult CRUDContactoCorreo(ContactoCorreoDto correo)
        {
            try
            {
                switch (correo.Action)
                {
                    case "C":
                        Email em = new Email();
                        em.email = correo.email;
                        em.esPrincipal = correo.esPrincipal;
                        em.EntidadId = correo.EntidadId;
                        em.UsuarioAlta = correo.Usuario;
                        em.UsuarioMod = correo.Usuario;
                        em.fch_Modificacion = DateTime.Now;
                        db.Emails.Add(em);
                        db.SaveChanges();
                        var IdCorreo = db.Emails
                            .Where(e => e.email.Equals(correo.email) && e.EntidadId.Equals(correo.EntidadId))
                            .Select(e => e.Id).FirstOrDefault();
                        var obj = new { id = IdCorreo.ToString() };
                        return Ok(obj);
                    case "U":
                        var Email = db.Emails.Find(correo.Id);
                        db.Entry(Email).State = EntityState.Modified;
                        Email.email = correo.email;
                        Email.fch_Modificacion = DateTime.Now;
                        Email.UsuarioMod = correo.Usuario;
                        db.SaveChanges(); 
                        return Ok(HttpStatusCode.OK);
                    case "D":
                        var delete = db.Emails.Find(correo.Id);
                        db.Entry(delete).State = EntityState.Deleted;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    default:
                        return Ok(HttpStatusCode.NotFound);

                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("getCliente")]
        [HttpGet]
        
        public IHttpActionResult GetCliente(Guid ClienteId)
        {
            try
            {
                var cliente = db.Clientes
                    .Where(x => x.Id.Equals(ClienteId))
                    .Select(x => new
                    {
                        Id = x.Id,
                        RazonSocial = x.RazonSocial != null ? x.RazonSocial : "",
                        RFC = x.RFC != null ? x.RFC : "",
                        NombreComercial = x.Nombrecomercial,
                        GiroEmpresa = x.GiroEmpresas,
                        ActividadEmpresa = x.ActividadEmpresas,
                        TamanoEmpresa = x.TamanoEmpresas,
                        TipoEmpresa = x.TipoEmpresas,
                        TipoBase = x.TipoBases,
                        esCliente = x.esCliente,
                        NumeroEmpleados = x.NumeroEmpleados,
                        Clasificacion = x.Clasificacion,
                        Direcciones = x.direcciones.Select(d => new
                        {
                            Id = d.Id,
                            EntidadId = d.EntidadId,
                            Activo = d.Activo,
                            Calle = d.Calle,
                            CodigoPostal = d.CodigoPostal,
                            Colonia = d.Colonia.colonia,
                            ColoniaId = d.ColoniaId,
                            esPrincipal = d.esPrincipal,
                            Estado = d.Estado.estado,
                            EstadoId = d.EstadoId,
                            Municipio = d.Municipio.municipio,
                            MunicipioId = d.MunicipioId,
                            NumeroInterior = d.NumeroInterior,
                            NumeroExterior = d.NumeroExterior,
                            Pais = d.Pais.pais,
                            PaisId = d.PaisId,
                            Referencia = d.Referencia != null ? d.Referencia : "",
                            TipoDireccion = d.TipoDireccion.tipoDireccion,
                            TipoDireccionId = d.TipoDireccionId

                        }).ToList(),
                        Telefonos = db.Telefonos
                                    .Where(t => t.EntidadId.Equals(ClienteId))
                                    .Select(t => new
                                    {
                                        Id = t.Id,
                                        EntidadId = t.EntidadId,
                                        IdDT = db.DireccionesTelefonos
                                                .Where(dt => dt.TelefonoId.Equals(t.Id))
                                                .Select(dt => dt.Id)
                                                .FirstOrDefault(),
                                        Calle = db.DireccionesTelefonos
                                                    .Where(dt => dt.TelefonoId.Equals(t.Id)).FirstOrDefault() != null ? 
                                                    db.DireccionesTelefonos
                                                    .Where(dt => dt.TelefonoId.Equals(t.Id))
                                                    .Select(dt => dt.Direccion.Calle + " No. " + dt.Direccion.NumeroExterior + " C.P. " + dt.Direccion.CodigoPostal)
                                                    .FirstOrDefault() : "Sin Registro",
                                        DireccionId = db.DireccionesTelefonos.Where(dt => dt.TelefonoId.Equals(t.Id)).Select(dt => dt.DireccionId).FirstOrDefault(),
                                        ClavePais = t.ClavePais,
                                        ClaveLada = t.ClaveLada,
                                        Extension = t.Extension !=  null  ? t.Extension : "",
                                        Telefono = t.telefono,
                                        TTelefono = t.TipoTelefono.Tipo,
                                        TipoTelefonoId = t.TipoTelefonoId,
                                        Activo = t.Activo,
                                        esPrincipal = t.esPrincipal
                                    })
                                    .ToList(),
                        Correos = db.Emails
                                    .Where(e => e.EntidadId.Equals(ClienteId))
                                    .Select(e => new {
                                        Id = e.Id,
                                        EntidadId = e.EntidadId,
                                        IdDE = db.DireccionesEmails.Where(de => de.EmailId.Equals(e.Id)).Select(de => de.Id).FirstOrDefault(),
                                        Calle = db.DireccionesEmails
                                                    .Where(de => de.EmailId.Equals(e.Id)).FirstOrDefault() != null ? 
                                                    db.DireccionesEmails
                                                    .Where(de => de.EmailId.Equals(e.Id))
                                                    .Select(de => de.Direccion.Calle + " No. " + de.Direccion.NumeroExterior + " C.P. " + de.Direccion.CodigoPostal)
                                                    .FirstOrDefault() : "Sin Registro",
                                        DireccionId = db.DireccionesEmails.Where(de => de.EmailId.Equals(e.Id)).Select(de => de.DireccionId).FirstOrDefault(),
                                        Email = e.email
                                    })
                                    .ToList(),
                        Contactos = db.Contactos
                                    .Where(c => c.ClienteId.Equals(ClienteId))
                                    .Select(c => new
                                    {

                                        ApellidoPaterno = c.ApellidoPaterno,
                                        ApellidoMaterno = c.ApellidoMaterno != null ? c.ApellidoMaterno : "",
                                        Calle = db.DireccionesContactos
                                                    .Where(dc => dc.ContactoId.Equals(c.Id)).FirstOrDefault() != null ? db.DireccionesContactos
                                                    .Where(dc => dc.ContactoId.Equals(c.Id))
                                                    .Select(dc => dc.Direccion.Calle + " No. " + dc.Direccion.NumeroExterior + " C.P. " + dc.Direccion.CodigoPostal)
                                                    .FirstOrDefault() : "Sin Registro",
                                        ClienteId = c.ClienteId,
                                        DireccionId = db.DireccionesContactos
                                                        .Where(dc => dc.ContactoId.Equals(c.Id))
                                                        .Select(de => de.DireccionId)
                                                        .FirstOrDefault(),
                                        Emails = c.emails
                                                    .Select(ce => new {
                                                        Id = ce.Id,
                                                        Email = ce.email,
                                                    }).ToList(),
                                        EmailAux = c.emails.Select(e => e.email).FirstOrDefault(),
                                        Id = c.Id,
                                        IdDCn = db.DireccionesContactos.Where(dc => dc.ContactoId.Equals(c.Id)).Select(de => de.Id).FirstOrDefault(),
                                        Nombre = c.Nombre,
                                        nombreAux = c.Nombre + " " + c.ApellidoPaterno,
                                        Puesto = c.Puesto,
                                        InfoAdicional = c.InfoAdicional != null ? c.InfoAdicional : "",
                                        Telefonos = c.telefonos
                                                    .Select(ct => new
                                                    {
                                                        Id = ct.Id,
                                                        ClavePais = ct.ClavePais,
                                                        ClaveLada = ct.ClaveLada,
                                                        Extension = ct.Extension != null ? ct.Extension : "",
                                                        Telefono = ct.telefono,
                                                        TipoTelefono = ct.TipoTelefono.Tipo,
                                                        TipoTelefonoId = ct.TipoTelefonoId
                                                    }).ToList(),
                                        TelefonoAux = c.telefonos.Select(t => t.telefono).FirstOrDefault(),
                                        TipoTelefonoAux = c.telefonos.Select(t => t.TipoTelefono.Tipo).FirstOrDefault(),
                                       
                                    })
                                    .ToList()
                    }).FirstOrDefault();
                    
                return Ok(cliente);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [Route("CoincidenciaCliente")]
        [HttpPost]
        
        public IHttpActionResult Similitud(ClienteCoincidenciaDto cliente)
        {
            try
            {
                var match = db.Clientes
                    .Where(x => x.Nombrecomercial.Contains(cliente.Cliente) || x.RazonSocial.Contains(cliente.Cliente))
                    .Select(c => new
                    {
                        c.esCliente,
                        c.RazonSocial,
                        c.Nombrecomercial, 
                        c.RFC
                    })
                    .OrderBy(x => x.esCliente)
                    .ToList();
                return Ok(match);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

    }
}
