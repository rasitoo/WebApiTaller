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
    public async Task<IActionResult> GetAll()
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var components = await _components.Find(_ => true).ToListAsync();
        var dtoComponents = components.Select(c => new DTOComponentRead
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            ParentAssemblyId = c.ParentAssemblyId
        });

        return Ok(dtoComponents);
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
