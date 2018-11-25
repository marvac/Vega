using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vega.Controllers.Resources;
using Vega.Models;

namespace Vega.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //append .ReverseMap() to allow 2-way mapping
            CreateMap<Make, MakeResource>();
            CreateMap<Model, ModelResource>();
        }
    }
}
