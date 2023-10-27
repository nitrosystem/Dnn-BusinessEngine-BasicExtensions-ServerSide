using NitroSystem.Dnn.BusinessEngine.Core.Models;
using NitroSystem.Dnn.BusinessEngine.Framework.Contracts;
using NitroSystem.Dnn.BusinessEngine.BasicActions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroSystem.Dnn.BusinessEngine.BasicActions.Models
{
    public class RunServiceInfo 
    {
        public IEnumerable<ParamInfo> Params { get; set; }
        public IEnumerable<ExpressionInfo> OnCompleted { get; set; }
    }
}
