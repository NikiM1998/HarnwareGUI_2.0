namespace HarnwareGUI
{
    partial class LoginForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlLogIn = new Krypton.Toolkit.KryptonPanel();
            lblPassword = new Krypton.Toolkit.KryptonLabel();
            lblUserName = new Krypton.Toolkit.KryptonLabel();
            txtbxPassword = new Krypton.Toolkit.KryptonTextBox();
            txtbxUserName = new Krypton.Toolkit.KryptonTextBox();
            lblLogInInfo = new Krypton.Toolkit.KryptonLabel();
            btnLogIn = new Krypton.Toolkit.KryptonButton();
            btnCreateUser = new Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)pnlLogIn).BeginInit();
            pnlLogIn.SuspendLayout();
            SuspendLayout();
            // 
            // pnlLogIn
            // 
            pnlLogIn.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlLogIn.Controls.Add(lblPassword);
            pnlLogIn.Controls.Add(lblUserName);
            pnlLogIn.Controls.Add(txtbxPassword);
            pnlLogIn.Controls.Add(txtbxUserName);
            pnlLogIn.Controls.Add(lblLogInInfo);
            pnlLogIn.Location = new Point(12, 12);
            pnlLogIn.Name = "pnlLogIn";
            pnlLogIn.Size = new Size(473, 200);
            pnlLogIn.TabIndex = 0;
            // 
            // lblPassword
            // 
            lblPassword.Location = new Point(204, 128);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(62, 20);
            lblPassword.TabIndex = 4;
            lblPassword.Values.Text = "Password";
            // 
            // lblUserName
            // 
            lblUserName.Location = new Point(204, 53);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(71, 20);
            lblUserName.TabIndex = 3;
            lblUserName.Values.Text = "User Name";
            // 
            // txtbxPassword
            // 
            txtbxPassword.Location = new Point(10, 154);
            txtbxPassword.Name = "txtbxPassword";
            txtbxPassword.PasswordChar = '●';
            txtbxPassword.Size = new Size(458, 23);
            txtbxPassword.TabIndex = 2;
            txtbxPassword.TextAlign = HorizontalAlignment.Center;
            txtbxPassword.UseSystemPasswordChar = true;
            // 
            // txtbxUserName
            // 
            txtbxUserName.Location = new Point(10, 79);
            txtbxUserName.Name = "txtbxUserName";
            txtbxUserName.Size = new Size(458, 23);
            txtbxUserName.TabIndex = 1;
            txtbxUserName.TextAlign = HorizontalAlignment.Center;
            // 
            // lblLogInInfo
            // 
            lblLogInInfo.Location = new Point(10, 9);
            lblLogInInfo.Name = "lblLogInInfo";
            lblLogInInfo.Size = new Size(252, 20);
            lblLogInInfo.TabIndex = 0;
            lblLogInInfo.Values.Text = "Please log in with your Harnware credentials";
            // 
            // btnLogIn
            // 
            btnLogIn.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            btnLogIn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnLogIn.Location = new Point(12, 226);
            btnLogIn.Name = "btnLogIn";
            btnLogIn.Size = new Size(116, 44);
            btnLogIn.TabIndex = 1;
            btnLogIn.Values.Text = "Log in";
            btnLogIn.Click += btnLogIn_Click;
            // 
            // btnCreateUser
            // 
            btnCreateUser.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            btnCreateUser.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnCreateUser.Location = new Point(369, 226);
            btnCreateUser.Name = "btnCreateUser";
            btnCreateUser.Size = new Size(116, 44);
            btnCreateUser.TabIndex = 2;
            btnCreateUser.Values.Text = "Create User";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(497, 282);
            Controls.Add(btnCreateUser);
            Controls.Add(btnLogIn);
            Controls.Add(pnlLogIn);
            Name = "LoginForm";
            Text = "Log in";
            ((System.ComponentModel.ISupportInitialize)pnlLogIn).EndInit();
            pnlLogIn.ResumeLayout(false);
            pnlLogIn.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Krypton.Toolkit.KryptonPanel pnlLogIn;
        private Krypton.Toolkit.KryptonButton btnLogIn;
        private Krypton.Toolkit.KryptonButton btnCreateUser;
        private Krypton.Toolkit.KryptonLabel lblLogInInfo;
        private Krypton.Toolkit.KryptonLabel lblPassword;
        private Krypton.Toolkit.KryptonLabel lblUserName;
        private Krypton.Toolkit.KryptonTextBox txtbxPassword;
        private Krypton.Toolkit.KryptonTextBox txtbxUserName;
    }
}
