namespace PTZ_Controller
{
    partial class PTZForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCmd = new System.Windows.Forms.TextBox();
            this.buttonGo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "PTZ Command";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(192, 91);
            this.label2.TabIndex = 1;
            this.label2.Text = "INPUT FORMAT EXAMPLES:\r\n\"PL30\" == Pan left 30 degrees\r\n\"tu30\" == Tilt up 30 degre" +
    "es\r\n\"Zo\" == Zoom out\r\n\r\nUse Arrow Keys to Pan or Tilt\r\nUse Number Keys/Pad to Zo" +
    "om In/Out";
            // 
            // textBoxCmd
            // 
            this.textBoxCmd.Location = new System.Drawing.Point(96, 27);
            this.textBoxCmd.Name = "textBoxCmd";
            this.textBoxCmd.Size = new System.Drawing.Size(119, 20);
            this.textBoxCmd.TabIndex = 2;
            // 
            // buttonGo
            // 
            this.buttonGo.Location = new System.Drawing.Point(134, 53);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(81, 33);
            this.buttonGo.TabIndex = 3;
            this.buttonGo.Text = "Go";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // PTZForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 244);
            this.Controls.Add(this.buttonGo);
            this.Controls.Add(this.textBoxCmd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PTZForm";
            this.Text = "PTZForm";
            this.Load += new System.EventHandler(this.PTZForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PTZForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PTZForm_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCmd;
        private System.Windows.Forms.Button buttonGo;
    }
}