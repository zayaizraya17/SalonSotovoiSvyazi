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
        private readonly Color _primaryColor = Color.FromArgb(108, 92, 231); // Фиолетовый
        private readonly Color _secondaryColor = Color.FromArgb(59, 130, 246); // Голубой
        private readonly Color _accentPink = Color.FromArgb(236, 72, 153); // Розовый
        private readonly Color _accentPurple = Color.FromArgb(139, 92, 246); // Светло-фиолетовый
        private readonly Color _accentCyan = Color.FromArgb(34, 211, 238); // Голубой циан
        private readonly Color _bgColor = Color.FromArgb(250, 245, 255); // Светлый фон с розовым оттенком
        private readonly Color _textDark = Color.FromArgb(76, 29, 149); // Тёмно-фиолетовый текст

        public SalesForm(DatabaseHelper db)
        {
            _db = db;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "💰 Продажи";
            this.Size = new System.Drawing.Size(1100, 600);
            this.BackColor = _bgColor;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Верхняя панель с выбором телефона
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 160,
                BackColor = Color.FromArgb(253, 240, 250),
                Padding = new Padding(20, 15, 20, 15),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            var lblPhone = new Label
            {
                Text = "Телефон:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = _textDark,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            _cmbPhones = new ComboBox
            {
                Width = 500,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                Location = new Point(120, 15)
            };
            _cmbPhones.DisplayMember = "FullName";
            _cmbPhones.ValueMember = "Id";
            _cmbPhones.DataSource = _db.GetPhones();

            var lblQuantity = new Label
            {
                Text = "Кол-во:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = _textDark,
                AutoSize = true,
                Location = new Point(20, 60)
            };

            var txtQuantity = new TextBox
            {
                Width = 80,
                Text = "1",
                Font = new Font("Segoe UI", 10),
                Location = new Point(120, 55)
            };

            var lblCustomer = new Label
            {
                Text = "Покупатель:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = _textDark,
                AutoSize = true,
                Location = new Point(230, 60)
            };

            var txtCustomerName = new TextBox
            {
                Width = 200,
                Font = new Font("Segoe UI", 10),
                Location = new Point(330, 55)
            };

            var lblCustomerPhone = new Label
            {
                Text = "Телефон:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = _textDark,
                AutoSize = true,
                Location = new Point(560, 60)
            };

            var txtCustomerPhoneNum = new TextBox
            {
                Width = 150,
                Font = new Font("Segoe UI", 10),
                Location = new Point(640, 55)
            };

            topPanel.Controls.Add(lblPhone);
            topPanel.Controls.Add(_cmbPhones);
            topPanel.Controls.Add(lblQuantity);
            topPanel.Controls.Add(txtQuantity);
            topPanel.Controls.Add(lblCustomer);
            topPanel.Controls.Add(txtCustomerName);
            topPanel.Controls.Add(lblCustomerPhone);
            topPanel.Controls.Add(txtCustomerPhoneNum);

            // Нижняя панель с кнопками
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(253, 240, 250)
            };

            var btnSell = CreateStyledButton("💰 Продать", _accentCyan, (s, e) =>
            {
                if (_cmbPhones.SelectedItem != null)
                {
                    try
                    {
                        var phone = (Phone)_cmbPhones.SelectedItem;
                        
                        if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
                        {
                            MessageBox.Show("❌ Введите корректное количество!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (quantity > phone.Quantity)
                        {
                            MessageBox.Show("❌ Недостаточно на складе!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                        {
                            MessageBox.Show("❌ Введите имя покупателя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var sale = new Sale
                        {
                            PhoneId = phone.Id,
                            CustomerName = txtCustomerName.Text.Trim(),
                            CustomerPhone = txtCustomerPhoneNum.Text.Trim(),
                            Quantity = quantity,
                            TotalPrice = phone.Price * quantity,
                            SaleDate = DateTime.Now
                        };

                        _db.AddSale(sale);

                        phone.Quantity -= quantity;
                        _db.UpdatePhone(phone);

                        MessageBox.Show($"✅ Продажа оформлена!\nСумма: {sale.TotalPrice:C2}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        txtQuantity.Text = "1";
                        txtCustomerName.Text = "";
                        txtCustomerPhoneNum.Text = "";
                        
                        LoadData();
                        _cmbPhones.DataSource = null;
                        _cmbPhones.DataSource = _db.GetPhones();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            });

            var btnClose = CreateStyledButton("❌ Закрыть", _accentPurple, (s, e) => this.Close());
            
            btnPanel.Controls.Add(btnClose);
            btnPanel.Controls.Add(btnSell);

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
            _grid.ColumnHeadersDefaultCellStyle.BackColor = _accentCyan;
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _grid.ColumnHeadersHeight = 40;
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(153, 229, 236);
            _grid.DefaultCellStyle.SelectionForeColor = _textDark;
            _grid.RowTemplate.Height = 35;

            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 60 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Телефон", Width = 250 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CustomerName", HeaderText = "Покупатель", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CustomerPhone", HeaderText = "Телефон покупателя", Width = 130 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SaleDate", HeaderText = "Дата", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Количество", Width = 100 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalPrice", HeaderText = "Сумма", Width = 150 });

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
            var phones = _db.GetPhones().ToDictionary(p => p.Id, p => p);

            var salesWithNames = sales.Select(s => new
            {
                s.Id,
                PhoneName = phones.ContainsKey(s.PhoneId) ? $"{phones[s.PhoneId].Brand} {phones[s.PhoneId].Model}" : "Неизвестно",
                Brand = phones.ContainsKey(s.PhoneId) ? phones[s.PhoneId].Brand : "",
                s.CustomerName,
                s.CustomerPhone,
                SaleDate = s.SaleDate.ToString("dd.MM.yyyy HH:mm"),
                s.Quantity,
                TotalPrice = s.TotalPrice.ToString("C2")
            }).ToList();

            _grid.DataSource = salesWithNames;
        }
    }
}