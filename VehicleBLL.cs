using System;
using System.Data;
using HRMS_ERP.DataAccess;
using HRMS_ERP.Models;

namespace HRMS_ERP.BusinessLogic
{
    /// <summary>
    /// Vehicle Business Logic Layer
    /// Manages vehicle registration, assignment to employees, and monthly costs
    /// </summary>
    public class VehicleBLL
    {
        private readonly VehicleDAL           _vehicleDal    = new VehicleDAL();
        private readonly VehicleAssignmentDAL _assignmentDal = new VehicleAssignmentDAL();
        private readonly VehicleCostDAL       _costDal       = new VehicleCostDAL();

        // ── VEHICLE ────────────────────────────────────────────────────────

        public DataTable GetAllVehicles()
        {
            return _vehicleDal.GetAll();
        }

        public DataTable GetActiveVehicles()
        {
            return _vehicleDal.GetActive();
        }

        public int AddVehicle(Vehicle v, string performedBy)
        {
            if (string.IsNullOrWhiteSpace(v.VehicleType))
                throw new ArgumentException("Vehicle Type is required.");

            if (string.IsNullOrWhiteSpace(v.RegistrationNumber))
                throw new ArgumentException("Registration Number is required.");

            if (_vehicleDal.IsRegNumberDuplicate(v.RegistrationNumber))
                return -1; // duplicate registration number

            return _vehicleDal.Insert(v, performedBy);
        }

        public int UpdateVehicle(Vehicle v, string performedBy)
        {
            if (string.IsNullOrWhiteSpace(v.RegistrationNumber))
                throw new ArgumentException("Registration Number is required.");

            if (_vehicleDal.IsRegNumberDuplicate(v.RegistrationNumber, v.VehicleID))
                return -1; // duplicate

            return _vehicleDal.Update(v, performedBy);
        }

        // ── ASSIGNMENT ─────────────────────────────────────────────────────

        public DataTable GetActiveAssignments()
        {
            return _assignmentDal.GetActive();
        }

        public DataTable GetAllAssignments()
        {
            return _assignmentDal.GetAll();
        }

        public int AssignVehicle(int vehicleID, int employeeID, DateTime startDate, string performedBy)
        {
            if (vehicleID <= 0)
                throw new ArgumentException("Please select a vehicle.");

            if (employeeID <= 0)
                throw new ArgumentException("Please select an employee.");

            if (startDate > DateTime.Today.AddDays(1))
                throw new ArgumentException("Start date cannot be far in the future.");

            return _assignmentDal.Assign(vehicleID, employeeID, startDate, performedBy);
        }

        public int EndAssignment(int assignmentID, DateTime endDate, string performedBy)
        {
            if (endDate > DateTime.Today)
                throw new ArgumentException("End date cannot be in the future.");

            return _assignmentDal.EndAssignment(assignmentID, endDate, performedBy);
        }

        // ── COSTS ──────────────────────────────────────────────────────────

        public DataTable GetCostsByAssignment(int assignmentID)
        {
            return _costDal.GetByAssignment(assignmentID);
        }

        public DataTable GetMonthlyCostReport(int month, int year)
        {
            return _costDal.GetMonthlyReport(month, year);
        }

        public int AddCost(int assignmentID, decimal fuel, decimal maintenance,
                           int month, int year, string performedBy)
        {
            if (fuel < 0)
                throw new ArgumentException("Fuel cost cannot be negative.");

            if (maintenance < 0)
                throw new ArgumentException("Maintenance cost cannot be negative.");

            if (fuel == 0 && maintenance == 0)
                throw new ArgumentException("At least one cost (fuel or maintenance) must be greater than zero.");

            if (month < 1 || month > 12)
                throw new ArgumentException("Invalid month.");

            return _costDal.Insert(assignmentID, fuel, maintenance, month, year, performedBy);
        }

        public int UpdateCost(int costID, decimal fuel, decimal maintenance, string performedBy)
        {
            if (fuel < 0 || maintenance < 0)
                throw new ArgumentException("Cost values cannot be negative.");

            return _costDal.Update(costID, fuel, maintenance, performedBy);
        }

        public int DeleteCost(int costID, string performedBy)
        {
            return _costDal.Delete(costID, performedBy);
        }
    }
}
