using NitroSystem.Dnn.BusinessEngine.Core.Models;
using NitroSystem.Dnn.BusinessEngine.Framework.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroSystem.Dnn.BusinessEngine.BasicActions.Models
{
    public class VariableInfo 
    {
        public IEnumerable<ExpressionInfo> Properties { get; set; }
    }
}
