using System;
using System.Data;
using System.Data.SqlClient;

namespace HRMS_ERP.DataAccess
{
    public class EmployeeSalaryStructureDAL : BaseDAL
    {
        public DataTable GetActive(int employeeID)
        {
            string q = @"SELECT ss.SalaryStructureID, ss.EmployeeID, e.FullName,
                       ss.BasicPay, ss.Allowances, ss.Bonus, ss.EffectiveFrom, ss.EffectiveTo, ss.IsActive
                FROM EmployeeSalaryStructure ss
                INNER JOIN Employee e ON ss.EmployeeID = e.EmployeeID
                WHERE ss.EmployeeID = @EID AND ss.IsActive = 1";
            return GetDataTable(q, new[] { new SqlParameter("@EID", employeeID) });
        }

        public DataTable GetHistory(int employeeID)
        {
            string q = @"SELECT SalaryStructureID, BasicPay, Allowances, Bonus,
                       EffectiveFrom, EffectiveTo, IsActive
                FROM EmployeeSalaryStructure WHERE EmployeeID = @EID ORDER BY EffectiveFrom DESC";
            return GetDataTable(q, new[] { new SqlParameter("@EID", employeeID) });
        }

        public DataTable GetAllActive()
        {
            string q = @"SELECT ss.SalaryStructureID, ss.EmployeeID, e.FullName, e.Department,
                       ss.BasicPay, ss.Allowances, ss.Bonus,
                       (ss.BasicPay + ISNULL(ss.Allowances,0) + ISNULL(ss.Bonus,0)) AS TotalPackage,
                       ss.EffectiveFrom, ss.EffectiveTo, ss.IsActive
                FROM EmployeeSalaryStructure ss
                INNER JOIN Employee e ON ss.EmployeeID = e.EmployeeID
                WHERE ss.IsActive = 1 ORDER BY e.FullName";
            return GetDataTable(q);
        }

        public int Insert(int employeeID, decimal basicPay, decimal allowances,
                          decimal bonus, DateTime effectiveFrom, string performedBy)
        {
            string deact = @"UPDATE EmployeeSalaryStructure SET IsActive=0, EffectiveTo=@EffFrom
                WHERE EmployeeID=@EID AND IsActive=1";
            ExecuteNonQuery(deact, new[] {
                new SqlParameter("@EID", employeeID),
                new SqlParameter("@EffFrom", effectiveFrom) });

            string q = @"INSERT INTO EmployeeSalaryStructure
                    (EmployeeID,BasicPay,Allowances,Bonus,EffectiveFrom,IsActive)
                VALUES (@EID,@Basic,@Allow,@Bonus,@EffFrom,1); SELECT SCOPE_IDENTITY();";
            object result = ExecuteScalar(q, new[] {
                new SqlParameter("@EID",     employeeID),
                new SqlParameter("@Basic",   basicPay),
                new SqlParameter("@Allow",   allowances),
                new SqlParameter("@Bonus",   bonus),
                new SqlParameter("@EffFrom", effectiveFrom) });
            int newID = result != null ? Convert.ToInt32(result) : 0;
            AuditLogDAL.LogAction("EmployeeSalaryStructure", "INSERT", newID, performedBy);
            return newID;
        }

        public int Update(int salaryStructureID, decimal basicPay, decimal allowances,
                          decimal bonus, string performedBy)
        {
            string q = @"UPDATE EmployeeSalaryStructure
                SET BasicPay=@Basic, Allowances=@Allow, Bonus=@Bonus
                WHERE SalaryStructureID=@ID AND IsActive=1";
            int rows = ExecuteNonQuery(q, new[] {
                new SqlParameter("@ID",    salaryStructureID),
                new SqlParameter("@Basic", basicPay),
                new SqlParameter("@Allow", allowances),
                new SqlParameter("@Bonus", bonus) });
            AuditLogDAL.LogAction("EmployeeSalaryStructure", "UPDATE", salaryStructureID, performedBy);
            return rows;
        }

        // ─── DEACTIVATE — sets IsActive=0, EffectiveTo=today ──────────────
        public int Deactivate(int salaryStructureID, string performedBy)
        {
            string q = @"UPDATE EmployeeSalaryStructure
                SET IsActive=0, EffectiveTo=@Today WHERE SalaryStructureID=@ID";
            int rows = ExecuteNonQuery(q, new[] {
                new SqlParameter("@ID",    salaryStructureID),
                new SqlParameter("@Today", DateTime.Today) });
            AuditLogDAL.LogAction("EmployeeSalaryStructure", "DEACTIVATE", salaryStructureID, performedBy);
            return rows;
        }

        public bool HasActiveSalary(int employeeID)
        {
            string q = "SELECT COUNT(*) FROM EmployeeSalaryStructure WHERE EmployeeID=@EID AND IsActive=1";
            object r = ExecuteScalar(q, new[] { new SqlParameter("@EID", employeeID) });
            return r != null && Convert.ToInt32(r) > 0;
        }
    }

    public class TaxSlabDAL : BaseDAL
    {
        public DataTable GetAll()
            => GetDataTable("SELECT SlabID,FromAmount,ToAmount,TaxPercent FROM TaxSlab ORDER BY FromAmount");

        public decimal GetTaxPercent(decimal grossSalary)
        {
            string q = "SELECT TOP 1 TaxPercent FROM TaxSlab WHERE @Gross BETWEEN FromAmount AND ToAmount";
            object r = ExecuteScalar(q, new[] { new SqlParameter("@Gross", grossSalary) });
            return (r != null && r != DBNull.Value) ? Convert.ToDecimal(r) : 0;
        }

        public int Insert(decimal from, decimal to, decimal percent, string performedBy)
        {
            string q = @"INSERT INTO TaxSlab (FromAmount,ToAmount,TaxPercent)
                VALUES (@From,@To,@Pct); SELECT SCOPE_IDENTITY();";
            object r = ExecuteScalar(q, new[] {
                new SqlParameter("@From", from), new SqlParameter("@To", to), new SqlParameter("@Pct", percent) });
            int newID = r != null ? Convert.ToInt32(r) : 0;
            AuditLogDAL.LogAction("TaxSlab", "INSERT", newID, performedBy);
            return newID;
        }

        public int Update(int slabID, decimal from, decimal to, decimal percent, string performedBy)
        {
            string q = "UPDATE TaxSlab SET FromAmount=@From,ToAmount=@To,TaxPercent=@Pct WHERE SlabID=@ID";
            int rows = ExecuteNonQuery(q, new[] {
                new SqlParameter("@ID", slabID), new SqlParameter("@From", from),
                new SqlParameter("@To", to),     new SqlParameter("@Pct", percent) });
            AuditLogDAL.LogAction("TaxSlab", "UPDATE", slabID, performedBy);
            return rows;
        }

        public int Delete(int slabID, string performedBy)
        {
            int rows = ExecuteNonQuery("DELETE FROM TaxSlab WHERE SlabID=@ID",
                new[] { new SqlParameter("@ID", slabID) });
            AuditLogDAL.LogAction("TaxSlab", "DELETE", slabID, performedBy);
            return rows;
        }

        public void SeedDefaults()
        {
            object cnt = ExecuteScalar("SELECT COUNT(*) FROM TaxSlab");
            if (cnt != null && Convert.ToInt32(cnt) > 0) return;
            ExecuteNonQuery(@"INSERT INTO TaxSlab(FromAmount,ToAmount,TaxPercent) VALUES(0,50000,0);
                INSERT INTO TaxSlab(FromAmount,ToAmount,TaxPercent) VALUES(50001,100000,5);
                INSERT INTO TaxSlab(FromAmount,ToAmount,TaxPercent) VALUES(100001,200000,10);
                INSERT INTO TaxSlab(FromAmount,ToAmount,TaxPercent) VALUES(200001,500000,15);
                INSERT INTO TaxSlab(FromAmount,ToAmount,TaxPercent) VALUES(500001,999999999,20);");
        }
    }

    public class EmployeeTaxDAL : BaseDAL
    {
        public DataTable GetByEmployee(int employeeID)
        {
            string q = @"SELECT TaxID,TaxMonth,TaxYear,TaxableIncome,TaxAmount
                FROM EmployeeTax WHERE EmployeeID=@EID ORDER BY TaxYear DESC,TaxMonth DESC";
            return GetDataTable(q, new[] { new SqlParameter("@EID", employeeID) });
        }

        public DataTable GetMonthlyReport(int month, int year)
        {
            string q = @"SELECT t.TaxID,t.EmployeeID,e.FullName,e.Department,
                       t.TaxMonth,t.TaxYear,t.TaxableIncome,t.TaxAmount
                FROM EmployeeTax t INNER JOIN Employee e ON t.EmployeeID=e.EmployeeID
                WHERE t.TaxMonth=@Month AND t.TaxYear=@Year ORDER BY e.FullName";
            return GetDataTable(q, new[] { new SqlParameter("@Month", month), new SqlParameter("@Year", year) });
        }

        public DataTable GetYearlySummary(int year)
        {
            string q = @"SELECT t.EmployeeID,e.FullName,e.Department,
                       SUM(t.TaxableIncome) AS TotalIncome,SUM(t.TaxAmount) AS TotalTaxPaid
                FROM EmployeeTax t INNER JOIN Employee e ON t.EmployeeID=e.EmployeeID
                WHERE t.TaxYear=@Year GROUP BY t.EmployeeID,e.FullName,e.Department ORDER BY e.FullName";
            return GetDataTable(q, new[] { new SqlParameter("@Year", year) });
        }
    }
}
