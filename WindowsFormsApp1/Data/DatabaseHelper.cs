using System;
using System.Windows.Forms;
using System.IO;
using LiteDB;
using MusicStoreApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace MusicStoreApp.Data
{
    public class DatabaseHelper
    {
        private string _connectionString;

        public void Initialize()
        {
            string dbPath = Path.Combine(Application.StartupPath, "musicstore.db");
            _connectionString = dbPath;
        }

        public List<Instrument> GetInstruments()
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var col = db.GetCollection<Instrument>("Instruments");
                return col.FindAll().ToList();
            }
        }

        public void AddInstrument(Instrument item)
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var col = db.GetCollection<Instrument>("Instruments");
                col.Insert(item);
            }
        }

        public void UpdateInstrument(Instrument item)
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var col = db.GetCollection<Instrument>("Instruments");
                col.Update(item);
            }
        }

        public void DeleteInstrument(int id)
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var col = db.GetCollection<Instrument>("Instruments");
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
            var instruments = GetInstruments();

            return sales.GroupBy(s => s.InstrumentId)
                .Select(g => new
                {
                    Instrument = instruments.FirstOrDefault(i => i.Id == g.Key)?.Name,
                    Count = g.Count(),
                    Total = g.Sum(x => x.TotalPrice)
                }).ToList();
        }
    }
}