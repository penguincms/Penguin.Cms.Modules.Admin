using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Penguin.Cms.Modules.Admin.Areas.Admin.Models;
using Penguin.Cms.Web.Mvc;
using Penguin.Persistence.Abstractions;
using Penguin.Web.Mvc.Attributes;
using System;
using System.Linq;

namespace Penguin.Cms.Modules.Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SetupController : Controller
    {
        private const string ConnectionStrings = "ConnectionStrings";
        protected IApplicationLifetime AppLifetime { get; set; }
        private IServiceProvider ServiceProvider { get; }

        public SetupController(IApplicationLifetime appLifetime, IServiceProvider serviceProvider)
        {
            AppLifetime = appLifetime;
            ServiceProvider = serviceProvider;
        }

        public ConnectionStringSetupModel CheckModel(string ToCheck = null)
        {
            ToCheck ??= ServiceProvider.GetService<PersistenceConnectionInfo>()?.ConnectionString;

            ConnectionStringSetupModel toReturn = new ConnectionStringSetupModel();

            if (string.IsNullOrEmpty(ToCheck))
            {
                return toReturn;
            }

            toReturn.ConnectionString = ToCheck;

            toReturn.Exceptions = Startup.Exceptions.ToList();

            return toReturn;
        }

        [IsLocal]
        public ActionResult Index()
        {
            return View(CheckModel());
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

            ConnectionStringSetupModel checkModel = CheckModel(connectionString);

            if (checkModel.Exceptions.Any())
            {
                return View("Index", checkModel);
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
                ConnectionStrings[Penguin.Persistence.Abstractions.Constants.Strings.CONNECTION_STRING_NAME] = connectionString;
            }

            System.IO.File.WriteAllText(HostBuilder.ApplicationConfig, config.ToString());

            AppLifetime.StopApplication();

            return View();
        }
    }
}