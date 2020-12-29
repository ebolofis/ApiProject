using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.DeliveryAgent
{
    public class DA_OrderNoDT : IDA_OrderNoDT
    {
        string connectionString;
        IUsersToDatabasesXML usersToDatabases;
        IGenericDAO<DA_OrderNoDTO> daOrderNoDao;

        public DA_OrderNoDT(IUsersToDatabasesXML _usersToDatabases, IGenericDAO<DA_OrderNoDTO> _daOrderNoDao)
        {
            this.usersToDatabases = _usersToDatabases;
            this.daOrderNoDao = _daOrderNoDao;
        }

        public long FetchOrderNo(IDbConnection db)
        {
            long newOrderNo;
            string sqlUpdateQuery = @"UPDATE DA_OrderNo SET OrderNo = OrderNo + 1";
            long rowsAffected = daOrderNoDao.Execute(db, sqlUpdateQuery);
            if (rowsAffected == 0)
            {
                string sqlInsertQuery = @"INSERT INTO DA_OrderNo (OrderNo) VALUES (1)";
                long rowsAffected2 = daOrderNoDao.Execute(db, sqlInsertQuery);
            }
            DA_OrderNoDTO orderNoModel = daOrderNoDao.Select(db, "DA_OrderNo").FirstOrDefault();
            newOrderNo = orderNoModel.OrderNo;
            return newOrderNo;
        }

    }
}
