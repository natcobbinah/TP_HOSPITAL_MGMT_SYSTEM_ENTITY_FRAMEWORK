using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Configurations;

public class StaffMemberConfiguration : IEntityTypeConfiguration<StaffMember>
{
    public void Configure(EntityTypeBuilder<StaffMember> builder)
    {
        // TPH: single table with discriminator
        builder.ToTable("StaffMembers");

        builder.HasKey(s => s.Id);

        builder.HasDiscriminator<string>("StaffType")
            .HasValue<Nurse>("Nurse")
            .HasValue<AdministrativeStaff>("Administrative");

        builder.Property(s => s.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.LastName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Salary).HasColumnType("decimal(18,2)");
    }
}