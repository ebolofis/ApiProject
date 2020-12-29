using Symposium.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.DeliveryAgent
{
    public class DA_EfoodModel
    {
        public List<Da_EfoodOrderModel> orders { get; set; }
    }

    public class Da_EfoodOrderModel
    {
        public long id { get; set; }
        public string brand { get; set; }
        public string timezone { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime promised_customer_timestamp { get; set; }
        public Da_EfoodRestaurantModel restaurant { get; set; }
        public Da_EfoodCustomerModel customer { get; set; }
        public decimal tip { get; set; }
        public Da_EfoodBagsModel bags { get; set; }
        public string type { get; set; }
        public Da_EfoodTransportMethodModel transport_method { get; set; }
        public decimal price { get; set; }
        public string payment_type { get; set; }
        public List<Da_EfoodProductModel> products { get; set; }
        public List<Da_EfoodDiscountModel> discounts { get; set; }
        public List<Da_EfoodCouponModel> coupons { get; set; }
        public List<Da_EfoodJokerModel> joker { get; set; }
        public long PriceListId { get; set; } = 0;
    }
    public class Da_EfoodRestaurantModel
    {
        public long id { get; set; }
        public string vendor_external_id { get; set; }
    }
    public class Da_EfoodCustomerModel
    {
        public long id { get; set; }
        public string address_id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string area { get; set; }
        public string postal_code { get; set; }
        public string street { get; set; }
        public string street_number { get; set; }
        public string floor { get; set; }
        public string telephone { get; set; }
        public float longitude { get; set; }
        public float latitude { get; set; }
        public string doorbell { get; set; }
        public string alternate_telephone { get; set; }
        public string notes { get; set; }
    }
    public class Da_EfoodBagsModel
    {
        public long count { get; set; }
        public decimal amount { get; set; }
    }
    public class Da_EfoodTransportMethodModel
    {
        public string key { get; set; }
        public string title { get; set; }
        public string value { get; set; }
    }
    public class Da_EfoodProductModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public string notes { get; set; }
        public List<Da_EfoodmaterialModel> materials { get; set; }
    }
    public class Da_EfoodmaterialModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string notes { get; set; }
        public int quantity { get; set; }

        public decimal price { get; set; }
    }
    public class Da_EfoodDiscountModel { 
        public string type { get; set; }
        public decimal amount { get; set; }
    }
    public class Da_EfoodCouponModel { }
    public class Da_EfoodJokerModel { }
}
