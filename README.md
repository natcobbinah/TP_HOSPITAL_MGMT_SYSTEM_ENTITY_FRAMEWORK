# Hospital Management System

A complete hospital management API built with ASP.NET Core 8, Entity Framework Core 8, and SQLite.

## Architecture

This project follows **Clean Architecture** with 4 layers:

```
HospitalManagement.Domain → Entities, Enums, Interfaces (no dependencies)
HospitalManagement.Application → DTOs, Services, Business Logic
HospitalManagement.Infrastructure → EF Core DbContext, Configurations, Repositories
HospitalManagement.API → Controllers, DI Configuration, Program.cs
HospitalManagement.Tests → Unit Tests (xUnit + InMemory DB)
```


## Getting Started

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

## Build
dotnet build

## Run the API
> dotnet run --project HospitalManagement.API

To access all the api-endpoints via swagger
> http://localhost:5064/swagger/index.html

To access application UI
> http://localhost:5064/

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

## SAMPLE RUNNING APPLICATION SNAPSHOTS
### UNIT TESTS

<img width="2868" height="1786" alt="hospital_db_unit_tests" src="https://github.com/user-attachments/assets/5ee7e9a0-354c-4fa7-9df8-b925a6abcb3a" />

### APPLICATION LAUNCH 

<img width="2880" height="1800" alt="application_launch success" src="https://github.com/user-attachments/assets/f35433ef-6800-4d5c-8dc4-85aaaf686ecc" />

### APPLICATION API-ENDPOINTS

<img width="2876" height="1724" alt="application api endpoints" src="https://github.com/user-attachments/assets/c5d3b072-a6a2-4353-8edb-ce77e4618099" />

## APPLICATION UI WITH RAZOR PAGES
### Dashboard
<img width="1918" height="1038" alt="dashboard" src="https://github.com/user-attachments/assets/c38ebd0f-338a-4f6f-a9f9-3294896c6a81" />

###  Dashboard patient 
<img width="1918" height="1038" alt="dashboard_patient_record_view" src="https://github.com/user-attachments/assets/2ac898e9-9376-4727-917b-00220d016cb4" />

<img width="1916" height="1038" alt="dashboard_patient_record" src="https://github.com/user-attachments/assets/06b23119-91f7-4ab6-9e4b-4bee83d54416" />

### Dashboard department
<img width="1916" height="1038" alt="dashboard_department" src="https://github.com/user-attachments/assets/106afd1e-88d7-4582-bb1a-176c60490ef5" />

### Dashboard department statistics
<img width="1916" height="1038" alt="dashboard_dept_statistics" src="https://github.com/user-attachments/assets/15769bbd-90b3-4325-aaa6-c52c5ca0125b" />

### Dashboard doctors
<img width="1918" height="1038" alt="dashboard_doctors" src="https://github.com/user-attachments/assets/f34f2fc3-f775-4cd8-9f9c-a07e4a70688e" />

<img width="1920" height="1036" alt="dashboard_doctor_planning" src="https://github.com/user-attachments/assets/6938e992-a0ce-450b-b417-dc00f5acf589" />

### Dashboard consultation
<img width="1918" height="1040" alt="dashboard_consultation" src="https://github.com/user-attachments/assets/12f3d0fb-d3cb-4dac-86ce-481edd94108c" />

### Dashboard after content added to it
<img width="1918" height="1038" alt="dashboard" src="https://github.com/user-attachments/assets/7a37c190-ffe0-4513-bc0d-e5e2fae2d7f7" />






