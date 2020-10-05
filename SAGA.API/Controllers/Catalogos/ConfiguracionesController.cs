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
        public ConfiguracionesController()
        {
            db = new SAGADBContext();
        }

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

                    foreach (GrupoVacaciones g in datos.VacacionesRelacion)
                    {
                        g.ConfigVacacionesId = a.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }

                    db.GrupoVacaciones.AddRange(datos.VacacionesRelacion);
                    db.SaveChanges();
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
                              
                    var gps = db.GrupoVacaciones.Where(x => x.ConfigVacacionesId.Equals(datos.VacacionesPpal.Id));
                    foreach (GrupoVacaciones g in datos.VacacionesRelacion)
                    {
                        g.ConfigVacacionesId = datos.VacacionesPpal.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }
                    db.GrupoVacaciones.RemoveRange(gps);
                    db.GrupoVacaciones.AddRange(datos.VacacionesRelacion);
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

                    foreach (GruposIncapacidad g in datos.IncapacidadesRelacion)
                    {
                        g.ConfigIncapacidadesId = a.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }

                    db.GruposIncapacidad.AddRange(datos.IncapacidadesRelacion);
                    db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigIncapacidades.Find(datos.Incapacidades.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.Nombre = datos.Incapacidades.Nombre;
                    a.Comentarios = datos.Incapacidades.Comentarios;
                    a.UsuarioMod = datos.Incapacidades.UsuarioMod;
                    a.fch_Modificacion = DateTime.Now;
                    db.SaveChanges();

                    var apt = db.ConfigIncapacidadesDias.Where(x => x.ConfigIncapacidadesId.Equals(datos.Incapacidades.Id));
                    db.ConfigIncapacidadesDias.RemoveRange(apt);

                    foreach (ConfigIncapacidadesDias d in datos.IncapacidadesDias)
                    {
                        d.ConfigIncapacidadesId = datos.Incapacidades.Id;
                    }

                    db.ConfigIncapacidadesDias.AddRange(datos.IncapacidadesDias);
                    db.SaveChanges();

                    var gps = db.GruposIncapacidad.Where(x => x.ConfigIncapacidadesId.Equals(datos.Incapacidades.Id));
                    db.GruposIncapacidad.RemoveRange(gps);

                    foreach (GruposIncapacidad g in datos.IncapacidadesRelacion)
                    {
                        g.ConfigIncapacidadesId = datos.Incapacidades.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }
                   
                    db.GruposIncapacidad.AddRange(datos.IncapacidadesRelacion);
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

                    foreach (GruposDiasEconomicos g in datos.DiasEconomicosRel)
                    {
                        g.ConfigDiasEconomicosId = a.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }

                    db.GruposDiasEconomicos.AddRange(datos.DiasEconomicosRel);
                    db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigDiasEconomicos.Find(datos.DiasEconomicos.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.Nombre = datos.DiasEconomicos.Nombre;
                    a.Comentarios = datos.DiasEconomicos.Comentarios;
                    a.UsuarioMod = datos.DiasEconomicos.UsuarioMod;
                    a.fch_Modificacion = DateTime.Now;

                    var cd = db.ConfigDiasEconomicosDias.Where(x => x.ConfigDiasEconomicosId.Equals(datos.DiasEconomicos.Id));
                    db.ConfigDiasEconomicosDias.RemoveRange(cd);
                    foreach (ConfigDiasEconomicosDias d in datos.DiasEconomicosDias)
                    {
                        d.ConfigDiasEconomicosId = datos.DiasEconomicos.Id;
                        db.SaveChanges();
                    }
                    db.ConfigDiasEconomicosDias.AddRange(datos.DiasEconomicosDias);

                    var gps = db.GruposDiasEconomicos.Where(x => x.ConfigDiasEconomicosId.Equals(datos.DiasEconomicos.Id));
                    foreach (GruposDiasEconomicos g in datos.DiasEconomicosRel)
                    {
                        g.ConfigDiasEconomicosId = datos.DiasEconomicos.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }
                    db.GruposDiasEconomicos.RemoveRange(gps);
                    db.GruposDiasEconomicos.AddRange(datos.DiasEconomicosRel);
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

                    foreach (GruposGuardia g in datos.GuardiasRelacion)
                    {
                        g.ConfigGuardiasId = a.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }

                    db.GruposGuardia.AddRange(datos.GuardiasRelacion);
                    db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigGuardias.Find(datos.Guardias.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.NoGuardias = datos.Guardias.NoGuardias;
                    a.Consecutivas = datos.Guardias.Consecutivas;
                    a.Comentarios = datos.Guardias.Comentarios;
                    a.UsuarioMod = datos.Guardias.UsuarioMod;
                    a.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();

                    var gps = db.GruposGuardia.Where(x => x.ConfigGuardiasId.Equals(datos.Guardias.Id));
                    db.GruposGuardia.RemoveRange(gps);
                    foreach (GruposGuardia g in datos.GuardiasRelacion)
                    {
                        g.ConfigGuardiasId = datos.Guardias.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }
                    db.GruposGuardia.AddRange(datos.GuardiasRelacion);
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

                    foreach (GruposTiempoExtra g in datos.TiempoExtraRelacion)
                    {
                        g.ConfigTiempoExtraId = a.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }

                    db.GruposTiempoExtra.AddRange(datos.TiempoExtraRelacion);
                    db.SaveChanges();
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
                    a.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();

                    var gps = db.GruposTiempoExtra.Where(x => x.ConfigTiempoExtraId.Equals(datos.TiempoExtra.Id));
                    db.GruposTiempoExtra.RemoveRange(gps);
                    foreach (GruposTiempoExtra g in datos.TiempoExtraRelacion)
                    {
                        g.ConfigTiempoExtraId = datos.TiempoExtra.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }
                    db.GruposTiempoExtra.AddRange(datos.TiempoExtraRelacion);
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

                    foreach (GruposSuspension g in datos.SuspensionesRelacion)
                    {
                        g.ConfigSuspensionNotasId = a.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }

                    db.GruposSuspension.AddRange(datos.SuspensionesRelacion);
                    db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigSuspensionNotas.Find(datos.Suspensiones.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.Nombre = datos.Suspensiones.Nombre;
                    a.Comentarios = datos.Suspensiones.Comentarios;
                    a.UsuarioMod = datos.Suspensiones.UsuarioMod;
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

                    var gps = db.GruposSuspension.Where(x => x.ConfigSuspensionNotasId.Equals(datos.Suspensiones.Id));
                    db.GruposSuspension.RemoveRange(gps);
                    foreach (GruposSuspension g in datos.SuspensionesRelacion)
                    {
                        g.ConfigSuspensionNotasId = datos.Suspensiones.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }
                    db.GruposSuspension.AddRange(datos.SuspensionesRelacion);
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

                    foreach (GruposPrima g in datos.PrimaRelacion)
                    {
                        g.ConfigPrimaId = a.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }

                    db.GruposPrima.AddRange(datos.PrimaRelacion);
                    db.SaveChanges();
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigPrima.Find(datos.PrimaDominical.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.Horas = datos.PrimaDominical.Horas;
                    a.porcentaje = datos.PrimaDominical.porcentaje;
                    a.Observaciones = datos.PrimaDominical.Observaciones;
                    a.UsuarioMod = datos.PrimaDominical.UsuarioMod;
                    a.fch_Modificacion = DateTime.Now;

                    db.SaveChanges();

                    var gps = db.GruposPrima.Where(x => x.ConfigPrimaId.Equals(datos.PrimaDominical.Id));
                    db.GruposPrima.RemoveRange(gps);
                    foreach (GruposPrima g in datos.PrimaRelacion)
                    {
                        g.ConfigPrimaId = datos.PrimaDominical.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }
                    db.GruposPrima.AddRange(datos.PrimaRelacion);
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

                        foreach (GruposTolerancia g in datos.ToleranciaRelacion)
                        {
                            g.ConfigToleranciaId = a.Id;
                            g.fch_Modificacion = DateTime.Now;
                        }

                        db.GruposTolerancia.AddRange(datos.ToleranciaRelacion);
                        db.SaveChanges();
                    }
                }
                else if (datos.crud == 3)
                {
                    var a = db.ConfigTolerancia.Find(datos.Tolerancia.Id);
                    db.Entry(a).State = EntityState.Modified;

                    a.Nombre = datos.Tolerancia.Nombre;
                    a.Observaciones = datos.Tolerancia.Observaciones;
                    a.UsuarioMod = datos.Tolerancia.UsuarioMod;
                    a.fch_Modificacion = DateTime.Now;

                    var cd = db.ConfigToleranciaTiempo.Where(x => x.ConfigToleranciaId.Equals(datos.Tolerancia.Id));
                    db.ConfigToleranciaTiempo.RemoveRange(cd);
                    foreach (ConfigToleranciaTiempo d in datos.ToleranciaTiempo)
                    {
                        d.ConfigToleranciaId = datos.Tolerancia.Id;
                        db.SaveChanges();
                    }
                    db.ConfigToleranciaTiempo.AddRange(datos.ToleranciaTiempo);

                    var gps = db.GruposTolerancia.Where(x => x.ConfigToleranciaId.Equals(datos.Tolerancia.Id));
                    foreach (GruposTolerancia g in datos.ToleranciaRelacion)
                    {
                        g.ConfigToleranciaId = datos.Tolerancia.Id;
                        g.fch_Modificacion = DateTime.Now;
                    }
                    db.GruposTolerancia.RemoveRange(gps);
                    db.GruposTolerancia.AddRange(datos.ToleranciaRelacion);
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
        [HttpGet]
        [Route("getCatalogoConfig")]
        [Authorize]
        public IHttpActionResult GetCatalogoConfig(string nombre)
        {
            try
            {
                if (nombre.ToLower().Equals("vacaciones"))
                {
                    try
                    {
                        var datos = db.ConfigVacaciones.OrderByDescending(o => o.fch_Modificacion).Where(x => x.Activo).Select(d => new
                        {
                            d.Id,
                            d.Nombre,
                            d.Observaciones,
                            d.DiasContinuos,
                            d.DiasExpiran,
                            d.DiasIncremento,
                            antiguedad = db.ConfigVacacionesDias.Where(x => x.ConfigVacacionesId.Equals(d.Id)).Select(dd => new
                            {
                                tiempo = dd.TiempoAntiguedadId,
                                nombre = dd.TiempoAntiguedad.Tiempo,
                                diasLey = dd.TiempoAntiguedad.DiasLey,
                                dd.Dias,
                                dd.Id
                            }).ToList(),
                            grupos = db.GrupoVacaciones.Where(x => x.ConfigVacacionesId.Equals(d.Id)).Select(g => g.GruposId).ToList(),
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
                            }).ToList(),
                            grupos = db.GruposIncapacidad.Where(x => x.ConfigIncapacidadesId.Equals(d.Id)).Select(g => g.GruposId).ToList(),
                            usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(d.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                        }).ToList();

                        var tiposIncapacidades = db.TiposIncapacidad.OrderBy(o => o.Id).Where(x => x.Activo).Select(r => new
                        {
                            r.Id,
                            r.Nombre,
                            r.Comentarios
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
                        grupos = db.GruposDiasEconomicos.Where(x => x.ConfigDiasEconomicosId.Equals(d.Id)).Select(g => g.GruposId).ToList(),
                        usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(d.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                    }).ToList();

                    var tiposDiasEconomicos = db.TiposDiasEconomicos.Where(x => x.Activo).Select(r => new
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
                        grupos = db.GruposGuardia.Where(x => x.ConfigGuardiasId.Equals(d.Id)).Select(g => g.GruposId).ToList(),
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
                        grupos = db.GruposTiempoExtra.Where(x => x.ConfigTiempoExtraId.Equals(d.Id)).Select(g => g.GruposId).ToList(),
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
                        Tipo = r.Tipo == 1 ? "Disciplinaria" : r.Tipo == 2 ? "Retardo" : r.Tipo == 3 ? "Nota Mala" : r.Tipo == 4 ? "Nota Buena" : "Acta Administrativa",
                        TipoId = r.Tipo,
                        r.ConfigSuspensionNotas.Comentarios,
                        grupos = db.GruposSuspension.Where(x => x.ConfigSuspensionNotasId.Equals(r.ConfigSuspensionNotasId)).Select(g => g.GruposId).ToList(),
                        usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(r.ConfigSuspensionNotas.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                    }).ToList();

                    return Ok(datos);
                }
                else if (nombre.ToLower().Equals("prima"))
                {
                    var datos = db.ConfigPrima.Where(x => x.Activo).Select(r => new
                    {
                        r.Horas,
                        r.porcentaje,
                        grupos = db.GruposPrima.Where(x => x.ConfigPrimaId.Equals(r.Id)).Select(g => g.GruposId).ToList(),
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
                        grupos = db.GruposTolerancia.Where(x => x.ConfigToleranciaId.Equals(r.Id)).Select(g => g.GruposId).ToList(),
                        usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(r.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
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
                if (datos.Catalogo == "horario")
                {
                    foreach (CandidatoHorario g in datos.CandidatosHorario)
                    {
                        g.fch_Modificacion = DateTime.Now;
                    }

                    //db.CandidatoHorario.AddRange(datos.CandidatosHorario);
                    //db.SaveChanges();
                }
                return Ok(HttpStatusCode.OK);

            }
            catch(Exception ex )
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
    }
}