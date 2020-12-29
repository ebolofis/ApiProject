using Dapper;
using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.DataAccess.Interfaces.DAO.HotelInfo;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs.HotelInfo
{
    public class HotelInfoV3DAO : IHotelInfoV3DAO
    {


        public List<HotelInfoDTO> GetHotelInfo(IDbConnection db)
        {
            List<HotelInfoDTO> model;

            string sql = @"SELECT * FROM HotelInfo";

            model = db.Query<HotelInfoDTO>(sql).ToList();

            return model;
        }


        public List<TransferMappingsDTO> GetTransferMappings(IDbConnection db, long HotelId, long ProdCatId)
        {
            List<TransferMappingsDTO> model;
            string sql = @"SELECT DISTINCT PmsDepartmentId, 
                                PmsDepDescription,
                                 ProductCategoryId,
                                 HotelId FROM TransferMappings 
                                 WHERE ProductCategoryId = @prodCatId AND HotelId = @hotId";

            model = db.Query<TransferMappingsDTO>(sql, new { prodCatId = ProdCatId, hotId = HotelId }).ToList();

            return model;

        }



        public void UpdateTransferMappings(IDbConnection db, long newPmsDepId, string newPmsDescr, long ProdCatId, long HotelId,long OldPmsDepartmentId)
        {
            string sql = @"UPDATE TransferMappings 
                             SET PmsDepartmentId = @newPmsDepId,
                             PmsDepDescription = @newPmsDepDescr 
                             WHERE ProductCategoryId = @prodCatId AND HotelId = @hotId AND PmsDepartmentId = @oldPmsDepId";

            db.Query<TransferMappingsDTO>(sql, new { newPmsDepId = newPmsDepId, newPmsDepDescr = newPmsDescr, prodCatId = ProdCatId, hotId = HotelId, oldPmsDepId= OldPmsDepartmentId }).FirstOrDefault();
            return;
        }

    }
}

