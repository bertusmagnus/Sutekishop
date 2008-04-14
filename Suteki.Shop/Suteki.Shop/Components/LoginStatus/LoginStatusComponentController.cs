using System.Web.Mvc;

namespace Suteki.Shop.Components.LoginStatus
{
    public class LoginStatusComponentController : ComponentControllerBase
    {
        public void Index()
        {
            RenderView("Index");
        }
    }
}
