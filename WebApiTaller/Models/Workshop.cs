using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebApiTaller.Models;

public class Workshop
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string UserId { get; set; }
    public string? Nif { get; set; }
    public string? Location { get; set; }
    public string? Speciality { get; set; }
    public string? Name { get; set; }
}
