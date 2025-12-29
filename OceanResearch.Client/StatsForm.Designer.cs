namespace OceanReseach.Client
{
    partial class StatsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.ComboBox cmbFilter;
        private System.Windows.Forms.ComboBox cmbSort;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.Label lblSort;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.DataGridView dgvStats;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label lblPageInfo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelTop = new Panel();
            btnExportCsv = new Button();
            btnQuery = new Button();
            cmbSort = new ComboBox();
            lblSort = new Label();
            cmbFilter = new ComboBox();
            lblFilter = new Label();
            dgvStats = new DataGridView();
            panelBottom = new Panel();
            btnNext = new Button();
            lblPageInfo = new Label();
            btnPrev = new Button();
            panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvStats).BeginInit();
            panelBottom.SuspendLayout();
            SuspendLayout();
            // 
            // panelTop
            // 
            panelTop.Controls.Add(btnExportCsv);
            panelTop.Controls.Add(btnQuery);
            panelTop.Controls.Add(cmbSort);
            panelTop.Controls.Add(lblSort);
            panelTop.Controls.Add(cmbFilter);
            panelTop.Controls.Add(lblFilter);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Padding = new Padding(10);
            panelTop.Size = new Size(1346, 60);
            panelTop.TabIndex = 1;
            // 
            // btnExportCsv
            // 
            btnExportCsv.Location = new Point(993, 14);
            btnExportCsv.Name = "btnExportCsv";
            btnExportCsv.Size = new Size(120, 34);
            btnExportCsv.TabIndex = 0;
            btnExportCsv.Text = "导出当前页CSV";
            btnExportCsv.Click += btnExportCsv_Click;
            // 
            // btnQuery
            // 
            btnQuery.Location = new Point(854, 16);
            btnQuery.Name = "btnQuery";
            btnQuery.Size = new Size(80, 30);
            btnQuery.TabIndex = 1;
            btnQuery.Text = "查询";
            btnQuery.Click += btnQuery_Click;
            // 
            // cmbSort
            // 
            cmbSort.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSort.Location = new Point(520, 16);
            cmbSort.Name = "cmbSort";
            cmbSort.Size = new Size(271, 32);
            cmbSort.TabIndex = 2;
            // 
            // lblSort
            // 
            lblSort.AutoSize = true;
            lblSort.Location = new Point(448, 20);
            lblSort.Name = "lblSort";
            lblSort.Size = new Size(50, 24);
            lblSort.TabIndex = 3;
            lblSort.Text = "排序:";
            // 
            // cmbFilter
            // 
            cmbFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilter.Location = new Point(84, 16);
            cmbFilter.Name = "cmbFilter";
            cmbFilter.Size = new Size(266, 32);
            cmbFilter.TabIndex = 4;
            // 
            // lblFilter
            // 
            lblFilter.AutoSize = true;
            lblFilter.Location = new Point(12, 20);
            lblFilter.Name = "lblFilter";
            lblFilter.Size = new Size(50, 24);
            lblFilter.TabIndex = 5;
            lblFilter.Text = "筛选:";
            // 
            // dgvStats
            // 
            dgvStats.AllowUserToAddRows = false;
            dgvStats.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStats.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStats.Dock = DockStyle.Fill;
            dgvStats.Location = new Point(0, 60);
            dgvStats.Name = "dgvStats";
            dgvStats.ReadOnly = true;
            dgvStats.RowHeadersWidth = 62;
            dgvStats.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStats.Size = new Size(1346, 668);
            dgvStats.TabIndex = 0;
            // 
            // panelBottom
            // 
            panelBottom.Controls.Add(btnNext);
            panelBottom.Controls.Add(lblPageInfo);
            panelBottom.Controls.Add(btnPrev);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 728);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(1346, 46);
            panelBottom.TabIndex = 2;
            // 
            // btnNext
            // 
            btnNext.Location = new Point(204, 10);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(75, 29);
            btnNext.TabIndex = 0;
            btnNext.Text = "下一页";
            btnNext.Click += btnNext_Click;
            // 
            // lblPageInfo
            // 
            lblPageInfo.AutoSize = true;
            lblPageInfo.Location = new Point(113, 15);
            lblPageInfo.Name = "lblPageInfo";
            lblPageInfo.Size = new Size(67, 24);
            lblPageInfo.TabIndex = 1;
            lblPageInfo.Text = "第 1 页";
            // 
            // btnPrev
            // 
            btnPrev.Location = new Point(12, 10);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(75, 29);
            btnPrev.TabIndex = 2;
            btnPrev.Text = "上一页";
            btnPrev.Click += btnPrev_Click;
            // 
            // StatsForm
            // 
            ClientSize = new Size(1346, 774);
            Controls.Add(dgvStats);
            Controls.Add(panelTop);
            Controls.Add(panelBottom);
            Name = "StatsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "标注统计概览";
            Load += StatsForm_Load_1;
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvStats).EndInit();
            panelBottom.ResumeLayout(false);
            panelBottom.PerformLayout();
            ResumeLayout(false);
        }
    }
}  