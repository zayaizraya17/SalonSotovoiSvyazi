using System;
using System.Drawing;
using System.Windows.Forms;
using MusicStoreApp.Data;
using MusicStoreApp.Models;

namespace MusicStoreApp.Forms
{
    public partial class InstrumentsForm : Form
    {
        private DatabaseHelper _db;
        private DataGridView _grid;

        public InstrumentsForm(DatabaseHelper db)
        {
            _db = db;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "🎹 Инструменты";
            this.Size = new System.Drawing.Size(900, 550);
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

            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 60 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Название", Width = 250 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Type", HeaderText = "Тип", Width = 180 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Price", HeaderText = "Цена", Width = 130 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Количество", Width = 130 });

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
            _grid.DataSource = _db.GetInstruments();
        }

        private void AddItem()
        {
            var form = new InstrumentEditForm(_db, null);
            if (form.ShowDialog() == DialogResult.OK)
                LoadData();
        }

        private void EditItem()
        {
            if (_grid.CurrentRow != null)
            {
                var item = (Instrument)_grid.CurrentRow.DataBoundItem;
                var form = new InstrumentEditForm(_db, item);
                if (form.ShowDialog() == DialogResult.OK)
                    LoadData();
            }
        }

        private void DeleteItem()
        {
            if (_grid.CurrentRow != null)
            {
                var item = (Instrument)_grid.CurrentRow.DataBoundItem;
                if (MessageBox.Show($"Удалить {item.Name}?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _db.DeleteInstrument(item.Id);
                    LoadData();
                }
            }
        }
    }

    public class InstrumentEditForm : Form
    {
        private DatabaseHelper _db;
        private Instrument _item;
        private TextBox _txtName;
        private TextBox _txtType;
        private TextBox _txtPrice;
        private TextBox _txtQuantity;

        public InstrumentEditForm(DatabaseHelper db, Instrument item)
        {
            _db = db;
            _item = item ?? new Instrument();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = _item.Id == 0 ? "➕ Добавить инструмент" : "✏️ Изменить инструмент";
            this.Size = new System.Drawing.Size(450, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(25)
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));

            var lblName = new Label { Text = "Название:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtName = new TextBox { Text = _item.Name, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblName, 0, 0);
            layout.Controls.Add(_txtName, 1, 0);

            var lblType = new Label { Text = "Тип:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtType = new TextBox { Text = _item.Type, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblType, 0, 1);
            layout.Controls.Add(_txtType, 1, 1);

            var lblPrice = new Label { Text = "Цена:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtPrice = new TextBox { Text = _item.Price.ToString(), Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblPrice, 0, 2);
            layout.Controls.Add(_txtPrice, 1, 2);

            var lblQuantity = new Label { Text = "Количество:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            _txtQuantity = new TextBox { Text = _item.Quantity.ToString(), Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(lblQuantity, 0, 3);
            layout.Controls.Add(_txtQuantity, 1, 3);

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

            layout.Controls.Add(btnPanel, 0, 5);
            layout.SetColumnSpan(btnPanel, 2);

            this.Controls.Add(layout);
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _item.Name = _txtName.Text;
                _item.Type = _txtType.Text;
                _item.Price = decimal.Parse(_txtPrice.Text);
                _item.Quantity = int.Parse(_txtQuantity.Text);

                if (_item.Id == 0)
                    _db.AddInstrument(_item);
                else
                    _db.UpdateInstrument(_item);

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