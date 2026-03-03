using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.LastName)
            .IsRequired()
            .HasMaxLength(100);

        // Unique license number
        builder.HasIndex(d => d.LicenseNumber)
            .IsUnique()
            .HasDatabaseName("IX_Doctors_LicenseNumber");

        builder.Property(d => d.LicenseNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.Specialty)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Many-to-One: Doctor → Department
        // DeleteBehavior.Restrict: Cannot delete department with assigned doctors.
        // Justification: Doctors are critical records — deleting them silently would lose
        // medical history. The system should force reassignment before department deletion.
        builder.HasOne(d => d.Department)
            .WithMany(dep => dep.Doctors)
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index on DepartmentId for fast lookups
        builder.HasIndex(d => d.DepartmentId)
            .HasDatabaseName("IX_Doctors_DepartmentId");
    }
}