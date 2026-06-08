using System;
using System.Drawing;
using System.Windows.Forms;
using MobileStoreApp.Data;
using MobileStoreApp.Models;

namespace MobileStoreApp.Forms
{
    public partial class PhonesForm : Form
    {
        private DatabaseHelper _db;
        private DataGridView _grid;

        public PhonesForm(DatabaseHelper db)
        {
            _db = db;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "📱 Телефоны";
            this.Size = new System.Drawing.Size(1000, 550);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.StartPosition = FormStartPosition.CenterScreen;

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false
            };

            // Красивые заголовки таблицы
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _grid.ColumnHeadersHeight = 40;
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(174, 214, 241);
            _grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            _grid.RowTemplate.Height = 35;

            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Brand", HeaderText = "Бренд", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Model", HeaderText = "Модель", Width = 200 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Color", HeaderText = "Цвет", Width = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Price", HeaderText = "Цена", Width = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Количество", Width = 100 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IMEI", HeaderText = "IMEI", Width = 180 });

            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(250, 250, 250)
            };

            var btnClose = CreateStyledButton("❌ Закрыть", Color.FromArgb(149, 165, 166), (s, e) => this.Close());
            var btnDelete = CreateStyledButton("🗑️ Удалить", Color.FromArgb(231, 76, 60), (s, e) => DeleteItem());
            var btnEdit = CreateStyledButton("✏️ Изменить", Color.FromArgb(230, 126, 34), (s, e) => EditItem());
            var btnAdd = CreateStyledButton("➕ Добавить", Color.FromArgb(46, 204, 113), (s, e) => AddItem());

            btnPanel.Controls.Add(btnClose);
            btnPanel.Controls.Add(btnDelete);
            btnPanel.Controls.Add(btnEdit);
            btnPanel.Controls.Add(btnAdd);

            this.Controls.Add(_grid);
            this.Controls.Add(btnPanel);

            LoadData();
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
                Size = new Size(150, 45),
                Margin = new Padding(10, 10, 10, 10),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.2f);
            btn.Click += clickHandler;
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

        public PhoneEditForm(DatabaseHelper db, Phone item)
        {
            _db = db;
            _item = item ?? new Phone();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = _item.Id == 0 ? "➕ Добавить телефон" : "✏️ Изменить телефон";
            this.Size = new System.Drawing.Size(450, 450);
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

            var lblBrand = new Label { Text = "Бренд:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtBrand = new TextBox { Text = _item.Brand, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblBrand, 0, 0);
            layout.Controls.Add(_txtBrand, 1, 0);

            var lblModel = new Label { Text = "Модель:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtModel = new TextBox { Text = _item.Model, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblModel, 0, 1);
            layout.Controls.Add(_txtModel, 1, 1);

            var lblColor = new Label { Text = "Цвет:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtColor = new TextBox { Text = _item.Color, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblColor, 0, 2);
            layout.Controls.Add(_txtColor, 1, 2);

            var lblPrice = new Label { Text = "Цена:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtPrice = new TextBox { Text = _item.Price.ToString(), Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblPrice, 0, 3);
            layout.Controls.Add(_txtPrice, 1, 3);

            var lblQuantity = new Label { Text = "Количество:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtQuantity = new TextBox { Text = _item.Quantity.ToString(), Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblQuantity, 0, 4);
            layout.Controls.Add(_txtQuantity, 1, 4);

            var lblIMEI = new Label { Text = "IMEI:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtIMEI = new TextBox { Text = _item.IMEI, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblIMEI, 0, 5);
            layout.Controls.Add(_txtIMEI, 1, 5);

            var btnPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill
            };

            var btnSave = new Button
            {
                Text = "💾 Сохранить",
                Width = 140,
                Height = 40,
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnSave.Click += BtnSave_Click;

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