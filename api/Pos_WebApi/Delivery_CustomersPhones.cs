
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
    
public partial class Delivery_CustomersPhones
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Delivery_CustomersPhones()
    {

        this.Delivery_CustomersPhonesAndAddress = new HashSet<Delivery_CustomersPhonesAndAddress>();

    }


    public long ID { get; set; }

    public long CustomerID { get; set; }

    public string PhoneNumber { get; set; }

    public int PhoneType { get; set; }

    public Nullable<bool> IsSelected { get; set; }

    public string ExtKey { get; set; }

    public Nullable<int> ExtType { get; set; }

    public string ExtObj { get; set; }



    public virtual Delivery_Customers Delivery_Customers { get; set; }

    public virtual Delivery_PhoneTypes Delivery_PhoneTypes { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Delivery_CustomersPhonesAndAddress> Delivery_CustomersPhonesAndAddress { get; set; }

}

}