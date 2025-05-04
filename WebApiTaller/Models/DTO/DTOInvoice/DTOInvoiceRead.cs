namespace WebApiTaller.Models.DTO.DTOInvoice;

public class DTOInvoiceRead
{
    public string Id { get; set; }
    public string ClientId { get; set; }
    public string WorkshopId { get; set; }
    public string MaintenanceId { get; set; }
    public decimal Total { get; set; }
    public DateTime Date { get; set; }
}
