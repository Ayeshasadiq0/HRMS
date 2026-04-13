using System;
using System.Data;
using System.Data.SqlClient;

namespace HRMS_ERP.DataAccess
{
    public class AttendanceSummaryDAL : BaseDAL
    {
        private const string SELECT_COLS = @"s.SummaryID, s.EmployeeID, e.FullName, e.Department,
            s.Month, s.Year, s.WorkingDays, s.PresentDays, s.LateDays,
            s.CasualLeaveTaken, s.SickLeaveTaken, s.AnnualLeaveTaken,
            s.LeaveWithoutPay, s.LeaveDeductedFromLate, s.ExtraUnpaidFromLate";

        public DataTable GenerateSummary(int employeeID, int month, int year)
        {
            return ExecuteStoredProcedure("sp_GetMonthlyAttendanceSummary", new[] {
                new SqlParameter("@EmployeeID", employeeID),
                new SqlParameter("@Month",      month),
                new SqlParameter("@Year",       year) });
        }

        public DataTable GetSummary(int employeeID, int month, int year)
        {
            string q = $@"SELECT {SELECT_COLS} FROM AttendanceSummary s
                INNER JOIN Employee e ON s.EmployeeID=e.EmployeeID
                WHERE s.EmployeeID=@EID AND s.Month=@Month AND s.Year=@Year";
            return GetDataTable(q, new[] {
                new SqlParameter("@EID",   employeeID),
                new SqlParameter("@Month", month),
                new SqlParameter("@Year",  year) });
        }

        public DataTable GetAllForMonth(int month, int year)
        {
            string q = $@"SELECT {SELECT_COLS} FROM AttendanceSummary s
                INNER JOIN Employee e ON s.EmployeeID=e.EmployeeID
                WHERE s.Month=@Month AND s.Year=@Year ORDER BY e.FullName";
            return GetDataTable(q, new[] {
                new SqlParameter("@Month", month),
                new SqlParameter("@Year",  year) });
        }

        public DataTable GetAllForYear(int year)
        {
            string q = $@"SELECT {SELECT_COLS} FROM AttendanceSummary s
                INNER JOIN Employee e ON s.EmployeeID=e.EmployeeID
                WHERE s.Year=@Year ORDER BY e.FullName, s.Month";
            return GetDataTable(q, new[] { new SqlParameter("@Year", year) });
        }

        public DataTable GetAllForEmployee(int employeeID)
        {
            string q = $@"SELECT {SELECT_COLS} FROM AttendanceSummary s
                INNER JOIN Employee e ON s.EmployeeID=e.EmployeeID
                WHERE s.EmployeeID=@EID ORDER BY s.Year DESC, s.Month DESC";
            return GetDataTable(q, new[] { new SqlParameter("@EID", employeeID) });
        }

        public bool SummaryExists(int employeeID, int month, int year)
        {
            string q = @"SELECT COUNT(*) FROM AttendanceSummary
                WHERE EmployeeID=@EID AND Month=@Month AND Year=@Year";
            object r = ExecuteScalar(q, new[] {
                new SqlParameter("@EID",   employeeID),
                new SqlParameter("@Month", month),
                new SqlParameter("@Year",  year) });
            return r != null && Convert.ToInt32(r) > 0;
        }

        public int DeleteSummary(int employeeID, int month, int year)
        {
            string q = @"DELETE FROM AttendanceSummary
                WHERE EmployeeID=@EID AND Month=@Month AND Year=@Year";
            return ExecuteNonQuery(q, new[] {
                new SqlParameter("@EID",   employeeID),
                new SqlParameter("@Month", month),
                new SqlParameter("@Year",  year) });
        }

        public int DeleteSummaryByID(int summaryID, string performedBy)
        {
            int rows = ExecuteNonQuery("DELETE FROM AttendanceSummary WHERE SummaryID=@ID",
                new[] { new SqlParameter("@ID", summaryID) });
            AuditLogDAL.LogAction("AttendanceSummary", "DELETE", summaryID, performedBy);
            return rows;
        }

        public int UpdateSummary(int summaryID, int workingDays, int presentDays, int lateDays,
            int cl, int sl, int al, int lwp, int deductedFromLate, int extraUnpaid, string performedBy)
        {
            string q = @"UPDATE AttendanceSummary SET
                WorkingDays=@WD, PresentDays=@PD, LateDays=@LD,
                CasualLeaveTaken=@CL, SickLeaveTaken=@SL, AnnualLeaveTaken=@AL,
                LeaveWithoutPay=@LWP, LeaveDeductedFromLate=@DFL, ExtraUnpaidFromLate=@EUL
                WHERE SummaryID=@ID";
            int rows = ExecuteNonQuery(q, new[] {
                new SqlParameter("@ID",  summaryID), new SqlParameter("@WD",  workingDays),
                new SqlParameter("@PD",  presentDays), new SqlParameter("@LD",  lateDays),
                new SqlParameter("@CL",  cl),  new SqlParameter("@SL",  sl),
                new SqlParameter("@AL",  al),  new SqlParameter("@LWP", lwp),
                new SqlParameter("@DFL", deductedFromLate), new SqlParameter("@EUL", extraUnpaid) });
            AuditLogDAL.LogAction("AttendanceSummary", "UPDATE", summaryID, performedBy);
            return rows;
        }

        public DataTable GetActiveEmployeeIDs()
            => GetDataTable("SELECT EmployeeID FROM Employee WHERE IsActive=1");
    }
}
