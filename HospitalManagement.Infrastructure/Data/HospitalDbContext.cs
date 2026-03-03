using HospitalManagement.Domain.Entities;
using HospitalManagement.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Data;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Consultation> Consultations => Set<Consultation>();
    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();
    public DbSet<Nurse> Nurses => Set<Nurse>();
    public DbSet<AdministrativeStaff> AdministrativeStaff => Set<AdministrativeStaff>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PatientConfiguration());
        modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        modelBuilder.ApplyConfiguration(new DoctorConfiguration());
        modelBuilder.ApplyConfiguration(new ConsultationConfiguration());
        modelBuilder.ApplyConfiguration(new StaffMemberConfiguration());
    }

    /// <summary>
    /// Automatically updates ConcurrencyStamp for all modified Patient entities.
    /// This ensures optimistic concurrency works without manual intervention.
    /// 
    /// How it works:
    /// 1. User A loads Patient (ConcurrencyStamp = "abc")
    /// 2. User B loads same Patient (ConcurrencyStamp = "abc")
    /// 3. User A saves → ConcurrencyStamp changes to "def"
    /// 4. User B saves → EF Core WHERE includes ConcurrencyStamp = "abc"
    ///    → 0 rows affected → DbUpdateConcurrencyException thrown
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var modifiedPatients = ChangeTracker.Entries<Patient>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in modifiedPatients)
        {
            entry.Entity.ConcurrencyStamp = Guid.NewGuid().ToString();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        var modifiedPatients = ChangeTracker.Entries<Patient>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in modifiedPatients)
        {
            entry.Entity.ConcurrencyStamp = Guid.NewGuid().ToString();
        }

        return base.SaveChanges();
    }
}