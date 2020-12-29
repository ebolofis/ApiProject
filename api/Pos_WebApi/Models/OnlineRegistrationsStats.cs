using Pos_WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Models
{
    /// <summary>
    /// class thar holds statistics about numbers of adults and children
    /// </summary>
    public class OnlineRegistrationsStats
    {
        /// <summary>
        /// number of adults
        /// </summary>
        public int Adults { get; set; }
        /// <summary>
        /// number of children
        /// </summary>
        public int Children { get; set; }
        /// <summary>
        /// number of adults and children
        /// </summary>
        public int Total { get; set; }
       
    }
}