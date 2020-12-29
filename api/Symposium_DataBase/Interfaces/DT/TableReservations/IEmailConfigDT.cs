﻿using Symposium.Models.Models;
using Symposium.Models.Models.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.TableReservations
{
    public interface IEmailConfigDT
    {
        /// <summary>
        /// Get the Email Config
        /// </summary>
        /// <returns></returns>
        EmailConfigModel GetEmailConfig(DBInfoModel Store);

        /// <summary>
        /// Insert new Email Config
        /// </summary>
        /// <returns></returns>
        EmailConfigModel InsertEmailConfig(DBInfoModel Store, EmailConfigModel model);

        /// <summary>
        /// Update an Email Config
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        EmailConfigModel UpdateEmailConfig(DBInfoModel Store, EmailConfigModel Model);

        /// <summary>
        /// Delete an Email Config
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteEmailConfig(DBInfoModel Store, long Id);
    }
}
