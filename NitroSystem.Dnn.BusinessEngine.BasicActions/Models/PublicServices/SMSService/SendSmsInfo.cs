﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NitroSystem.Dnn.BusinessEngine.Core.Contract;

namespace NitroSystem.Dnn.BusinessEngine.BasicActions.Models.PublicServices.SMSService
{
    public class SendSmsInfo
    {
        public string Provider { get; set; }
        public string Mobile { get; set; }
        public string Message { get; set; }
        public IEnumerable<TokenBase> Tokens { get; set; }
    }
}
