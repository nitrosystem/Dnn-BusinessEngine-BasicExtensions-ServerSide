using Dapper;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using Newtonsoft.Json.Linq;
using NitroSystem.Dnn.BusinessEngine.BasicActions.Models.DnnServices;
using NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Repositories;
using NitroSystem.Dnn.BusinessEngine.BasicServices.ViewModels;
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

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.DnnServices
{
    public class LoginUserService : ServiceBase<LoginUserInfo>, IService
    {
        public override async Task<ServiceResult> ExecuteAsync<T>()
        {
            ServiceResult result = new ServiceResult();

            try
            {
                var username = this.ParseParam(this.Model.Username);
                var password = this.ParseParam(this.Model.Password);

                string ip = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";

                UserLoginStatus status = UserLoginStatus.LOGIN_FAILURE;

                var user = UserController.ValidateUser(this.PortalSettings.PortalId, username, password, "", this.PortalSettings.PortalName, ip, ref status);
                if (status == UserLoginStatus.LOGIN_SUCCESS || status == UserLoginStatus.LOGIN_SUPERUSER)
                {
                    UserController.UserLogin(this.PortalSettings.PortalId, user, this.PortalSettings.PortalName, ip, true);

                    result.IsError = false;
                    result.Data = user.UserID;
                }
                else
                {
                    result.IsError = true;
                    result.Data = status;
                    result.ErrorException = new Exception(status.ToString());
                }

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
