namespace RotiseriaAPI.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public int? CustomerId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;

    // OrderType ahora puede ser: "Delivery", "Local" o "Salon"
    public string OrderType { get; set; } = "Delivery";

    // --- NUEVO: Para saber qué mesa es ---
    public int? TableId { get; set; }

    public string PaymentMethod { get; set; } = "Efectivo";
    public string? Comments { get; set; }
    public decimal DeliveryCost { get; set; }
    public decimal Total { get; set; }
    public bool IsPaid { get; set; } = false;

    // Relación con los items del pedido
    public List<OrderItem> Items { get; set; } = new();

    // --- Propiedades de Cocina y Logística ---
    // Status ahora usará: "Abierto" (comiendo), "Pendiente" (en cocina), "Despachado", "Cancelado"
    public string Status { get; set; } = "Pendiente";
    public DateTime? DispatchedAt { get; set; }
    public bool Alert30Dismissed { get; set; } = false;
}