using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApiTaller.Models;

namespace WebApiTaller.Controllers;

[ApiController]
[Route("api/components")]
public class ComponentController : ControllerBase
{
    private readonly IMongoCollection<Component> _components;

    public ComponentController(IMongoDatabase db)
    {
        _components = db.GetCollection<Component>("components");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _components.Find(_ => true).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Component component)
    {
        await _components.InsertOneAsync(component);
        return CreatedAtAction(nameof(GetAll), null, component);
    }
}
