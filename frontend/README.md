# Job Service Marketplace - Frontend

## Overview
This is the Angular 20 frontend application for the Job Service Marketplace platform. It provides a responsive, mobile-first user interface for clients, providers, and administrators.

## Tech Stack
- **Framework**: Angular 20
- **UI Library**: Angular Material 20
- **CSS Framework**: Tailwind CSS 3.4
- **Charts**: Chart.js 4.x + ng2-charts
- **State Management**: Service-based with RxJS
- **HTTP Client**: Angular HttpClient with Interceptors
- **Routing**: Angular Router with Guards

## Features

### Authentication & Authorization
- JWT token-based authentication
- Role-based access control (Client, Provider, Admin)
- Auto token refresh
- Secure token storage
- Route guards for protected routes

### User Roles
- **Client**: Search providers, book services, make payments, leave reviews
- **Provider**: Manage profile, set availability, accept bookings, view income dashboard
- **Admin**: Manage users, view analytics, handle disputes

### Core Modules

#### 1. Authentication Module (`features/auth`)
- Login component with reactive forms
- Registration with role selection
- Email/password validation
- Error handling

#### 2. Provider Module (`features/provider`)
- Dashboard with income charts
- Profile management
- Income summary with tax breakdown
- Booking management

#### 3. Client Module (`features/client`)
- Provider marketplace with search filters
- Provider detail view
- Booking creation form
- Review system

#### 4. Admin Module (`features/admin`)
- Platform analytics dashboard
- User management
- Revenue reports

## Project Structure

```
frontend/
├── src/
│   ├── app/
│   │   ├── core/                    # Core services, guards, interceptors
│   │   │   ├── services/           # Auth, API, Loading services
│   │   │   ├── guards/             # Auth, Role guards
│   │   │   └── interceptors/       # Auth, Error interceptors
│   │   ├── shared/                  # Shared components
│   │   │   └── components/         # Header, Footer, Loading
│   │   ├── features/               # Feature modules
│   │   │   ├── auth/               # Login, Register
│   │   │   ├── provider/           # Provider dashboard, profile
│   │   │   ├── client/             # Marketplace, booking
│   │   │   └── admin/              # Admin dashboard, users
│   │   ├── app.config.ts           # App configuration
│   │   ├── app.routes.ts           # Route definitions
│   │   └── app.ts                  # Root component
│   ├── environments/               # Environment configs
│   ├── styles.scss                 # Global styles
│   └── index.html                  # Main HTML
├── angular.json                    # Angular configuration
├── tailwind.config.js              # Tailwind configuration
├── tsconfig.json                   # TypeScript configuration
└── package.json                    # Dependencies
```

## Getting Started

### Prerequisites
- Node.js 20.x or higher
- npm 10.x or higher
- Angular CLI 20.x

### Installation

1. **Navigate to frontend directory**
   ```bash
   cd frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Configure environment**
   Edit `src/environments/environment.ts` to point to your backend API:
   ```typescript
   export const environment = {
     production: false,
     apiUrl: 'https://localhost:5001/api',
     // ... other configs
   };
   ```

4. **Run development server**
   ```bash
   npm start
   # or
   ng serve
   ```

5. **Access the application**
   Open your browser and navigate to `http://localhost:4200`

### Build for Production

```bash
# Production build
npm run build
# or
ng build --configuration production

# Output will be in dist/ folder
```

## API Integration

### Base Configuration
The application uses a centralized API service (`core/services/api.ts`) for all HTTP requests.

**Environment Configuration:**
- Development: `https://localhost:5001/api`
- Production: `https://api.jobservicemarketplace.com/api`

### Authentication Flow
1. User logs in via `/auth/login`
2. Backend returns JWT token
3. Token stored in localStorage
4. Auth interceptor adds token to all requests
5. Error interceptor handles 401 errors

### Key API Endpoints

#### Authentication
- `POST /auth/login` - User login
- `POST /auth/register` - User registration

#### Providers
- `GET /providers` - List all providers (with filters)
- `GET /providers/{id}` - Get provider details
- `POST /providers` - Create provider profile
- `PUT /providers/{id}` - Update provider profile
- `GET /providers/{id}/income/summary` - Get income summary
- `GET /providers/{id}/tax-documents` - Get tax documents

#### Bookings
- `GET /bookings` - List bookings
- `POST /bookings` - Create booking
- `POST /bookings/{id}/accept` - Accept booking
- `POST /bookings/{id}/complete` - Complete booking
- `POST /bookings/{id}/cancel` - Cancel booking

## UI Components & Styling

### Tailwind CSS
Custom utility classes defined in `styles.scss`:
- `.container-custom` - Responsive container
- `.card-custom` - Custom card styling
- `.btn-primary` - Primary button
- `.btn-secondary` - Secondary button

### Angular Material
Components used:
- MatCard - Card containers
- MatFormField - Form inputs
- MatButton - Buttons
- MatIcon - Icons
- MatSelect - Dropdowns
- MatPaginator - Pagination
- MatChip - Chips/tags
- MatProgressSpinner - Loading indicators

### Responsive Design
Mobile-first approach with Tailwind breakpoints:
- `sm`: 640px (Mobile landscape)
- `md`: 768px (Tablet)
- `lg`: 1024px (Desktop)
- `xl`: 1280px (Large desktop)
- `2xl`: 1536px (Extra large)

## Charts & Analytics

### ng2-charts Integration
Used in Provider Dashboard for income visualization:
- Bar charts for income breakdown
- Responsive chart configuration
- Custom colors matching theme

**Example:**
```typescript
public barChartData: ChartConfiguration<'bar'>['data'] = {
  labels: ['Total Income', 'Commission', 'Tax', 'Net Income'],
  datasets: [{ data: [0, 0, 0, 0], label: 'Amount (฿)' }]
};
```

## Form Validation

### Reactive Forms
All forms use Angular Reactive Forms with validation:

**Login Form:**
- Email: Required, valid email format
- Password: Required, minimum 6 characters

**Register Form:**
- Email: Required, valid email
- Password: Required, minimum 6 characters
- Confirm Password: Required, must match password
- First/Last Name: Required, minimum 2 characters
- Role: Required

**Custom Validators:**
- Password match validator
- Email format validator

## State Management

Service-based state management using RxJS:

**AuthService:**
- `currentUser$` - Observable for current user
- `isAuthenticated` - Authentication status
- `userRole` - Current user role

**LoadingService:**
- `loading$` - Global loading state

## Security Features

### Implemented
- JWT token authentication
- HTTP-only token storage
- Role-based route guards
- Auth interceptor for API requests
- Error interceptor for handling errors
- HTTPS enforcement (production)

### Guards
- **authGuard** - Checks if user is authenticated
- **roleGuard** - Checks if user has required role

## Version
- Frontend Version: 1.0.0
- Angular Version: 20.3.3
- Last Updated: 2024

---

**Happy coding! 🚀**
