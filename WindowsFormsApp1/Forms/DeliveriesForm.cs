using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MobileStoreApp.Data;
using MobileStoreApp.Models;

namespace MobileStoreApp.Forms
{
    public partial class DeliveriesForm : Form
    {
        private DatabaseHelper _db;
        private DataGridView _grid;
        private Panel _headerPanel;

        public DeliveriesForm(DatabaseHelper db)
        {
            _db = db;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "📦 Поставки";
            this.Size = new System.Drawing.Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 550);
            this.BackColor = Color.FromArgb(236, 240, 241);

            // Верхняя панель с градиентом
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.Transparent
            };
            _headerPanel.Paint += HeaderPanel_Paint;

            var titleLabel = new Label
            {
                Text = "📦 Управление поставками",
                Font = new Font("Segoe UI Semibold", 22, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(30, 15),
                BackColor = Color.Transparent
            };

            var subtitleLabel = new Label
            {
                Text = "Приём товаров от поставщиков",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(174, 236, 205),
                AutoSize = true,
                Location = new Point(35, 52),
                BackColor = Color.Transparent
            };

            _headerPanel.Controls.Add(titleLabel);
            _headerPanel.Controls.Add(subtitleLabel);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(236, 240, 241)
            };

            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(46, 204, 113);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            _grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            _grid.ColumnHeadersHeight = 45;
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            _grid.DefaultCellStyle.Padding = new Padding(5);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(174, 236, 205);
            _grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(44, 62, 80);
            _grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            _grid.RowTemplate.Height = 42;
            _grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 253);

            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 60 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SupplierName", HeaderText = "Поставщик", Width = 180 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DeliveryDate", HeaderText = "Дата", Width = 140 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Brand", HeaderText = "Бренд", Width = 140 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Model", HeaderText = "Модель", Width = 180 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Count", HeaderText = "Кол-во", Width = 90 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalCost", HeaderText = "Стоимость (₽)", Width = 130 });

            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(20, 15, 20, 15),
                BackColor = Color.FromArgb(250, 252, 253)
            };

            var btnClose = CreateStyledButton("❌ Закрыть", Color.FromArgb(149, 165, 166), (s, e) => this.Close());
            var btnAdd = CreateStyledButton("➕ Добавить поставку", Color.FromArgb(46, 204, 113), (s, e) => AddItem());

            btnPanel.Controls.Add(btnClose);
            btnPanel.Controls.Add(btnAdd);

            this.Controls.Add(_grid);
            this.Controls.Add(btnPanel);
            this.Controls.Add(_headerPanel);

            LoadData();
        }

        private void HeaderPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, _headerPanel.Width, _headerPanel.Height);
            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect,
                Color.FromArgb(39, 174, 96),
                Color.FromArgb(46, 204, 113),
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(brush, rect);
            }
        }

        private Button CreateStyledButton(string text, Color color, EventHandler clickHandler)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Size = new Size(180, 50),
                Margin = new Padding(12, 10, 12, 10),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.15f);
            btn.Click += clickHandler;
            return btn;
        }

        private void LoadData()
        {
            _grid.DataSource = _db.GetDeliveries();
        }

        private void AddItem()
        {
            var form = new DeliveryEditForm(_db);
            if (form.ShowDialog() == DialogResult.OK)
                LoadData();
        }
    }

    public class DeliveryEditForm : Form
    {
        private DatabaseHelper _db;

        public DeliveryEditForm(DatabaseHelper db)
        {
            _db = db;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "➕ Добавить поставку";
            this.Size = new System.Drawing.Size(480, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(25)
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));

            var lblSupplier = new Label { Text = "Поставщик:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var txtSupplier = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblSupplier, 0, 0);
            layout.Controls.Add(txtSupplier, 1, 0);

            var lblBrand = new Label { Text = "Бренд:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var txtBrand = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblBrand, 0, 1);
            layout.Controls.Add(txtBrand, 1, 1);

            var lblModel = new Label { Text = "Модель:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var txtModel = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblModel, 0, 2);
            layout.Controls.Add(txtModel, 1, 2);

            var lblCount = new Label { Text = "Количество:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var txtCount = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblCount, 0, 3);
            layout.Controls.Add(txtCount, 1, 3);

            var lblCost = new Label { Text = "Стоимость:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var txtCost = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblCost, 0, 4);
            layout.Controls.Add(txtCost, 1, 4);

            var lblDate = new Label { Text = "Дата:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            var dtpDate = new DateTimePicker { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
            layout.Controls.Add(lblDate, 0, 5);
            layout.Controls.Add(dtpDate, 1, 5);

            var btnPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };

            var btnSave = new Button
            {
                Text = "💾 Сохранить",
                Width = 140,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnSave.Click += (s, e) =>
            {
                try
                {
                    var delivery = new Delivery
                    {
                        SupplierName = txtSupplier.Text,
                        Brand = txtBrand.Text,
                        Model = txtModel.Text,
                        Count = int.Parse(txtCount.Text),
                        TotalCost = decimal.Parse(txtCost.Text),
                        DeliveryDate = dtpDate.Value
                    };
                    _db.AddDelivery(delivery);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            var btnCancel = new Button
            {
                Text = "Отмена",
                Width = 140,
                Height = 40,
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(127, 140, 141);

            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnSave);

            layout.Controls.Add(btnPanel, 0, 6);
            layout.SetColumnSpan(btnPanel, 2);

            this.Controls.Add(layout);
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }
    }
}