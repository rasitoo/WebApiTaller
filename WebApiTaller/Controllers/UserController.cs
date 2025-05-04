using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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
    public async Task<IActionResult> GetAll()
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var users = await _users.Find(_ => true).ToListAsync();
        var dtoUsers = users.Select(u => new DTOUserReadAll
        {
            Id = u.Id,
            Name = u.Name,
            Surname = u.Surname
        });

        return Ok(dtoUsers);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        if (user == null)
            return NotFound();

        var dtoUser = new DTOUserReadAll
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname
        };

        return Ok(dtoUser);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(DTOUserPost dtoUser)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var user = new User
        {
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
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var user = new User
        {
            Id = id,
            Name = dtoUser.Name,
            Surname = dtoUser.Surname
        };

        var result = await _users.ReplaceOneAsync(u => u.Id == id, user);
        return result.ModifiedCount > 0 ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!IsAuthorized(out var unauthorizedResult))
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
