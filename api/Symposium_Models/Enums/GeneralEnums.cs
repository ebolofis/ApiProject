using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Enums
{
    public enum FilterTypeEnum
    {
        AND = 0,
        OR = 1,
        OTHER= 10,
    }

    /// <summary>
    /// Enumeration switch policy over appconfiguration variables
    /// </summary>
    public enum ICouponApiSettingsEnum { Token = 0, GetCoupons = 1, UpdateCoupon = 2, PostSales = 3, AccountType = 4 }

    /// <summary>
    /// Enumeration of iCoupon Update action on form 
    /// </summary>
    public enum ICouponUpdateAction { Redeem = 0, Void = 1  }

    /// <summary>
    /// iCoupon.Type property makes functionality change over Discount policy
    /// </summary>
    public enum ICouponType { Discount = 1 , Tender = 2 }
}
