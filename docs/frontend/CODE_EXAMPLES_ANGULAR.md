# Frontend Code Examples

This document provides detailed code examples for the Job Service Marketplace Angular frontend.

## Table of Contents
1. [Component Examples](#component-examples)
2. [Service Examples](#service-examples)
3. [Form Examples](#form-examples)
4. [API Integration](#api-integration)
5. [Chart Examples](#chart-examples)
6. [Responsive Design](#responsive-design)

---

## Component Examples

### 1. Login Component (Complete Example)

**TypeScript (login.ts):**
```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../../core/services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  loading = false;
  submitted = false;
  error = '';

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  get f() {
    return this.loginForm.controls;
  }

  onSubmit(): void {
    this.submitted = true;
    this.error = '';

    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;

    this.authService.login(this.loginForm.value).subscribe({
      next: (response) => {
        const role = response.user.role;
        if (role === 'Provider') {
          this.router.navigate(['/provider/dashboard']);
        } else if (role === 'Client') {
          this.router.navigate(['/client/marketplace']);
        } else if (role === 'Admin') {
          this.router.navigate(['/admin/dashboard']);
        }
      },
      error: (error) => {
        this.error = error.message || 'Login failed';
        this.loading = false;
      }
    });
  }
}
```

**HTML Template (login.html):**
```html
<div class="min-h-screen flex items-center justify-center bg-gray-100 py-12 px-4">
  <div class="max-w-md w-full">
    <h2 class="text-center text-3xl font-bold text-gray-900 mb-8">
      Sign in to your account
    </h2>

    <mat-card class="p-8">
      <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
        <div class="space-y-6">
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Email Address</mat-label>
            <input matInput type="email" formControlName="email">
            <mat-error *ngIf="f['email'].errors?.['required']">
              Email is required
            </mat-error>
            <mat-error *ngIf="f['email'].errors?.['email']">
              Please enter a valid email
            </mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Password</mat-label>
            <input matInput type="password" formControlName="password">
            <mat-error *ngIf="f['password'].errors?.['required']">
              Password is required
            </mat-error>
          </mat-form-field>

          <div *ngIf="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
            {{ error }}
          </div>

          <button mat-raised-button color="primary" type="submit" class="w-full" [disabled]="loading">
            <span *ngIf="!loading">Sign In</span>
            <mat-spinner *ngIf="loading" diameter="20"></mat-spinner>
          </button>
        </div>
      </form>
    </mat-card>
  </div>
</div>
```

### 2. Provider Dashboard with Charts

**TypeScript (dashboard.ts):**
```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';
import { ProviderService, IncomeSummary } from '../../services/provider';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, BaseChartDirective],
  templateUrl: './dashboard.html'
})
export class DashboardComponent implements OnInit {
  incomeSummary?: IncomeSummary;
  
  public barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: ['Total Income', 'Commission', 'Tax', 'Net Income'],
    datasets: [{
      data: [0, 0, 0, 0],
      label: 'Amount (฿)',
      backgroundColor: [
        'rgba(37, 99, 235, 0.6)',
        'rgba(239, 68, 68, 0.6)',
        'rgba(245, 158, 11, 0.6)',
        'rgba(16, 185, 129, 0.6)'
      ]
    }]
  };

  public barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false
  };

  constructor(private providerService: ProviderService) {}

  ngOnInit(): void {
    this.loadIncomeSummary();
  }

  loadIncomeSummary(): void {
    const currentYear = new Date().getFullYear();
    this.providerService.getIncomeSummary(1, currentYear).subscribe({
      next: (data) => {
        this.incomeSummary = data;
        this.barChartData.datasets[0].data = [
          data.totalIncome,
          data.platformCommission,
          data.withholdingTax,
          data.netIncome
        ];
      }
    });
  }
}
```

### 3. Marketplace Component with Filters

**TypeScript (marketplace.ts):**
```typescript
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ProviderService, ProviderProfile } from '../../../provider/services/provider';

@Component({
  selector: 'app-marketplace',
  standalone: true,
  templateUrl: './marketplace.html'
})
export class MarketplaceComponent implements OnInit {
  searchForm!: FormGroup;
  providers: ProviderProfile[] = [];
  loading = true;
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;

  professions = ['Nurse', 'Electrician', 'Plumber', 'Carpenter'];
  locations = ['Bangkok', 'Chiang Mai', 'Phuket'];

  constructor(
    private formBuilder: FormBuilder,
    private providerService: ProviderService
  ) {}

  ngOnInit(): void {
    this.searchForm = this.formBuilder.group({
      profession: [''],
      location: [''],
      minRating: ['']
    });
    this.loadProviders();
  }

  loadProviders(): void {
    this.loading = true;
    const params = {
      ...this.searchForm.value,
      page: this.pageIndex + 1,
      pageSize: this.pageSize
    };

    this.providerService.getProviders(params).subscribe({
      next: (result) => {
        this.providers = result.data;
        this.totalCount = result.totalCount;
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    this.pageIndex = 0;
    this.loadProviders();
  }
}
```

---

## Service Examples

### 1. Auth Service (Complete Implementation)

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

export interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  role: 'Client' | 'Provider' | 'Admin';
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    const storedUser = localStorage.getItem('currentUser');
    if (storedUser) {
      this.currentUserSubject.next(JSON.parse(storedUser));
    }
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  public get isAuthenticated(): boolean {
    return !!this.currentUserValue && !!this.getToken();
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, credentials)
      .pipe(tap(response => this.setSession(response)));
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  private setSession(authResponse: AuthResponse): void {
    localStorage.setItem('token', authResponse.token);
    localStorage.setItem('currentUser', JSON.stringify(authResponse.user));
    this.currentUserSubject.next(authResponse.user);
  }
}
```

### 2. API Service (Generic HTTP Methods)

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface PaginatedResult<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  get<T>(endpoint: string, params?: any): Observable<T> {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach(key => {
        if (params[key] !== null && params[key] !== undefined) {
          httpParams = httpParams.append(key, params[key].toString());
        }
      });
    }
    return this.http.get<T>(`${this.apiUrl}${endpoint}`, { params: httpParams });
  }

  post<T>(endpoint: string, data: any): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}${endpoint}`, data);
  }

  put<T>(endpoint: string, data: any): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}${endpoint}`, data);
  }

  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.apiUrl}${endpoint}`);
  }
}
```

### 3. Provider Service

```typescript
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api';

export interface ProviderProfile {
  id: number;
  profession: string;
  bio: string;
  hourlyRate: number;
  location: string;
  averageRating: number;
  totalReviews: number;
}

@Injectable({
  providedIn: 'root'
})
export class ProviderService {
  constructor(private apiService: ApiService) {}

  getProviders(params?: any): Observable<any> {
    return this.apiService.get('/providers', params);
  }

  getProfile(id: number): Observable<ProviderProfile> {
    return this.apiService.get<ProviderProfile>(`/providers/${id}`);
  }

  updateProfile(id: number, data: Partial<ProviderProfile>): Observable<ProviderProfile> {
    return this.apiService.put<ProviderProfile>(`/providers/${id}`, data);
  }
}
```

---

## Form Examples

### 1. Reactive Form with Validation

```typescript
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-booking-form',
  template: `
    <form [formGroup]="bookingForm" (ngSubmit)="onSubmit()">
      <mat-form-field>
        <mat-label>Job Title</mat-label>
        <input matInput formControlName="jobTitle">
        <mat-error *ngIf="f['jobTitle'].errors?.['required']">
          Job title is required
        </mat-error>
      </mat-form-field>

      <mat-form-field>
        <mat-label>Start Date</mat-label>
        <input matInput type="date" formControlName="startDate">
        <mat-error *ngIf="f['startDate'].errors?.['required']">
          Start date is required
        </mat-error>
      </mat-form-field>

      <mat-form-field>
        <mat-label>Estimated Hours</mat-label>
        <input matInput type="number" formControlName="estimatedHours">
        <mat-error *ngIf="f['estimatedHours'].errors?.['min']">
          Must be at least 1 hour
        </mat-error>
      </mat-form-field>

      <button mat-raised-button color="primary" type="submit">
        Create Booking
      </button>
    </form>
  `
})
export class BookingFormComponent implements OnInit {
  bookingForm!: FormGroup;

  constructor(private formBuilder: FormBuilder) {}

  ngOnInit(): void {
    this.bookingForm = this.formBuilder.group({
      jobTitle: ['', [Validators.required, Validators.minLength(5)]],
      jobDescription: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      estimatedHours: [1, [Validators.required, Validators.min(1)]]
    });
  }

  get f() {
    return this.bookingForm.controls;
  }

  onSubmit(): void {
    if (this.bookingForm.valid) {
      console.log(this.bookingForm.value);
    }
  }
}
```

### 2. Custom Validator (Password Match)

```typescript
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function passwordMatchValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    return password.value === confirmPassword.value ? null : { passwordMismatch: true };
  };
}

// Usage in component:
this.registerForm = this.formBuilder.group({
  email: ['', [Validators.required, Validators.email]],
  password: ['', [Validators.required, Validators.minLength(6)]],
  confirmPassword: ['', Validators.required]
}, { validators: passwordMatchValidator() });
```

---

## API Integration

### 1. HTTP Interceptor (Auth Token)

```typescript
import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../services/auth';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req);
};
```

### 2. Error Interceptor

```typescript
import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        router.navigate(['/auth/login']);
      }
      return throwError(() => new Error(error.message));
    })
  );
};
```

### 3. Guard Implementation

```typescript
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated) {
    return true;
  }

  router.navigate(['/auth/login'], { 
    queryParams: { returnUrl: state.url } 
  });
  return false;
};

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const allowedRoles = route.data['roles'] as string[];
  const userRole = authService.userRole;

  if (allowedRoles && userRole && allowedRoles.includes(userRole)) {
    return true;
  }

  router.navigate(['/']);
  return false;
};
```

---

## Chart Examples

### 1. Bar Chart (Income Breakdown)

```typescript
import { ChartConfiguration } from 'chart.js';

public barChartData: ChartConfiguration<'bar'>['data'] = {
  labels: ['Total Income', 'Commission', 'Tax', 'Net Income'],
  datasets: [{
    data: [50000, 5000, 1500, 43500],
    label: 'Amount (฿)',
    backgroundColor: [
      'rgba(37, 99, 235, 0.6)',
      'rgba(239, 68, 68, 0.6)',
      'rgba(245, 158, 11, 0.6)',
      'rgba(16, 185, 129, 0.6)'
    ],
    borderColor: [
      'rgb(37, 99, 235)',
      'rgb(239, 68, 68)',
      'rgb(245, 158, 11)',
      'rgb(16, 185, 129)'
    ],
    borderWidth: 1
  }]
};

public barChartOptions: ChartConfiguration<'bar'>['options'] = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false
    },
    title: {
      display: true,
      text: 'Income Breakdown'
    }
  }
};
```

**HTML:**
```html
<div class="h-80">
  <canvas baseChart
    [data]="barChartData"
    [options]="barChartOptions"
    type="bar">
  </canvas>
</div>
```

### 2. Line Chart (Monthly Bookings)

```typescript
public lineChartData: ChartConfiguration<'line'>['data'] = {
  labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
  datasets: [{
    data: [10, 15, 13, 20, 25, 30],
    label: 'Bookings',
    borderColor: 'rgb(37, 99, 235)',
    backgroundColor: 'rgba(37, 99, 235, 0.1)',
    tension: 0.4
  }]
};

public lineChartOptions: ChartConfiguration<'line'>['options'] = {
  responsive: true,
  plugins: {
    title: {
      display: true,
      text: 'Monthly Bookings Trend'
    }
  }
};
```

---

## Responsive Design

### 1. Responsive Grid Layout

```html
<!-- Mobile: 1 column, Tablet: 2 columns, Desktop: 3 columns -->
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
  <div class="card-custom">Card 1</div>
  <div class="card-custom">Card 2</div>
  <div class="card-custom">Card 3</div>
</div>
```

### 2. Responsive Typography

```html
<!-- Small on mobile, larger on desktop -->
<h1 class="text-2xl md:text-3xl lg:text-4xl font-bold">
  Responsive Heading
</h1>

<p class="text-sm md:text-base lg:text-lg">
  Responsive paragraph text
</p>
```

### 3. Mobile Navigation

```html
<nav class="flex flex-col md:flex-row gap-4">
  <a routerLink="/home" class="px-4 py-2">Home</a>
  <a routerLink="/about" class="px-4 py-2">About</a>
  <a routerLink="/contact" class="px-4 py-2">Contact</a>
</nav>
```

### 4. Responsive Images

```html
<!-- Full width on mobile, max-width on larger screens -->
<img src="..." class="w-full md:max-w-md lg:max-w-lg" alt="...">
```

### 5. Conditional Display

```html
<!-- Hidden on mobile, visible on tablet and up -->
<div class="hidden md:block">
  Desktop-only content
</div>

<!-- Visible on mobile, hidden on tablet and up -->
<div class="block md:hidden">
  Mobile-only content
</div>
```

---

## Tailwind Utility Classes

### Custom Classes Defined in styles.scss

```scss
/* Container */
.container-custom {
  @apply max-w-7xl mx-auto px-4 sm:px-6 lg:px-8;
}

/* Card */
.card-custom {
  @apply bg-white rounded-lg shadow-md p-6;
}

/* Buttons */
.btn-primary {
  @apply bg-primary text-white px-4 py-2 rounded-md hover:bg-blue-700 transition-colors;
}

.btn-secondary {
  @apply bg-secondary text-white px-4 py-2 rounded-md hover:bg-green-700 transition-colors;
}
```

---

## Environment Configuration

### Development Environment

```typescript
// src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api',
  apiTimeout: 30000,
  maxRetries: 3,
  enableDebugMode: true,
};
```

### Production Environment

```typescript
// src/environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://api.jobservicemarketplace.com/api',
  apiTimeout: 30000,
  maxRetries: 3,
  enableDebugMode: false,
};
```

---

## Conclusion

These code examples demonstrate the key patterns and practices used throughout the Job Service Marketplace frontend application. All components follow Angular best practices, use TypeScript strict typing, and implement responsive design with Tailwind CSS.
