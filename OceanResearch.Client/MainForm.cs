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
                    // 在成功设置 pictureBoxes[i].Image 后或即便没有图片也设置视觉状态：
                    var selectedChoice = img.SelectedChoice; // null 或 "最清晰"/"研究价值高"/"剔除"
                    if (!string.IsNullOrEmpty(selectedChoice))
                    {
                        // 把 combo 设为该值（如果 Combo 没有，先添加）
                        if (!combos[i].Items.Contains(selectedChoice))
                            combos[i].Items.Add(selectedChoice);
                        combos[i].SelectedItem = selectedChoice;

                        // 如果选项是“最清晰”，把图片视为“选中”
                        bool isSelected = selectedChoice == "最清晰";
                        SetPictureSelectedVisual(pictureBoxes[i], isSelected);
                    }
                    else
                    {
                        combos[i].SelectedIndex = 0; // "未选择"
                        SetPictureSelectedVisual(pictureBoxes[i], false);
                    }

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
        private void SetPictureSelectedVisual(PictureBox pb, bool selected)
        {
            if (pb == null) return;
            if (selected)
            {
                pb.BorderStyle = BorderStyle.Fixed3D;
                pb.BackColor = Color.FromArgb(200, 230, 255); // 淡蓝
            }
            else
            {
                pb.BorderStyle = BorderStyle.FixedSingle;
                pb.BackColor = Color.Transparent;
            }

            // 在 picture 的父 panel 下标显示勾（Label），用名字区分
            var panel = pb.Parent as Panel;
            if (panel == null) return;

            var lbl = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Name == pb.Name + "_chk");
            if (lbl == null)
            {
                lbl = new Label
                {
                    Name = pb.Name + "_chk",
                    AutoSize = true,
                    ForeColor = Color.Green,
                    Font = new Font("Microsoft YaHei", 14, FontStyle.Bold),
                    BackColor = Color.Transparent
                };
                // 放到右上角内
                lbl.Location = new Point(panel.ClientSize.Width - 30, 4);
                lbl.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                panel.Controls.Add(lbl);
                lbl.BringToFront();
            }

            lbl.Text = selected ? "√ 最清晰" : "";
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            await SubmitSelectionsAsync();
        }

        //private async Task SubmitSelectionsAsync()
        //{
        //    PictureBox[] pictureBoxes =
        //    {
        //        pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
        //        pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10
        //    };

        //    ComboBox[] combos =
        //    {
        //        comboChoice1, comboChoice2, comboChoice3, comboChoice4, comboChoice5,
        //        comboChoice6, comboChoice7, comboChoice8, comboChoice9, comboChoice10
        //    };

        //    for (int i = 0; i < _pageSize; i++)
        //    {
        //        var tag = pictureBoxes[i].Tag as ImageDto;
        //        if (tag == null) continue;

        //        var choice = combos[i].SelectedItem?.ToString() ?? "未选择";
        //        if (choice == "未选择") continue;

        //        var dto = new { ImageId = tag.Id, Choice = choice };
        //        var resp = await _client.PostAsJsonAsync("api/selections", dto);

        //        if (!resp.IsSuccessStatusCode)
        //        {
        //            MessageBox.Show($"上报失败: 图片 {tag.Id} => {resp.StatusCode}");
        //        }
        //    }

        //    _page++;
        //    await LoadPageAsync();
        //}
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

            var list = new List<object>();
            for (int i = 0; i < _pageSize; i++)
            {
                var tag = pictureBoxes[i].Tag as ImageDto;
                if (tag == null) continue;

                var choice = combos[i].SelectedItem?.ToString() ?? "未选择";
                // 我们只保存非 "未选择" 的项；如果想要保存“取消选择”也写入后端，可改逻辑
                //if (choice == "未选择") continue;

                list.Add(new { ImageId = tag.Id, Choice = choice });
            }

            if (list.Count == 0)
            {
                MessageBox.Show("没有选择需要提交。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //var resp = await _client.PostAsJsonAsync("api/selections/bulk", list);
            //if (!resp.IsSuccessStatusCode)
            //{
            //    MessageBox.Show($"上报失败: {resp.StatusCode}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //MessageBox.Show("提交成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //// 重新加载当前页以获取最新持久化状态（后端会返回每张图片当前用户的 SelectedChoice）
            //await LoadPageAsync();
            try
            {
                var resp = await _client.PostAsJsonAsync("api/selections/bulk", list);
                if (!resp.IsSuccessStatusCode)
                {
                    var body = "";
                    try { body = await resp.Content.ReadAsStringAsync(); } catch { }
                    MessageBox.Show($"上报失败: {resp.StatusCode}\n{body}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // 成功后刷新当前页（从后端获取最新 SelectedChoice 并重绘）
                await LoadPageAsync();
                MessageBox.Show("提交并刷新成功！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"提交异常: {ex.Message}", "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        //private void pictureBox_Click(object sender, EventArgs e)
        //{
        //    //if (sender is PictureBox pb && pb.Tag is ImageDto dto && pb.Image != null)
        //    //{
        //    //    using var dlg = new Form
        //    //    {
        //    //        Text = dto.FileName,
        //    //        StartPosition = FormStartPosition.CenterParent,
        //    //        ClientSize = new Size(900, 600)
        //    //    };
        //    //    var big = new PictureBox
        //    //    {
        //    //        Dock = DockStyle.Fill,
        //    //        Image = (Image)pb.Image.Clone(),
        //    //        SizeMode = PictureBoxSizeMode.Zoom
        //    //    };
        //    //    dlg.Controls.Add(big);
        //    //    dlg.ShowDialog();
        //    //    big.Image.Dispose();
        //    //}
        //    PictureBox pb = sender as PictureBox;
        //    if (pb == null) return;

        //    int index = int.Parse(pb.Name.Replace("pictureBox", "")); // 获取图片编号
        //    imageSelections[index] = !imageSelections[index]; // 切换选中状态

        //    // 更新边框和标识
        //    UpdatePictureBoxVisual(pb, imageSelections[index]);
        //}
        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (sender is not PictureBox pb) return;
            // 找到 index
            var name = pb.Name; // pictureBox1..10
            if (!int.TryParse(name.Replace("pictureBox", ""), out int idx)) return;
            int i = idx - 1;

            // 切换为“最清晰”或取消
            var currentSelected = pb.BorderStyle == BorderStyle.Fixed3D; // 我们用 Fixed3D 表示已选
            var newSel = !currentSelected;
            SetPictureSelectedVisual(pb, newSel);

            // 同步 combo：如果选中把 combo 设为 "最清晰"，否则设为 "未选择"
            var combos = new[] { comboChoice1, comboChoice2, comboChoice3, comboChoice4, comboChoice5,
                         comboChoice6, comboChoice7, comboChoice8, comboChoice9, comboChoice10 };
            if (newSel)
            {
                if (!combos[i].Items.Contains("最清晰")) combos[i].Items.Add("最清晰");
                combos[i].SelectedItem = "最清晰";
            }
            else
            {
                combos[i].SelectedIndex = 0; // 未选择
            }
        }

        private void UpdatePictureBoxVisual(PictureBox pb, bool selected)
        {
            if (selected)
            {
                pb.BorderStyle = BorderStyle.Fixed3D; // 显示选中边框
                pb.BackColor = Color.LightBlue; // 淡蓝色背景
            }
            else
            {
                pb.BorderStyle = BorderStyle.FixedSingle; // 默认边框
                pb.BackColor = Color.Transparent;
            }

            // 显示/隐藏√标识
            Panel parent = pb.Parent as Panel;
            Label lblCheck = parent.Controls.OfType<Label>().FirstOrDefault(c => c.Name == "lblCheck");
            if (lblCheck == null)
            {
                lblCheck = new Label();
                lblCheck.Name = "lblCheck";
                lblCheck.AutoSize = true;
                lblCheck.ForeColor = Color.Green;
                lblCheck.Font = new Font("微软雅黑", 14, FontStyle.Bold);
                lblCheck.Location = new Point(5, pb.Height - 25); // 图片底部
                parent.Controls.Add(lblCheck);
                lblCheck.BringToFront();
            }
            lblCheck.Text = selected ? "√" : "";
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

        private void MainForm_Load_2(object sender, EventArgs e)
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
        public string? SelectedChoice { get; set; } // 来自后端，null 表示未选
    }

}
