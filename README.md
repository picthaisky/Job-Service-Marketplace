# Employment Career Marketplace Platform

## Overview
ระบบ **Platform Marketplace งานอาชีพ** เป็นระบบดิจิทัลสำหรับเชื่อมต่อ **ผู้ให้บริการอาชีพ** (Provider) เช่น พยาบาล, ผู้ช่วย, ช่างซ่อม, คนสวน ฯลฯ กับ **ผู้ว่าจ้าง/ลูกค้า** ที่ต้องการจ้างงานระยะสั้นหรือบริการเฉพาะ โดยระบบทำหน้าที่เป็น **ตัวกลางจัดการการจอง, การชำระเงิน, การคำนวณค่าคอมมิชชั่น, การจัดการรายได้ และภาษีประจำปี**  

ระบบออกแบบให้ **เปิดกว้างให้ทุกคนเข้ามาใช้งาน** และรองรับฟีเจอร์หลักดังนี้:

- ลงทะเบียนผู้ให้บริการและผู้ว่าจ้าง
- จัดการโปรไฟล์, Portfolio, Availability
- Job / Booking Marketplace
- Payment & Escrow
- Commission / Withholding Tax Calculation
- Transaction & Tax Documents
- Ratings & Reviews
- Dashboard & Analytics สำหรับ Provider, Client, Admin

---

## Tech Stack

### Backend
- **Framework:** .NET Core 9  
- **Database:** PostgreSQL  
- **API:** RESTful + Swagger/OpenAPI Documentation  
- **Authentication:** JWT Token  
- **Payment Gateway:** Stripe / Omise / TrueMoney  

### Frontend
- **Framework:** Angular 20  
- **UI:** Tailwind CSS + Angular Material  
- **Charts / Analytics:** ng2-charts / Chart.js  
- **Responsive Design:** Mobile-first  

---

## Features

### User Roles
- **Client:** ค้นหา Provider, จองงาน, ชำระเงิน, รีวิว  
- **Provider:** สร้าง Profile, Portfolio, Availability, รับงาน, รายได้, ภาษี  
- **Admin:** ตรวจสอบเอกสาร, จัดการ Booking/Dispute, Dashboard รายได้ & ภาษี  

### Core Modules
1. User Management (Register/Login, Role-based Access)  
2. Provider Profile & Availability Management  
3. Job / Booking Management  
4. Payment & Escrow Management  
5. Commission & Withholding Calculation  
6. Transaction & Tax Documents  
7. Ratings & Reviews  
8. Admin Reports & Analytics  

---

## 📚 Documentation

### Backend Documentation
- **[Backend README](backend/README.md)** - Complete setup and development guide
- **[ERD Schema](docs/backend/ERD.md)** - Database design and relationships
- **[API Endpoints](docs/backend/API_ENDPOINTS.md)** - Complete API reference
- **[Commission & Tax Calculation](docs/backend/COMMISSION_TAX_CALCULATION.md)** - Business logic and formulas
- **[Implementation Summary](docs/backend/IMPLEMENTATION_SUMMARY.md)** - Overview of what's been built

### AI Prompts (Templates)
- **[Backend Prompt](docs /prompts/AI_PROMPT_BACKEND.md)** - Template for AI backend development
- **[Frontend Prompt](docs /prompts/AI_PROMPT_FRONTEND.md)** - Template for AI frontend development

---

## Getting Started

### Prerequisites
- .NET 9 SDK  
- PostgreSQL 15+  
- Node.js 20+ (for frontend)
- Angular CLI 20+ (for frontend)

### Backend Setup
```bash
cd backend
dotnet restore
dotnet build

# Configure database connection in appsettings.json
cd JobServiceMarketplace.API

# Create and run migrations
dotnet ef migrations add InitialCreate --project ../JobServiceMarketplace.Infrastructure
dotnet ef database update

# Run the application
dotnet run
```

Access the API:
- Swagger UI: https://localhost:5001
- API: https://localhost:5001/api

### Frontend Setup
```bash
cd frontend
npm install
npm start
```

---

## 🎯 Implementation Status

### ✅ Completed
- Complete domain model (10 entities)
- Production-ready database schema
- Entity Framework Core configuration
- DTO layer for all operations
- Payment calculation service
- JWT authentication setup
- Swagger/OpenAPI documentation
- Example API controller (ProvidersController)
- Comprehensive documentation

### 🚧 In Progress
- Additional API controllers
- Repository pattern implementation
- Service layer completion
- Input validation

### 📋 Planned
- Password hashing
- File upload functionality
- Email notifications
- Payment gateway integration
- Unit and integration tests
- Frontend development

---

## 💰 Commission & Tax Calculation

### Quick Formula
```
Gross Amount:         X
Commission (10%):     X × 0.10
Withholding Tax (3%): X × 0.03
Net to Provider:      X × 0.87 (87%)
```

### Example
```
Booking: ฿4,000
Commission: ฿400 (Platform keeps)
Withholding Tax: ฿120 (Government remits)
Provider receives: ฿3,480
```

See [detailed documentation](docs/backend/COMMISSION_TAX_CALCULATION.md) for complete calculation flows.

---

## 🔐 Security

- JWT Bearer token authentication
- Role-based authorization (Client, Provider, Admin)
- HTTPS/TLS encryption
- CORS configuration
- Input validation (to be implemented)
- Password hashing (to be implemented)

---

## 🏗️ Architecture

### Clean Architecture Pattern
```
API Layer (Controllers)
    ↓
Application Layer (Services, DTOs)
    ↓
Domain Layer (Entities, Business Rules)
    ↓
Infrastructure Layer (Database, External Services)
```

### Key Technologies
- .NET 9.0
- Entity Framework Core 9.0
- PostgreSQL with Npgsql
- JWT Authentication
- Swagger/OpenAPI
- Dependency Injection

---

## 📊 Database

### Tables
- Users
- ProviderProfiles
- Availabilities
- Portfolios
- Bookings
- Payments
- Transactions
- Reviews
- ProviderIncomeSummaries
- TaxDocuments

See [ERD documentation](docs/backend/ERD.md) for complete schema.

---

## 🚀 API Endpoints

### Authentication
- POST `/api/auth/register` - Register new user
- POST `/api/auth/login` - User login

### Providers
- GET `/api/providers` - List all providers
- GET `/api/providers/{id}` - Get provider details
- POST `/api/providers` - Create provider profile
- PUT `/api/providers/{id}` - Update provider profile

### More endpoints in development...

See [complete API documentation](docs/backend/API_ENDPOINTS.md).

---

## 🧪 Testing

```bash
# Run tests (when implemented)
cd backend
dotnet test
```

---

## 📈 Performance

- Database indexing on foreign keys
- Pagination support (default: 10 items per page)
- Efficient query patterns with EF Core
- Async/await for I/O operations

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📝 License

[Add your license here]

---

## 📞 Support

- GitHub Issues: [Report bugs or request features](https://github.com/picthaisky/Job-Service-Marketplace/issues)
- Documentation: See `docs/` folder
- Backend Guide: See `backend/README.md`

---

## ✨ Key Features Highlights

- ✅ **10% Platform Commission** - Automated calculation
- ✅ **3% Withholding Tax** - Thai tax compliance (ภงด.3)
- ✅ **Escrow Payment** - Secure payment holding
- ✅ **Tax Documents** - Automatic PND3, Invoice, Receipt generation
- ✅ **Income Reports** - Annual income summaries for providers
- ✅ **Rating System** - 1-5 star reviews with comments
- ✅ **Booking Management** - Complete lifecycle tracking
- ✅ **JWT Security** - Token-based authentication

---

## 📅 Roadmap

### Q1 2024
- Complete all API controllers
- Implement repository pattern
- Add comprehensive validation
- Unit test coverage

### Q2 2024
- Payment gateway integration
- Email notification system
- File upload functionality
- Admin dashboard

### Q3 2024
- Frontend development (Angular)
- Real-time notifications
- Mobile app (Ionic/Flutter)
- Performance optimization

### Q4 2024
- Production deployment
- Load testing
- Security audit
- Documentation finalization

---

## 🙏 Acknowledgments

- ASP.NET Core team for the excellent framework
- Entity Framework Core for powerful ORM
- PostgreSQL community
- Swagger/OpenAPI for API documentation
- All contributors and supporters

---

**Built with ❤️ for the Job Service Marketplace community**

