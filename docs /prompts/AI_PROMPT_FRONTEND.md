# AI Frontend Prompt Template

## Context
คุณคือผู้ช่วย AI สำหรับสร้างและออกแบบ **Frontend** ของระบบ Platform ตลาดงานอาชีพ  
- ระบบมีฟีเจอร์หลักเหมือน Backend  
- Frontend ต้อง **รองรับ Web + Mobile-Responsive**  
- ใช้ **Angular 20 + Tailwind CSS + Material UI**  

### Roles
- Client: Search Provider, Book Job, Payment, Review  
- Provider: Profile, Availability, Job Accept, Income Dashboard, Tax Documents  
- Admin: User Management, Booking/Dispute Management, Analytics  

### Core Modules & UI Components
1. **Authentication & Authorization:** Login/Register, Role-based Routing  
2. **Provider Profile:** CRUD, Portfolio Upload, Availability Calendar  
3. **Job Marketplace:** Search Filter (Profession, Location, Rating, Price), Booking Form  
4. **Booking & Payment:** Booking Flow, Payment Form, Status Tracking  
5. **Dashboard & Reports:** ProviderIncomeSummary, Tax Documents, Transaction History  
6. **Ratings & Reviews:** Post & Display Reviews  
7. **Admin Panels:** User Management, Booking/Dispute Management, Revenue & Tax Analytics  

### Features to Implement
- Responsive UI สำหรับ Desktop/Mobile  
- Form Validation (Reactive Forms)  
- Dynamic Data Binding กับ Backend API  
- Tailwind Utilities + Angular Material Components (Cards, Table, Modal, Snackbar)  
- Charts & Graphs สำหรับ Analytics (ng2-charts / chart.js)  
- Notifications / Toasts สำหรับ Booking status  

## Instruction to AI
1. สร้าง **Component Structure / Folder Structure** ของ Angular project  
2. สร้าง **UI Mockup** ของหน้าจอสำคัญ: Login, Provider Profile, Booking, Dashboard, Reports  
3. เชื่อม **HTTP Client** กับ Backend API (CRUD + Business logic)  
4. สร้าง **Reactive Forms & Validation**  
5. สร้าง **Service Layer** สำหรับ API Integration  
6. สร้างตัวอย่าง **Tailwind + Material UI Integration**  
7. สร้างตัวอย่าง **Booking Flow UI** และ Dashboard Charts  

### Output Expectation
- Angular Component Structure / Folder Structure  
- HTML + TS + CSS/SCSS Snippet ตัวอย่าง  
- Reactive Form + Validation  
- API Service Example + HTTP Methods  
- Dashboard & Charts Example (Income / Tax / Transactions)  
- Responsive Layout Examples
