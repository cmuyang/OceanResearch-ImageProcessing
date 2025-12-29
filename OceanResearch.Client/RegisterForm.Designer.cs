namespace OceanReseach.Client
{
    partial class RegisterForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.TextBox txtPassConfirm;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.TextBox txtApiBase;
        private System.Windows.Forms.Label lblApi;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.Label lblPassConfirm;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtApiBase = new TextBox();
            lblApi = new Label();
            txtUser = new TextBox();
            lblUser = new Label();
            txtPass = new TextBox();
            lblPass = new Label();
            txtPassConfirm = new TextBox();
            lblPassConfirm = new Label();
            btnRegister = new Button();
            SuspendLayout();
            // 
            // txtApiBase
            // 
            txtApiBase.AllowDrop = true;
            txtApiBase.Location = new Point(362, 133);
            txtApiBase.Name = "txtApiBase";
            txtApiBase.Size = new Size(224, 30);
            txtApiBase.TabIndex = 1;
            txtApiBase.Text = "https://localhost:5001/";
            // 
            // lblApi
            // 
            lblApi.AutoSize = true;
            lblApi.Location = new Point(231, 139);
            lblApi.Name = "lblApi";
            lblApi.Size = new Size(100, 24);
            lblApi.TabIndex = 0;
            lblApi.Text = "后端地址：";
            // 
            // txtUser
            // 
            txtUser.Location = new Point(362, 173);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(224, 30);
            txtUser.TabIndex = 3;
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Location = new Point(231, 179);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(82, 24);
            lblUser.TabIndex = 2;
            lblUser.Text = "用户名：";
            // 
            // txtPass
            // 
            txtPass.Location = new Point(362, 213);
            txtPass.Name = "txtPass";
            txtPass.PasswordChar = '*';
            txtPass.Size = new Size(224, 30);
            txtPass.TabIndex = 5;
            // 
            // lblPass
            // 
            lblPass.AutoSize = true;
            lblPass.Location = new Point(231, 219);
            lblPass.Name = "lblPass";
            lblPass.Size = new Size(64, 24);
            lblPass.TabIndex = 4;
            lblPass.Text = "密码：";
            // 
            // txtPassConfirm
            // 
            txtPassConfirm.Location = new Point(362, 253);
            txtPassConfirm.Name = "txtPassConfirm";
            txtPassConfirm.PasswordChar = '*';
            txtPassConfirm.Size = new Size(224, 30);
            txtPassConfirm.TabIndex = 7;
            // 
            // lblPassConfirm
            // 
            lblPassConfirm.AutoSize = true;
            lblPassConfirm.Location = new Point(231, 259);
            lblPassConfirm.Name = "lblPassConfirm";
            lblPassConfirm.Size = new Size(100, 24);
            lblPassConfirm.TabIndex = 6;
            lblPassConfirm.Text = "确认密码：";
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(380, 308);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(178, 32);
            btnRegister.TabIndex = 8;
            btnRegister.Text = "注册并登录";
            btnRegister.Click += btnRegister_Click;
            // 
            // RegisterForm
            // 
            ClientSize = new Size(902, 487);
            Controls.Add(lblApi);
            Controls.Add(txtApiBase);
            Controls.Add(lblUser);
            Controls.Add(txtUser);
            Controls.Add(lblPass);
            Controls.Add(txtPass);
            Controls.Add(lblPassConfirm);
            Controls.Add(txtPassConfirm);
            Controls.Add(btnRegister);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "RegisterForm";
            Text = "注册";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
