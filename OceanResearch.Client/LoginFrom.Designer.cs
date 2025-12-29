namespace OceanReseach.Client
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtApiBase;
        private System.Windows.Forms.Label label3;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtUser = new TextBox();
            txtPass = new TextBox();
            btnLogin = new Button();
            label1 = new Label();
            label2 = new Label();
            txtApiBase = new TextBox();
            label3 = new Label();
            button1 = new Button();
            SuspendLayout();
            // 
            // txtUser
            // 
            txtUser.Location = new Point(401, 183);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(220, 30);
            txtUser.TabIndex = 0;
            txtUser.TextChanged += txtUser_TextChanged;
            // 
            // txtPass
            // 
            txtPass.Location = new Point(401, 227);
            txtPass.Name = "txtPass";
            txtPass.PasswordChar = '*';
            txtPass.Size = new Size(220, 30);
            txtPass.TabIndex = 1;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(276, 293);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(145, 40);
            btnLogin.TabIndex = 3;
            btnLogin.Text = "登录";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(286, 183);
            label1.Name = "label1";
            label1.Size = new Size(82, 24);
            label1.TabIndex = 6;
            label1.Text = "用户名：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(288, 230);
            label2.Name = "label2";
            label2.Size = new Size(64, 24);
            label2.TabIndex = 5;
            label2.Text = "密码：";
            // 
            // txtApiBase
            // 
            txtApiBase.Location = new Point(401, 138);
            txtApiBase.Name = "txtApiBase";
            txtApiBase.Size = new Size(220, 30);
            txtApiBase.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(283, 137);
            label3.Name = "label3";
            label3.Size = new Size(100, 24);
            label3.TabIndex = 0;
            label3.Text = "后端地址：";
            // 
            // button1
            // 
            button1.Location = new Point(488, 293);
            button1.Name = "button1";
            button1.Size = new Size(149, 40);
            button1.TabIndex = 7;
            button1.Text = "注册";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // LoginForm
            // 
            ClientSize = new Size(944, 532);
            Controls.Add(button1);
            Controls.Add(label3);
            Controls.Add(txtApiBase);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnLogin);
            Controls.Add(txtPass);
            Controls.Add(txtUser);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "LoginForm";
            Text = "OceanResearch 登录";
            ResumeLayout(false);
            PerformLayout();
        }
        private Button button1;
    }
}
