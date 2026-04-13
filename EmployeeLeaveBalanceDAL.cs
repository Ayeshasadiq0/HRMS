using System;
using System.Data;
using System.Data.SqlClient;

namespace HRMS_ERP.DataAccess
{
    public class EmployeeLeaveBalanceDAL : BaseDAL
    {
        // ─── GET leave balance for one employee for a year ────────────────
        public DataTable GetBalance(int employeeID, int year)
        {
            string query = @"
                SELECT lb.*, e.FullName
                FROM EmployeeLeaveBalance lb
                INNER JOIN Employee e ON lb.EmployeeID = e.EmployeeID
                WHERE lb.EmployeeID = @EID AND lb.Year = @Year";
            SqlParameter[] param = {
                new SqlParameter("@EID",  employeeID),
                new SqlParameter("@Year", year)
            };
            return GetDataTable(query, param);
        }

        // ─── GET all leave balances for a year (for the leave report) ─────
        public DataTable GetAllForYear(int year)
        {
            string query = @"
                SELECT lb.*, e.FullName, e.Department
                FROM EmployeeLeaveBalance lb
                INNER JOIN Employee e ON lb.EmployeeID = e.EmployeeID
                WHERE lb.Year = @Year
                ORDER BY e.FullName";
            SqlParameter[] param = { new SqlParameter("@Year", year) };
            return GetDataTable(query, param);
        }

        // ─── CREATE default leave balance for a new employee ─────────────
        // CL=10, SL=8, AL=30 (defaults set in DB)
        public int InsertDefault(int employeeID, int year)
        {
            // Don't insert if already exists
            string check = "SELECT COUNT(*) FROM EmployeeLeaveBalance WHERE EmployeeID = @EID AND Year = @Year";
            SqlParameter[] cp = { new SqlParameter("@EID", employeeID), new SqlParameter("@Year", year) };
            object cnt = ExecuteScalar(check, cp);
            if (cnt != null && Convert.ToInt32(cnt) > 0) return 0;

            string query = @"
                INSERT INTO EmployeeLeaveBalance (EmployeeID, Year, CasualLeaveRemaining, SickLeaveRemaining, AnnualLeaveRemaining)
                VALUES (@EID, @Year, 10, 8, 30)";
            SqlParameter[] param = {
                new SqlParameter("@EID",  employeeID),
                new SqlParameter("@Year", year)
            };
            return ExecuteNonQuery(query, param);
        }

        // ─── ADMIN manually corrects leave balance ────────────────────────
        public int UpdateBalance(int employeeID, int year, int casual, int sick, int annual, string performedBy)
        {
            string query = @"
                UPDATE EmployeeLeaveBalance SET
                    CasualLeaveRemaining  = @CL,
                    SickLeaveRemaining    = @SL,
                    AnnualLeaveRemaining  = @AL
                WHERE EmployeeID = @EID AND Year = @Year";
            SqlParameter[] param = {
                new SqlParameter("@EID",  employeeID),
                new SqlParameter("@Year", year),
                new SqlParameter("@CL",   casual),
                new SqlParameter("@SL",   sick),
                new SqlParameter("@AL",   annual)
            };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("EmployeeLeaveBalance", "UPDATE", employeeID, performedBy);
            return rows;
        }

        // ─── INITIALIZE leave for ALL active employees for a new year ─────
        // Call this every January 1st (or manually from Settings)
        public int InitializeYearForAllEmployees(int year)
        {
            string query = @"
                INSERT INTO EmployeeLeaveBalance (EmployeeID, Year, CasualLeaveRemaining, SickLeaveRemaining, AnnualLeaveRemaining)
                SELECT EmployeeID, @Year, 10, 8, 30
                FROM Employee
                WHERE IsActive = 1
                  AND EmployeeID NOT IN (
                      SELECT EmployeeID FROM EmployeeLeaveBalance WHERE Year = @Year
                  )";
            SqlParameter[] param = { new SqlParameter("@Year", year) };
            return ExecuteNonQuery(query, param);
        }
    }
}
