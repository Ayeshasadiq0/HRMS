using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

using System.Configuration;

namespace HRMS_ERP
{
    public static class DatabaseConfig
    {
        /// <summary>
        /// Returns the connection string from App.config
        /// Used by ALL DAL classes
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["HRMS"].ConnectionString;
            }
        }
    }
}

