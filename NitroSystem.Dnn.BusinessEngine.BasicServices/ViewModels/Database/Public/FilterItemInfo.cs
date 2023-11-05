using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.Database.Public
{
    public class FilterItemInfo
    {
        public int Type { get; set; }
        public string CustomQuery { get; set; }
        public string ConditionGroupName { get; set; }
    }
}