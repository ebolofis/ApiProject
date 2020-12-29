using System;
using System.Collections.Generic;

namespace Pos_WebApi.Helpers
{
    public static class GeneralUtils
    {
        public static List<KeyValuePair<int, string>> GetEnumList<T>()
        {
            var list = new List<KeyValuePair<int, string>>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                list.Add(new KeyValuePair<int, string>((int)e, e.ToString()));
            }
            return list;
        }



    }
}
