using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using WebApiTaller.Models;
using WebApiTaller.Models.DTO.DTOMaintenanceOrder;

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
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var workshopId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";

        var filter = Builders<MaintenanceOrder>.Filter.Eq(o => o.WorkshopId, workshopId);

        if (vehicle != null)
            filter &= Builders<MaintenanceOrder>.Filter.Eq(o => o.VehicleId, vehicle);

        if (component != null)
            filter &= Builders<MaintenanceOrder>.Filter.AnyEq(o => o.ComponentId, component);

        var results = await _orders.Find(filter).ToListAsync();
        var dtoResults = results.Select(o => new DTOMaintenanceOrderRead
        {
            Id = o.Id,
            WorkshopId = o.WorkshopId,
            VehicleId = o.VehicleId,
            ComponentIds = o.ComponentId,
            Date = o.Date,
            Description = o.Description
        });

        return Ok(dtoResults);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(DTOMaintenanceOrderPost dtoOrder)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var workshopId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";

        var order = new MaintenanceOrder
        {
            WorkshopId = workshopId,
            VehicleId = dtoOrder.VehicleId,
            ComponentId = dtoOrder.ComponentIds,
            Date = dtoOrder.Date,
            Description = dtoOrder.Description
        };

        await _orders.InsertOneAsync(order);
        return CreatedAtAction(nameof(GetFiltered), null, order);
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
