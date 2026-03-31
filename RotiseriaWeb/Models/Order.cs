namespace RotiseriaWeb.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int? CustomerId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public string OrderType { get; set; } = "Delivery";
        public string PaymentMethod { get; set; } = "Efectivo";
        public string Status { get; set; } = "Recibido";
        public string? Comments { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal Total { get; set; }
        public bool IsPaid { get; set; } = false;
        public List<OrderItem> Items { get; set; } = new();
    }
}