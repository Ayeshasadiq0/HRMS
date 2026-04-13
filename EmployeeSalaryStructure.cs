using System;

namespace HRMS_ERP.Models
{
    public class EmployeeSalaryStructure
    {
        public int       SalaryStructureID { get; set; }
        public int       EmployeeID        { get; set; }
        public decimal   BasicPay          { get; set; }
        public decimal?  Allowances        { get; set; }
        public decimal?  Bonus             { get; set; }  // ← NEW
        public DateTime  EffectiveFrom     { get; set; }
        public DateTime? EffectiveTo       { get; set; }
        public bool?     IsActive          { get; set; }
        public DateTime? CreatedAt         { get; set; }
    }
}
