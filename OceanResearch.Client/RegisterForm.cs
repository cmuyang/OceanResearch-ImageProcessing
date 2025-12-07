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

        public RegisterForm()
        {
            InitializeComponent();
            // 默认后端地址（请根据你的实际 dev 地址修改或在 LoginForm 传入）
            txtApiBase.Text = "https://localhost:5001/";
            _client = new HttpClient();
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            btnRegister.Enabled = false;
            try
            {
                var baseAddr = txtApiBase.Text.Trim();
                if (!baseAddr.EndsWith("/")) baseAddr += "/";
                _client.BaseAddress = new Uri(baseAddr);

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
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }

                // 直接打开主窗体（自动登录）
                var main = new MainForm(body.Token, txtApiBase.Text);
                this.Hide();
                main.FormClosed += (s, ev) => this.Close();
                main.Show();
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
    }
}
