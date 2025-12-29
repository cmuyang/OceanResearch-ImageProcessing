using OceanResearch.Client;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OceanReseach.Client
{
    public partial class LoginForm : Form
    {
        private readonly HttpClient _client;
        public LoginResponse? LoginResult { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
            // 默认后端地址（开发环境） - 修改为你的后端地址和端口
            txtApiBase.Text = "https://localhost:5001/";
            txtUser.Text = "testuser";
            txtPass.Text = "password123";
            _client = new HttpClient();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            try
            {
                var baseAddr = txtApiBase.Text.Trim();
                if (!baseAddr.EndsWith("/")) baseAddr += "/";
                _client.BaseAddress = new Uri(baseAddr);

                var req = new { Username = txtUser.Text.Trim(), Password = txtPass.Text };
                var resp = await _client.PostAsJsonAsync("api/auth/login", req);
                if (!resp.IsSuccessStatusCode)
                {
                    MessageBox.Show("登录失败，请检查用户名密码和后端地址。", "登录", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var body = await resp.Content.ReadFromJsonAsync<LoginResponse>();
                if (body == null)
                {
                    MessageBox.Show("登录返回解析失败", "登录", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Program.CurrentUser = txtUser.Text;
                LoginResult = body;
                // 打开主窗体
                Program.AuthToken = body.Token;
                var main = new MainForm(body.Token, baseAddr);
                this.Hide();
                main.FormClosed += (s, ev) => this.Close();
                main.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("登录异常: " + ex.Message);
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var reg = new RegisterForm();
            reg.ShowDialog();
        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Username { get; set; } = "";
        public int UserId { get; set; }
    }
}
