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
            /* Domain to API Resource */

            //append .ReverseMap() to allow 2-way mapping (if no extra params)
            CreateMap<Make, MakeResource>();
            CreateMap<Model, ModelResource>();
            CreateMap<Vehicle, VehicleResource>()
                .ForMember(r => r.Contact, options => options.MapFrom(v => new ContactResource { Name = v.ContactName, Phone = v.ContactPhone, Email = v.ContactEmail }))
                .ForMember(r => r.Features, options => options.MapFrom(v => v.Features.Select(f => f.FeatureId)));

            //CreateMap<Feature, FeatureResource>();


            /* API resource to Domain */
            CreateMap<VehicleResource, Vehicle>()
                .ForMember(v => v.ContactName, options => options.MapFrom(r => r.Contact.Name))
                .ForMember(v => v.ContactPhone, options => options.MapFrom(r => r.Contact.Phone))
                .ForMember(v => v.ContactEmail, options => options.MapFrom(r => r.Contact.Email))
                .ForMember(v => v.Features, options => options.MapFrom(r => r.Features.Select(id => new VehicleFeature { FeatureId = id })));
        }
    }
}
