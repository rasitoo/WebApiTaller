using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using WebApiTaller.Models;
using WebApiTaller.Models.DTO.DTOUser;

namespace WebApiTaller.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IMongoCollection<User> _users;

    public UserController(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("users");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? name, [FromQuery] string? surname, [FromQuery] string? userId)
    {
        var filterBuilder = Builders<User>.Filter;
        var filter = filterBuilder.Empty;

        if (!string.IsNullOrEmpty(name))
            filter &= filterBuilder.Eq(u => u.Name, name);

        if (!string.IsNullOrEmpty(surname))
            filter &= filterBuilder.Eq(u => u.Surname, surname);

        if (!string.IsNullOrEmpty(userId))
            filter &= filterBuilder.Eq(u => u.UserId, userId);

        var users = await _users.Find(filter).ToListAsync();
        var dtoUsers = users.Select(u => new DTOUserReadAll
        {
            Id = u.Id,
            UserId = u.UserId,
            Name = u.Name,
            Surname = u.Surname
        });

        return Ok(dtoUsers);
    }

    [Authorize]
    [HttpGet("{JWTid}")]
    public async Task<IActionResult> GetById(string JWTid)
    {
        var user = await _users.Find(u => u.UserId == JWTid).FirstOrDefaultAsync();
        if (user == null)
            return NotFound(new { message = "User not found." });

        var jwtUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (jwtUserId != user.UserId)
            return Forbid();

        var dtoUser = new DTOUserRead
        {
            Id = user.Id,
            UserId = user.UserId,
            Name = user.Name,
            Surname = user.Surname,
            dni = user.dni
        };

        return Ok(dtoUser);
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(DTOUserPost dtoUser)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userid))
            return BadRequest(new { message = "couldnt find jwtuserid." });

        var exists = await _users.Find(u => u.UserId == userid).AnyAsync();
        if (exists)
            return Conflict(new { message = "Already exists an user with that JWtuserId." });

        var user = new User
        {
            UserId = userid,
            Name = dtoUser.Name,
            Surname = dtoUser.Surname,
            dni = dtoUser.dni
        };

        await _users.InsertOneAsync(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }


    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, DTOUserUpdate dtoUser)
    {
        if (!IsAuthorized(out var unauthorizedResult) && User.FindFirst(ClaimTypes.NameIdentifier)?.Value == id)
            return unauthorizedResult;

        var user = new User
        {
            Id = id,
            Name = dtoUser.Name,
            Surname = dtoUser.Surname,
            dni = dtoUser.DNI
        };

        var result = await _users.ReplaceOneAsync(u => u.Id == id, user);
        return result.ModifiedCount > 0 ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!IsAuthorized(out var unauthorizedResult) && User.FindFirst(ClaimTypes.NameIdentifier)?.Value == id)
            return unauthorizedResult;

        var result = await _users.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0 ? NoContent() : NotFound();
    }

    private bool IsAuthorized(out IActionResult unauthorizedResult)
    {
        var userType = User.FindFirst("UserType")?.Value;

        if (userType != "1")
        {
            unauthorizedResult = Unauthorized(new { message = "Only users are allowed." });
            return false;
        }

        unauthorizedResult = null!;
        return true;
    }
}
