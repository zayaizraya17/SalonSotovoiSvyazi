using System;
using System.Drawing;
using System.Windows.Forms;
using MobileStoreApp.Data;
using MobileStoreApp.Models;

namespace MobileStoreApp.Forms
{
    public partial class DeliveriesForm : Form
    {
        private DatabaseHelper _db;
        private DataGridView _grid;

        public DeliveriesForm(DatabaseHelper db)
        {
            _db = db;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "📦 Поставки";
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

            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(46, 204, 113);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _grid.ColumnHeadersHeight = 40;
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(174, 236, 205);
            _grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            _grid.RowTemplate.Height = 35;

            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 60 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SupplierName", HeaderText = "Поставщик", Width = 200 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DeliveryDate", HeaderText = "Дата", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Brand", HeaderText = "Бренд", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Model", HeaderText = "Модель", Width = 200 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Count", HeaderText = "Количество", Width = 100 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalCost", HeaderText = "Стоимость", Width = 120 });

            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(250, 250, 250)
            };

            var btnClose = CreateStyledButton("❌ Закрыть", Color.FromArgb(149, 165, 166), (s, e) => this.Close());
            var btnAdd = CreateStyledButton("➕ Добавить", Color.FromArgb(46, 204, 113), (s, e) => AddItem());

            btnPanel.Controls.Add(btnClose);
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