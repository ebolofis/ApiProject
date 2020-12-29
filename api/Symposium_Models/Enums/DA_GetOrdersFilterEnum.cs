using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Enums
{
    public enum GetOrdersFilterEnum
    {
        /// <summary>
        /// all orders, no filter
        /// </summary>
        All,
        /// <summary>
        /// only historic orders (status: Canceled =5,Complete = 6, Returned=7)
        /// </summary>
        Historic,
        /// <summary>
        /// all orders but pending (status=11)
        /// </summary>
        NotPending
    }
}
