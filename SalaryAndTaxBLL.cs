using System;
using System.Data;
using HRMS_ERP.DataAccess;

namespace HRMS_ERP.BusinessLogic
{
    public class SalaryBLL
    {
        private readonly EmployeeSalaryStructureDAL _dal = new EmployeeSalaryStructureDAL();

        public DataTable GetActive(int employeeID)  => _dal.GetActive(employeeID);
        public DataTable GetHistory(int employeeID) => _dal.GetHistory(employeeID);
        public DataTable GetAllActive()             => _dal.GetAllActive();
        public bool      HasActiveSalary(int empID) => _dal.HasActiveSalary(empID);

        public int AssignSalary(int employeeID, decimal basicPay, decimal allowances,
                                decimal bonus, DateTime effectiveFrom, string performedBy)
        {
            if (basicPay <= 0)  throw new ArgumentException("Basic Pay must be greater than zero.");
            if (allowances < 0) throw new ArgumentException("Allowances cannot be negative.");
            if (bonus < 0)      throw new ArgumentException("Bonus cannot be negative.");
            if (effectiveFrom > DateTime.Today.AddMonths(1))
                throw new ArgumentException("Effective date cannot be more than one month in the future.");
            return _dal.Insert(employeeID, basicPay, allowances, bonus, effectiveFrom, performedBy);
        }

        public int UpdateSalary(int salaryStructureID, decimal basicPay, decimal allowances,
                                decimal bonus, string performedBy)
        {
            if (basicPay <= 0)  throw new ArgumentException("Basic Pay must be greater than zero.");
            if (allowances < 0) throw new ArgumentException("Allowances cannot be negative.");
            if (bonus < 0)      throw new ArgumentException("Bonus cannot be negative.");
            return _dal.Update(salaryStructureID, basicPay, allowances, bonus, performedBy);
        }

        public int DeactivateSalary(int salaryStructureID, string performedBy)
            => _dal.Deactivate(salaryStructureID, performedBy);
    }

    public class TaxBLL
    {
        private readonly TaxSlabDAL     _slabDal = new TaxSlabDAL();
        private readonly EmployeeTaxDAL _taxDal  = new EmployeeTaxDAL();

        public DataTable GetAllSlabs()                                => _slabDal.GetAll();
        public DataTable GetEmployeeTaxHistory(int employeeID)        => _taxDal.GetByEmployee(employeeID);
        public DataTable GetMonthlyReport(int month, int year)        => _taxDal.GetMonthlyReport(month, year);
        public DataTable GetYearlySummary(int year)                   => _taxDal.GetYearlySummary(year);
        public decimal   GetTaxPercent(decimal grossSalary)           => _slabDal.GetTaxPercent(grossSalary);

        public int AddSlab(decimal from, decimal to, decimal percent, string performedBy)
        {
            if (from < 0)              throw new ArgumentException("From Amount cannot be negative.");
            if (to <= from)            throw new ArgumentException("To Amount must be greater than From Amount.");
            if (percent < 0 || percent > 100) throw new ArgumentException("Tax Percent must be 0–100.");
            return _slabDal.Insert(from, to, percent, performedBy);
        }

        public int UpdateSlab(int slabID, decimal from, decimal to, decimal percent, string performedBy)
        {
            if (to <= from)            throw new ArgumentException("To Amount must be greater than From Amount.");
            if (percent < 0 || percent > 100) throw new ArgumentException("Tax Percent must be 0–100.");
            return _slabDal.Update(slabID, from, to, percent, performedBy);
        }

        public int DeleteSlab(int slabID, string performedBy) => _slabDal.Delete(slabID, performedBy);
    }
}
