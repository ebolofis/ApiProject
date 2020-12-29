using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Kds
{
    public class KdsResponceModel
    {
        public long OrderDetailsId { get; set; }
        public long OrderId { get; set; }
        public Nullable<DateTime> Day { get; set; }
        public Nullable<long> EndOfDayId { get; set; }
        public Nullable<long> OrderNo { get; set; }
        public Nullable<long> ReceiptNo { get; set; }
        public string OrderNotes { get; set; }
        public string ItemRemark { get; set; }
        public OrderStatusModel OrderStatus { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<System.DateTime> StatusTS { get; set; }
        public Nullable<int> PreparationTime { get; set; }
        public long PosId { get; set; }
        public decimal Total { get; set; }
        public Nullable<long> ProductId { get; set; }
        public string ProductDescr { get; set; }
        public Nullable<double> Qty { get; set; }
        public Nullable<long> KdsId { get; set; }
        public Nullable<long> SalesTypeId { get; set; }
        public string SalesTypeDescr { get; set; }
        public int KitchenStatus { get; set; }
        public Nullable<long> LoginStaffId { get; set; }
        public StaffModels StaffModel { get; set; }
        public List<OrderDetailIngredientsModel> OrderDetailIgredients { get; set; }
    }
}
