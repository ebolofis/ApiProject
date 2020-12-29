using System.Collections.Generic;


namespace Symposium.Helpers
{
    public static class KdsOrderIdListHelper
    {
        public static List<long> KdsOrdersIdStaticList = new List<long>();
        static object lockObj = 0;

        public static List<long> readKdsOrderIdsList()
        {
            lock (lockObj)
            {
                return KdsOrdersIdStaticList;
            }
        }

        public static List<long> addKdsOrderIdsToList(long OrderId)
        {
            lock (lockObj)
            {
                if (KdsOrdersIdStaticList.Contains(OrderId) == false)
                {
                    KdsOrdersIdStaticList.Add(OrderId);
                }
                return KdsOrdersIdStaticList;
            }
        }

        public static bool clearKdsOrderIdsList()
        {
            lock (lockObj)
            {
                KdsOrdersIdStaticList = new List<long>();
                return true;
            }
        }
    }
}
