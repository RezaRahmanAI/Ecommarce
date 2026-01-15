# Ecommarce .NET 8 API

This project provides the backend API for the Angular storefront. It includes JWT-based authentication, user management with ASP.NET Core Identity, and dashboard management endpoints.

## Prerequisites

- .NET 8 SDK
- SQLite (bundled with EF Core provider)

## Getting started

```bash
cd backend/Ecommarce.Api
dotnet restore
dotnet ef database update
dotnet run
```

The API will be available at `https://localhost:7201` (or `http://localhost:5201`). Swagger UI is enabled in all environments.

## Configuration

- Update `JwtSettings:SigningKey` in `appsettings.json` with a strong secret.
- Adjust `AllowedOrigins` to match the Angular host.
- Set `ConnectionStrings:DefaultConnection` if you want to use another database.
- The default admin credentials are configured under `AdminUser` in `appsettings.json`.

## Endpoints

- Registration is disabled; only admin logins are permitted.
- `POST /api/auth/login`
- `GET /api/dashboard/summary` (requires authentication)
- `GET /api/dashboard/widgets` (requires authentication)
- `POST /api/dashboard/widgets` (requires authentication)
- `PUT /api/dashboard/widgets/{id}` (requires authentication)
- `DELETE /api/dashboard/widgets/{id}` (requires authentication)
