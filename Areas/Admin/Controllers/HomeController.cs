using Microsoft.AspNetCore.Mvc;
using Penguin.Security.Abstractions.Interfaces;
using System;

namespace Penguin.Cms.Modules.Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public partial class HomeController : AdminController
    {
        public HomeController(IServiceProvider serviceProvider, IUserSession userSession) : base(serviceProvider, userSession)
        {
        }
    }
}