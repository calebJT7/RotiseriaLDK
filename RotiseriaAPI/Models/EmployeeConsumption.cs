namespace RotiseriaAPI.Models;

public class EmployeeConsumption
{
    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}