using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace HRMS_ERP.DataAccess
{
    public class UsersDAL : BaseDAL
    {
        // ─── PASSWORD HASHING ─────────────────────────────────────────────
        // Converts plain text "Admin@123" → a 64-character scrambled string
        // That scrambled string is what gets stored in the database
        // Even if someone reads the DB, they cannot reverse it back to the password
        public static string HashPassword(string plainTextPassword)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plainTextPassword));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        // ─── VALIDATE LOGIN ───────────────────────────────────────────────
        // Called when user presses Login button
        // Returns the user row if username + password match, otherwise null
        public DataRow ValidateLogin(string username, string plainTextPassword)
        {
            string hashed = HashPassword(plainTextPassword);
            string query = @"
                SELECT u.UserID, u.Username, u.Role, u.EmployeeID, 
                       ISNULL(e.FullName, u.Username) AS FullName
                FROM Users u
                LEFT JOIN Employee e ON u.EmployeeID = e.EmployeeID
                WHERE u.Username = @User AND u.PasswordHash = @Pass";
            SqlParameter[] param = {
                new SqlParameter("@User", username),
                new SqlParameter("@Pass", hashed)
            };
            DataTable dt = GetDataTable(query, param);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        // ─── GET ALL USERS (for user management grid) ─────────────────────
        public DataTable GetAll()
        {
            string query = @"
                SELECT u.UserID, u.Username, u.Role, u.EmployeeID,
                       ISNULL(e.FullName, '-') AS FullName
                FROM Users u
                LEFT JOIN Employee e ON u.EmployeeID = e.EmployeeID
                ORDER BY u.Username";
            return GetDataTable(query);
        }

        // ─── INSERT new user ──────────────────────────────────────────────
        public int Insert(string username, string plainTextPassword, string role, int? employeeID, string performedBy)
        {
            if (IsUsernameDuplicate(username)) return -1;

            string query = @"
                INSERT INTO Users (Username, PasswordHash, Role, EmployeeID)
                VALUES (@Username, @Password, @Role, @EmployeeID);
                SELECT SCOPE_IDENTITY();";
            SqlParameter[] param = {
                new SqlParameter("@Username",   username),
                new SqlParameter("@Password",   HashPassword(plainTextPassword)),
                new SqlParameter("@Role",       role),
                new SqlParameter("@EmployeeID", (object)employeeID ?? DBNull.Value)
            };
            object result = ExecuteScalar(query, param);
            int newID = result != null ? Convert.ToInt32(result) : 0;
            AuditLogDAL.LogAction("Users", "INSERT", newID, performedBy);
            return newID;
        }

        // ─── UPDATE user role / linked employee ───────────────────────────
        public int Update(int userID, string username, string role, int? employeeID, string performedBy)
        {
            string query = @"
                UPDATE Users SET Username = @Username, Role = @Role, EmployeeID = @EmployeeID
                WHERE UserID = @ID";
            SqlParameter[] param = {
                new SqlParameter("@ID",         userID),
                new SqlParameter("@Username",   username),
                new SqlParameter("@Role",       role),
                new SqlParameter("@EmployeeID", (object)employeeID ?? DBNull.Value)
            };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("Users", "UPDATE", userID, performedBy);
            return rows;
        }

        // ─── CHANGE PASSWORD ──────────────────────────────────────────────
        public int ChangePassword(int userID, string newPlainTextPassword, string performedBy)
        {
            string query = "UPDATE Users SET PasswordHash = @Pass WHERE UserID = @ID";
            SqlParameter[] param = {
                new SqlParameter("@ID",   userID),
                new SqlParameter("@Pass", HashPassword(newPlainTextPassword))
            };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("Users", "CHANGE_PASSWORD", userID, performedBy);
            return rows;
        }

        // ─── DELETE user ──────────────────────────────────────────────────
        public int Delete(int userID, string performedBy)
        {
            string query = "DELETE FROM Users WHERE UserID = @ID";
            SqlParameter[] param = { new SqlParameter("@ID", userID) };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("Users", "DELETE", userID, performedBy);
            return rows;
        }

        // ─── DUPLICATE check ──────────────────────────────────────────────
        public bool IsUsernameDuplicate(string username, int excludeUserID = 0)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND UserID <> @ExcludeID";
            SqlParameter[] param = {
                new SqlParameter("@Username",  username),
                new SqlParameter("@ExcludeID", excludeUserID)
            };
            object result = ExecuteScalar(query, param);
            return result != null && Convert.ToInt32(result) > 0;
        }

        // ─── SEED default admin (call once on first run) ──────────────────
        // Username: admin    Password: Admin@123
        public void SeedAdminUser()
        {
            string check = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
            object cnt = ExecuteScalar(check);
            if (cnt != null && Convert.ToInt32(cnt) > 0) return; // already exists

            string query = @"
                INSERT INTO Users (Username, PasswordHash, Role)
                VALUES ('admin', @Pass, 'Admin')";
            SqlParameter[] param = { new SqlParameter("@Pass", HashPassword("Admin@123")) };
            ExecuteNonQuery(query, param);
        }
    }
}
