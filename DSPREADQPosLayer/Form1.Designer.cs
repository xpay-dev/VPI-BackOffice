namespace DSPREADQPosLayer
{
    partial class Form1
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
            this.btnDecryptDukpt = new System.Windows.Forms.Button();
            this.txtBdk = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtKsn = new System.Windows.Forms.TextBox();
            this.txtEntracks = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.txtIpek = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtTerminalMasterKey = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnDecryptDukpt
            // 
            this.btnDecryptDukpt.Location = new System.Drawing.Point(318, 253);
            this.btnDecryptDukpt.Name = "btnDecryptDukpt";
            this.btnDecryptDukpt.Size = new System.Drawing.Size(75, 29);
            this.btnDecryptDukpt.TabIndex = 0;
            this.btnDecryptDukpt.Text = "Decrypt";
            this.btnDecryptDukpt.UseVisualStyleBackColor = true;
            this.btnDecryptDukpt.Click += new System.EventHandler(this.btnDecryptDukpt_Click);
            // 
            // txtBdk
            // 
            this.txtBdk.Location = new System.Drawing.Point(65, 12);
            this.txtBdk.Multiline = true;
            this.txtBdk.Name = "txtBdk";
            this.txtBdk.Size = new System.Drawing.Size(329, 29);
            this.txtBdk.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "BDK:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "KSN:";
            // 
            // txtKsn
            // 
            this.txtKsn.Location = new System.Drawing.Point(65, 121);
            this.txtKsn.Multiline = true;
            this.txtKsn.Name = "txtKsn";
            this.txtKsn.Size = new System.Drawing.Size(328, 31);
            this.txtKsn.TabIndex = 4;
            // 
            // txtEntracks
            // 
            this.txtEntracks.Location = new System.Drawing.Point(65, 158);
            this.txtEntracks.Multiline = true;
            this.txtEntracks.Name = "txtEntracks";
            this.txtEntracks.Size = new System.Drawing.Size(328, 89);
            this.txtEntracks.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(225, 197);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 172);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Entracks:";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(423, 28);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(328, 219);
            this.txtResult.TabIndex = 9;
            // 
            // txtIpek
            // 
            this.txtIpek.Location = new System.Drawing.Point(65, 84);
            this.txtIpek.Multiline = true;
            this.txtIpek.Name = "txtIpek";
            this.txtIpek.Size = new System.Drawing.Size(328, 31);
            this.txtIpek.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 92);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Enc Ipek:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(26, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "TMK:";
            // 
            // txtTerminalMasterKey
            // 
            this.txtTerminalMasterKey.Location = new System.Drawing.Point(65, 47);
            this.txtTerminalMasterKey.Multiline = true;
            this.txtTerminalMasterKey.Name = "txtTerminalMasterKey";
            this.txtTerminalMasterKey.Size = new System.Drawing.Size(328, 31);
            this.txtTerminalMasterKey.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(420, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Decryted Data:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 292);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtTerminalMasterKey);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtIpek);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtEntracks);
            this.Controls.Add(this.txtKsn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBdk);
            this.Controls.Add(this.btnDecryptDukpt);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDecryptDukpt;
        private System.Windows.Forms.TextBox txtBdk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtKsn;
        private System.Windows.Forms.TextBox txtEntracks;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.TextBox txtIpek;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtTerminalMasterKey;
        private System.Windows.Forms.Label label8;
    }
}

