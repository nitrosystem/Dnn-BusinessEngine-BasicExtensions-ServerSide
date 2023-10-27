using NitroSystem.Dnn.BusinessEngine.BasicActions.Models;
using NitroSystem.Dnn.BusinessEngine.BasicActions.Models.PublicServices.Webservice;
using NitroSystem.Dnn.BusinessEngine.Framework.Contracts;
using NitroSystem.Dnn.BusinessEngine.Framework.Dto;
using NitroSystem.Dnn.BusinessEngine.Framework.Models;
using NitroSystem.Dnn.BusinessEngine.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitroSystem.Dnn.BusinessEngine.BasicActions.PublicServices
{
    public class WebserviceRestfulAction : ActionBase<WebServiceOptions>, IAction
    {
        public WebserviceRestfulAction()
        {
            this.OnActionCompletedEvent += WebserviceRestfulAction_OnActionCompletedEvent;
        }

        public WebserviceRestfulAction(IServiceWorker serviceWorker, IActionWorker actionWorker, ActionDto action)
        {
            this.ServiceWorker = serviceWorker;
            this.ActionWorker = actionWorker;
            this.Action = action;

            this.OnActionCompletedEvent += WebserviceRestfulAction_OnActionCompletedEvent;
        }

        public override async Task<object> ExecuteAsync<T>(bool isServerSide)
        {
            var actionResult = await this.ServiceWorker.RunServiceByAction<T>(this.Action);

            this.ActionWorker.SetActionResults(this.Action, actionResult);

            return actionResult;
        }

        public override bool TryParseModel(string actionDetails)
        {
            throw new NotImplementedException();
        }

        private void WebserviceRestfulAction_OnActionCompletedEvent(object sender, ActionEventArgs e)
        {
        }
    }
}
