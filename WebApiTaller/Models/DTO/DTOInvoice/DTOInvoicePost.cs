namespace WebApiTaller.Models.DTO.DTOInvoice;

public class DTOInvoicePost
{
    public string ClientId { get; set; }
    public string MaintenanceId { get; set; }
    public decimal Total { get; set; }
    public DateTime Date { get; set; }
}
