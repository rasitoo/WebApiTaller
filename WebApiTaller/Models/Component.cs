using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebApiTaller.Models;

public class Component
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] 
    public string? Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ParentAssemblyId { get; set; }
}
