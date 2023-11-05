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
    public static class BindEntityServiceRepository
    {
        private static readonly string ConnectionString = DataProvider.Instance().ConnectionString;

        public static BindEntityServiceInfo GetBindEntityService(Guid serviceID)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var result = db.Get<BindEntityServiceInfo>(serviceID);

                return result;
            }
        }

        public static Guid AddBindEntityService(BindEntityServiceInfo bindEntityService)
        {
            bindEntityService.ItemID = Guid.NewGuid();

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                db.Insert<BindEntityServiceInfo>(bindEntityService);
            }

            return bindEntityService.ItemID;
        }

        public static void UpdateBindEntityService(BindEntityServiceInfo bindEntityService)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                db.Update<BindEntityServiceInfo>(bindEntityService);
            }
        }
    }
}
