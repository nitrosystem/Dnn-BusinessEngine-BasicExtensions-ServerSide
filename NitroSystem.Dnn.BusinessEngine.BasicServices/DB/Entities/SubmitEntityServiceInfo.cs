using Dapper.Contrib.Extensions;
using NitroSystem.Dnn.BusinessEngine.Api.ViewModels;
using NitroSystem.Dnn.BusinessEngine.BasicServices.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Entities
{
    [Table("BusinessEngineBasic_SubmitEntityServices")]
    public class SubmitEntityServiceInfo
    {
        public Guid ItemID { get; set; }
        [ExplicitKey]
        public Guid ServiceID { get; set; }
        public Guid EntityID { get; set; }
        public string BaseQuery { get; set; }
        public string InsertBaseQuery { get; set; }
        public string UpdateBaseQuery { get; set; }
        public DatabaseObjectType DatabaseObjectType { get; set; }
        public string StoredProcedureName { get; set; }
        public QueryType QueryType { get; set; }
        public ActionType ActionType { get; set; }
        public string Entity { get; set; }
        public string Settings { get; set; }
    }
}