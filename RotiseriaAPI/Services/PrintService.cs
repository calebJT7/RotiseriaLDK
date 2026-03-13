using ESC_POS_USB_NET.Printer;
using RotiseriaAPI.Models;

namespace RotiseriaAPI.Services;

public class PrintService
{
    public void PrintOrder(Order order)
    {
        // Acordate de verificar si el nombre en Windows es "POS-80"
        Printer printer = new Printer("POS-80");

        // --- ENCABEZADO ---
        printer.BoldMode("--- COMANDA ---");
        printer.Append($"Pedido #{order.Id} - {order.Date:HH:mm}");
        printer.Separator();

        // --- DATOS DEL CLIENTE ---
        printer.Append($"CLIENTE: {order.ClientName.ToUpper()}");
        if (order.OrderType == "Delivery")
        {
            printer.BoldMode($"DIR: {order.DeliveryAddress.ToUpper()}");
        }
        printer.Append($"TEL: {order.Phone}");
        printer.Separator();

        // --- EL PEDIDO (LO MÁS GRANDE) ---
        printer.BoldMode("DETALLE DE COCINA:");
        foreach (var item in order.Items)
        {
            // ExpandedMode(alto, ancho) -> true para ambos para que sea gigante
            printer.ExpandedMode("1");
            printer.Append($"{item.Quantity} x {item.ProductName}");

            // Para volver al tamaño normal usamos InitializePrint
            printer.InitializePrint();
        }
        printer.Separator();

        // --- NOTAS ---
        if (!string.IsNullOrEmpty(order.Comments))
        {
            printer.BoldMode("NOTAS:");
            printer.Append(order.Comments);
            printer.Separator();
        }

        // --- TOTAL Y PAGO ---
        printer.Append($"TIPO: {order.OrderType} | PAGO: {order.PaymentMethod}");
        printer.Append($"ENVIO: ${order.DeliveryCost}");
        printer.BoldMode($"TOTAL A COBRAR: ${order.Total}");

        printer.Append("\n¡Buen provecho!");
        printer.Append("--------------------------------\n");

        // Corte y ejecución
        printer.FullPaperCut();
        printer.PrintDocument();
    }
}