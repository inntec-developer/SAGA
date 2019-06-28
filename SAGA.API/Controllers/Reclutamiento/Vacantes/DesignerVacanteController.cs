using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using SAGA.API.Dtos;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/dvacante")]
    public class DesignerVacanteController : ApiController
    {
        private SAGADBContext db;
        public DesignerVacanteController()
        {
            db = new SAGADBContext();
        }
        [HttpGet]
        [Route("get")]
        public IHttpActionResult Action()
        {
            var datos = db.Requisiciones.Select(e => new
            {
                e.Id
                , e.VBtra
                , e.Experiencia
                //, e.Area
                , e.TipoReclutamiento
                , Actividad = db.ActividadesRequis.Where(a => a.RequisicionId == e.Id).
                Select(a => a.Actividades).FirstOrDefault()
            }).ToList();
            return Ok(datos);
        }

        private bool ValidaHaycampos(int SubSesionID)
        {
            var estructura = db.Estructuras.Where(e => e.IdPadre == SubSesionID && e.TipoEstructuraId == 8 && e.TipoMovimientoId == 3).ToList();
            var lista = estructura.Select(e => e.Id).ToList();
            var ConfiguracionesMov = db.ConfiguracionesMov.Where(e => lista.Contains(e.EstructuraId)).ToList();
            bool bandera = false;
            foreach (var item in ConfiguracionesMov)
            {
                if (item.esPublicable)
                {
                    bandera = true;
                }
            }
            return bandera;
        }

        [HttpGet]
        [Route("getGenerales")]
        public IHttpActionResult General(Guid Requi)
        {
            var ConfiguracionesMov = db.ConfiguracionesMov.ToList();
            var CfgRequi = db.CfgRequi.ToList();
            List<listadoEstru> lista = new List<listadoEstru>();
            var datos = db.Estructuras.Where(a => a.Activo == true
                                               && a.TipoEstructuraId == 8
                                               && a.TipoMovimientoId == 3
                                            ).OrderBy(e => e.Orden).ToList();
            var configura = db.ConfiguracionRequis.Where(e => e.RequisicionId == Requi).ToList();
            var requi = db.Requisiciones.Where(r => r.Id.Equals(Requi)).Select(r => new
            {
                r.Id,
                r.Experiencia,
                r.Folio,
                r.fch_Aprobacion,
                r.fch_Creacion,
                r.fch_Cumplimiento,
                r.fch_Limite,
                r.fch_Modificacion,
                //r.Genero.genero,
                //r.Aprobada,
                //r.Aprobador,
                ActividadesRequis = db.ActividadesRequis.Where(a => a.RequisicionId.Equals(r.Id)).Select(a => a.Actividades).ToList(),
                r.Area.areaExperiencia,
                AptitudesRequis = db.AptitudesRequis.Where(a => a.RequisicionId.Equals(r.Id)).Select(a => a.Aptitud.aptitud).ToList(),
                //r.Asignada,
                beneficiosRequi = db.BeneficiosRequis.Where(b => b.RequisicionId.Equals(r.Id)).Select(b => new
                { b.TipoBeneficio.tipoBeneficio, b.Cantidad, b.Observaciones
                }).ToList(),
                //r.ClaseReclutamiento.clasesReclutamiento,
                r.Cliente.Nombrecomercial,
                logotipo = r.Cliente.Foto,
                r.Cliente.RFC,
                r.Cliente.GiroEmpresas.giroEmpresa,
                r.Cliente.ActividadEmpresas.actividadEmpresa,
                r.Cliente.RazonSocial,
                CompetenciasArea = db.CompetenciasAreaRequis.Where(c => c.RequisicionId.Equals(r.Id)).Select(c => new
                {
                    c.Nivel,
                    c.Competencia.competenciaArea
                }).ToList(),
                CompetenciasCardinal = db.CompetenciasCardinalRequis.Where(c => c.RequisicionId.Equals(r.Id)).Select(c => new
                {
                    c.Nivel,
                    c.Competencia.competenciaCardinal
                }).ToList(),
                CompetenciaGerencial = db.CompetetenciasGerencialRequis.Where(c => c.RequisicionId.Equals(r.Id)).Select(c => new
                {
                    c.Nivel,
                    c.Competencia.competenciaGerencial
                }).ToList(),
                //r.Confidencial,
                r.ContratoInicial.periodoPrueba,
                r.ContratoInicial.tipoContrato,
                Contactos = db.Clientes.Where(c => c.Id.Equals(r.ClienteId)).Select(c => c.Contactos.Select(s => new
                {
                    Nombre = s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno,
                    s.Puesto,
                    s.TipoEntidad.tipoEntidad,
                    extension = db.Telefonos.Where(e => e.EntidadId.Equals(s.Id)).Select(f => f.Extension).FirstOrDefault(),
                    Telefono = db.Telefonos.Where(t => t.EntidadId.Equals(s.Id)).Select(e => e.telefono).FirstOrDefault(),
                    Email = db.Emails.Where(e => e.EntidadId.Equals(s.Id)).Select(m => m.email).FirstOrDefault()
                })).ToList(),
                DiaCorte = r.DiaCorte.diaSemana,
                DiaPago = r.DiaPago.diaSemana,
                r.DiasEnvio,
                Direccion = db.Direcciones.Where(d => d.Id.Equals(r.DireccionId)).Select(d => new
                {
                    d.Calle,
                    d.Colonia.colonia,
                    d.Municipio.municipio,
                    d.Estado.estado,
                    d.Pais.pais,
                    d.CodigoPostal,
                    d.NumeroInterior,
                    d.NumeroExterior,
                    d.TipoDireccion.tipoDireccion,
                    d.esPrincipal,
                    d.Activo
                }).FirstOrDefault(),
                DocumentosCliente = db.DocumentosClienteRequis.Where(d => d.RequisicionId.Equals(r.Id)).Select(d => d.Documento).ToList(),
                DocumentosDamsa = db.DocumentosDamsa.Select(d => d.documentoDamsa).ToList(),
                //r.EdadMaxima,
                //r.EdadMinima,
                EscolaridadesRequi = db.EscolaridadesRequis.Where(e => e.RequisicionId.Equals(r.Id)).Select(e => new
                {
                    e.Escolaridad.gradoEstudio,
                    e.EstadoEstudio.estadoEstudio
                }).ToList(),
                //r.Especifique,
                //r.EstadoCivil.estadoCivil,
                Estatus = r.Estatus.Descripcion,
                //EstatusTpMov = r.Estatus.TipoMovimiento,
                //r.FlexibilidadHorario,
                horariosRequi = db.HorariosRequis.Where(h => h.RequisicionId.Equals(r.Id)).Select(h => new
                {
                    h.Nombre,
                    deDia = h.deDia.diaSemana,
                    aDia = h.aDia.diaSemana,
                    h.deHora,
                    h.aHora,
                    h.numeroVacantes,
                    h.Especificaciones,
                    h.Activo
                }).ToList(),
                r.PeriodoPago.periodoPago,
                ObservacionesRequi = db.ObservacionesRequis.Where(o => o.RequisicionId.Equals(r.Id)).Select(o => o.Observaciones).ToList(),
                PrestacionesRequi = db.PrestacionesClienteRequis.Where(p => p.RequisicionId.Equals(r.Id)).Select(p => p.Prestamo).ToList(),
                prestacionLey = db.PrestacionesLey.Select(p => p.prestacionLey).ToList(),
                Prioridad = r.Prioridad.Descripcion,
                ProcesoRequi = db.ProcesoRequis.Where(p => p.RequisicionId.Equals(r.Id)).Select(p => new
                {
                    p.Orden,
                    p.Proceso
                }).ToList(),
                //r.Propietario,
                PsicoCliente = db.PsicometriasClienteRequis.Where(p => p.RequisicionId.Equals(r.Id)).Select(p => new
                {
                    p.Psicometria,
                    p.Descripcion
                }).ToList(),
                PsicoDamsa = db.PsicometriasDamsaRequis.Where(p => p.RequisicionId.Equals(r.Id)).Select(p => p.Psicometria).ToList(),
                r.SueldoMaximo,
                r.SueldoMinimo,
                telefono = db.Clientes.Where(t => t.Id.Equals(r.ClienteId)).Select(t => t.telefonos.Select(e => new
                {
                    e.TipoTelefono.Tipo,
                    e.telefono,
                    e.Extension,
                    e.Activo,
                    e.esPrincipal
                })).ToList(),
                //TCOrden = r.TiempoContrato.Orden,
                TCTiempo = r.TiempoContrato.Tiempo,
                //TMModalidad = r.TipoModalidad.Modalidad,
                //TMOrden = r.TipoModalidad.Orden,
                r.TipoNomina.tipoDeNomina,
                r.TipoReclutamiento.tipoReclutamiento,
                r.VBtra
            }
            ).FirstOrDefault();
            foreach (var item in datos)
            {
                listadoEstru pieza = new listadoEstru();
                pieza.Id = item.Id;
                pieza.idPadre = item.IdPadre;
                pieza.Nombre = item.Nombre;
                pieza.Descripcion = item.Descripcion;
                pieza.Activo = item.Activo;
                pieza.Confidencial = item.Confidencial;
                pieza.Icono = item.Icono;
                pieza.Resumen = false;
                pieza.Detalle = false;
                pieza.Publica = false;
                pieza.Requi = requi;
                try
                {
                    pieza.Resumen = CfgRequi.Where(e => e.ConfigMovId == ConfiguracionesMov.Where(a => a.EstructuraId == item.Id).FirstOrDefault().Id).FirstOrDefault().R;
                    pieza.Detalle = CfgRequi.Where(e => e.ConfigMovId == ConfiguracionesMov.Where(a => a.EstructuraId == item.Id).FirstOrDefault().Id).FirstOrDefault().D;
                }
                catch (Exception)
                {
                }
                if (ConfiguracionesMov.Count > 0)
                {
                    if (ConfiguracionesMov.Where(a => a.EstructuraId == item.Id).Count() > 0)
                    {
                        pieza.Publica = ConfiguracionesMov.Where(e => e.EstructuraId == item.Id).FirstOrDefault().esPublicable;
                    }
                }


                if (configura.Count > 0)
                {
                    pieza.Resumen = configura.Where(e => e.IdEstructura == item.Id).Select(e => e.Resumen).FirstOrDefault();
                    pieza.Detalle = configura.Where(e => e.IdEstructura == item.Id).Select(e => e.Detalle).FirstOrDefault();
                }
                lista.Add(pieza);
            }

            return Ok(lista);
        }

        [HttpGet]
        [Route("getCampos")]
        public IHttpActionResult Campos()
        {

            var datos = db.Estructuras.Where(a =>
                                                a.TipoEstructuraId == 8
                                                && a.TipoMovimientoId == 3
                                                && a.Activo == true
                                            ).OrderBy(e => e.Orden).ToList();
            return Ok(datos);
        }

        [HttpGet]
        [Route("getClasificaciones")]
        public IHttpActionResult Clasificaciones()
        {
            var datos = db.Estructuras.Where(a =>
                                              a.TipoEstructuraId == 7
                                                && a.TipoMovimientoId == 3
                                                && a.Activo == true
                                            ).OrderBy(e => e.Id).ToList();
            List<listadoEstru> lista = new List<listadoEstru>();
            foreach (var item in datos)
            {
                listadoEstru pieza = new listadoEstru();
                pieza.Id = item.Id;
                pieza.idPadre = item.IdPadre;
                pieza.Nombre = item.Nombre;
                pieza.Descripcion = item.Descripcion;
                pieza.Activo = item.Activo;
                pieza.Confidencial = item.Confidencial;
                pieza.Icono = item.Icono;
                pieza.Resumen = false;
                pieza.Detalle = false;
                pieza.Publica = ValidaHaycampos(item.Id);
                lista.Add(pieza);
            }
            return Ok(lista);
        }



        [HttpPost]
        [Route("GuardarVacante")]
        public IHttpActionResult GuardarVacante(List<listaPublicar> ListadoJson)
        {
            string mensaje = "Publicacion Exitosa, configuracion guardada";
            bool bandera = true;
            try
            {
                var requi = db.ConfiguracionRequis.ToList();
                Guid idRequi = ListadoJson.Select(a => a.id).FirstOrDefault();
                var datos = db.ConfiguracionRequis.Where(e => e.RequisicionId == idRequi).ToList();

                if (datos.Count < ListadoJson.Count)
                {
                    var listaID = datos.Select(e => e.IdEstructura).ToList();
                    var diferente = ListadoJson.Where(e => !listaID.Contains(e.idCampo)).ToList();
                    foreach (var item in diferente)
                    {
                        ConfiguracionRequi caja = new ConfiguracionRequi();

                        // caja.id = Guid.NewGuid();
                        caja.Campo = item.nombre;
                        caja.RequisicionId = item.id;
                        caja.Detalle = item.detalle;
                        caja.Resumen = item.resumen;
                        caja.R_D = ResumenDetalle(item.resumen, item.detalle);
                        caja.IdEstructura = item.idCampo;
                        db.ConfiguracionRequis.Add(caja);
                        db.SaveChanges();
                    }
                    datos = db.ConfiguracionRequis.Where(e => e.RequisicionId == idRequi).ToList();
                }

                foreach (var item in ListadoJson)
                {
                    var lista = datos.Where(e => e.IdEstructura == item.idCampo).FirstOrDefault();
                    lista.Detalle = item.detalle;
                    lista.Resumen = item.resumen;
                    lista.R_D = ResumenDetalle(item.resumen, item.detalle);
                    db.SaveChanges();
                }
                var EstatusRequi = db.Requisiciones.Where(e => e.Id == idRequi).FirstOrDefault();
                //EstatusRequi.EstatusId = 7;
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                bandera = false;
            }
            var obj = new { Mensaje = mensaje, Bandera = bandera };
            return Ok(obj);
        }

        [HttpPost]
        [Route("updatePublicar")]
        public IHttpActionResult PublicarVacante(UpdatePublicarDto ListadoJson)
        {
            string mensaje = "Publicacion Exitosa, configuracion guardada";
            bool bandera = true;

            try
            {

                if (ListadoJson.RequiId == null || ListadoJson.RequiId == "")
                {
                    var listadoJson = ListadoJson.ListaPublicar;
                    var requi = db.ConfiguracionRequis.ToList();
                    Guid idRequi = listadoJson.Select(a => a.id).FirstOrDefault();
                    var datos = db.ConfiguracionRequis.Where(e => e.RequisicionId == idRequi).ToList();

                    if (datos.Count < listadoJson.Count)
                    {
                        var listaID = datos.Select(e => e.IdEstructura).ToList();
                        var diferente = listadoJson.Where(e => !listaID.Contains(e.idCampo)).ToList();
                        foreach (var item in diferente)
                        {
                            ConfiguracionRequi caja = new ConfiguracionRequi();

                            // caja.id = Guid.NewGuid();
                            caja.Campo = item.nombre;
                            caja.RequisicionId = item.id;
                            caja.Detalle = item.detalle;
                            caja.Resumen = item.resumen;
                            caja.R_D = ResumenDetalle(item.resumen, item.detalle);
                            caja.IdEstructura = item.idCampo;
                            db.ConfiguracionRequis.Add(caja);
                            db.SaveChanges();
                        }
                        datos = db.ConfiguracionRequis.Where(e => e.RequisicionId == idRequi).ToList();
                    }

                    foreach (var item in listadoJson)
                    {
                        var lista = datos.Where(e => e.IdEstructura == item.idCampo).FirstOrDefault();
                        lista.Detalle = item.detalle;
                        lista.Resumen = item.resumen;
                        lista.R_D = ResumenDetalle(item.resumen, item.detalle);
                        db.SaveChanges();
                    }
                    var EstatusRequi = db.Requisiciones.Where(e => e.Id == idRequi).FirstOrDefault();
                    EstatusRequi.EstatusId = 7;
                    db.SaveChanges();
                }
                else
                {
                    
                    Guid Requi = new Guid(ListadoJson.RequiId);
                    var listas = db.ConfiguracionRequis.Where(e => e.RequisicionId == Requi).ToList();
                    if (listas.Count > 1)
                    {
                        var obj2 = new { Mensaje = mensaje, Bandera = bandera };
                        return Ok(obj2);
                    }
                    var CfgRequi = db.CfgRequi.ToList();
                    var datos2 = db.Estructuras.Where(a => a.Activo == true
                                                      && a.TipoEstructuraId == 8
                                                      && a.TipoMovimientoId == 3
                                                   ).Select(e => new { e.Id, e.Nombre, IdMov = db.ConfiguracionesMov.Where(x=>x.EstructuraId == e.Id).FirstOrDefault().Id, e.Orden }).OrderBy(e => e.Orden).ToList();
                    foreach (var item in CfgRequi.Where(e=> datos2.Select(x=>x.IdMov).ToList().Contains(e.Id)))
                    {
                        ConfiguracionRequi pieza = new ConfiguracionRequi();
                        pieza.IdEstructura = datos2.Where(e => e.IdMov == item.Id).Select(e => e.Id).FirstOrDefault();
                        pieza.Resumen = item.R;
                        pieza.Detalle = item.D;
                        pieza.R_D = item.R_D;
                        pieza.Campo = datos2.Where(e => e.IdMov == item.Id).Select(e=>e.Nombre).FirstOrDefault();
                        pieza.RequisicionId = Requi;
                        var add = db.ConfiguracionRequis.Add(pieza);
                        db.SaveChanges();
                    }
                    var requilis = db.Requisiciones.Where(e => e.Id == Requi).FirstOrDefault();
                    requilis.Publicado = true;
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                bandera = false;
            }
            var obj = new { Mensaje = mensaje, Bandera = bandera };
            return Ok(obj);
        }

        [HttpGet]
        [Route("setResumen")]
        public IHttpActionResult ActualizarResumen(Guid Requi, int Idcampo, bool resumen)
        {
            string mensaje;
            bool bandera;
            try
            {
                var datos = db.ConfiguracionRequis.Where(e => e.RequisicionId == Requi).ToList();
                if (datos.Count == 0)
                {
                    var estructura = db.Estructuras.Where(e => e.TipoEstructuraId == 7 ).ToList();
                    foreach (var item in estructura)
                    {
                        ConfiguracionRequi caja = new ConfiguracionRequi();
                        caja.Campo = item.Nombre;
                        caja.RequisicionId = Requi;
                        caja.Detalle = false;
                        caja.Resumen = false;
                        caja.R_D = 0;
                        caja.IdEstructura = item.Id;
                        db.ConfiguracionRequis.Add(caja);
                        db.SaveChanges();
                    }
                    datos = db.ConfiguracionRequis.Where(e => e.RequisicionId == Requi).ToList();
                }

                var listaConfi = db.ConfiguracionRequis.Where(e => e.RequisicionId == Requi && e.IdEstructura == Idcampo).ToList();
                if (listaConfi.Count == 0)
                {
                    var estructura = db.Estructuras.Where(e => e.TipoEstructuraId == 7 && e.Id == Idcampo).FirstOrDefault();
                    ConfiguracionRequi caja = new ConfiguracionRequi();
                    caja.Campo = estructura.Nombre;
                    caja.RequisicionId = Requi;
                    caja.Detalle = false;
                    caja.Resumen = false;
                    caja.R_D = 0;
                    caja.IdEstructura = Idcampo;
                    db.ConfiguracionRequis.Add(caja);
                    db.SaveChanges();
                    datos = db.ConfiguracionRequis.Where(e => e.RequisicionId == Requi).ToList();
                }
                var lista = datos.Where(e => e.IdEstructura == Idcampo).FirstOrDefault();
                lista.Resumen = resumen;
                lista.R_D = Bandera(Requi, Idcampo);
            //    db.SaveChanges();
                mensaje = "Exito al guardar tu configuracion";
                bandera = true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                bandera = false;
            }
            var obj = new { Mensaje = mensaje, Bandera = bandera };
            return Ok(obj);
        }

        [HttpGet]
        [Route("setDetalle")]
        public IHttpActionResult ActualizarDetalle(Guid Requi, int Idcampo, bool detalle)
        {
            string mensaje;
            bool bandera;
            try
            {
                var datos = db.ConfiguracionRequis.Where(e => e.RequisicionId == Requi).ToList();
                if (datos.Count == 0)
                {
                    var estructura = db.Estructuras.Where(e => e.TipoEstructuraId == 7).ToList();
                    foreach (var item in estructura)
                    {
                        ConfiguracionRequi caja = new ConfiguracionRequi();
                        caja.Campo = item.Nombre;
                        caja.RequisicionId = Requi;
                        caja.Detalle = false;
                        caja.Resumen = false;
                        caja.R_D = 0;
                        caja.IdEstructura = item.Id;
                        db.ConfiguracionRequis.Add(caja);
                        db.SaveChanges();
                    }
                    datos = db.ConfiguracionRequis.Where(e => e.RequisicionId == Requi).ToList();
                }
                var listaConfi = db.ConfiguracionRequis.Where(e => e.RequisicionId == Requi && e.IdEstructura == Idcampo).ToList();
                if (listaConfi.Count == 0)
                {
                    var estructura = db.Estructuras.Where(e => e.TipoEstructuraId == 7 && e.Id == Idcampo).FirstOrDefault();
                    ConfiguracionRequi caja = new ConfiguracionRequi();
                    caja.Campo = estructura.Nombre;
                    caja.RequisicionId = Requi;
                    caja.Detalle = false;
                    caja.Resumen = false;
                    caja.R_D = 0;
                    caja.IdEstructura = Idcampo;
                    db.ConfiguracionRequis.Add(caja);
                    db.SaveChanges();
                    datos = db.ConfiguracionRequis.Where(e => e.RequisicionId == Requi).ToList();
                }

                var lista = datos.Where(e => e.IdEstructura == Idcampo).FirstOrDefault();
                lista.Detalle = detalle;
                lista.R_D = Bandera(Requi, Idcampo);
             //   db.SaveChanges();
                mensaje = "Exito al guardar tu configuracion";
                bandera = true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                bandera = false;
            }
            var obj = new { Mensaje = mensaje, Bandera = bandera };
            return Ok(obj);
        }




        [HttpGet]
        [Route("getContrato")]
        public IHttpActionResult Contrato(Guid Requi)
        {
            var datos = Listado(2, Requi);
            return Ok(datos);
        }


        [HttpGet]
        [Route("getPuestoReclutar")]
        public IHttpActionResult PuestoReclutar(Guid Requi)
        {
            var datos = Listado(3, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getHorario")]
        public IHttpActionResult Horario(Guid Requi)
        {
            var datos = Listado(4, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getsueldo")]
        public IHttpActionResult sueldo(Guid Requi)
        {
            var datos = Listado(5, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getOtros")]
        public IHttpActionResult Otros(Guid Requi)
        {
            var datos = Listado(6, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getActividad")]
        public IHttpActionResult getActividad(Guid Requi)
        {
            var datos = Listado(7, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getBeneficio")]
        public IHttpActionResult getBeneficio(Guid Requi)
        {
            var datos = Listado(8, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getDireccion")]
        public IHttpActionResult getDireccion(Guid Requi)
        {
            var datos = Listado(9, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getTelefono")]
        public IHttpActionResult getTelefono(Guid Requi)
        {
            var datos = Listado(10, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getContacto")]
        public IHttpActionResult getContacto(Guid Requi)
        {
            var datos = Listado(11, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getPsicometria")]
        public IHttpActionResult getPsicometria(Guid Requi)
        {
            var datos = Listado(12, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getDocumento")]
        public IHttpActionResult getDocumentos(Guid Requi)
        {
            var datos = Listado(13, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getProceso")]
        public IHttpActionResult getProceso(Guid Requi)
        {
            var datos = Listado(14, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getCopetencia")]
        public IHttpActionResult getCopetencia(Guid Requi)
        {
            var datos = Listado(15, Requi);
            return Ok(datos);
        }

        [HttpGet]
        [Route("getUbicacion")]
        public IHttpActionResult getUbicacion(Guid Requi)
        {
            var datos = Listado(16, Requi);
            return Ok(datos);
        }
        private int ResumenDetalle(bool resumen, bool detalle)
        {

            if (resumen == true && detalle == true)
            {
                return 3;
            }

            if (resumen == false && detalle == true)
            {
                return 2;
            }

            if (resumen == true && detalle == false)
            {
                return 1;
            }

            return 0;
        }

        private int Bandera(Guid Requi, int Idcampo)
        {
            var datos = db.ConfiguracionRequis.Where(e => e.RequisicionId == Requi && e.IdEstructura == Idcampo).FirstOrDefault();
            if (datos.Resumen == true && datos.Detalle == true)
            {
                return 3;
            }

            if (datos.Resumen == false && datos.Detalle == true)
            {
                return 2;
            }

            if (datos.Resumen == true && datos.Detalle == false)
            {
                return 1;
            }

            return 0;
        }

        private List<listadoEstru> Listado(int Orden, Guid Requi)
        {

            List<listadoEstru> lista = new List<listadoEstru>();
            var datos = db.Estructuras.Where(a => a.Activo == true
                                                && a.TipoEstructuraId == 8
                                                && a.TipoMovimientoId == 3
                                             ).OrderBy(e=>e.Orden).ToList();
            var configuracion = db.ConfiguracionRequis.Where(e => e.RequisicionId == Requi).ToList();
            foreach (var item in datos)
            {
                listadoEstru pieza = new listadoEstru();
                pieza.Id = item.Id;
                pieza.idPadre = item.IdPadre;
                pieza.Nombre = item.Nombre;
                pieza.Descripcion = item.Descripcion;
                pieza.Activo = item.Activo;
                pieza.Confidencial = item.Confidencial;
           //     pieza.DescripcionInclusivo = item.DescripcionInclusivo;
           //     pieza.Inclusivo = item.Inclusivo;
                pieza.Icono = item.Icono;
                pieza.Resumen = false;
                pieza.Detalle = false;
                if (configuracion.Count > 0)
                {
                    pieza.Resumen = configuracion.Where(e => e.IdEstructura == item.Id).Select(e => e.Resumen).FirstOrDefault();
                    pieza.Detalle = configuracion.Where(e => e.IdEstructura == item.Id).Select(e => e.Detalle).FirstOrDefault();
                }

                lista.Add(pieza);
            }
            return lista;
        }
        public class listadoEstru
        {
            public int Id { get; set; }
            public int idPadre { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public bool Activo { get; set; }
            public bool Confidencial { get; set; }
            public string DescripcionInclusivo { get; set; }
            public bool Inclusivo { get; set; }
            public string Icono { get; set; }
            public bool? Resumen { get; set; }
            public bool? Detalle { get; set; }
            public bool? Publica { get; set; }
            public object Requi { get; set; }
        }

        public class listaPublicar
        {
            public string nombre { get; set; }
            public bool detalle { get; set; }
            public bool resumen { get; set; }
            public int idCampo { get; set; }
            public Guid id { get; set; }
        }

        public class UpdatePublicarDto
        {
            public List<listaPublicar> ListaPublicar { get; set; }
            public string RequiId { get; set; }
        }
    }
}
