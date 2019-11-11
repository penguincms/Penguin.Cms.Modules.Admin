using Penguin.Cms.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Penguin.Cms.Modules.Admin.Areas.Admin.Models
{
    public class ConnectionStringSetupModel
    {
        public List<StartupException> Exceptions { get; set; } = new List<StartupException>();
        public string ConnectionString { get; set; }
    }
}
