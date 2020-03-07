using Microsoft.AspNetCore.Mvc;
using Penguin.Messaging.Application.Messages;
using Penguin.Messaging.Core;
using System.Diagnostics.CodeAnalysis;

namespace Penguin.Cms.Modules.Admin.Areas.Admin.Controllers
{
    [Area("admin")]
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
    [SuppressMessage("Design", "CA1054:Uri parameters should not be strings")]
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