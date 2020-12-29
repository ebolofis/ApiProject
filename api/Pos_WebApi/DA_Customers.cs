
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Pos_WebApi
{

using System;
    using System.Collections.Generic;
    
public partial class DA_Customers
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public DA_Customers()
    {

        this.DA_CustomerMessages = new HashSet<DA_CustomerMessages>();

        this.DA_CustomersTokens = new HashSet<DA_CustomersTokens>();

        this.DA_LoyalPoints = new HashSet<DA_LoyalPoints>();

        this.DA_Orders = new HashSet<DA_Orders>();

    }


    public long Id { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone1 { get; set; }

    public string Phone2 { get; set; }

    public string Mobile { get; set; }

    public Nullable<long> BillingAddressesId { get; set; }

    public string VatNo { get; set; }

    public string Doy { get; set; }

    public string JobName { get; set; }

    public Nullable<long> LastAddressId { get; set; }

    public string Notes { get; set; }

    public string SecretNotes { get; set; }

    public string LastOrderNote { get; set; }

    public Nullable<bool> GTPR_Marketing { get; set; }

    public Nullable<bool> GTPR_Returner { get; set; }

    public Nullable<bool> Loyalty { get; set; }

    public string SessionKey { get; set; }

    public string ExtId1 { get; set; }

    public string ExtId2 { get; set; }

    public string ExtId3 { get; set; }

    public string ExtId4 { get; set; }

    public bool IsDeleted { get; set; }

    public string PhoneComp { get; set; }

    public bool SendSms { get; set; }

    public bool SendEmail { get; set; }

    public string Proffesion { get; set; }

    public string EmailOld { get; set; }

    public string AuthToken { get; set; }

    public Nullable<System.DateTime> CreateDate { get; set; }

    public string Metadata { get; set; }

    public Nullable<int> IsAnonymous { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DA_CustomerMessages> DA_CustomerMessages { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DA_CustomersTokens> DA_CustomersTokens { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DA_LoyalPoints> DA_LoyalPoints { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<DA_Orders> DA_Orders { get; set; }

}

}