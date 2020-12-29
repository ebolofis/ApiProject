using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT
{
    public class TransferMappingsDT : ITransferMappingsDT
    {
        IGenericDAO<TransferMappingsDTO> dt;
        IUsersToDatabasesXML users;
        string connectionstring;

        public TransferMappingsDT(IGenericDAO<TransferMappingsDTO> dt, IUsersToDatabasesXML users)
        {
            this.dt = dt;
            this.users = users;
        }


        /// <summary>
        /// Return's a Transfer Mapping Record To Create new TransfertoPMS
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosDepartmentId"></param>
        /// <param name="ProductCategoryId"></param>
        /// <param name="PriceListId"></param>
        /// <param name="HotelId"></param>
        /// <returns></returns>
        public TransferMappingsModel GetTransferMappingForNewTransaction(DBInfoModel Store, long PosDepartmentId, long ProductCategoryId, 
            long PriceListId, int HotelId)
        {
            connectionstring = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionstring))
            {
                return AutoMapper.Mapper.Map<TransferMappingsModel>(dt.SelectFirst(db, "WHERE PosDepartmentId = @PosDepartmentId AND ProductCategoryId = @ProductCategoryId \n"
                                                                                      + "  AND PriceListId = @PriceListId AND HotelId = @HotelId",
                                                                    new { PosDepartmentId = PosDepartmentId, ProductCategoryId = ProductCategoryId, PriceListId = PriceListId, HotelId = HotelId }));
            }
        }

        /// <summary>
        /// Return's a List of TransferMappingsModel for PosDepartmentId and HotelId if exists
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="PosDepartmentId"></param>
        /// <param name="HotelId"></param>
        /// <returns></returns>
        public List<TransferMappingsModel> GetTransferMappingsByHotelAndDepartment(DBInfoModel Store, long PosDepartmentId, int HotelId)
        {
            connectionstring = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionstring))
            {
                string SQL = "";
                if (PosDepartmentId > 0 && HotelId > 0)
                {
                    SQL += " WHERE PosDepartmentId = @PosDepartmentId AND HotelId = @HotelId ";
                    return AutoMapper.Mapper.Map<List<TransferMappingsModel>>(dt.Select(db, SQL, new { PosDepartmentId = PosDepartmentId, HotelId = HotelId }));
                }
                if (PosDepartmentId > 0)
                {
                    SQL += " WHERE PosDepartmentId = @PosDepartmentId ";
                    return AutoMapper.Mapper.Map<List<TransferMappingsModel>>(dt.Select(db, SQL, new { PosDepartmentId = PosDepartmentId }));
                }
                if (HotelId > 0)
                {
                    SQL += " WHERE HotelId = @HotelId ";
                    return AutoMapper.Mapper.Map<List<TransferMappingsModel>>(dt.Select(db, SQL, new { HotelId = HotelId }));
                }
                else
                    return AutoMapper.Mapper.Map<List<TransferMappingsModel>>(dt.Select(db));
            }
        }
    }
}
