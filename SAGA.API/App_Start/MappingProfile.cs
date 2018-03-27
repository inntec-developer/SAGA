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
            Mapper.CreateMap<DAMFO_290, Damfo290Dto>();
            Mapper.CreateMap<Damfo290Dto, DAMFO_290>();
            Mapper.CreateMap<Damfo290Dto, RequisicionDto>();
            Mapper.CreateMap<Requisicion, Damfo290Dto>();
            Mapper.CreateMap<Damfo290Dto, Requisicion>();

        }
    }
}