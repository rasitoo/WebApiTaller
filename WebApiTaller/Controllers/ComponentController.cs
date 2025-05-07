using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApiTaller.Models;
using WebApiTaller.Models.DTO.DTOComponent;

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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? Parent)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var filter = Builders<Component>.Filter.Empty;

        if (!string.IsNullOrEmpty(name))
            filter &= Builders<Component>.Filter.Eq(o => o.Name, name);

        if (!string.IsNullOrEmpty(Parent))
            filter &= Builders<Component>.Filter.Eq(o => o.ParentAssemblyId, Parent);

        var components = await _components.Find(filter).ToListAsync();

        var dtoComponents = components.Select(c => new DTOComponentRead
        {
            Id = c.Id ?? string.Empty,
            Name = c.Name,
            Description = c.Description,
            ParentAssemblyId = c.ParentAssemblyId
        });

        return Ok(dtoComponents);
    }
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var component = await _components.Find(c => c.Id == id).FirstOrDefaultAsync();

        if (component == null)
            return NotFound(new { message = "Component not found." });

        var dtoComponent = new DTOComponentRead
        {
            Id = component.Id ?? string.Empty,
            Name = component.Name,
            Description = component.Description,
            ParentAssemblyId = component.ParentAssemblyId
        };

        return Ok(dtoComponent);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(DTOComponentPost dtoComponent)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var component = new Component
        {
            Name = dtoComponent.Name,
            Description = dtoComponent.Description,
            ParentAssemblyId = dtoComponent.ParentAssemblyId
        };

        await _components.InsertOneAsync(component);
        return CreatedAtAction(nameof(GetAll), null, component);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, DTOComponentPut dtoComponent)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var component = new Component
        {
            Id = id,
            Name = dtoComponent.Name,
            Description = dtoComponent.Description,
            ParentAssemblyId = dtoComponent.ParentAssemblyId
        };

        var result = await _components.ReplaceOneAsync(u => u.Id == id, component);
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
