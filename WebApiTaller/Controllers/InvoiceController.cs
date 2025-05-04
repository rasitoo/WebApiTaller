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
    public async Task<IActionResult> GetAll()
    {
        if (!IsAuthorized(out var unauthorizedResult))
            return unauthorizedResult;

        var invoices = await _invoices.Find(_ => true).ToListAsync();
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
