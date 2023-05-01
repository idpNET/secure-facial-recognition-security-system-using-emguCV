namespace Secure_Facial_Recognition_Security_System_using_EmguCV
{
    partial class frmAuthentication
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAuthentication));
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblName = new System.Windows.Forms.Label();
            this.BoxFrameGrabber = new Emgu.CV.UI.ImageBox();
            this.gbInformation = new System.Windows.Forms.GroupBox();
            this.txtNotification = new System.Windows.Forms.RichTextBox();
            this.gbFaceRecognition = new System.Windows.Forms.GroupBox();
            this.gpLogin = new System.Windows.Forms.GroupBox();
            this.lblPasscode1 = new System.Windows.Forms.Label();
            this.chAutoLogin = new System.Windows.Forms.CheckBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtPasscodeLogin = new System.Windows.Forms.TextBox();
            this.gpRegisteration = new System.Windows.Forms.GroupBox();
            this.lblPasscode2 = new System.Windows.Forms.Label();
            this.txtPasscodeRegister = new System.Windows.Forms.TextBox();
            this.tiNotification = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.BoxFrameGrabber)).BeginInit();
            this.gbInformation.SuspendLayout();
            this.gbFaceRecognition.SuspendLayout();
            this.gpLogin.SuspendLayout();
            this.gpRegisteration.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtUserName
            // 
            this.txtUserName.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.txtUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserName.Location = new System.Drawing.Point(84, 27);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(160, 22);
            this.txtUserName.TabIndex = 5;
            this.txtUserName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnRegister
            // 
            this.btnRegister.BackColor = System.Drawing.Color.Gray;
            this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRegister.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold);
            this.btnRegister.ForeColor = System.Drawing.Color.Black;
            this.btnRegister.Location = new System.Drawing.Point(7, 87);
            this.btnRegister.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(237, 32);
            this.btnRegister.TabIndex = 7;
            this.btnRegister.Text = "Register";
            this.btnRegister.UseVisualStyleBackColor = false;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.Gray;
            this.btnConnect.Enabled = false;
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnConnect.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold);
            this.btnConnect.ForeColor = System.Drawing.Color.Black;
            this.btnConnect.Location = new System.Drawing.Point(278, 278);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(320, 32);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect to Camera";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.ForeColor = System.Drawing.Color.White;
            this.lblName.Location = new System.Drawing.Point(10, 29);
            this.lblName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 14);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "Name";
            // 
            // BoxFrameGrabber
            // 
            this.BoxFrameGrabber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BoxFrameGrabber.Location = new System.Drawing.Point(278, 30);
            this.BoxFrameGrabber.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.BoxFrameGrabber.Name = "BoxFrameGrabber";
            this.BoxFrameGrabber.Size = new System.Drawing.Size(320, 240);
            this.BoxFrameGrabber.TabIndex = 18;
            this.BoxFrameGrabber.TabStop = false;
            // 
            // gbInformation
            // 
            this.gbInformation.Controls.Add(this.txtNotification);
            this.gbInformation.ForeColor = System.Drawing.Color.PapayaWhip;
            this.gbInformation.Location = new System.Drawing.Point(43, 363);
            this.gbInformation.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gbInformation.Name = "gbInformation";
            this.gbInformation.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gbInformation.Size = new System.Drawing.Size(609, 109);
            this.gbInformation.TabIndex = 21;
            this.gbInformation.TabStop = false;
            this.gbInformation.Text = "Notifications";
            // 
            // txtNotification
            // 
            this.txtNotification.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtNotification.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNotification.ForeColor = System.Drawing.Color.Gray;
            this.txtNotification.Location = new System.Drawing.Point(19, 25);
            this.txtNotification.Name = "txtNotification";
            this.txtNotification.ReadOnly = true;
            this.txtNotification.Size = new System.Drawing.Size(579, 68);
            this.txtNotification.TabIndex = 9;
            this.txtNotification.Text = "Nothing to report.";
            // 
            // gbFaceRecognition
            // 
            this.gbFaceRecognition.Controls.Add(this.gpLogin);
            this.gbFaceRecognition.Controls.Add(this.BoxFrameGrabber);
            this.gbFaceRecognition.Controls.Add(this.btnConnect);
            this.gbFaceRecognition.Controls.Add(this.gpRegisteration);
            this.gbFaceRecognition.ForeColor = System.Drawing.Color.PapayaWhip;
            this.gbFaceRecognition.Location = new System.Drawing.Point(43, 23);
            this.gbFaceRecognition.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gbFaceRecognition.Name = "gbFaceRecognition";
            this.gbFaceRecognition.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gbFaceRecognition.Size = new System.Drawing.Size(609, 332);
            this.gbFaceRecognition.TabIndex = 22;
            this.gbFaceRecognition.TabStop = false;
            this.gbFaceRecognition.Text = "Face Recognition";
            // 
            // gpLogin
            // 
            this.gpLogin.Controls.Add(this.lblPasscode1);
            this.gpLogin.Controls.Add(this.chAutoLogin);
            this.gpLogin.Controls.Add(this.btnLogin);
            this.gpLogin.Controls.Add(this.txtPasscodeLogin);
            this.gpLogin.Enabled = false;
            this.gpLogin.ForeColor = System.Drawing.Color.PapayaWhip;
            this.gpLogin.Location = new System.Drawing.Point(11, 23);
            this.gpLogin.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gpLogin.Name = "gpLogin";
            this.gpLogin.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gpLogin.Size = new System.Drawing.Size(258, 132);
            this.gpLogin.TabIndex = 23;
            this.gpLogin.TabStop = false;
            this.gpLogin.Text = "Login";
            // 
            // lblPasscode1
            // 
            this.lblPasscode1.AutoSize = true;
            this.lblPasscode1.BackColor = System.Drawing.Color.Transparent;
            this.lblPasscode1.ForeColor = System.Drawing.Color.White;
            this.lblPasscode1.Location = new System.Drawing.Point(10, 27);
            this.lblPasscode1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblPasscode1.Name = "lblPasscode1";
            this.lblPasscode1.Size = new System.Drawing.Size(63, 14);
            this.lblPasscode1.TabIndex = 20;
            this.lblPasscode1.Text = "Passcode";
            // 
            // chAutoLogin
            // 
            this.chAutoLogin.AutoSize = true;
            this.chAutoLogin.Location = new System.Drawing.Point(8, 94);
            this.chAutoLogin.Name = "chAutoLogin";
            this.chAutoLogin.Size = new System.Drawing.Size(131, 18);
            this.chAutoLogin.TabIndex = 4;
            this.chAutoLogin.Text = "Prompt to login";
            this.chAutoLogin.UseVisualStyleBackColor = true;
            this.chAutoLogin.CheckedChanged += new System.EventHandler(this.chAutoLogin_CheckedChanged);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.Gray;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnLogin.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogin.ForeColor = System.Drawing.Color.Black;
            this.btnLogin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogin.Location = new System.Drawing.Point(8, 55);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(237, 32);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtPasscodeLogin
            // 
            this.txtPasscodeLogin.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.txtPasscodeLogin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPasscodeLogin.ForeColor = System.Drawing.Color.Brown;
            this.txtPasscodeLogin.Location = new System.Drawing.Point(85, 25);
            this.txtPasscodeLogin.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.txtPasscodeLogin.Name = "txtPasscodeLogin";
            this.txtPasscodeLogin.PasswordChar = '*';
            this.txtPasscodeLogin.Size = new System.Drawing.Size(160, 22);
            this.txtPasscodeLogin.TabIndex = 2;
            this.txtPasscodeLogin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // gpRegisteration
            // 
            this.gpRegisteration.Controls.Add(this.lblName);
            this.gpRegisteration.Controls.Add(this.lblPasscode2);
            this.gpRegisteration.Controls.Add(this.btnRegister);
            this.gpRegisteration.Controls.Add(this.txtPasscodeRegister);
            this.gpRegisteration.Controls.Add(this.txtUserName);
            this.gpRegisteration.Enabled = false;
            this.gpRegisteration.ForeColor = System.Drawing.Color.PapayaWhip;
            this.gpRegisteration.Location = new System.Drawing.Point(10, 178);
            this.gpRegisteration.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gpRegisteration.Name = "gpRegisteration";
            this.gpRegisteration.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.gpRegisteration.Size = new System.Drawing.Size(258, 132);
            this.gpRegisteration.TabIndex = 22;
            this.gpRegisteration.TabStop = false;
            this.gpRegisteration.Text = "Register";
            // 
            // lblPasscode2
            // 
            this.lblPasscode2.AutoSize = true;
            this.lblPasscode2.BackColor = System.Drawing.Color.Transparent;
            this.lblPasscode2.ForeColor = System.Drawing.Color.White;
            this.lblPasscode2.Location = new System.Drawing.Point(9, 59);
            this.lblPasscode2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblPasscode2.Name = "lblPasscode2";
            this.lblPasscode2.Size = new System.Drawing.Size(63, 14);
            this.lblPasscode2.TabIndex = 20;
            this.lblPasscode2.Text = "Passcode";
            // 
            // txtPasscodeRegister
            // 
            this.txtPasscodeRegister.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.txtPasscodeRegister.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPasscodeRegister.ForeColor = System.Drawing.Color.Brown;
            this.txtPasscodeRegister.Location = new System.Drawing.Point(84, 57);
            this.txtPasscodeRegister.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.txtPasscodeRegister.Name = "txtPasscodeRegister";
            this.txtPasscodeRegister.PasswordChar = '*';
            this.txtPasscodeRegister.Size = new System.Drawing.Size(160, 22);
            this.txtPasscodeRegister.TabIndex = 6;
            this.txtPasscodeRegister.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tiNotification
            // 
            this.tiNotification.Interval = 1000;
            this.tiNotification.Tick += new System.EventHandler(this.tiNotification_Tick);
            // 
            // frmAuthentication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.ClientSize = new System.Drawing.Size(694, 501);
            this.Controls.Add(this.gbInformation);
            this.Controls.Add(this.gbFaceRecognition);
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(710, 540);
            this.MinimumSize = new System.Drawing.Size(710, 540);
            this.Name = "frmAuthentication";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Secure Facial Recognition Security System [authentication]";
            this.Load += new System.EventHandler(this.frmAuthentication_Load);
            ((System.ComponentModel.ISupportInitialize)(this.BoxFrameGrabber)).EndInit();
            this.gbInformation.ResumeLayout(false);
            this.gbFaceRecognition.ResumeLayout(false);
            this.gpLogin.ResumeLayout(false);
            this.gpLogin.PerformLayout();
            this.gpRegisteration.ResumeLayout(false);
            this.gpRegisteration.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblName;
        private Emgu.CV.UI.ImageBox BoxFrameGrabber;
        private System.Windows.Forms.GroupBox gbInformation;
        private System.Windows.Forms.GroupBox gbFaceRecognition;
        private System.Windows.Forms.RichTextBox txtNotification;
        private System.Windows.Forms.TextBox txtPasscodeRegister;
        private System.Windows.Forms.Timer tiNotification;
        private System.Windows.Forms.Label lblPasscode2;
        private System.Windows.Forms.CheckBox chAutoLogin;
        private System.Windows.Forms.GroupBox gpLogin;
        private System.Windows.Forms.Label lblPasscode1;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtPasscodeLogin;
        private System.Windows.Forms.GroupBox gpRegisteration;
    }
}

