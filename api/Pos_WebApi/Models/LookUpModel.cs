using Pos_WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pos_WebApi.Models
{
    public class LookUpModel
    {
        public string IPAddress { get; set; }
        public string StoreId { get; set; }
        public long? DepartmentId { get; set; }
        public string DepartmentDescription { get; set; }
        public long? ModuleId { get; set; }

    }

    public class PosLookUpsModel : LookUpModel
    {
        public CustomerPolicyEnum CustomerPolicy { get; set; }
        public IEnumerable<StaffForClient> StaffList { get; set; }
        public IEnumerable<PaymentOptions> PaymentOptions { get; set; }
        public IEnumerable<Pricelist> Pricelists { get; set; }
        public IEnumerable<AccountModel> Accounts { get; set; }
        public IEnumerable<AccountModel> CreditCards { get; set; }
        public IEnumerable<KitchenInstruction> KitchenInstructions { get; set; }
        public IEnumerable<KitchenRegion> KitchenRegions { get; set; }
        public IEnumerable<AllowedMealsPerBoard> AllowedMeals { get; set; }
        public IEnumerable<StaffAuthorizationModel> StaffAuthorizationModels { get; set; }
        public IEnumerable<SalesType> SalesTypes { get; set; }
        public IEnumerable<Vat> Vats { get; set; }

    }

    public class StaffForClient
    {
        public StaffForClient()
        {

        }
        public Int64 Id { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return string.Join(FirstName, " ", LastName); } }
        public string Password { get; set; }
        public string ImageUri { get; set; }

        public IEnumerable<long?> AssignedPositions { set; get; }
        public IEnumerable<long?> UserActions { set; get; }
    }

    public class PaymentOptions
    {
        public Int64 Id { get; set; }
        public Int64? PosInfoId { get; set; }
        public string Abbreviation { get; set; }
        public Int64? Counter { get; set; }
        public String Description { get; set; }
        public Int64? InvoiceTypeId { get; set; }
        public Int64? InvoiceId { get; set; }
        public string ButtonDescription { get; set; }
        public bool CreateTransaction { get; set; }
        public bool IsInvoice { get; set; }
        public bool IsCancel { get; set; }
        public Int32? GroupId { get; set; }
    }

    public class AccountModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public short? AccountType { get; set; }
        public bool SendsTransfer { get; set; }
        public long? TransferRoom { get; set; }
        public long PosInfoId { get; set; }
    }

    public class StaffAuthorizationModel
    {
        public Int64? ActionId { get; set; }
        public Int64? AuthorizedGroupId { get; set; }
        public string Description { get; set; }
        public string ActionDescription { get; set; }
    }
}