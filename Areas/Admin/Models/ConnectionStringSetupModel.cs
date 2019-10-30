using System;
using System.Collections.Generic;
using System.Text;

namespace Penguin.Cms.Modules.Admin.Areas.Admin.Models
{
    public class ConnectionStringSetupModel
    {
        public Exception Exception { get; set; }
        public string ConnectionString { get; set; }
    }
}
