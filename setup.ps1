# Education ERP - Quick Setup Script for Windows
# This script helps set up the development environment

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Education ERP - Development Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check .NET SDK
Write-Host "Checking .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK $dotnetVersion found" -ForegroundColor Green
} catch {
    Write-Host "✗ .NET SDK not found. Please install .NET 10 SDK" -ForegroundColor Red
    Write-Host "  Download from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

# Check PostgreSQL
Write-Host ""
Write-Host "Checking PostgreSQL..." -ForegroundColor Yellow
try {
    $psqlVersion = psql --version
    Write-Host "✓ PostgreSQL found: $psqlVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ PostgreSQL not found" -ForegroundColor Red
    Write-Host "  Please install PostgreSQL 15+ from: https://www.postgresql.org/download/" -ForegroundColor Yellow
    Write-Host "  Or run: winget install PostgreSQL.PostgreSQL" -ForegroundColor Yellow
    $continue = Read-Host "Continue anyway? (y/n)"
    if ($continue -ne 'y') {
        exit 1
    }
}

# Restore NuGet packages
Write-Host ""
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Packages restored successfully" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to restore packages" -ForegroundColor Red
    exit 1
}

# Build solution
Write-Host ""
Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build --no-restore
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Build successful" -ForegroundColor Green
} else {
    Write-Host "✗ Build failed" -ForegroundColor Red
    exit 1
}

# Database setup prompt
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Database Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Before running migrations, ensure:" -ForegroundColor Yellow
Write-Host "  1. PostgreSQL is running" -ForegroundColor White
Write-Host "  2. Database 'education_erp_dev' is created" -ForegroundColor White
Write-Host "  3. Connection string in appsettings.Development.json is correct" -ForegroundColor White
Write-Host ""

$setupDb = Read-Host "Do you want to run database migrations now? (y/n)"
if ($setupDb -eq 'y') {
    Write-Host ""
    Write-Host "Running database migrations..." -ForegroundColor Yellow
    
    # This will work once we create the DbContext and migrations
    dotnet ef database update --project src/EducationERP.Infrastructure --startup-project src/EducationERP.API
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Database migrations applied successfully" -ForegroundColor Green
    } else {
        Write-Host "✗ Database migration failed" -ForegroundColor Red
        Write-Host "  Please check:" -ForegroundColor Yellow
        Write-Host "    - PostgreSQL is running" -ForegroundColor White
        Write-Host "    - Database exists" -ForegroundColor White
        Write-Host "    - Connection string is correct" -ForegroundColor White
    }
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Review DATABASE_SETUP.md for PostgreSQL configuration" -ForegroundColor White
Write-Host "  2. Update connection string in appsettings.Development.json" -ForegroundColor White
Write-Host "  3. Run: dotnet run --project src/EducationERP.API" -ForegroundColor White
Write-Host "  4. Open: https://localhost:5001/swagger" -ForegroundColor White
Write-Host ""
Write-Host "For detailed documentation, see:" -ForegroundColor Yellow
Write-Host "  - README.md - Getting started guide" -ForegroundColor White
Write-Host "  - ARCHITECTURE.md - System architecture" -ForegroundColor White
Write-Host "  - DATABASE_SETUP.md - PostgreSQL setup" -ForegroundColor White
Write-Host ""
