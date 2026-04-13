using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using HRMS_ERP.BusinessLogic;
using HRMS_ERP.DataAccess;
using HRMS_ERP.Models;

namespace HRMS_ERP.Forms
{
    public partial class AddEmployee : Form
    {
        private readonly EmployeeBLL _bll = new EmployeeBLL();
        private readonly EmployeeDAL _dal = new EmployeeDAL();

        private readonly bool _isEditMode;
        private readonly int _employeeID;

        private ToolTip _tip = new ToolTip();

        public AddEmployee()
        {
            InitializeComponent();
            _isEditMode = false;
            _employeeID = 0;
        }

        public AddEmployee(int employeeID)
        {
            InitializeComponent();
            _isEditMode = true;
            _employeeID = employeeID;
        }

        // ─────────────────────────────────────────────────────────────
        // FORM LOAD
        // ─────────────────────────────────────────────────────────────
        private void AddEmployee_Load(object sender, EventArgs e)
        {
            SetupLabels();
            SetupValidation();

            if (_isEditMode)
            {
                this.Text = "Edit Employee";
                groupBox1.Text = "Edit Employee Information";
                button1.Text = "Update";
                button1.BackColor = Color.FromArgb(0, 120, 212);

                txtEmployeeCode.ReadOnly = true;
                txtEmployeeCode.BackColor = Color.FromArgb(240, 240, 240);

                LoadEmployeeData(_employeeID);
            }
            else
            {
                this.Text = "Add New Employee";
                groupBox1.Text = "New Employee Information";
                button1.Text = "Save";
                button1.BackColor = Color.Green;

                checkBox1.Checked = true;
                dateTimePicker1.Value = DateTime.Today;
                txtEmployeeCode.Text = "EMP-" + DateTime.Today.ToString("yyyyMM") + "-" + new Random().Next(100, 999);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // SETUP LABELS & TOOLTIPS
        // ─────────────────────────────────────────────────────────────
        private void SetupLabels()
        {
            FirstName.Text = "Full Name *";
            LastName.Text = "Employee Code *";
            Email.Text = "CNIC *  (format: 12345-1234567-1)";
            department.Text = "Department *";
            label4.Text = "Joining Date *";
            label2.Text = "Designation";
            label1.Text = "Phone";
            label3.Text = "Bank Account Number";
            label5.Text = "Bank Name";
            label6.Text = "Reporting Manager ID (optional)";
            checkBox1.Text = "Active Employee";

            _tip.SetToolTip(txtFirstName, "e.g. Ahmed Khan");
            _tip.SetToolTip(txtcnic, "e.g. 42101-1234567-1");
            _tip.SetToolTip(txtPhone, "e.g. 0300-1234567");
            _tip.SetToolTip(Designation, "e.g. Software Engineer");
            _tip.SetToolTip(Bank_Account_number, "e.g. 1234567890");
            _tip.SetToolTip(Bank_Name, "e.g. HBL, UBL, Meezan");
            _tip.SetToolTip(Reporting_Manager, "Enter manager Employee ID (number)");
            _tip.SetToolTip(txtDepartment, "e.g. HR, Finance, IT, Operations");
        }

        private void SetupValidation()
        {
            txtcnic.KeyPress += delegate (object s, KeyPressEventArgs ev)
            {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != '-' && ev.KeyChar != '\b')
                    ev.Handled = true;
            };

            // Only digits in Reporting Manager field
            Reporting_Manager.KeyPress += delegate (object s, KeyPressEventArgs ev)
            {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != '\b')
                    ev.Handled = true;
            };
        }

        // ─────────────────────────────────────────────────────────────
        // LOAD EXISTING DATA (Edit mode)
        // ─────────────────────────────────────────────────────────────
        private void LoadEmployeeData(int employeeID)
        {
            try
            {
                DataTable dt = _dal.GetByID(employeeID);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Employee not found.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                DataRow row = dt.Rows[0];

                // ── Map each textbox to its correct DB column ──
                txtFirstName.Text = ColStr(row, "FullName");
                txtEmployeeCode.Text = ColStr(row, "EmployeeCode");
                txtcnic.Text = ColStr(row, "CNIC");
                txtPhone.Text = ColStr(row, "ContactNumber");
                txtDepartment.Text = ColStr(row, "Department");
                Designation.Text = ColStr(row, "Designation");
                Bank_Account_number.Text = ColStr(row, "BankAccountNumber");
                Bank_Name.Text = ColStr(row, "BankName");

                if (row["ReportingManagerID"] != DBNull.Value)
                    Reporting_Manager.Text = row["ReportingManagerID"].ToString();

                if (row["JoiningDate"] != DBNull.Value)
                    dateTimePicker1.Value = Convert.ToDateTime(row["JoiningDate"]);

                checkBox1.Checked = row["IsActive"] != DBNull.Value &&
                                    Convert.ToBoolean(row["IsActive"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employee:\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // VALIDATION
        // ─────────────────────────────────────────────────────────────
        private bool ValidateAll()
        {
            bool ok = true;

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            { SetInvalid(txtFirstName, "Full Name is required"); ok = false; }
            else SetValid(txtFirstName);

            if (string.IsNullOrWhiteSpace(txtEmployeeCode.Text))
            { SetInvalid(txtEmployeeCode, "Employee Code is required"); ok = false; }
            else SetValid(txtEmployeeCode);

            if (string.IsNullOrWhiteSpace(txtcnic.Text))
            { SetInvalid(txtcnic, "CNIC is required"); ok = false; }
            else if (!EmployeeBLL.IsValidCNIC(txtcnic.Text.Trim()))
            { SetInvalid(txtcnic, "Format must be: 12345-1234567-1"); ok = false; }
            else SetValid(txtcnic);

            if (string.IsNullOrWhiteSpace(txtDepartment.Text))
            { txtDepartment.BackColor = Color.FromArgb(255, 230, 230); ok = false; }
            else txtDepartment.BackColor = Color.White;

            if (dateTimePicker1.Value.Date > DateTime.Today)
                ok = false;

            if (!ok)
                MessageBox.Show("Please fill in all required fields correctly.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return ok;
        }

        private void SetInvalid(TextBox tb, string msg)
        {
            tb.BackColor = Color.FromArgb(255, 230, 230);
            _tip.SetToolTip(tb, msg);
        }

        private void SetValid(TextBox tb)
        {
            tb.BackColor = Color.White;
            _tip.SetToolTip(tb, "");
        }

        // ─────────────────────────────────────────────────────────────
        // SAVE / UPDATE
        // ─────────────────────────────────────────────────────────────
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateAll()) return;

            int? managerID = null;
            int mgr;
            if (int.TryParse(Reporting_Manager.Text.Trim(), out mgr) && mgr > 0)
                managerID = mgr;

            Employee emp = new Employee();
            emp.EmployeeID = _employeeID;
            emp.EmployeeCode = txtEmployeeCode.Text.Trim().ToUpper();
            emp.FullName = txtFirstName.Text.Trim();
            emp.CNIC = txtcnic.Text.Trim();
            emp.ContactNumber = txtPhone.Text.Trim();
            emp.Department = txtDepartment.Text.Trim();
            emp.Designation = Designation.Text.Trim();           // correct
            emp.BankAccountNumber = Bank_Account_number.Text.Trim();   // correct
            emp.BankName = Bank_Name.Text.Trim();             // correct
            emp.ReportingManagerID = managerID;
            emp.JoiningDate = dateTimePicker1.Value.Date;
            emp.IsActive = checkBox1.Checked;

            // Fixed shift: 9 AM to 5 PM for every new employee
            emp.ShiftStartTime = new TimeSpan(9, 0, 0);
            emp.ShiftEndTime = new TimeSpan(17, 0, 0);

            button1.Enabled = false;
            button1.Text = _isEditMode ? "Updating..." : "Saving...";

            try
            {
                if (_isEditMode)
                {
                    int result = _bll.UpdateEmployee(emp, CurrentUser.AuditName);

                    if (result == -1)
                    {
                        MessageBox.Show("This CNIC is already registered to another employee.",
                            "Duplicate CNIC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        SetInvalid(txtcnic, "Already exists");
                        return;
                    }
                    if (result == -2)
                    {
                        MessageBox.Show("This Employee Code is already in use.",
                            "Duplicate Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        SetInvalid(txtEmployeeCode, "Already exists");
                        return;
                    }
                }
                else
                {
                    int newID = _bll.AddEmployee(emp, CurrentUser.AuditName);

                    if (newID == -1)
                    {
                        MessageBox.Show("This CNIC is already registered in the system.",
                            "Duplicate CNIC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        SetInvalid(txtcnic, "Already exists");
                        return;
                    }
                    if (newID == -2)
                    {
                        MessageBox.Show("This Employee Code is already in use.",
                            "Duplicate Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        SetInvalid(txtEmployeeCode, "Already exists");
                        return;
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving:\n\n" + ex.Message, "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button1.Enabled = true;
                button1.Text = _isEditMode ? "Update" : "Save";
            }
        }

        // ─────────────────────────────────────────────────────────────
        // CANCEL
        // ─────────────────────────────────────────────────────────────
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // ─────────────────────────────────────────────────────────────
        // HELPERS
        // ─────────────────────────────────────────────────────────────
        private string ColStr(DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col)) return "";
            if (row[col] == DBNull.Value) return "";
            return row[col].ToString();
        }

        // Designer stubs
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }
    }
}