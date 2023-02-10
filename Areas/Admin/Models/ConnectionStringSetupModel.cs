using Penguin.Cms.Web.Mvc;
using System.Collections.Generic;

namespace Penguin.Cms.Modules.Admin.Areas.Admin.Models
{
    /// <summary>
    /// Model used to display the database configuration page when database connection fails on initialization
    /// </summary>
    public class ConnectionStringSetupModel
    {
        /// <summary>
        /// The last attempted connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Any exceptions that occurred when initializing the database connection
        /// </summary>
        public List<StartupException> Exceptions { get; } = new List<StartupException>();
    }
}