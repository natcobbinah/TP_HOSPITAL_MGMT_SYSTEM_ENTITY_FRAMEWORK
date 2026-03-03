using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Infrastructure.Repositories;
using HospitalManagement.Tests.Helpers;

namespace HospitalManagement.Tests.Services;

public class ConsultationServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;

    public ConsultationServiceTests()
    {
        _factory = new TestDbContextFactory();
    }

    private (ConsultationService service, UnitOfWork uow) CreateServiceWithSeededData()
    {
        var context = _factory.CreateContext();

        // Seed a department
        var department = new Department
        {
            Name = "Cardiology",
            Location = "Building A"
        };
        context.Departments.Add(department);
        context.SaveChanges();

        // Seed doctors (using the generated department ID)
        var doctor = new Doctor
        {
            FirstName = "Marie",
            LastName = "Bernard",
            LicenseNumber = "LIC-001",
            Specialty = Specialty.Cardiologist,
            DepartmentId = department.Id,
            HireDate = DateTime.UtcNow.AddYears(-5)
        };
        context.Doctors.Add(doctor);

        var doctor2 = new Doctor
        {
            FirstName = "Paul",
            LastName = "Lefevre",
            LicenseNumber = "LIC-002",
            Specialty = Specialty.Surgeon,
            DepartmentId = department.Id,
            HireDate = DateTime.UtcNow.AddYears(-3)
        };
        context.Doctors.Add(doctor2);

        // Seed patients
        var patient = new Patient
        {
            FileNumber = "PAT-001",
            FirstName = "Jean",
            LastName = "Dupont",
            DateOfBirth = new DateTime(1990, 5, 15),
            Email = "jean@email.com",
            Phone = "+33612345678",
            Address = new Address
            {
                Street = "10 Rue de Paris",
                City = "Paris",
                ZipCode = "75001",
                Country = "France"
            }
        };
        context.Patients.Add(patient);

        var patient2 = new Patient
        {
            FileNumber = "PAT-002",
            FirstName = "Claire",
            LastName = "Martin",
            DateOfBirth = new DateTime(1985, 8, 22),
            Email = "claire@email.com",
            Phone = "+33698765432",
            Address = new Address
            {
                Street = "25 Avenue Victor Hugo",
                City = "Lyon",
                ZipCode = "69001",
                Country = "France"
            }
        };
        context.Patients.Add(patient2);

        context.SaveChanges();

        var uow = new UnitOfWork(context);
        var service = new ConsultationService(uow);
        return (service, uow);
    }

    // ─────────────────────────────────────────────────────
    // ALL TEST METHODS REMAIN EXACTLY THE SAME AS BEFORE
    // (PlanAsync, UpdateStatusAsync, CancelAsync, Query tests)
    // Just replace the hard-coded IDs with dynamic lookups
    // ─────────────────────────────────────────────────────

    [Fact]
    public async Task PlanAsync_ValidConsultation_ReturnsConsultationDto()
    {
        var (service, _) = CreateServiceWithSeededData();
        var dto = new CreateConsultationDto
        {
            Date = DateTime.UtcNow.AddDays(7),
            Reason = "Annual checkup",
            PatientId = 1,
            DoctorId = 1
        };

        var result = await service.PlanAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Planned", result.Status);
        Assert.Equal("Annual checkup", result.Reason);
        Assert.Contains("Jean", result.PatientName);
        Assert.Contains("Bernard", result.DoctorName);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task PlanAsync_PastDate_ThrowsArgumentException()
    {
        var (service, _) = CreateServiceWithSeededData();
        var dto = new CreateConsultationDto
        {
            Date = DateTime.UtcNow.AddDays(-1),
            PatientId = 1,
            DoctorId = 1
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.PlanAsync(dto));
        Assert.Contains("future", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PlanAsync_NonExistentPatient_ThrowsKeyNotFoundException()
    {
        var (service, _) = CreateServiceWithSeededData();
        var dto = new CreateConsultationDto
        {
            Date = DateTime.UtcNow.AddDays(7),
            PatientId = 999,
            DoctorId = 1
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.PlanAsync(dto));
    }

    [Fact]
    public async Task PlanAsync_NonExistentDoctor_ThrowsKeyNotFoundException()
    {
        var (service, _) = CreateServiceWithSeededData();
        var dto = new CreateConsultationDto
        {
            Date = DateTime.UtcNow.AddDays(7),
            PatientId = 1,
            DoctorId = 999
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.PlanAsync(dto));
    }

    [Fact]
    public async Task PlanAsync_DuplicateConsultation_ThrowsInvalidOperationException()
    {
        var (service, _) = CreateServiceWithSeededData();
        var futureDate = DateTime.UtcNow.AddDays(7);

        await service.PlanAsync(new CreateConsultationDto
        {
            Date = futureDate,
            Reason = "First visit",
            PatientId = 1,
            DoctorId = 1
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.PlanAsync(new CreateConsultationDto
            {
                Date = futureDate,
                Reason = "Duplicate attempt",
                PatientId = 1,
                DoctorId = 1
            }));
        Assert.Contains("already exists", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PlanAsync_SamePatientDifferentDoctor_Succeeds()
    {
        var (service, _) = CreateServiceWithSeededData();
        var futureDate = DateTime.UtcNow.AddDays(7);

        var r1 = await service.PlanAsync(new CreateConsultationDto
        {
            Date = futureDate, PatientId = 1, DoctorId = 1
        });
        var r2 = await service.PlanAsync(new CreateConsultationDto
        {
            Date = futureDate, PatientId = 1, DoctorId = 2
        });

        Assert.NotEqual(r1.DoctorId, r2.DoctorId);
        Assert.Equal(r1.PatientId, r2.PatientId);
    }

    [Fact]
    public async Task PlanAsync_SameDoctorDifferentPatient_Succeeds()
    {
        var (service, _) = CreateServiceWithSeededData();
        var futureDate = DateTime.UtcNow.AddDays(7);

        var r1 = await service.PlanAsync(new CreateConsultationDto
        {
            Date = futureDate, PatientId = 1, DoctorId = 1
        });
        var r2 = await service.PlanAsync(new CreateConsultationDto
        {
            Date = futureDate, PatientId = 2, DoctorId = 1
        });

        Assert.NotEqual(r1.PatientId, r2.PatientId);
        Assert.Equal(r1.DoctorId, r2.DoctorId);
    }

    [Fact]
    public async Task UpdateStatusAsync_PlannedToCompleted_Succeeds()
    {
        var (service, _) = CreateServiceWithSeededData();
        var planned = await service.PlanAsync(new CreateConsultationDto
        {
            Date = DateTime.UtcNow.AddDays(1), Reason = "Checkup",
            PatientId = 1, DoctorId = 1
        });

        var result = await service.UpdateStatusAsync(planned.Id, new UpdateConsultationStatusDto
        {
            Status = "Completed",
            Notes = "Patient in good health. Blood pressure normal."
        });

        Assert.Equal("Completed", result.Status);
        Assert.Contains("good health", result.Notes);
    }

    [Fact]
    public async Task UpdateStatusAsync_InvalidStatus_ThrowsArgumentException()
    {
        var (service, _) = CreateServiceWithSeededData();
        var planned = await service.PlanAsync(new CreateConsultationDto
        {
            Date = DateTime.UtcNow.AddDays(1), PatientId = 1, DoctorId = 1
        });

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateStatusAsync(planned.Id, new UpdateConsultationStatusDto
            {
                Status = "InvalidStatus"
            }));
        Assert.Contains("Invalid status", ex.Message);
    }

    [Fact]
    public async Task CancelAsync_PlannedConsultation_Succeeds()
    {
        var (service, _) = CreateServiceWithSeededData();
        var planned = await service.PlanAsync(new CreateConsultationDto
        {
            Date = DateTime.UtcNow.AddDays(3), PatientId = 1, DoctorId = 1
        });

        var result = await service.CancelAsync(planned.Id);

        Assert.Equal("Cancelled", result.Status);
    }

    [Fact]
    public async Task CancelAsync_AlreadyCancelled_ThrowsInvalidOperationException()
    {
        var (service, _) = CreateServiceWithSeededData();
        var planned = await service.PlanAsync(new CreateConsultationDto
        {
            Date = DateTime.UtcNow.AddDays(3), PatientId = 1, DoctorId = 1
        });
        await service.CancelAsync(planned.Id);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CancelAsync(planned.Id));
        Assert.Contains("already cancelled", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CancelAsync_CompletedConsultation_ThrowsInvalidOperationException()
    {
        var (service, _) = CreateServiceWithSeededData();
        var planned = await service.PlanAsync(new CreateConsultationDto
        {
            Date = DateTime.UtcNow.AddDays(1), PatientId = 1, DoctorId = 1
        });
        await service.UpdateStatusAsync(planned.Id, new UpdateConsultationStatusDto
        {
            Status = "Completed"
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CancelAsync(planned.Id));
        Assert.Contains("completed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingConsultation_ReturnsDto()
    {
        var (service, _) = CreateServiceWithSeededData();
        var created = await service.PlanAsync(new CreateConsultationDto
        {
            Date = DateTime.UtcNow.AddDays(5), Reason = "Follow-up",
            PatientId = 1, DoctorId = 1
        });

        var result = await service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result!.Id);
        Assert.Equal("Follow-up", result.Reason);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistent_ReturnsNull()
    {
        var (service, _) = CreateServiceWithSeededData();
        var result = await service.GetByIdAsync(999);
        Assert.Null(result);
    }

    public void Dispose()
    {
        _factory.Dispose();
    }
}