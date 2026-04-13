using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_ERP.Models
{
    public class TaxSlab
    {
        public int SlabID { get; set; }
        public decimal? FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public decimal? TaxPercent { get; set; }
    }
}
