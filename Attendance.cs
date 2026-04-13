using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_ERP.Models
{
    public class Attendance
    {
        public int AttendanceID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime AttendanceDate { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public decimal? WorkingHours { get; set; }
        public decimal? OvertimeHours { get; set; }
        public string AttendanceStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
