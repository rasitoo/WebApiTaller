namespace WebApiTaller.Models;

public class Invoice
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string WorkshopId { get; set; }
    public string MaintenanceId { get; set; }
    public decimal Total { get; set; }
    public DateTime Date { get; set; }
}
