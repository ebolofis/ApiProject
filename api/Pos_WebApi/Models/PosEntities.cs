using Pos_WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace Pos_WebApi
{
    public partial class PosEntities
    {

        public PosEntities(bool lazyloading)
            : base("name=PosEntities")
        {
            Configuration.LazyLoadingEnabled = false;
            this.Database.Connection.ConnectionString = ConnectionUtil.ConfigureConnectionString();
            this.Configuration.ProxyCreationEnabled = false;

            var adapter = (IObjectContextAdapter)this;
            var objectContext = adapter.ObjectContext;
            objectContext.CommandTimeout = 2 * 60; // value in seconds

        }


        public PosEntities(bool lazyloading, Guid storeid)
            : base("name=PosEntities")
        {
            Configuration.LazyLoadingEnabled = false;
            this.Database.Connection.ConnectionString = ConnectionUtil.ConfigureConnectionStringByStoreId(storeid);
            this.Configuration.ProxyCreationEnabled = false;

        }

    }

    public static class TransactionHelper
    {
        public static DbTransaction BeginTransaction(this PosEntities context)
        {
            if (context.Database.Connection.State != ConnectionState.Open)
            {
                context.Database.Connection.Open();
            }
            return context.Database.Connection.BeginTransaction();
        }
    }

}