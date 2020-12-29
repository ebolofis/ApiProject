﻿using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IStoreDT
    {
        /// <summary>
        /// Get Store Details
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        StoreDetailsModel GetStores(DBInfoModel Store, string storeid);
    }
}