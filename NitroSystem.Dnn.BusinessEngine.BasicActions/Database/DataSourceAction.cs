using Newtonsoft.Json.Linq;
using NitroSystem.Dnn.BusinessEngine.BasicActions.Models;
using NitroSystem.Dnn.BusinessEngine.Framework.Contracts;
using NitroSystem.Dnn.BusinessEngine.Framework.Dto;
using NitroSystem.Dnn.BusinessEngine.Framework.Enums;
using NitroSystem.Dnn.BusinessEngine.Framework.Models;
using NitroSystem.Dnn.BusinessEngine.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroSystem.Dnn.BusinessEngine.BasicActions.Database
{
    public class DataSourceAction : ActionBase<DatabaseInfo>, IAction
    {
        public DataSourceAction()
        {
            this.OnActionCompletedEvent += DataSourceAction_OnActionCompletedEvent;
        }

        public DataSourceAction(IServiceWorker serviceWorker, IActionWorker actionWorker, ActionDto action)
        {
            this.ServiceWorker = serviceWorker;
            this.ActionWorker = actionWorker;
            this.Action = action;

            this.OnActionCompletedEvent += DataSourceAction_OnActionCompletedEvent;
        }

        public override async Task<object> ExecuteAsync<T>(bool isServerSide)
        {
            var actionResult = await this.ServiceWorker.RunServiceByAction<ServiceResult>(this.Action);

            object data = null;

            this.ActionWorker.SetActionResults(this.Action, actionResult);

            return actionResult;
        }

        public override bool TryParseModel(string actionDetails)
        {
            throw new NotImplementedException();
        }

        private void DataSourceAction_OnActionCompletedEvent(object sender, ActionEventArgs e)
        {

        }
    }
}
