using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        // Unique file number
        builder.HasIndex(p => p.FileNumber)
            .IsUnique()
            .HasDatabaseName("IX_Patients_FileNumber");

        // Unique email
        builder.HasIndex(p => p.Email)
            .IsUnique()
            .HasDatabaseName("IX_Patients_Email");

        // Index on LastName for fast search (Step 7)
        builder.HasIndex(p => p.LastName)
            .HasDatabaseName("IX_Patients_LastName");

        builder.Property(p => p.FileNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Phone)
            .HasMaxLength(20);

        // ✅ Concurrency token — EF Core will include this in the WHERE clause
        // on UPDATE/DELETE. If the value changed since the entity was loaded,
        // DbUpdateConcurrencyException is thrown.
        builder.Property(p => p.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(36);

        // Owned type: Address
        builder.OwnsOne(p => p.Address, a =>
        {
            a.Property(addr => addr.Street).HasMaxLength(200).HasColumnName("Address_Street");
            a.Property(addr => addr.City).HasMaxLength(100).HasColumnName("Address_City");
            a.Property(addr => addr.ZipCode).HasMaxLength(20).HasColumnName("Address_ZipCode");
            a.Property(addr => addr.Country).HasMaxLength(100).HasColumnName("Address_Country");
        });
    }
}