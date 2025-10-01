# Frontend Implementation Guide - Angular 20

## Overview
This guide provides comprehensive instructions for implementing the Job Service Marketplace frontend using Angular 20, Tailwind CSS, and Angular Material.

---

## 📁 Project Structure

```
frontend/
├── src/
│   ├── app/
│   │   ├── core/                          # Core module - singleton services
│   │   │   ├── services/
│   │   │   │   ├── auth.service.ts        # Authentication service
│   │   │   │   ├── token.service.ts       # JWT token management
│   │   │   │   └── api.service.ts         # Base API service
│   │   │   ├── guards/
│   │   │   │   ├── auth.guard.ts          # Route authentication
│   │   │   │   └── role.guard.ts          # Role-based access
│   │   │   ├── interceptors/
│   │   │   │   ├── auth.interceptor.ts    # Add JWT to requests
│   │   │   │   ├── error.interceptor.ts   # Global error handling
│   │   │   │   └── loading.interceptor.ts # Loading state
│   │   │   └── models/
│   │   │       ├── user.model.ts          # User interface
│   │   │       └── api-response.model.ts  # API response types
│   │   │
│   │   ├── shared/                        # Shared module - reusable components
│   │   │   ├── components/
│   │   │   │   ├── header/
│   │   │   │   ├── footer/
│   │   │   │   ├── sidebar/
│   │   │   │   ├── loading-spinner/
│   │   │   │   ├── confirmation-dialog/
│   │   │   │   └── rating-display/
│   │   │   ├── directives/
│   │   │   │   └── role-access.directive.ts
│   │   │   ├── pipes/
│   │   │   │   ├── currency-thai.pipe.ts
│   │   │   │   └── date-thai.pipe.ts
│   │   │   └── validators/
│   │   │       └── custom-validators.ts
│   │   │
│   │   ├── features/                      # Feature modules
│   │   │   ├── auth/
│   │   │   │   ├── components/
│   │   │   │   │   ├── login/
│   │   │   │   │   ├── register/
│   │   │   │   │   └── forgot-password/
│   │   │   │   ├── services/
│   │   │   │   └── auth-routing.module.ts
│   │   │   │
│   │   │   ├── provider/
│   │   │   │   ├── components/
│   │   │   │   │   ├── profile/
│   │   │   │   │   │   ├── profile-view/
│   │   │   │   │   │   ├── profile-edit/
│   │   │   │   │   │   └── profile-create/
│   │   │   │   │   ├── availability/
│   │   │   │   │   │   └── availability-calendar/
│   │   │   │   │   ├── portfolio/
│   │   │   │   │   │   ├── portfolio-list/
│   │   │   │   │   │   └── portfolio-upload/
│   │   │   │   │   ├── dashboard/
│   │   │   │   │   │   ├── income-summary/
│   │   │   │   │   │   ├── booking-list/
│   │   │   │   │   │   └── tax-documents/
│   │   │   │   │   └── jobs/
│   │   │   │   │       ├── job-offers/
│   │   │   │   │       └── job-history/
│   │   │   │   ├── services/
│   │   │   │   │   ├── provider.service.ts
│   │   │   │   │   ├── availability.service.ts
│   │   │   │   │   └── portfolio.service.ts
│   │   │   │   └── provider-routing.module.ts
│   │   │   │
│   │   │   ├── client/
│   │   │   │   ├── components/
│   │   │   │   │   ├── marketplace/
│   │   │   │   │   │   ├── search-providers/
│   │   │   │   │   │   ├── provider-details/
│   │   │   │   │   │   └── search-filters/
│   │   │   │   │   ├── booking/
│   │   │   │   │   │   ├── booking-create/
│   │   │   │   │   │   ├── booking-details/
│   │   │   │   │   │   └── booking-list/
│   │   │   │   │   ├── payment/
│   │   │   │   │   │   ├── payment-form/
│   │   │   │   │   │   └── payment-confirmation/
│   │   │   │   │   └── reviews/
│   │   │   │   │       ├── review-create/
│   │   │   │   │       └── review-list/
│   │   │   │   ├── services/
│   │   │   │   │   ├── booking.service.ts
│   │   │   │   │   ├── payment.service.ts
│   │   │   │   │   └── review.service.ts
│   │   │   │   └── client-routing.module.ts
│   │   │   │
│   │   │   └── admin/
│   │   │       ├── components/
│   │   │       │   ├── dashboard/
│   │   │       │   │   ├── analytics-overview/
│   │   │       │   │   ├── revenue-charts/
│   │   │       │   │   └── tax-summary/
│   │   │       │   ├── users/
│   │   │       │   │   ├── user-list/
│   │   │       │   │   └── user-details/
│   │   │       │   ├── bookings/
│   │   │       │   │   ├── booking-management/
│   │   │       │   │   └── dispute-resolution/
│   │   │       │   └── reports/
│   │   │       │       ├── income-reports/
│   │   │       │       └── tax-reports/
│   │   │       ├── services/
│   │   │       │   └── admin.service.ts
│   │   │       └── admin-routing.module.ts
│   │   │
│   │   ├── app.component.ts
│   │   ├── app.component.html
│   │   ├── app.component.scss
│   │   ├── app-routing.module.ts
│   │   └── app.module.ts
│   │
│   ├── assets/
│   │   ├── images/
│   │   ├── icons/
│   │   └── i18n/                          # Internationalization files
│   │       ├── en.json
│   │       └── th.json
│   │
│   ├── environments/
│   │   ├── environment.ts
│   │   └── environment.prod.ts
│   │
│   ├── styles/
│   │   ├── _variables.scss                # SCSS variables
│   │   ├── _mixins.scss                   # SCSS mixins
│   │   ├── _tailwind.scss                 # Tailwind imports
│   │   └── styles.scss                    # Global styles
│   │
│   ├── index.html
│   ├── main.ts
│   └── styles.scss
│
├── tailwind.config.js
├── angular.json
├── package.json
├── tsconfig.json
└── README.md
```

---

## 🎨 UI Component Organization

### Material UI Components Used
- **Cards**: Provider profiles, booking cards, dashboard widgets
- **Tables**: Booking lists, transaction history, user management
- **Forms**: Login, registration, booking forms, profile editing
- **Dialogs**: Confirmations, alerts, detail views
- **Snackbars**: Notifications, success/error messages
- **Tabs**: Dashboard sections, profile tabs
- **Date Pickers**: Booking date selection, availability calendar
- **Select/Autocomplete**: Search filters, profession selection
- **Buttons**: Primary actions, secondary actions, icon buttons
- **Progress Bar/Spinner**: Loading states
- **Chips**: Tags, filters, status indicators
- **Tooltips**: Help text, information

### Tailwind CSS Utilities
- **Layout**: grid, flex, container
- **Spacing**: m-*, p-*, gap-*
- **Typography**: text-*, font-*
- **Colors**: bg-*, text-*, border-*
- **Responsive**: sm:, md:, lg:, xl:, 2xl:
- **States**: hover:, focus:, active:, disabled:

---

## 🔐 Authentication & Authorization

### Auth Flow
1. User submits login credentials
2. Backend validates and returns JWT token
3. Frontend stores token in localStorage
4. Token attached to all API requests via interceptor
5. Guards protect routes based on authentication and role

### Implementation Details

**Auth Service** (`core/services/auth.service.ts`):
- login()
- register()
- logout()
- getCurrentUser()
- isAuthenticated()
- hasRole()

**Auth Guard** (`core/guards/auth.guard.ts`):
- Protects authenticated routes
- Redirects to login if not authenticated

**Role Guard** (`core/guards/role.guard.ts`):
- Protects routes based on user role
- Supports multiple role requirements

---

## 📡 API Integration

### Base API Service
Centralized HTTP client with:
- Base URL configuration
- Error handling
- Response transformation
- Request/response logging (dev mode)

### Feature Services
Each feature module has dedicated services:
- **AuthService**: Authentication operations
- **ProviderService**: Provider profile CRUD
- **BookingService**: Booking operations
- **PaymentService**: Payment processing
- **ReviewService**: Review management
- **AdminService**: Admin operations

### HTTP Interceptors
1. **Auth Interceptor**: Adds JWT token to headers
2. **Error Interceptor**: Global error handling
3. **Loading Interceptor**: Manages loading state

---

## 📝 Forms & Validation

### Reactive Forms Pattern
All forms use Angular Reactive Forms with:
- FormBuilder for form creation
- Custom validators
- Real-time validation
- Error message display
- Submit handling

### Common Validations
- Required fields
- Email format
- Password strength (min 8 chars, uppercase, lowercase, number, special char)
- Phone number format (Thai format)
- Min/max length
- Pattern matching
- Custom business logic validators

### Form State Management
- Pristine/dirty tracking
- Touched/untouched tracking
- Valid/invalid status
- Disabled state control

---

## 📊 Dashboard & Analytics

### Charts Library: ng2-charts (Chart.js wrapper)

### Chart Types
1. **Line Chart**: Revenue over time, bookings trend
2. **Bar Chart**: Monthly income, commission breakdown
3. **Pie Chart**: Booking status distribution, tax breakdown
4. **Doughnut Chart**: Provider profession distribution

### Dashboard Widgets
- Total revenue card
- Active bookings count
- Completed jobs count
- Average rating display
- Recent transactions table
- Upcoming bookings calendar

---

## 📱 Responsive Design

### Breakpoints (Tailwind CSS)
```
sm: 640px   // Mobile landscape
md: 768px   // Tablet
lg: 1024px  // Desktop
xl: 1280px  // Large desktop
2xl: 1536px // Extra large desktop
```

### Mobile-First Approach
- Default styles for mobile
- Progressive enhancement for larger screens
- Touch-friendly UI elements
- Optimized images and assets
- Hamburger menu for mobile navigation

### Layout Strategies
- Flex/Grid responsive layouts
- Collapsible sidebar on mobile
- Stacked cards on mobile, grid on desktop
- Responsive tables (horizontal scroll or card view)
- Bottom navigation for mobile

---

## 🔔 Notifications & Feedback

### Angular Material Snackbar
- Success messages (green)
- Error messages (red)
- Info messages (blue)
- Warning messages (yellow)
- Duration: 3-5 seconds
- Action button support

### Use Cases
- Login success/failure
- Form submission success/error
- Booking created/updated
- Payment processed
- Profile updated
- Data saved/deleted

---

## 🌐 Routing Strategy

### Route Configuration
```
/                           # Home/Landing page
/auth/login                 # Login page
/auth/register              # Registration page

/client/marketplace         # Search providers
/client/providers/:id       # Provider details
/client/bookings            # My bookings
/client/bookings/create     # Create booking
/client/bookings/:id        # Booking details

/provider/dashboard         # Provider dashboard
/provider/profile           # View/Edit profile
/provider/availability      # Manage availability
/provider/portfolio         # Manage portfolio
/provider/jobs              # Job offers
/provider/income            # Income summary
/provider/tax-documents     # Tax documents

/admin/dashboard            # Admin dashboard
/admin/users                # User management
/admin/bookings             # Booking management
/admin/analytics            # Platform analytics
/admin/reports              # Reports

/unauthorized               # Access denied page
/not-found                  # 404 page
```

### Route Guards
- **AuthGuard**: Applied to all authenticated routes
- **RoleGuard**: Applied to role-specific routes (provider, admin)
- **CanDeactivate**: Applied to forms with unsaved changes

---

## 🎯 State Management

### Approach
Simple service-based state management using:
- BehaviorSubject for reactive state
- Services as state containers
- Component-level state for UI-specific data

### State Services
- **AuthStateService**: Current user, authentication status
- **BookingStateService**: Active bookings, booking cache
- **NotificationService**: Loading state, notifications
- **ThemeService**: Theme preferences (optional)

---

## 🧪 Testing Strategy

### Unit Tests (Jasmine + Karma)
- Component tests
- Service tests
- Pipe tests
- Directive tests
- Guard tests

### E2E Tests (Cypress/Protractor)
- Critical user flows
- Login/registration flow
- Booking creation flow
- Payment flow
- Provider profile creation

### Test Coverage Goals
- 80%+ coverage for services
- 70%+ coverage for components
- 100% coverage for guards and interceptors

---

## 🚀 Build & Deployment

### Development
```bash
npm install
ng serve
```

### Production Build
```bash
ng build --configuration production
```

### Build Optimization
- Ahead-of-Time (AOT) compilation
- Tree shaking
- Minification
- Lazy loading modules
- Image optimization
- Bundle analysis

### Environment Configuration
- Development: Local API, debug mode
- Staging: Test API, limited logging
- Production: Production API, error tracking only

---

## 📦 Key Dependencies

```json
{
  "dependencies": {
    "@angular/animations": "^20.0.0",
    "@angular/common": "^20.0.0",
    "@angular/core": "^20.0.0",
    "@angular/forms": "^20.0.0",
    "@angular/material": "^20.0.0",
    "@angular/platform-browser": "^20.0.0",
    "@angular/router": "^20.0.0",
    "ng2-charts": "^5.0.0",
    "chart.js": "^4.0.0",
    "rxjs": "^7.8.0",
    "tailwindcss": "^3.4.0",
    "@angular/cdk": "^20.0.0"
  }
}
```

---

## 🔧 Configuration Files

### tailwind.config.js
Custom theme, colors, and utilities

### angular.json
Project configuration, build options, assets

### tsconfig.json
TypeScript compiler options

### environment.ts
API URL, feature flags, configuration

---

## 📚 Next Steps

1. Set up Angular project with Angular CLI
2. Install and configure Tailwind CSS and Material UI
3. Implement core module (services, guards, interceptors)
4. Create shared components
5. Implement authentication feature
6. Build feature modules one by one
7. Add responsive layouts
8. Implement charts and analytics
9. Add testing
10. Performance optimization

---

## 🎨 Design System

### Color Palette
- **Primary**: Blue (#2563EB) - Main brand color
- **Secondary**: Green (#10B981) - Success, positive actions
- **Accent**: Purple (#8B5CF6) - Highlights, special features
- **Danger**: Red (#EF4444) - Errors, destructive actions
- **Warning**: Yellow (#F59E0B) - Warnings, cautions
- **Info**: Cyan (#06B6D4) - Information, neutral actions
- **Background**: Gray (#F3F4F6) - Page backgrounds
- **Surface**: White (#FFFFFF) - Card backgrounds

### Typography
- **Heading 1**: 2.5rem (40px), font-bold
- **Heading 2**: 2rem (32px), font-bold
- **Heading 3**: 1.5rem (24px), font-semibold
- **Body**: 1rem (16px), font-normal
- **Small**: 0.875rem (14px), font-normal
- **Caption**: 0.75rem (12px), font-normal

### Spacing System (Tailwind)
- xs: 0.25rem (4px)
- sm: 0.5rem (8px)
- md: 1rem (16px)
- lg: 1.5rem (24px)
- xl: 2rem (32px)
- 2xl: 3rem (48px)

---

## 🎯 Performance Best Practices

1. **Lazy Loading**: Load feature modules on demand
2. **OnPush Change Detection**: Reduce change detection cycles
3. **Virtual Scrolling**: For long lists
4. **Track By**: In *ngFor loops
5. **Pure Pipes**: For data transformation
6. **Debounce**: For search inputs
7. **Image Optimization**: Responsive images, lazy loading
8. **Bundle Size**: Code splitting, tree shaking
9. **Caching**: HTTP responses where appropriate
10. **Service Workers**: Offline support (optional)

---

**Ready to build the best Job Service Marketplace frontend! 🚀**
