using Dapper.Contrib.Extensions;
using NitroSystem.Dnn.BusinessEngine.Api.ViewModels;
using NitroSystem.Dnn.BusinessEngine.BasicServices.Enums;
using NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.DataSource
{
    public class DataSourceServiceViewModel
    {
        public Guid ItemID { get; set; }
        public Guid ServiceID { get; set; }
        public Guid? ViewModelID { get; set; }
        public QueryType QueryType { get; set; }
        public DatabaseObjectType DatabaseObjectType { get; set; }
        public string StoredProcedureName { get; set; }
        public string BaseQuery { get; set; }
        public IEnumerable<EntityInfo> Entities { get; set; }
        public IEnumerable<EntityJoinRelationInfo> JoinRelationships { get; set; }
        public IEnumerable<ViewModelPropertyInfo> ViewModelProperties { get; set; }
        public IEnumerable<FilterItemInfo> Filters { get; set; }
        public IEnumerable<SortItemInfo> SortItems { get; set; }
        public bool EnablePaging { get; set; }
        public string PageIndexParam { get; set; }
        public string PageSizeParam { get; set; }
        public string CustomQuery{ get; set; }
        public IDictionary<string, object> Settings { get; set; }
        public ServiceViewModel Service { get; set; }
    }
}