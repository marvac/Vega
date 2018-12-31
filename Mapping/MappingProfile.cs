using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vega.Controllers.Resources;
using Vega.Core.Models;

namespace Vega.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            /* Domain to API Resource */

            //append .ReverseMap() to allow 2-way mapping (if no extra params)
            CreateMap<Make, MakeResource>();
            CreateMap<Make, KeyValuePairResource>();
            CreateMap<Model, KeyValuePairResource>();
            CreateMap<Filter, FilterResource>();

            CreateMap<Vehicle, SaveVehicleResource>()
                .ForMember(r => r.Contact, options => options.MapFrom(v => new ContactResource { Name = v.ContactName, Phone = v.ContactPhone, Email = v.ContactEmail }))
                .ForMember(r => r.Features, options => options.MapFrom(v => v.Features.Select(f => f.FeatureId)));

            CreateMap<Vehicle, VehicleResource>()
                .ForMember(r => r.Make, options => options.MapFrom(v => v.Model.Make))
                .ForMember(r => r.Contact, options => options.MapFrom(v => new ContactResource { Name = v.ContactName, Phone = v.ContactPhone, Email = v.ContactEmail }))
                .ForMember(r => r.Features, options => options.MapFrom(v => v.Features.Select(f => new KeyValuePairResource { Id = f.Feature.Id, Name = f.Feature.Name })));

            /* API resource to Domain */
            CreateMap<FilterResource, Filter>();

            CreateMap<SaveVehicleResource, Vehicle>()
                .ForMember(v => v.Id, options => options.Ignore())
                .ForMember(v => v.ContactName, options => options.MapFrom(r => r.Contact.Name))
                .ForMember(v => v.ContactPhone, options => options.MapFrom(r => r.Contact.Phone))
                .ForMember(v => v.ContactEmail, options => options.MapFrom(r => r.Contact.Email))
                .ForMember(v => v.Features, options => options.Ignore())
                .AfterMap((r, v) =>
                {
                    // Remove unselected features
                    var removedFeatures = v.Features.Where(f => !r.Features.Contains(f.FeatureId)).ToList();
                    foreach (var f in removedFeatures)
                    {
                        v.Features.Remove(f);
                    }
                        
                    // Add new features
                    var addedFeatures = r.Features.Where(id => !v.Features.Any(f => f.FeatureId == id))
                    .Select(id => new VehicleFeature { FeatureId = id }).ToList();

                    foreach (var f in addedFeatures)
                    {
                        v.Features.Add(f);
                    }
                });
        }
    }
}
