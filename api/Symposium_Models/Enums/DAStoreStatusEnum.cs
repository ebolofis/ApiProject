using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Enums
{
    public enum DAStoreStatusEnum
    {
        Closed = 0,
        DeliveryOnly = 1,
        TakeoutOnly = 2,
        FullOperational = 4,
    }

    public enum DAShortageEnum
    {
        Temporary = 0,
        Permanent = 1,
    }

    public enum DAPriceListTypes
    {
        ForDelivery = 20,
        ForTakeOut = 21,
        ForDineIn = 22
    }

    /// <summary>
    /// Enumerator for Order Type.
    /// </summary>
    public enum OrderTypeStatus
    {
        Delivery = 20,
        TakeOut = 21,
        DineIn = 22
    }
}
