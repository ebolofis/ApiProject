using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Enums
{
    public enum DA_CustomerSearchTypeEnum
    {
        Phone = -1,
        Name = 0,
        Address = 1,
        VatNo = 2,
        Email = 3
    }

    public enum DA_CustomerAnonymousTypeEnum
    {
        IsNotAnonymous = 0,
        WillBeAnonymous = 1,
        IsAnonymous = 2,
    }

}
