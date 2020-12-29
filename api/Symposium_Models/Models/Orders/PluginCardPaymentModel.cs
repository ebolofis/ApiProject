
namespace Symposium.Models.Models.Orders
{
   public class PluginCardPaymentModel
    {
        public decimal Amount { get; set; }

        public int Posid { get; set; }

        public  int Orderid { get; set; }

        public int Ordernum { get; set; }

        /// <summary>
        /// for future use
        /// </summary>
        public int Code { get; set; }
    }

    public class PluginCardPaymentResultModel
    {
        /// <summary>
        /// true for success
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// the retutn code. For success return "". On error return an error code
        /// </summary>
        public string ReturnCode { get; set; }

    }
}
