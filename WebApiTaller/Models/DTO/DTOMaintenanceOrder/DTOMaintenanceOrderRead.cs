namespace WebApiTaller.Models.DTO.DTOMaintenanceOrder;

public class DTOMaintenanceOrderRead
{
    public string Id { get; set; }
    public string WorkshopId { get; set; }
    public string VehicleId { get; set; }
    public List<string> ComponentIds { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
}
