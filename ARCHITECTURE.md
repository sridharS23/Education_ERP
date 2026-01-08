# Education ERP System - Architecture & Implementation Guide

## 📋 Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [Future-Proofing & Extensibility](#future-proofing--extensibility)
- [System Architecture](#system-architecture)
- [Database Design](#database-design)
- [Backend Structure (.NET 10)](#backend-structure-net-10)
- [Frontend Structure (Angular)](#frontend-structure-angular)
- [Security & Authentication](#security--authentication)
- [API Design Guidelines](#api-design-guidelines)
- [Development Workflow](#development-workflow)
- [Deployment Strategy](#deployment-strategy)

---

## Overview

This document outlines the architecture and implementation plan for a **production-grade Education ERP system** designed for large educational institutions (schools, colleges, universities) with thousands of users.

### Core Principles

- ✅ **Clean Architecture** - Separation of concerns, dependency inversion
- ✅ **SOLID Principles** - Maintainable, testable, extensible code
- ✅ **Domain-Driven Design** - Business logic at the core
- ✅ **Security-First** - JWT auth, role-based access, audit logging
- ✅ **Scalability** - Designed for thousands of concurrent users
- ✅ **Future-Proof** - Easy to extend with new modules and features

---

## Technology Stack

### Backend

- **Framework**: .NET 10 Web API
- **Database**: PostgreSQL 15+
- **ORM**: Entity Framework Core 10
- **Authentication**: JWT (Access + Refresh Tokens)
- **Validation**: FluentValidation
- **Logging**: Serilog
- **Caching**: Redis (optional)
- **Background Jobs**: Hangfire (optional)

### Frontend

- **Framework**: Angular 18+ (latest)
- **UI Library**: Angular Material
- **State Management**: RxJS + Services
- **HTTP Client**: Angular HttpClient
- **Routing**: Angular Router with lazy loading
- **Forms**: Reactive Forms

### DevOps

- **Version Control**: Git
- **CI/CD**: GitHub Actions / Azure DevOps
- **Containerization**: Docker
- **Orchestration**: Kubernetes (optional)
- **Monitoring**: Application Insights / ELK Stack

---

## Future-Proofing & Extensibility

> **Built for Evolution**: This architecture is designed with future updates in mind. You can add new modules, features, and integrations without disrupting existing functionality.

### 1. Modular Architecture Benefits

The Clean Architecture approach provides natural extension points:

- **Add New Modules**: Create new feature folders in Application layer without touching existing code
- **Swap Implementations**: Replace infrastructure services (email, storage, payment) without changing business logic
- **Database Changes**: Use EF Core migrations for version-controlled schema evolution
- **API Versioning**: Support multiple API versions simultaneously for backward compatibility

### 2. Extension Mechanisms

#### Plugin Architecture

```csharp
// Future modules can be added as plugins
public interface IModulePlugin
{
    string ModuleName { get; }
    void RegisterServices(IServiceCollection services);
    void ConfigureRoutes(IEndpointRouteBuilder endpoints);
}

// Example: Add Library Management module later
public class LibraryManagementPlugin : IModulePlugin
{
    public string ModuleName => "LibraryManagement";

    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<ILibraryService, LibraryService>();
        services.AddScoped<IBookRepository, BookRepository>();
    }

    public void ConfigureRoutes(IEndpointRouteBuilder endpoints)
    {
        // Register library-specific routes
    }
}
```

#### Feature Flags

```csharp
// Enable/disable features without code changes
public class FeatureFlags
{
    public bool EnableOnlineExams { get; set; }
    public bool EnableBiometricAttendance { get; set; }
    public bool EnableAIGrading { get; set; }
    public bool EnableBlockchainCertificates { get; set; }
    public bool EnableMobileApp { get; set; }
    public bool EnableVideoLectures { get; set; }
}

// Usage in controller
[HttpGet("online-exams")]
public async Task<IActionResult> GetOnlineExams()
{
    if (!_featureFlags.EnableOnlineExams)
        return NotFound("Feature not available");

    // Implementation
}
```

#### API Versioning

```csharp
// Support multiple API versions
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class StudentsController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetStudentsV1([FromQuery] PaginationParams pagination)
    {
        // V1 implementation - basic fields
        var students = await _mediator.Send(new GetStudentsQueryV1(pagination));
        return Ok(students);
    }

    [HttpGet]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetStudentsV2([FromQuery] PaginationParams pagination)
    {
        // V2 implementation - includes additional fields, better performance
        var students = await _mediator.Send(new GetStudentsQueryV2(pagination));
        return Ok(students);
    }
}
```

### 3. Database Extensibility

#### Flexible Schema Design

**JSONB Columns for Dynamic Attributes**

```sql
-- Add custom fields without schema changes
ALTER TABLE students ADD COLUMN custom_fields JSONB;

-- Query custom fields
SELECT * FROM students
WHERE custom_fields->>'nationality' = 'Indian';

-- Index for performance
CREATE INDEX idx_students_custom_fields ON students USING GIN (custom_fields);
```

**Metadata Tables for Configurable Fields**

```sql
CREATE TABLE custom_field_definitions (
    field_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    entity_type VARCHAR(50) NOT NULL,  -- 'student', 'faculty', etc.
    field_name VARCHAR(100) NOT NULL,
    field_label VARCHAR(200) NOT NULL,
    field_type VARCHAR(20) NOT NULL,   -- 'text', 'number', 'date', 'dropdown'
    validation_rules JSONB,
    is_required BOOLEAN DEFAULT false,
    display_order INTEGER,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Example: Add custom field for student caste category
INSERT INTO custom_field_definitions (entity_type, field_name, field_label, field_type, validation_rules)
VALUES ('student', 'caste_category', 'Caste Category', 'dropdown',
        '{"options": ["General", "OBC", "SC", "ST"]}');
```

#### Migration Strategy

```bash
# Version-controlled migrations
dotnet ef migrations add AddLibraryModule --project src/EducationERP.Infrastructure
dotnet ef database update --project src/EducationERP.Infrastructure

# Rollback capability
dotnet ef database update PreviousMigrationName --project src/EducationERP.Infrastructure

# Generate SQL script for production
dotnet ef migrations script --project src/EducationERP.Infrastructure --output migration.sql
```

### 4. Frontend Extensibility

#### Lazy-Loaded Modules

```typescript
// Add new modules without rebuilding entire app
const routes: Routes = [
  {
    path: "auth",
    loadChildren: () =>
      import("./features/auth/auth.module").then((m) => m.AuthModule),
  },
  {
    path: "library",
    loadChildren: () =>
      import("./features/library/library.module").then((m) => m.LibraryModule),
    canActivate: [AuthGuard],
    data: { roles: ["Admin", "Librarian"] },
  },
  {
    path: "hostel",
    loadChildren: () =>
      import("./features/hostel/hostel.module").then((m) => m.HostelModule),
    canActivate: [AuthGuard],
    data: { roles: ["Admin", "HostelWarden"] },
  },
  {
    path: "transport",
    loadChildren: () =>
      import("./features/transport/transport.module").then(
        (m) => m.TransportModule
      ),
    canActivate: [AuthGuard],
  },
];
```

#### Dynamic Menu System

```typescript
// Menu items configured from backend based on user permissions
export interface MenuItem {
  label: string;
  route: string;
  icon: string;
  permissions: string[];
  children?: MenuItem[];
  badge?: {
    value: string;
    color: string;
  };
}

// Menu service
@Injectable({ providedIn: "root" })
export class MenuService {
  getMenuForUser(userId: string): Observable<MenuItem[]> {
    return this.http.get<MenuItem[]>(`/api/menu/user/${userId}`);
  }
}

// Usage in component
export class SidenavComponent implements OnInit {
  menuItems: MenuItem[] = [];

  ngOnInit() {
    const userId = this.authService.getCurrentUserId();
    this.menuService.getMenuForUser(userId).subscribe((menu) => {
      this.menuItems = menu;
    });
  }
}
```

### 5. Integration Points

#### Webhook System

```csharp
// Allow external systems to subscribe to events
public interface IWebhookService
{
    Task PublishEventAsync(string eventType, object payload);
    Task RegisterWebhookAsync(string url, string[] eventTypes, string secret);
}

public class WebhookService : IWebhookService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWebhookRepository _repository;

    public async Task PublishEventAsync(string eventType, object payload)
    {
        var webhooks = await _repository.GetActiveWebhooksForEventAsync(eventType);

        foreach (var webhook in webhooks)
        {
            var client = _httpClientFactory.CreateClient();
            var signature = GenerateSignature(payload, webhook.Secret);

            client.DefaultRequestHeaders.Add("X-Webhook-Signature", signature);
            await client.PostAsJsonAsync(webhook.Url, new
            {
                EventType = eventType,
                Timestamp = DateTime.UtcNow,
                Data = payload
            });
        }
    }
}

// Usage: Notify external LMS when student enrolls
await _webhookService.PublishEventAsync("student.enrolled", new {
    StudentId = student.Id,
    StudentName = $"{student.FirstName} {student.LastName}",
    CourseId = enrollment.CourseId,
    CourseName = enrollment.Course.CourseName,
    EnrollmentDate = enrollment.EnrollmentDate
});
```

#### REST API + GraphQL Support

```csharp
// Future: Add GraphQL for flexible data fetching
[HttpPost("graphql")]
public async Task<IActionResult> GraphQL([FromBody] GraphQLQuery query)
{
    var result = await _graphQLExecutor.ExecuteAsync(query);
    return Ok(result);
}

// Example GraphQL query from frontend
/*
query {
  student(id: "123") {
    firstName
    lastName
    enrollments {
      course {
        courseName
      }
      subjects {
        subjectName
        credits
      }
    }
    feeRecords {
      totalAmount
      paidAmount
      status
    }
  }
}
*/
```

### 6. Scalability Patterns

#### Microservices Ready

The modular structure allows future migration to microservices:

- **Authentication Service** (already isolated)

  - User management
  - JWT token generation
  - Role & permission management

- **Academic Service**

  - Students, Faculty, Courses
  - Attendance, Exams, Results
  - Certificates

- **Finance Service**

  - Fee structures
  - Payments, Receipts
  - Financial reports

- **Notification Service**
  - Email, SMS, Push notifications
  - Template management
  - Delivery tracking

#### Caching Strategy

```csharp
// Add caching layer without changing business logic
public class CachedStudentRepository : IStudentRepository
{
    private readonly IStudentRepository _inner;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachedStudentRepository> _logger;

    public async Task<Student> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var cacheKey = $"student:{id}";
        var cached = await _cache.GetStringAsync(cacheKey, ct);

        if (cached != null)
        {
            _logger.LogInformation("Cache hit for student {StudentId}", id);
            return JsonSerializer.Deserialize<Student>(cached);
        }

        _logger.LogInformation("Cache miss for student {StudentId}", id);
        var student = await _inner.GetByIdAsync(id, ct);

        if (student != null)
        {
            var serialized = JsonSerializer.Serialize(student);
            await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            }, ct);
        }

        return student;
    }

    public async Task UpdateAsync(Student student, CancellationToken ct)
    {
        await _inner.UpdateAsync(student, ct);

        // Invalidate cache
        var cacheKey = $"student:{student.Id}";
        await _cache.RemoveAsync(cacheKey, ct);
    }
}
```

#### Message Queue Integration

```csharp
// Process heavy operations asynchronously
public interface IMessagePublisher
{
    Task PublishAsync<T>(string queue, T message);
}

public class RabbitMQPublisher : IMessagePublisher
{
    private readonly IConnection _connection;

    public async Task PublishAsync<T>(string queue, T message)
    {
        using var channel = _connection.CreateModel();
        channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        channel.BasicPublish(exchange: "", routingKey: queue, body: body);

        await Task.CompletedTask;
    }
}

// Example: Generate certificates in background
await _messagePublisher.PublishAsync("certificate-generation", new {
    StudentId = studentId,
    CourseId = courseId,
    AcademicYear = academicYear,
    RequestedBy = userId
});
```

### 7. Configuration Management

#### Environment-Based Settings

```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=education_erp_dev;Username=postgres;Password=dev123"
  },
  "JwtSettings": {
    "Secret": "dev-secret-key-min-32-chars-long",
    "Issuer": "EducationERP.Dev",
    "Audience": "EducationERP.Client",
    "ExpiryMinutes": 60
  },
  "Features": {
    "EnableOnlineExams": true,
    "EnableAIGrading": false,
    "EnableBiometricAttendance": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  }
}

// appsettings.Production.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod-db.example.com;Port=5432;Database=education_erp_prod;Username=erp_user"
  },
  "JwtSettings": {
    "Secret": "${JWT_SECRET}",  // From environment variable
    "Issuer": "EducationERP.Prod",
    "Audience": "EducationERP.Client",
    "ExpiryMinutes": 30
  },
  "Features": {
    "EnableOnlineExams": true,
    "EnableAIGrading": true,
    "EnableBiometricAttendance": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning"
    }
  }
}
```

#### External Configuration

```csharp
// Load configuration from Azure App Configuration, AWS Parameter Store, etc.
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, config) =>
        {
            var settings = config.Build();

            if (context.HostingEnvironment.IsProduction())
            {
                // Azure App Configuration
                config.AddAzureAppConfiguration(options =>
                {
                    options.Connect(settings["AzureAppConfig:ConnectionString"])
                           .ConfigureRefresh(refresh =>
                           {
                               refresh.Register("FeatureFlags:*", refreshAll: true)
                                      .SetCacheExpiration(TimeSpan.FromMinutes(5));
                           });
                });

                // Azure Key Vault for secrets
                config.AddAzureKeyVault(
                    new Uri(settings["KeyVault:Url"]),
                    new DefaultAzureCredential());
            }
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

### 8. Testing Strategy for Updates

```csharp
// Contract tests ensure backward compatibility
public class StudentApiContractTests
{
    private readonly HttpClient _client;

    [Fact]
    public async Task GetStudent_V1_ReturnsExpectedSchema()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/students/{studentId}");
        var student = await response.Content.ReadFromJsonAsync<StudentDtoV1>();

        // Assert - Ensure existing fields are present
        Assert.NotNull(student);
        Assert.NotEqual(Guid.Empty, student.Id);
        Assert.NotNull(student.Email);
        Assert.NotNull(student.FirstName);
        Assert.NotNull(student.LastName);
        // New fields in V2 should not break V1
    }

    [Fact]
    public async Task GetStudent_V2_IncludesNewFields()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v2/students/{studentId}");
        var student = await response.Content.ReadFromJsonAsync<StudentDtoV2>();

        // Assert - V2 includes all V1 fields plus new ones
        Assert.NotNull(student);
        Assert.NotNull(student.ProfilePictureUrl); // New in V2
        Assert.NotNull(student.SocialMediaLinks);  // New in V2
    }
}
```

### 9. Future Module Examples

The architecture easily supports adding these modules:

#### 📚 Library Management

- Book catalog, borrowing, returns
- Fine calculation
- Digital library resources
- Reading room allocation

#### 🏨 Hostel Management

- Room allocation
- Mess management
- Visitor tracking
- Maintenance requests

#### 🚌 Transport Management

- Bus routes and schedules
- Student transport allocation
- GPS tracking integration
- Transport fee management

#### 👥 HR Management

- Staff payroll
- Leave management
- Performance appraisals
- Recruitment tracking

#### 📦 Inventory Management

- Lab equipment tracking
- Consumables management
- Purchase orders
- Vendor management

#### 🎓 Alumni Management

- Alumni database
- Events and reunions
- Job portal
- Donation tracking

#### 💻 Online Learning

- Video lectures
- Assignments and submissions
- Discussion forums
- Live classes integration

#### 📱 Mobile App

- React Native / Flutter
- Uses same REST API
- Push notifications
- Offline support

#### 🤖 AI/ML Features

- Predictive analytics (dropout risk)
- Automated grading
- Chatbot for queries
- Personalized learning paths

#### ⛓️ Blockchain

- Immutable certificates
- Credential verification
- Tamper-proof transcripts

### 10. Upgrade Path

```
Version 1.0 - Core Modules
├── Authentication & Authorization
├── Student Management
├── Faculty Management
├── Course & Subject Management
├── Attendance Tracking
├── Exam & Results
└── Fee Management

Version 1.5 - Extended Features
├── All V1.0 features
├── Library Management
├── Certificate Generation
└── Basic Reporting

Version 2.0 - Advanced Features
├── All V1.5 features
├── Online Exams
├── Hostel Management
├── Transport Management
└── Advanced Analytics

Version 2.5 - AI & Automation
├── All V2.0 features
├── AI-powered Grading
├── Predictive Analytics
├── Chatbot Support
└── Automated Workflows

Version 3.0 - Microservices & Scale
├── All V2.5 features
├── Microservices Architecture
├── Mobile App
├── Blockchain Certificates
└── Multi-tenant Support
```

**Key Principles for Updates:**

1. ✅ **Never break existing APIs** - Use versioning, deprecation notices
2. ✅ **Database migrations are reversible** - Always test rollback procedures
3. ✅ **Feature flags for gradual rollout** - Enable for subset of users first
4. ✅ **Comprehensive testing** - Unit, integration, contract, E2E tests
5. ✅ **Documentation updates** - Keep API docs, architecture docs in sync with code
6. ✅ **Backward compatibility** - Support old clients for at least 2 versions
7. ✅ **Performance monitoring** - Track metrics before and after updates
8. ✅ **Staged deployments** - Dev → QA → Staging → Production

---

## System Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                     Presentation Layer                       │
│              (API Controllers, Middleware)                   │
│  Dependencies: Application Layer only                        │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                     Application Layer                        │
│         (Use Cases, Commands, Queries, DTOs)                │
│  Dependencies: Domain Layer only                             │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                       Domain Layer                           │
│      (Entities, Value Objects, Domain Events)               │
│  Dependencies: None (Pure business logic)                    │
└─────────────────────────────────────────────────────────────┘
                            ↑
┌─────────────────────────────────────────────────────────────┐
│                   Infrastructure Layer                       │
│  (EF Core, Repositories, External Services)                 │
│  Dependencies: Application + Domain Layers                   │
└─────────────────────────────────────────────────────────────┘
```

### Responsibility of Each Layer

#### **1. Presentation Layer (API)**

- HTTP request/response handling
- Routing and model binding
- Authentication/Authorization middleware
- Exception handling middleware
- Request validation
- API documentation (Swagger)

**What NOT to put here:**

- ❌ Business logic
- ❌ Database queries
- ❌ Complex validations

#### **2. Application Layer**

- Orchestrate business workflows
- Coordinate domain objects
- Handle use cases (CQRS pattern)
- Define DTOs for data transfer
- Input validation (FluentValidation)
- Define interfaces for infrastructure

**What NOT to put here:**

- ❌ HTTP concerns
- ❌ Database implementation details
- ❌ Core business rules (those go in Domain)

#### **3. Domain Layer**

- Core business entities
- Business rules and invariants
- Domain events
- Value objects
- Domain exceptions
- Specifications

**What NOT to put here:**

- ❌ Database concerns
- ❌ External service calls
- ❌ DTOs or API models

#### **4. Infrastructure Layer**

- Database context (EF Core)
- Repository implementations
- External service integrations (Email, SMS, Payment)
- File storage
- Caching
- Background jobs

**What NOT to put here:**

- ❌ Business logic
- ❌ Use case orchestration

---

## Database Design

### Core Principles

1. **Normalization**: Minimize data redundancy
2. **Indexing**: Optimize query performance
3. **Constraints**: Enforce data integrity
4. **Triggers**: Automate workflows (use sparingly)
5. **Audit Trail**: Track all critical changes

### Key Tables

#### Authentication & Authorization

- `users` - User accounts
- `roles` - System roles (Admin, Faculty, Student, Parent)
- `permissions` - Granular permissions
- `user_roles` - Many-to-many relationship
- `role_permissions` - Many-to-many relationship
- `refresh_tokens` - JWT refresh tokens
- `audit_logs` - Audit trail

#### Academic Management

- `students` - Student profiles
- `faculty` - Faculty profiles
- `parents` - Parent profiles
- `student_parents` - Student-parent relationships
- `courses` - Degree programs
- `subjects` - Course subjects
- `enrollments` - Student course enrollments
- `faculty_subjects` - Faculty teaching assignments
- `attendance` - Attendance records
- `exams` - Exam schedules
- `exam_results` - Student exam results
- `certificates` - Generated certificates

#### Fee & Finance

- `fee_structures` - Fee definitions per course
- `fee_records` - Student fee records
- `payments` - Payment transactions

### Database Triggers (Use Sparingly)

**When to use triggers:**

- ✅ Data integrity constraints
- ✅ Audit logging
- ✅ Simple calculations (grade from marks)

**When NOT to use triggers:**

- ❌ Complex business logic
- ❌ External API calls
- ❌ Long-running operations

**Recommended approach:** Use triggers for data integrity, domain events for business workflows.

---

## Backend Structure (.NET 10)

### Solution Structure

```
EducationERP/
├── src/
│   ├── EducationERP.Domain/              # Domain Layer
│   │   ├── Common/
│   │   │   ├── BaseEntity.cs
│   │   │   └── IAuditableEntity.cs
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Student.cs
│   │   │   ├── Faculty.cs
│   │   │   └── ...
│   │   ├── ValueObjects/
│   │   │   ├── Address.cs
│   │   │   ├── Email.cs
│   │   │   └── PhoneNumber.cs
│   │   ├── Enums/
│   │   │   ├── Gender.cs
│   │   │   ├── EnrollmentStatus.cs
│   │   │   └── ...
│   │   ├── Events/
│   │   │   ├── StudentEnrolledEvent.cs
│   │   │   └── PaymentCompletedEvent.cs
│   │   ├── Exceptions/
│   │   │   ├── DomainException.cs
│   │   │   └── NotFoundException.cs
│   │   └── Specifications/
│   │
│   ├── EducationERP.Application/         # Application Layer
│   │   ├── Common/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IApplicationDbContext.cs
│   │   │   │   ├── IStudentRepository.cs
│   │   │   │   └── ...
│   │   │   ├── DTOs/
│   │   │   │   ├── StudentDto.cs
│   │   │   │   ├── CourseDto.cs
│   │   │   │   └── ...
│   │   │   ├── Mappings/
│   │   │   │   └── MappingProfile.cs
│   │   │   └── Validators/
│   │   ├── Features/
│   │   │   ├── Auth/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── LoginCommand.cs
│   │   │   │   │   └── RefreshTokenCommand.cs
│   │   │   │   └── Queries/
│   │   │   ├── Students/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── CreateStudentCommand.cs
│   │   │   │   │   └── UpdateStudentCommand.cs
│   │   │   │   └── Queries/
│   │   │   │       ├── GetStudentByIdQuery.cs
│   │   │   │       └── GetStudentsQuery.cs
│   │   │   └── ...
│   │   └── Services/
│   │       ├── IEmailService.cs
│   │       └── IPaymentGatewayService.cs
│   │
│   ├── EducationERP.Infrastructure/      # Infrastructure Layer
│   │   ├── Data/
│   │   │   ├── Configurations/
│   │   │   │   ├── UserConfiguration.cs
│   │   │   │   ├── StudentConfiguration.cs
│   │   │   │   └── ...
│   │   │   ├── Repositories/
│   │   │   │   ├── StudentRepository.cs
│   │   │   │   └── ...
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── Migrations/
│   │   ├── Identity/
│   │   │   ├── JwtTokenGenerator.cs
│   │   │   └── PasswordHasher.cs
│   │   ├── Services/
│   │   │   ├── EmailService.cs
│   │   │   ├── PaymentGatewayService.cs
│   │   │   └── FileStorageService.cs
│   │   └── BackgroundJobs/
│   │       ├── CertificateGenerationJob.cs
│   │       └── FeeReminderJob.cs
│   │
│   └── EducationERP.API/                 # Presentation Layer
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── StudentsController.cs
│       │   ├── FacultyController.cs
│       │   └── ...
│       ├── Middleware/
│       │   ├── ExceptionHandlingMiddleware.cs
│       │   └── RequestLoggingMiddleware.cs
│       ├── Filters/
│       │   ├── ValidateModelAttribute.cs
│       │   └── RequirePermissionAttribute.cs
│       ├── Extensions/
│       │   ├── ServiceCollectionExtensions.cs
│       │   └── ClaimsPrincipalExtensions.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── appsettings.Production.json
│       └── Program.cs
│
└── tests/
    ├── EducationERP.Domain.Tests/
    ├── EducationERP.Application.Tests/
    ├── EducationERP.Infrastructure.Tests/
    └── EducationERP.API.Tests/
```

---

## Frontend Structure (Angular)

### Project Structure

```
education-erp-ui/
├── src/
│   ├── app/
│   │   ├── core/                      # Singleton services, guards, interceptors
│   │   │   ├── guards/
│   │   │   │   ├── auth.guard.ts
│   │   │   │   ├── role.guard.ts
│   │   │   │   └── permission.guard.ts
│   │   │   ├── interceptors/
│   │   │   │   ├── auth.interceptor.ts
│   │   │   │   ├── error.interceptor.ts
│   │   │   │   └── loading.interceptor.ts
│   │   │   ├── services/
│   │   │   │   ├── auth.service.ts
│   │   │   │   ├── notification.service.ts
│   │   │   │   └── storage.service.ts
│   │   │   └── models/
│   │   │       ├── user.model.ts
│   │   │       └── api-response.model.ts
│   │   │
│   │   ├── shared/                    # Shared components, directives, pipes
│   │   │   ├── components/
│   │   │   │   ├── page-header/
│   │   │   │   ├── data-table/
│   │   │   │   ├── confirmation-dialog/
│   │   │   │   └── loading-spinner/
│   │   │   ├── directives/
│   │   │   │   ├── has-permission.directive.ts
│   │   │   │   └── has-role.directive.ts
│   │   │   ├── pipes/
│   │   │   │   ├── date-format.pipe.ts
│   │   │   │   └── currency-format.pipe.ts
│   │   │   └── material.module.ts
│   │   │
│   │   ├── features/                  # Feature modules (lazy loaded)
│   │   │   ├── auth/
│   │   │   │   ├── login/
│   │   │   │   ├── forgot-password/
│   │   │   │   └── auth-routing.module.ts
│   │   │   ├── admin/
│   │   │   │   ├── dashboard/
│   │   │   │   ├── students/
│   │   │   │   │   ├── student-list/
│   │   │   │   │   ├── student-detail/
│   │   │   │   │   └── student-form/
│   │   │   │   ├── faculty/
│   │   │   │   ├── courses/
│   │   │   │   ├── reports/
│   │   │   │   └── admin-routing.module.ts
│   │   │   ├── faculty/
│   │   │   │   ├── dashboard/
│   │   │   │   ├── attendance/
│   │   │   │   ├── exams/
│   │   │   │   └── faculty-routing.module.ts
│   │   │   ├── student/
│   │   │   │   ├── dashboard/
│   │   │   │   ├── courses/
│   │   │   │   ├── attendance/
│   │   │   │   ├── results/
│   │   │   │   ├── fees/
│   │   │   │   └── student-routing.module.ts
│   │   │   └── parent/
│   │   │       ├── dashboard/
│   │   │       ├── children/
│   │   │       └── parent-routing.module.ts
│   │   │
│   │   ├── layout/
│   │   │   ├── main-layout/
│   │   │   ├── sidenav/
│   │   │   └── toolbar/
│   │   │
│   │   ├── app-routing.module.ts
│   │   ├── app.component.ts
│   │   └── app.module.ts
│   │
│   ├── environments/
│   │   ├── environment.ts
│   │   └── environment.prod.ts
│   ├── assets/
│   │   ├── images/
│   │   └── icons/
│   ├── styles/
│   │   ├── _variables.scss
│   │   ├── _mixins.scss
│   │   └── styles.scss
│   └── index.html
│
├── angular.json
├── package.json
└── tsconfig.json
```

---

## Security & Authentication

### JWT Authentication Flow

```
1. User Login
   ├── POST /api/auth/login { email, password }
   ├── Verify credentials
   ├── Generate Access Token (30 min expiry)
   ├── Generate Refresh Token (7 days expiry)
   └── Return { accessToken, refreshToken, user }

2. Authenticated Request
   ├── Include: Authorization: Bearer {accessToken}
   ├── JWT Middleware validates token
   ├── Extract user claims (id, roles, permissions)
   └── Process request

3. Token Refresh
   ├── POST /api/auth/refresh { refreshToken }
   ├── Validate refresh token
   ├── Generate new Access Token
   └── Return { accessToken }

4. Logout
   ├── POST /api/auth/logout
   ├── Revoke refresh token
   └── Clear client-side tokens
```

### Role-Based Access Control (RBAC)

**Roles:**

- `Admin` - Full system access
- `Faculty` - Teaching, attendance, grading
- `Student` - View own data, submit assignments
- `Parent` - View children's data

**Permission-Based Access:**

```csharp
[RequirePermission("students.create")]
public async Task<IActionResult> CreateStudent(CreateStudentCommand command)
{
    // Only users with "students.create" permission can access
}
```

---

## API Design Guidelines

### RESTful Conventions

```
GET    /api/students              - List all students (paginated)
GET    /api/students/{id}         - Get student by ID
POST   /api/students              - Create new student
PUT    /api/students/{id}         - Update student
DELETE /api/students/{id}         - Delete student
GET    /api/students/{id}/courses - Get student's courses
```

### Response Format

**Success Response:**

```json
{
  "success": true,
  "data": {
    "id": "123",
    "firstName": "John",
    "lastName": "Doe"
  },
  "message": "Student retrieved successfully"
}
```

**Error Response:**

```json
{
  "success": false,
  "errors": [
    {
      "field": "email",
      "message": "Email is already in use"
    }
  ],
  "message": "Validation failed"
}
```

### Pagination

```
GET /api/students?page=1&pageSize=20&sortBy=firstName&sortOrder=asc

Response:
{
  "data": [...],
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalRecords": 100
  }
}
```

---

## Development Workflow

### 1. Setup Development Environment

```bash
# Backend
cd src/EducationERP.API
dotnet restore
dotnet ef database update --project ../EducationERP.Infrastructure
dotnet run

# Frontend
cd education-erp-ui
npm install
ng serve
```

### 2. Create New Feature

```bash
# 1. Create domain entity
# 2. Create repository interface
# 3. Create commands/queries
# 4. Create controller
# 5. Create Angular service
# 6. Create Angular components
# 7. Add routes
# 8. Write tests
```

### 3. Database Migration

```bash
# Add migration
dotnet ef migrations add AddLibraryModule --project src/EducationERP.Infrastructure

# Update database
dotnet ef database update --project src/EducationERP.Infrastructure

# Rollback
dotnet ef database update PreviousMigration --project src/EducationERP.Infrastructure
```

---

## Deployment Strategy

### Environments

1. **Development** - Local development
2. **QA** - Testing environment
3. **Staging** - Pre-production
4. **Production** - Live system

### CI/CD Pipeline

```yaml
# .github/workflows/deploy.yml
name: Deploy

on:
  push:
    branches: [main]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2

      - name: Build Backend
        run: dotnet build

      - name: Run Tests
        run: dotnet test

      - name: Build Frontend
        run: |
          cd education-erp-ui
          npm install
          ng build --prod

      - name: Deploy to Azure
        run: # Deployment commands
```

---

## Conclusion

This architecture provides a solid foundation for building a production-grade Education ERP system that can evolve over time. The modular design, clean separation of concerns, and extensibility mechanisms ensure that new features can be added without disrupting existing functionality.

**Remember:**

- Start with core modules
- Add features incrementally
- Test thoroughly
- Document changes
- Monitor performance
- Gather user feedback

For questions or clarifications, refer to the implementation plan or contact the development team.
