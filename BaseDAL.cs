using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace HRMS_ERP.DataAccess
{
    public class BaseDAL
    {
        // Fixed: Full qualification + using added above
        protected string ConnectionString => DatabaseConfig.ConnectionString;

        #region Common Methods - Used by ALL DALs (Market Standard)

        /// <summary>
        /// Returns DataTable for SELECT queries
        /// </summary>
        public DataTable GetDataTable(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Execute INSERT/UPDATE/DELETE (returns rows affected)
        /// </summary>
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Execute Stored Procedure that returns DataTable
        /// </summary>
        public DataTable ExecuteStoredProcedure(string procName, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(procName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Execute Stored Procedure that does not return data
        /// </summary>
        public int ExecuteStoredProcedureNonQuery(string procName, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(procName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// NEW: Returns single value (used for OUTPUT INSERTED.EmployeeID)
        /// This fixes your Insert method in EmployeeDAL
        /// </summary>
        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    con.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        #endregion
    }
}