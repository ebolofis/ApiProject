using Pos_WebApi.Helpers;
using System;
using System.Linq;
using System.Web.Http;

namespace Pos_WebApi.Controllers.BOControllers
{
    public class LookupEnumController : ApiController
    {

        public object GetLookUp(string enumName)
        {
            var pairs = Enum.GetValues(typeof(CustomerPolicyEnum))
                  .Cast<HotelInfoCustomerPolicyEnum>()
                  .Select(t => new {
                      Descr = t.ToString(),
                      Id = ((int)t).ToString(),
                  });
            return pairs;

        }
    }
}
