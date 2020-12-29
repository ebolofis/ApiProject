using Dapper;
using Symposium.WebApi.DataAccess.Interfaces.DAO.Goodys;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs.Goodys
{
    public class GoodysDao : IGoodysDAO
    {

        public long GetOpenOrders(IDbConnection db )
        {
            string sql = @"select count(distinct orderid) 
                                from [order] o
                                cross apply (
	                                select max(oss.TimeChanged) TimeChanged
	                                from orderstatus oss 
	                                where oss.OrderId = o.Id
                                ) oss
                                inner join orderstatus os on os.OrderId = o.Id and os.TimeChanged = oss.TimeChanged and os.Status in (0,1,2,3)
                                where o.ExtType=1  and o.EndOfDayId Is null";
          long OpenOrdersCount = db.Query<long>(sql).FirstOrDefault();
            return OpenOrdersCount;
        }

      public  long GetInvoiceid(IDbConnection db, long orderno)
        {
            string sql = @"select distinct odi.invoicesId from orderdetailinvoices
 as odi inner join orderdetail as od on od.id =odi.orderdetailid inner join
[order] as o on o.id = od.orderid and o.orderno=@orderno";

            long invoiceid = db.Query<long>(sql, new { orderno = orderno }).FirstOrDefault();
            return invoiceid;
        }
        public OrderDTO GetGoodysExternalOrderID(IDbConnection db, long InvoiceId)
        {
            OrderDTO goodys;
            string sql = @"SELECT * 
                                FROM [Order] AS o
                                WHERE o.Id =
                                (
                                SELECT distinct o.Id 
                                FROM [Order] AS o 
                                INNER JOIN OrderDetail AS od ON od.OrderId = o.Id
                                INNER JOIN OrderDetailInvoices AS odi ON odi.OrderDetailId = od.Id
                                INNER JOIN Invoices AS i ON odi.InvoicesId = i.Id
                                WHERE i.id = @id
                                )";
            goodys = db.Query<OrderDTO>(sql, new { id = InvoiceId }).FirstOrDefault();
            return goodys;
        }

    }
}