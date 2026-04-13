using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_ERP.Models
{
    public class PayrollMaster
    {
        public int PayrollID { get; set; }
        public int PayrollMonth { get; set; }
        public int PayrollYear { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string Status { get; set; }
    }
}
