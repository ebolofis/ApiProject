using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Models
{
    public partial class CredAccounts
    {
        public long Id { get; set; }
        public string Codes { get; set; }
        public double Amount { get; set; }
    }
}