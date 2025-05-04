using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApiTaller.Models;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll() =>
        Ok(await _users.Find(_ => true).ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(string id) =>
        Ok(await _users.Find(u => u.Id == id).FirstOrDefaultAsync());

    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        await _users.InsertOneAsync(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, User updatedUser)
    {
        var result = await _users.ReplaceOneAsync(u => u.Id == id, updatedUser);
        return result.ModifiedCount > 0 ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0 ? NoContent() : NotFound();
    }
}
