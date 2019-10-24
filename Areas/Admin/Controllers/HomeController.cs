using Microsoft.AspNetCore.Mvc;
using System;

namespace Penguin.Cms.Modules.Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public partial class HomeController : AdminController
    {
        public HomeController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}