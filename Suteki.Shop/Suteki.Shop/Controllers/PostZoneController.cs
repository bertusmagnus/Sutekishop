using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Permissions;

namespace Suteki.Shop.Controllers
{
    [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
    public class PostZoneController : ScaffoldController<PostZone>
    {
    }
}
