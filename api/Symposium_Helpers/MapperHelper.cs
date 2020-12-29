using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers
{
    public class MapperHelper
    {
        public void Init()
        {
            Mapper.Initialize(cfg => { });
        }

    }

}
