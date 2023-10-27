using Dapper;
using DotNetNuke.Abstractions.Portals;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Mail;
using Newtonsoft.Json.Linq;
using NitroSystem.Dnn.BusinessEngine.BasicActions.Models.PublicServices;
using NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Repositories;
using NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.PublicServices
{
    public class SendEmailService : ServiceBase<SendEmailInfo>, IService
    {
        public override async Task<ServiceResult> ExecuteAsync<T>()
        {
            ServiceResult result = new ServiceResult();

            try
            {
                var email = this.ParseParam(this.Model.Email);
                var subject = this.ParseParam(this.Model.Subject);
                var body = this.ParseParam(this.Model.Body);

                Mail.SendEmail("no-reply@metalverse.shop", email, subject , body);

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
