using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers
{
    public class ForkeyException : Exception
    {
        public string Description { get; set; }

        public ForkeyException(string message) : base(message)
        {

        }

        public ForkeyException(string message, string description) : base(message)
        {
            this.Description = description;
        }
    }
}
