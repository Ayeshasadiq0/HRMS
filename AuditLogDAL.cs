using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HRMS_ERP.DataAccess
{
    public class AuditLogDAL : BaseDAL
    {
        public static void LogAction(string tableName, string actionType, int recordID, string performedBy)
        {
            string query = @"INSERT INTO AuditLog (TableName, ActionType, RecordID, PerformedBy) 
                             VALUES (@Table, @Action, @Record, @By)";

            SqlParameter[] param = {
                new SqlParameter("@Table", tableName),
                new SqlParameter("@Action", actionType),
                new SqlParameter("@Record", recordID),
                new SqlParameter("@By", performedBy)
            };

            new AuditLogDAL().ExecuteNonQuery(query, param);
        }
    }
}
