namespace HRMS_ERP.Forms
{
    partial class AttendanceSummary
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView_attendancesummary = new System.Windows.Forms.DataGridView();
            this.Employee_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Casual_Leave_Taken = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Annaul_leave_Taken = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sick_leave_Taken = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Leave_without_pay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Leave_deducted_from_late = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Extra_Unpaid_from_late = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Edit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.button_refresh = new System.Windows.Forms.Button();
            this.comboBox_employee = new System.Windows.Forms.ComboBox();
            this.button_loadData = new System.Windows.Forms.Button();
            this.comboBox_year = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.comboBox_month = new System.Windows.Forms.ComboBox();
            this.label25 = new System.Windows.Forms.Label();
            this.generate_finance_report_of_tax = new System.Windows.Forms.Button();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.Generate_summaryOfattendance = new System.Windows.Forms.Button();
            this.Regenerate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_attendancesummary)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView_attendancesummary
            // 
            this.dataGridView_attendancesummary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_attendancesummary.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_attendancesummary.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridView_attendancesummary.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_attendancesummary.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridView_attendancesummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_attendancesummary.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Employee_Name,
            this.Casual_Leave_Taken,
            this.Annaul_leave_Taken,
            this.Sick_leave_Taken,
            this.Leave_without_pay,
            this.Leave_deducted_from_late,
            this.Extra_Unpaid_from_late,
            this.Edit,
            this.Delete});
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_attendancesummary.DefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridView_attendancesummary.Location = new System.Drawing.Point(9, 183);
            this.dataGridView_attendancesummary.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView_attendancesummary.Name = "dataGridView_attendancesummary";
            this.dataGridView_attendancesummary.ReadOnly = true;
            this.dataGridView_attendancesummary.Size = new System.Drawing.Size(1368, 482);
            this.dataGridView_attendancesummary.TabIndex = 52;
            this.dataGridView_attendancesummary.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_attendancesummary_CellContentClick);
            // 
            // Employee_Name
            // 
            this.Employee_Name.HeaderText = "Employee Name";
            this.Employee_Name.Name = "Employee_Name";
            this.Employee_Name.ReadOnly = true;
            // 
            // Casual_Leave_Taken
            // 
            this.Casual_Leave_Taken.HeaderText = "Casual Leave Taken";
            this.Casual_Leave_Taken.Name = "Casual_Leave_Taken";
            this.Casual_Leave_Taken.ReadOnly = true;
            // 
            // Annaul_leave_Taken
            // 
            this.Annaul_leave_Taken.HeaderText = "Annaul leave Taken";
            this.Annaul_leave_Taken.Name = "Annaul_leave_Taken";
            this.Annaul_leave_Taken.ReadOnly = true;
            // 
            // Sick_leave_Taken
            // 
            this.Sick_leave_Taken.HeaderText = "Sick leave Taken";
            this.Sick_leave_Taken.Name = "Sick_leave_Taken";
            this.Sick_leave_Taken.ReadOnly = true;
            // 
            // Leave_without_pay
            // 
            this.Leave_without_pay.HeaderText = "Leave without pay";
            this.Leave_without_pay.Name = "Leave_without_pay";
            this.Leave_without_pay.ReadOnly = true;
            // 
            // Leave_deducted_from_late
            // 
            this.Leave_deducted_from_late.HeaderText = "Leave deducted from late";
            this.Leave_deducted_from_late.Name = "Leave_deducted_from_late";
            this.Leave_deducted_from_late.ReadOnly = true;
            // 
            // Extra_Unpaid_from_late
            // 
            this.Extra_Unpaid_from_late.HeaderText = "Extra Unpaid from late";
            this.Extra_Unpaid_from_late.Name = "Extra_Unpaid_from_late";
            this.Extra_Unpaid_from_late.ReadOnly = true;
            // 
            // Edit
            // 
            this.Edit.HeaderText = "Edit";
            this.Edit.Name = "Edit";
            this.Edit.ReadOnly = true;
            // 
            // Delete
            // 
            this.Delete.HeaderText = "Delete";
            this.Delete.Name = "Delete";
            this.Delete.ReadOnly = true;
            // 
            // button_refresh
            // 
            this.button_refresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.button_refresh.FlatAppearance.BorderSize = 0;
            this.button_refresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(177)))));
            this.button_refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_refresh.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_refresh.ForeColor = System.Drawing.Color.White;
            this.button_refresh.Location = new System.Drawing.Point(813, 95);
            this.button_refresh.Name = "button_refresh";
            this.button_refresh.Size = new System.Drawing.Size(140, 38);
            this.button_refresh.TabIndex = 148;
            this.button_refresh.Text = "Refresh";
            this.button_refresh.UseVisualStyleBackColor = false;
            this.button_refresh.Click += new System.EventHandler(this.button_refresh_Click);
            // 
            // comboBox_employee
            // 
            this.comboBox_employee.FormattingEnabled = true;
            this.comboBox_employee.Location = new System.Drawing.Point(119, 109);
            this.comboBox_employee.Name = "comboBox_employee";
            this.comboBox_employee.Size = new System.Drawing.Size(100, 24);
            this.comboBox_employee.TabIndex = 147;
            this.comboBox_employee.SelectedIndexChanged += new System.EventHandler(this.comboBox_employee_SelectedIndexChanged);
            // 
            // button_loadData
            // 
            this.button_loadData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.button_loadData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_loadData.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_loadData.ForeColor = System.Drawing.Color.White;
            this.button_loadData.Location = new System.Drawing.Point(640, 96);
            this.button_loadData.Margin = new System.Windows.Forms.Padding(4);
            this.button_loadData.Name = "button_loadData";
            this.button_loadData.Size = new System.Drawing.Size(141, 37);
            this.button_loadData.TabIndex = 146;
            this.button_loadData.Text = "Load Data";
            this.button_loadData.UseVisualStyleBackColor = false;
            this.button_loadData.Click += new System.EventHandler(this.button_loadData_Click);
            // 
            // comboBox_year
            // 
            this.comboBox_year.FormattingEnabled = true;
            this.comboBox_year.Location = new System.Drawing.Point(494, 106);
            this.comboBox_year.Name = "comboBox_year";
            this.comboBox_year.Size = new System.Drawing.Size(115, 24);
            this.comboBox_year.TabIndex = 145;
            this.comboBox_year.SelectedIndexChanged += new System.EventHandler(this.comboBox_year_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.Black;
            this.label13.Location = new System.Drawing.Point(449, 106);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 20);
            this.label13.TabIndex = 144;
            this.label13.Text = "Year:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(8, 146);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(1560, 17);
            this.label20.TabIndex = 143;
            this.label20.Text = "_________________________________________________________________________________" +
    "________________________________________________________________________________" +
    "_________________________________\r\n";
            // 
            // comboBox_month
            // 
            this.comboBox_month.FormattingEnabled = true;
            this.comboBox_month.Location = new System.Drawing.Point(300, 106);
            this.comboBox_month.Name = "comboBox_month";
            this.comboBox_month.Size = new System.Drawing.Size(115, 24);
            this.comboBox_month.TabIndex = 142;
            this.comboBox_month.SelectedIndexChanged += new System.EventHandler(this.comboBox_month_SelectedIndexChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.Color.Black;
            this.label25.Location = new System.Drawing.Point(238, 110);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(60, 20);
            this.label25.TabIndex = 141;
            this.label25.Text = "Month:";
            // 
            // generate_finance_report_of_tax
            // 
            this.generate_finance_report_of_tax.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.generate_finance_report_of_tax.BackColor = System.Drawing.Color.Gold;
            this.generate_finance_report_of_tax.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.generate_finance_report_of_tax.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generate_finance_report_of_tax.ForeColor = System.Drawing.Color.Black;
            this.generate_finance_report_of_tax.Location = new System.Drawing.Point(1072, 13);
            this.generate_finance_report_of_tax.Margin = new System.Windows.Forms.Padding(4);
            this.generate_finance_report_of_tax.Name = "generate_finance_report_of_tax";
            this.generate_finance_report_of_tax.Size = new System.Drawing.Size(267, 52);
            this.generate_finance_report_of_tax.TabIndex = 139;
            this.generate_finance_report_of_tax.Text = "Export to CSV";
            this.generate_finance_report_of_tax.UseVisualStyleBackColor = false;
            this.generate_finance_report_of_tax.Click += new System.EventHandler(this.generate_finance_report_of_tax_Click);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.ForeColor = System.Drawing.Color.Black;
            this.label26.Location = new System.Drawing.Point(47, 26);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(262, 32);
            this.label26.TabIndex = 138;
            this.label26.Text = "Attendance Summary";
            this.label26.Click += new System.EventHandler(this.label26_Click);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(13, 58);
            this.label27.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(1560, 17);
            this.label27.TabIndex = 137;
            this.label27.Text = "_________________________________________________________________________________" +
    "________________________________________________________________________________" +
    "_________________________________\r\n";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(21, 113);
            this.label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(81, 20);
            this.label28.TabIndex = 136;
            this.label28.Text = "Employee:";
            // 
            // Generate_summaryOfattendance
            // 
            this.Generate_summaryOfattendance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(124)))), ((int)(((byte)(16)))));
            this.Generate_summaryOfattendance.FlatAppearance.BorderSize = 0;
            this.Generate_summaryOfattendance.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(177)))));
            this.Generate_summaryOfattendance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Generate_summaryOfattendance.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Generate_summaryOfattendance.ForeColor = System.Drawing.Color.White;
            this.Generate_summaryOfattendance.Location = new System.Drawing.Point(1002, 96);
            this.Generate_summaryOfattendance.Name = "Generate_summaryOfattendance";
            this.Generate_summaryOfattendance.Size = new System.Drawing.Size(140, 38);
            this.Generate_summaryOfattendance.TabIndex = 149;
            this.Generate_summaryOfattendance.Text = "Generate";
            this.Generate_summaryOfattendance.UseVisualStyleBackColor = false;
            this.Generate_summaryOfattendance.Click += new System.EventHandler(this.Generate_summaryOfattendance_Click);
            // 
            // Regenerate
            // 
            this.Regenerate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(124)))), ((int)(((byte)(16)))));
            this.Regenerate.FlatAppearance.BorderSize = 0;
            this.Regenerate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(177)))));
            this.Regenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Regenerate.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Regenerate.ForeColor = System.Drawing.Color.White;
            this.Regenerate.Location = new System.Drawing.Point(1189, 96);
            this.Regenerate.Name = "Regenerate";
            this.Regenerate.Size = new System.Drawing.Size(140, 38);
            this.Regenerate.TabIndex = 150;
            this.Regenerate.Text = "Regenerate";
            this.Regenerate.UseVisualStyleBackColor = false;
            this.Regenerate.Click += new System.EventHandler(this.Regenerate_Click);
            // 
            // AttendanceSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1386, 675);
            this.Controls.Add(this.Regenerate);
            this.Controls.Add(this.Generate_summaryOfattendance);
            this.Controls.Add(this.button_refresh);
            this.Controls.Add(this.comboBox_employee);
            this.Controls.Add(this.button_loadData);
            this.Controls.Add(this.comboBox_year);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.comboBox_month);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.generate_finance_report_of_tax);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.dataGridView_attendancesummary);
            this.Name = "AttendanceSummary";
            this.Text = "AttendanceSummary";
            this.Load += new System.EventHandler(this.AttendanceSummary_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_attendancesummary)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView_attendancesummary;
        private System.Windows.Forms.DataGridViewTextBoxColumn Employee_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Casual_Leave_Taken;
        private System.Windows.Forms.DataGridViewTextBoxColumn Annaul_leave_Taken;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sick_leave_Taken;
        private System.Windows.Forms.DataGridViewTextBoxColumn Leave_without_pay;
        private System.Windows.Forms.DataGridViewTextBoxColumn Leave_deducted_from_late;
        private System.Windows.Forms.DataGridViewTextBoxColumn Extra_Unpaid_from_late;
        private System.Windows.Forms.DataGridViewButtonColumn Edit;
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
        private System.Windows.Forms.Button button_refresh;
        private System.Windows.Forms.ComboBox comboBox_employee;
        private System.Windows.Forms.Button button_loadData;
        private System.Windows.Forms.ComboBox comboBox_year;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox comboBox_month;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button generate_finance_report_of_tax;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Button Generate_summaryOfattendance;
        private System.Windows.Forms.Button Regenerate;
    }
}