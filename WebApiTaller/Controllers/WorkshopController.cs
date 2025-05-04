using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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
    public async Task<IActionResult> GetAll()
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var workshops = await _workshops.Find(_ => true).ToListAsync();
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
