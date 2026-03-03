## `ANALYSE.md` — Project Root


# ANALYSE — Hospital Management System

## 1. Advantages and Disadvantages of the Model

### Advantages

| Aspect | Detail |
|--------|--------|
| **Clean Architecture** | Clear separation of concerns — Domain has zero dependencies |
| **Repository + UoW** | Business logic is testable without a real database |
| **Owned Types (Address)** | DRY — address logic factorized, stored inline in parent table |
| **TPH Inheritance** | Single table for all staff types — simple queries, good performance |
| **Explicit Consultation Entity** | Many-to-Many with payload — captures date, status, notes |
| **Concurrency Control** | RowVersion prevents silent data overwrites in multi-user scenarios |
| **Comprehensive Indexing** | FileNumber, Email, LastName, Doctor+Date — optimized for frequent queries |
| **Pagination** | All list endpoints are paginated — safe at any data scale |

###  Disadvantages

| Aspect | Detail |
|--------|--------|
| **TPH Discriminator** | Nullable columns for type-specific fields waste space |
| **No Soft Delete** | Physical deletion loses audit trail — production systems need soft delete |
| **No Audit Logging** | No CreatedAt/UpdatedAt/CreatedBy tracking |
| **Circular Reference** | Department.HeadDoctorId → Doctor.DepartmentId requires careful migration ordering |
| **No Caching** | Statistics endpoint hits the DB every time — should be cached |
| **Simplified Auth** | No authentication/authorization layer |

---

## 2. Optimizations for 100,000 Patients

### Database Level
- **Composite indexes** on frequently filtered columns (LastName + FirstName)
- **Covering indexes** for search queries (include Email, Phone in index)
- **Table partitioning** on Consultations by date range (archive old data)
- **Read replicas** for dashboard/statistics queries

### Application Level
- **Response caching** on statistics endpoints (Redis or in-memory)
- **Compiled queries** for hot paths:
  ```csharp
  private static readonly Func<HospitalDbContext, string, Task<Patient?>> 
      GetByFileNumber = EF.CompileAsyncQuery(
          (HospitalDbContext ctx, string fn) => 
              ctx.Patients.FirstOrDefault(p => p.FileNumber == fn));
  ```