using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using DotNetNuke.Data;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Newtonsoft.Json;
using NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Entities;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.DB.Repositories
{
    public static class DataSourceServiceRepository
    {
        private static readonly string ConnectionString = DataProvider.Instance().ConnectionString;

        public static DataSourceServiceInfo GetDataSourceService(Guid serviceID)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var result = db.Get<DataSourceServiceInfo>(serviceID);

                return result;
            }
        }

        public static Guid AddDataSourceService(DataSourceServiceInfo dataSourceService)
        {
            dataSourceService.ItemID = Guid.NewGuid();

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                db.Insert<DataSourceServiceInfo>(dataSourceService);
            }

            return dataSourceService.ItemID;
        }

        public static void UpdateDataSourceService(DataSourceServiceInfo dataSourceService)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                db.Update<DataSourceServiceInfo>(dataSourceService);
            }
        }
    }
}
