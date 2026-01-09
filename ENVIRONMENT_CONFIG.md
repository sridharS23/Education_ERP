# Environment Configuration Guide

## Backend Configuration

### Using appsettings.json (Current Method)

The backend currently uses `appsettings.Development.json` for configuration:

- Location: `d:\Education_ERP\Backend\EducationERP.API\appsettings.Development.json`
- Contains: Database connection, JWT settings, CORS

### Using .env File (Optional)

For better security, you can use a `.env` file:

1. **Copy the example file:**

   ```powershell
   cd d:\Education_ERP\Backend
   copy .env.example .env
   ```

2. **Update `.env` with your values:**

   - Set your PostgreSQL password
   - Change JWT_SECRET to a secure random string
   - Adjust other settings as needed

3. **Install dotenv package (if you want to use .env):**

   ```powershell
   cd d:\Education_ERP\Backend\EducationERP.API
   dotnet add package DotNetEnv
   ```

4. **Load .env in Program.cs** (add at the top):
   ```csharp
   DotNetEnv.Env.Load();
   ```

**Note:** The `.env` file is already added to `.gitignore` to keep secrets safe.

## Frontend Configuration

### Angular Environment Files

The frontend uses Angular's environment system:

**Development** (`src/environments/environment.ts`):

```typescript
export const environment = {
  production: false,
  apiUrl: "https://localhost:5001/api",
  apiBaseUrl: "https://localhost:5001",
};
```

**Production** (`src/environments/environment.prod.ts`):

```typescript
export const environment = {
  production: true,
  apiUrl: "https://your-production-domain.com/api",
  apiBaseUrl: "https://your-production-domain.com",
};
```

### Using Environment in Angular

**In your services:**

```typescript
import { environment } from "../environments/environment";

export class AuthService {
  private apiUrl = environment.apiUrl;

  login(credentials) {
    return this.http.post(`${this.apiUrl}/Auth/login`, credentials);
  }
}
```

## Connection Flow

```
┌─────────────────┐         HTTPS/HTTP          ┌──────────────────┐
│  Angular App    │ ────────────────────────────▶│  .NET API        │
│  localhost:4200 │         API Requests         │  localhost:5001  │
│                 │◀────────────────────────────│                  │
│  environment.ts │         JSON Response        │  appsettings.json│
└─────────────────┘                              └──────────────────┘
                                                          │
                                                          │ Connection String
                                                          ▼
                                                  ┌──────────────────┐
                                                  │  PostgreSQL      │
                                                  │  localhost:5432  │
                                                  └──────────────────┘
```

## Current Configuration

### Backend (Already Configured)

- **API URL**: https://localhost:5001
- **Swagger**: https://localhost:5001/swagger
- **CORS**: Allows requests from http://localhost:4200

### Frontend (Just Configured)

- **Dev Server**: http://localhost:4200
- **API Endpoint**: https://localhost:5001/api (from environment.ts)

## Next Steps

1. ✅ Environment files created
2. ⏳ Update `.env` with your PostgreSQL password (if using .env)
3. ⏳ Create Angular services to use the API
4. ⏳ Build authentication flow in frontend

The frontend can now connect to the backend using the configured API URL!
