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

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _invoices.Find(_ => true).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Invoice invoice)
    {
        await _invoices.InsertOneAsync(invoice);
        return CreatedAtAction(nameof(GetAll), null, invoice);
    }
}

