using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebApiTaller.Models;

public class MaintenanceOrder
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string WorkshopId { get; set; }
    public string VehicleId { get; set; }
    public List<string> ComponentId { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
}
