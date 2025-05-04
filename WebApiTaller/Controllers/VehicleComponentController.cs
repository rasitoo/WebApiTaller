using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApiTaller.Models;
using WebApiTaller.Models.DTO.DTOVehicleComponent;

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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddComponent(string vehicleId, DTOVehicleComponentAdd dto)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var update = Builders<Vehicle>.Update.AddToSet(v => v.ComponentIds, dto.ComponentId);
        var result = await _vehicles.UpdateOneAsync(v => v.Id == vehicleId, update);
        return result.ModifiedCount > 0 ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> RemoveComponent(string vehicleId, DTOVehicleComponentRemove dto)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var update = Builders<Vehicle>.Update.Pull(v => v.ComponentIds, dto.ComponentId);
        var result = await _vehicles.UpdateOneAsync(v => v.Id == vehicleId, update);
        return result.ModifiedCount > 0 ? NoContent() : NotFound();
    }

    private bool IsAuthorized(out IActionResult unauthorizedResult)
    {
        var userType = User.FindFirst("UserType")?.Value;

        if (userType != "2")
        {
            unauthorizedResult = Unauthorized(new { message = "Only workshops are allowed." });
            return false;
        }

        unauthorizedResult = null!;
        return true;
    }
}
