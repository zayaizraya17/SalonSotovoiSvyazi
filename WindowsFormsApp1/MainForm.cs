using System;
using System.Drawing;
using System.Windows.Forms;
using MobileStoreApp.Data;
using MobileStoreApp.Forms;

namespace MobileStoreApp
{
    public partial class MainForm : Form
    {
        private DatabaseHelper _db;

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
            this.Size = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Заголовок
            var headerLabel = new Label
            {
                Text = "📱 MOBILE STORE",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                AutoSize = true,
                Location = new Point(50, 30)
            };

            var subtitleLabel = new Label
            {
                Text = "Салон сотовой связи",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(55, 80)
            };

            // Панель с кнопками
            var mainPanel = new FlowLayoutPanel
            {
                Location = new Point(50, 120),
                Size = new System.Drawing.Size(780, 420),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            AddMenuButton(mainPanel, "📱", "Телефоны", Color.FromArgb(52, 152, 219), () => new PhonesForm(_db).ShowDialog());
            AddMenuButton(mainPanel, "📦", "Поставки", Color.FromArgb(46, 204, 113), () => new DeliveriesForm(_db).ShowDialog());
            AddMenuButton(mainPanel, "💰", "Продажи", Color.FromArgb(230, 126, 34), () => new SalesForm(_db).ShowDialog());
            AddMenuButton(mainPanel, "📊", "Отчёты", Color.FromArgb(155, 89, 182), () => new ReportsForm(_db).ShowDialog());

            this.Controls.Add(headerLabel);
            this.Controls.Add(subtitleLabel);
            this.Controls.Add(mainPanel);
        }

        private void AddMenuButton(FlowLayoutPanel panel, string icon, string title, Color color, Action clickAction)
        {
            var card = new Panel
            {
                Size = new System.Drawing.Size(320, 150),
                Margin = new Padding(15),
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 36),
                AutoSize = false,
                Size = new System.Drawing.Size(80, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 35)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(110, 55)
            };

            var btn = new Button
            {
                Text = "Открыть →",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Size = new System.Drawing.Size(120, 35),
                Location = new Point(180, 100),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.2f);
            btn.Click += (s, e) => clickAction();

            card.Controls.Add(iconLabel);
            card.Controls.Add(titleLabel);
            card.Controls.Add(btn);

            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(245, 245, 245);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            panel.Controls.Add(card);
        }
    }
}