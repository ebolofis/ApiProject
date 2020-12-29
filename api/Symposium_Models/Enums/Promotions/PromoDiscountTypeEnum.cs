using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Enums.Promotions
{
    /// <summary>
    /// Discount Type For Promotion
    /// </summary>
    public enum PromoDiscountTypeEnum
    {
        /// <summary>
        /// Discount all products
        /// </summary>
        AllDiscount = 0,

        /// <summary>
        /// Cheepest only
        /// </summary>
        CheepestOnly = 1,

        /// <summary>
        /// Most Expensive Only
        /// </summary>
        MostExpensiveOnly = 2,

        /// <summary>
        /// User Decision
        /// </summary>
        UserDecision = 3
    }
}
