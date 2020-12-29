using Pos_WebApi.Customer_Modules;
using Symposium.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    public class BaseController: ApiController
    {

        public ICustomerClass LoadCustomerClass()
        {
            string cType = MainConfigurationHelper.GetSubConfiguration("api", "CustomerClass");
            if (!string.IsNullOrEmpty(cType))
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type classType = assembly.GetType(cType);
                ICustomerClass instanceOfMyType = (ICustomerClass)Activator.CreateInstance(classType);
                return instanceOfMyType;
            }
            return null;
        }
    }
}