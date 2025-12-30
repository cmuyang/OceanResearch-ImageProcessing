using OceanReseach.Client.OceanReseach.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OceanReseach.Client
{
    public partial class StatsForm : Form
    {
        private readonly HttpClient _client;
        private int _currentPage = 1;
        private int _pageSize = 20;
        private int _totalCount = 0;
        private List<ImageSummaryDto> _currentData = new();

        // 筛选选项映射
        private readonly Dictionary<string, string> _filterMap = new()
        {
            { "显示所有", "all" },
            { "排除已剔除", "exclude_removed" },
            { "只看最清晰", "only_clearest" },
            { "只看有研究价值", "only_research" },
            { "有价值但非最清晰", "research_not_clearest" }
        };

        // 排序选项映射
        private readonly Dictionary<string, string> _sortMap = new()
        {
            { "最近更新", "recent" },
            { "最清晰票数", "clearest" },
            { "研究价值票数", "research" },
            { "剔除票数", "remove" }
        };

        public StatsForm(HttpClient client)
        {
            InitializeComponent();
            // 设置窗口启动位置为屏幕中央
            this.StartPosition = FormStartPosition.CenterScreen;

            _client = client;

            // 初始化下拉框
            foreach (var key in _filterMap.Keys) cmbFilter.Items.Add(key);
            cmbFilter.SelectedIndex = 0;

            foreach (var key in _sortMap.Keys) cmbSort.Items.Add(key);
            cmbSort.SelectedIndex = 0;

            this.Load += StatsForm_Load;
        }

        private async void StatsForm_Load(object? _sender, EventArgs e)
        {
            //object s = sender!;
            await LoadStatsAsync();
        }

        //private async Task LoadStatsAsync()
        //{
        //    try
        //    {
        //        btnQuery.Enabled = false;

        //        // 获取参数
        //        string filterKey = cmbFilter.SelectedItem?.ToString() ?? "显示所有";
        //        string sortKey = cmbSort.SelectedItem?.ToString() ?? "最近更新";

        //        string filterVal = _filterMap[filterKey];
        //        string sortVal = _sortMap[sortKey];

        //        // 构建 URL
        //        string url = $"api/stats?page={_currentPage}&pageSize={_pageSize}&filter={filterVal}&sortBy={sortVal}";

        //        var response = await _client.GetAsync(url);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var result = await response.Content.ReadFromJsonAsync<StatsResponse>();
        //            if (result != null)
        //            {
        //                _currentData = result.Items;
        //                _totalCount = result.TotalCount;

        //                // 绑定数据
        //                dgvStats.DataSource = _currentData;

        //                // 更新分页UI
        //                int totalPages = (int)Math.Ceiling((double)_totalCount / _pageSize);
        //                if (totalPages < 1) totalPages = 1;
        //                lblPageInfo.Text = $"第 {_currentPage} / {totalPages} 页 (共 {_totalCount} 条)";

        //                btnPrev.Enabled = _currentPage > 1;
        //                btnNext.Enabled = _currentPage < totalPages;
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show($"获取数据失败: {response.StatusCode}", "错误");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"加载异常: {ex.Message}", "错误");
        //    }
        //    finally
        //    {
        //        btnQuery.Enabled = true;
        //    }
        //}
        private async Task LoadStatsAsync()
        {
            try
            {
                btnQuery.Enabled = false;

                // 1. 获取参数
                string filterKey = cmbFilter.SelectedItem?.ToString() ?? "显示所有";
                string sortKey = cmbSort.SelectedItem?.ToString() ?? "最近更新";

                string filterVal = _filterMap.ContainsKey(filterKey) ? _filterMap[filterKey] : "all";
                string sortVal = _sortMap.ContainsKey(sortKey) ? _sortMap[sortKey] : "recent";

                // 2. 构建并打印完整 URL (用于检查 BaseAddress 是否正确)
                string relativeUrl = $"api/stats?page={_currentPage}&pageSize={_pageSize}&filter={filterVal}&sortBy={sortVal}";

                // 调试：检查 BaseAddress
                if (_client.BaseAddress == null)
                {
                    MessageBox.Show("错误：HttpClient 的 BaseAddress 未设置！", "配置错误");
                    return;
                }

                // 3. 发送请求
                // 建议：如果你之前的代码使用了 JWT，请确保 _client.DefaultRequestHeaders.Authorization 已设置
                var response = await _client.GetAsync(relativeUrl);

                // 4. 读取原始内容 (无论成功失败，先作为字符串读取，方便调试)
                string rawContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        // 5. 尝试反序列化
                        // 增加 CaseInsensitive 选项，防止因为大小写问题导致数据为空
                        var options = new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        var result = System.Text.Json.JsonSerializer.Deserialize<StatsResponse>(rawContent, options);

                        if (result != null)
                        {
                            _currentData = result.Items ?? new List<ImageSummaryDto>(); // 防止 Items 为 null
                            _totalCount = result.TotalCount;

                            // 绑定数据
                            dgvStats.DataSource = null; // 先清空，触发刷新
                            dgvStats.DataSource = _currentData;

                            // 更新分页UI
                            int totalPages = (int)Math.Ceiling((double)_totalCount / _pageSize);
                            if (totalPages < 1) totalPages = 1;
                            lblPageInfo.Text = $"第 {_currentPage} / {totalPages} 页 (共 {_totalCount} 条)";

                            btnPrev.Enabled = _currentPage > 1;
                            btnNext.Enabled = _currentPage < totalPages;

                            // 调试：如果数据列表是空的，提示一下
                            if (_currentData.Count == 0)
                            {
                                // 这是一个非阻塞的提示，可以在调试器输出窗口看到
                                System.Diagnostics.Debug.WriteLine($"请求成功，但返回了 0 条数据。原始内容: {rawContent}");
                            }
                        }
                        else
                        {
                            MessageBox.Show("解析结果为 null。原始内容:\n" + rawContent, "解析错误");
                        }
                    }
                    catch (Exception jsonEx)
                    {
                        // 捕获专门的 JSON 格式错误
                        MessageBox.Show($"JSON 解析异常: {jsonEx.Message}\n\n服务器返回的原始数据:\n{rawContent}", "数据格式错误");
                    }
                }
                else
                {
                    // 6. 如果状态码不是 200，显示详细的服务端错误信息
                    string errorMsg = $"状态码: {(int)response.StatusCode} ({response.ReasonPhrase})\n";
                    errorMsg += $"请求地址: {_client.BaseAddress}{relativeUrl}\n\n";
                    errorMsg += $"服务端返回内容:\n{rawContent}";

                    MessageBox.Show(errorMsg, "请求失败");
                }
            }
            catch (Exception ex)
            {
                // 7. 捕获网络或其他底层异常，显示堆栈
                MessageBox.Show($"发生未处理异常:\n{ex.ToString()}", "系统错误");
            }
            finally
            {
                btnQuery.Enabled = true;
            }
        }
        private async void btnQuery_Click(object sender, EventArgs e)
        {
            _currentPage = 1; // 重置为第一页
            await LoadStatsAsync();
        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await LoadStatsAsync();
            }
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            _currentPage++;
            await LoadStatsAsync();
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            if (_currentData.Count == 0)
            {
                MessageBox.Show("当前没有数据可导出", "提示");
                return;
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "CSV 文件|*.csv";
            sfd.FileName = $"统计数据_{DateTime.Now:yyyyMMddHHmm}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sb = new StringBuilder();
                    // CSV 表头
                    sb.AppendLine("文件名,总标注数,最清晰票数,研究价值票数,剔除票数,最后更新时间,图片URL");

                    foreach (var item in _currentData)
                    {
                        var line = $"{Escape(item.Metadata)},{item.TotalAnnotations},{item.ClearestCount},{item.ResearchValueCount},{item.RemoveCount},{item.LastUpdated},{item.Url}";
                        sb.AppendLine(line);
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("导出成功！", "提示");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出失败: {ex.Message}", "错误");
                }
            }
        }

        private string Escape(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n"))
            {
                return $"\"{s.Replace("\"", "\"\"")}\"";
            }
            return s;
        }

        private void StatsForm_Load_1(object sender, EventArgs e)
        {

        }
    }



    namespace OceanReseach.Client
    {
        public class ImageSummaryDto
        {
            public string Metadata { get; set; } = ""; // 文件名
            public string Url { get; set; } = "";
            public int TotalAnnotations { get; set; }
            public int ClearestCount { get; set; }
            public int ResearchValueCount { get; set; }
            public int RemoveCount { get; set; }
            public DateTime LastUpdated { get; set; }
        }

        public class StatsResponse
        {
            public int TotalCount { get; set; }
            public string Filter { get; set; } = "";
            public string SortBy { get; set; } = "";
            public int Page { get; set; }
            public int PageSize { get; set; }
            public List<ImageSummaryDto> Items { get; set; } = new();
        }
    }
}