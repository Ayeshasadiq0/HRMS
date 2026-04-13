using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HRMS_ERP.DataAccess;
using HRMS_ERP.Forms;

namespace HRMS_ERP
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Make password box hide characters (shows *** while typing)
            textBox2.PasswordChar = '*';

            // Allow pressing ENTER to trigger login
            this.AcceptButton = loginbtn;

            // Window title
           // this.Text = "HRMS_ERP — Login";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void loginbtn_Click(object sender, EventArgs e)
        {
            // ── Step 1: Basic validation ──────────────────────────────────
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter your username.",
                    "Username Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter your password.",
                    "Password Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            // ── Step 2: Show loading state ────────────────────────────────
            loginbtn.Enabled = false;
            loginbtn.Text = "Logging in...";

            try
            {
                // ── Step 3: Call UsersDAL to validate ─────────────────────
                // UsersDAL hashes the password in C# first, then checks DB
                UsersDAL dal = new UsersDAL();
                DataRow user = dal.ValidateLogin(textBox1.Text.Trim(), textBox2.Text);

                // ── Step 4: Check result ──────────────────────────────────
                if (user == null)
                {
                    // Wrong username or password
                    MessageBox.Show("Incorrect username or password.\nPlease try again.",
                        "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    textBox2.Clear();
                    textBox2.Focus();
                    return;
                }

                // ── Step 5: Login succeeded — fill CurrentUser session ────
                CurrentUser.SetUser(
                    Convert.ToInt32(user["UserID"]),
                    user["Username"].ToString(),
                    user["Role"].ToString(),
                    user["EmployeeID"] == DBNull.Value
                        ? (int?)null
                        : Convert.ToInt32(user["EmployeeID"]),
                    user["FullName"].ToString()
                );

                // ── Step 6: Log the login action in audit trail ───────────
                AuditLogDAL.LogAction("Users", "LOGIN", CurrentUser.UserID, CurrentUser.Username);

                // ── Step 7: Open Main form and hide login ─────────────────
                Main mainForm = new Main();
                mainForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during login:\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Always restore the button whether login passed or failed
                loginbtn.Enabled = true;
                loginbtn.Text = "Login";
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
