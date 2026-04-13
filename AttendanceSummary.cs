using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_ERP.Models
{
    public class AttendanceSummary
    {
        public int SummaryID { get; set; }
        public int EmployeeID { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int? WorkingDays { get; set; }
        public int? PresentDays { get; set; }
        public int? LateDays { get; set; }
        public int? CasualLeaveTaken { get; set; }
        public int? SickLeaveTaken { get; set; }
        public int? AnnualLeaveTaken { get; set; }
        public int? LeaveWithoutPay { get; set; }
        public int? LeaveDeductedFromLate { get; set; }
        public int? ExtraUnpaidFromLate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
