using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_ERP.Models
{
    public class VehicleAssignment
    {
        public int AssignmentID { get; set; }
        public int VehicleID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
