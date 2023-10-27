using Dapper.Contrib.Extensions;
using NitroSystem.Dnn.BusinessEngine.Api.ViewModels;
using NitroSystem.Dnn.BusinessEngine.BasicServices.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.CustomQuery
{
    public class CustomQueryServiceViewModel 
    {
        public Guid ItemID { get; set; }
        public Guid ServiceID { get; set; }
        public DatabaseObjectType DatabaseObjectType { get; set; }
        public string StoredProcedureName { get; set; }
        public string Query { get; set; }
        public IDictionary<string, object> Settings { get; set; }
        [Computed]
        public ServiceViewModel Service { get; set; }
    }
}
