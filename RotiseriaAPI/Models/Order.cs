namespace RotiseriaAPI.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public int? CustomerId { get; set; } // Nullable for orders without customer
    public string ClientName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty; // Se guarda siempre
    public string OrderType { get; set; } = "Delivery"; // Delivery o Retira
    public string PaymentMethod { get; set; } = "Efectivo"; // Efectivo, Transferencia, Tarjeta
    public string Status { get; set; } = "Recibido"; // Recibido, Cocina, Listo, Entregado
    public string? Comments { get; set; } // "Sin cebolla", "Tocar timbre", etc.
    public decimal DeliveryCost { get; set; }
    public decimal Total { get; set; }
    public bool IsPaid { get; set; } = false;

    // Relación con los items del pedido
    public List<OrderItem> Items { get; set; } = new();
}