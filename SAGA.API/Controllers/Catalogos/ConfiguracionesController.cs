using SAGA.DAL;
using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.API.Dtos;
using SAGA.API.Controllers.Admin;
using SAGA.API.Dtos.Catalogos;
using SAGA.API.Utilerias;
using System.Data.Entity;
using System.Globalization;
using SAGA.API.Dtos.Reclutamiento.Ingresos;

namespace SAGA.API.Controllers.Catalogos
{
    [RoutePrefix("api/Configuraciones")]
    public class ConfiguracionesController : ApiController
    {
        private SAGADBContext db;
        public Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");
        public ConfiguracionesController()
        {
            db = new SAGADBContext();
        }

#region crud ingresos
        public HttpResponseMessage CRUDVacaciones(CRUDConfiguracionesDto datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    ConfigVacaciones a = new ConfigVacaciones();
                    a = datos.VacacionesPpal;
                    a.Activo = true;
                    a.fch_Modificacion = DateTime.Now;
                    db.ConfigVacaciones.Add(a);
                    db.SaveChanges();

                    foreach (ConfigVacacionesDias d in datos.VacacionesDias)
                    {
                        d.ConfigVacacionesId = a.Id;
                    }

                    db.ConfigVacacionesDias.AddRange(datos.VacacionesDias);
                    db.SaveChanges();

                    //foreach (EmpleadoVacaciones g in datos.VacacionesRelacion)
                    //{
                    //    g.ConfigVacacionesId = a.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}

                    //db.GrupoVacaciones.AddRange(datos.VacacionesRelacion);
                    //db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigVacaciones.Find(datos.VacacionesPpal.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.Nombre = datos.VacacionesPpal.Nombre;
                    a.DiasContinuos = datos.VacacionesPpal.DiasContinuos;
                    a.DiasExpiran = datos.VacacionesPpal.DiasExpiran;
                    a.DiasIncremento = datos.VacacionesPpal.DiasIncremento;
                    a.Porcentaje = datos.VacacionesPpal.Porcentaje;
                    a.Observaciones = datos.VacacionesPpal.Observaciones;
                    a.UsuarioMod = datos.VacacionesPpal.UsuarioMod;
                    a.ClienteId = datos.VacacionesPpal.ClienteId;
                    a.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();

                    var apt = db.ConfigVacacionesDias.Where(x => x.ConfigVacacionesId.Equals(datos.VacacionesPpal.Id));
                    db.ConfigVacacionesDias.RemoveRange(apt);

                    foreach (ConfigVacacionesDias d in datos.VacacionesDias)
                    {
                        d.ConfigVacacionesId = datos.VacacionesPpal.Id;
                    }

                    db.ConfigVacacionesDias.AddRange(datos.VacacionesDias);
                    db.SaveChanges();
                              
                    //var gps = db.GrupoVacaciones.Where(x => x.ConfigVacacionesId.Equals(datos.VacacionesPpal.Id));
                    //foreach (EmpleadoVacaciones g in datos.VacacionesRelacion)
                    //{
                    //    g.ConfigVacacionesId = datos.VacacionesPpal.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}
                    //db.GrupoVacaciones.RemoveRange(gps);
                    //db.GrupoVacaciones.AddRange(datos.VacacionesRelacion);
                }
                else if (datos.crud == 4)
                {
                    var a = db.ConfigVacaciones.Find(datos.VacacionesPpal.Id);
                    db.Entry(a).Property(x => x.Activo).IsModified = true;
                    db.Entry(a).Property(x => x.fch_Modificacion).IsModified = true;
                    db.Entry(a).Property(x => x.UsuarioMod).IsModified = true;

                    a.Activo = false;
                    a.fch_Modificacion = DateTime.Now;
                    a.UsuarioMod = datos.VacacionesPpal.UsuarioMod;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.ConfigVacaciones - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDIncapacidades(CRUDConfiguracionesDto datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    ConfigIncapacidades a = new ConfigIncapacidades();
                    a = datos.Incapacidades;
                    a.Activo = true;
                    a.fch_Modificacion = DateTime.Now;
                    db.ConfigIncapacidades.Add(a);
                    db.SaveChanges();

                    foreach (ConfigIncapacidadesDias d in datos.IncapacidadesDias)
                    {
                        d.ConfigIncapacidadesId = a.Id;
                    }

                    db.ConfigIncapacidadesDias.AddRange(datos.IncapacidadesDias);
                    db.SaveChanges();

                    //foreach (EmpleadoIncapacidad g in datos.IncapacidadesRelacion)
                    //{
                    //    g.ConfigIncapacidadesId = a.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}

                    //db.EmpleadoIncapacidad.AddRange(datos.IncapacidadesRelacion);
                    //db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigIncapacidades.Find(datos.Incapacidades.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.Nombre = datos.Incapacidades.Nombre;
                    a.Comentarios = datos.Incapacidades.Comentarios;
                    a.UsuarioMod = datos.Incapacidades.UsuarioMod;
                    a.fch_Modificacion = DateTime.Now;
                    a.ClienteId = datos.Incapacidades.ClienteId;
                    db.SaveChanges();

                    var apt = db.ConfigIncapacidadesDias.Where(x => x.ConfigIncapacidadesId.Equals(datos.Incapacidades.Id));
                    db.ConfigIncapacidadesDias.RemoveRange(apt);

                    foreach (ConfigIncapacidadesDias d in datos.IncapacidadesDias)
                    {
                        d.ConfigIncapacidadesId = datos.Incapacidades.Id;
                    }

                    db.ConfigIncapacidadesDias.AddRange(datos.IncapacidadesDias);
                    db.SaveChanges();

                    //var gps = db.EmpleadoIncapacidad.Where(x => x.ConfigIncapacidadesId.Equals(datos.Incapacidades.Id));
                    //db.EmpleadoIncapacidad.RemoveRange(gps);

                    //foreach (EmpleadoIncapacidad g in datos.IncapacidadesRelacion)
                    //{
                    //    g.ConfigIncapacidadesId = datos.Incapacidades.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}
                   
                    //db.EmpleadoIncapacidad.AddRange(datos.IncapacidadesRelacion);
                }
                else if (datos.crud == 4)
                {
                    var a = db.ConfigIncapacidades.Find(datos.Incapacidades.Id);
                    db.Entry(a).Property(x => x.Activo).IsModified = true;
                    db.Entry(a).Property(x => x.fch_Modificacion).IsModified = true;
                    db.Entry(a).Property(x => x.UsuarioMod).IsModified = true;

                    a.Activo = false;
                    a.fch_Modificacion = DateTime.Now;
                    a.UsuarioMod = datos.Incapacidades.UsuarioMod;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.ConfigIncapacidades - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDDiasEconomicos(CRUDConfiguracionesDto datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    ConfigDiasEconomicos a = new ConfigDiasEconomicos();
                    a = datos.DiasEconomicos;
                    a.fch_Modificacion = DateTime.Now;
                    db.ConfigDiasEconomicos.Add(a);
                    db.SaveChanges();

                    foreach (ConfigDiasEconomicosDias d in datos.DiasEconomicosDias)
                    {
                        d.ConfigDiasEconomicosId = a.Id;
                    }

                    db.ConfigDiasEconomicosDias.AddRange(datos.DiasEconomicosDias);
                    db.SaveChanges();

                    //foreach (EmpleadoDiasEconomicos g in datos.DiasEconomicosRel)
                    //{
                    //    g.ConfigDiasEconomicosId = a.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}

                    //db.EmpleadoDiasEconomicos.AddRange(datos.DiasEconomicosRel);
                    //db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigDiasEconomicos.Find(datos.DiasEconomicos.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.Nombre = datos.DiasEconomicos.Nombre;
                    a.Comentarios = datos.DiasEconomicos.Comentarios;
                    a.UsuarioMod = datos.DiasEconomicos.UsuarioMod;
                    a.ClienteId = datos.DiasEconomicos.ClienteId;
                    a.fch_Modificacion = DateTime.Now;

                    var cd = db.ConfigDiasEconomicosDias.Where(x => x.ConfigDiasEconomicosId.Equals(datos.DiasEconomicos.Id));
                    db.ConfigDiasEconomicosDias.RemoveRange(cd);
                    foreach (ConfigDiasEconomicosDias d in datos.DiasEconomicosDias)
                    {
                        d.ConfigDiasEconomicosId = datos.DiasEconomicos.Id;
                        db.SaveChanges();
                    }
                    db.ConfigDiasEconomicosDias.AddRange(datos.DiasEconomicosDias);

                    //var gps = db.EmpleadoDiasEconomicos.Where(x => x.ConfigDiasEconomicosId.Equals(datos.DiasEconomicos.Id));
                    //foreach (EmpleadoDiasEconomicos g in datos.DiasEconomicosRel)
                    //{
                    //    g.ConfigDiasEconomicosId = datos.DiasEconomicos.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}
                    //db.EmpleadoDiasEconomicos.RemoveRange(gps);
                    //db.EmpleadoDiasEconomicos.AddRange(datos.DiasEconomicosRel);
                }
                else if (datos.crud == 4)
                {
                    var a = db.ConfigDiasEconomicos.Find(datos.DiasEconomicos.Id);
                    db.Entry(a).Property(x => x.Activo).IsModified = true;
                    db.Entry(a).Property(x => x.fch_Modificacion).IsModified = true;
                    db.Entry(a).Property(x => x.UsuarioMod).IsModified = true;

                    a.Activo = false;
                    a.UsuarioMod = datos.DiasEconomicos.UsuarioMod;
                    a.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.ConfigDiasEconomicos - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDGuardias(CRUDConfiguracionesDto datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    ConfigGuardias a = new ConfigGuardias();
                    a = datos.Guardias;
                    a.fch_Modificacion = DateTime.Now;
                    db.ConfigGuardias.Add(a);
                    db.SaveChanges();

                    //foreach (EmpleadoGuardia g in datos.GuardiasRelacion)
                    //{
                    //    g.ConfigGuardiasId = a.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}

                    //db.EmpleadoGuardia.AddRange(datos.GuardiasRelacion);
                    //db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigGuardias.Find(datos.Guardias.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.NoGuardias = datos.Guardias.NoGuardias;
                    a.Consecutivas = datos.Guardias.Consecutivas;
                    a.Comentarios = datos.Guardias.Comentarios;
                    a.UsuarioMod = datos.Guardias.UsuarioMod;
                    a.ClienteId = datos.Guardias.ClienteId;
                    a.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();

                    //var gps = db.EmpleadoGuardia.Where(x => x.ConfigGuardiasId.Equals(datos.Guardias.Id));
                    //db.EmpleadoGuardia.RemoveRange(gps);
                    //foreach (EmpleadoGuardia g in datos.GuardiasRelacion)
                    //{
                    //    g.ConfigGuardiasId = datos.Guardias.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}
                    //db.EmpleadoGuardia.AddRange(datos.GuardiasRelacion);
                }
                else if (datos.crud == 4)
                {
                    var a = db.ConfigGuardias.Find(datos.Guardias.Id);
                    db.Entry(a).Property(x => x.Activo).IsModified = true;
                    db.Entry(a).Property(x => x.fch_Modificacion).IsModified = true;
                    db.Entry(a).Property(x => x.UsuarioMod).IsModified = true;

                    a.Activo = false;
                    a.fch_Modificacion = DateTime.Now;
                    a.UsuarioMod = datos.Guardias.UsuarioMod;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.ConfigGuardias - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDTiempoExtra(CRUDConfiguracionesDto datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    ConfigTiempoExtra a = new ConfigTiempoExtra();
                    a = datos.TiempoExtra;
                    a.fch_Modificacion = DateTime.Now;
                    db.ConfigTiempoExtra.Add(a);
                    db.SaveChanges();

                    //foreach (EmpleadoTiempoExtra g in datos.TiempoExtraRelacion)
                    //{
                    //    g.ConfigTiempoExtraId = a.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}

                    //db.EmpleadoTiempoExtra.AddRange(datos.TiempoExtraRelacion);
                    //db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigTiempoExtra.Find(datos.TiempoExtra.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.Redondeo = datos.TiempoExtra.Redondeo;
                    a.TE_Dobles = datos.TiempoExtra.TE_Dobles;
                    a.TE_Triple = datos.TiempoExtra.TE_Triple;
                    a.TE_Hora = datos.TiempoExtra.TE_Hora;
                    a.TE_Media = datos.TiempoExtra.TE_Media;
                    a.TE_Total = datos.TiempoExtra.TE_Total;
                    a.UsuarioMod = datos.TiempoExtra.UsuarioMod;
                    a.ClienteId = datos.TiempoExtra.ClienteId;
                    a.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();

                    //var gps = db.EmpleadoTiempoExtra.Where(x => x.ConfigTiempoExtraId.Equals(datos.TiempoExtra.Id));
                    //db.EmpleadoTiempoExtra.RemoveRange(gps);
                    //foreach (EmpleadoTiempoExtra g in datos.TiempoExtraRelacion)
                    //{
                    //    g.ConfigTiempoExtraId = datos.TiempoExtra.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}
                    //db.EmpleadoTiempoExtra.AddRange(datos.TiempoExtraRelacion);
                }
                else if (datos.crud == 4)
                {
                    var a = db.ConfigTiempoExtra.Find(datos.TiempoExtra.Id);
                    db.Entry(a).Property(x => x.Activo).IsModified = true;
                    db.Entry(a).Property(x => x.fch_Modificacion).IsModified = true;
                    db.Entry(a).Property(x => x.UsuarioMod).IsModified = true;

                    a.Activo = false;
                    a.fch_Modificacion = DateTime.Now;
                    a.UsuarioMod = datos.TiempoExtra.UsuarioMod;
                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.ConfigTiempo_Extra - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDSuspension(CRUDConfiguracionesDto datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    ConfigSuspensionNotas a = new ConfigSuspensionNotas();
                    a = datos.Suspensiones;
                    a.fch_Modificacion = DateTime.Now;

                    db.ConfigSuspensionNotas.Add(a);
                    db.SaveChanges();

                    foreach (ConfigSuspensionNotasDias sn in datos.SuspensionesDias)
                    {
                        sn.ConfigSuspensionNotasId = a.Id;
                    }

                    db.ConfigSuspensionNotasDias.AddRange(datos.SuspensionesDias);
                    db.SaveChanges();

                    //foreach (EmpleadoSuspension g in datos.SuspensionesRelacion)
                    //{
                    //    g.ConfigSuspensionNotasId = a.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}

                    //db.EmpleadoSuspension.AddRange(datos.SuspensionesRelacion);
                    //db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigSuspensionNotas.Find(datos.Suspensiones.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.Nombre = datos.Suspensiones.Nombre;
                    a.Comentarios = datos.Suspensiones.Comentarios;
                    a.UsuarioMod = datos.Suspensiones.UsuarioMod;
                    a.ClienteId = datos.Suspensiones.ClienteId;
                    a.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();

                    var s = db.ConfigSuspensionNotasDias.Where(x => x.ConfigSuspensionNotasId.Equals(datos.Suspensiones.Id));
                    db.ConfigSuspensionNotasDias.RemoveRange(s);
                    foreach (ConfigSuspensionNotasDias sn in datos.SuspensionesDias)
                    {
                        sn.ConfigSuspensionNotasId = datos.Suspensiones.Id;
                    }

                    db.ConfigSuspensionNotasDias.AddRange(datos.SuspensionesDias);
                    db.SaveChanges();

                    //var gps = db.EmpleadoSuspension.Where(x => x.ConfigSuspensionNotasId.Equals(datos.Suspensiones.Id));
                    //db.EmpleadoSuspension.RemoveRange(gps);
                    //foreach (EmpleadoSuspension g in datos.SuspensionesRelacion)
                    //{
                    //    g.ConfigSuspensionNotasId = datos.Suspensiones.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}
                    //db.EmpleadoSuspension.AddRange(datos.SuspensionesRelacion);
                }
                else if (datos.crud == 4)
                {
                    var a = db.ConfigSuspensionNotas.Find(datos.Suspensiones.Id);
                    db.Entry(a).Property(x => x.Activo).IsModified = true;
                    db.Entry(a).Property(x => x.fch_Modificacion).IsModified = true;
                    db.Entry(a).Property(x => x.UsuarioMod).IsModified = true;

                    a.Activo = false;
                    a.fch_Modificacion = DateTime.Now;
                    a.UsuarioMod = datos.Suspensiones.UsuarioMod;
                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.ConfigSuspension - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDPrima(CRUDConfiguracionesDto datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    ConfigPrima a = new ConfigPrima();
                    a = datos.PrimaDominical;
                    a.fch_Modificacion = DateTime.Now;

                    db.ConfigPrima.Add(a);
                    db.SaveChanges();

                    //foreach (EmpleadoPrima g in datos.PrimaRelacion)
                    //{
                    //    g.ConfigPrimaId = a.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}

                    //db.EmpleadoPrima.AddRange(datos.PrimaRelacion);
                    //db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigPrima.Find(datos.PrimaDominical.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.Horas = datos.PrimaDominical.Horas;
                    a.porcentaje = datos.PrimaDominical.porcentaje;
                    a.Observaciones = datos.PrimaDominical.Observaciones;
                    a.UsuarioMod = datos.PrimaDominical.UsuarioMod;
                    a.ClienteId = datos.PrimaDominical.ClienteId;
                    a.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();

                    //var gps = db.EmpleadoPrima.Where(x => x.ConfigPrimaId.Equals(datos.PrimaDominical.Id));
                    //db.EmpleadoPrima.RemoveRange(gps);
                    //foreach (EmpleadoPrima g in datos.PrimaRelacion)
                    //{
                    //    g.ConfigPrimaId = datos.PrimaDominical.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}
                    //db.EmpleadoPrima.AddRange(datos.PrimaRelacion);
                }
                else if (datos.crud == 4)
                {
                    var a = db.ConfigPrima.Find(datos.PrimaDominical.Id);
                    db.Entry(a).Property(x => x.Activo).IsModified = true;
                    db.Entry(a).Property(x => x.fch_Modificacion).IsModified = true;
                    db.Entry(a).Property(x => x.UsuarioMod).IsModified = true;

                    a.Activo = false;
                    a.fch_Modificacion = DateTime.Now;
                    a.UsuarioMod = datos.PrimaDominical.UsuarioMod;
                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.ConfigPrimaDominical - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDTolerancia(CRUDConfiguracionesDto datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    ConfigTolerancia a = new ConfigTolerancia();
                    a = datos.Tolerancia;
                    a.fch_Modificacion = DateTime.Now;
                    db.ConfigTolerancia.Add(a);
                    db.SaveChanges();

                    if (datos.ToleranciaTiempo.Count() > 0)
                    {
                        foreach (ConfigToleranciaTiempo d in datos.ToleranciaTiempo)
                        {
                            d.ConfigToleranciaId = a.Id;
                        }

                        db.ConfigToleranciaTiempo.AddRange(datos.ToleranciaTiempo);
                        db.SaveChanges();

                        //foreach (EmpleadoTolerancia g in datos.ToleranciaRelacion)
                        //{
                        //    g.ConfigToleranciaId = a.Id;
                        //    g.fch_Modificacion = DateTime.Now;
                        //}

                        //db.EmpleadoTolerancia.AddRange(datos.ToleranciaRelacion);
                        //db.SaveChanges();
                    }
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigTolerancia.Find(datos.Tolerancia.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.Nombre = datos.Tolerancia.Nombre;
                    a.Observaciones = datos.Tolerancia.Observaciones;
                    a.UsuarioMod = datos.Tolerancia.UsuarioMod;
                    a.ClienteId = datos.Tolerancia.ClienteId;
                    a.fch_Modificacion = DateTime.Now;

                    var cd = db.ConfigToleranciaTiempo.Where(x => x.ConfigToleranciaId.Equals(datos.Tolerancia.Id));
                    db.ConfigToleranciaTiempo.RemoveRange(cd);
                    foreach (ConfigToleranciaTiempo d in datos.ToleranciaTiempo)
                    {
                        d.ConfigToleranciaId = datos.Tolerancia.Id;
                    }
                    db.ConfigToleranciaTiempo.AddRange(datos.ToleranciaTiempo);
                    db.SaveChanges();
                    //var gps = db.EmpleadoTolerancia.Where(x => x.ConfigToleranciaId.Equals(datos.Tolerancia.Id));
                    //foreach (EmpleadoTolerancia g in datos.ToleranciaRelacion)
                    //{
                    //    g.ConfigToleranciaId = datos.Tolerancia.Id;
                    //    g.fch_Modificacion = DateTime.Now;
                    //}
                    //db.EmpleadoTolerancia.RemoveRange(gps);
                    //db.EmpleadoTolerancia.AddRange(datos.ToleranciaRelacion);
                }
                else if (datos.crud == 4)
                {
                    var a = db.ConfigTolerancia.Find(datos.Tolerancia.Id);
                    db.Entry(a).Property(x => x.Activo).IsModified = true;
                    db.Entry(a).Property(x => x.fch_Modificacion).IsModified = true;
                    db.Entry(a).Property(x => x.UsuarioMod).IsModified = true;

                    a.Activo = false;
                    a.UsuarioMod = datos.Tolerancia.UsuarioMod;
                    a.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.ConfigTolerancia - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        public HttpResponseMessage CRUDBono(CRUDConfiguracionesDto datos)
        {

            try
            {
                if (datos.crud == 1)
                {
                    ConfigBono a = new ConfigBono();
                    a = datos.ConfigBono;
                    a.fch_Modificacion = DateTime.Now;
                    db.ConfigBono.Add(a);
                    db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigBono.Find(datos.ConfigBono.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.Activo = datos.ConfigBono.Activo;
                    a.Comentarios = datos.ConfigBono.Comentarios;
                    a.PeriodosId = datos.ConfigBono.PeriodosId;
                    a.UsuarioMod = datos.Tolerancia.UsuarioMod;
                    a.ClienteId = datos.Tolerancia.ClienteId;
                    a.fch_Modificacion = DateTime.Now;
                }
                else if (datos.crud == 4)
                {
                    var a = db.ConfigBono.Find(datos.ConfigBono.Id);
                    db.Entry(a).Property(x => x.Activo).IsModified = true;
                    db.Entry(a).Property(x => x.fch_Modificacion).IsModified = true;
                    db.Entry(a).Property(x => x.UsuarioMod).IsModified = true;

                    a.Activo = false;
                    a.UsuarioMod = datos.ConfigBono.UsuarioMod;
                    a.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.Continue);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                APISAGALog log = new APISAGALog();
                log.WriteError("db.ConfigBono - " + ex.Message + " InnerException - " + ex.InnerException.Message);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        [Route("crudConfiguracionIngresos")]
        [Authorize]
        public IHttpActionResult CRUDConfiguracionIngresos(CRUDConfiguracionesDto datos)
        {
            try
            {
                if (datos.Catalogo.ToLower().Equals("vacaciones"))
                {
                    var result = this.CRUDVacaciones(datos).StatusCode;

                    return Ok(result);
                }
                if (datos.Catalogo.ToLower().Equals("incapacidad"))
                {
                    var result = this.CRUDIncapacidades(datos).StatusCode;

                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("dias"))
                {
                    var result = this.CRUDDiasEconomicos(datos).StatusCode;

                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("guardias"))
                {
                    var result = this.CRUDGuardias(datos).StatusCode;

                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("tiempo_extra"))
                {
                    var result = this.CRUDTiempoExtra(datos).StatusCode;

                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("suspension"))
                {
                    var result = this.CRUDSuspension(datos).StatusCode;

                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("prima"))
                {
                    var result = this.CRUDPrima(datos).StatusCode;

                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("tolerancia"))
                {
                    var result = this.CRUDTolerancia(datos).StatusCode;

                    return Ok(result);
                }
                else if (datos.Catalogo.ToLower().Equals("bonos"))
                {
                    var result = this.CRUDBono(datos).StatusCode;

                    return Ok(result);
                }
                else
                {
                    return Ok(HttpStatusCode.Continue);
                }
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        #endregion
        [HttpGet]
        [Route("getCatalogoConfig")]
        [Authorize]
        public IHttpActionResult GetCatalogoConfig(string nombre, Guid clienteId)
        {
            try
            {
                if (nombre.ToLower().Equals("vacaciones"))
                {
                    try
                    {
                        var datos = db.ConfigVacaciones.OrderByDescending(o => o.fch_Modificacion).Where(x => x.Activo && x.ClienteId.Equals(clienteId)).Select(d => new
                        {
                            d.Id,
                            d.Nombre,
                            d.Observaciones,
                            d.DiasContinuos,
                            d.DiasExpiran,
                            d.DiasIncremento,
                            d.Porcentaje,
                            antiguedad = db.ConfigVacacionesDias.Where(x => x.ConfigVacacionesId.Equals(d.Id)).Select(dd => new
                            {
                                tiempo = dd.TiempoAntiguedadId,
                                nombre = dd.TiempoAntiguedad.Tiempo,
                                diasLey = dd.TiempoAntiguedad.DiasLey,
                                dd.Dias,
                                dd.Id
                            }).ToList(),
                            empleados = db.GrupoVacaciones.Where(x => x.ConfigVacacionesId.Equals(d.Id)).Select(g => g.empleadoId).ToList(),
                            usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(d.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                        }).ToList();

                        var registro = db.TiempoAntiguedad.OrderBy(o => o.Id).Where(x => x.Activo).Select(r => new
                        {
                            r.Id,
                            r.Tiempo,
                            r.Descripcion
                        }).ToList();
                        return Ok(new { datos, registro });
                    }
                    catch (Exception ex)
                    {
                        return Ok(HttpStatusCode.BadRequest);
                    }
                }
                else if (nombre.ToLower().Equals("incapacidad"))
                {
                    try
                    {
                        var datos = db.ConfigIncapacidades.OrderByDescending(o => o.fch_Modificacion).Where(x => x.Activo).Select(d => new
                        {
                            d.Id,
                            d.Nombre,
                            d.Comentarios,
                            incapacidades = db.ConfigIncapacidadesDias.Where(x => x.ConfigIncapacidadesId.Equals(d.Id)).Select(dd => new
                            {
                                dd.TiposIncapacidad.Id,
                                tipoIncapacidad = dd.TiposIncapacidad.Nombre,
                                dd.Porcentaje,
                                dd.Dias
                            }).ToList(),
                            empleados = db.EmpleadoIncapacidad.Where(x => x.ConfigIncapacidadesId.Equals(d.Id)).Select(g => g.empleadoId).ToList(),
                            usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(d.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                        }).ToList();

                        var tiposIncapacidades = db.TiposIncapacidad.OrderBy(o => o.Id).Where(x => x.Activo).Select(r => new
                        {
                            r.Id,
                            r.Nombre,
                            r.Comentarios,
                            tipo = r.Comentarios == "SIN REGISTRO" ? 1 : 2
                        }).ToList();
                        return Ok(new { datos, tiposIncapacidades });
                    }
                    catch (Exception ex)
                    {
                        return Ok(HttpStatusCode.BadRequest);
                    }
                }
                else if (nombre.ToLower().Equals("dias"))
                {
                    var datos = db.ConfigDiasEconomicos.Where(x => x.Activo).Select(d => new
                    {
                        d.Id,
                        d.Nombre,
                        diasMax = d.DiasConSueldo,
                        diasSin = d.DiasSinSueldo,
                        Dias = db.ConfigDiasEconomicosDias.Where(x => x.ConfigDiasEconomicosId.Equals(d.Id)).Select(dd => new
                        {
                            Tipo = dd.TiposDiasEconomicos.Nombre,
                            Descripcion = dd.TiposDiasEconomicos.Comentarios,
                            id = dd.TiposDiasEconomicosId,
                            dd.Dias
                        }).ToList(),
                        d.Comentarios,
                        empleados = db.EmpleadoDiasEconomicos.Where(x => x.ConfigDiasEconomicosId.Equals(d.Id)).Select(g => g.empleadoId).ToList(),
                        usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(d.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                    }).ToList();

                    var tiposDiasEconomicos = db.TiposDiasEconomicos.Where(x => x.Activo).OrderBy(o => o.Orden).Select(r => new
                    {
                        r.Id,
                        r.Nombre,
                        r.Comentarios
                    }).ToList();
                    return Ok(new { datos, tiposDiasEconomicos });
                }
                else if (nombre.ToLower().Equals("guardias"))
                {
                    var datos = db.ConfigGuardias.Where(x => x.Activo).Select(d => new
                    {
                        d.Id,
                        d.NoGuardias,
                        consecutivas = d.Consecutivas ? "Activas" : "No Activas",
                        d.Comentarios,
                        empleados = db.EmpleadoGuardia.Where(x => x.ConfigGuardiasId.Equals(d.Id)).Select(g => g.empleadoId).ToList(),
                        usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(d.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                    }).ToList();

                    return Ok(datos);
                }
                else if (nombre.ToLower().Equals("tiempo_extra"))
                {
                    var datos = db.ConfigTiempoExtra.Where(x => x.Activo).Select(d => new
                    {
                        d.Id,
                        d.TE_Total,
                        d.TE_Media,
                        d.TE_Hora,
                        d.TE_Dobles,
                        d.TE_Triple,
                        d.Redondeo,
                        d.Comentarios,
                        empleados = db.EmpleadoTiempoExtra.Where(x => x.ConfigTiempoExtraId.Equals(d.Id)).Select(g => g.empleadoId).ToList(),
                        usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(d.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                    }).ToList();

                    return Ok(datos);
                }
                else if (nombre.ToLower().Equals("suspension"))
                {
                    var datos = db.ConfigSuspensionNotasDias.Where(x => x.ConfigSuspensionNotas.Activo).Select(r => new
                    {
                        r.ConfigSuspensionNotas.Nombre,
                        r.ConfigSuspensionNotas.Id,
                        r.Retardos,
                        r.Dias,
                        Tipo = r.Tipo == 1 ? "Disciplinaria" : r.Tipo == 2 ? "Retardo" : r.Tipo == 3 ? "Memorandum" : r.Tipo == 4 ? "Reconocimiento" : "Acta Administrativa",
                        TipoId = r.Tipo,
                        r.ConfigSuspensionNotas.Comentarios,
                        empleados = db.EmpleadoSuspension.Where(x => x.ConfigSuspensionNotasId.Equals(r.ConfigSuspensionNotasId)).Select(g => g.empleadoId).ToList(),
                        usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(r.ConfigSuspensionNotas.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                    }).ToList();

                    return Ok(datos);
                }
                else if (nombre.ToLower().Equals("prima"))
                {
                    var datos = db.ConfigPrima.Where(x => x.Activo).Select(r => new
                    {
                        r.Id,
                        r.Horas,
                        r.porcentaje,
                        r.Observaciones,
                        empleados = db.EmpleadoPrima.Where(x => x.ConfigPrimaId.Equals(r.Id)).Select(g => g.empleadoId).ToList(),
                        usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(r.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                    }).ToList();

                    return Ok(datos);
                }
                else if (nombre.ToLower().Equals("tolerancia"))
                {
                    var datos = db.ConfigTolerancia.Where(x => x.Activo).Select(r => new
                    {
                        r.Id,
                        r.Nombre,
                        r.Observaciones,
                        tiempo = db.ConfigToleranciaTiempo.Where(x => x.ConfigToleranciaId.Equals(r.Id)).Select(t => new
                        {
                            t.Id,
                            t.Tiempo,
                            t.Tipo
                        }),
                        empleados = db.EmpleadoTolerancia.Where(x => x.ConfigToleranciaId.Equals(r.Id)).Select(g => g.empleadoId).ToList(),
                        usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(r.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                    }).ToList();

                    return Ok(datos);
                }
                else if (nombre.ToLower().Equals("horarios"))
                {
                    var datos = db.HorariosIngresos.Where(x => x.Activo && x.ClienteId.Equals(clienteId)).Select(h => new
                    {
                        h.Id,
                        h.Nombre,
                        h.Clave,
                        turno = h.TurnosHorarios.Nombre,
                        turnoId = h.TurnosHorariosId,
                        h.Especificaciones,
                        usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(h.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                        diashoras = db.DiasHorasIngresos.Where(x => x.Activo && x.HorariosIngresosId.Equals(h.Id)).GroupBy(g => g.Dia)
                        .Select(d => new
                        {
                            d.Key,
                            Dia = d.Key,
                            deHora = d.Where(x => x.Dia.Equals(d.Key) && x.Tipo.Equals(1)).Select(hh => hh.DeHora),
                            aHora = d.Where(x => x.Dia.Equals(d.Key) && x.Tipo.Equals(1)).Select(hh => hh.AHora),
                            deHoraComida = d.Where(x => x.Dia.Equals(d.Key) && x.Tipo.Equals(2)).Select(hh => hh.DeHora),
                            aHoraComida = d.Where(x => x.Dia.Equals(d.Key) && x.Tipo.Equals(2)).Select(hh => hh.AHora),
                            descansos = d.Where(x => x.Dia.Equals(d.Key) && x.Tipo.Equals(3)).Select(hh => new
                            {
                                deHora = hh.DeHora,
                                aHora = hh.AHora
                            }).ToList()
                        }).ToList(),
                        diashorasE = db.DiasHorasEspecial.Where(x => x.Activo && x.HorariosIngresosId.Equals(h.Id)).GroupBy(g => g.Tipo)
                        .Select(d => new
                        {
                            fchInicio = d.Select(dd => dd.fchInicio).FirstOrDefault(),
                            fchFin = d.Select(dd => dd.fchFin).FirstOrDefault(),
                            DeHora = d.Where(x => x.Tipo.Equals(1)).Select(dd => dd.DeHora).FirstOrDefault(),
                            AHora = d.Where(x => x.Tipo.Equals(1)).Select(dd => dd.AHora).FirstOrDefault(),
                            DeHoraComida = d.Where(x => x.Tipo.Equals(2)).Select(dd => dd.DeHora).FirstOrDefault(),
                            AHoraComida = d.Where(x => x.Tipo.Equals(2)).Select(dd => dd.AHora).FirstOrDefault(),
                            tipo = d.Key
                        }).ToList(),
                        horario = db.DiasHorasIngresos.Where(x => x.Activo && x.HorariosIngresosId.Equals(h.Id)).Count()
                    }).ToList();

                    return Ok(datos);
                }
                else if (nombre.ToLower().Equals("bonos"))
                {
                    var datos = db.ConfigBono.Where(x => x.Activo && x.ClienteId.Equals(clienteId)).Select(r => new
                    {
                        r.Id,
                        r.TiposBono.Nombre,
                        r.TiposBonoId,
                        r.Comentarios,
                        Periodo = r.Periodos.Nombre,
                        r.PeriodosId,
                        empleados = db.EmpleadoBono.Where(x => x.ConfigBonoId.Equals(r.Id)).Select(g => g.empleadoId).ToList(),
                        usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(r.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                    }).ToList();

                    return Ok(datos);
                }
                else if (nombre.ToLower().Equals("puestos"))
                {
                    var datos = db.PuestosCliente.Where(x => x.PuestosIngresos.Activo && x.ClienteId.Equals(clienteId)).Select(r => new
                    {
                        r.PuestosIngresos.Nombre,
                        id = r.PuestosIngresos.Id,
                        r.PuestosIngresos.Descripcion,
                        r.PuestosIngresos.Clave
                    }).ToList();

                    return Ok(datos);
                }
                else
                {
                    return Ok(HttpStatusCode.Continue);
                }

            }
            catch(Exception ex)
            {
                APISAGALog log = new APISAGALog();

                log.WriteError(ex.Message + " - InnerException: " + ex.InnerException.Message);

                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        [Route("asignarConfiguracion")]
        [Authorize]
        public IHttpActionResult AsignarConfiguracion(CRUDConfiguracionesDto datos)
        {
            try
            {
                if (datos.Catalogo.ToLower().Equals("vacaciones"))
                {
                    List<EmpleadoVacaciones> Listobj = new List<EmpleadoVacaciones>();
                    EmpleadoVacaciones obj = new EmpleadoVacaciones();
                    foreach (var g in datos.Asignacion)
                    {
                        obj.empleadoId = g.EmpleadoId;
                        obj.ConfigVacacionesId = g.Id;
                        obj.Activo = g.Activo;
                        obj.UsuarioAlta = g.UsuarioAlta;
                        obj.UsuarioMod = g.UsuarioMod;
                        obj.fch_Modificacion = DateTime.Now;

                        Listobj.Add(obj);
                        obj = new EmpleadoVacaciones();
                    }

                    db.GrupoVacaciones.AddRange(Listobj);
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("incapacidad"))
                {
                    List<EmpleadoIncapacidad> Listobj = new List<EmpleadoIncapacidad>();
                    EmpleadoIncapacidad obj = new EmpleadoIncapacidad();
                    foreach (var g in datos.Asignacion)
                    {
                        obj.empleadoId = g.EmpleadoId;
                        obj.ConfigIncapacidadesId = g.Id;
                        obj.Activo = g.Activo;
                        obj.UsuarioAlta = g.UsuarioAlta;
                        obj.UsuarioMod = g.UsuarioMod;
                        obj.fch_Modificacion = DateTime.Now;

                        Listobj.Add(obj);
                        obj = new EmpleadoIncapacidad();
                    }

                    db.EmpleadoIncapacidad.AddRange(Listobj);
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("dias"))
                {
                    List<EmpleadoDiasEconomicos> Listobj = new List<EmpleadoDiasEconomicos>();
                    EmpleadoDiasEconomicos obj = new EmpleadoDiasEconomicos();
                    foreach (var g in datos.Asignacion)
                    {
                        obj.empleadoId = g.EmpleadoId;
                        obj.ConfigDiasEconomicosId = g.Id;
                        obj.Activo = g.Activo;
                        obj.UsuarioAlta = g.UsuarioAlta;
                        obj.UsuarioMod = g.UsuarioMod;
                        obj.fch_Modificacion = DateTime.Now;

                        Listobj.Add(obj);
                        obj = new EmpleadoDiasEconomicos();
                    }

                    db.EmpleadoDiasEconomicos.AddRange(Listobj);
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("guardias"))
                {
                    List<EmpleadoGuardia> Listobj = new List<EmpleadoGuardia>();
                    EmpleadoGuardia obj = new EmpleadoGuardia();
                    foreach (var g in datos.Asignacion)
                    {
                        obj.empleadoId = g.EmpleadoId;
                        obj.ConfigGuardiasId = g.Id;
                        obj.Activo = g.Activo;
                        obj.UsuarioAlta = g.UsuarioAlta;
                        obj.UsuarioMod = g.UsuarioMod;
                        obj.fch_Modificacion = DateTime.Now;

                        Listobj.Add(obj);
                        obj = new EmpleadoGuardia();
                    }
                    db.EmpleadoGuardia.AddRange(Listobj);
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("tiempo_extra"))
                {
                    List<EmpleadoTiempoExtra> Listobj = new List<EmpleadoTiempoExtra>();
                    EmpleadoTiempoExtra obj = new EmpleadoTiempoExtra();
                    foreach (var g in datos.Asignacion)
                    {
                        obj.empleadoId = g.EmpleadoId;
                        obj.ConfigTiempoExtraId = g.Id;
                        obj.Activo = g.Activo;
                        obj.UsuarioAlta = g.UsuarioAlta;
                        obj.UsuarioMod = g.UsuarioMod;
                        obj.fch_Modificacion = DateTime.Now;

                        Listobj.Add(obj);
                        obj = new EmpleadoTiempoExtra();
                    }
                    db.EmpleadoTiempoExtra.AddRange(Listobj);
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("suspension"))
                {
                    List<EmpleadoSuspension> Listobj = new List<EmpleadoSuspension>();
                    EmpleadoSuspension obj = new EmpleadoSuspension();
                    foreach (var g in datos.Asignacion)
                    {
                        obj.empleadoId = g.EmpleadoId;
                        obj.ConfigSuspensionNotasId = g.Id;
                        obj.Activo = g.Activo;
                        obj.UsuarioAlta = g.UsuarioAlta;
                        obj.UsuarioMod = g.UsuarioMod;
                        obj.fch_Modificacion = DateTime.Now;

                        Listobj.Add(obj);
                        obj = new EmpleadoSuspension();
                    }
                    db.EmpleadoSuspension.AddRange(Listobj);
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("prima"))
                {
                    List<EmpleadoPrima> Listobj = new List<EmpleadoPrima>();
                    EmpleadoPrima obj = new EmpleadoPrima();
                    foreach (var g in datos.Asignacion)
                    {
                        obj.empleadoId = g.EmpleadoId;
                        obj.ConfigPrimaId = g.Id;
                        obj.Activo = g.Activo;
                        obj.UsuarioAlta = g.UsuarioAlta;
                        obj.UsuarioMod = g.UsuarioMod;
                        obj.fch_Modificacion = DateTime.Now;

                        Listobj.Add(obj);
                        obj = new EmpleadoPrima();
                    }

                    db.EmpleadoPrima.AddRange(Listobj);
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("tolerancia"))
                {
                    List<EmpleadoTolerancia> Listobj = new List<EmpleadoTolerancia>();
                    EmpleadoTolerancia obj = new EmpleadoTolerancia();
                    foreach (var g in datos.Asignacion)
                    {
                        obj.empleadoId = g.EmpleadoId;
                        obj.ConfigToleranciaId = g.Id;
                        obj.Activo = g.Activo;
                        obj.UsuarioAlta = g.UsuarioAlta;
                        obj.UsuarioMod = g.UsuarioMod;
                        obj.fch_Modificacion = DateTime.Now;

                        Listobj.Add(obj);
                        obj = new EmpleadoTolerancia();
                    }
                    db.EmpleadoTolerancia.AddRange(Listobj);
                    db.SaveChanges();
                }
                else if (datos.Catalogo == "horario")
                {
                    List<EmpleadoHorario> Listobj = new List<EmpleadoHorario>();
                    EmpleadoHorario obj = new EmpleadoHorario();
                    foreach (var g in datos.Asignacion)
                    {
                        obj.empleadoId = g.EmpleadoId;
                        obj.HorariosIngresosId = g.IdG;
                        obj.Activo = g.Activo;
                        obj.UsuarioAlta = g.UsuarioAlta;
                        obj.UsuarioMod = g.UsuarioMod;
                        obj.fch_Modificacion = DateTime.Now;

                        Listobj.Add(obj);
                        obj = new EmpleadoHorario();
                    }

                    db.EmpleadoHorario.AddRange(Listobj);
                    db.SaveChanges();
                }
                else if (datos.Catalogo == "bonos")
                {
                    List<EmpleadoBono> Listobj = new List<EmpleadoBono>();
                    EmpleadoBono obj = new EmpleadoBono();
                    foreach (var g in datos.Asignacion)
                    {
                        obj.empleadoId = g.EmpleadoId;
                        obj.ConfigBonoId = g.IdG;
                        obj.Activo = g.Activo;
                        obj.UsuarioAlta = g.UsuarioAlta;
                        obj.UsuarioMod = g.UsuarioMod;
                        obj.fch_Modificacion = DateTime.Now;

                        Listobj.Add(obj);
                        obj = new EmpleadoBono();
                    }

                    db.EmpleadoBono.AddRange(Listobj);
                    db.SaveChanges();
                }
                else
                {
                    return Ok(HttpStatusCode.Continue);
                }
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        
        }
        [HttpPost]
        [Route("desasignarConfiguracion")]
        [Authorize]
        public IHttpActionResult DesasignarConfiguracion(CRUDConfiguracionesDto datos)
        {
            try
            {
                if (datos.Catalogo.ToLower().Equals("vacaciones"))
                {
                    var id = db.GrupoVacaciones.Where(x => x.empleadoId.Equals(datos.Usuario)).Select(c => c.Id).FirstOrDefault();
                    var a = db.GrupoVacaciones.Find(id);
                    db.Entry(a).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("incapacidad"))
                {
                    var id = db.EmpleadoIncapacidad.Where(x => x.empleadoId.Equals(datos.Usuario)).Select(c => c.Id).FirstOrDefault();
                    var a = db.EmpleadoIncapacidad.Find(id);
                    db.Entry(a).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("dias"))
                {
                    var id = db.EmpleadoDiasEconomicos.Where(x => x.empleadoId.Equals(datos.Usuario)).Select(c => c.Id).FirstOrDefault();
                    var a = db.EmpleadoDiasEconomicos.Find(id);
                    db.Entry(a).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("guardias"))
                {
                    var id = db.EmpleadoGuardia.Where(x => x.empleadoId.Equals(datos.Usuario)).Select(c => c.Id).FirstOrDefault();
                    var a = db.EmpleadoGuardia.Find(id);
                    db.Entry(a).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("tiempo_extra"))
                {
                    var id = db.EmpleadoTiempoExtra.Where(x => x.empleadoId.Equals(datos.Usuario)).Select(c => c.Id).FirstOrDefault();
                    var a = db.EmpleadoTiempoExtra.Find(id);
                    db.Entry(a).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("suspension"))
                {
                    var id = db.EmpleadoSuspension.Where(x => x.empleadoId.Equals(datos.Usuario)).Select(c => c.Id).FirstOrDefault();
                    var a = db.EmpleadoSuspension.Find(id);
                    db.Entry(a).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("prima"))
                {
                    var id = db.EmpleadoPrima.Where(x => x.empleadoId.Equals(datos.Usuario)).Select(c => c.Id).FirstOrDefault();
                    var a = db.EmpleadoPrima.Find(id);
                    db.Entry(a).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("tolerancia"))
                {
                    var id = db.EmpleadoTolerancia.Where(x => x.empleadoId.Equals(datos.Usuario)).Select(c => c.Id).FirstOrDefault();
                    var a = db.EmpleadoTolerancia.Find(id);
                    db.Entry(a).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                else if (datos.Catalogo == "horario")
                {
                    var id = db.EmpleadoHorario.Where(x => x.empleadoId.Equals(datos.Usuario)).Select(c => c.Id).FirstOrDefault();
                    var a = db.EmpleadoHorario.Find(id);
                    db.Entry(a).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                else if (datos.Catalogo.ToLower().Equals("bonos"))
                {
                    var id = db.EmpleadoBono.Where(x => x.empleadoId.Equals(datos.Usuario)).Select(c => c.Id).FirstOrDefault();
                    var a = db.EmpleadoBono.Find(id);
                    db.Entry(a).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                else
                {
                    return Ok(HttpStatusCode.Continue);
                }
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }
        [HttpPost]
        [Route("getAsignadosByConfig")]
        [Authorize]
        public IHttpActionResult GetAsignadosByConfig(CRUDConfiguracionesDto asignados)
        {
            try
            {
                if (asignados.Catalogo.ToLower().Equals("vacaciones"))
                {
                    var datos = db.GrupoVacaciones.Where(x => x.Activo).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("incapacidad"))
                {
                    var datos = db.EmpleadoIncapacidad.Where(x => x.Activo).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("dias"))
                {
                    var datos = db.EmpleadoDiasEconomicos.Where(x => x.Activo).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("guardias"))
                {
                    var datos = db.EmpleadoGuardia.Where(x => x.Activo).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("tiempo_extra"))
                {
                    var datos = db.EmpleadoTiempoExtra.Where(x => x.Activo).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("suspension"))
                {
                    var datos = db.EmpleadoSuspension.Where(x => x.Activo).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("prima"))
                {
                    var datos = db.EmpleadoPrima.Where(x => x.Activo).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("tolerancia"))
                {
                    var datos = db.EmpleadoTolerancia.Where(x => x.Activo).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower() == "horario")
                {
                    var datos = db.EmpleadoHorario.Where(x => x.Activo).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("bonos"))
                {
                    var datos = db.EmpleadoBono.Where(x => x.Activo).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
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
        [Route("getAsignadosConfiguracion")]
        [Authorize]
        public IHttpActionResult GetAsignadosConfig(CRUDConfiguracionesDto asignados)
        {
            try
            {
                if (asignados.Catalogo.ToLower().Equals("vacaciones"))
                {
                    var datos = db.GrupoVacaciones.Where(x => x.Activo && x.ConfigVacacionesId.Equals(asignados.Id)).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("incapacidad"))
                {
                    var datos = db.EmpleadoIncapacidad.Where(x => x.Activo && x.ConfigIncapacidadesId.Equals(asignados.Id)).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("dias"))
                {
                    var datos = db.EmpleadoDiasEconomicos.Where(x => x.Activo && x.ConfigDiasEconomicosId.Equals(asignados.Id)).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("guardias"))
                {
                    var datos = db.EmpleadoGuardia.Where(x => x.Activo && x.ConfigGuardiasId.Equals(asignados.Id)).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("tiempo_extra"))
                {
                    var datos = db.EmpleadoTiempoExtra.Where(x => x.Activo && x.ConfigTiempoExtraId.Equals(asignados.Id)).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("suspension"))
                {
                    var datos = db.EmpleadoSuspension.Where(x => x.Activo && x.ConfigSuspensionNotasId.Equals(asignados.Id)).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("prima"))
                {
                    var datos = db.EmpleadoPrima.Where(x => x.Activo && x.ConfigPrimaId.Equals(asignados.Id)).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("tolerancia"))
                {
                    var datos = db.EmpleadoTolerancia.Where(x => x.Activo && x.ConfigToleranciaId.Equals(asignados.Id)).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower() == "horario")
                {
                    var datos = db.EmpleadoHorario.Where(x => x.Activo && x.HorariosIngresosId.Equals(asignados.IdG)).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
                }
                else if (asignados.Catalogo.ToLower().Equals("bonos"))
                {
                    var datos = db.EmpleadoBono.Where(x => x.Activo && x.ConfigBonoId.Equals(asignados.IdG)).Select(d => new
                    {
                        id = d.Empleado.Id,
                        nombre = d.Empleado.Nombre,
                        apellidoPaterno = d.Empleado.ApellidoPaterno,
                        apellidoMaterno = d.Empleado.ApellidoMaterno,
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(d.empleadoId)).Select(e => e.PuestosIngresos.Nombre).FirstOrDefault(),
                        clave = "DAL0000"
                    }).ToList();

                    return Ok(datos);
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

    }
}