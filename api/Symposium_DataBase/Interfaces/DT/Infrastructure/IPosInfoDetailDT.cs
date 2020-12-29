﻿using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{

    /// <summary>
    /// Class that handles data related to Pos Info Detail
    /// </summary>
    public interface IPosInfoDetailDT
    {

        /// <summary>
        /// Get pos info detail according to selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="PosInfoDetailId"> PosInfoDetail </param>
        /// <returns> Pos info detail model. See: <seealso cref="Symposium.Models.Models.PosInfoDetailModel"/> </returns>
        PosInfoDetailModel GetSinglePosInfoDetail(DBInfoModel Store, long PosInfoDetailId, IDbConnection dbTran = null, IDbTransaction dbTransact = null);

        /// <summary>
        /// Get pos info detail according to selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="PosInfoDetailId"> PosInfoDetail </param>
        /// <returns> Pos info detail model. See: <seealso cref="Symposium.Models.Models.PosInfoDetailModel"/> </returns>
        PosInfoDetailModel GetSinglePosInfoDetailByposId(DBInfoModel Store, long PosInfoId);

        /// <summary>
        /// Return new counter for specific PosIfo and PosInfoDetail
        /// Update's PosInfoDetail all record's for same GroupId and posinfo id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <returns></returns>
       long GetNextCounter(DBInfoModel Store, long PosInfoId, long PosInfoDetailId);

        /// <summary>
        /// Return new counter for specific PosIfo and PosInfoDetail
        /// Update's PosInfoDetail all record's for same GroupId and posinfo id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <param name="InvoiceTypeType"></param>  
        /// <returns></returns>
        long GetNextCounter(DBInfoModel Store, long PosInfoId, long PosInfoDetailId, int InvoiceTypeType);

        /// <summary>
        /// Return new counter for specific PosIfo and PosInfoDetail
        /// Update's PosInfoDetail all record's for same GroupId and posinfo id
        /// <param name="db"></param>
        /// <param name="dbTransact"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="PosInfoDetailId"></param>
        /// <param name="InvoiceTypeId"></param>
        /// <returns></returns>
        long GetNextCounter(IDbConnection db, IDbTransaction dbTransact, long PosInfoId, long PosInfoDetailId, long? InvoiceTypeId);

        /// <summary>
        /// Get pos info detail according to PosInfo Id and GroupId
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="PosInfoDetailId"> PosInfoDetail </param>
        /// <param name="GroupId"></param>
        /// <returns> Pos info detail model. See: <seealso cref="Symposium.Models.Models.PosInfoDetailModel"/> </returns>
        PosInfoDetailModel GetSinglePosInfoDetail(DBInfoModel Store, long PosInfoId, long GroupId);
    }
}