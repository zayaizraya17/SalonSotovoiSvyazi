using System;
using System.Windows.Forms;
using System.IO;
using LiteDB;
using MobileStoreApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace MobileStoreApp.Data
{
    public class DatabaseHelper
    {
        private string _connectionString;

        public void Initialize()
        {
            string dbPath = Path.Combine(Application.StartupPath, "mobilestore.db");
            _connectionString = dbPath;
            
            // Создаем индексы для улучшения производительности
            using (var db = new LiteDatabase(_connectionString))
            {
                var phones = db.GetCollection<Phone>("Phones");
                phones.EnsureIndex(p => p.Brand);
                phones.EnsureIndex(p => p.Model);
                
                var sales = db.GetCollection<Sale>("Sales");
                sales.EnsureIndex(s => s.SaleDate);
                sales.EnsureIndex(s => s.PhoneId);
                
                var deliveries = db.GetCollection<Delivery>("Deliveries");
                deliveries.EnsureIndex(d => d.DeliveryDate);
                deliveries.EnsureIndex(d => d.SupplierName);
            }
        }

        public List<Phone> GetPhones()
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var col = db.GetCollection<Phone>("Phones");
                return col.FindAll().ToList();
            }
        }

        public void AddPhone(Phone item)
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var col = db.GetCollection<Phone>("Phones");
                col.Insert(item);
            }
        }

        public void UpdatePhone(Phone item)
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var col = db.GetCollection<Phone>("Phones");
                col.Update(item);
            }
        }

        public void DeletePhone(int id)
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var col = db.GetCollection<Phone>("Phones");
                col.Delete(id);
            }
        }

        public List<Delivery> GetDeliveries()
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var col = db.GetCollection<Delivery>("Deliveries");
                return col.FindAll().ToList();
            }
        }

        public void AddDelivery(Delivery item)
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var col = db.GetCollection<Delivery>("Deliveries");
                col.Insert(item);
            }
        }

        public List<Sale> GetSales()
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var col = db.GetCollection<Sale>("Sales");
                return col.FindAll().ToList();
            }
        }

        public void AddSale(Sale item)
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var col = db.GetCollection<Sale>("Sales");
                col.Insert(item);
            }
        }

        public object GetSalesReport()
        {
            // Простой отчет
            var sales = GetSales();
            var phones = GetPhones();

            return sales.GroupBy(s => s.PhoneId)
                .Select(g => new
                {
                    Phone = phones.FirstOrDefault(i => i.Id == g.Key).Model ?? "Неизвестно",
                    Brand = phones.FirstOrDefault(i => i.Id == g.Key).Brand ?? "",
                    Count = g.Count(),
                    Total = g.Sum(x => x.TotalPrice)
                }).ToList();
        }

        /// <summary>
        /// Получает расширенный отчет по продажам с группировкой по периодам
        /// </summary>
        public object GetExtendedSalesReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            var sales = GetSales();
            var phones = GetPhones();

            var query = sales.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value);

            var report = query.GroupBy(s => s.PhoneId)
                .Select(g => new
                {
                    PhoneId = g.Key,
                    Brand = phones.FirstOrDefault(i => i.Id == g.Key).Brand ?? "",
                    Model = phones.FirstOrDefault(i => i.Id == g.Key).Model ?? "Неизвестно",
                    QuantitySold = g.Sum(x => x.Quantity),
                    SalesCount = g.Count(),
                    TotalRevenue = g.Sum(x => x.TotalPrice),
                    AvgPrice = g.Average(x => x.TotalPrice)
                }).OrderByDescending(x => x.TotalRevenue).ToList();

            return report;
        }

        /// <summary>
        /// Получает статистику по остаткам товаров
        /// </summary>
        public object GetInventoryStatus()
        {
            var phones = GetPhones();

            return phones.Select(p => new
            {
                p.Id,
                p.Brand,
                p.Model,
                p.Color,
                p.Price,
                p.Quantity,
                TotalValue = p.Price * p.Quantity,
                Status = p.Quantity == 0 ? "Нет в наличии" : 
                         p.Quantity < 3 ? "Мало" : 
                         p.Quantity < 10 ? "Средне" : "В наличии"
            }).ToList();
        }

        /// <summary>
        /// Поиск телефонов по названию или бренду
        /// </summary>
        public List<Phone> SearchPhones(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetPhones();

            using (var db = new LiteDatabase(_connectionString))
            {
                var col = db.GetCollection<Phone>("Phones");
                var term = searchTerm.ToLower();
                return col.Find(p => 
                    p.Brand.ToLower().Contains(term) || 
                    p.Model.ToLower().Contains(term) ||
                    p.Color.ToLower().Contains(term)).ToList();
            }
        }

        /// <summary>
        /// Обновляет количество товара после продажи
        /// </summary>
        public bool UpdatePhoneQuantity(int phoneId, int quantityChange)
        {
            try
            {
                using (var db = new LiteDatabase(_connectionString))
                {
                    var col = db.GetCollection<Phone>("Phones");
                    var phone = col.FindById(phoneId);
                    if (phone != null)
                    {
                        phone.Quantity += quantityChange;
                        if (phone.Quantity < 0)
                            return false;
                        col.Update(phone);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка обновления количества: {ex.Message}");
                return false;
            }
        }
    }
}