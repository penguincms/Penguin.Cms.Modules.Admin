using Microsoft.AspNetCore.Mvc;
using Penguin.Messaging.Application.Messages;
using Penguin.Messaging.Core;

namespace Penguin.Cms.Modules.Admin.Areas.Admin.Controllers
{
    [Area("admin")]
    public partial class InitController : Controller
    {
        private MessageBus MessageBus { get; set; }

        public InitController(MessageBus messageBus)
        {
            this.MessageBus = messageBus;
        }

        public ActionResult Index()
        {
            this.MessageBus.Send(new Setup());

            return this.Content("If you're seeing this, everything is probably OK");
        }
    }
}