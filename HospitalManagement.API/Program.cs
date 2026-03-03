using HospitalManagement.Application.Services;
using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Infrastructure.Data;
using HospitalManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────
// 1. Database Configuration (SQLite)
// ──────────────────────────────────────────────
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("HospitalManagement.Infrastructure")));

// ──────────────────────────────────────────────
// 2. Dependency Injection — Repository & UoW
// ──────────────────────────────────────────────
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IConsultationRepository, ConsultationRepository>();

// ──────────────────────────────────────────────
// 3. Dependency Injection — Application Services
// ──────────────────────────────────────────────
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IConsultationService, ConsultationService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// ──────────────────────────────────────────────
// 4. Controllers + JSON options
// ──────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

//  Add Razor Pages support
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hospital Management API",
        Version = "v1",
        Description = "API for managing patients, doctors, departments, and consultations.",
        Contact = new OpenApiContact
        {
            Name = "HealthTech Solutions",
            Email = "dev@healthtech.com"
        }
    });
});

var app = builder.Build();

// ──────────────────────────────────────────────
// 5. Middleware Pipeline
// ──────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hospital API v1");
        options.RoutePrefix = "swagger"; // Accessible at /swagger
    });
}

// Serve static files (CSS, JS, images)
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthorization();

// Map both API controllers AND Razor Pages
app.MapControllers();
app.MapRazorPages();

// ──────────────────────────────────────────────
// 6. Auto-apply migrations on startup (dev only)
// ──────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
    db.Database.Migrate();
}

app.Run();