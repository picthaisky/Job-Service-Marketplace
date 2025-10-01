# AI Backend Prompt Template

## Context
คุณคือผู้ช่วย AI สำหรับสร้างและออกแบบ **Backend API** ของระบบ Platform ตลาดงานอาชีพ (Job & Service Marketplace)  
ระบบเป็น **Marketplace สำหรับผู้ให้บริการอาชีพต่าง ๆ** เช่น พยาบาล, ผู้ช่วย, ช่างซ่อม, คนสวน ฯลฯ  
มีฟีเจอร์สำคัญดังนี้:  

### Roles
- **Client:** ผู้ว่าจ้าง, ค้นหา Provider, จองงาน, ชำระเงิน, รีวิว  
- **Provider:** ลงทะเบียน, สร้าง Profile, ระบุ Availability, รับงาน, รายได้, ภาษี  
- **Admin:** ตรวจสอบเอกสาร, จัดการ Booking/Dispute, Dashboard รายได้ & ภาษี  

### Core Modules
1. User Management (Register, Login, Role-based Access)  
2. Provider Profile & Availability Management  
3. Job / Booking Management  
4. Payment & Escrow Management  
5. Commission & Withholding Calculation  
6. Transaction & Tax Documents  
7. Ratings & Reviews  
8. Admin Reports & Analytics  

### Technical Stack
- .NET Core 9  
- PostgreSQL (Relational DB)  
- RESTful API / Swagger Documentation  
- JWT Authentication  
- Payment Integration (Stripe, Omise, TrueMoney)  

### Features to Implement
- CRUD Users, Profiles, Bookings  
- Booking Flow: Create → Accept → Complete → Payment Release  
- Commission/Withholding calculation  
- ProviderIncomeSummary (Annual Report)  
- TaxDocuments generation (PND3, Invoice, Receipt)  
- Transaction logging  

## Instruction to AI
1. สร้าง **ERD แบบ production-ready** พร้อม table, relationship, key, datatype สำหรับ PostgreSQL  
2. สร้าง **Backend API endpoints** (CRUD + Business logic)  
3. สร้าง **Flow การคำนวณ Commission, Withholding, Net Income**  
4. ทำตัวอย่าง **C# Entity & DbContext**, Repository และ Service Layer  
5. ให้มีตัวอย่าง **Swagger/OpenAPI specification**  

### Output Expectation
- ERD Diagram / Table Structure  
- API Endpoint List + HTTP Method + Request/Response  
- C# Code Snippet สำหรับ Entity/DTO/Service  
- Flow Diagram สำหรับเงิน + ภาษี  
- ตัวอย่างการคำนวณ Commission/Withholding
