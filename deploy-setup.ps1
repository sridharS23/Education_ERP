# Education ERP - Complete Setup and Deployment Script
# This script prepares the entire system for production deployment

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Education ERP - Production Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"

try {
    # Navigate to Backend
    Set-Location Backend

    Write-Host "Step 1: Adding Project References..." -ForegroundColor Yellow
    dotnet add EducationERP.Application/EducationERP.Application.csproj reference EducationERP.Domain/EducationERP.Domain.csproj
    dotnet add EducationERP.Infrastructure/EducationERP.Infrastructure.csproj reference EducationERP.Domain/EducationERP.Domain.csproj
    dotnet add EducationERP.Infrastructure/EducationERP.Infrastructure.csproj reference EducationERP.Application/EducationERP.Application.csproj
    dotnet add EducationERP.API/EducationERP.API.csproj reference EducationERP.Application/EducationERP.Application.csproj
    dotnet add EducationERP.API/EducationERP.API.csproj reference EducationERP.Infrastructure/EducationERP.Infrastructure.csproj
    Write-Host "✓ Project references added" -ForegroundColor Green

    Write-Host "`nStep 2: Installing NuGet Packages..." -ForegroundColor Yellow
    
    # Infrastructure packages
    dotnet add EducationERP.Infrastructure/EducationERP.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
    dotnet add EducationERP.Infrastructure/EducationERP.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.0
    
    # Application packages
    dotnet add EducationERP.Application/EducationERP.Application.csproj package MediatR --version 12.2.0
    dotnet add EducationERP.Application/EducationERP.Application.csproj package FluentValidation.DependencyInjectionExtensions --version 11.9.0
    
    # API packages
    dotnet add EducationERP.API/EducationERP.API.csproj package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
    dotnet add EducationERP.API/EducationERP.API.csproj package Swashbuckle.AspNetCore --version 6.5.0
    dotnet add EducationERP.API/EducationERP.API.csproj package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
    dotnet add EducationERP.API/EducationERP.API.csproj package BCrypt.Net-Next --version 4.0.3
    
    Write-Host "✓ NuGet packages installed" -ForegroundColor Green

    Write-Host "`nStep 3: Restoring Packages..." -ForegroundColor Yellow
    dotnet restore
    Write-Host "✓ Packages restored" -ForegroundColor Green

    Write-Host "`nStep 4: Building Solution..." -ForegroundColor Yellow
    dotnet build --configuration Release
    Write-Host "✓ Solution built successfully" -ForegroundColor Green

    Set-Location ..

    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "Setup Complete!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Yellow
    Write-Host "1. Ensure PostgreSQL is running" -ForegroundColor White
    Write-Host "2. Create database: CREATE DATABASE education_erp;" -ForegroundColor White
    Write-Host "3. Run migrations (I'll create these next)" -ForegroundColor White
    Write-Host "4. Start the API: cd Backend/EducationERP.API && dotnet run" -ForegroundColor White
    Write-Host ""
    Write-Host "API will be available at: https://localhost:5001/swagger" -ForegroundColor Cyan
    Write-Host ""

}
catch {
    Write-Host "`n❌ Error occurred: $_" -ForegroundColor Red
    Set-Location ..
    exit 1
}
