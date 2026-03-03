using HospitalManagement.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Tests.Helpers;

/// <summary>
/// Factory to create SQLite in-memory DbContext instances for unit tests.
/// 
/// WHY SQLite instead of InMemory provider?
/// - InMemory does NOT auto-generate RowVersion/Timestamp values
/// - InMemory does NOT enforce unique constraints or foreign keys
/// - SQLite in-memory gives us real SQL behavior with full constraint support
/// 
/// IMPORTANT: The SqliteConnection must stay OPEN for the lifetime of the test.
/// When the connection closes, the in-memory database is destroyed.
/// </summary>
public class TestDbContextFactory : IDisposable
{
    private readonly SqliteConnection _connection;

    public TestDbContextFactory()
    {
        // Create and open a shared in-memory SQLite connection
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public HospitalDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<HospitalDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new HospitalDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}