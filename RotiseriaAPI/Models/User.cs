namespace RotiseriaAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; // Nunca guardamos la clave real
    public string Role { get; set; } = "Cajero"; // "Admin" o "Cajero"
}