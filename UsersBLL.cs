using System;
using System.Data;
using HRMS_ERP.DataAccess;

namespace HRMS_ERP.BusinessLogic
{
    /// <summary>
    /// Users BLL
    /// Manages system user accounts — create, update, change password, delete
    /// Login validation is handled directly in frmLogin via UsersDAL
    /// </summary>
    public class UsersBLL
    {
        private readonly UsersDAL _dal = new UsersDAL();

        // ─── GET all users for the user management grid ───────────────────
        public DataTable GetAll()
        {
            return _dal.GetAll();
        }

        // ─── CREATE new user account ───────────────────────────────────────
        // Returns: new UserID on success, -1 if username duplicate
        public int CreateUser(string username, string password, string role,
                              int? employeeID, string performedBy)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.");

            if (password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters.");

            string[] validRoles = { "Admin", "HR", "Finance" };
            bool validRole = false;
            foreach (string r in validRoles) if (r == role) validRole = true;
            if (!validRole)
                throw new ArgumentException("Role must be Admin, HR, or Finance.");

            if (_dal.IsUsernameDuplicate(username))
                return -1; // username already taken

            return _dal.Insert(username, password, role, employeeID, performedBy);
        }

        // ─── UPDATE user role or linked employee ───────────────────────────
        public int UpdateUser(int userID, string username, string role,
                              int? employeeID, string performedBy)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.");

            if (_dal.IsUsernameDuplicate(username, userID))
                return -1; // duplicate with another user

            return _dal.Update(userID, username, role, employeeID, performedBy);
        }

        // ─── CHANGE PASSWORD ───────────────────────────────────────────────
        public int ChangePassword(int userID, string newPassword, string confirmPassword, string performedBy)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("New password is required.");

            if (newPassword.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters.");

            if (newPassword != confirmPassword)
                throw new ArgumentException("New password and confirm password do not match.");

            return _dal.ChangePassword(userID, newPassword, performedBy);
        }

        // ─── DELETE user ───────────────────────────────────────────────────
        public int DeleteUser(int userID, string performedBy)
        {
            // Cannot delete yourself
            if (userID == CurrentUser.UserID)
                throw new InvalidOperationException("You cannot delete your own account.");

            return _dal.Delete(userID, performedBy);
        }
    }
}
