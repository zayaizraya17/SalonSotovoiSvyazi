using System;

namespace MusicStoreApp.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string ItemName { get; set; }
        public int Count { get; set; }
    }
}