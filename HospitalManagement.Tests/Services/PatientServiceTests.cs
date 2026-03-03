using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using HospitalManagement.Infrastructure.Repositories;
using HospitalManagement.Tests.Helpers;

namespace HospitalManagement.Tests.Services;

public class PatientServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;

    public PatientServiceTests()
    {
        _factory = new TestDbContextFactory();
    }

    /// <summary>
    /// Creates a PatientService backed by a real SQLite in-memory database.
    /// The connection stays open via _factory for the entire test class lifetime.
    /// </summary>
    private PatientService CreateService()
    {
        var context = _factory.CreateContext();
        var unitOfWork = new UnitOfWork(context);
        return new PatientService(unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_ValidPatient_ReturnsPatientDto()
    {
        // Arrange
        var service = CreateService();
        var dto = new CreatePatientDto
        {
            FileNumber = "PAT-001",
            FirstName = "Jean",
            LastName = "Dupont",
            DateOfBirth = new DateTime(1990, 5, 15),
            Phone = "+33612345678",
            Email = "jean.dupont@email.com",
            Street = "10 Rue de la Paix",
            City = "Paris",
            ZipCode = "75001",
            Country = "France"
        };

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PAT-001", result.FileNumber);
        Assert.Equal("Jean", result.FirstName);
        Assert.Equal("Dupont", result.LastName);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task CreateAsync_FutureDateOfBirth_ThrowsArgumentException()
    {
        // Arrange
        var service = CreateService();
        var dto = new CreatePatientDto
        {
            FileNumber = "PAT-002",
            FirstName = "Marie",
            LastName = "Curie",
            DateOfBirth = DateTime.UtcNow.AddYears(1), // Future date!
            Email = "marie@email.com"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = CreateService();
        var dto1 = new CreatePatientDto
        {
            FileNumber = "PAT-003",
            FirstName = "Alice",
            LastName = "Martin",
            DateOfBirth = new DateTime(1985, 3, 20),
            Email = "alice@email.com"
        };
        var dto2 = new CreatePatientDto
        {
            FileNumber = "PAT-004",
            FirstName = "Bob",
            LastName = "Martin",
            DateOfBirth = new DateTime(1988, 7, 10),
            Email = "alice@email.com" // Duplicate!
        };

        // Act
        await service.CreateAsync(dto1);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto2));
    }

    [Fact]
    public async Task CreateAsync_DuplicateFileNumber_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = CreateService();
        var dto1 = new CreatePatientDto
        {
            FileNumber = "PAT-DUP",
            FirstName = "Test1",
            LastName = "User1",
            DateOfBirth = new DateTime(1990, 1, 1),
            Email = "test1@email.com"
        };
        var dto2 = new CreatePatientDto
        {
            FileNumber = "PAT-DUP", // Duplicate!
            FirstName = "Test2",
            LastName = "User2",
            DateOfBirth = new DateTime(1991, 2, 2),
            Email = "test2@email.com"
        };

        await service.CreateAsync(dto1);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto2));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPaginatedResults()
    {
        // Arrange
        var service = CreateService();
        for (int i = 1; i <= 25; i++)
        {
            await service.CreateAsync(new CreatePatientDto
            {
                FileNumber = $"PAT-{i:D3}",
                FirstName = $"Patient{i}",
                LastName = $"Test{i}",
                DateOfBirth = new DateTime(1990, 1, 1),
                Email = $"patient{i}@email.com"
            });
        }

        // Act
        var page1 = await service.GetAllAsync(page: 1, pageSize: 10);
        var page2 = await service.GetAllAsync(page: 2, pageSize: 10);
        var page3 = await service.GetAllAsync(page: 3, pageSize: 10);

        // Assert
        Assert.Equal(10, page1.Items.Count());
        Assert.Equal(10, page2.Items.Count());
        Assert.Equal(5, page3.Items.Count());
        Assert.Equal(25, page1.TotalCount);
        Assert.Equal(3, page1.TotalPages);
        Assert.True(page1.HasNext);
        Assert.False(page1.HasPrevious);
    }

    [Fact]
    public async Task DeleteAsync_NonExistentPatient_ThrowsKeyNotFoundException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteAsync(999));
    }

    [Fact]
    public async Task SearchByNameAsync_FindsMatchingPatients()
    {
        // Arrange
        var service = CreateService();
        await service.CreateAsync(new CreatePatientDto
        {
            FileNumber = "PAT-S1", FirstName = "Jean", LastName = "Dupont",
            DateOfBirth = new DateTime(1990, 1, 1), Email = "jd@email.com"
        });
        await service.CreateAsync(new CreatePatientDto
        {
            FileNumber = "PAT-S2", FirstName = "Marie", LastName = "Durand",
            DateOfBirth = new DateTime(1991, 2, 2), Email = "md@email.com"
        });
        await service.CreateAsync(new CreatePatientDto
        {
            FileNumber = "PAT-S3", FirstName = "Pierre", LastName = "Martin",
            DateOfBirth = new DateTime(1992, 3, 3), Email = "pm@email.com"
        });

        // Act
        var result = await service.SearchByNameAsync("Dur");

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("Marie", result.Items.First().FirstName);
    }

    [Fact]
    public async Task UpdateAsync_ValidUpdate_ReturnsUpdatedDto()
    {
        // Arrange
        var service = CreateService();
        var created = await service.CreateAsync(new CreatePatientDto
        {
            FileNumber = "PAT-UPD",
            FirstName = "Original",
            LastName = "Name",
            DateOfBirth = new DateTime(1990, 1, 1),
            Email = "original@email.com"
        });

        // Act
        var result = await service.UpdateAsync(created.Id, new UpdatePatientDto
        {
            FirstName = "Updated",
            LastName = "Name",
            Phone = "+33699999999",
            Email = "updated@email.com",
            Street = "New Street",
            City = "Lyon",
            ZipCode = "69001",
            Country = "France"
        });

        // Assert
        Assert.Equal("Updated", result.FirstName);
        Assert.Equal("updated@email.com", result.Email);
        Assert.Equal("Lyon", result.City);
    }

    public void Dispose()
    {
        _factory.Dispose();
    }
}