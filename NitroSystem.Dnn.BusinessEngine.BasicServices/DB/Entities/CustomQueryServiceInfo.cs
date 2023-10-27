using Dapper.Contrib.Extensions;
using NitroSystem.Dnn.BusinessEngine.Api.ViewModels;
using NitroSystem.Dnn.BusinessEngine.BasicServices.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Entities
{
    [Table("BusinessEngineBasic_CustomQueryServices")]
    public class CustomQueryServiceInfo
    {
        public Guid ItemID { get; set; }
        [ExplicitKey]
        public Guid ServiceID { get; set; }
        public DatabaseObjectType DatabaseObjectType { get; set; }
        public string StoredProcedureName { get; set; }
        public string Query { get; set; }
        public string Settings { get; set; }
    }
}
