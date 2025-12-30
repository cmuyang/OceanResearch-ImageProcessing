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
        // 添加一个字段来存储后端地址
        private string _apiBase = "https://localhost:5001/";
        public LoginResponse? LoginResult { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
            // 设置窗口启动位置为屏幕中央
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // 默认后端地址（开发环境） - 修改为你的后端地址和端口
            // txtApiBase.Text = "https://localhost:5001/"; // 已删除控件，不再赋值
            txtUser.Text = "testuser";
            txtPass.Text = "password123";
            _client = new HttpClient();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            try
            {
                // 直接使用变量而不是从控件获取
                var baseAddr = _apiBase;
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
                // 当主窗体关闭时退出应用
                main.FormClosed += (s, ev) => Application.Exit();
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
            // 打开注册页并隐藏当前登录页；注册页通过构造函数持有对登录页的引用，
            // 以便用户点击"返回"时能重新显示登录页
            // 直接传递变量
            var baseAddr = _apiBase;
            var reg = new RegisterForm(this, baseAddr);
            this.Hide();
            reg.Show();
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
