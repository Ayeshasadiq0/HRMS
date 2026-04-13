namespace HRMS_ERP
{
    /// <summary>
    /// Holds the currently logged-in user's session data.
    /// Set once after successful login. Cleared on logout.
    /// Used by every form to know WHO is logged in and WHAT they can do.
    ///
    /// Usage anywhere in the project:
    ///   CurrentUser.Username   → "admin"
    ///   CurrentUser.Role       → "Admin" / "HR" / "Finance"
    ///   CurrentUser.IsAdmin    → true/false
    ///   CurrentUser.AuditName  → pass this to every DAL call for audit logging
    /// </summary>
    public static class CurrentUser
    {
        public static int     UserID     { get; set; }
        public static string  Username   { get; set; }
        public static string  Role       { get; set; }
        public static int?    EmployeeID { get; set; }
        public static string  FullName   { get; set; }

        // ─── Role helpers — used to show/hide menu items ──────────────────
        public static bool IsAdmin   { get { return Role == "Admin"; } }
        public static bool IsHR      { get { return Role == "HR"      || Role == "Admin"; } }
        public static bool IsFinance { get { return Role == "Finance"  || Role == "Admin"; } }

        // ─── Set after successful login ───────────────────────────────────
        public static void SetUser(int userID, string username, string role, int? employeeID, string fullName)
        {
            UserID     = userID;
            Username   = username;
            Role       = role;
            EmployeeID = employeeID;
            FullName   = fullName;
        }

        // ─── Clear on logout ──────────────────────────────────────────────
        public static void Clear()
        {
            UserID     = 0;
            Username   = null;
            Role       = null;
            EmployeeID = null;
            FullName   = null;
        }

        // ─── Use this in every DAL call: dal.Insert(emp, CurrentUser.AuditName)
        public static string AuditName
        {
            get { return string.IsNullOrEmpty(Username) ? "System" : Username; }
        }
    }
}
