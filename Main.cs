using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HRMS_ERP.BusinessLogic;
using HRMS_ERP.DataAccess;
using HRMS_ERP.Models;
using System.IO.Compression;

namespace HRMS_ERP.Forms
{
    public partial class Main : Form
    {
        // ── BLL instances (one per tab) ─────────────────────────────────────
        private readonly EmployeeBLL _empBLL = new EmployeeBLL();
        private readonly AttendanceBLL _attBLL = new AttendanceBLL();
        private readonly LeaveBLL _leaveBLL = new LeaveBLL();
        private readonly SalaryBLL _salaryBLL = new SalaryBLL();
        private readonly TaxBLL _taxBLL = new TaxBLL();
        private readonly PayrollBLL _payrollBLL = new PayrollBLL();
        private readonly VehicleBLL _vehicleBLL = new VehicleBLL();
        private readonly EmployeeDAL _empDal = new EmployeeDAL();

        // ── Employee Master inline-edit state ──────────────────────────────
        private int _editingRowIndex = -1;
        private int _editingEmployeeID = 0;
        private bool _suppressFilter = false;

        // ── Salary tab inline-edit state ───────────────────────────────────
        private int _editingSalaryStructureID = 0;
        private bool _salaryEditMode = false;

        // ── Tax slab inline-edit state ─────────────────────────────────────
        private int _editingSlabID = 0;
        private bool _slabEditMode = false;

        public Main()
        {
            InitializeComponent();
        }

        // ════════════════════════════════════════════════════════════════════
        // FORM LOAD
        // ════════════════════════════════════════════════════════════════════
        private void Main_Load(object sender, EventArgs e)
        {
            // Tab 1 – Employee Master
            ConfigureEmployeeGrid();
            LoadDepartmentFilter();
            LoadEmployees();

            // Tab 2 – Payroll
            InitPayrollTab();

            // Tab 3 – Taxation
            InitTaxTab();

            // Tab 5 – Attendance
            InitAttendanceTab();

            // Tab 6 – Salary Structure
            InitSalaryTab();

            // Role-based tab visibility
            ApplyRoleVisibility();
        }

        private void ApplyRoleVisibility()
        {
            // Finance-only tabs: Payroll (tabPage2), Taxation (tabPage3), Vehicle (tabPage4)
            // HR-only tabs: Attendance (tabpage5)
            // Shared: Employee (tabPage1), Salary (tabPage6)
            if (!CurrentUser.IsAdmin)
            {
                if (!CurrentUser.IsFinance)
                {
                    HR_Management_module.TabPages.Remove(tabPage2);
                    HR_Management_module.TabPages.Remove(tabPage3);
                    HR_Management_module.TabPages.Remove(tabPage4);
                }
                if (!CurrentUser.IsHR)
                {
                    HR_Management_module.TabPages.Remove(tabpage5);
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════
        // ── TAB 1: EMPLOYEE MASTER ──────────────────────────────────────────
        // ════════════════════════════════════════════════════════════════════
        private void ConfigureEmployeeGrid()
        {
            EmployeeGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 212);
            EmployeeGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            EmployeeGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            EmployeeGrid.EnableHeadersVisualStyles = false;
            EmployeeGrid.RowTemplate.Height = 28;

            EmployeeGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 228, 247);
            EmployeeGrid.DefaultCellStyle.SelectionForeColor = Color.Black;

            Edit.DefaultCellStyle.BackColor = Color.FromArgb(0, 120, 212);
            Edit.DefaultCellStyle.ForeColor = Color.White;
            Edit.FlatStyle = FlatStyle.Flat;
            Edit.ReadOnly = false;

            Delete.DefaultCellStyle.BackColor = Color.FromArgb(168, 0, 0);
            Delete.DefaultCellStyle.ForeColor = Color.White;
            Delete.FlatStyle = FlatStyle.Flat;
            Delete.Text = "Delete";
            Delete.UseColumnTextForButtonValue = true;
            Delete.ReadOnly = false;

            EmployeeGrid.CellContentClick -= EmployeeGrid_CellContentClick;
            EmployeeGrid.CellContentClick += EmployeeGrid_CellContentClick;
            EmployeeGrid.KeyDown += EmployeeGrid_KeyDown;
        }

        private void LoadDepartmentFilter()
        {
            try
            {
                _suppressFilter = true;
                cmbDepartment.Items.Clear();
                cmbDepartment.Items.Add("All Departments");
                DataTable depts = _empBLL.GetDepartments();
                foreach (DataRow r in depts.Rows)
                {
                    string d = r["Department"] == DBNull.Value ? "" : r["Department"].ToString();
                    if (!string.IsNullOrWhiteSpace(d))
                        cmbDepartment.Items.Add(d);
                }
                cmbDepartment.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Dept filter error: " + ex.Message);
            }
            finally { _suppressFilter = false; }
        }

        private void LoadEmployees(string keyword = "", string department = "")
        {
            try
            {
                DataTable dt = (string.IsNullOrWhiteSpace(keyword) && string.IsNullOrWhiteSpace(department))
                    ? _empBLL.GetAll()
                    : _empBLL.Search(keyword, department);
                BindEmployeeGrid(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employees:\n\n" + ex.Message,
                    "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindEmployeeGrid(DataTable dt)
        {
            EmployeeGrid.Rows.Clear();
            foreach (DataRow row in dt.Rows)
            {
                int idx = EmployeeGrid.Rows.Add();
                DataGridViewRow gr = EmployeeGrid.Rows[idx];
                gr.Cells["EmployeeID"].Value = row["EmployeeID"];
                gr.Cells["EmployeeCode"].Value = SafeStr(row, "EmployeeCode");
                gr.Cells["FullName"].Value = SafeStr(row, "FullName");
                gr.Cells["Department"].Value = SafeStr(row, "Department");
                gr.Cells["Designation"].Value = SafeStr(row, "Designation");
                gr.Cells["CNIC"].Value = SafeStr(row, "CNIC");
                gr.Cells["Bank_Account"].Value = SafeStr(row, "BankAccountNumber");
                gr.Cells["Bank_Name"].Value = SafeStr(row, "BankName");
                gr.Cells["Contact"].Value = SafeStr(row, "ContactNumber");
                gr.Cells["Reporting_Manager"].Value = SafeStr(row, "ReportingManager");
                gr.Cells["Shift_start"].Value = SafeStr(row, "ShiftStartTime");
                gr.Cells["Shift_End"].Value = SafeStr(row, "ShiftEndTime");

                if (dt.Columns.Contains("JoiningDate") && row["JoiningDate"] != DBNull.Value)
                {
                    DateTime jd;
                    if (DateTime.TryParse(row["JoiningDate"].ToString(), out jd))
                        gr.Cells["JoiningDate"].Value = jd.ToString("yyyy-MM-dd");
                }

                bool active = dt.Columns.Contains("IsActive") && row["IsActive"] != DBNull.Value
                    && Convert.ToBoolean(row["IsActive"]);
                gr.Cells["Is_Active"].Value = active;

                if (!active)
                { gr.DefaultCellStyle.ForeColor = Color.Gray; gr.DefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Italic); }
                else
                { gr.DefaultCellStyle.ForeColor = Color.Black; gr.DefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Regular); }
            }
        }

        private string SafeStr(DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col)) return "";
            if (row[col] == DBNull.Value) return "";
            return row[col].ToString();
        }

        private string GetSelectedDepartment()
        {
            string text = cmbDepartment.Text == null ? "" : cmbDepartment.Text.Trim();
            return (string.IsNullOrWhiteSpace(text) || text == "All Departments") ? "" : text;
        }

        // ── Employee Master events ──────────────────────────────────────────
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (_suppressFilter) return;
            LoadEmployees(txtSearch.Text.Trim(), GetSelectedDepartment());
        }

        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressFilter) return;
            LoadEmployees(txtSearch.Text.Trim(), GetSelectedDepartment());
        }

        private void cmbDepartment_TextChanged(object sender, EventArgs e)
        {
            if (_suppressFilter) return;
            LoadEmployees(txtSearch.Text.Trim(), GetSelectedDepartment());
        }

        // ── Toolbar buttons ─────────────────────────────────────────────────
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            AddEmployee addForm = new AddEmployee();
            addForm.FormClosed += delegate { LoadDepartmentFilter(); LoadEmployees(txtSearch.Text.Trim(), GetSelectedDepartment()); };
            addForm.ShowDialog(this);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            CancelInlineEdit();
            _suppressFilter = true;
            txtSearch.Clear();
            cmbDepartment.SelectedIndex = 0;
            _suppressFilter = false;
            LoadDepartmentFilter();
            LoadEmployees();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (EmployeeGrid.Rows.Count == 0)
            { MessageBox.Show("No data to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "Employees_" + DateTime.Today.ToString("yyyyMMdd") + ".csv"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Employee Code,Full Name,Department,Designation,CNIC,Bank Account,Bank Name,Contact,Joining Date,Active");
                foreach (DataGridViewRow row in EmployeeGrid.Rows)
                {
                    sb.AppendLine(string.Join(",", new[] {
                        Csv(row,"EmployeeCode"), Csv(row,"FullName"), Csv(row,"Department"),
                        Csv(row,"Designation"),  Csv(row,"CNIC"),     Csv(row,"Bank_Account"),
                        Csv(row,"Bank_Name"),    Csv(row,"Contact"),  Csv(row,"JoiningDate"),
                        (row.Cells["Is_Active"].Value != null && Convert.ToBoolean(row.Cells["Is_Active"].Value)) ? "Yes" : "No"
                    }));
                }
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Exported to:\n" + sfd.FileName, "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            { MessageBox.Show("Export failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private string Csv(DataGridViewRow row, string col)
        {
            object v = row.Cells[col].Value;
            if (v == null) return "";
            string s = v.ToString();
            return (s.Contains(",") || s.Contains("\"") || s.Contains("\n"))
                ? "\"" + s.Replace("\"", "\"\"") + "\""
                : s;
        }

        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (EmployeeGrid.SelectedRows.Count == 0)
            { MessageBox.Show("Please select a row first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            DataGridViewRow row = EmployeeGrid.SelectedRows[0];
            string name = row.Cells["FullName"].Value?.ToString() ?? "";
            int id = Convert.ToInt32(row.Cells["EmployeeID"].Value);
            DeleteEmployee(id, name, row);
        }

        // ── Grid cell click (Edit / Delete / Active toggle) ─────────────────
        private void EmployeeGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = EmployeeGrid.Rows[e.RowIndex];
            int empID = Convert.ToInt32(row.Cells["EmployeeID"].Value);
            string name = row.Cells["FullName"].Value?.ToString() ?? "";

            if (e.ColumnIndex == EmployeeGrid.Columns["Edit"].Index)
            {
                if (_editingRowIndex >= 0 && _editingRowIndex != e.RowIndex) CancelInlineEdit();
                StartInlineEdit(e.RowIndex, empID);
                return;
            }
            if (e.ColumnIndex == EmployeeGrid.Columns["Delete"].Index)
            { DeleteEmployee(empID, name, row); return; }

            if (e.ColumnIndex == EmployeeGrid.Columns["Is_Active"].Index)
            {
                bool currentlyActive = row.Cells["Is_Active"].Value != null && Convert.ToBoolean(row.Cells["Is_Active"].Value);
                string action = currentlyActive ? "Deactivate" : "Activate";
                if (MessageBox.Show(action + " employee:\n\n  " + name, "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                try
                {
                    if (currentlyActive)
                    { _empBLL.DeactivateEmployee(empID, CurrentUser.AuditName); row.Cells["Is_Active"].Value = false; row.DefaultCellStyle.ForeColor = Color.Gray; row.DefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Italic); }
                    else
                    { _empBLL.ReactivateEmployee(empID, CurrentUser.AuditName); row.Cells["Is_Active"].Value = true; row.DefaultCellStyle.ForeColor = Color.Black; row.DefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Regular); }
                }
                catch (Exception ex)
                { MessageBox.Show("Failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        // ── Inline edit ─────────────────────────────────────────────────────
        private void StartInlineEdit(int rowIndex, int employeeID)
        {
            _editingRowIndex = rowIndex;
            _editingEmployeeID = employeeID;
            EmployeeGrid.ReadOnly = false;
            DataGridViewRow row = EmployeeGrid.Rows[rowIndex];
            row.Cells["EmployeeID"].ReadOnly = true;
            row.Cells["EmployeeCode"].ReadOnly = true;
            row.Cells["JoiningDate"].ReadOnly = true;
            row.Cells["Edit"].ReadOnly = true;
            row.Cells["Delete"].ReadOnly = true;
            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 220);
            EmployeeGrid.CurrentCell = row.Cells["FullName"];
            EmployeeGrid.BeginEdit(true);
        }

        private void CommitInlineEdit()
        {
            if (_editingRowIndex < 0) return;
            EmployeeGrid.EndEdit();
            DataGridViewRow row = EmployeeGrid.Rows[_editingRowIndex];
            try
            {
                DataTable existing = _empBLL.GetByID(_editingEmployeeID);
                if (existing.Rows.Count == 0)
                { MessageBox.Show("Employee record not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                DataRow dbRow = existing.Rows[0];

                Employee emp = new Employee
                {
                    EmployeeID = _editingEmployeeID,
                    EmployeeCode = CellStr(row, "EmployeeCode"),
                    FullName = CellStr(row, "FullName").Trim(),
                    CNIC = CellStr(row, "CNIC").Trim(),
                    Department = CellStr(row, "Department"),
                    Designation = CellStr(row, "Designation"),
                    ContactNumber = CellStr(row, "Contact"),
                    BankAccountNumber = CellStr(row, "Bank_Account"),
                    BankName = CellStr(row, "Bank_Name"),
                    IsActive = row.Cells["Is_Active"].Value != null && Convert.ToBoolean(row.Cells["Is_Active"].Value)
                };
                int mgr;
                string mgrText = CellStr(row, "Reporting_Manager").Trim();
                emp.ReportingManagerID = (int.TryParse(mgrText, out mgr) && mgr > 0) ? (int?)mgr : null;
                emp.JoiningDate = GetDateFromCell(row.Cells["JoiningDate"].Value);
                if (dbRow["JoiningDate"] != DBNull.Value) emp.JoiningDate = Convert.ToDateTime(dbRow["JoiningDate"]);
                if (dbRow["ShiftStartTime"] != DBNull.Value) emp.ShiftStartTime = (TimeSpan)dbRow["ShiftStartTime"];
                if (dbRow["ShiftEndTime"] != DBNull.Value) emp.ShiftEndTime = (TimeSpan)dbRow["ShiftEndTime"];

                int result = _empBLL.UpdateEmployee(emp, CurrentUser.AuditName);
                if (result == -1)
                { MessageBox.Show("CNIC already exists for another employee.", "Duplicate CNIC", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (result == -2)
                { MessageBox.Show("Employee Code already in use.", "Duplicate Code", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                ResetRowReadOnly(row);
                LoadEmployees(txtSearch.Text.Trim(), GetSelectedDepartment());
            }
            catch (ArgumentException argEx)
            { MessageBox.Show(argEx.Message, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            catch (Exception ex)
            { MessageBox.Show("Update failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void CancelInlineEdit()
        {
            if (_editingRowIndex < 0) return;
            EmployeeGrid.CancelEdit();
            _editingRowIndex = -1;
            _editingEmployeeID = 0;
            EmployeeGrid.ReadOnly = true;
            LoadEmployees(txtSearch.Text.Trim(), GetSelectedDepartment());
        }

        private void ResetRowReadOnly(DataGridViewRow row)
        {
            row.DefaultCellStyle.BackColor = Color.Empty;
            row.DefaultCellStyle.Font = null;
            foreach (DataGridViewCell cell in row.Cells) cell.ReadOnly = true;
            EmployeeGrid.ReadOnly = true;
            _editingRowIndex = -1;
            _editingEmployeeID = 0;
        }

        private void EmployeeGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (_editingRowIndex < 0) return;
            if (e.KeyCode == Keys.Return)
            { e.Handled = true; e.SuppressKeyPress = true; CommitInlineEdit(); }
            else if (e.KeyCode == Keys.Escape)
            { e.Handled = true; CancelInlineEdit(); }
        }

        private void DeleteEmployee(int employeeID, string name, DataGridViewRow row)
        {
            if (MessageBox.Show("Permanently delete:\n\n  " + name + "\n\nThis cannot be undone.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            try
            { _empBLL.DeleteEmployee(employeeID, CurrentUser.AuditName); EmployeeGrid.Rows.Remove(row); }
            catch (Exception ex)
            { MessageBox.Show("Delete failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private string CellStr(DataGridViewRow row, string col) => row.Cells[col].Value?.ToString() ?? "";
        private DateTime GetDateFromCell(object val)
        {
            DateTime r;
            return (val != null && DateTime.TryParse(val.ToString(), out r)) ? r : DateTime.Today;
        }

        // ════════════════════════════════════════════════════════════════════
        // ── TAB 2: PAYROLL PROCESSING ───────────────────────────────────────
        // ════════════════════════════════════════════════════════════════════
        // ════════════════════════════════════════════════════════════════════
        // ── TAB 2: PAYROLL PROCESSING ────────────────────────────────────────
        // ════════════════════════════════════════════════════════════════════
        private DataTable _payrollCache; // used for inline-edit tracking

        private void InitPayrollTab()
        {
            // Month combo
            comboBoxmonth.Items.Clear();
            comboBoxmonth.Items.AddRange(new object[] {
                "January","February","March","April","May","June",
                "July","August","September","October","November","December" });
            comboBoxmonth.SelectedIndex = DateTime.Today.Month - 1;

            // Year combo
            comboBoxyear.Items.Clear();
            for (int y = DateTime.Today.Year - 2; y <= DateTime.Today.Year; y++)
                comboBoxyear.Items.Add(y.ToString());
            comboBoxyear.SelectedItem = DateTime.Today.Year.ToString();

            LoadPayrollEmployeeDropdown();

            // Style grid — match Employee Master (same StyleGridHeaders call)
            StyleGridHeaders(dataGridViewpayrolldetail);
            dataGridViewpayrolldetail.AllowUserToAddRows = false;
            dataGridViewpayrolldetail.SelectionMode      = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewpayrolldetail.ReadOnly           = false;
            dataGridViewpayrolldetail.EditMode           = DataGridViewEditMode.EditOnKeystrokeOrF2;

            // Style Edit/Delete/Generate button columns
            StylePayrollBtnCol("dataGridViewButtonColumn1", Color.FromArgb(0, 120, 212));
            StylePayrollBtnCol("dataGridViewButtonColumn2", Color.FromArgb(168, 0, 0));
            StylePayrollBtnCol("Generate",                  Color.FromArgb(16, 124, 16));

            // Wire buttons
            buttonloadallemployee.Click                += BtnPayrollLoadAll_Click;
            buttonprocess_individual_payroll.Click     += BtnProcessIndividual_Click;
            buttonprocessallpayroll.Click              += BtnProcessAll_Click;
            buttonlockpayrollmonth.Click               += BtnLockMonth_Click;
            buttonrefreshgride.Click                   += BtnPayrollRefresh_Click;
            Generate_yearly_payroll_summary.Click      += BtnGenerateSummaryReport_Click;
            buttongenerate_all_salary_slips_in_zip.Click += BtnGenerateAllSlips_Click;

            dataGridViewpayrolldetail.CellContentClick += DataGridViewPayroll_CellContentClick;
            dataGridViewpayrolldetail.CellEndEdit      += DataGridViewPayroll_CellEndEdit;
            dataGridViewpayrolldetail.KeyDown          += DataGridViewPayroll_KeyDown;

            // Update status on month/year change
            comboBoxmonth.SelectedIndexChanged += (s, e) => UpdatePayrollLockStatus();
            comboBoxyear.SelectedIndexChanged  += (s, e) => UpdatePayrollLockStatus();

            UpdatePayrollLockStatus();
        }

        private void StylePayrollBtnCol(string name, Color color)
        {
            if (!dataGridViewpayrolldetail.Columns.Contains(name)) return;
            var col = dataGridViewpayrolldetail.Columns[name] as DataGridViewButtonColumn;
            if (col == null) return;
            col.DefaultCellStyle.BackColor = color;
            col.DefaultCellStyle.ForeColor = Color.White;
            col.FlatStyle = FlatStyle.Flat;
            col.ReadOnly  = false;
        }

        private void LoadPayrollEmployeeDropdown()
        {
            try
            {
                comboBoxEmployeeid.Items.Clear();
                comboBoxEmployeeid.Items.Add(new ComboItem("-- Select Employee --", 0));
                DataTable emps = _empDal.GetActiveEmployees();
                foreach (DataRow r in emps.Rows)
                    comboBoxEmployeeid.Items.Add(new ComboItem(
                        r["FullName"] + " (" + r["EmployeeCode"] + ")", Convert.ToInt32(r["EmployeeID"])));
                comboBoxEmployeeid.SelectedIndex = 0;
            }
            catch (Exception ex)
            { System.Diagnostics.Debug.WriteLine("Payroll emp dropdown error: " + ex.Message); }
        }

        private void GetPayrollMonthYear(out int month, out int year)
        {
            month = comboBoxmonth.SelectedIndex + 1;
            year  = comboBoxyear.SelectedItem != null ? int.Parse(comboBoxyear.SelectedItem.ToString()) : DateTime.Today.Year;
        }

        // ── Load All Employees (active + inactive) for month ─────────────────
        private void BtnPayrollLoadAll_Click(object sender, EventArgs e)
        {
            int month, year;
            GetPayrollMonthYear(out month, out year);
            try
            {
                DataTable dt = _payrollBLL.GetMonthlyPayrollReport(month, year);
                _payrollCache = dt;
                BindPayrollGrid(dt);
                label18.Text = "Showing " + dt.Rows.Count + " record(s) for " + GetMonthName(month) + " " + year;
            }
            catch (Exception ex)
            { MessageBox.Show("Error loading payroll:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BindPayrollGrid(DataTable dt)
        {
            dataGridViewpayrolldetail.Rows.Clear();
            foreach (DataRow row in dt.Rows)
            {
                int idx = dataGridViewpayrolldetail.Rows.Add();
                DataGridViewRow gr = dataGridViewpayrolldetail.Rows[idx];

                gr.Cells["dataGridViewTextBoxColumn1"].Value = SafeStr(row, "FullName") + " — " + SafeStr(row, "Department");
                gr.Cells["Payroll_ID"].Value          = row["PayrollDetailID"];
                gr.Cells["Basic_Pay1"].Value          = FormatCurrency(row, "BasicPay");
                gr.Cells["Allowances1"].Value         = FormatCurrency(row, "Allowances");
                gr.Cells["Overtime_amount"].Value     = FormatCurrency(row, "OvertimeAmount");
                gr.Cells["Bonus"].Value               = FormatCurrency(row, "Bonus");
                gr.Cells["Gross_salary"].Value        = FormatCurrency(row, "GrossSalary");
                gr.Cells["Tax_deduction"].Value       = FormatCurrency(row, "TaxDeduction");
                gr.Cells["Attendance_deduction"].Value= FormatCurrency(row, "AttendanceDeductions");
                gr.Cells["Other_Deductions"].Value    = FormatCurrency(row, "OtherDeductions");

                // Tag stores EmployeeID and PayrollDetailID: empID|detailID
                gr.Tag = row["EmployeeID"].ToString() + "|" + row["PayrollDetailID"].ToString();

                // Lock non-editable columns
                gr.Cells["dataGridViewTextBoxColumn1"].ReadOnly = true;
                gr.Cells["Payroll_ID"].ReadOnly                = true;
                gr.Cells["Gross_salary"].ReadOnly              = true;

                // Match Employee Master colour: inactive rows (if IsActive present) go grey
                if (dt.Columns.Contains("IsActive") && row["IsActive"] != DBNull.Value && !Convert.ToBoolean(row["IsActive"]))
                { gr.DefaultCellStyle.ForeColor = Color.Gray; gr.DefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Italic); }
            }
        }

        private string FormatCurrency(DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col) || row[col] == DBNull.Value) return "0.00";
            return Convert.ToDecimal(row[col]).ToString("N2");
        }

        // ── Payroll Grid: Edit / Delete / Generate (inline) ──────────────────
        private int _payrollEditRowIndex   = -1;
        private int _payrollEditDetailID   = 0;

        private void DataGridViewPayroll_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row  = dataGridViewpayrolldetail.Rows[e.RowIndex];
            string[]        tags = (row.Tag?.ToString() ?? "0|0").Split('|');
            int empID = 0, detailID = 0;
            int.TryParse(tags[0], out empID);
            if (tags.Length > 1) int.TryParse(tags[1], out detailID);

            // Edit — inline edit on numeric deduction/amount columns
            if (dataGridViewpayrolldetail.Columns.Contains("dataGridViewButtonColumn1") &&
                e.ColumnIndex == dataGridViewpayrolldetail.Columns["dataGridViewButtonColumn1"].Index)
            {
                if (_payrollEditRowIndex >= 0 && _payrollEditRowIndex != e.RowIndex) CommitPayrollEdit();
                _payrollEditRowIndex = e.RowIndex;
                _payrollEditDetailID = detailID;

                foreach (string col in new[] { "Basic_Pay1","Allowances1","Overtime_amount","Bonus","Tax_deduction","Attendance_deduction","Other_Deductions" })
                    if (dataGridViewpayrolldetail.Columns.Contains(col)) row.Cells[col].ReadOnly = false;

                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 204);
                dataGridViewpayrolldetail.CurrentCell = row.Cells["Basic_Pay1"];
                dataGridViewpayrolldetail.BeginEdit(true);
            }
            // Delete — delete payroll detail record from DB
            else if (dataGridViewpayrolldetail.Columns.Contains("dataGridViewButtonColumn2") &&
                     e.ColumnIndex == dataGridViewpayrolldetail.Columns["dataGridViewButtonColumn2"].Index)
            {
                string empName = row.Cells["dataGridViewTextBoxColumn1"].Value?.ToString() ?? "";
                if (MessageBox.Show("Delete payroll record for:\n\n  " + empName +
                    "\n\nThis permanently removes the record from the database.",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        _payrollBLL.DeletePayrollDetail(detailID, CurrentUser.AuditName);
                        dataGridViewpayrolldetail.Rows.RemoveAt(e.RowIndex);
                        label18.Text = "Record deleted.";
                    }
                    catch (Exception ex)
                    { MessageBox.Show("Delete failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
            // Generate — generate PDF salary slip for this employee
            else if (dataGridViewpayrolldetail.Columns.Contains("Generate") &&
                     e.ColumnIndex == dataGridViewpayrolldetail.Columns["Generate"].Index)
            {
                int month, year;
                GetPayrollMonthYear(out month, out year);
                GeneratePdfPayslip(empID, month, year);
            }
        }

        private void CommitPayrollEdit()
        {
            if (_payrollEditRowIndex < 0) return;
            dataGridViewpayrolldetail.EndEdit();
            DataGridViewRow gr = dataGridViewpayrolldetail.Rows[_payrollEditRowIndex];

            decimal bp, al, ot, bn, tax, attDed, othDed;
            decimal.TryParse(CellVal(gr, "Basic_Pay1").Replace(",",""),        out bp);
            decimal.TryParse(CellVal(gr, "Allowances1").Replace(",",""),       out al);
            decimal.TryParse(CellVal(gr, "Overtime_amount").Replace(",",""),   out ot);
            decimal.TryParse(CellVal(gr, "Bonus").Replace(",",""),             out bn);
            decimal.TryParse(CellVal(gr, "Tax_deduction").Replace(",",""),     out tax);
            decimal.TryParse(CellVal(gr, "Attendance_deduction").Replace(",",""), out attDed);
            decimal.TryParse(CellVal(gr, "Other_Deductions").Replace(",",""),  out othDed);

            decimal gross = bp + al + ot + bn;
            decimal net   = gross - tax - attDed - othDed;

            try
            {
                _payrollBLL.UpdatePayrollDetail(_payrollEditDetailID, bp, al, ot, bn, gross, tax, attDed, othDed, net, CurrentUser.AuditName);
                // Refresh gross in grid
                gr.Cells["Gross_salary"].Value = gross.ToString("N2");
                gr.DefaultCellStyle.BackColor = Color.White;
                _payrollEditRowIndex = -1; _payrollEditDetailID = 0;
                MessageBox.Show("Payroll record updated successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            { MessageBox.Show("Update failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private string CellVal(DataGridViewRow gr, string col)
        { return dataGridViewpayrolldetail.Columns.Contains(col) ? (gr.Cells[col].Value?.ToString() ?? "0") : "0"; }

        private void DataGridViewPayroll_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        { if (e.RowIndex == _payrollEditRowIndex) CommitPayrollEdit(); }

        private void DataGridViewPayroll_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter  && _payrollEditRowIndex >= 0) { CommitPayrollEdit(); e.Handled = true; }
            if (e.KeyCode == Keys.Escape && _payrollEditRowIndex >= 0)
            { dataGridViewpayrolldetail.CancelEdit(); _payrollEditRowIndex = -1; _payrollEditDetailID = 0; e.Handled = true; }
        }

        // ── Process Individual Payroll ────────────────────────────────────────
        private void BtnProcessIndividual_Click(object sender, EventArgs e)
        {
            ComboItem selected = comboBoxEmployeeid.SelectedItem as ComboItem;
            if (selected == null || selected.Value == 0)
            { MessageBox.Show("Please select an employee first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            int month, year;
            GetPayrollMonthYear(out month, out year);

            if (MessageBox.Show("Process payroll for:\n\n  " + selected.Text + "\n  " + GetMonthName(month) + " " + year,
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                _payrollBLL.ProcessPayroll(selected.Value, month, year, CurrentUser.AuditName);
                MessageBox.Show("Payroll processed successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BtnPayrollLoadAll_Click(sender, e);
                UpdatePayrollLockStatus();
            }
            catch (Exception ex)
            { MessageBox.Show("Payroll processing failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ── Process All Active Employees ──────────────────────────────────────
        private void BtnProcessAll_Click(object sender, EventArgs e)
        {
            int month, year;
            GetPayrollMonthYear(out month, out year);

            if (MessageBox.Show("Process payroll for ALL active employees for " + GetMonthName(month) + " " + year + "?\n\nThis may take a moment.",
                "Confirm Bulk Process", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            DataTable emps = _empDal.GetActiveEmployees();
            int success = 0, failed = 0;
            StringBuilder errors = new StringBuilder();
            foreach (DataRow emp in emps.Rows)
            {
                int empID = Convert.ToInt32(emp["EmployeeID"]);
                try { _payrollBLL.ProcessPayroll(empID, month, year, CurrentUser.AuditName); success++; }
                catch (Exception ex) { failed++; if (failed <= 5) errors.AppendLine("• " + emp["FullName"] + ": " + ex.Message); }
            }

            string result = "Processed: " + success + " employees.";
            if (failed > 0) result += "\nFailed: " + failed + "\n\n" + errors;
            MessageBox.Show(result, "Bulk Payroll Done", MessageBoxButtons.OK, failed > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

            BtnPayrollLoadAll_Click(sender, e);
            UpdatePayrollLockStatus();
        }

        // ── Lock Payroll Month ────────────────────────────────────────────────
        private void BtnLockMonth_Click(object sender, EventArgs e)
        {
            int month, year;
            GetPayrollMonthYear(out month, out year);
            if (_payrollBLL.IsMonthLocked(month, year))
            { MessageBox.Show(GetMonthName(month) + " " + year + " is already locked.", "Already Locked", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            if (MessageBox.Show("Lock payroll for " + GetMonthName(month) + " " + year + "?\n\nThis cannot be undone — no further changes will be allowed.",
                "Confirm Lock", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                _payrollBLL.LockMonth(month, year, CurrentUser.AuditName);
                UpdatePayrollLockStatus();
                MessageBox.Show("Payroll month locked successfully.", "Locked", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            { MessageBox.Show("Lock failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ── Refresh ───────────────────────────────────────────────────────────
        private void BtnPayrollRefresh_Click(object sender, EventArgs e)
        {
            dataGridViewpayrolldetail.Rows.Clear();
            label18.Text = "";
            UpdatePayrollLockStatus();
        }

        // ── Status (Locked / Processed / Draft) ───────────────────────────────
        private void UpdatePayrollLockStatus()
        {
            int month, year;
            GetPayrollMonthYear(out month, out year);
            try
            {
                bool locked = _payrollBLL.IsMonthLocked(month, year);
                // Check if any payroll exists (Processed) or none (Draft)
                string statusText;
                Color  statusColor;
                Color  panelColor;
                if (locked)
                { statusText = "LOCKED"; statusColor = Color.DarkRed;   panelColor = Color.FromArgb(255, 220, 220); }
                else
                {
                    DataTable chk = _payrollBLL.GetMonthlyPayrollReport(month, year);
                    if (chk.Rows.Count > 0)
                    { statusText = "Processed (" + chk.Rows.Count + ")"; statusColor = Color.DarkBlue; panelColor = Color.FromArgb(220, 235, 255); }
                    else
                    { statusText = "Draft (no data)"; statusColor = Color.DarkGreen; panelColor = Color.FromArgb(220, 255, 220); }
                }
                labelstatus.Text       = "Status: " + statusText;
                labelstatus.ForeColor  = statusColor;
                Status_color_panel.BackColor = panelColor;
                buttonlockpayrollmonth.Enabled            = !locked;
                buttonprocess_individual_payroll.Enabled  = !locked;
                buttonprocessallpayroll.Enabled           = !locked;
            }
            catch { }
        }

        // ── Generate Summary Report CSV (for selected month/year) ─────────────
        private void BtnGenerateSummaryReport_Click(object sender, EventArgs e)
        {
            int month, year;
            GetPayrollMonthYear(out month, out year);

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter   = "CSV files (*.csv)|*.csv",
                FileName = "Payroll_Summary_" + GetMonthName(month) + "_" + year + ".csv"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                DataTable dt = _payrollBLL.GetMonthlyPayrollReport(month, year);
                if (dt.Rows.Count == 0)
                { MessageBox.Show("No payroll data found for " + GetMonthName(month) + " " + year + ".\nProcess payroll first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Employee,Department,Month,Year,BasicPay,Allowances,OvertimeAmount,Bonus,GrossSalary,TaxDeduction,AttendanceDeductions,OtherDeductions,NetSalary");
                foreach (DataRow row in dt.Rows)
                    sb.AppendLine(string.Join(",", new[] {
                        CsvSafe(row,"FullName"), CsvSafe(row,"Department"), GetMonthName(month), year.ToString(),
                        row["BasicPay"].ToString(), row["Allowances"].ToString(),
                        row["OvertimeAmount"].ToString(), row["Bonus"].ToString(),
                        row["GrossSalary"].ToString(), row["TaxDeduction"].ToString(),
                        row["AttendanceDeductions"].ToString(), row["OtherDeductions"].ToString(),
                        row["NetSalary"].ToString()
                    }));
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Summary report exported:\n" + sfd.FileName, "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            { MessageBox.Show("Export failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ── Generate All Salary Slips → ZIP of PDFs ───────────────────────────
        private void BtnGenerateAllSlips_Click(object sender, EventArgs e)
        {
            int month, year;
            GetPayrollMonthYear(out month, out year);
            DataTable dt = _payrollBLL.GetMonthlyPayrollReport(month, year);
            if (dt.Rows.Count == 0)
            { MessageBox.Show("No payroll data found for " + GetMonthName(month) + " " + year + ". Process payroll first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter   = "ZIP files (*.zip)|*.zip",
                FileName = "SalarySlips_" + GetMonthName(month) + "_" + year + ".zip"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                // Create a temp folder for HTML payslips, then zip them
                string tempDir = Path.Combine(Path.GetTempPath(), "PayslipTemp_" + Guid.NewGuid().ToString("N").Substring(0, 8));
                Directory.CreateDirectory(tempDir);

                foreach (DataRow row in dt.Rows)
                {
                    int empID = Convert.ToInt32(row["EmployeeID"]);
                    DataTable slip = _payrollBLL.GetPayslip(empID, month, year);
                    if (slip.Rows.Count == 0) continue;
                    DataRow r = slip.Rows[0];

                    // Build an HTML payslip (browser-printable, saves as PDF)
                    string html = BuildPayslipHtml(r, month, year);
                    string safeName = r["FullName"].ToString().Replace(" ", "_").Replace("/", "_");
                    string filePath = Path.Combine(tempDir, safeName + "_" + GetMonthName(month) + "_" + year + ".html");
                    File.WriteAllText(filePath, html, Encoding.UTF8);
                }

                // Zip the temp folder
                if (File.Exists(sfd.FileName)) File.Delete(sfd.FileName);
                System.IO.Compression.ZipFile.CreateFromDirectory(tempDir, sfd.FileName);

                // Cleanup temp
                try { Directory.Delete(tempDir, true); } catch { }

                MessageBox.Show("All salary slips packaged:\n" + sfd.FileName +
                    "\n\nOpen the HTML files in a browser and use Print → Save as PDF.",
                    "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            { MessageBox.Show("Failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ── Generate single PDF payslip (from Generate button in grid) ────────
        private void GeneratePdfPayslip(int empID, int month, int year)
        {
            try
            {
                DataTable slip = _payrollBLL.GetPayslip(empID, month, year);
                if (slip.Rows.Count == 0)
                { MessageBox.Show("Payslip not found. Process payroll for this employee first.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                DataRow r = slip.Rows[0];
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter   = "HTML files (*.html)|*.html",
                    FileName = r["FullName"].ToString().Replace(" ", "_") + "_" + GetMonthName(month) + "_" + year + ".html"
                };
                if (sfd.ShowDialog() != DialogResult.OK) return;

                string html = BuildPayslipHtml(r, month, year);
                File.WriteAllText(sfd.FileName, html, Encoding.UTF8);
                MessageBox.Show("Salary slip saved:\n" + sfd.FileName +
                    "\n\nOpen in browser → Print → Save as PDF.",
                    "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Auto-open in default browser
                try { System.Diagnostics.Process.Start(sfd.FileName); } catch { }
            }
            catch (Exception ex)
            { MessageBox.Show("Error generating payslip:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private string BuildPayslipHtml(DataRow r, int month, int year)
        {
            Func<string, string> Dec = col => r.Table.Columns.Contains(col) && r[col] != DBNull.Value
                ? Convert.ToDecimal(r[col]).ToString("N2") : "0.00";
            Func<string, string> Str = col => r.Table.Columns.Contains(col) && r[col] != DBNull.Value
                ? r[col].ToString() : "";

            return $@"<!DOCTYPE html>
<html lang=""en""><head><meta charset=""UTF-8"">
<title>Salary Slip — {Str("FullName")} — {GetMonthName(month)} {year}</title>
<style>
  body {{ font-family: 'Segoe UI', Arial, sans-serif; margin: 40px; color: #222; background:#fff; }}
  .slip {{ max-width: 700px; margin: auto; border: 1px solid #0078d4; border-radius: 8px; overflow: hidden; }}
  .header {{ background: #0078d4; color: #fff; padding: 20px 28px; }}
  .header h1 {{ margin:0; font-size: 22px; letter-spacing: 1px; }}
  .header p  {{ margin:4px 0 0; font-size: 13px; opacity:.85; }}
  .body {{ padding: 24px 28px; }}
  .info-row {{ display:flex; gap:40px; margin-bottom:14px; }}
  .info-row div {{ flex:1; }}
  .info-row label {{ font-size:11px; color:#777; text-transform:uppercase; letter-spacing:.5px; }}
  .info-row span  {{ display:block; font-size:14px; font-weight:600; }}
  table {{ width:100%; border-collapse:collapse; margin-top:12px; }}
  th {{ background:#f0f4fa; padding:8px 12px; text-align:left; font-size:12px; color:#555; border-bottom:2px solid #0078d4; }}
  td {{ padding:8px 12px; font-size:13px; border-bottom:1px solid #eee; }}
  td.amount {{ text-align:right; font-family:monospace; }}
  .deduct td {{ color:#c00; }}
  .total td {{ font-weight:700; font-size:15px; background:#e8f4e8; color:#155724; }}
  .footer {{ background:#f5f5f5; padding:12px 28px; font-size:11px; color:#888; text-align:center; }}
  @media print {{ body {{ margin:0; }} .slip {{ border:none; }} }}
</style></head><body>
<div class=""slip"">
  <div class=""header"">
    <h1>SALARY SLIP</h1>
    <p>{GetMonthName(month)} {year} &nbsp;|&nbsp; Generated: {DateTime.Today:dd MMM yyyy}</p>
  </div>
  <div class=""body"">
    <div class=""info-row"">
      <div><label>Employee</label><span>{Str("FullName")}</span></div>
      <div><label>Designation</label><span>{Str("Designation")}</span></div>
      <div><label>Department</label><span>{Str("Department")}</span></div>
    </div>
    <div class=""info-row"">
      <div><label>Bank</label><span>{Str("BankName")}</span></div>
      <div><label>Account Number</label><span>{Str("BankAccountNumber")}</span></div>
      <div><label>Period</label><span>{GetMonthName(month)} {year}</span></div>
    </div>
    <table>
      <thead><tr><th>Description</th><th style=""text-align:right"">Amount (PKR)</th></tr></thead>
      <tbody>
        <tr><td>Basic Pay</td><td class=""amount"">{Dec("BasicPay")}</td></tr>
        <tr><td>Allowances</td><td class=""amount"">{Dec("Allowances")}</td></tr>
        <tr><td>Overtime</td><td class=""amount"">{Dec("OvertimeAmount")}</td></tr>
        <tr><td>Bonus</td><td class=""amount"">{Dec("Bonus")}</td></tr>
        <tr style=""font-weight:600;background:#f0f4fa""><td>Gross Salary</td><td class=""amount"">{Dec("GrossSalary")}</td></tr>
        <tr class=""deduct""><td>(-) Tax Deduction</td><td class=""amount"">({Dec("TaxDeduction")})</td></tr>
        <tr class=""deduct""><td>(-) Attendance Deductions</td><td class=""amount"">({Dec("AttendanceDeductions")})</td></tr>
        <tr class=""deduct""><td>(-) Other Deductions</td><td class=""amount"">({Dec("OtherDeductions")})</td></tr>
        <tr class=""total""><td>NET SALARY</td><td class=""amount"">{Dec("NetSalary")}</td></tr>
      </tbody>
    </table>
  </div>
  <div class=""footer"">This is a computer-generated salary slip. No signature required.</div>
</div>
</body></html>";
        }

        private string GetMonthName(int month) => month >= 1 && month <= 12 ? new DateTime(2000, month, 1).ToString("MMMM") : "";


        // ════════════════════════════════════════════════════════════════════
        // ── TAB 3: TAXATION ─────────────────────────────────────────────────
        // ════════════════════════════════════════════════════════════════════
        private void InitTaxTab()
        {
            // Employee dropdown
            LoadTaxEmployeeDropdown();

            // Year combo
            comboBox_year.Items.Clear();
            for (int y = DateTime.Today.Year - 3; y <= DateTime.Today.Year; y++)
                comboBox_year.Items.Add(y.ToString());
            comboBox_year.SelectedItem = DateTime.Today.Year.ToString();

            // Month combo
            comboBox_month.Items.Clear();
            string[] months = { "January","February","March","April","May","June",
                                 "July","August","September","October","November","December" };
            comboBox_month.Items.AddRange(months);
            comboBox_month.SelectedIndex = DateTime.Today.Month - 1;

            // Style grids
            StyleGridHeaders(dataGridView_EmployeeTax);
            StyleGridHeaders(dataGridView_Taxslab);

            // Wire buttons
            button_loadData.Click += BtnTaxLoad_Click;
            button_refresh.Click += BtnTaxRefresh_Click;
            generate_early_tax_summary1.Click += BtnYearlyTaxSummary_Click;
            generate_finance_report_of_tax.Click += BtnTaxFinanceReport_Click;

            // Tax slab grid click (Edit/Delete)
            dataGridView_Taxslab.CellContentClick += DataGridView_Taxslab_CellClick;
            dataGridView_Taxslab.ReadOnly = false;

            // Load tax slabs on init
            LoadTaxSlabs();
        }

        private void LoadTaxEmployeeDropdown()
        {
            try
            {
                comboBox_employee.Items.Clear();
                comboBox_employee.Items.Add(new ComboItem("-- All --", 0));
                DataTable emps = _empDal.GetActiveEmployees();
                foreach (DataRow r in emps.Rows)
                    comboBox_employee.Items.Add(new ComboItem(r["FullName"] + " (" + r["EmployeeCode"] + ")", Convert.ToInt32(r["EmployeeID"])));
                comboBox_employee.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadTaxSlabs()
        {
            try
            {
                DataTable dt = _taxBLL.GetAllSlabs();
                dataGridView_Taxslab.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    int idx = dataGridView_Taxslab.Rows.Add();
                    DataGridViewRow gr = dataGridView_Taxslab.Rows[idx];
                    gr.Cells["SlabID"].Value = row["SlabID"];
                    gr.Cells["From_amount1"].Value = Convert.ToDecimal(row["FromAmount"]).ToString("N2");
                    gr.Cells["TO_amount"].Value = Convert.ToDecimal(row["ToAmount"]).ToString("N2");
                    gr.Cells["Tax_percent"].Value = row["TaxPercent"] + "%";
                    gr.Tag = row["SlabID"];
                }

                // Add a row at bottom to insert new slab
                int newIdx = dataGridView_Taxslab.Rows.Add();
                DataGridViewRow newRow = dataGridView_Taxslab.Rows[newIdx];
                newRow.Cells["SlabID"].Value = "[New]";
                newRow.Cells["From_amount1"].Value = "";
                newRow.Cells["TO_amount"].Value = "";
                newRow.Cells["Tax_percent"].Value = "";
                newRow.Tag = -1; // signals "new row"
            }
            catch (Exception ex)
            { MessageBox.Show("Error loading tax slabs:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void DataGridView_Taxslab_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dataGridView_Taxslab.Rows[e.RowIndex];
            object tag = row.Tag;

            // Edit3 column
            if (e.ColumnIndex == dataGridView_Taxslab.Columns["Edit3"].Index)
            {
                if (tag == null || Convert.ToInt32(tag) <= 0)
                {
                    // New row — save
                    SaveNewTaxSlab(row);
                }
                else
                {
                    // Existing row — start inline edit
                    dataGridView_Taxslab.ReadOnly = false;
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 220);
                    _editingSlabID = Convert.ToInt32(tag);
                    _slabEditMode = true;
                    dataGridView_Taxslab.CurrentCell = row.Cells["From_amount1"];
                    dataGridView_Taxslab.BeginEdit(true);
                }
            }
            // Delete3 column
            else if (e.ColumnIndex == dataGridView_Taxslab.Columns["Delete3"].Index)
            {
                if (tag == null || Convert.ToInt32(tag) <= 0) return;
                int slabID = Convert.ToInt32(tag);
                if (MessageBox.Show("Delete this tax slab?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    { _taxBLL.DeleteSlab(slabID, CurrentUser.AuditName); LoadTaxSlabs(); }
                    catch (Exception ex)
                    { MessageBox.Show("Delete failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void SaveNewTaxSlab(DataGridViewRow row)
        {
            decimal from, to, pct;
            if (!decimal.TryParse(row.Cells["From_amount1"].Value?.ToString() ?? "", out from) ||
                !decimal.TryParse(row.Cells["TO_amount"].Value?.ToString() ?? "", out to) ||
                !decimal.TryParse(row.Cells["Tax_percent"].Value?.ToString()?.Replace("%", "").Trim() ?? "", out pct))
            { MessageBox.Show("Please enter valid numeric values for all fields.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            try
            { _taxBLL.AddSlab(from, to, pct, CurrentUser.AuditName); LoadTaxSlabs(); }
            catch (Exception ex)
            { MessageBox.Show("Save failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnTaxLoad_Click(object sender, EventArgs e)
        {
            int year = comboBox_year.SelectedItem != null ? int.Parse(comboBox_year.SelectedItem.ToString()) : DateTime.Today.Year;
            int month = comboBox_month.SelectedIndex + 1;
            ComboItem empSel = comboBox_employee.SelectedItem as ComboItem;
            int empID = (empSel != null) ? empSel.Value : 0;

            try
            {
                DataTable dt = empID > 0
                    ? _taxBLL.GetEmployeeTaxHistory(empID)
                    : _taxBLL.GetMonthlyReport(month, year);

                dataGridView_EmployeeTax.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    int idx = dataGridView_EmployeeTax.Rows.Add();
                    DataGridViewRow gr = dataGridView_EmployeeTax.Rows[idx];
                    gr.Cells["TaxID"].Value = row["TaxID"];
                    gr.Cells["EmployeeID1"].Value = row["EmployeeID"];
                    gr.Cells["Tax_Month"].Value = row["TaxMonth"];
                    gr.Cells["Tax_Year"].Value = row["TaxYear"];
                    gr.Cells["Taxable_income"].Value = row["TaxableIncome"] != DBNull.Value ? Convert.ToDecimal(row["TaxableIncome"]).ToString("N2") : "0.00";
                    gr.Cells["Tax_Amount"].Value = row["TaxAmount"] != DBNull.Value ? Convert.ToDecimal(row["TaxAmount"]).ToString("N2") : "0.00";
                }
            }
            catch (Exception ex)
            { MessageBox.Show("Error loading tax data:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnTaxRefresh_Click(object sender, EventArgs e)
        {
            dataGridView_EmployeeTax.Rows.Clear();
            comboBox_employee.SelectedIndex = 0;
            LoadTaxSlabs();
        }

        private void BtnYearlyTaxSummary_Click(object sender, EventArgs e)
        {
            int year = comboBox_year.SelectedItem != null ? int.Parse(comboBox_year.SelectedItem.ToString()) : DateTime.Today.Year;
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "Tax_Yearly_Summary_" + year + ".csv"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            try
            {
                DataTable dt = _taxBLL.GetYearlySummary(year);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("EmployeeID,Employee,Year,TotalTaxableIncome,TotalTaxDeducted");
                foreach (DataRow row in dt.Rows)
                    sb.AppendLine(string.Join(",", new[] {
                        row["EmployeeID"].ToString(), CsvSafe(row,"FullName"),
                        year.ToString(), row["TotalTaxableIncome"].ToString(), row["TotalTaxAmount"].ToString()
                    }));
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Yearly tax summary exported:\n" + sfd.FileName, "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            { MessageBox.Show("Export failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnTaxFinanceReport_Click(object sender, EventArgs e)
        {
            int year = comboBox_year.SelectedItem != null ? int.Parse(comboBox_year.SelectedItem.ToString()) : DateTime.Today.Year;
            int month = comboBox_month.SelectedIndex + 1;
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "Tax_Finance_Report_" + GetMonthName(month) + "_" + year + ".csv"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            try
            {
                DataTable dt = _taxBLL.GetMonthlyReport(month, year);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("EmployeeID,Employee,Month,Year,TaxableIncome,TaxAmount");
                foreach (DataRow row in dt.Rows)
                    sb.AppendLine(string.Join(",", new[] {
                        row["EmployeeID"].ToString(), CsvSafe(row,"FullName"),
                        GetMonthName(month), year.ToString(),
                        row["TaxableIncome"].ToString(), row["TaxAmount"].ToString()
                    }));
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Finance tax report exported:\n" + sfd.FileName, "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            { MessageBox.Show("Export failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ════════════════════════════════════════════════════════════════════
        // ── TAB 5: ATTENDANCE MANAGEMENT ────────────────────────────────────
        // ════════════════════════════════════════════════════════════════════
        private DataTable _attendanceCache; // cache for export

        private void InitAttendanceTab()
        {
            // Default date to today
            textBoxDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            textBoxEmployeeid.Text = "";

            // Status combo
            comboBoxStatus.Items.Clear();
            comboBoxStatus.Items.AddRange(new object[] { "Present", "Late", "CL", "SL", "AL", "LWP" });
            comboBoxStatus.SelectedIndex = 0;

            StyleGridHeaders(dataGridView_attendance);

            // Wire buttons
            CheckIn_attendance.Click += BtnCheckIn_Click;
            CheckOut_attendance.Click += BtnCheckOut_Click;
            Mark_absent.Click += BtnMarkLeave_Click;
            Export_attendance_inCSV.Click += BtnExportAttendance_Click;
            Upload_attendance_inCSV.Click += BtnUploadAttendance_Click;
            View_attendance_summary.Click += BtnViewAttendanceSummary_Click;

            // Grid button click (Edit / Delete)
            dataGridView_attendance.CellContentClick += DataGridViewAttendance_CellContentClick;

            // Load today's attendance on startup
            LoadAttendanceByDate(DateTime.Today);
        }

        private void LoadAttendanceByDate(DateTime date)
        {
            try
            {
                DataTable dt = _attBLL.GetByDate(date);
                _attendanceCache = dt;
                BindAttendanceGrid(dt);
            }
            catch (Exception ex)
            { MessageBox.Show("Error loading attendance:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BindAttendanceGrid(DataTable dt)
        {
            dataGridView_attendance.Rows.Clear();
            foreach (DataRow row in dt.Rows)
            {
                int idx = dataGridView_attendance.Rows.Add();
                DataGridViewRow gr = dataGridView_attendance.Rows[idx];
                gr.Cells["Employee_name_attend"].Value = SafeStr(row, "FullName") + " (" + SafeStr(row, "Department") + ")";
                gr.Cells["Attendance_date"].Value = row["AttendanceDate"] != DBNull.Value ? Convert.ToDateTime(row["AttendanceDate"]).ToString("yyyy-MM-dd") : "";
                gr.Cells["CheckIn"].Value = SafeStr(row, "CheckInTime");
                gr.Cells["Check_out"].Value = SafeStr(row, "CheckOutTime");
                gr.Cells["Working_hours"].Value = row["WorkingHours"] != DBNull.Value ? Convert.ToDecimal(row["WorkingHours"]).ToString("N2") : "";
                gr.Cells["Overtime"].Value = row["OvertimeHours"] != DBNull.Value ? Convert.ToDecimal(row["OvertimeHours"]).ToString("N2") : "";
                gr.Cells["status"].Value = SafeStr(row, "AttendanceStatus");
                gr.Tag = row["AttendanceID"]; // store ID for edit/delete

                // Color coding by status
                string s = SafeStr(row, "AttendanceStatus");
                if (s == "Late") gr.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205);
                else if (s == "LWP") gr.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                else if (s == "Present") gr.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);
                else if (s == "CL" || s == "SL" || s == "AL") gr.DefaultCellStyle.BackColor = Color.FromArgb(220, 235, 255);
            }
        }

        private void BtnCheckIn_Click(object sender, EventArgs e)
        {
            int empID;
            if (!int.TryParse(textBoxEmployeeid.Text.Trim(), out empID) || empID <= 0)
            { MessageBox.Show("Please enter a valid Employee ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            DateTime date;
            if (!DateTime.TryParse(textBoxDate.Text.Trim(), out date))
            { MessageBox.Show("Please enter a valid date (yyyy-MM-dd).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            TimeSpan checkIn = DateTime.Now.TimeOfDay;
            if (checkBox_leave.Checked)
            {
                // If leave checkbox is checked, mark leave instead
                BtnMarkLeave_Click(sender, e);
                return;
            }

            try
            {
                _attBLL.MarkCheckIn(empID, date, checkIn, CurrentUser.AuditName);
                MessageBox.Show("Check-in marked at " + checkIn.ToString(@"hh\:mm"), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAttendanceByDate(date);
            }
            catch (Exception ex)
            { MessageBox.Show("Check-in failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnCheckOut_Click(object sender, EventArgs e)
        {
            if (dataGridView_attendance.SelectedRows.Count == 0)
            { MessageBox.Show("Please select an attendance row to mark check-out.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            DataGridViewRow row = dataGridView_attendance.SelectedRows[0];
            object tag = row.Tag;
            if (tag == null) return;
            int attendanceID = Convert.ToInt32(tag);

            // Try to get employee ID and existing check-in from the row
            string empIDStr = textBoxEmployeeid.Text.Trim();
            int empID;
            if (!int.TryParse(empIDStr, out empID) || empID <= 0)
            { MessageBox.Show("Please enter the Employee ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string checkInStr = row.Cells["CheckIn"].Value?.ToString() ?? "";
            TimeSpan checkIn;
            if (!TimeSpan.TryParse(checkInStr, out checkIn))
            { MessageBox.Show("Cannot determine check-in time for this record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            DateTime date;
            if (!DateTime.TryParse(textBoxDate.Text.Trim(), out date)) date = DateTime.Today;

            TimeSpan checkOut = DateTime.Now.TimeOfDay;
            try
            {
                _attBLL.MarkCheckOut(attendanceID, empID, date, checkIn, checkOut, CurrentUser.AuditName);
                MessageBox.Show("Check-out marked at " + checkOut.ToString(@"hh\:mm"), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAttendanceByDate(date);
            }
            catch (Exception ex)
            { MessageBox.Show("Check-out failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnMarkLeave_Click(object sender, EventArgs e)
        {
            int empID;
            if (!int.TryParse(textBoxEmployeeid.Text.Trim(), out empID) || empID <= 0)
            { MessageBox.Show("Please enter a valid Employee ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            DateTime date;
            if (!DateTime.TryParse(textBoxDate.Text.Trim(), out date))
            { MessageBox.Show("Please enter a valid date (yyyy-MM-dd).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string leaveType = comboBoxStatus.SelectedItem?.ToString() ?? "LWP";
            if (leaveType == "Present" || leaveType == "Late") leaveType = "LWP";

            try
            {
                _attBLL.MarkLeave(empID, date, leaveType, CurrentUser.AuditName);
                MessageBox.Show("Leave marked: " + leaveType, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAttendanceByDate(date);
            }
            catch (Exception ex)
            { MessageBox.Show("Mark leave failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void DataGridViewAttendance_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dataGridView_attendance.Rows[e.RowIndex];
            object tag = row.Tag;
            if (tag == null) return;
            int attendanceID = Convert.ToInt32(tag);

            if (e.ColumnIndex == dataGridView_attendance.Columns["Delete_attendance"].Index)
            {
                if (MessageBox.Show("Delete this attendance record?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        _attBLL.DeleteRecord(attendanceID, CurrentUser.AuditName);
                        DateTime date;
                        DateTime.TryParse(textBoxDate.Text.Trim(), out date);
                        LoadAttendanceByDate(date == DateTime.MinValue ? DateTime.Today : date);
                    }
                    catch (Exception ex)
                    { MessageBox.Show("Delete failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void BtnExportAttendance_Click(object sender, EventArgs e)
        {
            if (_attendanceCache == null || _attendanceCache.Rows.Count == 0)
            { MessageBox.Show("No data to export. Load attendance first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "Attendance_" + DateTime.Today.ToString("yyyyMMdd") + ".csv"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("EmployeeID,Employee,Department,Date,CheckIn,CheckOut,WorkingHours,OvertimeHours,Status");
                foreach (DataRow row in _attendanceCache.Rows)
                    sb.AppendLine(string.Join(",", new[] {
                        row["EmployeeID"].ToString(), CsvSafe(row,"FullName"), CsvSafe(row,"Department"),
                        row["AttendanceDate"].ToString(), row["CheckInTime"].ToString(),
                        row["CheckOutTime"].ToString(), row["WorkingHours"].ToString(),
                        row["OvertimeHours"].ToString(), row["AttendanceStatus"].ToString()
                    }));
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Exported:\n" + sfd.FileName, "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            { MessageBox.Show("Export failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnUploadAttendance_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "CSV files (*.csv)|*.csv", Title = "Upload Attendance CSV" };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            int success = 0, failed = 0;
            try
            {
                string[] lines = File.ReadAllLines(ofd.FileName, Encoding.UTF8);
                for (int i = 1; i < lines.Length; i++) // skip header
                {
                    string[] cols = lines[i].Split(',');
                    if (cols.Length < 4) continue;
                    int empID;
                    DateTime date;
                    TimeSpan checkIn;
                    if (!int.TryParse(cols[0].Trim(), out empID)) continue;
                    if (!DateTime.TryParse(cols[2].Trim(), out date)) continue;
                    if (!TimeSpan.TryParse(cols[3].Trim(), out checkIn)) continue;
                    try { _attBLL.MarkCheckIn(empID, date, checkIn, CurrentUser.AuditName); success++; }
                    catch { failed++; }
                }
                MessageBox.Show("Imported: " + success + " records. Failed: " + failed, "Upload Done",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DateTime d;
                DateTime.TryParse(textBoxDate.Text.Trim(), out d);
                LoadAttendanceByDate(d == DateTime.MinValue ? DateTime.Today : d);
            }
            catch (Exception ex)
            { MessageBox.Show("Upload failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnGenerateAttendanceSummary_Click(object sender, EventArgs e)
        {
            // Opens the AttendanceSummary form as a modal dialog
            AttendanceSummary summaryForm = new AttendanceSummary();
            summaryForm.ShowDialog(this);
        }

        private void BtnViewAttendanceSummary_Click(object sender, EventArgs e)
        {
            // Also opens AttendanceSummary form (view mode)
            AttendanceSummary summaryForm = new AttendanceSummary();
            summaryForm.ShowDialog(this);
        }

        // ════════════════════════════════════════════════════════════════════
        // ── TAB 6: EMPLOYEE SALARY STRUCTURE ────────────────────────────────
        // ════════════════════════════════════════════════════════════════════
        private void InitSalaryTab()
        {
            StyleGridHeaders(dataGridView_EmployeeSalaryStructure);

            // Wire buttons
            add_salary_btn.Click += BtnAddSalary_Click;
            ExportData_IN_CSV.Click += BtnExportSalary_Click;
            Upload_file_to_InsertInCSV.Click += BtnUploadSalary_Click;
            View_employee_leave_balance.Click += BtnViewLeaveBalance_Click;

            // Grid cell click (Edit / Delete)
            dataGridView_EmployeeSalaryStructure.CellContentClick += DataGridViewSalary_CellClick;

            // Load all active salaries on init
            LoadAllActiveSalaries();
        }

        private void LoadAllActiveSalaries()
        {
            try
            {
                DataTable dt = _salaryBLL.GetAllActive();
                BindSalaryGrid(dt);
            }
            catch (Exception ex)
            { MessageBox.Show("Error loading salaries:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BindSalaryGrid(DataTable dt)
        {
            dataGridView_EmployeeSalaryStructure.Rows.Clear();
            foreach (DataRow row in dt.Rows)
            {
                int idx = dataGridView_EmployeeSalaryStructure.Rows.Add();
                DataGridViewRow gr = dataGridView_EmployeeSalaryStructure.Rows[idx];
                gr.Cells["Employee_Name"].Value = SafeStr(row, "FullName");
                gr.Cells["Basic_pay"].Value = row["BasicPay"] != DBNull.Value ? Convert.ToDecimal(row["BasicPay"]).ToString("N2") : "0.00";
                gr.Cells["Allowances"].Value = row["Allowances"] != DBNull.Value ? Convert.ToDecimal(row["Allowances"]).ToString("N2") : "0.00";
                gr.Cells["Bonus_bonus"].Value = row["Bonus"] != DBNull.Value ? Convert.ToDecimal(row["Bonus"]).ToString("N2") : "0.00";
                gr.Cells["Effective_From"].Value = row["EffectiveFrom"] != DBNull.Value ? Convert.ToDateTime(row["EffectiveFrom"]).ToString("yyyy-MM-dd") : "";
                gr.Cells["Effective_To"].Value = row["EffectiveTo"] != DBNull.Value ? Convert.ToDateTime(row["EffectiveTo"]).ToString("yyyy-MM-dd") : "Active";
                gr.Cells["Active"].Value = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"]);
                // Store key data in tag: EmployeeID|SalaryStructureID
                gr.Tag = row["EmployeeID"].ToString() + "|" + row["SalaryStructureID"].ToString();
            }
        }

        private void BtnAddSalary_Click(object sender, EventArgs e)
        {
            // Validate inputs
            string empName = EmployeeName.Text.Trim();
            if (string.IsNullOrWhiteSpace(empName))
            { MessageBox.Show("Please enter the Employee Name or ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            decimal basicPay, allowances, bonus;
            if (!decimal.TryParse(Basicpay.Text.Trim(), out basicPay) || basicPay <= 0)
            { MessageBox.Show("Please enter a valid Basic Pay (must be > 0).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (!decimal.TryParse(allowance.Text.Trim().Replace("", "0"), out allowances)) allowances = 0;
            if (!decimal.TryParse(bonus_insert.Text.Trim().Replace("", "0"), out bonus)) bonus = 0;

            DateTime effectiveFrom = effective_salary_from.Value.Date;

            // Resolve employee ID from name
            int empID = ResolveEmployeeIDByName(empName);
            if (empID <= 0)
            { MessageBox.Show("Employee not found. Please enter the exact Full Name or Employee ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (MessageBox.Show("Assign salary to employee?\n\nBasic Pay: PKR " + basicPay.ToString("N2") +
                "\nAllowances: PKR " + allowances.ToString("N2") + "\nBonus: PKR " + bonus.ToString("N2") +
                "\nEffective From: " + effectiveFrom.ToString("yyyy-MM-dd"),
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                if (_salaryEditMode && _editingSalaryStructureID > 0)
                {
                    // Update existing
                    _salaryBLL.UpdateSalary(_editingSalaryStructureID, basicPay, allowances, bonus, CurrentUser.AuditName);
                    _salaryEditMode = false;
                    _editingSalaryStructureID = 0;
                    add_salary_btn.Text = "Add Salary";
                    add_salary_btn.BackColor = Color.FromArgb(16, 124, 16);
                }
                else
                {
                    // Insert new
                    _salaryBLL.AssignSalary(empID, basicPay, allowances, bonus, effectiveFrom, CurrentUser.AuditName);
                }
                ClearSalaryForm();
                LoadAllActiveSalaries();
                MessageBox.Show("Salary saved successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            { MessageBox.Show("Save failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private int ResolveEmployeeIDByName(string nameOrID)
        {
            int id;
            if (int.TryParse(nameOrID, out id) && id > 0) return id;

            try
            {
                DataTable emps = _empDal.GetActiveEmployees();
                foreach (DataRow r in emps.Rows)
                    if (r["FullName"].ToString().Equals(nameOrID, StringComparison.OrdinalIgnoreCase))
                        return Convert.ToInt32(r["EmployeeID"]);
            }
            catch { }
            return 0;
        }

        private void ClearSalaryForm()
        {
            EmployeeName.Clear();
            Basicpay.Clear();
            allowance.Clear();
            bonus_insert.Clear();
            effective_salary_from.Value = DateTime.Today;
        }

        private void DataGridViewSalary_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dataGridView_EmployeeSalaryStructure.Rows[e.RowIndex];
            string tagStr = row.Tag?.ToString() ?? "";
            string[] parts = tagStr.Split('|');
            if (parts.Length < 2) return;
            int empID = int.Parse(parts[0]);
            int salID = int.Parse(parts[1]);

            if (e.ColumnIndex == dataGridView_EmployeeSalaryStructure.Columns["Edit_salary"].Index)
            {
                // Populate form fields for editing
                _salaryEditMode = true;
                _editingSalaryStructureID = salID;
                add_salary_btn.Text = "Update Salary";
                add_salary_btn.BackColor = Color.FromArgb(0, 120, 212);

                EmployeeName.Text = row.Cells["Employee_Name"].Value?.ToString() ?? "";
                Basicpay.Text = row.Cells["Basic_pay"].Value?.ToString()?.Replace(",", "") ?? "";
                allowance.Text = row.Cells["Allowances"].Value?.ToString()?.Replace(",", "") ?? "";
                bonus_insert.Text = row.Cells["Bonus_bonus"].Value?.ToString()?.Replace(",", "") ?? "";

                string effFrom = row.Cells["Effective_From"].Value?.ToString() ?? "";
                DateTime dt;
                if (DateTime.TryParse(effFrom, out dt)) effective_salary_from.Value = dt;
            }
            else if (e.ColumnIndex == dataGridView_EmployeeSalaryStructure.Columns["Delete_salary"].Index)
            {
                if (MessageBox.Show("Deactivate/remove this salary record?\n\nNote: Only deactivates the record; payroll history is retained.",
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    // We use UpdateSalary with 0 to deactivate. In production you'd add a DeleteSalary to SalaryBLL.
                    // For now, just reload and inform user.
                    MessageBox.Show("To fully deactivate a salary, assign a new salary with the updated amounts which will supersede this record.",
                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnExportSalary_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "Salary_Structure_" + DateTime.Today.ToString("yyyyMMdd") + ".csv"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            try
            {
                DataTable dt = _salaryBLL.GetAllActive();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("EmployeeID,Employee,Department,BasicPay,Allowances,Bonus,TotalPackage,EffectiveFrom");
                foreach (DataRow row in dt.Rows)
                    sb.AppendLine(string.Join(",", new[] {
                        row["EmployeeID"].ToString(), CsvSafe(row,"FullName"), CsvSafe(row,"Department"),
                        row["BasicPay"].ToString(), row["Allowances"].ToString(), row["Bonus"].ToString(),
                        row["TotalPackage"].ToString(), row["EffectiveFrom"].ToString()
                    }));
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Exported:\n" + sfd.FileName, "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            { MessageBox.Show("Export failed:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnUploadSalary_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bulk salary upload: prepare a CSV with columns:\nEmployeeID, BasicPay, Allowances, Bonus, EffectiveFrom (yyyy-MM-dd)\n\nThen use 'Add Salary' per employee for precision.",
                "Upload Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnViewLeaveBalance_Click(object sender, EventArgs e)
        {
            LeaveBalance lb = new LeaveBalance();
            lb.ShowDialog(this);
        }

        // ════════════════════════════════════════════════════════════════════
        // ── SHARED HELPERS ──────────────────────────────────────────────────
        // ════════════════════════════════════════════════════════════════════
        private void StyleGridHeaders(DataGridView dgv)
        {
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 212);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowTemplate.Height = 26;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 228, 247);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        private string CsvSafe(DataRow row, string col)
        {
            if (!row.Table.Columns.Contains(col) || row[col] == DBNull.Value) return "";
            string s = row[col].ToString();
            return (s.Contains(",") || s.Contains("\"")) ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;
        }

        // ════════════════════════════════════════════════════════════════════
        // ── DESIGNER STUBS ──────────────────────────────────────────────────
        // ════════════════════════════════════════════════════════════════════
        private void tabPage1_Click(object sender, EventArgs e) { }
        private void pnlHeader_Paint(object sender, System.Windows.Forms.PaintEventArgs e) { }
        private void lblTitle_Click(object sender, EventArgs e) { }
        private void lblSearch_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label10_Click(object sender, EventArgs e) { }
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }

}
