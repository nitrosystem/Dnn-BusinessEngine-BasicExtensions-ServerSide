using AutoMapper;
using Newtonsoft.Json;
using NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Entities;
using NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Repositories;
using NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.CustomQuery;
using NitroSystem.Dnn.BusinessEngine.Common.TypeCasting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroSystem.Dnn.BusinessEngine.Api.Mapping
{
    internal static class CustomQueryServiceMapping
    {
        internal static CustomQueryServiceViewModel GetCustomQueryServiceViewModel(Guid serviceID)
        {
            var objCustomQueryServiceInfo = CustomQueryServiceRepository.GetCustomQueryService(serviceID);

            return GetCustomQueryServiceViewModel(objCustomQueryServiceInfo);
        }

        internal static CustomQueryServiceViewModel GetCustomQueryServiceViewModel(CustomQueryServiceInfo objCustomQueryServiceInfo)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CustomQueryServiceInfo, CustomQueryServiceViewModel>()
                .ForMember(dest => dest.Settings, map => map.MapFrom(source => TypeCastingUtil<IDictionary<string, object>>.TryJsonCasting(source.Settings)));
            });

            IMapper mapper = config.CreateMapper();
            var result = mapper.Map<CustomQueryServiceViewModel>(objCustomQueryServiceInfo);

            return result;
        }
    }
}