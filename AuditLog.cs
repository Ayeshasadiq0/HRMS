using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_ERP.Models
{
    public class AuditLog
    {
        public int AuditID { get; set; }
        public string TableName { get; set; }
        public string ActionType { get; set; }
        public int? RecordID { get; set; }
        public DateTime? ActionDate { get; set; }
        public string PerformedBy { get; set; }
    }
}
