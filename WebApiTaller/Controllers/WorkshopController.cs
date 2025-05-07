using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using WebApiTaller.Models;
using WebApiTaller.Models.DTO.DTOWorkshop;

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
    public async Task<IActionResult> GetAll([FromQuery] string? nif, [FromQuery] string? location, [FromQuery] string? speciality)
    {
        var filterBuilder = Builders<Workshop>.Filter;
        var filter = filterBuilder.Empty;

        if (!string.IsNullOrEmpty(nif))
            filter &= filterBuilder.Eq(w => w.Nif, nif);

        if (!string.IsNullOrEmpty(location))
            filter &= filterBuilder.Eq(w => w.Location, location);

        if (!string.IsNullOrEmpty(speciality))
            filter &= filterBuilder.Eq(w => w.Speciality, speciality);

        var workshops = await _workshops.Find(filter).ToListAsync();
        var dtoWorkshops = workshops.Select(w => new DTOWorkshopRead
        {
            Id = w.Id,
            Nif = w.Nif,
            Location = w.Location,
            Speciality = w.Speciality
        });

        return Ok(dtoWorkshops);
    }
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var workshop = await _workshops.Find(w => w.Id == id).FirstOrDefaultAsync();
        if (workshop == null)
            return NotFound(new { message = "Workshop not found." });

        var dtoWorkshop = new DTOWorkshopRead
        {
            Id = workshop.Id,
            Nif = workshop.Nif,
            Location = workshop.Location,
            Speciality = workshop.Speciality,
            Name = workshop.Name
        };

        return Ok(dtoWorkshop);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(DTOWorkshopPost dtoWorkshop)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var workshop = new Workshop
        {
            Nif = dtoWorkshop.Nif,
            Location = dtoWorkshop.Location,
            Speciality = dtoWorkshop.Speciality
        };

        await _workshops.InsertOneAsync(workshop);
        return CreatedAtAction(nameof(GetAll), null, workshop);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != id)
            return Forbid("You can only delete your own workshop");

        var result = await _workshops.DeleteOneAsync(w => w.Id == id);

        if (result.DeletedCount == 0)
            return NotFound(new { message = "Workshop not found." });

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
