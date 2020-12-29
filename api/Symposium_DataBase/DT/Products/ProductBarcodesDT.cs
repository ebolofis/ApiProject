using AutoMapper;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.DT.Products;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DT.Products
{
    public class ProductBarcodesDT : IProductBarcodesDT
    {
        IGenericDAO<ProductBarcodesDTO> dt;
        IUsersToDatabasesXML users;
        IProductDT prodDT;


        string connectionString;

        public ProductBarcodesDT(IGenericDAO<ProductBarcodesDTO> dt, IUsersToDatabasesXML users, IProductDT prodDT)
        {
            this.dt = dt;
            this.users = users;
            this.prodDT = prodDT;
        }

        /// <summary>
        /// Return's list of ProductBarcodes after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<ProductBarcodesSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (ProductBarcodesSched_Model item in model)
                {
                    if (item.ProductId != null)
                        item.ProductId = prodDT.GetIdByDAIs(Store, item.ProductId ?? 0);
                }


                results = dt.Upsert(db, Mapper.Map<List<ProductBarcodesDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }


        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<ProductBarcodesSched_Model> model)
        {
            UpsertListResultModel results = new UpsertListResultModel();
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (ProductBarcodesSched_Model item in model)
                {
                    item.DAId = item.TableId;
                    ProductBarcodesDTO tmp = GetModelByDAId(Store, item.TableId ?? 0);
                    if (tmp != null)
                        item.Id = tmp.Id;
                }

                results = dt.DeleteOrSetIsDeletedList(db, Mapper.Map<List<ProductBarcodesDTO>>(model));
                //{TODO Make It Better }
                SchedulerHelper schHlp = new SchedulerHelper();
                results.Results = schHlp.ReturnMasterIds(results.Results, model);
            }
            return results;
        }

        /// <summary>
        /// Return's a model based on DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAId"></param>
        /// <returns></returns>
        public ProductBarcodesDTO GetModelByDAId(DBInfoModel Store, long DAId)
        {
            connectionString = users.ConfigureConnectionString(Store);
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return dt.Select(db, "WHERE DAId = @DAId", new { DAId = DAId }).FirstOrDefault();
            }
        }

    }
}
