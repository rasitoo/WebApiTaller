using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebApiTaller.Models;

public class Vehicle
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string License { get; set; }
    public string? Vin { get; set; }
    public string UserId { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public List<string>? ComponentIds { get; set; } 
}
