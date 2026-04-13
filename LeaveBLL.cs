using System;
using System.Data;
using HRMS_ERP.DataAccess;

namespace HRMS_ERP.BusinessLogic
{
    /// <summary>
    /// Leave Balance BLL
    /// Manages viewing and manually adjusting leave balances
    /// Actual deductions happen automatically via sp_GetMonthlyAttendanceSummary
    /// </summary>
    public class LeaveBLL
    {
        private readonly EmployeeLeaveBalanceDAL _dal = new EmployeeLeaveBalanceDAL();

        // ─── GET leave balance for one employee for a year ────────────────
        public DataTable GetBalance(int employeeID, int year)
        {
            return _dal.GetBalance(employeeID, year);
        }

        // ─── GET all balances for a year (leave report) ───────────────────
        public DataTable GetAllForYear(int year)
        {
            return _dal.GetAllForYear(year);
        }

        // ─── MANUALLY ADJUST leave balance (Admin only) ───────────────────
        public int AdjustBalance(int employeeID, int year, int casual,
                                 int sick, int annual, string performedBy)
        {
            if (casual < 0)
                throw new ArgumentException("Casual Leave balance cannot be negative.");

            if (sick < 0)
                throw new ArgumentException("Sick Leave balance cannot be negative.");

            if (annual < 0)
                throw new ArgumentException("Annual Leave balance cannot be negative.");

            // Hard cap — cannot exceed annual entitlement
            if (casual > 10)
                throw new ArgumentException("Casual Leave cannot exceed 10 days.");

            if (sick > 8)
                throw new ArgumentException("Sick Leave cannot exceed 8 days.");

            if (annual > 30)
                throw new ArgumentException("Annual Leave cannot exceed 30 days.");

            return _dal.UpdateBalance(employeeID, year, casual, sick, annual, performedBy);
        }

        // ─── INITIALIZE leave for all active employees for a new year ─────
        // Should be called every January
        public int InitializeNewYear(int year, string performedBy)
        {
            if (year < DateTime.Today.Year)
                throw new ArgumentException("Cannot initialize leave for a past year.");

            return _dal.InitializeYearForAllEmployees(year);
        }

        // ─── CREATE leave balance for a single new employee ───────────────
        public int CreateForNewEmployee(int employeeID, int year)
        {
            return _dal.InsertDefault(employeeID, year);
        }
    }
}
