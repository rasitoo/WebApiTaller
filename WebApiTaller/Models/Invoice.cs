using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebApiTaller.Models;

public class Invoice
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string ClientId { get; set; }
    public string WorkshopId { get; set; }
    public string MaintenanceId { get; set; }
    public decimal Total { get; set; }
    public DateTime Date { get; set; }
}
