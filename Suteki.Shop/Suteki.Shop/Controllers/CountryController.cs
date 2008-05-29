using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Common.Controllers;
using Suteki.Shop.Repositories;
using Suteki.Shop.ViewData;
using Suteki.Shop.Services;
using System.Security.Permissions;

namespace Suteki.Shop.Controllers
{
    [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
    public class CountryController : ScaffoldController<Country>
    {
    }
}