using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApiTaller.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApiTaller.Controllers;

[ApiController]
[Route("api/orders")]
public class MaintenanceOrderController : ControllerBase
{
    private readonly IMongoCollection<MaintenanceOrder> _orders;

    public MaintenanceOrderController(IMongoDatabase db)
    {
        _orders = db.GetCollection<MaintenanceOrder>("maintenance_orders");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetFiltered(
        [FromQuery] string? vehicle,
        [FromQuery] string? component)
    {
        if (!IsAuthorized(out var unauthorizedResult, out var userId))
            return unauthorizedResult;

        var filter = Builders<MaintenanceOrder>.Filter.Eq(o => o.WorkshopId, userId);

        if (!string.IsNullOrEmpty(vehicle))
            filter &= Builders<MaintenanceOrder>.Filter.Eq(o => o.VehicleId, vehicle);

        if (!string.IsNullOrEmpty(component))
            filter &= Builders<MaintenanceOrder>.Filter.Eq(o => o.ComponentId, component);

        var results = await _orders.Find(filter).ToListAsync();
        return Ok(results);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(MaintenanceOrder order)
    {
        if (!IsAuthorized(out var unauthorizedResult, out var userId))
            return unauthorizedResult;

        order.WorkshopId = userId;
        await _orders.InsertOneAsync(order);
        return CreatedAtAction(nameof(GetFiltered), null, order);
    }

    private bool IsAuthorized(out IActionResult unauthorizedResult, out string userId)
    {
        userId = User.FindFirst("NameIdentifier")?.Value;
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
