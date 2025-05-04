namespace WebApiTaller.Models.DTO.DTOVehicle;

public class DTOVehicleUpdate
{
    public string Id { get; set; }
    public string License { get; set; }
    public string Vin { get; set; }
    public string UserId { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public List<string> ComponentIds { get; set; }
}
