using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(d => d.Name)
            .IsUnique()
            .HasDatabaseName("IX_Departments_Name");

        builder.Property(d => d.Location)
            .HasMaxLength(200);

        // Self-referencing hierarchy for sub-departments (Step 6 - Feature 3)
        builder.HasOne(d => d.ParentDepartment)
            .WithMany(d => d.SubDepartments)
            .HasForeignKey(d => d.ParentDepartmentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Head doctor relationship: optional one-to-one
        // We use HasOne/WithOne but constrain from Department side
        builder.HasOne(d => d.HeadDoctor)
            .WithOne()
            .HasForeignKey<Department>(d => d.HeadDoctorId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Owned type: Address for department contact
        builder.OwnsOne(d => d.ContactAddress, a =>
        {
            a.Property(addr => addr.Street).HasMaxLength(200).HasColumnName("ContactAddress_Street");
            a.Property(addr => addr.City).HasMaxLength(100).HasColumnName("ContactAddress_City");
            a.Property(addr => addr.ZipCode).HasMaxLength(20).HasColumnName("ContactAddress_ZipCode");
            a.Property(addr => addr.Country).HasMaxLength(100).HasColumnName("ContactAddress_Country");
        });
    }
}