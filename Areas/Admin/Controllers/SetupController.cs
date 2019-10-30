using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Penguin.Cms.Web.Mvc;
using Penguin.Web.Mvc.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Penguin.Cms.Modules.Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SetupController : Controller
    {
        protected IApplicationLifetime AppLifetime { get; set; }
        private const string ConnectionStrings = "ConnectionStrings";

        public SetupController(IApplicationLifetime appLifetime)
        {
            AppLifetime = appLifetime;
        }

        [IsLocal]
        public ActionResult Index()
        {
            return View();
        }

        [IsLocal]
        public ActionResult SetConnectionString(string DatabaseName, string Server, string User, string Password)
        {
            string connectionString = $"Data Source={Server};Initial Catalog={DatabaseName.Replace(".", "_")};MultipleActiveResultSets=True;";

            if (string.IsNullOrWhiteSpace(User))
            {
                connectionString += $"Integrated Security=True;";
            }
            else
            {
                connectionString += $"User ID={User};Password={Password};";
            }

            string configuration = System.IO.File.ReadAllText(HostBuilder.ApplicationConfig);

            JObject config = JObject.Parse(configuration);

            if (!config.ContainsKey(SetupController.ConnectionStrings))
            {
                config.Add(SetupController.ConnectionStrings, new JObject());
            }

            JObject ConnectionStrings = config[SetupController.ConnectionStrings] as JObject;

            if (!ConnectionStrings.ContainsKey(Penguin.Persistence.Abstractions.Constants.Strings.CONNECTION_STRING_NAME))
            {
                ConnectionStrings.Add(Penguin.Persistence.Abstractions.Constants.Strings.CONNECTION_STRING_NAME, connectionString);
            }
            else
            {
                (ConnectionStrings[Penguin.Persistence.Abstractions.Constants.Strings.CONNECTION_STRING_NAME] as JProperty).Value = connectionString;
            }

            System.IO.File.WriteAllText(HostBuilder.ApplicationConfig, config.ToString());

            AppLifetime.StopApplication();

            return View();
        }
    }
}