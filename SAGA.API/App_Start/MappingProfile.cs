using AutoMapper;
using SAGA.DAL;
using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.API.Dtos;

namespace SAGA.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<Usuarios, PersonasDtos>();
            Mapper.CreateMap<PersonasDtos, Usuarios>();
            Mapper.CreateMap<Grupos, GruposDtos>();
            Mapper.CreateMap<GruposDtos, Grupos>();

            Mapper.CreateMap<AsignacionRequi, AsignadosDto>();
            Mapper.CreateMap<AsignadosDto, AsignacionRequi>();

            Mapper.CreateMap<Cliente, ProspectoDto>();
            Mapper.CreateMap<ProspectoDto, Cliente>();
            Mapper.CreateMap<Direccion, DireccionClienteDto>();
            Mapper.CreateMap<DireccionClienteDto, Direccion>();
            Mapper.CreateMap<Email, EmailClienteDto>();
            Mapper.CreateMap<EmailClienteDto, Email>();
            Mapper.CreateMap<Telefono, TelefonoClienteDto>();
            Mapper.CreateMap<TelefonoClienteDto, Telefono>();
            Mapper.CreateMap<Contacto, ContactoClienteDto>();
            Mapper.CreateMap<ContactoClienteDto, Contacto>();
        }
    }
}