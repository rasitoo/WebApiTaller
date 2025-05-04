using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebApiTaller.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; }
    public string? Surname { get; set; }
    public string? dni { get; set; }
}
