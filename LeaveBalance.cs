using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HRMS_ERP.BusinessLogic;
using HRMS_ERP.DataAccess;

namespace HRMS_ERP.Forms
{
    public partial class LeaveBalance : Form
    {
        private readonly LeaveBLL _leaveBLL = new LeaveBLL();
        private readonly EmployeeDAL _empDal = new EmployeeDAL();

        private DataTable _cache;

        // Inline-edit state
        private int _editEmpID = 0;
        private int _editYear = 0;
        private int _editRow = -1;

        // Danger thresholds
        private const int DANGER_CL = 2, DANGER_SL = 1, DANGER_AL = 5;

        public LeaveBalance() { InitializeComponent(); }

        // ── Form Load ────────────────────────────────────────────────────────
        private void LeaveBalance_Load(object sender, EventArgs e)
        {
            // Style grid to match Employee Master
            dataGridViewleaveBalance.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 212);
            dataGridViewleaveBalance.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewleaveBalance.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dataGridViewleaveBalance.EnableHeadersVisualStyles = false;
            dataGridViewleaveBalance.RowTemplate.Height = 27;
            dataGridViewleaveBalance.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 228, 247);
            dataGridViewleaveBalance.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridViewleaveBalance.ReadOnly = false;
            dataGridViewleaveBalance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewleaveBalance.AllowUserToAddRows = false;
            dataGridViewleaveBalance.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            // Style Edit button column (blue) — same as Employee Master Edit column
            if (dataGridViewleaveBalance.Columns.Contains("Edit"))
            {
                var ec = (DataGridViewButtonColumn)dataGridViewleaveBalance.Columns["Edit"];
                ec.DefaultCellStyle.BackColor = Color.FromArgb(0, 120, 212);
                ec.DefaultCellStyle.ForeColor = Color.White;
                ec.FlatStyle = FlatStyle.Flat;
                ec.ReadOnly = false;
            }
            // Style Delete button column (red)
            if (dataGridViewleaveBalance.Columns.Contains("Delete"))
            {
                var dc = (DataGridViewButtonColumn)dataGridViewleaveBalance.Columns["Delete"];
                dc.DefaultCellStyle.BackColor = Color.FromArgb(168, 0, 0);
                dc.DefaultCellStyle.ForeColor = Color.White;
                dc.FlatStyle = FlatStyle.Flat;
                dc.ReadOnly = false;
            }

            // Repurpose existing designer controls:
            // filter_by_remaining_leaves (ComboBox at top) → year selector
            // btnupdateleavebalance (green button)         → Refresh/Export now wired below
            // searchLeavebalncegride (TextBox)             → search by name
            // label34 "Year"                               → already says Year
            // label33 "Search"                             → already says Search

            // Populate year combo
            filter_by_remaining_leaves.Items.Clear();
            filter_by_remaining_leaves.Items.Add("⚠ Danger Zone");
            filter_by_remaining_leaves.Items.Add("All Employees");
            for (int yr = DateTime.Today.Year - 3; yr <= DateTime.Today.Year + 1; yr++)
                filter_by_remaining_leaves.Items.Add(yr.ToString());
            filter_by_remaining_leaves.SelectedItem = DateTime.Today.Year.ToString();

            // Repurpose btnupdateleavebalance as "Refresh + Export" via context:
            // We use it as Refresh (green = load/refresh)
            btnupdateleavebalance.Text = "Refresh";

            // Wire events
            filter_by_remaining_leaves.SelectedIndexChanged += (s, ev) => ApplyFilter();
            searchLeavebalncegride.TextChanged += (s, ev) => ApplyFilter();
            btnupdateleavebalance.Click += (s, ev) => LoadAllBalances(SelectedYear());
            dataGridViewleaveBalance.CellContentClick += Grid_CellContentClick;
            dataGridViewleaveBalance.CellEndEdit += Grid_CellEndEdit;
            dataGridViewleaveBalance.KeyDown += Grid_KeyDown;

            LoadAllBalances(DateTime.Today.Year);
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        private int SelectedYear()
        {
            int yr;
            if (filter_by_remaining_leaves.SelectedItem == null) return DateTime.Today.Year;
            return int.TryParse(filter_by_remaining_leaves.SelectedItem.ToString(), out yr)
                ? yr : DateTime.Today.Year;
        }

        private void LoadAllBalances(int year)
        {
            try { _cache = _leaveBLL.GetAllForYear(year); ApplyFilter(); }
            catch (Exception ex)
            { MessageBox.Show("Error loading leave balances:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ApplyFilter()
        {
            if (_cache == null) return;
            string kw = searchLeavebalncegride.Text.Trim().ToLower();
            bool dangerOnly = filter_by_remaining_leaves.SelectedItem?.ToString() == "⚠ Danger Zone";

            DataTable filtered = _cache.Clone();
            foreach (DataRow row in _cache.Rows)
            {
                if (!string.IsNullOrEmpty(kw))
                {
                    string name = row["FullName"] != DBNull.Value ? row["FullName"].ToString().ToLower() : "";
                    string dept = _cache.Columns.Contains("Department") && row["Department"] != DBNull.Value
                        ? row["Department"].ToString().ToLower() : "";
                    if (!name.Contains(kw) && !dept.Contains(kw)) continue;
                }
                if (dangerOnly)
                {
                    int cl = row["CasualLeaveRemaining"] != DBNull.Value ? Convert.ToInt32(row["CasualLeaveRemaining"]) : 10;
                    int sl = row["SickLeaveRemaining"] != DBNull.Value ? Convert.ToInt32(row["SickLeaveRemaining"]) : 8;
                    int al = row["AnnualLeaveRemaining"] != DBNull.Value ? Convert.ToInt32(row["AnnualLeaveRemaining"]) : 30;
                    if (cl > DANGER_CL && sl > DANGER_SL && al > DANGER_AL) continue;
                }
                filtered.ImportRow(row);
            }
            BindGrid(filtered);
        }

        private void BindGrid(DataTable dt)
        {
            if (_editRow >= 0) { dataGridViewleaveBalance.CancelEdit(); _editRow = -1; }
            dataGridViewleaveBalance.Rows.Clear();
            foreach (DataRow row in dt.Rows)
            {
                int idx = dataGridViewleaveBalance.Rows.Add();
                DataGridViewRow gr = dataGridViewleaveBalance.Rows[idx];

                string dept = _cache.Columns.Contains("Department") && row["Department"] != DBNull.Value
                    ? " (" + row["Department"] + ")" : "";
                gr.Cells["Employee"].Value = (row["FullName"] != DBNull.Value ? row["FullName"].ToString() : "") + dept;
                gr.Cells["Casual_Leave_Remaining"].Value = row["CasualLeaveRemaining"] != DBNull.Value ? row["CasualLeaveRemaining"].ToString() : "10";
                gr.Cells["Annaul_leave_Remaining"].Value = row["AnnualLeaveRemaining"] != DBNull.Value ? row["AnnualLeaveRemaining"].ToString() : "30";
                gr.Cells["Sick_leave_Remaining"].Value = row["SickLeaveRemaining"] != DBNull.Value ? row["SickLeaveRemaining"].ToString() : "8";
                gr.Tag = row["EmployeeID"].ToString() + "|" + row["Year"].ToString();

                // Lock display-only cells
                gr.Cells["Employee"].ReadOnly = true;
                if (dataGridViewleaveBalance.Columns.Contains("Edit")) gr.Cells["Edit"].ReadOnly = false;
                if (dataGridViewleaveBalance.Columns.Contains("Delete")) gr.Cells["Delete"].ReadOnly = false;

                // Colour coding — same palette as Employee Master danger/warning
                int cl2 = row["CasualLeaveRemaining"] != DBNull.Value ? Convert.ToInt32(row["CasualLeaveRemaining"]) : 10;
                int sl2 = row["SickLeaveRemaining"] != DBNull.Value ? Convert.ToInt32(row["SickLeaveRemaining"]) : 8;
                int al2 = row["AnnualLeaveRemaining"] != DBNull.Value ? Convert.ToInt32(row["AnnualLeaveRemaining"]) : 30;

                if (cl2 == 0 && sl2 == 0 && al2 == 0)
                {
                    // All exhausted → red (matches inactive employee style in Employee Master)
                    gr.DefaultCellStyle.BackColor = Color.FromArgb(255, 199, 206);
                    gr.DefaultCellStyle.ForeColor = Color.DarkRed;
                    gr.DefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                }
                else if (cl2 <= DANGER_CL || sl2 <= DANGER_SL || al2 <= DANGER_AL)
                {
                    // Running low → orange/amber
                    gr.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 178);
                    gr.DefaultCellStyle.ForeColor = Color.FromArgb(128, 64, 0);
                }
                else
                {
                    gr.DefaultCellStyle.BackColor = Color.White;
                    gr.DefaultCellStyle.ForeColor = Color.Black;
                    gr.DefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
                }
            }
        }

        // ── Inline Edit ──────────────────────────────────────────────────────
        private void StartEdit(int rowIndex)
        {
            if (!CurrentUser.IsAdmin)
            { MessageBox.Show("Only Admin users can adjust leave balances.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (_editRow >= 0 && _editRow != rowIndex) CommitEdit();
            _editRow = rowIndex;
            DataGridViewRow gr = dataGridViewleaveBalance.Rows[rowIndex];
            string[] parts = (gr.Tag?.ToString() ?? "0|0").Split('|');
            int.TryParse(parts[0], out _editEmpID);
            int.TryParse(parts.Length > 1 ? parts[1] : "0", out _editYear);

            gr.Cells["Casual_Leave_Remaining"].ReadOnly = false;
            gr.Cells["Sick_leave_Remaining"].ReadOnly = false;
            gr.Cells["Annaul_leave_Remaining"].ReadOnly = false;
            gr.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 204);
            gr.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewleaveBalance.CurrentCell = gr.Cells["Casual_Leave_Remaining"];
            dataGridViewleaveBalance.BeginEdit(true);
        }

        private void CommitEdit()
        {
            if (_editRow < 0) return;
            dataGridViewleaveBalance.EndEdit();
            DataGridViewRow gr = dataGridViewleaveBalance.Rows[_editRow];
            int cl, sl, al;
            if (!int.TryParse(gr.Cells["Casual_Leave_Remaining"].Value?.ToString() ?? "", out cl) ||
                !int.TryParse(gr.Cells["Sick_leave_Remaining"].Value?.ToString() ?? "", out sl) ||
                !int.TryParse(gr.Cells["Annaul_leave_Remaining"].Value?.ToString() ?? "", out al))
            { MessageBox.Show("Enter valid whole numbers for all leave fields.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                _leaveBLL.AdjustBalance(_editEmpID, _editYear, cl, sl, al, CurrentUser.AuditName);
                int yr = _editYear;
                _editRow = -1; _editEmpID = 0; _editYear = 0;
                LoadAllBalances(yr);
                MessageBox.Show("Leave balance updated.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ArgumentException ax) { MessageBox.Show(ax.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            catch (Exception ex) { MessageBox.Show("Update failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ── Grid Events ──────────────────────────────────────────────────────
        private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow gr = dataGridViewleaveBalance.Rows[e.RowIndex];
            string[] parts = (gr.Tag?.ToString() ?? "0|0").Split('|');
            int empID = 0, year = 0;
            int.TryParse(parts[0], out empID);
            int.TryParse(parts.Length > 1 ? parts[1] : "0", out year);

            if (dataGridViewleaveBalance.Columns.Contains("Edit") &&
                e.ColumnIndex == dataGridViewleaveBalance.Columns["Edit"].Index)
            {
                StartEdit(e.RowIndex);
            }
            else if (dataGridViewleaveBalance.Columns.Contains("Delete") &&
                     e.ColumnIndex == dataGridViewleaveBalance.Columns["Delete"].Index)
            {
                if (!CurrentUser.IsAdmin)
                { MessageBox.Show("Only Admin users can reset leave balances.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                string empName = gr.Cells["Employee"].Value?.ToString() ?? "";
                if (MessageBox.Show("Reset leave balance for:\n\n  " + empName +
                    "\n\nSets CL=10, SL=8, AL=30 for year " + year + ".\nThis affects the database.",
                    "Confirm Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        _leaveBLL.AdjustBalance(empID, year, 10, 8, 30, CurrentUser.AuditName); LoadAllBalances(year);
                        MessageBox.Show("Leave balance reset to defaults.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    { MessageBox.Show("Reset failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void Grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        { if (e.RowIndex == _editRow) CommitEdit(); }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && _editRow >= 0) { CommitEdit(); e.Handled = true; }
            if (e.KeyCode == Keys.Escape && _editRow >= 0)
            { dataGridViewleaveBalance.CancelEdit(); _editRow = -1; LoadAllBalances(SelectedYear()); e.Handled = true; }
        }
    }
}
