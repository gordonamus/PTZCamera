namespace PTZ_Controller
{
    partial class LoginForm
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
            this.textBoxDevName = new System.Windows.Forms.TextBox();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.textBoxport = new System.Windows.Forms.TextBox();
            this.labelDevName = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelIP = new System.Windows.Forms.Label();
            this.labelport = new System.Windows.Forms.Label();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxDevName
            // 
            this.textBoxDevName.Location = new System.Drawing.Point(102, 44);
            this.textBoxDevName.Name = "textBoxDevName";
            this.textBoxDevName.Size = new System.Drawing.Size(123, 20);
            this.textBoxDevName.TabIndex = 0;
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(102, 70);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(123, 20);
            this.textBoxUsername.TabIndex = 1;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(102, 96);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(123, 20);
            this.textBoxPassword.TabIndex = 2;
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(102, 122);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(123, 20);
            this.textBoxIP.TabIndex = 3;
            // 
            // textBoxport
            // 
            this.textBoxport.Location = new System.Drawing.Point(102, 148);
            this.textBoxport.Name = "textBoxport";
            this.textBoxport.Size = new System.Drawing.Size(123, 20);
            this.textBoxport.TabIndex = 4;
            // 
            // labelDevName
            // 
            this.labelDevName.AutoSize = true;
            this.labelDevName.Location = new System.Drawing.Point(24, 47);
            this.labelDevName.Name = "labelDevName";
            this.labelDevName.Size = new System.Drawing.Size(72, 13);
            this.labelDevName.TabIndex = 5;
            this.labelDevName.Text = "Device Name";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(24, 73);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(55, 13);
            this.labelUsername.TabIndex = 6;
            this.labelUsername.Text = "Username";
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(24, 99);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(53, 13);
            this.labelPassword.TabIndex = 7;
            this.labelPassword.Text = "Password";
            // 
            // labelIP
            // 
            this.labelIP.AutoSize = true;
            this.labelIP.Location = new System.Drawing.Point(24, 125);
            this.labelIP.Name = "labelIP";
            this.labelIP.Size = new System.Drawing.Size(58, 13);
            this.labelIP.TabIndex = 8;
            this.labelIP.Text = "IP Address";
            // 
            // labelport
            // 
            this.labelport.AutoSize = true;
            this.labelport.Location = new System.Drawing.Point(24, 151);
            this.labelport.Name = "labelport";
            this.labelport.Size = new System.Drawing.Size(26, 13);
            this.labelport.TabIndex = 9;
            this.labelport.Text = "Port";
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(142, 196);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(83, 26);
            this.buttonStart.TabIndex = 10;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(27, 196);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(75, 23);
            this.buttonExit.TabIndex = 11;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 234);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.labelport);
            this.Controls.Add(this.labelIP);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelUsername);
            this.Controls.Add(this.labelDevName);
            this.Controls.Add(this.textBoxport);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.textBoxDevName);
            this.Name = "LoginForm";
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBoxDevName;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.TextBox textBoxport;
        private System.Windows.Forms.Label labelDevName;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Label labelIP;
        private System.Windows.Forms.Label labelport;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonExit;
    }
}

