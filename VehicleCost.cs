using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_ERP.Models
{
    public class VehicleCost
    {
        public int CostID { get; set; }
        public int AssignmentID { get; set; }
        public decimal? FuelCost { get; set; }
        public decimal? MaintenanceCost { get; set; }
        public int? CostMonth { get; set; }
        public int? CostYear { get; set; }
    }
}
