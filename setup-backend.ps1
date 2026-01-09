# Education ERP - Complete Setup Script
# This script sets up the entire backend with .NET and prepares for Angular frontend

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Education ERP - Complete Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Change to Backend directory
Set-Location Backend

# Update all projects to target net8.0
Write-Host "Updating project target frameworks to net8.0..." -ForegroundColor Yellow

$projects = @(
    "EducationERP.Domain\EducationERP.Domain.csproj",
    "EducationERP.Application\EducationERP.Application.csproj",
    "EducationERP.Infrastructure\EducationERP.Infrastructure.csproj",
    "EducationERP.API\EducationERP.API.csproj"
)

foreach ($proj in $projects) {
    if (Test-Path $proj) {
        (Get-Content $proj) -replace 'net10.0', 'net8.0' | Set-Content $proj
        Write-Host "✓ Updated $proj" -ForegroundColor Green
    }
}

# Add projects to solution
Write-Host ""
Write-Host "Adding projects to solution..." -ForegroundColor Yellow
dotnet sln add EducationERP.Domain\EducationERP.Domain.csproj
dotnet sln add EducationERP.Application\EducationERP.Application.csproj
dotnet sln add EducationERP.Infrastructure\EducationERP.Infrastructure.csproj
dotnet sln add EducationERP.API\EducationERP.API.csproj

# Add project references
Write-Host ""
Write-Host "Setting up project references..." -ForegroundColor Yellow
dotnet add EducationERP.Application\EducationERP.Application.csproj reference EducationERP.Domain\EducationERP.Domain.csproj
dotnet add EducationERP.Infrastructure\EducationERP.Infrastructure.csproj reference EducationERP.Domain\EducationERP.Domain.csproj
dotnet add EducationERP.Infrastructure\EducationERP.Infrastructure.csproj reference EducationERP.Application\EducationERP.Application.csproj
dotnet add EducationERP.API\EducationERP.API.csproj reference EducationERP.Application\EducationERP.Application.csproj
dotnet add EducationERP.API\EducationERP.API.csproj reference EducationERP.Infrastructure\EducationERP.Infrastructure.csproj

# Add NuGet packages
Write-Host ""
Write-Host "Installing NuGet packages..." -ForegroundColor Yellow

# Infrastructure packages
dotnet add EducationERP.Infrastructure\EducationERP.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
dotnet add EducationERP.Infrastructure\EducationERP.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.0

# Application packages
dotnet add EducationERP.Application\EducationERP.Application.csproj package MediatR --version 12.2.0
dotnet add EducationERP.Application\EducationERP.Application.csproj package FluentValidation --version 11.9.0
dotnet add EducationERP.Application\EducationERP.Application.csproj package FluentValidation.DependencyInjectionExtensions --version 11.9.0
dotnet add EducationERP.Application\EducationERP.Application.csproj package AutoMapper --version 13.0.1

# API packages
dotnet add EducationERP.API\EducationERP.API.csproj package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
dotnet add EducationERP.API\EducationERP.API.csproj package Swashbuckle.AspNetCore --version 6.5.0
dotnet add EducationERP.API\EducationERP.API.csproj package Microsoft.EntityFrameworkCore.Tools --version 8.0.0

# Restore and build
Write-Host ""
Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore

Write-Host ""
Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. I'll now create all the Domain entities" -ForegroundColor White
Write-Host "2. Set up the Database context" -ForegroundColor White
Write-Host "3. Create the initial migration" -ForegroundColor White
Write-Host "4. Set up the Angular frontend" -ForegroundColor White
Write-Host ""

Set-Location ..
