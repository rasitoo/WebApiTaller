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
        [FromQuery] string? component,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var workshopId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";

        var filter = Builders<MaintenanceOrder>.Filter.Eq(o => o.WorkshopId, workshopId);

        if (vehicle != null)
            filter &= Builders<MaintenanceOrder>.Filter.Eq(o => o.VehicleId, vehicle);

        if (component != null)
            filter &= Builders<MaintenanceOrder>.Filter.AnyEq(o => o.ComponentId, component);

        if (startDate.HasValue)
            filter &= Builders<MaintenanceOrder>.Filter.Gte(o => o.Date, startDate.Value);

        if (endDate.HasValue)
            filter &= Builders<MaintenanceOrder>.Filter.Lte(o => o.Date, endDate.Value);

        var results = await _orders.Find(filter).ToListAsync();
        var dtoResults = results.Select(o => new DTOMaintenanceOrderRead
        {
            Id = o.Id,
            WorkshopId = o.WorkshopId,
            VehicleId = o.VehicleId,
            ComponentIds = o.ComponentId,
            Date = o.Date ?? DateTime.Now,
            Description = o.Description
        });

        return Ok(dtoResults);
    }
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var workshopId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";

        var order = await _orders.Find(o => o.Id == id && o.WorkshopId == workshopId).FirstOrDefaultAsync();

        if (order == null)
            return NotFound(new { message = "Maintenance order not found." });

        var dtoOrder = new DTOMaintenanceOrderRead
        {
            Id = order.Id,
            WorkshopId = order.WorkshopId,
            VehicleId = order.VehicleId,
            ComponentIds = order.ComponentId,
            Date = order.Date ?? DateTime.Now,
            Description = order.Description
        };

        return Ok(dtoOrder);
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

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var result = await _orders.DeleteOneAsync(o => o.Id == id);

        if (result.DeletedCount == 0)
            return NotFound(new { message = "Maintenance order not found." });

        return NoContent();
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
