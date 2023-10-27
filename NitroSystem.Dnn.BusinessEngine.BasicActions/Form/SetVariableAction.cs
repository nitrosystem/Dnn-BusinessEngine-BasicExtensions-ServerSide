using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NitroSystem.Dnn.BusinessEngine.BasicActions.Models;
using NitroSystem.Dnn.BusinessEngine.Framework.Contracts;
using NitroSystem.Dnn.BusinessEngine.Framework.Dto;
using NitroSystem.Dnn.BusinessEngine.BasicActions.Models;
using NitroSystem.Dnn.BusinessEngine.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NitroSystem.Dnn.BusinessEngine.Framework.Models;

namespace NitroSystem.Dnn.BusinessEngine.BasicActions.Form
{
    public class SetVariableAction : ActionBase<VariableInfo>, IAction
    {
        public SetVariableAction()
        {
            this.OnActionCompletedEvent += SetVariableAction_OnActionCompletedEvent;
        }

        public SetVariableAction(IServiceWorker serviceWorker, IActionWorker actionWorker, ActionDto action)
        {
            this.ServiceWorker = serviceWorker;
            this.ActionWorker = actionWorker;
            this.Action = action;

            this.OnActionCompletedEvent += SetVariableAction_OnActionCompletedEvent;
        }

        public override async Task<object> ExecuteAsync<T>(bool isServerSide)
        {
            if (isServerSide) this.ActionWorker.SetActionResults(this.Action, new { });

            return true;
        }

        public override bool TryParseModel(string actionDetails)
        {
            return true;
        }

        private void SetVariableAction_OnActionCompletedEvent(object sender, ActionEventArgs e)
        {
        }
    }
}
