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
        }
    }
}