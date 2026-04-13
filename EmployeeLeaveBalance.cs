using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_ERP.Models
{
    public class EmployeeLeaveBalance
    {
        public int LeaveBalanceID { get; set; }
        public int EmployeeID { get; set; }
        public int Year { get; set; }
        public int? CasualLeaveRemaining { get; set; }
        public int? SickLeaveRemaining { get; set; }
        public int? AnnualLeaveRemaining { get; set; }
    }
}
