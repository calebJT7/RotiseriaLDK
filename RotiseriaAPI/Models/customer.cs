namespace RotiseriaAPI.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; } // Opcional
    public string? Address { get; set; } // Opcional (para el delivery)
    public string? Notes { get; set; } // Ejemplo: "Tocar timbre fuerte"
}