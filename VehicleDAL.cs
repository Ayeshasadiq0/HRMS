using System;
using System.Data;
using System.Data.SqlClient;
using HRMS_ERP.Models;

namespace HRMS_ERP.DataAccess
{
    // ═══════════════════════════════════════════════════════════════════════
    // VEHICLE DAL  — registers and manages company vehicles
    // ═══════════════════════════════════════════════════════════════════════
    public class VehicleDAL : BaseDAL
    {
        public DataTable GetAll()
        {
            string query = "SELECT VehicleID, VehicleType, RegistrationNumber, Model, ChassisNumber, IsActive FROM Vehicle ORDER BY RegistrationNumber";
            return GetDataTable(query);
        }

        public DataTable GetActive()
        {
            string query = "SELECT VehicleID, VehicleType, RegistrationNumber, Model FROM Vehicle WHERE IsActive = 1 ORDER BY RegistrationNumber";
            return GetDataTable(query);
        }

        public int Insert(Vehicle v, string performedBy)
        {
            string query = @"
                INSERT INTO Vehicle (VehicleType, RegistrationNumber, Model, ChassisNumber, IsActive)
                VALUES (@Type, @Reg, @Model, @Chassis, 1);
                SELECT SCOPE_IDENTITY();";
            SqlParameter[] param = {
                new SqlParameter("@Type",    v.VehicleType),
                new SqlParameter("@Reg",     v.RegistrationNumber),
                new SqlParameter("@Model",   (object)v.Model         ?? DBNull.Value),
                new SqlParameter("@Chassis", (object)v.ChassisNumber ?? DBNull.Value)
            };
            object result = ExecuteScalar(query, param);
            int newID = result != null ? Convert.ToInt32(result) : 0;
            AuditLogDAL.LogAction("Vehicle", "INSERT", newID, performedBy);
            return newID;
        }

        public int Update(Vehicle v, string performedBy)
        {
            string query = @"
                UPDATE Vehicle SET VehicleType = @Type, RegistrationNumber = @Reg,
                    Model = @Model, ChassisNumber = @Chassis, IsActive = @Active
                WHERE VehicleID = @ID";
            SqlParameter[] param = {
                new SqlParameter("@ID",      v.VehicleID),
                new SqlParameter("@Type",    v.VehicleType),
                new SqlParameter("@Reg",     v.RegistrationNumber),
                new SqlParameter("@Model",   (object)v.Model         ?? DBNull.Value),
                new SqlParameter("@Chassis", (object)v.ChassisNumber ?? DBNull.Value),
                new SqlParameter("@Active",  v.IsActive ?? true)
            };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("Vehicle", "UPDATE", v.VehicleID, performedBy);
            return rows;
        }

        public bool IsRegNumberDuplicate(string regNumber, int excludeID = 0)
        {
            string query = "SELECT COUNT(*) FROM Vehicle WHERE RegistrationNumber = @Reg AND VehicleID <> @ExcludeID";
            SqlParameter[] param = { new SqlParameter("@Reg", regNumber), new SqlParameter("@ExcludeID", excludeID) };
            object result = ExecuteScalar(query, param);
            return result != null && Convert.ToInt32(result) > 0;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // VEHICLE ASSIGNMENT DAL  — assigns a vehicle to an employee
    // ═══════════════════════════════════════════════════════════════════════
    public class VehicleAssignmentDAL : BaseDAL
    {
        // Active assignments = EndDate is NULL (vehicle still assigned)
        public DataTable GetActive()
        {
            string query = @"
                SELECT va.AssignmentID, v.RegistrationNumber, v.VehicleType,
                       e.FullName, e.Department, va.StartDate
                FROM VehicleAssignment va
                INNER JOIN Vehicle v ON va.VehicleID = v.VehicleID
                INNER JOIN Employee e ON va.EmployeeID = e.EmployeeID
                WHERE va.EndDate IS NULL
                ORDER BY e.FullName";
            return GetDataTable(query);
        }

        public DataTable GetAll()
        {
            string query = @"
                SELECT va.AssignmentID, v.RegistrationNumber, v.VehicleType,
                       e.FullName, va.StartDate, va.EndDate
                FROM VehicleAssignment va
                INNER JOIN Vehicle v ON va.VehicleID = v.VehicleID
                INNER JOIN Employee e ON va.EmployeeID = e.EmployeeID
                ORDER BY va.StartDate DESC";
            return GetDataTable(query);
        }

        public int Assign(int vehicleID, int employeeID, DateTime startDate, string performedBy)
        {
            // End any previous assignment of this same vehicle
            string endPrev = "UPDATE VehicleAssignment SET EndDate = @Start WHERE VehicleID = @VID AND EndDate IS NULL";
            SqlParameter[] ep = { new SqlParameter("@VID", vehicleID), new SqlParameter("@Start", startDate) };
            ExecuteNonQuery(endPrev, ep);

            string query = @"
                INSERT INTO VehicleAssignment (VehicleID, EmployeeID, StartDate)
                VALUES (@VID, @EID, @Start);
                SELECT SCOPE_IDENTITY();";
            SqlParameter[] param = {
                new SqlParameter("@VID",   vehicleID),
                new SqlParameter("@EID",   employeeID),
                new SqlParameter("@Start", startDate)
            };
            object result = ExecuteScalar(query, param);
            int newID = result != null ? Convert.ToInt32(result) : 0;
            AuditLogDAL.LogAction("VehicleAssignment", "INSERT", newID, performedBy);
            return newID;
        }

        public int EndAssignment(int assignmentID, DateTime endDate, string performedBy)
        {
            string query = "UPDATE VehicleAssignment SET EndDate = @End WHERE AssignmentID = @ID";
            SqlParameter[] param = { new SqlParameter("@ID", assignmentID), new SqlParameter("@End", endDate) };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("VehicleAssignment", "END", assignmentID, performedBy);
            return rows;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // VEHICLE COST DAL  — records fuel + maintenance costs per month
    // These costs are deducted from employee payroll automatically
    // ═══════════════════════════════════════════════════════════════════════
    public class VehicleCostDAL : BaseDAL
    {
        public DataTable GetByAssignment(int assignmentID)
        {
            string query = @"
                SELECT CostID, FuelCost, MaintenanceCost,
                       (FuelCost + MaintenanceCost) AS TotalCost,
                       CostMonth, CostYear
                FROM VehicleCost WHERE AssignmentID = @AID
                ORDER BY CostYear DESC, CostMonth DESC";
            SqlParameter[] param = { new SqlParameter("@AID", assignmentID) };
            return GetDataTable(query, param);
        }

        public DataTable GetMonthlyReport(int month, int year)
        {
            string query = @"
                SELECT e.FullName, v.RegistrationNumber,
                       vc.FuelCost, vc.MaintenanceCost,
                       (vc.FuelCost + vc.MaintenanceCost) AS TotalCost
                FROM VehicleCost vc
                INNER JOIN VehicleAssignment va ON vc.AssignmentID = va.AssignmentID
                INNER JOIN Vehicle v            ON va.VehicleID    = v.VehicleID
                INNER JOIN Employee e           ON va.EmployeeID   = e.EmployeeID
                WHERE vc.CostMonth = @Month AND vc.CostYear = @Year
                ORDER BY e.FullName";
            SqlParameter[] param = { new SqlParameter("@Month", month), new SqlParameter("@Year", year) };
            return GetDataTable(query, param);
        }

        public int Insert(int assignmentID, decimal fuel, decimal maintenance, int month, int year, string performedBy)
        {
            string query = @"
                INSERT INTO VehicleCost (AssignmentID, FuelCost, MaintenanceCost, CostMonth, CostYear)
                VALUES (@AID, @Fuel, @Maint, @Month, @Year);
                SELECT SCOPE_IDENTITY();";
            SqlParameter[] param = {
                new SqlParameter("@AID",   assignmentID),
                new SqlParameter("@Fuel",  fuel),
                new SqlParameter("@Maint", maintenance),
                new SqlParameter("@Month", month),
                new SqlParameter("@Year",  year)
            };
            object result = ExecuteScalar(query, param);
            int newID = result != null ? Convert.ToInt32(result) : 0;
            AuditLogDAL.LogAction("VehicleCost", "INSERT", newID, performedBy);
            return newID;
        }

        public int Update(int costID, decimal fuel, decimal maintenance, string performedBy)
        {
            string query = "UPDATE VehicleCost SET FuelCost = @Fuel, MaintenanceCost = @Maint WHERE CostID = @ID";
            SqlParameter[] param = {
                new SqlParameter("@ID",    costID),
                new SqlParameter("@Fuel",  fuel),
                new SqlParameter("@Maint", maintenance)
            };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("VehicleCost", "UPDATE", costID, performedBy);
            return rows;
        }

        public int Delete(int costID, string performedBy)
        {
            string query = "DELETE FROM VehicleCost WHERE CostID = @ID";
            SqlParameter[] param = { new SqlParameter("@ID", costID) };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("VehicleCost", "DELETE", costID, performedBy);
            return rows;
        }
    }
}
