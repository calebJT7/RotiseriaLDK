namespace RotiseriaAPI.Models;

public class Debt
{
    public int Id { get; set; }
    public int CustomerId { get; set; } // Relación con la tabla de Clientes
    public Customer? Customer { get; set; } // Propiedad de navegación

    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public bool IsPaid { get; set; } = false;
}