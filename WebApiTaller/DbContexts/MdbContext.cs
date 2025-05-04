using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using WebApiTaller.Models;

namespace WebApiTaller.DbContexts;

public class MdbContext : DbContext
{
    public DbSet<User> Users { get; init; }
    public DbSet<Vehicle> Vehicles { get; init; }
    public DbSet<Component> Components { get; init; }
    public DbSet<Workshop> Workshops { get; init; }
    public DbSet<MaintenanceOrder> MaintenanceOrders { get; init; }
    public DbSet<Invoice> Invoices { get; init; }

    public MdbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToCollection("users");
        modelBuilder.Entity<Vehicle>().ToCollection("vehicles");
        modelBuilder.Entity<Component>().ToCollection("components");
        modelBuilder.Entity<Workshop>().ToCollection("workshops");
        modelBuilder.Entity<MaintenanceOrder>().ToCollection("maintenance_orders");
        modelBuilder.Entity<Invoice>().ToCollection("invoices");
    }
}
