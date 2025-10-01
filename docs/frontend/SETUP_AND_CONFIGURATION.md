# Angular Setup and Configuration Guide

Complete guide for setting up and configuring the Job Service Marketplace Angular 20 frontend application.

---

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Initial Setup](#initial-setup)
3. [Project Structure Setup](#project-structure-setup)
4. [Angular Material Setup](#angular-material-setup)
5. [Tailwind CSS Setup](#tailwind-css-setup)
6. [Chart.js Setup](#chartjs-setup)
7. [Environment Configuration](#environment-configuration)
8. [Module Organization](#module-organization)
9. [Routing Configuration](#routing-configuration)
10. [Build Configuration](#build-configuration)

---

## Prerequisites

### Required Software
```bash
# Node.js (v20.x or higher)
node --version  # Should output v20.x.x or higher

# npm (v10.x or higher)
npm --version   # Should output v10.x.x or higher

# Angular CLI (v20.x)
npm install -g @angular/cli@20
ng version
```

### Verify Installation
```bash
# Check Node.js
node --version

# Check npm
npm --version

# Check Angular CLI
ng version
```

---

## Initial Setup

### 1. Create New Angular Project

```bash
# Navigate to project root
cd /path/to/Job-Service-Marketplace

# Create Angular app
ng new frontend --routing --style=scss

# Choose the following options:
# - Would you like to add Angular routing? Yes
# - Which stylesheet format would you like to use? SCSS
```

### 2. Navigate to Frontend Directory

```bash
cd frontend
```

### 3. Install Core Dependencies

```bash
# Install Angular Material
ng add @angular/material

# When prompted:
# - Choose a prebuilt theme: Custom
# - Set up global Angular Material typography styles? Yes
# - Include the Angular animations module? Include and enable animations

# Install Tailwind CSS
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init

# Install Chart.js
npm install chart.js ng2-charts

# Install date handling
npm install date-fns

# Install HTTP client (already included in Angular)
# Install Forms (already included in Angular)
```

---

## Project Structure Setup

### 1. Generate Core Modules

```bash
# Generate core module (singleton services, guards, interceptors)
ng generate module core --module app

# Generate shared module (reusable components, directives, pipes)
ng generate module shared --module app
```

### 2. Generate Feature Modules

```bash
# Authentication module
ng generate module features/auth --routing

# Provider module
ng generate module features/provider --routing

# Client module
ng generate module features/client --routing

# Admin module
ng generate module features/admin --routing
```

### 3. Generate Core Services

```bash
# Auth service
ng generate service core/services/auth

# API service
ng generate service core/services/api

# Token service
ng generate service core/services/token

# Loading service
ng generate service core/services/loading
```

### 4. Generate Guards

```bash
# Auth guard
ng generate guard core/guards/auth

# Role guard
ng generate guard core/guards/role
```

### 5. Generate Interceptors

```bash
# Auth interceptor
ng generate interceptor core/interceptors/auth

# Error interceptor
ng generate interceptor core/interceptors/error

# Loading interceptor
ng generate interceptor core/interceptors/loading
```

### 6. Generate Shared Components

```bash
# Header component
ng generate component shared/components/header

# Footer component
ng generate component shared/components/footer

# Loading spinner
ng generate component shared/components/loading-spinner

# Confirmation dialog
ng generate component shared/components/confirmation-dialog

# Rating display
ng generate component shared/components/rating-display
```

---

## Angular Material Setup

### 1. Configure Material Theme

Create `src/styles/material-theme.scss`:

```scss
@use '@angular/material' as mat;

// Include the common styles for Angular Material
@include mat.core();

// Define custom palettes
$primary-palette: (
  50: #e3f2fd,
  100: #bbdefb,
  200: #90caf9,
  300: #64b5f6,
  400: #42a5f5,
  500: #2196f3,
  600: #1e88e5,
  700: #1976d2,
  800: #1565c0,
  900: #0d47a1,
  A100: #82b1ff,
  A200: #448aff,
  A400: #2979ff,
  A700: #2962ff,
  contrast: (
    50: rgba(black, 0.87),
    100: rgba(black, 0.87),
    200: rgba(black, 0.87),
    300: rgba(black, 0.87),
    400: rgba(black, 0.87),
    500: white,
    600: white,
    700: white,
    800: white,
    900: white,
    A100: rgba(black, 0.87),
    A200: white,
    A400: white,
    A700: white,
  )
);

$accent-palette: mat.define-palette(mat.$green-palette, 500);
$warn-palette: mat.define-palette(mat.$red-palette, 500);

// Create the theme
$theme: mat.define-light-theme((
  color: (
    primary: mat.define-palette($primary-palette),
    accent: $accent-palette,
    warn: $warn-palette,
  ),
  typography: mat.define-typography-config(),
  density: 0,
));

// Include theme styles for core and each component
@include mat.all-component-themes($theme);

// Custom Material overrides
.mat-mdc-raised-button {
  font-weight: 500;
  letter-spacing: 0.5px;
}

.mat-mdc-card {
  border-radius: 8px;
}

.mat-mdc-form-field {
  width: 100%;
}

// Snackbar custom styles
.success-snackbar {
  --mdc-snackbar-container-color: #10b981;
  --mdc-snackbar-supporting-text-color: white;
}

.error-snackbar {
  --mdc-snackbar-container-color: #ef4444;
  --mdc-snackbar-supporting-text-color: white;
}

.warning-snackbar {
  --mdc-snackbar-container-color: #f59e0b;
  --mdc-snackbar-supporting-text-color: white;
}

.info-snackbar {
  --mdc-snackbar-container-color: #3b82f6;
  --mdc-snackbar-supporting-text-color: white;
}
```

### 2. Import Material Modules

Create `src/app/shared/material.module.ts`:

```typescript
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatTabsModule } from '@angular/material/tabs';
import { MatChipsModule } from '@angular/material/chips';
import { MatBadgeModule } from '@angular/material/badge';
import { MatStepperModule } from '@angular/material/stepper';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSliderModule } from '@angular/material/slider';

const materialModules = [
  MatButtonModule,
  MatCardModule,
  MatFormFieldModule,
  MatInputModule,
  MatSelectModule,
  MatCheckboxModule,
  MatRadioModule,
  MatDatepickerModule,
  MatNativeDateModule,
  MatIconModule,
  MatToolbarModule,
  MatSidenavModule,
  MatListModule,
  MatTableModule,
  MatPaginatorModule,
  MatSortModule,
  MatProgressSpinnerModule,
  MatProgressBarModule,
  MatDialogModule,
  MatSnackBarModule,
  MatTooltipModule,
  MatMenuModule,
  MatTabsModule,
  MatChipsModule,
  MatBadgeModule,
  MatStepperModule,
  MatExpansionModule,
  MatAutocompleteModule,
  MatSlideToggleModule,
  MatSliderModule,
];

@NgModule({
  imports: materialModules,
  exports: materialModules,
})
export class MaterialModule {}
```

---

## Tailwind CSS Setup

### 1. Configure Tailwind

Edit `tailwind.config.js`:

```javascript
/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#eff6ff',
          100: '#dbeafe',
          200: '#bfdbfe',
          300: '#93c5fd',
          400: '#60a5fa',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8',
          800: '#1e40af',
          900: '#1e3a8a',
        },
        secondary: {
          50: '#f0fdf4',
          100: '#dcfce7',
          200: '#bbf7d0',
          300: '#86efac',
          400: '#4ade80',
          500: '#22c55e',
          600: '#16a34a',
          700: '#15803d',
          800: '#166534',
          900: '#14532d',
        },
        accent: {
          50: '#faf5ff',
          100: '#f3e8ff',
          200: '#e9d5ff',
          300: '#d8b4fe',
          400: '#c084fc',
          500: '#a855f7',
          600: '#9333ea',
          700: '#7e22ce',
          800: '#6b21a8',
          900: '#581c87',
        },
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', '-apple-system', 'sans-serif'],
      },
      spacing: {
        '128': '32rem',
        '144': '36rem',
      },
      borderRadius: {
        '4xl': '2rem',
      },
      minHeight: {
        'screen-90': '90vh',
      },
      animation: {
        'slideDown': 'slideDown 0.3s ease-out',
        'slideUp': 'slideUp 0.3s ease-out',
        'fadeIn': 'fadeIn 0.3s ease-in',
      },
      keyframes: {
        slideDown: {
          '0%': { transform: 'translateY(-10px)', opacity: 0 },
          '100%': { transform: 'translateY(0)', opacity: 1 },
        },
        slideUp: {
          '0%': { transform: 'translateY(10px)', opacity: 0 },
          '100%': { transform: 'translateY(0)', opacity: 1 },
        },
        fadeIn: {
          '0%': { opacity: 0 },
          '100%': { opacity: 1 },
        },
      },
    },
  },
  plugins: [
    require('@tailwindcss/forms'),
    require('@tailwindcss/typography'),
    require('@tailwindcss/aspect-ratio'),
    require('@tailwindcss/line-clamp'),
  ],
}
```

### 2. Install Tailwind Plugins

```bash
npm install -D @tailwindcss/forms @tailwindcss/typography @tailwindcss/aspect-ratio @tailwindcss/line-clamp
```

### 3. Configure Global Styles

Edit `src/styles.scss`:

```scss
/* Import Tailwind CSS */
@import 'tailwindcss/base';
@import 'tailwindcss/components';
@import 'tailwindcss/utilities';

/* Import Material Theme */
@import './styles/material-theme.scss';

/* Custom Global Styles */
* {
  box-sizing: border-box;
}

html, body {
  height: 100%;
  margin: 0;
  padding: 0;
}

body {
  font-family: 'Inter', system-ui, -apple-system, sans-serif;
  @apply text-gray-900 bg-gray-50;
}

/* Custom utility classes */
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Smooth scrolling */
html {
  scroll-behavior: smooth;
}

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

::-webkit-scrollbar-track {
  @apply bg-gray-100;
}

::-webkit-scrollbar-thumb {
  @apply bg-gray-400 rounded;
}

::-webkit-scrollbar-thumb:hover {
  @apply bg-gray-500;
}

/* Focus styles for accessibility */
*:focus-visible {
  @apply outline-2 outline-offset-2 outline-blue-600;
}
```

---

## Chart.js Setup

### 1. Register Chart.js

Create `src/app/shared/chart-config.ts`:

```typescript
import { Chart, registerables } from 'chart.js';

// Register all Chart.js components
Chart.register(...registerables);

// Default configuration
export const defaultChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: true,
      position: 'top' as const,
    },
  },
};

// Color schemes
export const chartColors = {
  primary: '#3b82f6',
  secondary: '#10b981',
  accent: '#8b5cf6',
  warning: '#f59e0b',
  danger: '#ef4444',
  info: '#06b6d4',
};
```

### 2. Import in App Module

```typescript
// app.module.ts
import { NgChartsModule } from 'ng2-charts';
import './shared/chart-config';

@NgModule({
  imports: [
    NgChartsModule,
    // ... other imports
  ]
})
export class AppModule { }
```

---

## Environment Configuration

### 1. Development Environment

Edit `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api',
  apiTimeout: 30000,
  enableDebugLogs: true,
  maxRetries: 3,
  retryDelay: 1000,
  itemsPerPage: 10,
  
  // Feature flags
  features: {
    enableNotifications: true,
    enableFileUpload: true,
    enableRealTimeUpdates: false,
  },
  
  // Third-party services
  googleMapsApiKey: 'YOUR_GOOGLE_MAPS_API_KEY',
  stripePublicKey: 'YOUR_STRIPE_PUBLIC_KEY',
};
```

### 2. Production Environment

Edit `src/environments/environment.prod.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.jobservicemarketplace.com/api',
  apiTimeout: 30000,
  enableDebugLogs: false,
  maxRetries: 3,
  retryDelay: 1000,
  itemsPerPage: 10,
  
  features: {
    enableNotifications: true,
    enableFileUpload: true,
    enableRealTimeUpdates: true,
  },
  
  googleMapsApiKey: 'PROD_GOOGLE_MAPS_API_KEY',
  stripePublicKey: 'PROD_STRIPE_PUBLIC_KEY',
};
```

### 3. Staging Environment (Optional)

Create `src/environments/environment.staging.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://staging-api.jobservicemarketplace.com/api',
  apiTimeout: 30000,
  enableDebugLogs: true,
  maxRetries: 3,
  retryDelay: 1000,
  itemsPerPage: 10,
  
  features: {
    enableNotifications: true,
    enableFileUpload: true,
    enableRealTimeUpdates: true,
  },
  
  googleMapsApiKey: 'STAGING_GOOGLE_MAPS_API_KEY',
  stripePublicKey: 'STAGING_STRIPE_PUBLIC_KEY',
};
```

---

## Module Organization

### 1. App Module Configuration

Edit `src/app/app.module.ts`:

```typescript
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

// Core module
import { CoreModule } from './core/core.module';

// Shared module
import { SharedModule } from './shared/shared.module';

// Interceptors
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { ErrorInterceptor } from './core/interceptors/error.interceptor';
import { LoadingInterceptor } from './core/interceptors/loading.interceptor';

// Chart.js configuration
import './shared/chart-config';

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    AppRoutingModule,
    CoreModule,
    SharedModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

### 2. Core Module Configuration

Edit `src/app/core/core.module.ts`:

```typescript
import { NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

// Services
import { AuthService } from './services/auth.service';
import { ApiService } from './services/api.service';
import { LoadingService } from './services/loading.service';

// Guards
import { AuthGuard } from './guards/auth.guard';
import { RoleGuard } from './guards/role.guard';

// Interceptors
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { ErrorInterceptor } from './interceptors/error.interceptor';
import { LoadingInterceptor } from './interceptors/loading.interceptor';

@NgModule({
  declarations: [],
  imports: [CommonModule],
  providers: [
    AuthService,
    ApiService,
    LoadingService,
    AuthGuard,
    RoleGuard,
  ],
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error('CoreModule is already loaded. Import it in the AppModule only');
    }
  }
}
```

### 3. Shared Module Configuration

Edit `src/app/shared/shared.module.ts`:

```typescript
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Material module
import { MaterialModule } from './material.module';

// Components
import { HeaderComponent } from './components/header/header.component';
import { FooterComponent } from './components/footer/footer.component';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';
import { RatingDisplayComponent } from './components/rating-display/rating-display.component';

// Directives
import { RoleAccessDirective } from './directives/role-access.directive';

// Pipes
import { CurrencyThaiPipe } from './pipes/currency-thai.pipe';
import { DateThaiPipe } from './pipes/date-thai.pipe';

const components = [
  HeaderComponent,
  FooterComponent,
  LoadingSpinnerComponent,
  ConfirmationDialogComponent,
  RatingDisplayComponent,
];

const directives = [RoleAccessDirective];

const pipes = [CurrencyThaiPipe, DateThaiPipe];

@NgModule({
  declarations: [...components, ...directives, ...pipes],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    MaterialModule,
  ],
  exports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    MaterialModule,
    ...components,
    ...directives,
    ...pipes,
  ],
})
export class SharedModule {}
```

---

## Routing Configuration

### Main App Routing

Edit `src/app/app-routing.module.ts`:

```typescript
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';
import { UserRole } from './core/services/auth.service';

const routes: Routes = [
  {
    path: '',
    redirectTo: '/auth/login',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: 'client',
    loadChildren: () => import('./features/client/client.module').then(m => m.ClientModule),
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [UserRole.Client] }
  },
  {
    path: 'provider',
    loadChildren: () => import('./features/provider/provider.module').then(m => m.ProviderModule),
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [UserRole.Provider] }
  },
  {
    path: 'admin',
    loadChildren: () => import('./features/admin/admin.module').then(m => m.AdminModule),
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [UserRole.Admin] }
  },
  {
    path: '**',
    redirectTo: '/auth/login'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    enableTracing: false, // Set to true for debugging
    scrollPositionRestoration: 'top',
    anchorScrolling: 'enabled',
    onSameUrlNavigation: 'reload'
  })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
```

---

## Build Configuration

### 1. Update angular.json

Key configurations in `angular.json`:

```json
{
  "projects": {
    "frontend": {
      "architect": {
        "build": {
          "configurations": {
            "production": {
              "optimization": true,
              "outputHashing": "all",
              "sourceMap": false,
              "namedChunks": false,
              "aot": true,
              "extractLicenses": true,
              "vendorChunk": false,
              "buildOptimizer": true,
              "budgets": [
                {
                  "type": "initial",
                  "maximumWarning": "2mb",
                  "maximumError": "5mb"
                },
                {
                  "type": "anyComponentStyle",
                  "maximumWarning": "6kb",
                  "maximumError": "10kb"
                }
              ]
            },
            "development": {
              "optimization": false,
              "sourceMap": true,
              "namedChunks": true
            }
          }
        }
      }
    }
  }
}
```

### 2. Update package.json Scripts

```json
{
  "scripts": {
    "ng": "ng",
    "start": "ng serve",
    "start:prod": "ng serve --configuration production",
    "build": "ng build",
    "build:prod": "ng build --configuration production",
    "build:staging": "ng build --configuration staging",
    "watch": "ng build --watch --configuration development",
    "test": "ng test",
    "test:ci": "ng test --watch=false --code-coverage",
    "lint": "ng lint",
    "e2e": "ng e2e",
    "analyze": "ng build --stats-json && webpack-bundle-analyzer dist/frontend/stats.json"
  }
}
```

### 3. TypeScript Configuration

Edit `tsconfig.json`:

```json
{
  "compileOnSave": false,
  "compilerOptions": {
    "baseUrl": "./",
    "outDir": "./dist/out-tsc",
    "sourceMap": true,
    "declaration": false,
    "downlevelIteration": true,
    "experimentalDecorators": true,
    "moduleResolution": "node",
    "importHelpers": true,
    "target": "ES2022",
    "module": "ES2022",
    "useDefineForClassFields": false,
    "lib": [
      "ES2022",
      "dom"
    ],
    "paths": {
      "@app/*": ["src/app/*"],
      "@env/*": ["src/environments/*"],
      "@core/*": ["src/app/core/*"],
      "@shared/*": ["src/app/shared/*"],
      "@features/*": ["src/app/features/*"]
    },
    "strict": true,
    "strictNullChecks": true,
    "noImplicitAny": true,
    "skipLibCheck": true
  },
  "angularCompilerOptions": {
    "enableI18nLegacyMessageIdFormat": false,
    "strictInjectionParameters": true,
    "strictInputAccessModifiers": true,
    "strictTemplates": true
  }
}
```

---

## Running the Application

### Development Server

```bash
# Start development server
npm start

# Or with specific host/port
ng serve --host 0.0.0.0 --port 4200

# With production configuration
npm run start:prod
```

### Production Build

```bash
# Build for production
npm run build:prod

# Output will be in dist/ directory

# Serve production build locally (install http-server first)
npm install -g http-server
http-server dist/frontend -p 8080
```

### Bundle Analysis

```bash
# Install webpack-bundle-analyzer
npm install -D webpack-bundle-analyzer

# Run analysis
npm run analyze
```

---

## Verification Checklist

After setup, verify:

### ✅ Dependencies
- [ ] All npm packages installed successfully
- [ ] No security vulnerabilities (`npm audit`)
- [ ] Angular CLI version matches project

### ✅ Configuration
- [ ] Tailwind CSS working (test utility classes)
- [ ] Angular Material theme applied
- [ ] Environment files configured
- [ ] Routing configured correctly

### ✅ Build & Run
- [ ] Development server starts without errors
- [ ] Production build completes successfully
- [ ] No console errors in browser
- [ ] Hot reload working

### ✅ Features
- [ ] HTTP requests working
- [ ] Guards protecting routes
- [ ] Interceptors functioning
- [ ] Material components rendering
- [ ] Responsive layout working

---

## Troubleshooting

### Common Issues

**Issue: Tailwind classes not working**
```bash
# Rebuild with cache clear
rm -rf .angular/cache
npm start
```

**Issue: Material styles not loading**
```bash
# Ensure material theme is imported in styles.scss
# Check angular.json includes styles.scss
```

**Issue: Module not found errors**
```bash
# Clear node_modules and reinstall
rm -rf node_modules package-lock.json
npm install
```

**Issue: Build fails with memory error**
```bash
# Increase Node memory
export NODE_OPTIONS="--max-old-space-size=4096"
npm run build:prod
```

---

**Setup Complete! Ready to develop! 🚀**
