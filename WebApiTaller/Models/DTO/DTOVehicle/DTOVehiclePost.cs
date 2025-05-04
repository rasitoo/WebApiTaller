namespace WebApiTaller.Models.DTO.DTOVehicle;

public class DTOVehiclePost
{
    public string? License { get; set; } 
    public string? Vin { get; set; } 
    public string? Brand { get; set; } 
    public string? Model { get; set; }
    public int? Year { get; set; } 
    public List<string>? ComponentIds { get; set; } 
}
