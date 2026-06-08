using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MobileStoreApp.Data;
using MobileStoreApp.Models;

namespace MobileStoreApp.Forms
{
    public partial class PhonesForm : Form
    {
        private DatabaseHelper _db;
        private DataGridView _grid;
        private Panel _headerPanel;
        private readonly Color _primaryColor = Color.FromArgb(26, 95, 160);
        private readonly Color _secondaryColor = Color.FromArgb(52, 152, 219);
        private readonly Color _accentGreen = Color.FromArgb(39, 174, 96);
        private readonly Color _accentOrange = Color.FromArgb(211, 84, 0);
        private readonly Color _accentRed = Color.FromArgb(231, 76, 60);
        private readonly Color _accentGray = Color.FromArgb(149, 165, 166);
        private readonly Color _bgColor = Color.FromArgb(245, 247, 249);
        private readonly Color _cardBg = Color.White;
        private readonly Color _textDark = Color.FromArgb(44, 62, 80);
        private readonly Color _textLight = Color.FromArgb(127, 140, 141);

        public PhonesForm(DatabaseHelper db)
        {
            _db = db;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "📱 Телефоны";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);
            this.BackColor = _bgColor;

            // Верхняя панель с улучшенным градиентом
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.Transparent
            };
            _headerPanel.Paint += HeaderPanel_Paint;

            var titleLabel = new Label
            {
                Text = "📱 Управление телефонами",
                Font = new Font("Segoe UI Variable Display", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(40, 18),
                BackColor = Color.Transparent
            };

            var subtitleLabel = new Label
            {
                Text = "Добавление, редактирование и удаление товаров",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(180, 220, 255),
                AutoSize = true,
                Location = new Point(45, 58),
                BackColor = Color.Transparent
            };

            _headerPanel.Controls.Add(titleLabel);
            _headerPanel.Controls.Add(subtitleLabel);

            // Таблица с современным стилем
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = _cardBg,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(236, 240, 241)
            };

            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = _secondaryColor;
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold);
            _grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            _grid.ColumnHeadersHeight = 50;
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            _grid.DefaultCellStyle.Padding = new Padding(8);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(174, 214, 241);
            _grid.DefaultCellStyle.SelectionForeColor = _textDark;
            _grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            _grid.RowTemplate.Height = 48;
            _grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 253);

            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 70 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Brand", HeaderText = "Бренд", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Model", HeaderText = "Модель", Width = 190 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Color", HeaderText = "Цвет", Width = 130 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Price", HeaderText = "Цена (₽)", Width = 130 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Кол-во", Width = 100 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IMEI", HeaderText = "IMEI", Width = 200 });

            // Нижняя панель с кнопками
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(30, 20, 30, 20),
                BackColor = Color.FromArgb(250, 252, 253)
            };

            var btnClose = CreateStyledButton("❌ Закрыть", _accentGray, (s, e) => this.Close());
            var btnDelete = CreateStyledButton("🗑️ Удалить", _accentRed, (s, e) => DeleteItem());
            var btnEdit = CreateStyledButton("✏️ Изменить", _accentOrange, (s, e) => EditItem());
            var btnAdd = CreateStyledButton("➕ Добавить телефон", _accentGreen, (s, e) => AddItem());

            btnPanel.Controls.Add(btnClose);
            btnPanel.Controls.Add(btnDelete);
            btnPanel.Controls.Add(btnEdit);
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
                _primaryColor,
                _secondaryColor,
                LinearGradientMode.Horizontal))
            {
                ColorBlend blend = new ColorBlend();
                blend.Colors = new Color[] { _primaryColor, _secondaryColor, Color.FromArgb(64, 169, 228) };
                blend.Positions = new float[] { 0.0f, 0.7f, 1.0f };
                brush.InterpolationColors = blend;
                g.FillRectangle(brush, rect);
            }
            
            // Тонкая линия внизу
            using (SolidBrush lineBrush = new SolidBrush(Color.FromArgb(80, 255, 255, 255)))
            {
                g.FillRectangle(lineBrush, 0, _headerPanel.Height - 2, _headerPanel.Width, 2);
            }
        }

        private Button CreateStyledButton(string text, Color color, EventHandler clickHandler)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Size = new Size(185, 55),
                Margin = new Padding(15, 12, 15, 12),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.15f);
            btn.Click += clickHandler;
            
            // Скругляем углы кнопки
            int radius = 10;
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            btn.Region = new Region(path);
            
            return btn;
        }

        private void LoadData()
        {
            _grid.DataSource = _db.GetPhones();
        }

        private void AddItem()
        {
            var form = new PhoneEditForm(_db, null);
            if (form.ShowDialog() == DialogResult.OK)
                LoadData();
        }

        private void EditItem()
        {
            if (_grid.CurrentRow != null)
            {
                var item = (Phone)_grid.CurrentRow.DataBoundItem;
                var form = new PhoneEditForm(_db, item);
                if (form.ShowDialog() == DialogResult.OK)
                    LoadData();
            }
        }

        private void DeleteItem()
        {
            if (_grid.CurrentRow != null)
            {
                var item = (Phone)_grid.CurrentRow.DataBoundItem;
                if (MessageBox.Show($"Удалить {item.Brand} {item.Model}?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _db.DeletePhone(item.Id);
                    LoadData();
                }
            }
        }
    }

    public class PhoneEditForm : Form
    {
        private DatabaseHelper _db;
        private Phone _item;
        private TextBox _txtBrand;
        private TextBox _txtModel;
        private TextBox _txtColor;
        private TextBox _txtPrice;
        private TextBox _txtQuantity;
        private TextBox _txtIMEI;
        private Panel _headerPanel;

        public PhoneEditForm(DatabaseHelper db, Phone item)
        {
            _db = db;
            _item = item ?? new Phone();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = _item.Id == 0 ? "➕ Добавить телефон" : "✏️ Изменить телефон";
            this.Size = new System.Drawing.Size(550, 580);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 247, 249);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Верхняя панель с градиентом
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.Transparent
            };
            _headerPanel.Paint += HeaderPanel_Paint;

            var titleLabel = new Label
            {
                Text = _item.Id == 0 ? "📱 Новый телефон" : "✏️ Редактирование",
                Font = new Font("Segoe UI Variable Display", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(30, 25),
                BackColor = Color.Transparent
            };
            _headerPanel.Controls.Add(titleLabel);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(40, 15, 40, 25)
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));

            var lblBrand = new Label { Text = "Бренд:", AutoSize = true, Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtBrand = CreateStyledTextBox(_item.Brand);
            layout.Controls.Add(lblBrand, 0, 0);
            layout.Controls.Add(_txtBrand, 1, 0);

            var lblModel = new Label { Text = "Модель:", AutoSize = true, Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtModel = CreateStyledTextBox(_item.Model);
            layout.Controls.Add(lblModel, 0, 1);
            layout.Controls.Add(_txtModel, 1, 1);

            var lblColor = new Label { Text = "Цвет:", AutoSize = true, Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtColor = CreateStyledTextBox(_item.Color);
            layout.Controls.Add(lblColor, 0, 2);
            layout.Controls.Add(_txtColor, 1, 2);

            var lblPrice = new Label { Text = "Цена (₽):", AutoSize = true, Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtPrice = CreateStyledTextBox(_item.Price.ToString());
            layout.Controls.Add(lblPrice, 0, 3);
            layout.Controls.Add(_txtPrice, 1, 3);

            var lblQuantity = new Label { Text = "Количество:", AutoSize = true, Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtQuantity = CreateStyledTextBox(_item.Quantity.ToString());
            layout.Controls.Add(lblQuantity, 0, 4);
            layout.Controls.Add(_txtQuantity, 1, 4);

            var lblIMEI = new Label { Text = "IMEI:", AutoSize = true, Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtIMEI = CreateStyledTextBox(_item.IMEI);
            layout.Controls.Add(lblIMEI, 0, 5);
            layout.Controls.Add(_txtIMEI, 1, 5);

            var btnPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 20, 0, 0)
            };

            var btnSave = new Button
            {
                Text = "💾 Сохранить",
                Width = 160,
                Height = 50,
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(32, 145, 80);
            btnSave.Click += BtnSave_Click;
            
            // Скругляем кнопку сохранения
            int radius = 10;
            System.Drawing.Drawing2D.GraphicsPath savePath = new System.Drawing.Drawing2D.GraphicsPath();
            savePath.AddArc(0, 0, radius, radius, 180, 90);
            savePath.AddArc(btnSave.Width - radius, 0, radius, radius, 270, 90);
            savePath.AddArc(btnSave.Width - radius, btnSave.Height - radius, radius, radius, 0, 90);
            savePath.AddArc(0, btnSave.Height - radius, radius, radius, 90, 90);
            btnSave.Region = new Region(savePath);

            var btnCancel = new Button
            {
                Text = "Отмена",
                Width = 160,
                Height = 50,
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(127, 140, 141);
            
            // Скругляем кнопку отмены
            System.Drawing.Drawing2D.GraphicsPath cancelPath = new System.Drawing.Drawing2D.GraphicsPath();
            cancelPath.AddArc(0, 0, radius, radius, 180, 90);
            cancelPath.AddArc(btnCancel.Width - radius, 0, radius, radius, 270, 90);
            cancelPath.AddArc(btnCancel.Width - radius, btnCancel.Height - radius, radius, radius, 0, 90);
            cancelPath.AddArc(0, btnCancel.Height - radius, radius, radius, 90, 90);
            btnCancel.Region = new Region(cancelPath);

            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnSave);

            layout.Controls.Add(btnPanel, 0, 6);
            layout.SetColumnSpan(btnPanel, 2);

            this.Controls.Add(layout);
            this.Controls.Add(_headerPanel);
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }
        
        private TextBox CreateStyledTextBox(string text)
        {
            return new TextBox 
            { 
                Text = text, 
                Dock = DockStyle.Fill, 
                Font = new Font("Segoe UI", 11),
                Padding = new Padding(10, 8, 10, 8),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(44, 62, 80)
            };
        }

        private void HeaderPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, _headerPanel.Width, _headerPanel.Height);
            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect,
                Color.FromArgb(26, 95, 160),
                Color.FromArgb(52, 152, 219),
                LinearGradientMode.Horizontal))
            {
                ColorBlend blend = new ColorBlend();
                blend.Colors = new Color[] { Color.FromArgb(26, 95, 160), Color.FromArgb(52, 152, 219), Color.FromArgb(64, 169, 228) };
                blend.Positions = new float[] { 0.0f, 0.7f, 1.0f };
                brush.InterpolationColors = blend;
                g.FillRectangle(brush, rect);
            }
            
            // Тонкая линия внизу
            using (SolidBrush lineBrush = new SolidBrush(Color.FromArgb(80, 255, 255, 255)))
            {
                g.FillRectangle(lineBrush, 0, _headerPanel.Height - 2, _headerPanel.Width, 2);
            }
        }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _item.Brand = _txtBrand.Text;
                _item.Model = _txtModel.Text;
                _item.Color = _txtColor.Text;
                _item.Price = decimal.Parse(_txtPrice.Text);
                _item.Quantity = int.Parse(_txtQuantity.Text);
                _item.IMEI = _txtIMEI.Text;

                if (_item.Id == 0)
                    _db.AddPhone(_item);
                else
                    _db.UpdatePhone(_item);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}