using Penguin.Cms.Web.Mvc;
using System.Collections.Generic;

namespace Penguin.Cms.Modules.Admin.Areas.Admin.Models
{
    public class ConnectionStringSetupModel
    {
        public string ConnectionString { get; set; }
        public List<StartupException> Exceptions { get; set; } = new List<StartupException>();
    }
}