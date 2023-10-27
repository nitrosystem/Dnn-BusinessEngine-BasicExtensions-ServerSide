using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels.ServiceModels
{
    public class ServiceUserLoginOptions
    {
        public bool RememberMe { get; set; }
        public bool EmailAsUsername { get; set; }
        public bool MobileAsUsername { get; set; }
    }
}