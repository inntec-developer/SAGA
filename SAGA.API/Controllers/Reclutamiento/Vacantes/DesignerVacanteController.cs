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
using SAGA.API.Dtos.Vacantes;
using System.IO;
using System.Data.Entity;

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
        [Route("getTitulos")]
        [Authorize]
        public IHttpActionResult GetTitulos()
        {
            try
            {
                var titulos = db.TitulosArte.Where(x => x.Activo).Select(a => new
                {
                    a.Id,
                    a.Nombre,
                    a.Descripcion,
                    a.Orden
                }).OrderBy(o => o.Orden).ToList();

                return Ok(titulos);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        [Route("CRUDArte")]
        [Authorize]
        public IHttpActionResult CRUDArte(ArteDto arte)
        {
            try
            {
                if (arte.crud == "C")
                {
                    foreach (ArteRequi ar in arte.ArteRequiList)
                    {
                        ar.fch_Modificacion = DateTime.Now;
                        ar.Ruta = "img/ArteRequi/Arte/" + ar.Ruta;
                    }
                    db.ArteRequi.AddRange(arte.ArteRequiList);
                    db.SaveChanges();

                    this.GuardarArte(arte);

                    return Ok(HttpStatusCode.OK);
                }
                else if (arte.crud == "R")
                {
                    var ar = db.ArteRequi.Where(x => x.RequisicionId.Equals(arte.requisicionId) && x.Activo).Select(a => new
                    {
                        arterequiId = a.Id,
                        id = a.TitulosArteId,
                        nombre = a.TitulosArte.Nombre,
                        descripcion = a.Contenido,
                        a.TitulosArte.Orden,
                        a.Ruta,
                        a.BG
                    }).OrderBy(o => o.Orden).ToList();

                    var bg = db.Requisiciones.Where(x => x.Id.Equals(arte.requisicionId)).Select(d => d.DAMFO290.Arte).FirstOrDefault();
                   
                    return Ok(new { datos = ar, bg });
                   
                }
                else if (arte.crud == "U")
                {
                    var apt = db.ArteRequi.Where(x => x.RequisicionId.Equals(arte.requisicionId));
                    db.ArteRequi.RemoveRange(apt);

                    foreach (ArteRequi ar in arte.ArteRequiList)
                    {
                        ar.fch_Modificacion = DateTime.Now;
                        ar.Ruta = "img/ArteRequi/Arte/" + ar.Ruta;
                    }

                    db.ArteRequi.AddRange(arte.ArteRequiList);
                    db.SaveChanges();

                    this.GuardarArte(arte);
                    return Ok(HttpStatusCode.OK);
                }
                else if (arte.crud == "D")
                {
                    var apt = db.ArteRequi.Where(x => x.RequisicionId.Equals(arte.requisicionId));
                    db.ArteRequi.RemoveRange(apt);

                    string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/ArteRequi/Arte/" + arte.requisicionId.ToString() + ".png");

                    if (File.Exists(fullPath)) 
                        File.Delete(fullPath);

                    db.SaveChanges();
                    return Ok(HttpStatusCode.OK);
                }
                else
                {
                    return Ok(HttpStatusCode.Continue);
                }
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        [Route("guardarArte")]
        public IHttpActionResult GuardarArte(ArteDto Arte)
        {
            try
            {
                string x = Arte.arte.Replace("data:image/png;base64,", "");
                byte[] imageBytes = Convert.FromBase64String(x);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);

                string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/ArteRequi/Arte/" + Arte.requisicionId.ToString() + ".png");

                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                image.Save(fullPath);

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
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
            List<string> titulos = new List<string> {
                "datos de la vacante", "contrato", "direcciones", "puesto a reclutar", "horarios" , "beneficios", "puesto",
                "sueldos", "documentacion", "prestaciones"};
            List<string> campos = new List<string>() {
                "folio", "tipo", "dias de prueba", "estado", "municipio", "nivel", "escolaridad",
                "aptitudes",
                "experiencia",
               "nombre" ,
               "de" ,
                "a",
               "desde",
               "hasta",
               "especificaciones",
               "nombre",
               "cantidad",
               "observaciones",
               "actividades",
               "periodo",
               "nomina",
               "sueldo minimo",
               "sueldo maximo",
               "damsa",
               "de ley",
               "prestaciones", "superiores" };
            var papas = db.Estructuras.Where(a => titulos.Contains(a.Nombre.ToLower()) &&
                                          a.TipoEstructuraId == 7
                                            && a.TipoMovimientoId == 3
                                            && a.Activo == true
                                        ).Select(p => p.Id).ToList();
            var datos = db.Estructuras.Where(a => a.Activo == true && papas.Contains(a.IdPadre)
                                               && a.TipoEstructuraId == 8
                                               && a.TipoMovimientoId == 3
                                               && campos.Contains(a.Nombre.ToLower())
                                            ).Select(e => new {
                                                e.Id,
                                                e.IdPadre,
                                                e.Nombre,
                                                e.Orden,
                                                e.TipoEstructuraId,
                                                e.TipoMovimientoId,
                                                e.Descripcion,
                                                papa = db.Estructuras.Where(x => x.Id.Equals(e.IdPadre)).Select(n => n.Nombre).FirstOrDefault()
                                            }).OrderBy(e => e.Orden).ToList();
            var configura = db.ConfiguracionRequis.Where(e => e.RequisicionId == Requi).ToList();
            var requi = db.Requisiciones.Where(r => r.Id.Equals(Requi)).Select(r => new
            {
                r.Id,
                r.Experiencia,
                r.Folio,
                ActividadesRequis = db.ActividadesRequis.Where(a => a.RequisicionId.Equals(r.Id)).Select(a => a.Actividades).ToList(),
                r.Area.areaExperiencia,
                AptitudesRequis = db.AptitudesRequis.Where(a => a.RequisicionId.Equals(r.Id)).Select(a => a.Aptitud.aptitud).ToList(),
                beneficiosRequi = db.BeneficiosRequis.Where(b => b.RequisicionId.Equals(r.Id)).Select(b => new
                {
                    b.TipoBeneficio.tipoBeneficio,
                    b.Cantidad, b.Observaciones
                }).ToList(),
                r.ContratoInicial.periodoPrueba,
                r.ContratoInicial.tipoContrato,
                DiaCorte = r.DiaCorte.diaSemana,
                DiaPago = r.DiaPago.diaSemana,
                Direccion = db.Direcciones.Where(d => d.Id.Equals(r.DireccionId) && d.Activo && d.esPrincipal).Select(d => new
                {
                    d.Municipio.municipio,
                    d.Estado.estado
                }).FirstOrDefault(),
                DocumentosDamsa = db.DocumentosDamsa.Select(d => d.documentoDamsa).ToList(),
                EscolaridadesRequi = db.EscolaridadesRequis.Where(e => e.RequisicionId.Equals(r.Id)).Select(e => new
                {
                    e.Escolaridad.gradoEstudio,
                    e.EstadoEstudio.estadoEstudio
                }).ToList(),
                Estatus = r.Estatus.Descripcion,
                horariosRequi = db.HorariosRequis.Where(h => h.RequisicionId.Equals(r.Id) && h.Activo).Select(h => new
                {
                    h.Nombre,
                    deDia = h.deDia.diaSemana,
                    aDia = h.aDia.diaSemana,
                    h.deHora,
                    h.aHora,
                    h.Especificaciones,
                }).ToList(),
                r.PeriodoPago.periodoPago,
                PrestacionesRequi = db.PrestacionesClienteRequis.Where(p => p.RequisicionId.Equals(r.Id)).Select(p => p.Prestamo).ToList(),
                prestacionLey = db.PrestacionesLey.Select(p => p.prestacionLey).ToList(),
                r.SueldoMaximo,
                r.SueldoMinimo,
                TCTiempo = r.TiempoContrato.Tiempo,
                r.TipoNomina.tipoDeNomina,
                r.VBtra
            }).FirstOrDefault();
            foreach (var item in datos)
            {
                if ((item.papa.ToLower() == "direcciones" && item.Nombre.ToLower().Equals("tipo")) || (item.papa.ToLower().Equals("puesto") && item.Nombre.ToLower().Equals("observaciones")))
                { }
                else { 
                    listadoEstru pieza = new listadoEstru();
                    pieza.Id = item.Id;
                    pieza.idPadre = item.IdPadre;
                    pieza.Nombre = item.Nombre;
                    pieza.Descripcion = item.Descripcion;
                    //pieza.Activo = item.Activo;
                    //pieza.Confidencial = item.Confidencial;
                    //pieza.Icono = item.Icono;
                    pieza.Resumen = false;
                    pieza.Detalle = false;
                    pieza.Publica = false;
                    //pieza.Requi = requi;
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
               
            }

            return Ok(new { lista, requi });
        }

        [HttpGet]
        [Route("getCampos")]
        public IHttpActionResult Campos()
        {
            List<string> titulos = new List<string> {
                "datos de la vacante", "contrato", "direcciones", "puesto a reclutar", "horarios" , "beneficios", "puesto",
                "sueldos", "documentacion", "prestaciones"};
            List<string> campos = new List<string>() {
                "folio", "tipo", "dias de prueba", "estado", "municipio", "nivel", "escolaridad",
                "aptitudes",
                "experiencia",
               "nombre" ,
               "de" ,
                "a",
               "desde",
               "hasta",
               "especificaciones",
               "nombre",
               "cantidad",
               "observaciones",
               "actividades",
               "periodo",
               "nomina",
               "sueldo minimo",
               "sueldo maximo",
               "damsa",
               "de ley",
               "prestaciones", "superiores" };

            var papas = db.Estructuras.Where(a => titulos.Contains(a.Nombre.ToLower()) &&
                                          a.TipoEstructuraId == 7
                                            && a.TipoMovimientoId == 3
                                            && a.Activo == true
                                        ).Select(p => p.Id).ToList();

            var datos = db.Estructuras.Where(a => papas.Contains(a.IdPadre) &&
                                                a.TipoEstructuraId == 8
                                                && a.TipoMovimientoId == 3
                                                && a.Activo == true
                                                && campos.Contains(a.Nombre.ToLower())
                                            ).OrderBy(e => e.Orden).ToList();
            return Ok(datos);
        }

        [HttpGet]
        [Route("getClasificaciones")]
        public IHttpActionResult Clasificaciones()
        {
            List<string> titulos = new List<string> {
                "datos de la vacante", "contrato", "direcciones", "puesto a reclutar", "horarios" , "beneficios", "puesto",
                "sueldos", "documentacion", "prestaciones"};
            var datos = db.Estructuras.Where(a => titulos.Contains(a.Nombre.ToLower()) &&
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
                        caja.Fch_Modificacion = DateTime.Now;
                        caja.UsuarioId = item.usuarioId;
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
                            caja.UsuarioId = item.usuarioId;
                            caja.Fch_Modificacion = DateTime.Now;
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
                        pieza.UsuarioId = ListadoJson.usuarioId;
                        pieza.Fch_Modificacion = DateTime.Now;

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
            //falta poner el usuario y fecha
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
           public Guid usuarioId { get; set; }
            public Guid id { get; set; }
        }

        public class UpdatePublicarDto
        {
            public List<listaPublicar> ListaPublicar { get; set; }
            public string RequiId { get; set; }
            public Guid usuarioId { get; set; }
        }
    }
}
