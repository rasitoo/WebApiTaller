using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using WebApiTaller.Models;
using WebApiTaller.Models.DTO.DTOInvoice;

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
    public async Task<IActionResult> GetAll([FromQuery] string? clientId, [FromQuery] string? maintenanceId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var workshopId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";

        var filterBuilder = Builders<Invoice>.Filter;
        var filter = filterBuilder.Eq(i => i.WorkshopId, workshopId);

        if (!string.IsNullOrEmpty(clientId))
            filter &= filterBuilder.Eq(i => i.ClientId, clientId);

        if (!string.IsNullOrEmpty(maintenanceId))
            filter &= filterBuilder.Eq(i => i.MaintenanceId, maintenanceId);

        if (startDate.HasValue)
            filter &= filterBuilder.Gte(i => i.Date, startDate.Value);

        if (endDate.HasValue)
            filter &= filterBuilder.Lte(i => i.Date, endDate.Value);

        var invoices = await _invoices.Find(filter).ToListAsync();
        var dtoInvoices = invoices.Select(i => new DTOInvoiceRead
        {
            Id = i.Id,
            ClientId = i.ClientId,
            WorkshopId = i.WorkshopId,
            MaintenanceId = i.MaintenanceId,
            Total = i.Total,
            Date = i.Date
        });

        return Ok(dtoInvoices);
    }
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var workshopId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";

        var invoice = await _invoices.Find(i => i.Id == id && i.WorkshopId == workshopId).FirstOrDefaultAsync();

        if (invoice == null)
            return NotFound(new { message = "Invoice not found." });

        var dtoInvoice = new DTOInvoiceRead
        {
            Id = invoice.Id,
            ClientId = invoice.ClientId,
            WorkshopId = invoice.WorkshopId,
            MaintenanceId = invoice.MaintenanceId,
            Total = invoice.Total,
            Date = invoice.Date
        };

        return Ok(dtoInvoice);
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(DTOInvoicePost dtoInvoice)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;
        var workshopId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";

        var invoice = new Invoice
        {
            ClientId = dtoInvoice.ClientId,
            WorkshopId = workshopId,
            MaintenanceId = dtoInvoice.MaintenanceId,
            Total = dtoInvoice.Total,
            Date = dtoInvoice.Date
        };

        await _invoices.InsertOneAsync(invoice);
        return CreatedAtAction(nameof(GetAll), null, invoice);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var result = await _invoices.DeleteOneAsync(i => i.Id == id);

        if (result.DeletedCount == 0)
            return NotFound(new { message = "Invoice not found." });

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
