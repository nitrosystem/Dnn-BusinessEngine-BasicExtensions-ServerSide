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
    public static class CustomQueryServiceRepository
    {
        private static readonly string ConnectionString = DataProvider.Instance().ConnectionString;

        public static CustomQueryServiceInfo GetCustomQueryService(Guid serviceID)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var result = db.Get<CustomQueryServiceInfo>(serviceID);

                return result;
            }
        }

        public static Guid AddCustomQueryService(CustomQueryServiceInfo customQueryService)
        {
            customQueryService.ItemID = Guid.NewGuid();

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                db.Insert<CustomQueryServiceInfo>(customQueryService);
            }

            return customQueryService.ItemID;
        }

        public static void UpdateCustomQueryService(CustomQueryServiceInfo customQueryService)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                db.Update<CustomQueryServiceInfo>(customQueryService);
            }
        }
    }
}
