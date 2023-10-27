using NitroSystem.Dnn.BusinessEngine.BasicActions.Models;
using NitroSystem.Dnn.BusinessEngine.Core.Manager;
using NitroSystem.Dnn.BusinessEngine.Core.Models;
using NitroSystem.Dnn.BusinessEngine.Data.Repositories;
using NitroSystem.Dnn.BusinessEngine.Framework.Contracts;
using NitroSystem.Dnn.BusinessEngine.Framework.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NitroSystem.Dnn.BusinessEngine.BasicActions.Programming
{
    public class RunService : ActionBase<RunServiceInfo>, IAction
    {
        public RunService()
        {
            this.OnActionCompletedEvent += RunService_OnActionCompletedEvent;
        }

        public override async Task ExecuteAsync()
        {
            foreach (var param in this.Model.Params ?? Enumerable.Empty<ParamInfo>())
            {
                param.ParamValue = this.ExpressionService.ParseExpression(param.ParamValue, this.ModuleData);
            }

            var actionResult = await this.ServiceWorker.RunService(this.Action.ServiceID.Value, this.Action.ActionName, this.Model.Params);

            this.ActionWorker.SetActionResults(this.Action, actionResult);
        }

        public override bool TryParseModel(string actionDetails)
        {
            throw new NotImplementedException();
        }

        private void RunService_OnActionCompletedEvent(object sender, ActionEventArgs e)
        {

        }
    }
}
