using Dapper;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using Newtonsoft.Json.Linq;
using NitroSystem.Dnn.BusinessEngine.BasicActions.Models.PublicServices.SMSService;
using NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Repositories;
using NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels;
using NitroSystem.Dnn.BusinessEngine.Core.Contract;
using NitroSystem.Dnn.BusinessEngine.Core.Providers.SmsGateway;
using NitroSystem.Dnn.BusinessEngine.Data.Entities.Tables;
using NitroSystem.Dnn.BusinessEngine.Framework.Contracts;
using NitroSystem.Dnn.BusinessEngine.Framework.Models;
using NitroSystem.Dnn.BusinessEngine.Framework.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.PublicServices
{
    public class SendSmsService : ServiceBase<SendSmsInfo>, IService
    {
        public override async Task<ServiceResult> ExecuteAsync<T>()
        {
            ServiceResult result = new ServiceResult();

            try
            {
                var provider = this.ParseParam(this.Model.Provider);
                var mobile = this.ParseParam(this.Model.Mobile);
                var message = this.ParseParam(this.Model.Message);
                
                foreach (var token in this.Model.Tokens ?? Enumerable.Empty<TokenBase>())
                {
                    token.TokenValue = this.ParseParam((string)token.TokenValue);
                }

                SmsGatewayService.SendSms(this.PortalSettings, provider, mobile, message, this.Model.Tokens);

                return result;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.ErrorException = ex;
            }

            return result;
        }

        public override bool TryParseModel(string serviceSettings)
        {
            throw new NotImplementedException();
        }
    }
}
