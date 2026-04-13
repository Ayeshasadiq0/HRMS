using System;
using System.Data;
using System.Data.SqlClient;

namespace HRMS_ERP.DataAccess
{
    public class PayrollMasterDAL : BaseDAL
    {
        public void ProcessMonthlyPayroll(int employeeID, int month, int year, string processedBy)
        {
            SqlParameter[] param = {
                new SqlParameter("@EmployeeID",  employeeID),
                new SqlParameter("@Month",       month),
                new SqlParameter("@Year",        year),
                new SqlParameter("@ProcessedBy", processedBy)
            };
            ExecuteStoredProcedureNonQuery("sp_ProcessMonthlyPayroll", param);
        }

        public void LockPayrollMonth(int month, int year)
        {
            SqlParameter[] param = {
                new SqlParameter("@Month", month),
                new SqlParameter("@Year",  year)
            };
            ExecuteStoredProcedureNonQuery("sp_LockPayrollMonth", param);
        }

        public bool IsMonthLocked(int month, int year)
        {
            string query = @"SELECT COUNT(*) FROM PayrollMaster
                WHERE PayrollMonth = @Month AND PayrollYear = @Year AND Status = 'Processed'";
            SqlParameter[] param = {
                new SqlParameter("@Month", month),
                new SqlParameter("@Year",  year)
            };
            object result = ExecuteScalar(query, param);
            return result != null && Convert.ToInt32(result) > 0;
        }

        public DataTable GetPayrollStatus(int month, int year)
        {
            string query = "SELECT * FROM PayrollMaster WHERE PayrollMonth = @M AND PayrollYear = @Y";
            SqlParameter[] param = { new SqlParameter("@M", month), new SqlParameter("@Y", year) };
            return GetDataTable(query, param);
        }

        public DataTable GetAllMonths()
        {
            string query = @"SELECT PayrollID, PayrollMonth, PayrollYear, ProcessedDate, Status
                FROM PayrollMaster ORDER BY PayrollYear DESC, PayrollMonth DESC";
            return GetDataTable(query);
        }

        public DataTable GetPayrollDetails(int month, int year)
        {
            string query = @"
                SELECT pd.PayrollDetailID, pd.EmployeeID, e.FullName, e.Department,
                       pd.BasicPay, pd.Allowances, pd.OvertimeAmount, pd.Bonus,
                       pd.GrossSalary, pd.TaxDeduction, pd.AttendanceDeductions,
                       pd.OtherDeductions, pd.NetSalary, e.IsActive
                FROM PayrollDetail pd
                INNER JOIN Employee e      ON pd.EmployeeID = e.EmployeeID
                INNER JOIN PayrollMaster pm ON pd.PayrollID  = pm.PayrollID
                WHERE pm.PayrollMonth = @Month AND pm.PayrollYear = @Year
                ORDER BY e.FullName";
            SqlParameter[] param = {
                new SqlParameter("@Month", month),
                new SqlParameter("@Year",  year)
            };
            return GetDataTable(query, param);
        }

        public DataTable GetPayslip(int employeeID, int month, int year)
        {
            string query = @"
                SELECT pd.*, e.FullName, e.Department, e.Designation,
                       e.BankAccountNumber, e.BankName,
                       pm.PayrollMonth, pm.PayrollYear
                FROM PayrollDetail pd
                INNER JOIN Employee e      ON pd.EmployeeID = e.EmployeeID
                INNER JOIN PayrollMaster pm ON pd.PayrollID  = pm.PayrollID
                WHERE pd.EmployeeID   = @EID
                  AND pm.PayrollMonth = @Month
                  AND pm.PayrollYear  = @Year";
            SqlParameter[] param = {
                new SqlParameter("@EID",   employeeID),
                new SqlParameter("@Month", month),
                new SqlParameter("@Year",  year)
            };
            return GetDataTable(query, param);
        }

        public bool IsAlreadyProcessed(int employeeID, int month, int year)
        {
            string query = @"SELECT COUNT(*) FROM PayrollDetail pd
                INNER JOIN PayrollMaster pm ON pd.PayrollID = pm.PayrollID
                WHERE pd.EmployeeID = @EID AND pm.PayrollMonth = @Month AND pm.PayrollYear = @Year";
            SqlParameter[] param = {
                new SqlParameter("@EID",   employeeID),
                new SqlParameter("@Month", month),
                new SqlParameter("@Year",  year)
            };
            object result = ExecuteScalar(query, param);
            return result != null && Convert.ToInt32(result) > 0;
        }

        // ─── UPDATE a payroll detail record (inline edit) ──────────────────
        public int UpdatePayrollDetail(int payrollDetailID, decimal basicPay, decimal allowances,
            decimal overtimeAmount, decimal bonus, decimal grossSalary, decimal taxDeduction,
            decimal attendanceDeductions, decimal otherDeductions, decimal netSalary, string performedBy)
        {
            string query = @"
                UPDATE PayrollDetail
                SET BasicPay             = @BP,
                    Allowances           = @AL,
                    OvertimeAmount       = @OT,
                    Bonus                = @BN,
                    GrossSalary          = @GS,
                    TaxDeduction         = @TX,
                    AttendanceDeductions = @AD,
                    OtherDeductions      = @OD,
                    NetSalary            = @NS
                WHERE PayrollDetailID = @ID";
            SqlParameter[] param = {
                new SqlParameter("@ID", payrollDetailID),
                new SqlParameter("@BP", basicPay),
                new SqlParameter("@AL", allowances),
                new SqlParameter("@OT", overtimeAmount),
                new SqlParameter("@BN", bonus),
                new SqlParameter("@GS", grossSalary),
                new SqlParameter("@TX", taxDeduction),
                new SqlParameter("@AD", attendanceDeductions),
                new SqlParameter("@OD", otherDeductions),
                new SqlParameter("@NS", netSalary)
            };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("PayrollDetail", "UPDATE", payrollDetailID, performedBy);
            return rows;
        }

        // ─── DELETE a payroll detail record ───────────────────────────────
        public int DeletePayrollDetail(int payrollDetailID, string performedBy)
        {
            // Also delete related EmployeeTax entry for the same month
            string delTax = @"
                DELETE et FROM EmployeeTax et
                INNER JOIN PayrollDetail pd ON et.EmployeeID = pd.EmployeeID
                INNER JOIN PayrollMaster pm ON pd.PayrollID  = pm.PayrollID
                WHERE pd.PayrollDetailID = @ID
                  AND et.TaxMonth = pm.PayrollMonth
                  AND et.TaxYear  = pm.PayrollYear";
            SqlParameter[] taxParam = { new SqlParameter("@ID", payrollDetailID) };
            ExecuteNonQuery(delTax, taxParam);

            string query = "DELETE FROM PayrollDetail WHERE PayrollDetailID = @ID";
            SqlParameter[] param = { new SqlParameter("@ID", payrollDetailID) };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("PayrollDetail", "DELETE", payrollDetailID, performedBy);
            return rows;
        }
    }
}
