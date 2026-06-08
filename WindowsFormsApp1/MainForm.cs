using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MobileStoreApp.Data;
using MobileStoreApp.Forms;

namespace MobileStoreApp
{
    public partial class MainForm : Form
    {
        private DatabaseHelper _db;
        private Panel _headerPanel;
        private readonly Color _primaryColor = Color.FromArgb(26, 95, 160);
        private readonly Color _secondaryColor = Color.FromArgb(52, 152, 219);
        private readonly Color _accentGreen = Color.FromArgb(39, 174, 96);
        private readonly Color _accentOrange = Color.FromArgb(211, 84, 0);
        private readonly Color _accentPurple = Color.FromArgb(142, 68, 173);
        private readonly Color _bgColor = Color.FromArgb(245, 247, 249);
        private readonly Color _cardBg = Color.White;
        private readonly Color _textDark = Color.FromArgb(44, 62, 80);
        private readonly Color _textLight = Color.FromArgb(127, 140, 141);

        public MainForm()
        {
            InitializeComponent();
            _db = new DatabaseHelper();
            _db.Initialize();
            CreateMainMenu();
        }

        private void CreateMainMenu()
        {
            this.Text = "📱 Mobile Store";
            this.Size = new System.Drawing.Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 650);
            
            // Современный фон формы
            this.BackColor = _bgColor;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Верхняя панель с улучшенным градиентом
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 160,
                BackColor = Color.Transparent
            };
            _headerPanel.Paint += HeaderPanel_Paint;

            // Заголовок с тенью
            var headerLabel = new Label
            {
                Text = "📱 MOBILE STORE",
                Font = new Font("Segoe UI Variable Display", 36, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(50, 30),
                BackColor = Color.Transparent
            };

            var subtitleLabel = new Label
            {
                Text = "Салон сотовой связи • Система управления магазином",
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 230, 255),
                AutoSize = true,
                Location = new Point(55, 85),
                BackColor = Color.Transparent
            };

            // Основная панель с карточками
            var mainPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(50, 30, 50, 50),
                BackColor = Color.Transparent,
                AutoScroll = true
            };

            AddMenuButton(mainPanel, "📱", "Телефоны", "Управление ассортиментом товаров", _secondaryColor, () => new PhonesForm(_db).ShowDialog());
            AddMenuButton(mainPanel, "📦", "Поставки", "Приём товаров от поставщиков", _accentGreen, () => new DeliveriesForm(_db).ShowDialog());
            AddMenuButton(mainPanel, "💰", "Продажи", "Оформление продаж клиентам", _accentOrange, () => new SalesForm(_db).ShowDialog());
            AddMenuButton(mainPanel, "📊", "Отчёты", "Аналитика и статистика продаж", _accentPurple, () => new ReportsForm(_db).ShowDialog());

            _headerPanel.Controls.Add(headerLabel);
            _headerPanel.Controls.Add(subtitleLabel);
            
            this.Controls.Add(mainPanel);
            this.Controls.Add(_headerPanel);
        }

        private void HeaderPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, _headerPanel.Width, _headerPanel.Height);
            
            // Улучшенный градиент с более глубокими цветами
            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect,
                _primaryColor,
                _secondaryColor,
                LinearGradientMode.Horizontal))
            {
                // Добавляем эффект свечения
                ColorBlend blend = new ColorBlend();
                blend.Colors = new Color[] { _primaryColor, _secondaryColor, Color.FromArgb(64, 169, 228) };
                blend.Positions = new float[] { 0.0f, 0.7f, 1.0f };
                brush.InterpolationColors = blend;
                
                g.FillRectangle(brush, rect);
            }
            
            // Добавляем тонкую линию внизу панели
            using (SolidBrush lineBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 255)))
            {
                g.FillRectangle(lineBrush, 0, _headerPanel.Height - 3, _headerPanel.Width, 3);
            }
        }

        private void AddMenuButton(FlowLayoutPanel panel, string icon, string title, string description, Color color, Action clickAction)
        {
            var card = new Panel
            {
                Size = new System.Drawing.Size(300, 220),
                Margin = new Padding(25),
                BackColor = _cardBg,
                Cursor = Cursors.Hand,
                Tag = color
            };
            
            // Скруглённые углы через Region
            int radius = 16;
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(card.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(card.Width - radius, card.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, card.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            card.Region = new Region(path);

            var iconPanel = new Panel
            {
                Size = new System.Drawing.Size(300, 100),
                Location = new Point(0, 0),
                BackColor = color,
                Cursor = Cursors.Hand
            };
            
            // Скругляем верхние углы иконки
            System.Drawing.Drawing2D.GraphicsPath iconPath = new System.Drawing.Drawing2D.GraphicsPath();
            iconPath.AddArc(0, 0, radius, radius, 180, 90);
            iconPath.AddArc(iconPanel.Width - radius, 0, radius, radius, 270, 90);
            iconPath.AddLine(iconPanel.Width, radius, iconPanel.Width, iconPanel.Height);
            iconPath.AddLine(iconPanel.Width, iconPanel.Height, 0, iconPanel.Height);
            iconPath.CloseFigure();
            iconPanel.Region = new Region(iconPath);

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 48, FontStyle.Regular),
                AutoSize = false,
                Size = new System.Drawing.Size(90, 90),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(105, 5),
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI Semibold", 18, FontStyle.Bold),
                ForeColor = _textDark,
                AutoSize = false,
                Size = new System.Drawing.Size(260, 40),
                Location = new Point(20, 108),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            var descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 11),
                ForeColor = _textLight,
                AutoSize = false,
                Size = new System.Drawing.Size(260, 30),
                Location = new Point(20, 145),
                BackColor = Color.Transparent
            };

            var btn = new Button
            {
                Text = "Открыть  →",
                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Size = new System.Drawing.Size(260, 42),
                Location = new Point(20, 172),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.15f);
            btn.Click += (s, e) => clickAction();
            
            // Скругляем кнопку
            System.Drawing.Drawing2D.GraphicsPath btnPath = new System.Drawing.Drawing2D.GraphicsPath();
            int btnRadius = 8;
            btnPath.AddArc(btn.Location.X, btn.Location.Y, btnRadius, btnRadius, 180, 90);
            btnPath.AddArc(btn.Location.X + btn.Width - btnRadius, btn.Location.Y, btnRadius, btnRadius, 270, 90);
            btnPath.AddArc(btn.Location.X + btn.Width - btnRadius, btn.Location.Y + btn.Height - btnRadius, btnRadius, btnRadius, 0, 90);
            btnPath.AddArc(btn.Location.X, btn.Location.Y + btn.Height - btnRadius, btnRadius, btnRadius, 90, 90);
            btnPath.CloseFigure();
            btn.Region = new Region(btnPath);

            iconPanel.Controls.Add(iconLabel);
            card.Controls.Add(iconPanel);
            card.Controls.Add(titleLabel);
            card.Controls.Add(descLabel);
            card.Controls.Add(btn);

            // Эффекты при наведении с анимацией цвета
            card.MouseEnter += (s, e) =>
            {
                card.BackColor = Color.FromArgb(252, 254, 255);
                btn.BackColor = ControlPaint.Light(color, 0.12f);
                card.Padding = new Padding(0, 0, 0, 4);
            };
            card.MouseLeave += (s, e) =>
            {
                card.BackColor = _cardBg;
                btn.BackColor = color;
                card.Padding = new Padding(0);
            };

            panel.Controls.Add(card);
        }
    }
}