using System;
using System.Drawing;
using System.Windows.Forms;
using MusicStoreApp.Data;
using MusicStoreApp.Models;
using System.Linq;

namespace MusicStoreApp.Forms
{
    public partial class SalesForm : Form
    {
        private DatabaseHelper _db;
        private DataGridView _grid;
        private ComboBox _cmbInstruments;

        public SalesForm(DatabaseHelper db)
        {
            _db = db;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "💰 Продажи";
            this.Size = new System.Drawing.Size(1000, 600);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Верхняя панель с выбором инструмента
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(20, 15, 20, 15)
            };

            var lblInstrument = new Label
            {
                Text = "Инструмент:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(20, 25)
            };

            _cmbInstruments = new ComboBox
            {
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                Location = new Point(120, 20)
            };
            _cmbInstruments.DisplayMember = "Name";
            _cmbInstruments.ValueMember = "Id";
            _cmbInstruments.DataSource = _db.GetInstruments();

            var lblQuantity = new Label
            {
                Text = "Кол-во:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(440, 25)
            };

            var txtQuantity = new TextBox
            {
                Width = 80,
                Text = "1",
                Font = new Font("Segoe UI", 10),
                Location = new Point(510, 20)
            };

            var btnSell = new Button
            {
                Text = "💰 Продать",
                Width = 150,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Location = new Point(610, 18)
            };
            btnSell.FlatAppearance.BorderSize = 0;
            btnSell.FlatAppearance.MouseOverBackColor = Color.FromArgb(211, 84, 0);
            btnSell.Click += (s, e) =>
            {
                if (_cmbInstruments.SelectedItem != null)
                {
                    try
                    {
                        var instrument = (Instrument)_cmbInstruments.SelectedItem;
                        int quantity = int.Parse(txtQuantity.Text);

                        if (quantity > instrument.Quantity)
                        {
                            MessageBox.Show("❌ Недостаточно на складе!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var sale = new Sale
                        {
                            InstrumentId = instrument.Id,
                            Quantity = quantity,
                            TotalPrice = instrument.Price * quantity,
                            SaleDate = DateTime.Now
                        };

                        _db.AddSale(sale);

                        instrument.Quantity -= quantity;
                        _db.UpdateInstrument(instrument);

                        MessageBox.Show("✅ Продажа оформлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        _cmbInstruments.DataSource = _db.GetInstruments();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            topPanel.Controls.Add(lblInstrument);
            topPanel.Controls.Add(_cmbInstruments);
            topPanel.Controls.Add(lblQuantity);
            topPanel.Controls.Add(txtQuantity);
            topPanel.Controls.Add(btnSell);

            // Таблица
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
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 126, 34);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _grid.ColumnHeadersHeight = 40;
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 219, 179);
            _grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            _grid.RowTemplate.Height = 35;

            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 60 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Инструмент", Width = 250 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SaleDate", HeaderText = "Дата", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Количество", Width = 130 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalPrice", HeaderText = "Сумма", Width = 150 });

            // Нижняя панель
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(250, 250, 250)
            };

            var btnClose = CreateStyledButton("❌ Закрыть", Color.FromArgb(149, 165, 166), (s, e) => this.Close());
            btnPanel.Controls.Add(btnClose);

            this.Controls.Add(_grid);
            this.Controls.Add(btnPanel);
            this.Controls.Add(topPanel);

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
            var sales = _db.GetSales();
            var instruments = _db.GetInstruments();

            var salesWithNames = sales.Select(s => new
            {
                s.Id,
                InstrumentName = instruments.FirstOrDefault(i => i.Id == s.InstrumentId)?.Name ?? "Неизвестно",
                s.SaleDate,
                s.Quantity,
                TotalPrice = s.TotalPrice.ToString("C2")
            }).ToList();

            _grid.DataSource = salesWithNames;
        }
    }
}