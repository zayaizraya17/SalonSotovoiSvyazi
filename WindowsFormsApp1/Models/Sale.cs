using System;

namespace MobileStoreApp.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public int PhoneId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}