using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using WebApiTaller.Models;
using WebApiTaller.Models.DTO.DTOVehicle;

namespace WebApiTaller.Controllers;

[ApiController]
[Route("api/vehicles")]
public class VehicleController : ControllerBase
{
    private readonly IMongoCollection<Vehicle> _vehicles;

    public VehicleController(IMongoDatabase db)
    {
        _vehicles = db.GetCollection<Vehicle>("vehicles");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? license, [FromQuery] string? userId)
    {
        var filterBuilder = Builders<Vehicle>.Filter;
        var filter = filterBuilder.Empty;

        if (!string.IsNullOrEmpty(license))
            filter &= filterBuilder.Eq(v => v.License, license);

        if (!string.IsNullOrEmpty(userId))
            filter &= filterBuilder.Eq(v => v.UserId, userId);

        var vehicles = await _vehicles.Find(filter).ToListAsync();
        var dtoVehicles = vehicles.Select(v => new DTOVehicleReadAll
        {
            Id = v.Id,
            License = v.License,
            UserId = v.UserId
        });

        return Ok(dtoVehicles);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {

        var vehicle = await _vehicles.Find(v => v.Id == id).FirstOrDefaultAsync();
        if (vehicle == null)
            return NotFound();

        var dtoVehicle = new DTOVehicleRead
        {
            Id = vehicle.Id,
            License = vehicle.License,
            Vin = vehicle.Vin,
            UserId = vehicle.UserId,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            ComponentIds = vehicle.ComponentIds
        };

        return Ok(dtoVehicle);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(DTOVehiclePost dtoVehicle)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";

        var vehicle = new Vehicle
        {
            License = dtoVehicle.License,
            Vin = dtoVehicle.Vin,
            UserId = userId,
            Brand = dtoVehicle.Brand,
            Model = dtoVehicle.Model,
            Year = dtoVehicle.Year,
            ComponentIds = dtoVehicle.ComponentIds
        };

        await _vehicles.InsertOneAsync(vehicle);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, DTOVehicleUpdate dtoVehicle)
    {
        if (!IsAuthorized(out var unauthorizedResult) && User.FindFirst(ClaimTypes.NameIdentifier)?.Value == dtoVehicle.UserId)
            return unauthorizedResult;

        var vehicle = new Vehicle
        {
            Id = id,
            License = dtoVehicle.License,
            Vin = dtoVehicle.Vin,
            UserId = dtoVehicle.UserId,
            Brand = dtoVehicle.Brand,
            Model = dtoVehicle.Model,
            Year = dtoVehicle.Year,
            ComponentIds = dtoVehicle.ComponentIds
        };

        var result = await _vehicles.ReplaceOneAsync(v => v.Id == id, vehicle);
        return result.ModifiedCount > 0 ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var vehicle = await _vehicles.Find(v => v.Id == id).FirstOrDefaultAsync();

        if (vehicle == null)
            return NotFound();

        if (vehicle.UserId != userId)
            return Unauthorized(new { message = "You are not authorized to delete this vehicle." });

        var result = await _vehicles.DeleteOneAsync(v => v.Id == id);
        return result.DeletedCount > 0 ? NoContent() : NotFound();
    }

    private bool IsAuthorized(out IActionResult unauthorizedResult)
    {
        var userType = User.FindFirst("UserType")?.Value;

        if (userType != "1")
        {
            unauthorizedResult = Unauthorized(new { message = "Only users are allowed." });
            return false;
        }

        unauthorizedResult = null!;
        return true;
    }
}
