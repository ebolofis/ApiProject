using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.OldGoodysModel
{
    public class OldGoodysLoginResponse
    {

        public string id { get; set; }
        public string accountStatus { get; set; }
        public string location { get; set; }
        public string name { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string type { get; set; }

        public List<address> addressList { get;set;}

    public class address
        {
            public string addressComment { get; set; }
            public string addressId { get; set; }
            public string addressNAme { get; set; }
            public string addresssFloor { get; set; }
            public string county { get; set; }
            public string NNHome { get; set; }
            public string phoneNumber { get; set; }
            public string postalCode { get; set; }
            public string shop { get; set; }
            public string shopId { get; set; }
            public string software { get; set; }
            public string specificAddrChar { get; set; }
            public string state { get; set; }
            public string streetAddress { get; set; }
            public string streetAlias { get; set; }
            public string isPrimary { get; set; }
        }





    }
}
