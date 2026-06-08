using System;

namespace MusicStoreApp.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public int InstrumentId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}