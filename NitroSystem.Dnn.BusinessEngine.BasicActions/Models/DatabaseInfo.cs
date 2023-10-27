using NitroSystem.Dnn.BusinessEngine.Core.Models;
using NitroSystem.Dnn.BusinessEngine.Framework.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroSystem.Dnn.BusinessEngine.BasicActions.Models
{
   public class DatabaseInfo
    {
        public IEnumerable<ParamInfo> Params { get; set; }
        public string ResultName { get; set; }
        public string ResultListName { get; set; }
        public string TotalCountIn { get; set; }
    }
}
