using Dapper.Contrib.Extensions;
using NitroSystem.Dnn.BusinessEngine.BasicServices.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Entities
{
    [Table("BusinessEngineBasic_DataSourceServices")]
    public class DataSourceServiceInfo
    {
        public Guid ItemID { get; set; }
        [ExplicitKey]
        public Guid ServiceID { get; set; }
        public Guid? ViewModelID { get; set; }
        public QueryType QueryType { get; set; }
        public DatabaseObjectType DatabaseObjectType { get; set; }
        public string StoredProcedureName { get; set; }
        public string BaseQuery { get; set; }
        public string Entities { get; set; }
        public string JoinRelationships { get; set; }
        public string ViewModelProperties { get; set; }
        public string Filters { get; set; }
        public string SortItems { get; set; }
        public bool EnablePaging { get; set; }
        public string PageIndexParam { get; set; }
        public string PageSizeParam { get; set; }
        public string Settings { get; set; }
    }
}
