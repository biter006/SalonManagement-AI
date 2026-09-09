using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalonManagement.Models;

namespace SalonManagement.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Stylist> Stylists => Set<Stylist>();
    public DbSet<SalonService> SalonServices => Set<SalonService>();
    public DbSet<StylistSchedule> StylistSchedules => Set<StylistSchedule>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<ServiceHistory> ServiceHistories => Set<ServiceHistory>();
    public DbSet<AIRecommendation> AIRecommendations => Set<AIRecommendation>();
    public DbSet<AIGeneratedMessage> AIGeneratedMessages => Set<AIGeneratedMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Customer>(entity =>
        {
            entity.HasIndex(x => x.Phone).IsUnique();
            entity.HasIndex(x => x.Email);
        });
        builder.Entity<Stylist>(entity => entity.HasIndex(x => x.Email));
        builder.Entity<SalonService>(entity => entity.HasIndex(x => x.Name).IsUnique());
        builder.Entity<StylistSchedule>(entity =>
        {
            entity.HasIndex(x => new { x.StylistId, x.WorkDate, x.StartTime, x.EndTime }).IsUnique();
            entity.HasOne(x => x.Stylist).WithMany(x => x.Schedules).HasForeignKey(x => x.StylistId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<Appointment>(entity =>
        {
            entity.HasIndex(x => new { x.StylistId, x.AppointmentDate, x.StartTime });
            entity.HasOne(x => x.Customer).WithMany(x => x.Appointments).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Stylist).WithMany(x => x.Appointments).HasForeignKey(x => x.StylistId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Service).WithMany(x => x.Appointments).HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<Invoice>(entity =>
        {
            entity.HasIndex(x => x.AppointmentId).IsUnique();
            entity.HasOne(x => x.Appointment).WithOne(x => x.Invoice).HasForeignKey<Invoice>(x => x.AppointmentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Customer).WithMany(x => x.Invoices).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<ServiceHistory>(entity =>
        {
            entity.HasIndex(x => x.AppointmentId).IsUnique();
            entity.HasOne(x => x.Customer).WithMany(x => x.ServiceHistories).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Appointment).WithMany(x => x.ServiceHistories).HasForeignKey(x => x.AppointmentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Service).WithMany(x => x.ServiceHistories).HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Stylist).WithMany(x => x.ServiceHistories).HasForeignKey(x => x.StylistId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<AIRecommendation>(entity => entity.HasOne(x => x.Customer).WithMany(x => x.AIRecommendations).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade));
        builder.Entity<AIGeneratedMessage>(entity =>
        {
            entity.HasOne(x => x.Customer).WithMany(x => x.AIGeneratedMessages).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Appointment).WithMany(x => x.AIGeneratedMessages).HasForeignKey(x => x.AppointmentId).OnDelete(DeleteBehavior.SetNull);
        });
    }
}
