using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Enums
{
    public enum OrderStatusEnum
    {
        /// <summary>
        /// for filtering
        /// </summary>
        All = -2,
        /// <summary>
        /// for filtering
        /// </summary>
        Open = -1,
        /// <summary>
        /// new order
        /// </summary>
        Received = 0,
        /// <summary>
        /// into kitchen (previous name: Pending)
        /// </summary>
        Preparing = 1,
        /// <summary>
        /// kitchen ready 
        /// </summary>
        Prepared = 2,
        /// <summary>
        /// ready for servicng/delivery/takeout
        /// </summary>
        Ready = 3,
        /// <summary>
        /// Onroad, delivering
        /// </summary>
        Onroad = 4,
        /// <summary>
        /// Canceled 
        /// </summary>
        Canceled = 5,
        /// <summary>
        /// completed (Is paid or Delivery boy returned or Is taken out)
        /// </summary>
        Complete = 6,
        /// <summary>
        /// customer never found (delivery)
        /// </summary>
        Returned = 7,
        /// <summary>
        /// Pending and Ready For Filter
        /// </summary>
        PendingReady = 8,
        /// <summary>
        /// Pending and Onroad For Filter
        /// </summary>
        PendingOnroad = 9,
        /// <summary>
        /// Ready and Onroad For Filter
        /// </summary>
        ReadyOnroad = 10,
        /// <summary>
        /// waiting confirmation from external bank payment
        /// </summary>
        OnHold = 11
    }

    public enum OrderOriginEnum
    {
        Local = 0,
        Web = 1,
        CallCenter = 2,
        MobileApp = 3,
        Delivery = 4
    }

    public enum UpdateStoreTableActionEnum
    {
        Insert = 0,
        Delete = 1,
        Update = 2
    }
}
