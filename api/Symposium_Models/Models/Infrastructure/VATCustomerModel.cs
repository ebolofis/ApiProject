
namespace Symposium.Models.Models.Infrastructure
{
    /// <summary>
    /// request object to be send to VATPlugin
    /// </summary>
    public class VATCustomerRequestModel
    {
        public string afm { get; set; }

        public int Posid { get; set; }

        public int Orderid { get; set; }

        public int Ordernum { get; set; }

        /// <summary>
        /// for future use
        /// </summary>
        public int Code { get; set; }
    }

    /// <summary>
    /// result returned from VATPlugin
    /// </summary>
    public class VATCustomerResultModel
    {
        public long Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string AFM { get; set; }
        public string DOY { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public long NationalityId { get; set; }
        public string Nationality { get; set; }
        public string CountryId { get; set; }
        public string Country { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string CompanyName { get; set; }
        public string Job { get; set; }
        public long VIP { get; set; }
        public string Code { get; set; }
    }
}
