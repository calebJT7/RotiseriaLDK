namespace RotiseriaAPI.Models;

public class Table
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Status { get; set; } = "Libre"; // Puede ser: "Libre" u "Ocupada"
    public int? CurrentOrderId { get; set; } // Acá guardamos el ID de la cuenta que está comiendo ahora
}