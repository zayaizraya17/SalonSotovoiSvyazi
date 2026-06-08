using System;
using System.Drawing;
using System.Windows.Forms;
using MobileStoreApp.Data;
using MobileStoreApp.Models;
using System.Linq;

namespace MobileStoreApp.Forms
{
    public partial class SalesForm : Form
    {
        private DatabaseHelper _db;
        private DataGridView _grid;
        private ComboBox _cmbPhones;

        public SalesForm(DatabaseHelper db)
        {
            _db = db;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "💰 Продажи";
            this.Size = new System.Drawing.Size(1100, 600);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Верхняя панель с выбором телефона
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(20, 15, 20, 15)
            };

            var lblPhone = new Label
            {
                Text = "Телефон:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(20, 25)
            };

            _cmbPhones = new ComboBox
            {
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                Location = new Point(120, 20)
            };
            _cmbPhones.DisplayMember = "Model";
            _cmbPhones.ValueMember = "Id";
            _cmbPhones.DataSource = _db.GetPhones();

            var lblQuantity = new Label
            {
                Text = "Кол-во:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(490, 25)
            };

            var txtQuantity = new TextBox
            {
                Width = 80,
                Text = "1",
                Font = new Font("Segoe UI", 10),
                Location = new Point(560, 20)
            };

            var lblCustomer = new Label
            {
                Text = "Покупатель:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(20, 65)
            };

            var txtCustomerName = new TextBox
            {
                Width = 200,
                Font = new Font("Segoe UI", 10),
                Location = new Point(120, 60)
            };

            var lblCustomerPhone = new Label
            {
                Text = "Телефон:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(340, 65)
            };

            var txtCustomerPhoneNum = new TextBox
            {
                Width = 150,
                Font = new Font("Segoe UI", 10),
                Location = new Point(420, 60)
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
                Location = new Point(660, 55)
            };
            btnSell.FlatAppearance.BorderSize = 0;
            btnSell.FlatAppearance.MouseOverBackColor = Color.FromArgb(211, 84, 0);
            btnSell.Click += (s, e) =>
            {
                if (_cmbPhones.SelectedItem != null)
                {
                    try
                    {
                        var phone = (Phone)_cmbPhones.SelectedItem;
                        int quantity = int.Parse(txtQuantity.Text);

                        if (quantity > phone.Quantity)
                        {
                            MessageBox.Show("❌ Недостаточно на складе!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var sale = new Sale
                        {
                            PhoneId = phone.Id,
                            CustomerName = txtCustomerName.Text,
                            CustomerPhone = txtCustomerPhoneNum.Text,
                            Quantity = quantity,
                            TotalPrice = phone.Price * quantity,
                            SaleDate = DateTime.Now
                        };

                        _db.AddSale(sale);

                        phone.Quantity -= quantity;
                        _db.UpdatePhone(phone);

                        MessageBox.Show("✅ Продажа оформлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        _cmbPhones.DataSource = _db.GetPhones();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            topPanel.Controls.Add(lblPhone);
            topPanel.Controls.Add(_cmbPhones);
            topPanel.Controls.Add(lblQuantity);
            topPanel.Controls.Add(txtQuantity);
            topPanel.Controls.Add(lblCustomer);
            topPanel.Controls.Add(txtCustomerName);
            topPanel.Controls.Add(lblCustomerPhone);
            topPanel.Controls.Add(txtCustomerPhoneNum);
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
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Телефон", Width = 250 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CustomerName", HeaderText = "Покупатель", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CustomerPhone", HeaderText = "Телефон покупателя", Width = 130 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SaleDate", HeaderText = "Дата", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Количество", Width = 100 });
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
            var phones = _db.GetPhones();

            var salesWithNames = sales.Select(s => new
            {
                s.Id,
                PhoneName = phones.FirstOrDefault(i => i.Id == s.PhoneId)?.Model ?? "Неизвестно",
                Brand = phones.FirstOrDefault(i => i.Id == s.PhoneId)?.Brand ?? "",
                s.CustomerName,
                s.CustomerPhone,
                s.SaleDate,
                s.Quantity,
                TotalPrice = s.TotalPrice.ToString("C2")
            }).ToList();

            _grid.DataSource = salesWithNames;
        }
    }
}