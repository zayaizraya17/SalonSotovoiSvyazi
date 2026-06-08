namespace MobileStoreApp.Models
{
    public class Phone
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string IMEI { get; set; }
        
        /// <summary>
        /// Общая стоимость всех единиц товара на складе
        /// </summary>
        public decimal TotalValue => Price * Quantity;
        
        /// <summary>
        /// Статус доступности товара
        /// </summary>
        public string AvailabilityStatus
        {
            get
            {
                if (Quantity == 0) return "Нет в наличии";
                if (Quantity < 3) return "Мало";
                if (Quantity < 10) return "Средне";
                return "В наличии";
            }
        }
        
        /// <summary>
        /// Полное наименование телефона (бренд + модель + цвет)
        /// </summary>
        public string FullName => $"{Brand} {Model} ({Color})";
    }
}