namespace MadsKristensen.ExtensibilityTools.VSCT.Signing
{
    partial class SignForm
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
            this.label4 = new System.Windows.Forms.Label();
            this.txtCertificatePassword = new System.Windows.Forms.TextBox();
            this.txtCertificatePath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.radioFromFile = new System.Windows.Forms.RadioButton();
            this.cmbCertificates = new System.Windows.Forms.ComboBox();
            this.txtSubjectFilter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.radioInstalled = new System.Windows.Forms.RadioButton();
            this.bttFindCertificate = new System.Windows.Forms.Button();
            this.bttFindPackage = new System.Windows.Forms.Button();
            this.txtPackagePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bttCancel = new System.Windows.Forms.Button();
            this.bttOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtCertificatePassword);
            this.groupBox1.Controls.Add(this.txtCertificatePath);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.radioFromFile);
            this.groupBox1.Controls.Add(this.cmbCertificates);
            this.groupBox1.Controls.Add(this.txtSubjectFilter);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.radioInstalled);
            this.groupBox1.Controls.Add(this.bttFindCertificate);
            this.groupBox1.Controls.Add(this.bttFindPackage);
            this.groupBox1.Controls.Add(this.txtPackagePath);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(847, 225);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Properties";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(185, 191);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Password:";
            // 
            // txtCertificatePassword
            // 
            this.txtCertificatePassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCertificatePassword.Location = new System.Drawing.Point(259, 188);
            this.txtCertificatePassword.Name = "txtCertificatePassword";
            this.txtCertificatePassword.Size = new System.Drawing.Size(527, 20);
            this.txtCertificatePassword.TabIndex = 10;
            // 
            // txtCertificatePath
            // 
            this.txtCertificatePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCertificatePath.Location = new System.Drawing.Point(259, 162);
            this.txtCertificatePath.Name = "txtCertificatePath";
            this.txtCertificatePath.Size = new System.Drawing.Size(527, 20);
            this.txtCertificatePath.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(185, 165);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "File path:";
            // 
            // radioFromFile
            // 
            this.radioFromFile.AutoSize = true;
            this.radioFromFile.Location = new System.Drawing.Point(24, 163);
            this.radioFromFile.Name = "radioFromFile";
            this.radioFromFile.Size = new System.Drawing.Size(111, 17);
            this.radioFromFile.TabIndex = 7;
            this.radioFromFile.TabStop = true;
            this.radioFromFile.Text = "Certificate from file";
            this.radioFromFile.UseVisualStyleBackColor = true;
            this.radioFromFile.CheckedChanged += new System.EventHandler(this.OnRadioCheckedChanged);
            // 
            // cmbCertificates
            // 
            this.cmbCertificates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCertificates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCertificates.FormattingEnabled = true;
            this.cmbCertificates.Location = new System.Drawing.Point(259, 102);
            this.cmbCertificates.Name = "cmbCertificates";
            this.cmbCertificates.Size = new System.Drawing.Size(527, 21);
            this.cmbCertificates.TabIndex = 6;
            // 
            // txtSubjectFilter
            // 
            this.txtSubjectFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubjectFilter.Location = new System.Drawing.Point(259, 76);
            this.txtSubjectFilter.Name = "txtSubjectFilter";
            this.txtSubjectFilter.Size = new System.Drawing.Size(527, 20);
            this.txtSubjectFilter.TabIndex = 5;
            this.txtSubjectFilter.TextChanged += new System.EventHandler(this.txtSubjectFilter_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(185, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Subject filter:";
            // 
            // radioInstalled
            // 
            this.radioInstalled.AutoSize = true;
            this.radioInstalled.Location = new System.Drawing.Point(24, 77);
            this.radioInstalled.Name = "radioInstalled";
            this.radioInstalled.Size = new System.Drawing.Size(113, 17);
            this.radioInstalled.TabIndex = 3;
            this.radioInstalled.TabStop = true;
            this.radioInstalled.Text = "Installed certificate";
            this.radioInstalled.UseVisualStyleBackColor = true;
            this.radioInstalled.CheckedChanged += new System.EventHandler(this.OnRadioCheckedChanged);
            // 
            // bttFindCertificate
            // 
            this.bttFindCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bttFindCertificate.Location = new System.Drawing.Point(792, 160);
            this.bttFindCertificate.Name = "bttFindCertificate";
            this.bttFindCertificate.Size = new System.Drawing.Size(49, 23);
            this.bttFindCertificate.TabIndex = 2;
            this.bttFindCertificate.Text = "...";
            this.bttFindCertificate.UseVisualStyleBackColor = true;
            this.bttFindCertificate.Click += new System.EventHandler(this.bttFindCertificate_Click);
            // 
            // bttFindPackage
            // 
            this.bttFindPackage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bttFindPackage.Location = new System.Drawing.Point(792, 26);
            this.bttFindPackage.Name = "bttFindPackage";
            this.bttFindPackage.Size = new System.Drawing.Size(49, 23);
            this.bttFindPackage.TabIndex = 2;
            this.bttFindPackage.Text = "...";
            this.bttFindPackage.UseVisualStyleBackColor = true;
            this.bttFindPackage.Click += new System.EventHandler(this.bttFindPackage_Click);
            // 
            // txtPackagePath
            // 
            this.txtPackagePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPackagePath.Location = new System.Drawing.Point(153, 28);
            this.txtPackagePath.Name = "txtPackagePath";
            this.txtPackagePath.Size = new System.Drawing.Size(633, 20);
            this.txtPackagePath.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Package path:";
            // 
            // bttCancel
            // 
            this.bttCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bttCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bttCancel.Location = new System.Drawing.Point(784, 243);
            this.bttCancel.Name = "bttCancel";
            this.bttCancel.Size = new System.Drawing.Size(75, 23);
            this.bttCancel.TabIndex = 1;
            this.bttCancel.Text = "&Cancel";
            this.bttCancel.UseVisualStyleBackColor = true;
            // 
            // bttOK
            // 
            this.bttOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bttOK.Location = new System.Drawing.Point(703, 243);
            this.bttOK.Name = "bttOK";
            this.bttOK.Size = new System.Drawing.Size(75, 23);
            this.bttOK.TabIndex = 2;
            this.bttOK.Text = "&OK";
            this.bttOK.UseVisualStyleBackColor = true;
            this.bttOK.Click += new System.EventHandler(this.bttOK_Click);
            // 
            // SignForm
            // 
            this.AcceptButton = this.bttOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bttCancel;
            this.ClientSize = new System.Drawing.Size(871, 278);
            this.Controls.Add(this.bttOK);
            this.Controls.Add(this.bttCancel);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(340, 220);
            this.Name = "SignForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Signing VSIX package";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bttCancel;
        private System.Windows.Forms.Button bttOK;
        private System.Windows.Forms.Button bttFindPackage;
        private System.Windows.Forms.TextBox txtPackagePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCertificatePassword;
        private System.Windows.Forms.TextBox txtCertificatePath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioFromFile;
        private System.Windows.Forms.ComboBox cmbCertificates;
        private System.Windows.Forms.TextBox txtSubjectFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioInstalled;
        private System.Windows.Forms.Button bttFindCertificate;
    }
}