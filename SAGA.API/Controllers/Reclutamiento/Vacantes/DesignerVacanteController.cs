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

        [HttpGet]
        [Route("getCampos")]
        public IHttpActionResult Campos()
        {
            var datos = db.Estructuras.Where(a =>
                                                a.Activo == true
                                                && a.TipoEstructuraId == 8
                                                && a.TipoMovimientoId == 3
                                                && a.Activo == true
                                            ).OrderBy(e => e.Id).ToList();
            return Ok(datos);
        }

        [HttpGet]
        [Route("getClasificaciones")]
        public IHttpActionResult Clasificaciones()
        {
            var datos = db.Estructuras.Where(a =>
                                                a.Activo == true
                                                && a.TipoEstructuraId == 7
                                                && a.TipoMovimientoId == 3
                                                && a.Activo == true
                                            ).OrderBy(e => e.Id).ToList();
            return Ok(datos);
        }


        [HttpPost]
        [Route("updatePublicar")]
        public IHttpActionResult PublicarVacante(List<listaPublicar> ListadoJson)
        {
            string mensaje = "Publicacion Exitosa, configuracion guardada";
            bool bandera = true;
            try
            {
                var requi = db.ConfiguracionRequis.ToList();
                Guid idRequi = ListadoJson.Select(a => a.id).FirstOrDefault();
                var datos = db.ConfiguracionRequis.Where(e => e.IdRequi == idRequi).ToList();

                if (datos.Count < ListadoJson.Count)
                {
                    var listaID = datos.Select(e => e.IdEstructura).ToList();
                    var diferente = ListadoJson.Where(e => !listaID.Contains(e.idCampo)).ToList();
                    foreach (var item in diferente)
                    {
                        ConfiguracionRequi caja = new ConfiguracionRequi();
                        caja.Campo = item.nombre;
                        caja.IdRequi = item.id;
                        caja.Detalle = item.detalle;
                        caja.Resumen = item.resumen;
                        caja.R_D = ResumenDetalle(item.resumen, item.detalle);
                        caja.IdEstructura = item.idCampo;
                        db.ConfiguracionRequis.Add(caja);
                        db.SaveChanges();
                    }
                    datos = db.ConfiguracionRequis.Where(e => e.IdRequi == idRequi).ToList();
                }

                foreach (var item in ListadoJson)
                {
                    var lista = datos.Where(e => e.IdEstructura == item.idCampo).FirstOrDefault();
                    lista.Detalle = item.detalle;
                    lista.Resumen = item.resumen;
                    lista.R_D = ResumenDetalle(item.resumen, item.detalle);
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
                var datos = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi).ToList();
                if (datos.Count == 0)
                {
                    var estructura = db.Estructuras.Where(e => e.TipoEstructuraId == 7 ).ToList();
                    foreach (var item in estructura)
                    {
                        ConfiguracionRequi caja = new ConfiguracionRequi();
                        caja.Campo = item.Nombre;
                        caja.IdRequi = Requi;
                        caja.Detalle = false;
                        caja.Resumen = false;
                        caja.R_D = 0;
                        caja.IdEstructura = item.Id;
                        db.ConfiguracionRequis.Add(caja);
                        db.SaveChanges();
                    }
                    datos = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi).ToList();
                }

                var listaConfi = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi && e.IdEstructura == Idcampo).ToList();
                if (listaConfi.Count == 0)
                {
                    var estructura = db.Estructuras.Where(e => e.TipoEstructuraId == 7 && e.Id == Idcampo).FirstOrDefault();
                    ConfiguracionRequi caja = new ConfiguracionRequi();
                    caja.Campo = estructura.Nombre;
                    caja.IdRequi = Requi;
                    caja.Detalle = false;
                    caja.Resumen = false;
                    caja.R_D = 0;
                    caja.IdEstructura = Idcampo;
                    db.ConfiguracionRequis.Add(caja);
                    db.SaveChanges();
                    datos = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi).ToList();
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
                var datos = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi).ToList();
                if (datos.Count == 0)
                {
                    var estructura = db.Estructuras.Where(e => e.TipoEstructuraId == 7).ToList();
                    foreach (var item in estructura)
                    {
                        ConfiguracionRequi caja = new ConfiguracionRequi();
                        caja.Campo = item.Nombre;
                        caja.IdRequi = Requi;
                        caja.Detalle = false;
                        caja.Resumen = false;
                        caja.R_D = 0;
                        caja.IdEstructura = item.Id;
                        db.ConfiguracionRequis.Add(caja);
                        db.SaveChanges();
                    }
                    datos = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi).ToList();
                }
                var listaConfi = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi && e.IdEstructura == Idcampo).ToList();
                if (listaConfi.Count == 0)
                {
                    var estructura = db.Estructuras.Where(e => e.TipoEstructuraId == 7 && e.Id == Idcampo).FirstOrDefault();
                    ConfiguracionRequi caja = new ConfiguracionRequi();
                    caja.Campo = estructura.Nombre;
                    caja.IdRequi = Requi;
                    caja.Detalle = false;
                    caja.Resumen = false;
                    caja.R_D = 0;
                    caja.IdEstructura = Idcampo;
                    db.ConfiguracionRequis.Add(caja);
                    db.SaveChanges();
                    datos = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi).ToList();
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
        [Route("getGenerales")]
        public IHttpActionResult General(Guid Requi)
        {
            List<listadoEstru> lista = new List<listadoEstru>();
            var datos = db.Estructuras.Where(a => a.Activo == true
                                               && a.TipoEstructuraId == 8
                                               && a.TipoMovimientoId == 3
                                            ).OrderBy(e => e.Orden).ToList();
            var configura = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi).ToList();          
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
            var datos = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi && e.IdEstructura == Idcampo).FirstOrDefault();
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
            var configuracion = db.ConfiguracionRequis.Where(e => e.IdRequi == Requi).ToList();
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
        }

        public class listaPublicar
        {
            public string nombre { get; set; }
            public bool detalle { get; set; }
            public bool resumen { get; set; }
            public int idCampo { get; set; }
            public Guid id { get; set; }
        }
    }
} 
