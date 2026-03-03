using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Configurations;

public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
{
    public void Configure(EntityTypeBuilder<Consultation> builder)
    {
        builder.ToTable("Consultations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Notes)
            .HasMaxLength(2000);

        builder.Property(c => c.Reason)
            .HasMaxLength(500);

        // Prevent duplicate: same patient, same doctor, same date/time
        builder.HasIndex(c => new { c.PatientId, c.DoctorId, c.Date })
            .IsUnique()
            .HasDatabaseName("IX_Consultations_Patient_Doctor_Date");

        // Index for querying consultations by doctor and date (Step 7)
        builder.HasIndex(c => new { c.DoctorId, c.Date })
            .HasDatabaseName("IX_Consultations_Doctor_Date");

        // Index for querying patient consultations
        builder.HasIndex(c => new { c.PatientId, c.Date })
            .HasDatabaseName("IX_Consultations_Patient_Date");

        // Relationships
        builder.HasOne(c => c.Patient)
            .WithMany(p => p.Consultations)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Doctor)
            .WithMany(d => d.Consultations)
            .HasForeignKey(c => c.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}