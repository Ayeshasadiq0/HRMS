namespace HRMS_ERP.Forms
{
    partial class LeaveBalance
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewleaveBalance = new System.Windows.Forms.DataGridView();
            this.Employee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Casual_Leave_Remaining = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Annaul_leave_Remaining = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sick_leave_Remaining = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Edit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.label33 = new System.Windows.Forms.Label();
            this.searchLeavebalncegride = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.filter_by_remaining_leaves = new System.Windows.Forms.ComboBox();
            this.btnupdateleavebalance = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewleaveBalance)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewleaveBalance
            // 
            this.dataGridViewleaveBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewleaveBalance.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewleaveBalance.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridViewleaveBalance.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewleaveBalance.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewleaveBalance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewleaveBalance.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Employee,
            this.Casual_Leave_Remaining,
            this.Annaul_leave_Remaining,
            this.Sick_leave_Remaining,
            this.Edit,
            this.Delete});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewleaveBalance.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewleaveBalance.Location = new System.Drawing.Point(13, 123);
            this.dataGridViewleaveBalance.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridViewleaveBalance.Name = "dataGridViewleaveBalance";
            this.dataGridViewleaveBalance.ReadOnly = true;
            this.dataGridViewleaveBalance.Size = new System.Drawing.Size(1121, 498);
            this.dataGridViewleaveBalance.TabIndex = 31;
            // 
            // Employee
            // 
            this.Employee.HeaderText = "Employee";
            this.Employee.Name = "Employee";
            this.Employee.ReadOnly = true;
            // 
            // Casual_Leave_Remaining
            // 
            this.Casual_Leave_Remaining.HeaderText = "Casual_Leave_Remaining";
            this.Casual_Leave_Remaining.Name = "Casual_Leave_Remaining";
            this.Casual_Leave_Remaining.ReadOnly = true;
            // 
            // Annaul_leave_Remaining
            // 
            this.Annaul_leave_Remaining.HeaderText = "Annaul_leave_Remaining";
            this.Annaul_leave_Remaining.Name = "Annaul_leave_Remaining";
            this.Annaul_leave_Remaining.ReadOnly = true;
            // 
            // Sick_leave_Remaining
            // 
            this.Sick_leave_Remaining.HeaderText = "Sick_leave_Remaining";
            this.Sick_leave_Remaining.Name = "Sick_leave_Remaining";
            this.Sick_leave_Remaining.ReadOnly = true;
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
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label33.Location = new System.Drawing.Point(388, 48);
            this.label33.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(89, 29);
            this.label33.TabIndex = 50;
            this.label33.Text = "Search";
            // 
            // searchLeavebalncegride
            // 
            this.searchLeavebalncegride.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchLeavebalncegride.Location = new System.Drawing.Point(512, 48);
            this.searchLeavebalncegride.Margin = new System.Windows.Forms.Padding(4);
            this.searchLeavebalncegride.Name = "searchLeavebalncegride";
            this.searchLeavebalncegride.Size = new System.Drawing.Size(209, 30);
            this.searchLeavebalncegride.TabIndex = 49;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label34.Location = new System.Drawing.Point(22, 48);
            this.label34.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(64, 29);
            this.label34.TabIndex = 48;
            this.label34.Text = "Year";
            // 
            // filter_by_remaining_leaves
            // 
            this.filter_by_remaining_leaves.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filter_by_remaining_leaves.FormattingEnabled = true;
            this.filter_by_remaining_leaves.Items.AddRange(new object[] {
            "Process",
            "Resolved",
            "Rejected",
            "P.R Process"});
            this.filter_by_remaining_leaves.Location = new System.Drawing.Point(117, 40);
            this.filter_by_remaining_leaves.Margin = new System.Windows.Forms.Padding(4);
            this.filter_by_remaining_leaves.Name = "filter_by_remaining_leaves";
            this.filter_by_remaining_leaves.Size = new System.Drawing.Size(201, 37);
            this.filter_by_remaining_leaves.TabIndex = 47;
            // 
            // btnupdateleavebalance
            // 
            this.btnupdateleavebalance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(124)))), ((int)(((byte)(16)))));
            this.btnupdateleavebalance.FlatAppearance.BorderSize = 0;
            this.btnupdateleavebalance.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(110)))), ((int)(((byte)(14)))));
            this.btnupdateleavebalance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnupdateleavebalance.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnupdateleavebalance.ForeColor = System.Drawing.Color.White;
            this.btnupdateleavebalance.Location = new System.Drawing.Point(890, 29);
            this.btnupdateleavebalance.Name = "btnupdateleavebalance";
            this.btnupdateleavebalance.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.btnupdateleavebalance.Size = new System.Drawing.Size(139, 48);
            this.btnupdateleavebalance.TabIndex = 51;
            this.btnupdateleavebalance.Text = "Refresh";
            this.btnupdateleavebalance.UseVisualStyleBackColor = false;
            // 
            // LeaveBalance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1139, 612);
            this.Controls.Add(this.btnupdateleavebalance);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.searchLeavebalncegride);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.filter_by_remaining_leaves);
            this.Controls.Add(this.dataGridViewleaveBalance);
            this.Name = "LeaveBalance";
            this.Text = "LeaveBalance";
            this.Load += new System.EventHandler(this.LeaveBalance_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewleaveBalance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewleaveBalance;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TextBox searchLeavebalncegride;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.ComboBox filter_by_remaining_leaves;
        private System.Windows.Forms.DataGridViewTextBoxColumn Employee;
        private System.Windows.Forms.DataGridViewTextBoxColumn Casual_Leave_Remaining;
        private System.Windows.Forms.DataGridViewTextBoxColumn Annaul_leave_Remaining;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sick_leave_Remaining;
        private System.Windows.Forms.DataGridViewButtonColumn Edit;
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
        private System.Windows.Forms.Button btnupdateleavebalance;
    }
}