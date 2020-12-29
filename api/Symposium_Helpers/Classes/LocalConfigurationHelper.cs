using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Symposium.Helpers.Classes
{
    public class LocalConfigurationHelper
    {


        private bool _isDeliveryAgent;

        private string _phonePrefix;
        private string _mobilePrefix;

        public  LocalConfigurationHelper()
        {
            _isDeliveryAgent = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "IsDeliveryAgent");
            string phonePrefixRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "PhonePrefix");
            _phonePrefix = phonePrefixRaw.Trim();
            string mobilePrefixRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "MobilePrefix");
            _mobilePrefix = mobilePrefixRaw.Trim();
        }

        /// <summary>
        /// if api is not DeliveryAgent then Throw Exception
        /// </summary>
        public void CheckDeliveryAgent()
        {
            if (!_isDeliveryAgent) throw new Exception(Environment.NewLine+"        Web Api is NO Delivery Agent   "+ Environment.NewLine);
        }

        /// <summary>
        /// Return the default country phone prefix (ex: +30)
        /// </summary>
        /// <returns></returns>
        public string PhonePrefix()
        {
            return _phonePrefix;
        }

        /// <summary>
        /// Return the default country mobile prefix (ex: 69)
        /// </summary>
        /// <returns></returns>
        public string MobilePrefix()
        {
            return _mobilePrefix;
        }

    }
}
