# 🏥 Hospital Management System

A complete hospital management API built with ASP.NET Core 8, Entity Framework Core 8, and SQLite.

## 🏗️ Architecture

This project follows **Clean Architecture** with 4 layers:

```
HospitalManagement.Domain → Entities, Enums, Interfaces (no dependencies)
HospitalManagement.Application → DTOs, Services, Business Logic
HospitalManagement.Infrastructure → EF Core DbContext, Configurations, Repositories
HospitalManagement.API → Controllers, DI Configuration, Program.cs
HospitalManagement.Tests → Unit Tests (xUnit + InMemory DB)
```


## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- EF Core CLI tools: `dotnet tool install --global dotnet-ef`

### Run the Application

# Clone and navigate
cd HospitalManagement

# Restore packages
dotnet restore

# Apply migrations
dotnet ef database update \
    --project HospitalManagement.Infrastructure \
    --startup-project HospitalManagement.API

# Run the API
> dotnet run --project HospitalManagement.API

To access all the api-endpoints via swagger
> http://localhost:5064/swagger/index.html

# RUN TESTS
Run Tests
> dotnet test HospitalManagement.Tests --verbosity normal

```
| Method | Endpoint                                   | Description                     |
| ------ | ------------------------------------------ | ------------------------------- |
| GET    | `/api/patients`                            | List patients (paginated)       |
| GET    | `/api/patients/{id}`                       | Get patient by ID               |
| GET    | `/api/patients/{id}/detail`                | Patient file + consultations    |
| GET    | `/api/patients/search?name=X`              | Search patients by name         |
| POST   | `/api/patients`                            | Create patient                  |
| PUT    | `/api/patients/{id}`                       | Update patient                  |
| DELETE | `/api/patients/{id}`                       | Delete patient                  |
| GET    | `/api/doctors`                             | List all doctors                |
| GET    | `/api/doctors/{id}/planning`               | Doctor planning + consultations |
| POST   | `/api/doctors`                             | Create doctor                   |
| GET    | `/api/departments`                         | List departments                |
| GET    | `/api/departments/stats`                   | Department statistics           |
| POST   | `/api/departments`                         | Create department               |
| GET    | `/api/consultations/{id}`                  | Get consultation                |
| GET    | `/api/consultations/patient/{id}/upcoming` | Upcoming consultations          |
| GET    | `/api/consultations/doctor/{id}/today`     | Today's consultations           |
| POST   | `/api/consultations`                       | Plan consultation               |
| PUT    | `/api/consultations/{id}/status`           | Update consultation status      |
| PUT    | `/api/consultations/{id}/cancel`           | Cancel consultation             |
```

## Database
Engine: SQLite (hospital.db created automatically)
Migrations: Auto-applied on startup in development mode

## Key Design Decisions
Owned Types for Address — reused across Patient and Department
TPH Inheritance for StaffMember hierarchy (Doctor, Nurse, Admin)
Optimistic Concurrency via RowVersion on Patient entity
Restrict Delete on Doctor→Department (prevent accidental data loss)
Cascade Delete on Consultation→Patient (consultations follow patient)
Projections for statistics (SQL-level aggregation)
