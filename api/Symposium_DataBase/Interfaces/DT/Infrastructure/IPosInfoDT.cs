using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    /// <summary>
    /// Class that handles data related to PosInfo
    /// </summary>
    public interface IPosInfoDT
    {

        /// <summary>
        /// Get pos info according to selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="PosInfoId"> PosInfo </param>
        /// <returns> Pos info model. See: <seealso cref="Symposium.Models.Models.PosInfoModel"/> </returns>
        PosInfoModel GetSinglePosInfo(DBInfoModel Store, long PosInfoId);

        /// <summary>
        /// Get posInfo Models on a generic via department id filter over dtos and parce them to a model list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DepatmentId">Filter of pos entities by departmentId</param>
        /// <returns>List of Posinfo Models collected from generic Posinfo DAO</returns>
        List<PosInfoModel> GetDepartmentPosInfo(DBInfoModel Store, long DepatmentId);

        /// <summary>
        /// Return's ExtEcr Name for specific PosInfo Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosInfoId"></param>
        /// <returns></returns>
        string GetExtEcrName(DBInfoModel Store, long PosInfoId);

        /// <summary>
        /// Return's next OrderNo from PosInfo
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransact"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        long GetNextOrderNo(IDbConnection db, IDbTransaction dbTransact, long PosInfoId, out string Error);

        /// <summary>
        /// Get first pos id
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        long GetFirstPosInfoId(DBInfoModel Store);

        /// <summary>
        /// Get All Configuration Files from PosInfo
        /// </summary>
        /// <returns>List of Pos Info Configuration Files</returns>
        List<string> GetAllPosInfoConfig(DBInfoModel DBInfo);
    }
}
