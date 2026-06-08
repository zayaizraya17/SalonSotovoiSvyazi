using System;

namespace MobileStoreApp.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Count { get; set; }
        public decimal TotalCost { get; set; }
        
        /// <summary>
        /// Стоимость одной единицы товара
        /// </summary>
        public decimal UnitCost => Count > 0 ? TotalCost / Count : 0;
        
        /// <summary>
        /// Полное наименование товара
        /// </summary>
        public string ItemName => $"{Brand} {Model}";
    }
}