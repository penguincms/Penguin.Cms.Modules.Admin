using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Penguin.Cms.Modules.Admin.Areas.Admin.Models;
using Penguin.Cms.Web.Mvc;
using Penguin.Persistence.Abstractions.Interfaces;
using Penguin.Web.Mvc.Attributes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Penguin.Persistence.Abstractions;

namespace Penguin.Cms.Modules.Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SetupController : Controller
    {
        protected IApplicationLifetime AppLifetime { get; set; }

        private const string ConnectionStrings = "ConnectionStrings";

        IServiceProvider ServiceProvider { get; }

        public SetupController(IApplicationLifetime appLifetime, IServiceProvider serviceProvider)
        {
            AppLifetime = appLifetime;
            ServiceProvider = serviceProvider;
        }

        [IsLocal]
        public ActionResult Index()
        {
            return View(CheckModel());
        }

        public ConnectionStringSetupModel CheckModel(string ToCheck = null)
        {
            ToCheck ??= ServiceProvider.GetService<PersistenceConnectionInfo>()?.ConnectionString;

            ConnectionStringSetupModel toReturn = new ConnectionStringSetupModel();

            if(string.IsNullOrEmpty(ToCheck))
            {
                return toReturn;
            }

            toReturn.ConnectionString = ToCheck;

            try
            {
                using (SqlConnection connection = new SqlConnection(ToCheck))
                {
                    connection.Open();
                }

                IPersistenceContextMigrator migrator = ServiceProvider.GetService<IPersistenceContextMigrator>();

                if(migrator is null)
                {
                    throw new Exception($"{nameof(IPersistenceContextMigrator)} was returned null from the internal service provider");
                }

                migrator.Migrate();

            } catch(Exception ex)
            {
                toReturn.Exception = ex;
                return toReturn;
            }

            return toReturn;
            
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

            if(checkModel.Exception != null)
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
                (ConnectionStrings[Penguin.Persistence.Abstractions.Constants.Strings.CONNECTION_STRING_NAME] as JProperty).Value = connectionString;
            }

            System.IO.File.WriteAllText(HostBuilder.ApplicationConfig, config.ToString());

            AppLifetime.StopApplication();

            return View();
        }
    }
}