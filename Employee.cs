using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_ERP.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string CNIC { get; set; }
        public string ContactNumber { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public int? ReportingManagerID { get; set; }
        public DateTime JoiningDate { get; set; }
        public TimeSpan? ShiftStartTime { get; set; }
        public TimeSpan? ShiftEndTime { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

