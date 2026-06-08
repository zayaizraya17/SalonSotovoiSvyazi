using System;
using System.Drawing;
using System.Windows.Forms;
using MobileStoreApp.Data;

namespace MobileStoreApp.Forms
{
    public partial class ReportsForm : Form
    {
        private DatabaseHelper _db;
        private DataGridView _grid;
        private readonly Color _primaryColor = Color.FromArgb(108, 92, 231); // Фиолетовый
        private readonly Color _secondaryColor = Color.FromArgb(59, 130, 246); // Голубой
        private readonly Color _accentPink = Color.FromArgb(236, 72, 153); // Розовый
        private readonly Color _accentPurple = Color.FromArgb(139, 92, 246); // Светло-фиолетовый
        private readonly Color _accentCyan = Color.FromArgb(34, 211, 238); // Голубой циан
        private readonly Color _bgColor = Color.FromArgb(250, 245, 255); // Светлый фон с розовым оттенком
        private readonly Color _textDark = Color.FromArgb(76, 29, 149); // Тёмно-фиолетовый текст

        public ReportsForm(DatabaseHelper db)
        {
            _db = db;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "📊 Отчёты";
            this.Size = new System.Drawing.Size(1100, 700);
            this.BackColor = _bgColor;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 600);

            // Верхняя панель с кнопками и фильтрами
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 120,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(30, 15, 20, 15),
                BackColor = Color.FromArgb(253, 242, 248)
            };

            var btnSalesReport = CreateStyledButton("📈 Отчёт по продажам", _accentPurple, (s, e) => LoadSalesReport());
            var btnInventory = CreateStyledButton("📦 Остатки товаров", _secondaryColor, (s, e) => LoadInventoryReport());
            btnPanel.Controls.Add(btnSalesReport);
            btnPanel.Controls.Add(btnInventory);
            
            // Панель фильтров для отчёта по продажам
            var filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = _bgColor,
                Padding = new Padding(30, 10, 30, 10)
            };
            
            var lblPeriod = new Label
            {
                Text = "📅 Период:",
                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                ForeColor = _textDark,
                AutoSize = true,
                Location = new Point(0, 12)
            };
            
            var dtpStart = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Location = new Point(80, 10),
                Width = 150
            };
            dtpStart.Value = DateTime.Now.AddMonths(-1);
            
            var lblTo = new Label
            {
                Text = "—",
                Font = new Font("Segoe UI", 11),
                ForeColor = _textDark,
                AutoSize = true,
                Location = new Point(235, 14)
            };
            
            var dtpEnd = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Location = new Point(255, 10),
                Width = 150
            };
            dtpEnd.Value = DateTime.Now;
            
            var btnFilter = new Button
            {
                Text = "Применить фильтр",
                Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = _accentPink,
                ForeColor = Color.White,
                Size = new Size(140, 35),
                Location = new Point(420, 8),
                Cursor = Cursors.Hand
            };
            btnFilter.FlatAppearance.BorderSize = 0;
            btnFilter.Click += (s, e) => LoadExtendedSalesReport(dtpStart.Value, dtpEnd.Value);

            filterPanel.Controls.Add(lblPeriod);
            filterPanel.Controls.Add(dtpStart);
            filterPanel.Controls.Add(lblTo);
            filterPanel.Controls.Add(dtpEnd);
            filterPanel.Controls.Add(btnFilter);

            // Таблица
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false
            };

            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = _accentPurple;
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _grid.ColumnHeadersHeight = 40;
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(215, 189, 226);
            _grid.DefaultCellStyle.SelectionForeColor = _textDark;
            _grid.RowTemplate.Height = 35;

            // Нижняя панель
            var bottomPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(253, 242, 248)
            };

            var btnClose = CreateStyledButton("❌ Закрыть", _accentPurple, (s, e) => this.Close());
            bottomPanel.Controls.Add(btnClose);

            this.Controls.Add(_grid);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(btnPanel);
            this.Controls.Add(filterPanel);
        }

        private Button CreateStyledButton(string text, Color color, EventHandler clickHandler)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Size = new Size(200, 45),
                Margin = new Padding(10, 10, 10, 10),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.2f);
            btn.Click += clickHandler;
            return btn;
        }

        private void LoadSalesReport()
        {
            _grid.DataSource = _db.GetSalesReport();
        }
        
        private void LoadExtendedSalesReport(DateTime startDate, DateTime endDate)
        {
            _grid.DataSource = _db.GetExtendedSalesReport(startDate, endDate);
        }
        
        private void LoadInventoryReport()
        {
            _grid.DataSource = _db.GetInventoryStatus();
        }
    }
}