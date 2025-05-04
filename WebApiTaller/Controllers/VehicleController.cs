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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetAll() =>
        Ok(await _vehicles.Find(_ => true).ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Vehicle>> GetById(string id) =>
        Ok(await _vehicles.Find(v => v.Id == id).FirstOrDefaultAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Vehicle vehicle)
    {
        await _vehicles.InsertOneAsync(vehicle);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Vehicle updated)
    {
        var result = await _vehicles.ReplaceOneAsync(v => v.Id == id, updated);
        return result.ModifiedCount > 0 ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _vehicles.DeleteOneAsync(v => v.Id == id);
        return result.DeletedCount > 0 ? NoContent() : NotFound();
    }
}
