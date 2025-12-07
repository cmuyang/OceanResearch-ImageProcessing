using OceanResearch.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OceanReseach.Client
{
    public partial class MainForm : Form
    {
        private readonly HttpClient _client;
        private readonly string _apiBase;
        private int _page = 1;
        private readonly int _pageSize = 10;
        private List<ImageDto> _currentImages = new();
        // 保存每张图片的选中状态，key: 图片索引（1-10），value: 是否选中
        private Dictionary<int, bool> imageSelections = new Dictionary<int, bool>();
        
        private readonly string[] _choices = new[] { "未选择", "最清晰", "研究价值高", "剔除" };

        public MainForm(string jwtToken, string apiBase)
        {
            InitializeComponent();

            for (int i = 1; i <= 10; i++)
            {
                imageSelections[i] = false;
            }

            _apiBase = apiBase.EndsWith("/") ? apiBase : apiBase + "/";
            _client = new HttpClient { BaseAddress = new Uri(_apiBase) };
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // 确保事件绑定生效（Designer 已把 Click 事件指向这些处理器）
            this.Load += MainForm_Load;
        }

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            await LoadPageAsync();
        }

        private async Task LoadPageAsync()
        {
            try
            {
                btnPrev.Enabled = false;
                btnNext.Enabled = false;
                btnSubmit.Enabled = false;

                var res = await _client.GetAsync($"api/images?page={_page}&pageSize={_pageSize}");
                if (!res.IsSuccessStatusCode)
                {
                    MessageBox.Show("获取图片失败: " + res.StatusCode, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _currentImages = await res.Content.ReadFromJsonAsync<List<ImageDto>>() ?? new List<ImageDto>();
                await DisplayImagesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载异常: " + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnPrev.Enabled = _page > 1;
                btnNext.Enabled = true;
                btnSubmit.Enabled = true;
            }
        }

        private async Task DisplayImagesAsync()
        {
            var pictureBoxes = new[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10 };
            var combos = new[] { comboChoice1, comboChoice2, comboChoice3, comboChoice4, comboChoice5, comboChoice6, comboChoice7, comboChoice8, comboChoice9, comboChoice10 };

            for (int i = 0; i < _pageSize; i++)
            {
                var combo = combos[i];
                combo.Items.Clear();
                combo.Items.AddRange(_choices);
                combo.SelectedIndex = 0;

                // 释放旧图像，避免内存泄漏
                var oldImage = pictureBoxes[i].Image;
                if (oldImage != null)
                {
                    pictureBoxes[i].Image = null;
                    oldImage.Dispose();
                }

                if (i < _currentImages.Count)
                {
                    var img = _currentImages[i];
                    pictureBoxes[i].Tag = img;

                    // 构建完整 URL
                    var fullUrl = new Uri(new Uri(_apiBase), img.Url.TrimStart('/')).ToString();

                    try
                    {
                        // 异步下载 bytes
                        var bytes = await _client.GetByteArrayAsync(fullUrl);
                        using var ms = new MemoryStream(bytes);
                        // 从流创建 Image
                        pictureBoxes[i].Image = Image.FromStream(ms);
                    }
                    catch (Exception)
                    {
                        // 下载或解码失败时显示占位（或清空）
                        pictureBoxes[i].Image = null;
                    }
                }
                else
                {
                    pictureBoxes[i].Tag = null;
                    pictureBoxes[i].Image = null;
                }
            }

            lblPage.Text = $"页：{_page}";
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            await SubmitSelectionsAsync();
        }

        private async Task SubmitSelectionsAsync()
        {
            PictureBox[] pictureBoxes =
            {
                pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
                pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10
            };

            ComboBox[] combos =
            {
                comboChoice1, comboChoice2, comboChoice3, comboChoice4, comboChoice5,
                comboChoice6, comboChoice7, comboChoice8, comboChoice9, comboChoice10
            };

            for (int i = 0; i < _pageSize; i++)
            {
                var tag = pictureBoxes[i].Tag as ImageDto;
                if (tag == null) continue;

                var choice = combos[i].SelectedItem?.ToString() ?? "未选择";
                if (choice == "未选择") continue;

                var dto = new { ImageId = tag.Id, Choice = choice };
                var resp = await _client.PostAsJsonAsync("api/selections", dto);

                if (!resp.IsSuccessStatusCode)
                {
                    MessageBox.Show($"上报失败: 图片 {tag.Id} => {resp.StatusCode}");
                }
            }

            _page++;
            await LoadPageAsync();
        }


        private async void btnNext_Click(object sender, EventArgs e)
        {
            _page++;
            await LoadPageAsync();
        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            if (_page > 1)
            {
                _page--;
                await LoadPageAsync();
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pb && pb.Tag is ImageDto dto && pb.Image != null)
            {
                using var dlg = new Form
                {
                    Text = dto.FileName,
                    StartPosition = FormStartPosition.CenterParent,
                    ClientSize = new Size(900, 600)
                };
                var big = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    Image = (Image)pb.Image.Clone(),
                    SizeMode = PictureBoxSizeMode.Zoom
                };
                dlg.Controls.Add(big);
                dlg.ShowDialog();
                big.Image.Dispose();
            }
        }
        private void btnLogout_Click(object sender, EventArgs e)
        {
            // 清除 Token 或登录状态
            Program.AuthToken = null;

            // 返回登录窗口
            LoginForm login = new LoginForm();
            login.Show();

            this.Hide();
        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {

        }
    }

    // 简单 DTO，可单独放文件
    public class ImageDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = "";
        public string Url { get; set; } = "";
        public string Metadata { get; set; } = "";
    }
}
