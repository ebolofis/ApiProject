using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers
{
    public class BusinessException:Exception
    {
        public string Description{ get; set; }

        public BusinessException(string message):base(message)
        {
            
        }

        public BusinessException(string message, string description) : base(message)
        {
            this.Description = description;
        }
    }
}
