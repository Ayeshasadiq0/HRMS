using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_ERP.Models
{
    public class EmployeeTax
    {
        public int TaxID { get; set; }
        public int EmployeeID { get; set; }
        public int TaxMonth { get; set; }
        public int TaxYear { get; set; }
        public decimal? TaxableIncome { get; set; }
        public decimal? TaxAmount { get; set; }
    }
}
