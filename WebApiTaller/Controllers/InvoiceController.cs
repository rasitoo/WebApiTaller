using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApiTaller.Models;

namespace WebApiTaller.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoiceController : ControllerBase
{
    private readonly IMongoCollection<Invoice> _invoices;

    public InvoiceController(IMongoDatabase db)
    {
        _invoices = db.GetCollection<Invoice>("invoices");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var invoices = await _invoices.Find(_ => true).ToListAsync();
        return Ok(invoices);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Invoice invoice)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        await _invoices.InsertOneAsync(invoice);
        return CreatedAtAction(nameof(GetAll), null, invoice);
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
