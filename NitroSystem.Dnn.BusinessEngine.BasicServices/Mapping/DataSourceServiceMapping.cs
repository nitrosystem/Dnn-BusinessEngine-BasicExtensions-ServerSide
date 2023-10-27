using AutoMapper;
using Newtonsoft.Json;
using NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Entities;
using NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Repositories;
using NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.DataSource;
using NitroSystem.Dnn.BusinessEngine.Common.TypeCasting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroSystem.Dnn.BusinessEngine.Api.Mapping
{
    internal static class DataSourceServiceMapping
    {
        internal static DataSourceServiceViewModel GetDataSourceServiceViewModel(Guid serviceID)
        {
            var objDataSourceServiceInfo = DataSourceServiceRepository.GetDataSourceService(serviceID);

            return GetDataSourceServiceViewModel(objDataSourceServiceInfo);
        }

        internal static DataSourceServiceViewModel GetDataSourceServiceViewModel(DataSourceServiceInfo objDataSourceServiceInfo)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DataSourceServiceInfo, DataSourceServiceViewModel>()
                .ForMember(dest => dest.Entities, map => map.MapFrom(source => TypeCastingUtil<IEnumerable<EntityInfo>>.TryJsonCasting(source.Entities)))
                .ForMember(dest => dest.JoinRelationships, map => map.MapFrom(source => TypeCastingUtil<IEnumerable<EntityJoinRelationInfo>>.TryJsonCasting(source.JoinRelationships)))
                .ForMember(dest => dest.ViewModelProperties, map => map.MapFrom(source => TypeCastingUtil<IEnumerable<ViewModelPropertyInfo>>.TryJsonCasting(source.ViewModelProperties)))
                .ForMember(dest => dest.Filters, map => map.MapFrom(source => TypeCastingUtil<IEnumerable<FilterItemInfo>>.TryJsonCasting(source.Filters)))
                .ForMember(dest => dest.SortItems, map => map.MapFrom(source => TypeCastingUtil<IEnumerable<SortItemInfo>>.TryJsonCasting(source.SortItems)))
                .ForMember(dest => dest.Settings, map => map.MapFrom(source => TypeCastingUtil<IDictionary<string, object>>.TryJsonCasting(source.Settings)));
            });

            IMapper mapper = config.CreateMapper();
            var result = mapper.Map<DataSourceServiceViewModel>(objDataSourceServiceInfo);

            return result;
        }
    }
}