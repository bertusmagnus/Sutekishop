using System;
using System.Web.Mvc;
using Castle.Core.Logging;
using Suteki.Common.Extensions;
using Suteki.Shop.Controllers;

namespace Suteki.Shop.Views.Shared.Rescues
{
    public partial class Default : ViewPage<Exception>
    {
        protected override void OnLoad(EventArgs e)
        {
            LogException(ViewData.Model);
        }

        public void LogException(Exception exception)
        {
            var controller = this.ViewContext.Controller as ControllerBase;
            if (controller == null)
            {
                return;
            }
            ILogger logger = controller.Logger;
            if (logger == null)
            {
                return;
            }
            logger.Error("Exception in Suteki Shop", exception);
        }
    }
}