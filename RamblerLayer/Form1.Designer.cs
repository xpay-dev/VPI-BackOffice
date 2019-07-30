namespace Format6_Decoder
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.IDC_KSN = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.IDC_BDK = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.IDC_Track2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.IDC_Decrypt = new System.Windows.Forms.Button();
            this.IDC_EncTrack_Hex = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.IDC_EncTrack_Hex);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.IDC_KSN);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(513, 83);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Swiper Input";
            // 
            // IDC_KSN
            // 
            this.IDC_KSN.Location = new System.Drawing.Point(80, 21);
            this.IDC_KSN.Name = "IDC_KSN";
            this.IDC_KSN.Size = new System.Drawing.Size(421, 22);
            this.IDC_KSN.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "KSN";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.IDC_BDK);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(12, 101);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(513, 52);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Derivation Key";
            // 
            // IDC_BDK
            // 
            this.IDC_BDK.Location = new System.Drawing.Point(80, 21);
            this.IDC_BDK.Name = "IDC_BDK";
            this.IDC_BDK.Size = new System.Drawing.Size(421, 22);
            this.IDC_BDK.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "Hex Format";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.IDC_Track2);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Location = new System.Drawing.Point(12, 159);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(513, 51);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Output";
            // 
            // IDC_Track2
            // 
            this.IDC_Track2.Location = new System.Drawing.Point(80, 21);
            this.IDC_Track2.Name = "IDC_Track2";
            this.IDC_Track2.Size = new System.Drawing.Size(421, 22);
            this.IDC_Track2.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "Track 2";
            // 
            // IDC_Decrypt
            // 
            this.IDC_Decrypt.Location = new System.Drawing.Point(531, 12);
            this.IDC_Decrypt.Name = "IDC_Decrypt";
            this.IDC_Decrypt.Size = new System.Drawing.Size(65, 43);
            this.IDC_Decrypt.TabIndex = 3;
            this.IDC_Decrypt.Text = "Decrypt";
            this.IDC_Decrypt.UseVisualStyleBackColor = true;
            this.IDC_Decrypt.Click += new System.EventHandler(this.IDC_Decrypt_Click);
            // 
            // IDC_EncTrack_Hex
            // 
            this.IDC_EncTrack_Hex.Location = new System.Drawing.Point(80, 49);
            this.IDC_EncTrack_Hex.Name = "IDC_EncTrack_Hex";
            this.IDC_EncTrack_Hex.Size = new System.Drawing.Size(421, 22);
            this.IDC_EncTrack_Hex.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Encrypted";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Track 2";
            // 
            // Form1
            // 
            this.AcceptButton = this.IDC_Decrypt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 217);
            this.Controls.Add(this.IDC_Decrypt);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Format 6 - Track 2 Decoder";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox IDC_KSN;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox IDC_BDK;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox IDC_Track2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button IDC_Decrypt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox IDC_EncTrack_Hex;
        private System.Windows.Forms.Label label2;
    }
}

