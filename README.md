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

## Getting Started

### Prerequisites
- .NET 9 SDK  
- PostgreSQL 15+  
- Node.js 20+  
- Angular CLI 20+  

### Backend Setup
```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run

