using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models
{
    /// <summary>
    /// Model that describes an Account
    /// </summary>
    public class AccountModel 
    {

        public long Id { get; set; }


        public string Description { get; set; }


        public Nullable<short> Type { get; set; }


        public Nullable<bool> IsDefault { get; set; }


        public Nullable<bool> SendsTransfer { get; set; }


        public Nullable<bool> CardOnly { get; set; }


        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<short> KeyboardType { get; set; }

        public long PMSPaymentId { get; set; }

        public long DAId { get; set; }

        public List<PosInfoDetail_Excluded_AccountsModel> PosInfoDetail_Excluded_Accounts { get; set; }
    }

    public class PosInfoDetail_Excluded_AccountsModel
    {
        public long Id { get; set; }
        public Nullable<long> PosInfoDetailId { get; set; }
        public Nullable<long> AccountId { get; set; }
    }

    /// <summary>
    /// Model with EODAccountToPmsTransfer and Accounts
    /// </summary>
    public class EODAccountAndAccountModel
    {
        public long Id { get; set; }

        public Nullable<long> PosInfoId { get; set; }

        public Nullable<long> AccountId { get; set; }

        public Nullable<long> PmsRoom { get; set; }

        public string Description { get; set; }

        public Nullable<bool> Status { get; set; }

        public string PmsDepartmentId { get; set; }

        public string PmsDepDescription { get; set; }

        public Nullable<long> Account_Id { get; set; }

        public string Account_Descr { get; set; }

        public Nullable<Int16 > Type { get; set; }

        public Nullable<bool> IsDefault { get; set; }

        public Nullable<bool> SendsTransfer { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<bool> CardOnly { get; set; }
    }

}
