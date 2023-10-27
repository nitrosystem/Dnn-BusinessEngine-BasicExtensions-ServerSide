using AutoMapper;
using Newtonsoft.Json;
using NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Entities;
using NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Repositories;
using NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.SubmitEntity;
using NitroSystem.Dnn.BusinessEngine.Common.TypeCasting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroSystem.Dnn.BusinessEngine.Api.Mapping
{
    internal static class SubmitEntityServiceMapping
    {
        internal static SubmitEntityServiceViewModel GetSubmitEntityServiceViewModel(Guid serviceID)
        {
            var objSubmitEntityServiceInfo = SubmitEntityServiceRepository.GetSubmitEntityService(serviceID);

            return GetSubmitEntityServiceViewModel(objSubmitEntityServiceInfo);
        }

        internal static SubmitEntityServiceViewModel GetSubmitEntityServiceViewModel(SubmitEntityServiceInfo objSubmitEntityServiceInfo)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SubmitEntityServiceInfo, SubmitEntityServiceViewModel>()
                .ForMember(dest => dest.Entity, map => map.MapFrom(source => TypeCastingUtil<EntityInfo>.TryJsonCasting(source.Entity)))
                .ForMember(dest => dest.Settings, map => map.MapFrom(source => TypeCastingUtil<IDictionary<string, object>>.TryJsonCasting(source.Settings)));
            });

            IMapper mapper = config.CreateMapper();
            var result = mapper.Map<SubmitEntityServiceViewModel>(objSubmitEntityServiceInfo);

            return result;
        }
    }
}