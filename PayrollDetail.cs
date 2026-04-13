using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_ERP.Models
{
    public class PayrollDetail
    {
        public int PayrollDetailID { get; set; }
        public int PayrollID { get; set; }
        public int EmployeeID { get; set; }
        public decimal BasicPay { get; set; }
        public decimal? Allowances { get; set; }
        public decimal? OvertimeAmount { get; set; }
        public decimal? Bonus { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal? TaxDeduction { get; set; }
        public decimal? OtherDeductions { get; set; }
        public decimal NetSalary { get; set; }
        public decimal? AttendanceDeductions { get; set; }
    }
}
