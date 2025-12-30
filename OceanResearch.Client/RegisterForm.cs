using OceanResearch.Client;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OceanReseach.Client
{
    public partial class RegisterForm : Form
    {
        private readonly HttpClient _client;
        private readonly LoginForm? _loginForm;
        private string _apiBase = "https://localhost:5001/";

        // 新增构造函数：接收可选的登录窗体引用与 base 地址
        public RegisterForm(LoginForm? loginForm = null, string? apiBase = null)
        {
            InitializeComponent();
            // 设置窗口启动位置为屏幕中央
            this.StartPosition = FormStartPosition.CenterScreen;

            _loginForm = loginForm;
            if (!string.IsNullOrWhiteSpace(apiBase))
            {
                // txtApiBase.Text = apiBase; // 已删除控件
                _apiBase = apiBase;
            }
            else
            {
                // txtApiBase.Text = _apiBase; // 已删除控件
            }
            _client = new HttpClient();
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            btnRegister.Enabled = false;
            try
            {
                // 直接使用变量
                var baseAddr = _apiBase;
                if (!baseAddr.EndsWith("/")) baseAddr += "/";
                _client.BaseAddress = new Uri(baseAddr);
                _apiBase = baseAddr;

                var username = txtUser.Text.Trim();
                var pwd = txtPass.Text;
                var pwd2 = txtPassConfirm.Text;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(pwd))
                {
                    MessageBox.Show("请输入用户名和密码");
                    return;
                }
                if (pwd != pwd2)
                {
                    MessageBox.Show("两次密码输入不一致");
                    return;
                }
                if (pwd.Length < 6)
                {
                    MessageBox.Show("密码长度至少 6 位");
                    return;
                }

                var req = new { Username = username, Password = pwd };
                var resp = await _client.PostAsJsonAsync("api/auth/register", req);
                if (!resp.IsSuccessStatusCode)
                {
                    var err = await resp.Content.ReadFromJsonAsync<object>();
                    MessageBox.Show("注册失败: " + resp.StatusCode + " " + (err?.ToString() ?? ""));
                    return;
                }

                // 解析返回的 LoginResponse（包含 token）
                var body = await resp.Content.ReadFromJsonAsync<LoginResponse>();
                if (body == null)
                {
                    MessageBox.Show("注册成功，但未能解析返回的 token。你可以手动登录。");
                    // 返回登录页（如果有引用）
                    _loginForm?.Show();
                    this.Close();
                    return;
                }

                // 自动登录并打开主窗体（只保留主窗体）
                Program.AuthToken = body.Token;
                Program.CurrentUser = username;

                // 关键修改：不要直接 Close 登录窗体，因为它是主消息循环的拥有者
                // 而是隐藏它，或者如果必须关闭，需要转移 ApplicationContext 的主窗体
                // 但最简单的做法是：隐藏登录窗体，让 MainForm 成为可见窗体
                // 当 MainForm 关闭时，再彻底退出应用
                
                if (_loginForm != null)
                {
                    _loginForm.Hide();
                }

                var main = new MainForm(body.Token, _apiBase);
                // 主窗体关闭时退出整个应用
                main.FormClosed += (s, ev) => Application.Exit();
                main.Show();

                // 关闭注册窗体（自己）
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("注册异常: " + ex.Message);
            }
            finally
            {
                btnRegister.Enabled = true;
            }
        }

        private void btnBackToLogin_Click(object sender, EventArgs e)
        {
            // 如果是从登录页打开的，关闭当前注册页并让登录页重新显示
            _loginForm?.Show();
            this.Close();
        }
    }
}
