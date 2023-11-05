using Dapper.Contrib.Extensions;
using NitroSystem.Dnn.BusinessEngine.Api.ViewModels;
using NitroSystem.Dnn.BusinessEngine.BasicServices.Enums;
using NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database;
using NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.Public;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.BindEntity
{
    public class BindEntityServiceViewModel
    {
        public Guid ItemID { get; set; }
        public Guid ServiceID { get; set; }
        public Guid EntityID { get; set; }
        public QueryType QueryType { get; set; }
        public DatabaseObjectType DatabaseObjectType { get; set; }
        public string StoredProcedureName { get; set; }
        public string BaseQuery { get; set; }
        public IEnumerable<EntityInfo> Entities { get; set; }
        public IEnumerable<EntityJoinRelationInfo> JoinRelationships { get; set; }
        public IEnumerable<EntityColumnInfo> EntityColumns { get; set; }
        public IEnumerable<FilterItemInfo> Filters { get; set; }
        public string CustomQuery{ get; set; }
        public IDictionary<string, object> Settings { get; set; }
        public ServiceViewModel Service { get; set; }
    }
}