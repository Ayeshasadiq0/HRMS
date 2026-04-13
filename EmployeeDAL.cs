using System;
using System.Data;
using System.Data.SqlClient;
using HRMS_ERP.Models;

namespace HRMS_ERP.DataAccess
{
    public class EmployeeDAL : BaseDAL
    {
        // GET ALL employees for the main grid
        public DataTable GetAll()
        {
            string query = @"
                SELECT e.EmployeeID, e.EmployeeCode, e.FullName, e.CNIC,
                       e.ContactNumber, e.Department, e.Designation,
                       m.FullName AS ReportingManager,
                       e.JoiningDate, e.ShiftStartTime, e.ShiftEndTime,
                       e.BankAccountNumber, e.BankName, e.IsActive
                FROM Employee e
                LEFT JOIN Employee m ON e.ReportingManagerID = m.EmployeeID
                ORDER BY e.FullName";
            return GetDataTable(query);
        }

        // GET active employees only (for dropdowns)
        public DataTable GetActiveEmployees()
        {
            string query = @"
                SELECT EmployeeID, EmployeeCode, FullName, Department, Designation
                FROM Employee WHERE IsActive = 1
                ORDER BY FullName";
            return GetDataTable(query);
        }

        // GET one employee by ID
        public DataTable GetByID(int employeeID)
        {
            string query = "SELECT * FROM Employee WHERE EmployeeID = @ID";
            SqlParameter[] param = { new SqlParameter("@ID", employeeID) };
            return GetDataTable(query, param);
        }

        // SEARCH — returns all columns so grid is fully populated
        public DataTable Search(string keyword, string department = "")
        {
            string query = @"
                SELECT e.EmployeeID, e.EmployeeCode, e.FullName, e.CNIC,
                       e.ContactNumber, e.Department, e.Designation,
                       e.JoiningDate, e.BankAccountNumber, e.BankName, e.IsActive
                FROM Employee e
                WHERE (e.FullName LIKE @Keyword OR e.EmployeeCode LIKE @Keyword
                       OR e.CNIC LIKE @Keyword OR e.Department LIKE @Keyword)
                  AND (@Department = '' OR e.Department = @Department)
                ORDER BY e.FullName";
            SqlParameter[] param = {
                new SqlParameter("@Keyword",    "%" + keyword + "%"),
                new SqlParameter("@Department", department)
            };
            return GetDataTable(query, param);
        }

        // INSERT new employee
        public int Insert(Employee emp, string performedBy)
        {
            string query = @"
                INSERT INTO Employee
                    (EmployeeCode, FullName, CNIC, ContactNumber, Department, Designation,
                     ReportingManagerID, JoiningDate, ShiftStartTime, ShiftEndTime,
                     BankAccountNumber, BankName)
                OUTPUT INSERTED.EmployeeID
                VALUES
                    (@Code, @Name, @CNIC, @Contact, @Dept, @Desig,
                     @Mgr, @Join, @ShiftStart, @ShiftEnd,
                     @BankAcc, @Bank)";

            SqlParameter[] param = {
                new SqlParameter("@Code",       emp.EmployeeCode),
                new SqlParameter("@Name",       emp.FullName),
                new SqlParameter("@CNIC",       emp.CNIC),
                new SqlParameter("@Contact",    (object)emp.ContactNumber     ?? DBNull.Value),
                new SqlParameter("@Dept",       (object)emp.Department        ?? DBNull.Value),
                new SqlParameter("@Desig",      (object)emp.Designation       ?? DBNull.Value),
                new SqlParameter("@Mgr",        emp.ReportingManagerID.HasValue ? (object)emp.ReportingManagerID.Value : DBNull.Value),
                new SqlParameter("@Join",       emp.JoiningDate),
                new SqlParameter("@ShiftStart", (object)emp.ShiftStartTime    ?? DBNull.Value),
                new SqlParameter("@ShiftEnd",   (object)emp.ShiftEndTime      ?? DBNull.Value),
                new SqlParameter("@BankAcc",    (object)emp.BankAccountNumber ?? DBNull.Value),
                new SqlParameter("@Bank",       (object)emp.BankName          ?? DBNull.Value)
            };

            object result = ExecuteScalar(query, param);
            int newID = result != null ? Convert.ToInt32(result) : 0;
            AuditLogDAL.LogAction("Employee", "INSERT", newID, performedBy);
            return newID;
        }

        // UPDATE existing employee
        public int Update(Employee emp, string performedBy)
        {
            string query = @"
                UPDATE Employee SET
                    FullName           = @Name,
                    CNIC               = @CNIC,
                    ContactNumber      = @Contact,
                    Department         = @Dept,
                    Designation        = @Desig,
                    ReportingManagerID = @Mgr,
                    JoiningDate        = @Join,
                    ShiftStartTime     = @ShiftStart,
                    ShiftEndTime       = @ShiftEnd,
                    BankAccountNumber  = @BankAcc,
                    BankName           = @Bank,
                    IsActive           = @IsActive
                WHERE EmployeeID = @ID";

            SqlParameter[] param = {
                new SqlParameter("@ID",         emp.EmployeeID),
                new SqlParameter("@Name",       emp.FullName),
                new SqlParameter("@CNIC",       emp.CNIC),
                new SqlParameter("@Contact",    (object)emp.ContactNumber     ?? DBNull.Value),
                new SqlParameter("@Dept",       (object)emp.Department        ?? DBNull.Value),
                new SqlParameter("@Desig",      (object)emp.Designation       ?? DBNull.Value),
                new SqlParameter("@Mgr",        emp.ReportingManagerID.HasValue ? (object)emp.ReportingManagerID.Value : DBNull.Value),
                new SqlParameter("@Join",       emp.JoiningDate),
                new SqlParameter("@ShiftStart", (object)emp.ShiftStartTime    ?? DBNull.Value),
                new SqlParameter("@ShiftEnd",   (object)emp.ShiftEndTime      ?? DBNull.Value),
                new SqlParameter("@BankAcc",    (object)emp.BankAccountNumber ?? DBNull.Value),
                new SqlParameter("@Bank",       (object)emp.BankName          ?? DBNull.Value),
                new SqlParameter("@IsActive",   emp.IsActive ?? true)
            };

            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("Employee", "UPDATE", emp.EmployeeID, performedBy);
            return rows;
        }

        // REACTIVATE — sets IsActive = 1
        public int Reactivate(int employeeID, string performedBy)
        {
            string query = "UPDATE Employee SET IsActive = 1 WHERE EmployeeID = @ID";
            SqlParameter[] param = { new SqlParameter("@ID", employeeID) };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("Employee", "REACTIVATE", employeeID, performedBy);
            return rows;
        }

        // DEACTIVATE — sets IsActive = 0 (kept for any future use)
        public int Deactivate(int employeeID, string performedBy)
        {
            string query = "UPDATE Employee SET IsActive = 0 WHERE EmployeeID = @ID";
            SqlParameter[] param = { new SqlParameter("@ID", employeeID) };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("Employee", "DEACTIVATE", employeeID, performedBy);
            return rows;
        }

        // PERMANENT DELETE — removes the record completely
        public void Delete(int employeeID, string performedBy)
        {
            string query = "DELETE FROM Employee WHERE EmployeeID = @ID";
            SqlParameter[] param = { new SqlParameter("@ID", employeeID) };
            ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("Employee", "DELETE", employeeID, performedBy);
        }

        // DUPLICATE CHECKS
        public bool IsCNICDuplicate(string cnic, int excludeEmployeeID = 0)
        {
            string query = "SELECT COUNT(*) FROM Employee WHERE CNIC = @CNIC AND EmployeeID <> @ExcludeID";
            SqlParameter[] param = {
                new SqlParameter("@CNIC",      cnic),
                new SqlParameter("@ExcludeID", excludeEmployeeID)
            };
            object result = ExecuteScalar(query, param);
            return result != null && Convert.ToInt32(result) > 0;
        }

        public bool IsCodeDuplicate(string code, int excludeEmployeeID = 0)
        {
            string query = "SELECT COUNT(*) FROM Employee WHERE EmployeeCode = @Code AND EmployeeID <> @ExcludeID";
            SqlParameter[] param = {
                new SqlParameter("@Code",      code),
                new SqlParameter("@ExcludeID", excludeEmployeeID)
            };
            object result = ExecuteScalar(query, param);
            return result != null && Convert.ToInt32(result) > 0;
        }

        // GET distinct departments for filter dropdown
        public DataTable GetDepartments()
        {
            string query = "SELECT DISTINCT Department FROM Employee WHERE Department IS NOT NULL ORDER BY Department";
            return GetDataTable(query);
        }
    }
}
