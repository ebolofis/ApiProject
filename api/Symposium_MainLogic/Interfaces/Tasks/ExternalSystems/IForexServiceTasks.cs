﻿using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IForexServiceTasks
    {

        List<ForexServiceModel> SelectAllForex(DBInfoModel store);

    }
}