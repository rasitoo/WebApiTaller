using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using WebApiTaller.Models;

namespace WebApiTaller.Controllers;

[ApiController]
[Route("api/workshops")]
public class WorkshopController : ControllerBase
{
    private readonly IMongoCollection<Workshop> _workshops;

    public WorkshopController(IMongoDatabase db)
    {
        _workshops = db.GetCollection<Workshop>("workshops");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userType = User.FindFirst("UserType")?.Value;

        if (userType != "2")
            return Unauthorized(new { message = "Only workshops are allowed." }); 

        return Ok(await _workshops.Find(_ => true).ToListAsync());
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Workshop workshop)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userType = User.FindFirst("UserType")?.Value;

        if (userType != "2")
            return Unauthorized(new { message = "Only workshops are allowed." });

        await _workshops.InsertOneAsync(workshop);
        return CreatedAtAction(nameof(GetAll), null, workshop);
    }
}

