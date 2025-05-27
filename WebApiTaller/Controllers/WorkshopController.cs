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
    public async Task<IActionResult> GetAll([FromQuery] string? userId, [FromQuery] string? location, [FromQuery] string? speciality, [FromQuery] string? name)
    {
        var filterBuilder = Builders<Workshop>.Filter;
        var filter = filterBuilder.Empty;

        if (!string.IsNullOrEmpty(userId))
            filter &= filterBuilder.Eq(w => w.UserId, userId);

        if (!string.IsNullOrEmpty(location))
            filter &= filterBuilder.Eq(w => w.Location, location);

        if (!string.IsNullOrEmpty(speciality))
            filter &= filterBuilder.Eq(w => w.Speciality, speciality);

        if (!string.IsNullOrEmpty(name))
            filter &= filterBuilder.Eq(w => w.Name, name);

        var workshops = await _workshops.Find(filter).ToListAsync();
        var dtoWorkshops = workshops.Select(w => new DTOWorkshopReadAll
        {
            Id = w.Id,
            UserId = w.UserId,
            Location = w.Location,
            Speciality = w.Speciality,
            Name = w.Name
        });

        return Ok(dtoWorkshops);
    }

    [Authorize]
    [HttpGet("{JWTid}")]
    public async Task<IActionResult> GetById(string JWTid)
    {
        var workshop = await _workshops.Find(w => w.UserId == JWTid).FirstOrDefaultAsync();
        if (workshop == null)
            return NotFound(new { message = "Workshop not found." });

        var jwtUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (jwtUserId != workshop.UserId)
            return Forbid();

        var dtoWorkshop = new DTOWorkshopRead
        {
            Id = workshop.Id,
            UserId = workshop.UserId,
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

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return BadRequest(new { message = "couldnt find jwtuserid." });

        var exists = await _workshops.Find(w => w.UserId == userId).AnyAsync();
        if (exists)
            return Conflict(new { message = "already exists a workshop with that jwtuserid" });

        var workshop = new Workshop
        {
            UserId = userId,
            Nif = dtoWorkshop.Nif,
            Location = dtoWorkshop.Location,
            Speciality = dtoWorkshop.Speciality,
            Name = dtoWorkshop.Name
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
