namespace WebApiTaller.Models.DTO.DTOMaintenanceOrder;

public class DTOMaintenanceOrderPost
{
    public string VehicleId { get; set; }
    public List<string> ComponentIds { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
}
