using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _workshops.Find(_ => true).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Workshop workshop)
    {
        await _workshops.InsertOneAsync(workshop);
        return CreatedAtAction(nameof(GetAll), null, workshop);
    }
}

