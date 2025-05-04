using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApiTaller.Models;

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
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetAll()
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var vehicles = await _vehicles.Find(_ => true).ToListAsync();
        return Ok(vehicles);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Vehicle>> GetById(string id)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var vehicle = await _vehicles.Find(v => v.Id == id).FirstOrDefaultAsync();
        return Ok(vehicle);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Vehicle vehicle)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        await _vehicles.InsertOneAsync(vehicle);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Vehicle updated)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var result = await _vehicles.ReplaceOneAsync(v => v.Id == id, updated);
        return result.ModifiedCount > 0 ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

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
