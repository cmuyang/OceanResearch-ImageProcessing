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
            // 先获取用户进度
            await LoadProgressAsync();
            await LoadPageAsync();
            lblUsername.Text = $"当前用户: {Program.CurrentUser}";
        }

        private async Task LoadProgressAsync()
        {
            try
            {
                var res = await _client.GetAsync("api/images/progress");
                if (res.IsSuccessStatusCode)
                {
                    var progress = await res.Content.ReadFromJsonAsync<ProgressDto>();
                    if (progress != null && progress.CurrentPage > 0)
                    {
                        _page = progress.CurrentPage;
                    }
                }
            }
            catch (Exception )
            {
                // 如果获取进度失败，从第一页开始
                _page = 1;
            }
        }

        private class ProgressDto
        {
            public int CurrentPage { get; set; }
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
            var panels = new[] { panel1, panel2, panel3, panel4, panel5, panel6, panel7, panel8, panel9, panel10 };

            // 清除所有"最清晰"标记
            foreach (var pb in pictureBoxes)
            {
                SetPictureSelectedVisual(pb, false);
            }

            for (int i = 0; i < _pageSize; i++)
            {
                var combo = combos[i];
                var panel = panels[i];

                // 隐藏不再使用的下拉框
                combo.Visible = false;

                // 清除旧的动态控件
                var oldControls = panel.Controls.OfType<CheckBox>().ToList();
                foreach (var ctrl in oldControls)
                {
                    panel.Controls.Remove(ctrl);
                    ctrl.Dispose();
                }

                // 创建复选框控件（放在原来下拉框的位置）
                var chkResearch = new CheckBox
                {
                    Text = "有研究价值",
                    AutoSize = true,
                    Location = new Point(combo.Location.X, combo.Location.Y),
                    Tag = i,
                    Name = $"chkResearch{i + 1}"
                };

                var chkRemove = new CheckBox
                {
                    Text = "剔除",
                    AutoSize = true,
                    Location = new Point(combo.Location.X + 120, combo.Location.Y),
                    Tag = i,
                    Name = $"chkRemove{i + 1}"
                };

                panel.Controls.Add(chkResearch);
                panel.Controls.Add(chkRemove);
                chkResearch.BringToFront();
                chkRemove.BringToFront();

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

                    // 设置"最清晰"标记
                    if (img.IsClearest)
                    {
                        SetPictureSelectedVisual(pictureBoxes[i], true);
                    }

                    // 设置复选框状态
                    chkResearch.Checked = img.HasResearchValue;
                    chkRemove.Checked = img.ShouldRemove;

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

            var panels = new[] { panel1, panel2, panel3, panel4, panel5, panel6, panel7, panel8, panel9, panel10 };

            var list = new List<object>();
            for (int i = 0; i < _pageSize; i++)
            {
                var tag = pictureBoxes[i].Tag as ImageDto;
                if (tag == null) continue;

                // 获取"最清晰"状态
                bool isClearest = pictureBoxes[i].BorderStyle == BorderStyle.Fixed3D;

                // 获取复选框状态
                var panel = panels[i];
                var chkResearch = panel.Controls.OfType<CheckBox>().FirstOrDefault(c => c.Text == "有研究价值");
                var chkRemove = panel.Controls.OfType<CheckBox>().FirstOrDefault(c => c.Text == "剔除");

                bool hasResearchValue = chkResearch?.Checked ?? false;
                bool shouldRemove = chkRemove?.Checked ?? false;

                list.Add(new 
                { 
                    ImageId = tag.Id, 
                    Metadata = tag.Metadata,
                    IsClearest = isClearest,
                    HasResearchValue = hasResearchValue,
                    ShouldRemove = shouldRemove
                });
            }

            if (list.Count == 0)
            {
                MessageBox.Show("没有选择需要提交。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var url = $"api/selections/bulk?currentPage={_page}";
                var resp = await _client.PostAsJsonAsync(url, list);
                if (!resp.IsSuccessStatusCode)
                {
                    var body = "";
                    try { body = await resp.Content.ReadAsStringAsync(); } catch { }
                    MessageBox.Show($"上报失败: {resp.StatusCode}\n{body}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // 成功后刷新当前页（从后端获取最新状态并重绘）
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

        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (sender is not PictureBox pb) return;
            // 找到 index
            var name = pb.Name; // pictureBox1..10
            if (!int.TryParse(name.Replace("pictureBox", ""), out int idx)) return;
            int i = idx - 1;

            // 如果当前图片已经是"最清晰"，则取消
            var currentSelected = pb.BorderStyle == BorderStyle.Fixed3D;
            if (currentSelected)
            {
                SetPictureSelectedVisual(pb, false);
                return;
            }

            // 清除同一页其他图片的"最清晰"标记
            var pictureBoxes = new[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
                         pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10 };
            for (int j = 0; j < pictureBoxes.Length; j++)
            {
                if (j != i && pictureBoxes[j].BorderStyle == BorderStyle.Fixed3D)
                {
                    SetPictureSelectedVisual(pictureBoxes[j], false);
                }
            }

            // 设置当前图片为"最清晰"
            SetPictureSelectedVisual(pb, true);
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

            // 断言父控件应为 Panel；使用 null-forgiving 告知编译器此处不可为 null
            var parent = pb.Parent as Panel;
            System.Diagnostics.Debug.Assert(parent != null, "PictureBox 的 Parent 应为 Panel");
            Panel parentPanel = parent!;

            Label? lblCheck = parentPanel.Controls.OfType<Label>().FirstOrDefault(c => c.Name == "lblCheck");
            if (lblCheck == null)
            {
                lblCheck = new Label();
                lblCheck.Name = "lblCheck";
                lblCheck.AutoSize = true;
                lblCheck.ForeColor = Color.Green;
                lblCheck.Font = new Font("微软雅黑", 14, FontStyle.Bold);
                lblCheck.Location = new Point(5, pb.Height - 25); // 图片底部
                parentPanel.Controls.Add(lblCheck);
                lblCheck.BringToFront();
            }
            lblCheck.Text = selected ? "√" : "";
        }
        private void btnLogout_Click(object sender, EventArgs e)
        {
            // 清除 Token 或登录状态
            Program.AuthToken = null;
            Program.CurrentUser = null;
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

        private void btnStats_Click(object sender, EventArgs e)
        {
            var statsForm = new StatsForm(_client);
            statsForm.ShowDialog();
        }
    }

    // 简单 DTO，可单独放文件
    public class ImageDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = "";
        public string Url { get; set; } = "";
        public string Metadata { get; set; } = "";
        public bool IsClearest { get; set; }      // 是否标记为"最清晰"
        public bool HasResearchValue { get; set; } // 是否标记为"有研究价值"
        public bool ShouldRemove { get; set; }    // 是否标记为"剔除"
    }

}
