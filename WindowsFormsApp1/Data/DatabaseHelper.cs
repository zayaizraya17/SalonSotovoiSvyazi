using System;
using System.Windows.Forms;
using System.IO;
using LiteDB;
using MobileStoreApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace MobileStoreApp.Data
{
    public class DatabaseHelper
    {
        private string _connectionString;

        public void Initialize()
        {
            string dbPath = Path.Combine(Application.StartupPath, "mobilestore.db");
            _connectionString = dbPath;
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
                    Phone = phones.FirstOrDefault(i => i.Id == g.Key)?.Model ?? "Неизвестно",
                    Brand = phones.FirstOrDefault(i => i.Id == g.Key)?.Brand ?? "",
                    Count = g.Count(),
                    Total = g.Sum(x => x.TotalPrice)
                }).ToList();
        }
    }
}