using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApiTaller.Models;

namespace WebApiTaller.Controllers;

[ApiController]
[Route("api/vehicles/{vehicleId}/components")]
public class VehicleComponentController : ControllerBase
{
    private readonly IMongoCollection<Vehicle> _vehicles;

    public VehicleComponentController(IMongoDatabase db)
    {
        _vehicles = db.GetCollection<Vehicle>("vehicles");
    }

    [HttpPost]
    public async Task<IActionResult> AddComponent(string vehicleId, [FromBody] string componentId)
    {
        var update = Builders<Vehicle>.Update.AddToSet(v => v.Components, componentId);
        var result = await _vehicles.UpdateOneAsync(v => v.Id == vehicleId, update);
        return result.ModifiedCount > 0 ? NoContent() : NotFound();
    }

    [HttpDelete("{componentId}")]
    public async Task<IActionResult> RemoveComponent(string vehicleId, string componentId)
    {
        var update = Builders<Vehicle>.Update.Pull(v => v.Components, componentId);
        var result = await _vehicles.UpdateOneAsync(v => v.Id == vehicleId, update);
        return result.ModifiedCount > 0 ? NoContent() : NotFound();
    }
}
