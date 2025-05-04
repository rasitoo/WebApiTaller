namespace WebApiTaller.Models.DTO.DTOVehicle;

public class DTOVehicleRead : DTOVehicleReadAll
{
    public string? Vin { get; set; } 
    public string? Brand { get; set; } 
    public string? Model { get; set; } 
    public int? Year { get; set; } 
    public List<string>? ComponentIds { get; set; } 
}
