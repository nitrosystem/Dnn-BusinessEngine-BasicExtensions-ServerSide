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
    public static class SubmitEntityServiceRepository
    {
        private static readonly string ConnectionString = DataProvider.Instance().ConnectionString;

        public static SubmitEntityServiceInfo GetSubmitEntityService(Guid serviceID)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var result = db.Get<SubmitEntityServiceInfo>(serviceID);

                return result;
            }
        }

        public static Guid AddSubmitEntityService(SubmitEntityServiceInfo submitEntityService)
        {
            submitEntityService.ItemID = Guid.NewGuid();

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                db.Insert<SubmitEntityServiceInfo>(submitEntityService);
            }

            return submitEntityService.ItemID;
        }

        public static void UpdateSubmitEntityService(SubmitEntityServiceInfo submitEntityService)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                db.Update<SubmitEntityServiceInfo>(submitEntityService);
            }
        }
    }
}
