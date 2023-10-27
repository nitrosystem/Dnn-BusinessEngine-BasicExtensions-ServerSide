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
using NitroSystem.Dnn.BusinessEngine.BasicActions.Models.Database;

namespace NitroSystem.Dnn.BusinessEngine.BasicActions.Repository
{
    public static class DbRepository
    {
        private static readonly string ConnectionString = DataProvider.Instance().ConnectionString;

        public static IEnumerable<CustomQueryResult> GetCustomQueryResults(Guid actionID)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var result = db.Query<CustomQueryResult>("SELECT * FROM dbo.BusinessEngineBasic_CustomQueryActionResults WHERE ActionID = @ActionID", new { ActionID = actionID });

                return result;
            }
        }

        public static Guid AddCustomQueryResult(CustomQueryResult customQueryResult)
        {
            customQueryResult.ItemID = Guid.NewGuid();

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                db.Insert<CustomQueryResult>(customQueryResult);
            }

            return customQueryResult.ItemID;
        }
        

        public static void DeleteCustomQueryResults(Guid actionID)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var result = db.Execute("DELETE FROM dbo.BusinessEngineBasic_CustomQueryActionResults WHERE ActionID = @ActionID", new { ActionID = actionID });
            }
        }
    }
}
