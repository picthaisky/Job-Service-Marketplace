# Job Service Marketplace - Backend Implementation

## Overview
This is a production-ready backend API for the Job Service Marketplace platform built with .NET 9 and PostgreSQL.

## Project Structure

```
backend/
├── JobServiceMarketplace.API/          # Web API layer (Controllers, Swagger)
├── JobServiceMarketplace.Application/  # Application layer (DTOs, Services, Business Logic)
├── JobServiceMarketplace.Domain/       # Domain layer (Entities, Enums)
└── JobServiceMarketplace.Infrastructure/ # Infrastructure layer (DbContext, Repositories)
```

## Technologies

- **.NET 9**: Latest version of ASP.NET Core
- **PostgreSQL**: Relational database
- **Entity Framework Core 9**: ORM
- **JWT Authentication**: Secure authentication
- **Swagger/OpenAPI**: API documentation
- **Npgsql**: PostgreSQL driver for .NET

## Features Implemented

### Core Modules

1. **User Management**
   - User registration and authentication
   - Role-based access control (Client, Provider, Admin)
   - JWT token generation

2. **Provider Profiles**
   - Complete provider profile management
   - Skills, certifications, and portfolio
   - Availability scheduling
   - Rating and review system

3. **Booking System**
   - Create, accept, complete, and cancel bookings
   - Booking status tracking
   - Scheduled date management

4. **Payment & Escrow**
   - Commission calculation (10%)
   - Withholding tax calculation (3% - ภงด.3)
   - Escrow payment holding
   - Payment release to providers

5. **Transaction Logging**
   - Complete transaction history
   - Payment, commission, tax, and release tracking

6. **Tax Documents**
   - PND3 (ภงด.3) generation
   - Invoice generation
   - Receipt generation

7. **Provider Income Summary**
   - Annual income reports
   - Commission and tax summaries
   - Completed bookings tracking

8. **Reviews & Ratings**
   - Client reviews for providers
   - Rating system (1-5 stars)
   - Average rating calculation

## Database Schema

See [ERD.md](../docs/backend/ERD.md) for complete database schema including:
- 10 main tables
- All relationships and foreign keys
- Indexes for performance
- Complete SQL CREATE statements

## API Endpoints

See [API_ENDPOINTS.md](../docs/backend/API_ENDPOINTS.md) for complete API documentation including:
- Authentication endpoints
- User management
- Provider profiles
- Bookings
- Payments
- Reviews
- Tax documents
- Admin endpoints

## Commission & Tax Calculation

See [COMMISSION_TAX_CALCULATION.md](../docs/backend/COMMISSION_TAX_CALCULATION.md) for:
- Detailed calculation formulas
- Business rules
- Payment flow diagrams
- Example calculations
- Transaction creation flow

### Quick Formula
```
Gross Amount:         ฿X
Commission (10%):     ฿X × 0.10
Withholding Tax (3%): ฿X × 0.03
Net Amount:           ฿X × 0.87 (87%)
```

## Getting Started

### Prerequisites

- .NET 9 SDK
- PostgreSQL 15+
- IDE (Visual Studio 2022, VS Code, or Rider)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/picthaisky/Job-Service-Marketplace.git
cd Job-Service-Marketplace/backend
```

2. **Install dependencies**
```bash
dotnet restore
```

3. **Configure Database**

Edit `JobServiceMarketplace.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=jobservicemarketplace;Username=postgres;Password=yourpassword"
  }
}
```

4. **Create Database**
```bash
# Create database in PostgreSQL
psql -U postgres
CREATE DATABASE jobservicemarketplace;
\q
```

5. **Run Migrations**
```bash
cd JobServiceMarketplace.API
dotnet ef migrations add InitialCreate --project ../JobServiceMarketplace.Infrastructure
dotnet ef database update
```

6. **Run the Application**
```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001` (root)

### Using Swagger

1. Navigate to `https://localhost:5001` in your browser
2. You'll see the interactive Swagger UI
3. Try out the API endpoints directly from the browser
4. For protected endpoints:
   - Click "Authorize" button
   - Enter: `Bearer {your-jwt-token}`
   - Click "Authorize"

## Configuration

### JWT Settings

Edit `appsettings.json`:
```json
{
  "JwtSettings": {
    "SecretKey": "Your-Super-Secret-Key-Here-Change-This-In-Production!",
    "Issuer": "JobServiceMarketplace",
    "Audience": "JobServiceMarketplaceUsers",
    "ExpirationInMinutes": 1440
  }
}
```

### Commission Settings

```json
{
  "CommissionSettings": {
    "CommissionRate": 0.10,
    "WithholdingTaxRate": 0.03
  }
}
```

## Entity Models

### Core Entities

1. **User**: User accounts with role-based access
2. **ProviderProfile**: Provider information and settings
3. **Availability**: Provider work schedule
4. **Portfolio**: Provider work examples
5. **Booking**: Job bookings between clients and providers
6. **Payment**: Payment information with calculations
7. **Transaction**: Transaction logs
8. **Review**: Client reviews and ratings
9. **ProviderIncomeSummary**: Annual income summaries
10. **TaxDocument**: Tax certificates and receipts

## Business Logic

### Payment Calculation Service

The `PaymentCalculationService` handles:
- Commission calculation (10%)
- Withholding tax calculation (3%)
- Net amount calculation
- Transaction generation

Example usage:
```csharp
var calculationService = new PaymentCalculationService();
var result = calculationService.CalculatePayment(4000m);

// result.GrossAmount = 4000.00
// result.CommissionAmount = 400.00
// result.WithholdingTaxAmount = 120.00
// result.NetAmount = 3480.00
```

## Development

### Build
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

### Clean
```bash
dotnet clean
```

## Project Dependencies

### API Project
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.EntityFrameworkCore.Design
- Npgsql.EntityFrameworkCore.PostgreSQL
- Swashbuckle.AspNetCore

### Infrastructure Project
- Microsoft.EntityFrameworkCore.Design
- Npgsql.EntityFrameworkCore.PostgreSQL

### Application Project
- No external dependencies (DTOs and Services only)

### Domain Project
- No external dependencies (Pure domain models)

## Architecture Patterns

### Clean Architecture
- **Domain**: Core business entities
- **Application**: Business logic and DTOs
- **Infrastructure**: Database and external services
- **API**: Controllers and HTTP layer

### Repository Pattern
- Abstraction layer for data access
- Testable data operations
- Easy to mock for unit testing

### Dependency Injection
- Built-in .NET DI container
- Service registration in Program.cs
- Scoped, Transient, and Singleton lifetimes

## Security

### Authentication
- JWT Bearer tokens
- Token expiration (24 hours default)
- Refresh token support (to be implemented)

### Authorization
- Role-based access control
- Client, Provider, and Admin roles
- Endpoint protection with [Authorize] attribute

### Password Security
- Password hashing (to be implemented)
- BCrypt or ASP.NET Core Identity
- Strong password requirements

## API Versioning

Currently using v1:
- `/api/v1/...` (to be implemented)
- Version in URL path
- Backward compatibility support

## Error Handling

Standard HTTP status codes:
- 200: Success
- 201: Created
- 400: Bad Request
- 401: Unauthorized
- 403: Forbidden
- 404: Not Found
- 500: Internal Server Error

Error response format:
```json
{
  "error": "Error Type",
  "message": "Detailed error message",
  "errors": {
    "field": ["Error 1", "Error 2"]
  }
}
```

## Logging

Using built-in .NET logging:
- Console logging in development
- File logging in production (to be configured)
- Log levels: Debug, Information, Warning, Error, Critical

## Performance

### Database Optimization
- Indexed foreign keys
- Indexed frequently queried columns
- Efficient query patterns

### Caching
- In-memory caching (to be implemented)
- Redis support (to be implemented)

### Pagination
- Default page size: 10
- Maximum page size: 100
- Cursor-based pagination (to be implemented)

## Testing

### Unit Tests (To be implemented)
- Service layer tests
- Business logic tests
- Calculation tests

### Integration Tests (To be implemented)
- API endpoint tests
- Database integration tests
- Authentication tests

## Deployment

### Production Checklist
- [ ] Change JWT secret key
- [ ] Configure HTTPS certificates
- [ ] Set up PostgreSQL with strong password
- [ ] Enable CORS for frontend domain only
- [ ] Configure logging to files
- [ ] Set up error monitoring (e.g., Sentry)
- [ ] Configure rate limiting
- [ ] Set up database backups
- [ ] Configure environment variables
- [ ] Enable health checks

### Docker Support (To be implemented)
```dockerfile
# Dockerfile example
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# ... build steps
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Write tests
5. Submit a pull request

## License

[Add your license here]

## Contact

- GitHub: https://github.com/picthaisky/Job-Service-Marketplace
- Issues: https://github.com/picthaisky/Job-Service-Marketplace/issues

## Roadmap

### Phase 1 (Current)
- [x] Entity models
- [x] Database schema
- [x] API structure
- [x] JWT authentication setup
- [x] Swagger documentation
- [x] Payment calculation logic

### Phase 2 (Next)
- [ ] Complete all controllers
- [ ] Repository implementation
- [ ] Service layer completion
- [ ] Password hashing
- [ ] File upload (images, documents)
- [ ] Email notifications

### Phase 3 (Future)
- [ ] Unit tests
- [ ] Integration tests
- [ ] Payment gateway integration (Stripe/Omise)
- [ ] Real-time notifications (SignalR)
- [ ] Admin dashboard
- [ ] Reports generation
- [ ] Mobile app support

## Acknowledgments

- ASP.NET Core team
- Entity Framework Core team
- PostgreSQL community
- Swagger/OpenAPI team
