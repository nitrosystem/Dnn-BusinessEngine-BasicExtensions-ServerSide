using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.Enums
{
    public enum DatabaseObjectType
    {
        [Description("StoredProcedure")]
        StoredProcedure = 0,
        [Description("QueryText")]
        QueryText = 1
    }
}
