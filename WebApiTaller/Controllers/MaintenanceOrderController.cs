using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApiTaller.Models;

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

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _orders.Find(_ => true).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(MaintenanceOrder order)
    {
        await _orders.InsertOneAsync(order);
        return CreatedAtAction(nameof(GetAll), null, order);
    }
}

