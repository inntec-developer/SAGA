﻿using SAGA.DAL;
using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.API.Dtos;
using SAGA.API.Controllers.Admin;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Catalogos")]
    public class CatalogosController : ApiController
    {
        private SAGADBContext db;
        public CatalogosController()
        {
            db = new SAGADBContext();
        }

        #region Catalogos

        [HttpGet]
        [Route("getDocDamsa")]
        public IHttpActionResult GetDocDamsa()
        {
            try
            {
                var documentosDamsa = db.DocumentosDamsa.ToList();
                return Ok(documentosDamsa);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet]
        [Route("getPrestacionesLey")]
        public IHttpActionResult GetPrestacionesLey()
        {
            try
            {
                var prestacionesLey = db.PrestacionesLey.ToList();
                return Ok(prestacionesLey);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet]
        [Route("getPaises")]
        public IHttpActionResult GetPaises()
        {
            CandidatosDto Paises = new CandidatosDto();

            Paises.Paises = (from pais in db.Paises
                             where pais.Id == 42
                             select pais).ToList();

            return Ok(Paises);
        }

        [HttpGet]
        [Route("getDepa")]
        public IHttpActionResult getDepartamento()
        {
            var tu = db.Departamentos.Select(t => new { t.Id, t.Nombre }).OrderBy(x => x.Nombre).ToList();
            return Ok(tu);
        }

        [HttpGet]
        [Route("getTipos")]
        public IHttpActionResult getTiposUsuarios()
        {
            var tu = db.TiposUsuarios.Select(t => new { t.Id, t.Tipo }).ToList();
            return Ok(tu);
        }

        [HttpGet]
        [Route("getGrupos")]
        public IHttpActionResult getGrupos()
        {
            PersonalController obj = new PersonalController();
            List<GruposDtos> data = new List<GruposDtos>();
            var grupos = db.Grupos.Select(g => new
            {
                Id = g.Id,
                Foto = g.Foto,
                Activo = g.Activo,
                Descripcion = g.Descripcion,
                Nombre = g.Nombre,
                UsuarioAlta = g.UsuarioAlta,
                TipoGrupoId = db.TiposUsuarios.Where(x => x.Id.Equals(g.TipoGrupoId)).Select(id => id.Id).FirstOrDefault(),
                TipoGrupo = db.TiposUsuarios.Where(x => x.Id.Equals(g.TipoGrupoId)).Select(n => n.Tipo).FirstOrDefault()
            }).OrderBy(g => g.Nombre).ToList();

            foreach (var g in grupos)
            {
                var aux = obj.GetImage(g.Foto);
                data.Add(new GruposDtos
                {
                    Id = g.Id,
                    Foto = g.Foto,
                    Activo = g.Activo,
                    Descripcion = g.Descripcion,
                    Nombre = g.Nombre,
                    UsuarioAlta = g.UsuarioAlta,
                    TipoGrupoId = g.TipoGrupoId,
                    TipoGrupo = g.TipoGrupo,
                    FotoAux = aux
                });
            }

            obj = null;
            return Ok(data);
        }

        [HttpGet]
        [Route("getRoles")]
        public IHttpActionResult getRoles()
        {
            var roles = db.Roles.Where(x => x.Activo).ToList();
            return Ok(roles);
        }

        [HttpGet]
        [Route("getPrioridades")]
        public IHttpActionResult GetRPioridad()
        {
            var prioridad = db.Prioridades
                .Where(x => x.Activo.Equals(true))
                .ToList();
            return Ok(prioridad);
        }

        [HttpGet]
        [Route("getEstatus")]
        public IHttpActionResult getEstatus(int tipoMov)
        {
            var estatus = db.Estatus
                    .Where(x => x.TipoMovimiento.Equals(tipoMov))
                    .Where(x => x.Activo.Equals(true))
                    .ToList();
            return Ok(estatus);
        }

        [HttpGet]
        [Route("getMotivosLiberacion")]
        public IHttpActionResult GetMotivosLiberacion()
        {
            var motivo = db.MotivosLiberacion.Where(m => m.Activo.Equals(true)).ToList();
            return Ok(motivo);
        }

        [HttpGet]
        [Route("getTiposActividadesRecl")]
        public IHttpActionResult GetTiposActividadesRecl()
        {
            var actividad = db.TipoActividadReclutador
                                .Where(x => x.Activo.Equals(true))
                                .ToList().OrderBy(x => x.Actividad);
            return Ok(actividad);
        }

        [HttpGet]
        [Route("getTipoTelefono")]
        public IHttpActionResult GetTipoTelefono()
        {
            var tipo = db.TiposTelefonos.ToList();
            return Ok(tipo);
        }

        [HttpGet]
        [Route("getTipoDireccion")]
        public IHttpActionResult GetTipoDireccion()
        {
            var tipo = db.TiposDirecciones.ToList();
            return Ok(tipo);
        }

        #endregion

        #region Catalogos para Prospectos Clientes

        [HttpGet]
        [Route("getGiroEmp")]
        public IHttpActionResult GetGiroEmpresa()
        {
            var giro = db.GirosEmpresas.ToList();
            return Ok(giro);
        }

        [HttpGet]
        [Route("getActividadEmp")]
        public IHttpActionResult GetActivadadEmpresa(int GiroId)
        {
            var Actividad = db.ActividadesEmpresas
                .Select(x => new
                {
                    x.Id,
                    x.GiroEmpresaId,
                    x.actividadEmpresa
                })
                .Where(x => x.GiroEmpresaId.Equals(GiroId)).ToList();
            return Ok(Actividad);
        }

        [HttpGet]
        [Route("getTamanioEmp")]
        public IHttpActionResult GetTamanioEmpresa()
        {
            var tamanio = db.TamanoEmpresas.ToList();
            return Ok(tamanio);
        }

        [HttpGet]
        [Route("getTipoEmp")]
        public IHttpActionResult GetTipoempresa()
        {
            var tipo = db.TiposEmpresas.ToList();
            return Ok(tipo);
        }

        [HttpGet]
        [Route("getTipoBase")]
        public IHttpActionResult GetTipoBase()
        {
            var tipo = db.TiposBases.ToList();
            return Ok(tipo);
        }
        #endregion

        #region Localidades
        [HttpGet]
        [Route("getPais")]
        public IHttpActionResult GetPais()
        {
            var pais = db.Paises.Where(x => x.Id.Equals(42)).ToList();
            return Ok(pais);
        }

        [HttpGet]
        [Route("getEstado")]
        public IHttpActionResult GetEstado(int PaisId)
        {
            var estado = db.Estados
                .OrderBy(x => x.estado)
                .Where(x => x.PaisId.Equals(PaisId))
                .Select(x => new
                {
                    x.Id,
                    x.estado
                })
                .ToList();
            return Ok(estado);
        }

        [HttpGet]
        [Route("getMunicipio")]
        public IHttpActionResult GetMunicipo(int EstadoId)
        {
            var municipio = db.Municipios
                .OrderBy(x => x.municipio)
                .Where(x => x.EstadoId.Equals(EstadoId))
                .Select(x => new
                {
                    x.Id,
                    x.municipio
                })
                .ToList();
            return Ok(municipio);
        }

        [HttpGet]
        [Route("getColonia")]
        public IHttpActionResult GetColonias(int MunicipioId)
        {
            var municipio = db.Colonias
                .OrderBy(x => x.colonia)
                .Where(x => x.MunicipioId.Equals(MunicipioId))
                .Select(x => new
                {
                    x.Id,
                    x.colonia,
                    x.CP
                })
                .ToList();
            return Ok(municipio);
        }

        [HttpGet]
        [Route("getInfoCP")]
        public IHttpActionResult GetInfoCP(string CP)
        {
            var info = db.Colonias
                .Where(x => x.CP.Equals(CP))
                .Select(x => new
                {
                    x.Id,
                    x.colonia,
                    x.CP,
                    x.PaisId,
                    x.EstadoId,
                    x.MunicipioId,
                }).ToList().OrderBy(x => x.colonia);
            return Ok(info);
        }
        #endregion

        #region Menu de Catalogos

        [HttpGet]
        [Route("getCatalogos")]
        public IHttpActionResult GetCatalogos()
        {
            var Catalogos = db.Estructuras
                .Select(x => new
                {
                    x.Id,
                    x.IdPadre,
                    x.Nombre,
                    x.Descripcion,
                    x.Activo,
                    Catalogos = db.Catalogos.Where( c => c.EstructuraId == x.Id).ToList()
                })
                .Where(e => e.Activo.Equals(true) && e.IdPadre.Equals(1) && e.Id != 1 && e.Catalogos.Count > 0)
                .ToList();

            return Ok(Catalogos);
        }

        [HttpGet]
        [Route("getCatalogosComplete")]
        public IHttpActionResult getCatalogosComplete(int IdCatalogo )
        {
            CatalogosDto Catalogo = new CatalogosDto();
            //Buscamos los datos del catalogo
            Catalogo.Catalogos = db.Catalogos
             .Where(c => c.Id.Equals(IdCatalogo))
             .SingleOrDefault();

            switch (IdCatalogo)
            {
                #region Sistemas
                case 1: // Paises

                    Catalogo.Pais = db.Paises
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 2: // Estados

                    Catalogo.Pais = db.Paises.ToList();
                    Catalogo.Estado = db.Estados
                        .Select( e => new EstadoDto
                        {
                            Id = e.Id,
                            estado = e.estado,
                            Clave = e.Clave,
                            Pais = e.Pais.pais,
                            Activo = e.Activo                   
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 3: // Municipios

                    //Catalogo.Pais = db.Paises.ToList();
                    Catalogo.Estado = db.Estados
                        .Select(e => new EstadoDto
                        {
                            Id = e.Id,
                            estado = e.estado,
                            Clave = e.Clave,
                            Pais = e.Pais.pais,
                            Activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 4: // Colonias

                    Catalogo.Pais = db.Paises.ToList();
                    Catalogo.Estado = db.Estados
                        .Select(e => new EstadoDto
                        {
                            Id = e.Id,
                            estado = e.estado,
                            Clave = e.Clave,
                            Pais = e.Pais.pais,
                            Activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();
                    Catalogo.Municipio = db.Municipios
                        .Select(m => new MunicipioDto
                        {
                            Id = m.Id,
                            municipio = m.municipio,
                            Estado = m.Estado.estado,
                            Activo = m.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();
                    

                    break;

                case 6: // Tipo de telefonos

                    Catalogo.TpTelefono = db.TiposTelefonos
                        .Select(t => new TpTelefonosDto
                        {
                            Id = t.Id,
                            Tipo = t.Tipo,
                            Activo = t.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 7: // Estados Civiles

                    Catalogo.EstadoCivil = db.EstadosCiviles
                        .Select(t => new EstadoCivilDto
                        {
                            Id = t.Id,
                            estadoCivil = t.estadoCivil,
                            Activo = t.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 41: // Tipo de usuario

                    Catalogo.TpUsuario = db.TiposUsuarios
                        .Select(t => new TpUsuarioDto
                        {
                            Id = t.Id,
                            tipo = t.Tipo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                #endregion

                #region Reclutamiento
                #endregion

                #region Ventas
                #endregion

                default:
                    break;
            }

            return Ok(Catalogo);
        }

        [HttpPost]
        [Route("FilterCatalogo")]
        public IHttpActionResult getCatalogofilter(ParamsDto Parametros)
        {
            CatalogosDto Catalogo = new CatalogosDto();
            //Buscamos los datos del catalogo
            Catalogo.Catalogos = db.Catalogos
             .Where(c => c.Id.Equals(Parametros.IdCat))
             .SingleOrDefault();

            if (Parametros.IdCat == 3) // Municipios
            {
                Catalogo.Estado = db.Estados
                       .Select(e => new EstadoDto
                       {
                           Id = e.Id,
                           estado = e.estado,
                           Clave = e.Clave,
                           Pais = e.Pais.pais,
                           Activo = e.Activo
                       })
                       .ToList();
                Catalogo.Municipio = db.Municipios
                    .Where(m => m.Estado.Id.Equals(Parametros.IdEstado))
                    .Select(m => new MunicipioDto
                    {
                        Id = m.Id,
                        municipio = m.municipio,
                        Estado = m.Estado.estado,
                        Activo = m.Activo
                    })
                    .OrderBy(c => c.Id)
                    .ToList();
            }
            else // Colonias
            {
                Catalogo.Pais = db.Paises.ToList();
                Catalogo.Estado = db.Estados
                    .Select(e => new EstadoDto
                    {
                        Id = e.Id,
                        estado = e.estado,
                        Clave = e.Clave,
                        Pais = e.Pais.pais,
                        Activo = e.Activo
                    })
                    .ToList();
                Catalogo.Municipio = db.Municipios
                    .Select(m => new MunicipioDto
                    {
                        Id = m.Id,
                        municipio = m.municipio,
                        Estado = m.Estado.estado,
                        Activo = m.Activo
                    })
                    .ToList();

                Catalogo.Colonia = db.Colonias
                    .Where(c => c.Municipio.Id == Parametros.IdMunicipio)
                    .Select(c => new ColoniasDto
                    {
                        Id = c.Id,
                        colonia = c.colonia,
                        TipoColonia = c.TipoColonia,
                        CP = c.CP,
                        Activo = c.Activo,
                        Pais = c.Pais.pais,
                        Estado = c.Estado.estado,
                        Municipio = c.Municipio.municipio
                    })
                    .OrderBy(c => c.Id)
                    .ToList();
            }

            return Ok(Catalogo);
        }

        [HttpPost]
        [Route("postCatalogo")]
        public IHttpActionResult postCatalogos(CatalogosDto Catalogo)
        {
            if (Catalogo.opt == 1) // Agregar
            {
                switch(Catalogo.Catalogos.Id) // ¿ Que catalogo es ?
                {
                    #region sistemas
                    case 1: // País

                        db.Paises.Add(Catalogo.Pais[0]);
                        db.SaveChanges();

                        break;

                    case 2: // Estado

                        Estado estado = new Estado();
                        
                        estado.Activo = Catalogo.Estado[0].Activo;
                        estado.estado = Catalogo.Estado[0].estado;
                        estado.Clave = Catalogo.Estado[0].Clave;
                        estado.PaisId = Convert.ToInt32(Catalogo.Estado[0].Pais);

                        db.Estados.Add(estado);
                        db.SaveChanges();

                        break;

                    case 3: // Municipios

                        Municipio municipio = new Municipio();

                        municipio.municipio = Catalogo.Municipio[0].municipio;
                        municipio.EstadoId = Convert.ToInt32(Catalogo.Municipio[0].Estado);
                        municipio.Activo = Catalogo.Municipio[0].Activo;

                        db.Municipios.Add(municipio);
                        db.SaveChanges();

                        break;

                    case 4: // Colonias

                        Colonia colonia = new Colonia();

                        colonia.colonia = Catalogo.Colonia[0].colonia;
                        colonia.TipoColonia = Catalogo.Colonia[0].TipoColonia;
                        colonia.CP = Catalogo.Colonia[0].CP;
                        colonia.EstadoId = Convert.ToInt32(Catalogo.Colonia[0].Estado);
                        colonia.MunicipioId = Convert.ToInt32(Catalogo.Colonia[0].Municipio);
                        colonia.PaisId = Convert.ToInt32(Catalogo.Colonia[0].Pais);
                        colonia.Activo = Catalogo.Colonia[0].Activo;

                        db.Colonias.Add(colonia);
                        db.SaveChanges();

                        break;

                    case 6: // Tipo de telefonos

                        TipoTelefono TpTelefonos = new TipoTelefono();

                        TpTelefonos.Tipo = Catalogo.TpTelefono[0].Tipo;
                        TpTelefonos.Activo = Catalogo.TpTelefono[0].Activo;

                        db.TiposTelefonos.Add(TpTelefonos);
                        db.SaveChanges();

                        break;

                    case 7: // Estado civil

                        EstadoCivil estadocivil = new EstadoCivil();

                        estadocivil.estadoCivil = Catalogo.EstadoCivil[0].estadoCivil;
                        estadocivil.Activo = Catalogo.EstadoCivil[0].Activo;

                        db.EstadosCiviles.Add(estadocivil);
                        db.SaveChanges();

                        break;

                    case 41: // Tipo de usuarios

                        TipoUsuario TpUsuario = new TipoUsuario();

                        TpUsuario.Tipo = Catalogo.TpUsuario[0].tipo;

                        db.TiposUsuarios.Add(TpUsuario);
                        db.SaveChanges();

                        break;

                        #endregion

                    #region Reclutamiento
                        #endregion

                    #region Ventas
                        #endregion
                }
            }
            else // Modificar
            {
                switch (Catalogo.Catalogos.Id) // ¿ Que catalogo es ?
                {
                    #region Sistema

                    case 1: // Países

                        db.Entry(Catalogo.Pais[0]).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 2: // Estados

                        Estado estado = new Estado();

                        estado.Id = Catalogo.Estado[0].Id;
                        estado.Activo = Catalogo.Estado[0].Activo;
                        estado.estado = Catalogo.Estado[0].estado;
                        estado.Clave = Catalogo.Estado[0].Clave;
                        estado.PaisId = Convert.ToInt32(Catalogo.Estado[0].Pais);

                        db.Entry(estado).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 3: // Municipios

                        Municipio municipio = new Municipio();

                        municipio.Id = Catalogo.Municipio[0].Id;
                        municipio.municipio = Catalogo.Municipio[0].municipio;
                        municipio.EstadoId = Convert.ToInt32(Catalogo.Municipio[0].Estado);
                        municipio.Activo = Catalogo.Municipio[0].Activo;

                        db.Entry(municipio).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 4: // Colonias

                        Colonia colonia = new Colonia();

                        colonia.Id = Catalogo.Colonia[0].Id;
                        colonia.colonia = Catalogo.Colonia[0].colonia;
                        colonia.TipoColonia = Catalogo.Colonia[0].TipoColonia;
                        colonia.CP = Catalogo.Colonia[0].CP;
                        colonia.EstadoId = Convert.ToInt32(Catalogo.Colonia[0].Estado);
                        colonia.MunicipioId = Convert.ToInt32(Catalogo.Colonia[0].Municipio);
                        colonia.PaisId = Convert.ToInt32(Catalogo.Colonia[0].Pais);
                        colonia.Activo = Catalogo.Colonia[0].Activo;

                        db.Entry(colonia).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 6: // Tipo de telefonos

                        TipoTelefono TpTelefonos = new TipoTelefono();

                        TpTelefonos.Id = Catalogo.TpTelefono[0].Id;
                        TpTelefonos.Tipo = Catalogo.TpTelefono[0].Tipo;
                        TpTelefonos.Activo = Catalogo.TpTelefono[0].Activo;


                        db.Entry(TpTelefonos).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 7: // Estado civil

                        EstadoCivil estadocivil = new EstadoCivil();

                        estadocivil.Id = Catalogo.EstadoCivil[0].Id;
                        estadocivil.estadoCivil = Catalogo.EstadoCivil[0].estadoCivil;
                        estadocivil.Activo = Catalogo.EstadoCivil[0].Activo;

                        db.Entry(estadocivil).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 41: // Tipo de usuarios

                        TipoUsuario TpUsuario = new TipoUsuario();

                        TpUsuario.Id = Catalogo.TpUsuario[0].Id;
                        TpUsuario.Tipo = Catalogo.TpUsuario[0].tipo;

                        db.Entry(TpUsuario).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                        #endregion

                    #region Reclutamiento
                        #endregion

                    #region Ventas
                        #endregion
                }
            }

            return Ok(true);
        }
        #endregion
    }
}