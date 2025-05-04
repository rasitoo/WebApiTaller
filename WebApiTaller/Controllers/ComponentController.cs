using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var components = await _components.Find(_ => true).ToListAsync();
        return Ok(components);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Component component)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

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
