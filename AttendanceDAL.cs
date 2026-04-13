using System;
using System.Data;
using System.Data.SqlClient;
using HRMS_ERP.Models;

namespace HRMS_ERP.DataAccess
{
    public class AttendanceDAL : BaseDAL
    {
        // ─── GET all attendance records for a specific date ────────────────
        // Used in the attendance grid when HR selects a date
        public DataTable GetByDate(DateTime date)
        {
            string query = @"
                SELECT a.AttendanceID, a.EmployeeID, e.FullName, e.Department,
                       a.AttendanceDate, a.CheckInTime, a.CheckOutTime,
                       a.WorkingHours, a.OvertimeHours, a.AttendanceStatus
                FROM Attendance a
                INNER JOIN Employee e ON a.EmployeeID = e.EmployeeID
                WHERE a.AttendanceDate = @Date
                ORDER BY e.FullName";
            SqlParameter[] param = { new SqlParameter("@Date", date.Date) };
            return GetDataTable(query, param);
        }

        // ─── GET attendance for one employee for a whole month ────────────
        public DataTable GetByEmployeeMonth(int employeeID, int month, int year)
        {
            string query = @"
                SELECT AttendanceID, AttendanceDate, CheckInTime, CheckOutTime,
                       WorkingHours, OvertimeHours, AttendanceStatus
                FROM Attendance
                WHERE EmployeeID = @EID
                  AND MONTH(AttendanceDate) = @Month
                  AND YEAR(AttendanceDate)  = @Year
                ORDER BY AttendanceDate";
            SqlParameter[] param = {
                new SqlParameter("@EID",   employeeID),
                new SqlParameter("@Month", month),
                new SqlParameter("@Year",  year)
            };
            return GetDataTable(query, param);
        }

        // ─── CHECK if a record already exists for employee + date ─────────
        public bool Exists(int employeeID, DateTime date)
        {
            string query = "SELECT COUNT(*) FROM Attendance WHERE EmployeeID = @EID AND AttendanceDate = @Date";
            SqlParameter[] param = {
                new SqlParameter("@EID",  employeeID),
                new SqlParameter("@Date", date.Date)
            };
            object result = ExecuteScalar(query, param);
            return result != null && Convert.ToInt32(result) > 0;
        }

        // ─── INSERT a new attendance record ───────────────────────────────
        public int Insert(Attendance att, string performedBy)
        {
            string query = @"
                INSERT INTO Attendance
                    (EmployeeID, AttendanceDate, CheckInTime, CheckOutTime,
                     WorkingHours, OvertimeHours, AttendanceStatus)
                VALUES
                    (@EID, @Date, @In, @Out, @Hours, @OT, @Status)";
            SqlParameter[] param = {
                new SqlParameter("@EID",    att.EmployeeID),
                new SqlParameter("@Date",   att.AttendanceDate.Date),
                new SqlParameter("@In",     (object)att.CheckInTime    ?? DBNull.Value),
                new SqlParameter("@Out",    (object)att.CheckOutTime   ?? DBNull.Value),
                new SqlParameter("@Hours",  (object)att.WorkingHours   ?? DBNull.Value),
                new SqlParameter("@OT",     (object)att.OvertimeHours  ?? DBNull.Value),
                new SqlParameter("@Status", att.AttendanceStatus)
            };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("Attendance", "INSERT", att.EmployeeID, performedBy);
            return rows;
        }

        // ─── UPDATE (e.g. add check-out time, fix status) ─────────────────
        public int Update(Attendance att, string performedBy)
        {
            string query = @"
                UPDATE Attendance SET
                    CheckInTime      = @In,
                    CheckOutTime     = @Out,
                    WorkingHours     = @Hours,
                    OvertimeHours    = @OT,
                    AttendanceStatus = @Status
                WHERE AttendanceID = @ID";
            SqlParameter[] param = {
                new SqlParameter("@ID",     att.AttendanceID),
                new SqlParameter("@In",     (object)att.CheckInTime   ?? DBNull.Value),
                new SqlParameter("@Out",    (object)att.CheckOutTime  ?? DBNull.Value),
                new SqlParameter("@Hours",  (object)att.WorkingHours  ?? DBNull.Value),
                new SqlParameter("@OT",     (object)att.OvertimeHours ?? DBNull.Value),
                new SqlParameter("@Status", att.AttendanceStatus)
            };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("Attendance", "UPDATE", att.AttendanceID, performedBy);
            return rows;
        }

        // ─── DELETE attendance record ──────────────────────────────────────
        public int Delete(int attendanceID, string performedBy)
        {
            string query = "DELETE FROM Attendance WHERE AttendanceID = @ID";
            SqlParameter[] param = { new SqlParameter("@ID", attendanceID) };
            int rows = ExecuteNonQuery(query, param);
            AuditLogDAL.LogAction("Attendance", "DELETE", attendanceID, performedBy);
            return rows;
        }

        // ─── MARK LEAVE for a date (CL / SL / AL / LWP) ──────────────────
        // If record exists, updates it. If not, creates it.
        public int MarkLeave(int employeeID, DateTime date, string leaveType, string performedBy)
        {
            if (Exists(employeeID, date))
            {
                string upd = @"UPDATE Attendance SET AttendanceStatus = @Status,
                    CheckInTime = NULL, CheckOutTime = NULL, WorkingHours = 0, OvertimeHours = 0
                    WHERE EmployeeID = @EID AND AttendanceDate = @Date";
                SqlParameter[] up = {
                    new SqlParameter("@EID",    employeeID),
                    new SqlParameter("@Date",   date.Date),
                    new SqlParameter("@Status", leaveType)
                };
                return ExecuteNonQuery(upd, up);
            }
            else
            {
                Attendance att = new Attendance {
                    EmployeeID       = employeeID,
                    AttendanceDate   = date,
                    AttendanceStatus = leaveType,
                    WorkingHours     = 0,
                    OvertimeHours    = 0
                };
                return Insert(att, performedBy);
            }
        }

        // ─── AUTO-CALCULATE working hours and overtime ────────────────────
        // Call this after setting CheckIn and CheckOut
        // Standard working hours per day = 8
        public static decimal CalcWorkingHours(TimeSpan checkIn, TimeSpan checkOut)
        {
            double total = (checkOut - checkIn).TotalHours;
            return (decimal)Math.Max(0, total);
        }

        public static decimal CalcOvertimeHours(TimeSpan checkIn, TimeSpan checkOut, double standardHours = 8.0)
        {
            double total = (checkOut - checkIn).TotalHours;
            return (decimal)Math.Max(0, total - standardHours);
        }

        // ─── AUTO STATUS: Present or Late based on shift start ────────────
        // Grace period: 15 minutes after shift start = Late
        public static string DetermineStatus(TimeSpan checkIn, TimeSpan shiftStart, int gracePeriodMinutes = 15)
        {
            return checkIn > shiftStart.Add(TimeSpan.FromMinutes(gracePeriodMinutes)) ? "Late" : "Present";
        }
    }
}
