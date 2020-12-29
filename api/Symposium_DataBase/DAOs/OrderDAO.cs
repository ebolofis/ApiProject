using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs
{
    public class OrderDAO : IOrderDAO
    {
        IGenericDAO<OrderDTO> genorder;
        public OrderDAO(IGenericDAO<OrderDTO> _genorder)
        {
            genorder = _genorder;
        }

        /// <summary>
        /// Function uses gen OrderDTO instance and Gets OrderDTO by id provided
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns>Account refered to Id asked </returns>
        public OrderDTO SelectById(IDbConnection db, long Id)
        {
            return genorder.Select(db, Id);
        }

        /// <summary>
        /// Function uses OrderFilterModel to create a gen query sql string and an object 
        /// in order to collect info with specific filter results 
        /// returns true if at least one obj found with specific query results 
        /// else it returns false
        /// </summary>
        /// <param name="db"></param>
        /// <param name="filter"></param>
        /// <param name="filterType">GeneralENUM.FilterTypeEnum flag </param>
        /// <returns> true if gen res > 0 </returns>
        public bool OrderExists(IDbConnection db, OrderFilterModel filter, int filterType = 0)
        {
            string query = "select * from [order]";
            string wq = "";
            string flg = " AND ";
            switch (filterType)
            {
                case (int)FilterTypeEnum.AND: flg = " AND "; break;
                case (int)FilterTypeEnum.OR: flg = " OR "; break;
                default: flg = " AND "; break;
            }

            if (filter.Id != null) { wq += " Id = @Id "; }
            if (filter.PosId != null) { if (wq.Length == 0) { wq += " PosId = @PosId "; } else { wq += flg + " PosId = @PosId "; } }
            if (filter.StaffId != null) { if (wq.Length == 0) { wq += " StaffId = @StaffId "; } else { wq += flg + " StaffId = @StaffId "; } }
            if (filter.EndOfDayId != null) { if (wq.Length == 0) { wq += " EndOfDayId = @EndOfDayId "; } else { wq += flg + " EndOfDayId = @EndOfDayId "; } }
            if (filter.ExtType != null) { if (wq.Length == 0) { wq += " ExtType = @ExtType "; } else { wq += flg + " ExtType = @ExtType "; } }
            if (!string.IsNullOrEmpty(filter.ExtKey)) { if (wq.Length == 0) { wq += " ExtKey = @ExtKey "; } else { wq += flg + " ExtKey = @ExtKey "; } }
            if (filter.IsDeleted != null) { if (wq.Length == 0) { wq += " isnull(IsDeleted ,0) = @IsDeleted "; } else { wq += flg + " isnull(IsDeleted ,0) = @IsDeleted "; } }


            object whereobj = new
            {
                @Id = filter.Id,
                @PosId = filter.PosId,
                @StaffId = filter.StaffId,
                @EndOfDayId = filter.EndOfDayId,
                @ExtKey = filter.ExtKey,
                @ExtType = filter.ExtType,
                @IsDeleted = filter.IsDeleted,
            };

            if (wq.Length != 0)
            {
                query = query + " where " + wq;
            }

            OrderDTO f = genorder.SelectFirst(query, whereobj, db);
            //genorder.Select(db, Id);
            return (f != null) ? true : false;
        }

        /// <summary>
        /// Function uses OrderFilterModel to create a gen query sql string and an object 
        /// in order to collect list of orders with specific filter results 
        /// returns list of orders that match filter on filter type  FilterTypeEnum.AND , OR
        /// </summary>
        /// <param name="db"></param>
        /// <param name="filter"></param>
        /// <param name="filterType">GeneralENUM.FilterTypeEnum flag </param>
        /// <returns> list orders that match filter </returns>
        public List<OrderDTO> FilteredOrders(IDbConnection db, OrderFilterModel filter, int filterType = 0)
        {
            string query = "select * from [order]";
            string wq = "";
            string flg = " AND ";
            switch (filterType)
            {
                case (int)FilterTypeEnum.AND: flg = " AND "; break;
                case (int)FilterTypeEnum.OR: flg = " OR "; break;
                default: flg = " AND "; break;
            }

            if (filter.Id != null) { wq += " Id = @Id "; }
            if (filter.PosId != null) { if (wq.Length == 0) { wq += " PosId = @PosId "; } else { wq += flg + " PosId = @PosId "; } }
            if (filter.StaffId != null) { if (wq.Length == 0) { wq += " StaffId = @StaffId "; } else { wq += flg + " StaffId = @StaffId "; } }
            if (filter.EndOfDayId != null) { if (wq.Length == 0) { wq += " EndOfDayId = @EndOfDayId "; } else { wq += flg + " EndOfDayId = @EndOfDayId "; } }
            if (filter.ExtType != null) { if (wq.Length == 0) { wq += " ExtType = @ExtType "; } else { wq += flg + " ExtType = @ExtType "; } }
            if (!string.IsNullOrEmpty(filter.OrderNo)) { if (wq.Length == 0) { wq += " OrderNo = ltrim(rtrim(@OrderNo)) "; } else { wq += flg + " OrderNo = ltrim(rtrim(@OrderNo)) "; } }
            if (!string.IsNullOrEmpty(filter.ExtKey)) { if (wq.Length == 0) { wq += " ExtKey = @ExtKey "; } else { wq += flg + " ExtKey = @ExtKey "; } }
            if (filter.IsDeleted != null) { if (wq.Length == 0) { wq += " isnull(IsDeleted ,0) = @IsDeleted "; } else { wq += flg + " isnull(IsDeleted ,0)  = @IsDeleted "; } }

            object whereobj = new
            {
                @Id = filter.Id,
                @PosId = filter.PosId,
                @StaffId = filter.StaffId,
                @EndOfDayId = filter.EndOfDayId,
                @OrderNo = filter.OrderNo,
                @ExtKey = filter.ExtKey,
                @ExtType = filter.ExtType,
                @IsDeleted = filter.IsDeleted,
            };
            if (wq.Length != 0)
            {
                query = query + " where " + wq;
            }
            List<OrderDTO> f = genorder.Select(query, whereobj, db);
            return f;
        }

        public OrderDTO GetOrderByInvoiceId(IDbConnection db, long InvoiceID) {
            string q = @"select Distinct o.* from invoices as i
                                    inner join OrderDetailInvoices as odi on i.id = odi.InvoicesId
                                    inner join OrderDetail as od on odi.OrderDetailId = od.Id
                                    inner join[Order] as o on o.Id = od.OrderId
                                    where i.EndOfDayId is null and i.Id = @invId";
            return genorder.SelectFirst(q, new { invId = InvoiceID }, db);
        }
    }
}
