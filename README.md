# Education ERP System

A production-grade Education ERP system built with .NET 10, Angular, and PostgreSQL.

## 🏗️ Architecture

This project follows **Clean Architecture** principles with:

- **Domain Layer**: Core business logic and entities
- **Application Layer**: Use cases, commands, queries (CQRS pattern)
- **Infrastructure Layer**: Data access, external services
- **Presentation Layer**: Web API controllers

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- PostgreSQL 15+ (local installation)
- Node.js 18+ (for Angular frontend)
- Visual Studio 2022 / VS Code / Rider

### Local Database Setup

1. **Install PostgreSQL** (if not already installed)

   - Download from: https://www.postgresql.org/download/
   - Default port: 5432
   - Remember your postgres user password

2. **Create Database**

   ```bash
   # Connect to PostgreSQL
   psql -U postgres

   # Create database
   CREATE DATABASE education_erp_dev;

   # Exit psql
   \q
   ```

3. **Update Connection String** (if needed)

   Edit `src/EducationERP.API/appsettings.Development.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=education_erp_dev;Username=postgres;Password=YOUR_PASSWORD"
     }
   }
   ```

### Running the Backend

```bash
# Restore packages
dotnet restore

# Run database migrations (once database schema is created)
dotnet ef database update --project src/EducationERP.Infrastructure --startup-project src/EducationERP.API

# Run the API
dotnet run --project src/EducationERP.API
```

The API will be available at:

- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000
- Swagger UI: https://localhost:5001/swagger

### Project Structure

```
EducationERP/
├── src/
│   ├── EducationERP.Domain/          # Domain entities, value objects, enums
│   ├── EducationERP.Application/     # Business logic, CQRS, DTOs
│   ├── EducationERP.Infrastructure/  # EF Core, repositories, external services
│   └── EducationERP.API/             # Web API controllers, middleware
├── tests/                            # Unit and integration tests
├── ARCHITECTURE.md                   # Detailed architecture documentation
└── README.md                         # This file
```

## 📦 NuGet Packages

### Infrastructure

- `Npgsql.EntityFrameworkCore.PostgreSQL` - PostgreSQL provider for EF Core
- `Microsoft.EntityFrameworkCore.Design` - EF Core design-time tools

### Application

- `MediatR` - CQRS pattern implementation
- `FluentValidation` - Input validation
- `AutoMapper` - Object-to-object mapping

### API

- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT authentication
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI documentation

## 🔐 Authentication

The system uses JWT (JSON Web Tokens) for authentication:

- **Access Token**: 60 minutes (production), 120 minutes (development)
- **Refresh Token**: 7 days (production), 30 days (development)

### Default Roles

- `Admin` - Full system access
- `Faculty` - Teaching staff
- `Student` - Enrolled students
- `Parent` - Student guardians

## 🗄️ Database

- **Database**: PostgreSQL 15+
- **ORM**: Entity Framework Core 10
- **Migrations**: Code-first approach

### Running Migrations

```bash
# Add a new migration
dotnet ef migrations add MigrationName --project src/EducationERP.Infrastructure --startup-project src/EducationERP.API

# Update database
dotnet ef database update --project src/EducationERP.Infrastructure --startup-project src/EducationERP.API

# Rollback to previous migration
dotnet ef database update PreviousMigrationName --project src/EducationERP.Infrastructure --startup-project src/EducationERP.API

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/EducationERP.Infrastructure --startup-project src/EducationERP.API
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

## 📚 API Documentation

Once the application is running, access the Swagger UI at:

- https://localhost:5001/swagger

## 🔄 Moving to Production

### Database Configuration

1. **Update Connection String** in `appsettings.Production.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=your-prod-server;Port=5432;Database=education_erp_prod;Username=erp_user;Password=STRONG_PASSWORD"
     }
   }
   ```

2. **Use Environment Variables** (recommended):

   ```bash
   export ConnectionStrings__DefaultConnection="Host=prod-server;..."
   export JwtSettings__Secret="YOUR_PRODUCTION_SECRET_KEY"
   ```

3. **Update JWT Secret** - Generate a strong secret key:
   ```bash
   # Generate a random secret (PowerShell)
   [Convert]::ToBase64String((1..64 | ForEach-Object { Get-Random -Maximum 256 }))
   ```

### Security Checklist

- ✅ Change JWT secret key
- ✅ Use strong database password
- ✅ Enable HTTPS only
- ✅ Configure CORS properly
- ✅ Set appropriate token expiry times
- ✅ Enable audit logging
- ✅ Regular database backups

## 🛠️ Development Guidelines

### Adding a New Feature

1. **Domain Layer**: Create entities, value objects
2. **Application Layer**: Create commands/queries, DTOs, validators
3. **Infrastructure Layer**: Implement repositories, configurations
4. **API Layer**: Create controllers, map endpoints

### Code Style

- Follow C# coding conventions
- Use meaningful variable names
- Write XML documentation for public APIs
- Keep methods small and focused
- Write unit tests for business logic

## 📖 Documentation

- [ARCHITECTURE.md](./ARCHITECTURE.md) - Detailed system architecture
- [Implementation Plan](./implementation_plan.md) - Development roadmap

## 🤝 Contributing

1. Create a feature branch
2. Make your changes
3. Write/update tests
4. Submit a pull request

## 📝 License

This project is proprietary software for educational institutions.

## 📞 Support

For issues or questions, contact the development team.

---

**Note**: This is a development setup. For production deployment, follow the security checklist and deployment guidelines in ARCHITECTURE.md.
