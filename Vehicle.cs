using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_ERP.Models
{
    public class Vehicle
    {
        public int VehicleID { get; set; }
        public string VehicleType { get; set; }
        public string RegistrationNumber { get; set; }
        public string Model { get; set; }
        public string ChassisNumber { get; set; }
        public bool? IsActive { get; set; }
    }
}
