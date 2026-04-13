namespace HRMS_ERP.Forms
{
    partial class AddEmployee
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtDepartment = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.Reporting_Manager = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Designation = new System.Windows.Forms.TextBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.Bank_Account_number = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Bank_Name = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.txtcnic = new System.Windows.Forms.TextBox();
            this.txtEmployeeCode = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.department = new System.Windows.Forms.Label();
            this.Email = new System.Windows.Forms.Label();
            this.LastName = new System.Windows.Forms.Label();
            this.FirstName = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(509, 660);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtDepartment);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.Reporting_Manager);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.Designation);
            this.groupBox1.Controls.Add(this.dateTimePicker1);
            this.groupBox1.Controls.Add(this.Bank_Account_number);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.Bank_Name);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtPhone);
            this.groupBox1.Controls.Add(this.txtcnic);
            this.groupBox1.Controls.Add(this.txtEmployeeCode);
            this.groupBox1.Controls.Add(this.txtFirstName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.department);
            this.groupBox1.Controls.Add(this.Email);
            this.groupBox1.Controls.Add(this.LastName);
            this.groupBox1.Controls.Add(this.FirstName);
            this.groupBox1.Location = new System.Drawing.Point(20, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(407, 644);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Employee Information";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // txtDepartment
            // 
            this.txtDepartment.Location = new System.Drawing.Point(6, 300);
            this.txtDepartment.Name = "txtDepartment";
            this.txtDepartment.Size = new System.Drawing.Size(164, 22);
            this.txtDepartment.TabIndex = 24;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Gray;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(208, 578);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(150, 52);
            this.button2.TabIndex = 23;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Green;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(17, 578);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 52);
            this.button1.TabIndex = 22;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.ForeColor = System.Drawing.Color.Black;
            this.checkBox1.Location = new System.Drawing.Point(17, 551);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(68, 21);
            this.checkBox1.TabIndex = 21;
            this.checkBox1.Text = "Active";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Reporting_Manager
            // 
            this.Reporting_Manager.Location = new System.Drawing.Point(11, 510);
            this.Reporting_Manager.Name = "Reporting_Manager";
            this.Reporting_Manager.Size = new System.Drawing.Size(363, 22);
            this.Reporting_Manager.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(14, 478);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(130, 17);
            this.label6.TabIndex = 19;
            this.label6.Text = "Reporting Manager";
            // 
            // Designation
            // 
            this.Designation.Location = new System.Drawing.Point(11, 375);
            this.Designation.Name = "Designation";
            this.Designation.Size = new System.Drawing.Size(363, 22);
            this.Designation.TabIndex = 18;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(180, 300);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 22);
            this.dateTimePicker1.TabIndex = 16;
            // 
            // Bank_Account_number
            // 
            this.Bank_Account_number.Location = new System.Drawing.Point(11, 435);
            this.Bank_Account_number.Name = "Bank_Account_number";
            this.Bank_Account_number.Size = new System.Drawing.Size(164, 22);
            this.Bank_Account_number.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(222, 415);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 17);
            this.label5.TabIndex = 14;
            this.label5.Text = "Bank Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(231, 262);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "Joining Date";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(14, 415);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(153, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Bank Account Number ";
            // 
            // Bank_Name
            // 
            this.Bank_Name.Location = new System.Drawing.Point(225, 435);
            this.Bank_Name.Name = "Bank_Name";
            this.Bank_Name.Size = new System.Drawing.Size(149, 22);
            this.Bank_Name.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(15, 346);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Position";
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(11, 219);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(369, 22);
            this.txtPhone.TabIndex = 8;
            // 
            // txtcnic
            // 
            this.txtcnic.Location = new System.Drawing.Point(11, 145);
            this.txtcnic.Name = "txtcnic";
            this.txtcnic.Size = new System.Drawing.Size(369, 22);
            this.txtcnic.TabIndex = 7;
            // 
            // txtEmployeeCode
            // 
            this.txtEmployeeCode.Location = new System.Drawing.Point(225, 71);
            this.txtEmployeeCode.Name = "txtEmployeeCode";
            this.txtEmployeeCode.Size = new System.Drawing.Size(155, 22);
            this.txtEmployeeCode.TabIndex = 6;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(11, 71);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(164, 22);
            this.txtFirstName.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(14, 190);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Phone";
            // 
            // department
            // 
            this.department.AutoSize = true;
            this.department.ForeColor = System.Drawing.Color.Black;
            this.department.Location = new System.Drawing.Point(15, 262);
            this.department.Name = "department";
            this.department.Size = new System.Drawing.Size(82, 17);
            this.department.TabIndex = 3;
            this.department.Text = "Department";
            // 
            // Email
            // 
            this.Email.AutoSize = true;
            this.Email.ForeColor = System.Drawing.Color.Black;
            this.Email.Location = new System.Drawing.Point(21, 115);
            this.Email.Name = "Email";
            this.Email.Size = new System.Drawing.Size(39, 17);
            this.Email.TabIndex = 2;
            this.Email.Text = "CNIC";
            // 
            // LastName
            // 
            this.LastName.AutoSize = true;
            this.LastName.ForeColor = System.Drawing.Color.Black;
            this.LastName.Location = new System.Drawing.Point(231, 38);
            this.LastName.Name = "LastName";
            this.LastName.Size = new System.Drawing.Size(107, 17);
            this.LastName.TabIndex = 1;
            this.LastName.Text = "Employee Code";
            // 
            // FirstName
            // 
            this.FirstName.AutoSize = true;
            this.FirstName.ForeColor = System.Drawing.Color.Black;
            this.FirstName.Location = new System.Drawing.Point(21, 38);
            this.FirstName.Name = "FirstName";
            this.FirstName.Size = new System.Drawing.Size(76, 17);
            this.FirstName.TabIndex = 0;
            this.FirstName.Text = "First Name";
            // 
            // AddEmployee
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(533, 682);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEmployee";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddEmployee";
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox Reporting_Manager;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Designation;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.TextBox Bank_Account_number;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Bank_Name;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.TextBox txtcnic;
        private System.Windows.Forms.TextBox txtEmployeeCode;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label department;
        private System.Windows.Forms.Label Email;
        private System.Windows.Forms.Label LastName;
        private System.Windows.Forms.Label FirstName;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtDepartment;
    }
}