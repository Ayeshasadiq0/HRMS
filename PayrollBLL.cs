using System;
using System.Data;
using HRMS_ERP.DataAccess;

namespace HRMS_ERP.BusinessLogic
{
    public class PayrollBLL
    {
        private readonly PayrollMasterDAL           _payrollDal = new PayrollMasterDAL();
        private readonly AttendanceSummaryDAL       _summaryDal = new AttendanceSummaryDAL();
        private readonly EmployeeSalaryStructureDAL _salaryDal  = new EmployeeSalaryStructureDAL();
        private readonly EmployeeDAL                _empDal     = new EmployeeDAL();

        public void ProcessPayroll(int employeeID, int month, int year, string processedBy)
        {
            if (month < 1 || month > 12) throw new ArgumentException("Month must be between 1 and 12.");
            if (new DateTime(year, month, 1) > DateTime.Today)
                throw new InvalidOperationException("Cannot process payroll for a future month.");

            DataTable emp = _empDal.GetByID(employeeID);
            if (emp.Rows.Count == 0) throw new InvalidOperationException("Employee not found.");
            if (emp.Rows[0]["IsActive"] == DBNull.Value || !(bool)emp.Rows[0]["IsActive"])
                throw new InvalidOperationException("Cannot process payroll for an inactive employee.");
            if (_payrollDal.IsMonthLocked(month, year))
                throw new InvalidOperationException($"Payroll for {GetMonthName(month)} {year} is locked.");
            if (_payrollDal.IsAlreadyProcessed(employeeID, month, year))
                throw new InvalidOperationException("Payroll already processed for this employee this month.");
            if (!_summaryDal.SummaryExists(employeeID, month, year))
                throw new InvalidOperationException(
                    "Attendance summary not found.\n\nGenerate the monthly attendance summary first.");
            if (!_salaryDal.HasActiveSalary(employeeID))
                throw new InvalidOperationException("No active salary structure found. Assign a salary first.");

            _payrollDal.ProcessMonthlyPayroll(employeeID, month, year, processedBy);
        }

        public void LockMonth(int month, int year, string performedBy)
        {
            if (_payrollDal.IsMonthLocked(month, year))
                throw new InvalidOperationException($"{GetMonthName(month)} {year} is already locked.");
            _payrollDal.LockPayrollMonth(month, year);
        }

        public DataTable GetMonthlyPayrollReport(int month, int year)
            => _payrollDal.GetPayrollDetails(month, year);

        public DataTable GetPayslip(int employeeID, int month, int year)
        {
            DataTable dt = _payrollDal.GetPayslip(employeeID, month, year);
            if (dt.Rows.Count == 0)
                throw new InvalidOperationException("Payslip not found. Process payroll first.");
            return dt;
        }

        public DataTable GetAllMonths()   => _payrollDal.GetAllMonths();
        public DataTable GetPayrollStatus(int month, int year) => _payrollDal.GetPayrollStatus(month, year);
        public bool      IsMonthLocked(int month, int year)   => _payrollDal.IsMonthLocked(month, year);

        // ─── UPDATE payroll detail (inline edit) ──────────────────────────
        public int UpdatePayrollDetail(int payrollDetailID, decimal basicPay, decimal allowances,
            decimal overtimeAmount, decimal bonus, decimal grossSalary, decimal taxDeduction,
            decimal attendanceDeductions, decimal otherDeductions, decimal netSalary, string performedBy)
            => _payrollDal.UpdatePayrollDetail(payrollDetailID, basicPay, allowances, overtimeAmount,
               bonus, grossSalary, taxDeduction, attendanceDeductions, otherDeductions, netSalary, performedBy);

        // ─── DELETE payroll detail ────────────────────────────────────────
        public int DeletePayrollDetail(int payrollDetailID, string performedBy)
            => _payrollDal.DeletePayrollDetail(payrollDetailID, performedBy);

        private string GetMonthName(int month)
            => month >= 1 && month <= 12 ? new DateTime(2000, month, 1).ToString("MMMM") : "";
    }
}
