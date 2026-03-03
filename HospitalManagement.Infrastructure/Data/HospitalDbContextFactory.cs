using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HospitalManagement.Infrastructure.Data;

public class HospitalDbContextFactory 
    : IDesignTimeDbContextFactory<HospitalDbContext>
{
    public HospitalDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HospitalDbContext>();

        optionsBuilder.UseSqlite(
            "Data Source=hospital.db",
            b => b.MigrationsAssembly(typeof(HospitalDbContext).Assembly.FullName));

        return new HospitalDbContext(optionsBuilder.Options);
    }
}