# Frontend Code Examples

This document provides detailed code examples for implementing the Job Service Marketplace frontend with Angular 20, Tailwind CSS, and Material UI.

---

## Table of Contents
1. [Core Services](#core-services)
2. [Authentication Components](#authentication-components)
3. [Provider Components](#provider-components)
4. [Client Components](#client-components)
5. [Shared Components](#shared-components)
6. [Routing Configuration](#routing-configuration)
7. [API Integration](#api-integration)
8. [Forms & Validation](#forms--validation)
9. [Dashboard & Charts](#dashboard--charts)
10. [Responsive Layouts](#responsive-layouts)

---

## Core Services

### Auth Service
**File:** `src/app/core/services/auth.service.ts`

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
  role: UserRole;
  token?: string;
}

export enum UserRole {
  Client = 1,
  Provider = 2,
  Admin = 3
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  role: UserRole;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    // Load user from localStorage on init
    const storedUser = localStorage.getItem('currentUser');
    if (storedUser) {
      this.currentUserSubject.next(JSON.parse(storedUser));
    }
  }

  login(credentials: LoginRequest): Observable<User> {
    return this.http.post<User>(`${environment.apiUrl}/auth/login`, credentials)
      .pipe(
        tap(user => {
          if (user && user.token) {
            localStorage.setItem('currentUser', JSON.stringify(user));
            localStorage.setItem('token', user.token);
            this.currentUserSubject.next(user);
          }
        })
      );
  }

  register(data: RegisterRequest): Observable<User> {
    return this.http.post<User>(`${environment.apiUrl}/auth/register`, data)
      .pipe(
        tap(user => {
          if (user && user.token) {
            localStorage.setItem('currentUser', JSON.stringify(user));
            localStorage.setItem('token', user.token);
            this.currentUserSubject.next(user);
          }
        })
      );
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return !!this.currentUserSubject.value && !!localStorage.getItem('token');
  }

  hasRole(role: UserRole): boolean {
    const user = this.getCurrentUser();
    return user ? user.role === role : false;
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }
}
```

### Token Interceptor
**File:** `src/app/core/interceptors/auth.interceptor.ts`

```typescript
import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = this.authService.getToken();
    
    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    return next.handle(request);
  }
}
```

### Error Interceptor
**File:** `src/app/core/interceptors/error.interceptor.ts`

```typescript
import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private snackBar: MatSnackBar,
    private router: Router
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An error occurred';

        if (error.error instanceof ErrorEvent) {
          // Client-side error
          errorMessage = `Error: ${error.error.message}`;
        } else {
          // Server-side error
          switch (error.status) {
            case 401:
              errorMessage = 'Unauthorized - Please login again';
              this.router.navigate(['/auth/login']);
              break;
            case 403:
              errorMessage = 'Access forbidden';
              this.router.navigate(['/unauthorized']);
              break;
            case 404:
              errorMessage = 'Resource not found';
              break;
            case 500:
              errorMessage = 'Internal server error';
              break;
            default:
              errorMessage = error.error?.message || `Error: ${error.status}`;
          }
        }

        this.snackBar.open(errorMessage, 'Close', {
          duration: 5000,
          horizontalPosition: 'center',
          verticalPosition: 'top',
          panelClass: ['error-snackbar']
        });

        return throwError(() => error);
      })
    );
  }
}
```

---

## Authentication Components

### Login Component
**File:** `src/app/features/auth/components/login/login.component.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService, UserRole } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  isLoading = false;
  hidePassword = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.authService.login(this.loginForm.value).subscribe({
        next: (user) => {
          this.isLoading = false;
          this.snackBar.open('Login successful!', 'Close', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });

          // Navigate based on role
          switch (user.role) {
            case UserRole.Client:
              this.router.navigate(['/client/marketplace']);
              break;
            case UserRole.Provider:
              this.router.navigate(['/provider/dashboard']);
              break;
            case UserRole.Admin:
              this.router.navigate(['/admin/dashboard']);
              break;
          }
        },
        error: (error) => {
          this.isLoading = false;
          // Error handled by interceptor
        }
      });
    }
  }

  getErrorMessage(fieldName: string): string {
    const field = this.loginForm.get(fieldName);
    if (field?.hasError('required')) {
      return `${fieldName} is required`;
    }
    if (field?.hasError('email')) {
      return 'Please enter a valid email';
    }
    if (field?.hasError('minlength')) {
      return `${fieldName} must be at least ${field.errors?.['minlength'].requiredLength} characters`;
    }
    return '';
  }
}
```

**File:** `src/app/features/auth/components/login/login.component.html`

```html
<div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
  <div class="max-w-md w-full">
    <mat-card class="shadow-lg">
      <mat-card-header class="mb-6">
        <mat-card-title class="text-center w-full">
          <h2 class="text-3xl font-bold text-gray-900">Sign in to your account</h2>
        </mat-card-title>
      </mat-card-header>

      <mat-card-content>
        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="space-y-6">
          <!-- Email Field -->
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Email</mat-label>
            <input 
              matInput 
              type="email" 
              formControlName="email"
              placeholder="your@email.com"
              autocomplete="email">
            <mat-icon matPrefix>email</mat-icon>
            <mat-error *ngIf="loginForm.get('email')?.invalid">
              {{ getErrorMessage('email') }}
            </mat-error>
          </mat-form-field>

          <!-- Password Field -->
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Password</mat-label>
            <input 
              matInput 
              [type]="hidePassword ? 'password' : 'text'" 
              formControlName="password"
              placeholder="Enter your password"
              autocomplete="current-password">
            <mat-icon matPrefix>lock</mat-icon>
            <button 
              mat-icon-button 
              matSuffix 
              type="button"
              (click)="hidePassword = !hidePassword"
              [attr.aria-label]="'Hide password'" 
              [attr.aria-pressed]="hidePassword">
              <mat-icon>{{hidePassword ? 'visibility_off' : 'visibility'}}</mat-icon>
            </button>
            <mat-error *ngIf="loginForm.get('password')?.invalid">
              {{ getErrorMessage('password') }}
            </mat-error>
          </mat-form-field>

          <!-- Remember Me & Forgot Password -->
          <div class="flex items-center justify-between">
            <mat-checkbox class="text-sm">Remember me</mat-checkbox>
            <a routerLink="/auth/forgot-password" class="text-sm text-blue-600 hover:text-blue-500">
              Forgot password?
            </a>
          </div>

          <!-- Submit Button -->
          <button 
            mat-raised-button 
            color="primary" 
            type="submit"
            class="w-full"
            [disabled]="isLoading || loginForm.invalid">
            <mat-spinner *ngIf="isLoading" diameter="20" class="inline-block mr-2"></mat-spinner>
            {{ isLoading ? 'Signing in...' : 'Sign in' }}
          </button>

          <!-- Register Link -->
          <div class="text-center text-sm">
            <span class="text-gray-600">Don't have an account?</span>
            <a routerLink="/auth/register" class="ml-2 text-blue-600 hover:text-blue-500 font-medium">
              Register now
            </a>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  </div>
</div>
```

**File:** `src/app/features/auth/components/login/login.component.scss`

```scss
.success-snackbar {
  background-color: #10B981;
  color: white;
}

.error-snackbar {
  background-color: #EF4444;
  color: white;
}

mat-card {
  padding: 2rem;
}

mat-form-field {
  margin-bottom: 1rem;
}
```

### Register Component
**File:** `src/app/features/auth/components/register/register.component.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService, UserRole } from '../../../../core/services/auth.service';
import { CustomValidators } from '../../../../shared/validators/custom-validators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  isLoading = false;
  hidePassword = true;
  hideConfirmPassword = true;
  roles = [
    { value: UserRole.Client, label: 'Client (Looking for services)' },
    { value: UserRole.Provider, label: 'Provider (Offering services)' }
  ];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required,
        Validators.minLength(8),
        CustomValidators.passwordStrength
      ]],
      confirmPassword: ['', Validators.required],
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      phoneNumber: ['', [
        Validators.required,
        CustomValidators.thaiPhoneNumber
      ]],
      role: [UserRole.Client, Validators.required],
      acceptTerms: [false, Validators.requiredTrue]
    }, {
      validators: CustomValidators.passwordMatch('password', 'confirmPassword')
    });
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.isLoading = true;
      const { confirmPassword, acceptTerms, ...registerData } = this.registerForm.value;
      
      this.authService.register(registerData).subscribe({
        next: (user) => {
          this.isLoading = false;
          this.snackBar.open('Registration successful!', 'Close', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          
          // Navigate based on role
          if (user.role === UserRole.Provider) {
            this.router.navigate(['/provider/profile']);
          } else {
            this.router.navigate(['/client/marketplace']);
          }
        },
        error: (error) => {
          this.isLoading = false;
        }
      });
    }
  }

  getErrorMessage(fieldName: string): string {
    const field = this.registerForm.get(fieldName);
    if (field?.hasError('required')) {
      return `${fieldName} is required`;
    }
    if (field?.hasError('email')) {
      return 'Please enter a valid email';
    }
    if (field?.hasError('minlength')) {
      const requiredLength = field.errors?.['minlength'].requiredLength;
      return `${fieldName} must be at least ${requiredLength} characters`;
    }
    if (field?.hasError('passwordStrength')) {
      return 'Password must contain uppercase, lowercase, number, and special character';
    }
    if (field?.hasError('thaiPhoneNumber')) {
      return 'Please enter a valid Thai phone number';
    }
    if (field?.hasError('passwordMismatch')) {
      return 'Passwords do not match';
    }
    return '';
  }
}
```

---

## Provider Components

### Provider Profile Component
**File:** `src/app/features/provider/components/profile/profile-edit/profile-edit.component.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProviderService } from '../../../services/provider.service';

export interface ProviderProfile {
  id?: number;
  userId: number;
  profession: string;
  bio: string;
  hourlyRate: number;
  experienceYears: number;
  certifications?: string;
  isVerified: boolean;
  averageRating?: number;
  totalReviews?: number;
}

@Component({
  selector: 'app-profile-edit',
  templateUrl: './profile-edit.component.html',
  styleUrls: ['./profile-edit.component.scss']
})
export class ProfileEditComponent implements OnInit {
  profileForm!: FormGroup;
  isLoading = false;
  currentProfile?: ProviderProfile;
  
  professions = [
    'Nurse', 'Home Care Assistant', 'Electrician', 'Plumber',
    'Gardener', 'Cleaner', 'Tutor', 'Chef', 'Driver', 'Other'
  ];

  constructor(
    private fb: FormBuilder,
    private providerService: ProviderService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadProfile();
  }

  initForm(): void {
    this.profileForm = this.fb.group({
      profession: ['', Validators.required],
      bio: ['', [Validators.required, Validators.minLength(50), Validators.maxLength(500)]],
      hourlyRate: [0, [Validators.required, Validators.min(100), Validators.max(10000)]],
      experienceYears: [0, [Validators.required, Validators.min(0), Validators.max(50)]],
      certifications: ['']
    });
  }

  loadProfile(): void {
    this.isLoading = true;
    this.providerService.getMyProfile().subscribe({
      next: (profile) => {
        this.currentProfile = profile;
        this.profileForm.patchValue(profile);
        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
        // Profile might not exist yet
      }
    });
  }

  onSubmit(): void {
    if (this.profileForm.valid) {
      this.isLoading = true;
      const profileData = this.profileForm.value;

      const request = this.currentProfile?.id
        ? this.providerService.updateProfile(this.currentProfile.id, profileData)
        : this.providerService.createProfile(profileData);

      request.subscribe({
        next: (profile) => {
          this.isLoading = false;
          this.currentProfile = profile;
          this.snackBar.open('Profile saved successfully!', 'Close', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
        },
        error: (error) => {
          this.isLoading = false;
        }
      });
    }
  }
}
```

**File:** `src/app/features/provider/components/profile/profile-edit/profile-edit.component.html`

```html
<div class="container mx-auto py-8 px-4 max-w-4xl">
  <mat-card>
    <mat-card-header>
      <mat-card-title>
        <h2 class="text-2xl font-bold text-gray-900">
          {{ currentProfile?.id ? 'Edit' : 'Create' }} Provider Profile
        </h2>
      </mat-card-title>
    </mat-card-header>

    <mat-card-content>
      <form [formGroup]="profileForm" (ngSubmit)="onSubmit()" class="space-y-6">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Profession -->
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Profession</mat-label>
            <mat-select formControlName="profession">
              <mat-option *ngFor="let profession of professions" [value]="profession">
                {{ profession }}
              </mat-option>
            </mat-select>
            <mat-error *ngIf="profileForm.get('profession')?.hasError('required')">
              Profession is required
            </mat-error>
          </mat-form-field>

          <!-- Hourly Rate -->
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Hourly Rate (฿)</mat-label>
            <input matInput type="number" formControlName="hourlyRate" placeholder="500">
            <span matPrefix>฿&nbsp;</span>
            <mat-error *ngIf="profileForm.get('hourlyRate')?.hasError('required')">
              Hourly rate is required
            </mat-error>
            <mat-error *ngIf="profileForm.get('hourlyRate')?.hasError('min')">
              Minimum rate is ฿100
            </mat-error>
          </mat-form-field>

          <!-- Experience Years -->
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Years of Experience</mat-label>
            <input matInput type="number" formControlName="experienceYears" placeholder="5">
            <mat-error *ngIf="profileForm.get('experienceYears')?.hasError('required')">
              Experience is required
            </mat-error>
          </mat-form-field>

          <!-- Certifications -->
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Certifications (Optional)</mat-label>
            <input matInput formControlName="certifications" 
                   placeholder="e.g., Licensed Nurse, CPR Certified">
          </mat-form-field>
        </div>

        <!-- Bio -->
        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Bio</mat-label>
          <textarea 
            matInput 
            formControlName="bio" 
            rows="5"
            placeholder="Tell clients about yourself, your experience, and what makes you unique..."></textarea>
          <mat-hint align="end">
            {{ profileForm.get('bio')?.value?.length || 0 }}/500 characters
          </mat-hint>
          <mat-error *ngIf="profileForm.get('bio')?.hasError('required')">
            Bio is required
          </mat-error>
          <mat-error *ngIf="profileForm.get('bio')?.hasError('minlength')">
            Bio must be at least 50 characters
          </mat-error>
        </mat-form-field>

        <!-- Current Stats (if exists) -->
        <div *ngIf="currentProfile?.id" class="grid grid-cols-1 sm:grid-cols-3 gap-4 p-4 bg-gray-50 rounded-lg">
          <div class="text-center">
            <p class="text-sm text-gray-600">Average Rating</p>
            <p class="text-2xl font-bold text-yellow-600">
              {{ currentProfile.averageRating?.toFixed(1) || 'N/A' }}
              <mat-icon class="inline-block align-middle">star</mat-icon>
            </p>
          </div>
          <div class="text-center">
            <p class="text-sm text-gray-600">Total Reviews</p>
            <p class="text-2xl font-bold text-blue-600">{{ currentProfile.totalReviews || 0 }}</p>
          </div>
          <div class="text-center">
            <p class="text-sm text-gray-600">Verification Status</p>
            <mat-chip [color]="currentProfile.isVerified ? 'primary' : 'warn'" selected>
              {{ currentProfile.isVerified ? 'Verified' : 'Pending' }}
            </mat-chip>
          </div>
        </div>

        <!-- Actions -->
        <div class="flex gap-4 justify-end">
          <button mat-button type="button" routerLink="/provider/dashboard">
            Cancel
          </button>
          <button 
            mat-raised-button 
            color="primary" 
            type="submit"
            [disabled]="isLoading || profileForm.invalid">
            <mat-spinner *ngIf="isLoading" diameter="20" class="inline-block mr-2"></mat-spinner>
            {{ isLoading ? 'Saving...' : 'Save Profile' }}
          </button>
        </div>
      </form>
    </mat-card-content>
  </mat-card>
</div>
```

---

## Client Components

### Search Providers Component
**File:** `src/app/features/client/components/marketplace/search-providers/search-providers.component.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProviderService } from '../../../../provider/services/provider.service';

export interface ProviderCard {
  id: number;
  profession: string;
  bio: string;
  hourlyRate: number;
  experienceYears: number;
  averageRating: number;
  totalReviews: number;
  isVerified: boolean;
  user: {
    firstName: string;
    lastName: string;
  };
}

@Component({
  selector: 'app-search-providers',
  templateUrl: './search-providers.component.html',
  styleUrls: ['./search-providers.component.scss']
})
export class SearchProvidersComponent implements OnInit {
  searchForm!: FormGroup;
  providers: ProviderCard[] = [];
  isLoading = false;
  totalCount = 0;
  page = 1;
  pageSize = 12;

  professions = [
    'All', 'Nurse', 'Home Care Assistant', 'Electrician', 'Plumber',
    'Gardener', 'Cleaner', 'Tutor', 'Chef', 'Driver', 'Other'
  ];

  sortOptions = [
    { value: 'rating', label: 'Highest Rated' },
    { value: 'reviews', label: 'Most Reviews' },
    { value: 'rate-low', label: 'Price: Low to High' },
    { value: 'rate-high', label: 'Price: High to Low' },
    { value: 'experience', label: 'Most Experienced' }
  ];

  constructor(
    private fb: FormBuilder,
    private providerService: ProviderService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.setupSearchListener();
    this.searchProviders();
  }

  initForm(): void {
    this.searchForm = this.fb.group({
      keyword: [''],
      profession: ['All'],
      minRate: [null],
      maxRate: [null],
      minRating: [null],
      verifiedOnly: [false],
      sortBy: ['rating']
    });
  }

  setupSearchListener(): void {
    this.searchForm.valueChanges
      .pipe(
        debounceTime(500),
        distinctUntilChanged()
      )
      .subscribe(() => {
        this.page = 1;
        this.searchProviders();
      });
  }

  searchProviders(): void {
    this.isLoading = true;
    const filters = this.searchForm.value;

    this.providerService.searchProviders({
      ...filters,
      page: this.page,
      pageSize: this.pageSize
    }).subscribe({
      next: (response) => {
        this.providers = response.data;
        this.totalCount = response.totalCount;
        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
      }
    });
  }

  onPageChange(event: any): void {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.searchProviders();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  viewProviderDetails(providerId: number): void {
    // Navigate to provider details
  }
}
```


**File:** `src/app/features/client/components/marketplace/search-providers/search-providers.component.html`

```html
<div class="min-h-screen bg-gray-50">
  <!-- Search Header -->
  <div class="bg-white shadow-sm border-b">
    <div class="container mx-auto py-6 px-4">
      <h1 class="text-3xl font-bold text-gray-900 mb-6">Find Service Providers</h1>
      
      <form [formGroup]="searchForm" class="space-y-4">
        <!-- Search Bar -->
        <div class="flex flex-col md:flex-row gap-4">
          <mat-form-field appearance="outline" class="flex-1">
            <mat-label>Search</mat-label>
            <input matInput formControlName="keyword" placeholder="Search by name or skill...">
            <mat-icon matPrefix>search</mat-icon>
          </mat-form-field>

          <mat-form-field appearance="outline" class="md:w-48">
            <mat-label>Profession</mat-label>
            <mat-select formControlName="profession">
              <mat-option *ngFor="let prof of professions" [value]="prof">
                {{ prof }}
              </mat-option>
            </mat-select>
          </mat-form-field>

          <mat-form-field appearance="outline" class="md:w-48">
            <mat-label>Sort By</mat-label>
            <mat-select formControlName="sortBy">
              <mat-option *ngFor="let option of sortOptions" [value]="option.value">
                {{ option.label }}
              </mat-option>
            </mat-select>
          </mat-form-field>
        </div>

        <!-- Advanced Filters -->
        <mat-expansion-panel>
          <mat-expansion-panel-header>
            <mat-panel-title>Advanced Filters</mat-panel-title>
          </mat-expansion-panel-header>

          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 py-4">
            <mat-form-field appearance="outline">
              <mat-label>Min Rate (฿/hr)</mat-label>
              <input matInput type="number" formControlName="minRate">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Max Rate (฿/hr)</mat-label>
              <input matInput type="number" formControlName="maxRate">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Min Rating</mat-label>
              <mat-select formControlName="minRating">
                <mat-option [value]="null">Any</mat-option>
                <mat-option [value]="4">4+ Stars</mat-option>
                <mat-option [value]="4.5">4.5+ Stars</mat-option>
              </mat-select>
            </mat-form-field>

            <div class="flex items-center">
              <mat-checkbox formControlName="verifiedOnly">
                Verified Only
              </mat-checkbox>
            </div>
          </div>
        </mat-expansion-panel>
      </form>
    </div>
  </div>

  <!-- Results -->
  <div class="container mx-auto py-8 px-4">
    <!-- Loading State -->
    <div *ngIf="isLoading" class="flex justify-center py-12">
      <mat-spinner></mat-spinner>
    </div>

    <!-- Results Count -->
    <div *ngIf="!isLoading" class="mb-6">
      <p class="text-gray-600">
        Found <span class="font-semibold">{{ totalCount }}</span> providers
      </p>
    </div>

    <!-- Provider Cards Grid -->
    <div *ngIf="!isLoading && providers.length > 0" 
         class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      <mat-card *ngFor="let provider of providers" 
                class="hover:shadow-xl transition-shadow cursor-pointer"
                (click)="viewProviderDetails(provider.id)">
        <mat-card-header>
          <div mat-card-avatar class="bg-blue-500 text-white flex items-center justify-center text-xl font-bold rounded-full">
            {{ provider.user.firstName.charAt(0) }}{{ provider.user.lastName.charAt(0) }}
          </div>
          <mat-card-title class="text-lg">
            {{ provider.user.firstName }} {{ provider.user.lastName }}
            <mat-icon *ngIf="provider.isVerified" 
                      class="text-blue-500 text-sm ml-1 align-middle"
                      matTooltip="Verified Provider">
              verified
            </mat-icon>
          </mat-card-title>
          <mat-card-subtitle>{{ provider.profession }}</mat-card-subtitle>
        </mat-card-header>

        <mat-card-content>
          <p class="text-sm text-gray-600 mb-3 line-clamp-2">{{ provider.bio }}</p>
          
          <div class="flex items-center mb-2">
            <mat-icon class="text-yellow-500 text-sm mr-1">star</mat-icon>
            <span class="font-semibold">{{ provider.averageRating.toFixed(1) }}</span>
            <span class="text-gray-500 text-sm ml-1">({{ provider.totalReviews }} reviews)</span>
          </div>

          <div class="flex items-center mb-2">
            <mat-icon class="text-gray-500 text-sm mr-1">work</mat-icon>
            <span class="text-sm text-gray-600">{{ provider.experienceYears }} years experience</span>
          </div>

          <div class="flex items-center justify-between mt-4 pt-4 border-t">
            <span class="text-2xl font-bold text-blue-600">฿{{ provider.hourlyRate }}</span>
            <span class="text-sm text-gray-500">/hour</span>
          </div>
        </mat-card-content>

        <mat-card-actions>
          <button mat-button color="primary" class="w-full">
            View Profile
          </button>
        </mat-card-actions>
      </mat-card>
    </div>

    <!-- No Results -->
    <div *ngIf="!isLoading && providers.length === 0" class="text-center py-12">
      <mat-icon class="text-gray-400 text-6xl mb-4">search_off</mat-icon>
      <p class="text-xl text-gray-600">No providers found</p>
      <p class="text-gray-500 mt-2">Try adjusting your search criteria</p>
    </div>

    <!-- Pagination -->
    <mat-paginator *ngIf="!isLoading && providers.length > 0"
                   [length]="totalCount"
                   [pageSize]="pageSize"
                   [pageSizeOptions]="[12, 24, 48]"
                   (page)="onPageChange($event)"
                   class="mt-8">
    </mat-paginator>
  </div>
</div>
```

---

## Shared Components

### Rating Display Component
**File:** `src/app/shared/components/rating-display/rating-display.component.ts`

```typescript
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-rating-display',
  template: `
    <div class="flex items-center gap-2">
      <div class="flex">
        <mat-icon *ngFor="let star of stars" 
                  [class]="star <= rating ? 'text-yellow-500' : 'text-gray-300'">
          {{ star <= rating ? 'star' : (star - 0.5 <= rating ? 'star_half' : 'star_border') }}
        </mat-icon>
      </div>
      <span *ngIf="showNumber" class="font-semibold">{{ rating.toFixed(1) }}</span>
      <span *ngIf="showCount && count" class="text-gray-500 text-sm">({{ count }})</span>
    </div>
  `,
  styles: [`
    mat-icon {
      font-size: 20px;
      width: 20px;
      height: 20px;
    }
  `]
})
export class RatingDisplayComponent {
  @Input() rating: number = 0;
  @Input() count?: number;
  @Input() showNumber = true;
  @Input() showCount = true;

  stars = [1, 2, 3, 4, 5];
}
```

### Custom Validators
**File:** `src/app/shared/validators/custom-validators.ts`

```typescript
import { AbstractControl, ValidationErrors, ValidatorFn, FormGroup } from '@angular/forms';

export class CustomValidators {
  // Password strength validator
  static passwordStrength(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) return null;

    const hasUpperCase = /[A-Z]/.test(value);
    const hasLowerCase = /[a-z]/.test(value);
    const hasNumber = /[0-9]/.test(value);
    const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(value);

    const valid = hasUpperCase && hasLowerCase && hasNumber && hasSpecialChar;
    
    return valid ? null : { passwordStrength: true };
  }

  // Thai phone number validator
  static thaiPhoneNumber(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) return null;

    // Thai phone number patterns: 0812345678 or +66812345678
    const pattern = /^(\+66|0)[0-9]{9}$/;
    
    return pattern.test(value) ? null : { thaiPhoneNumber: true };
  }

  // Password match validator for form group
  static passwordMatch(passwordField: string, confirmPasswordField: string): ValidatorFn {
    return (formGroup: AbstractControl): ValidationErrors | null => {
      const password = formGroup.get(passwordField);
      const confirmPassword = formGroup.get(confirmPasswordField);

      if (!password || !confirmPassword) return null;

      if (confirmPassword.errors && !confirmPassword.errors['passwordMismatch']) {
        return null;
      }

      if (password.value !== confirmPassword.value) {
        confirmPassword.setErrors({ passwordMismatch: true });
        return { passwordMismatch: true };
      } else {
        confirmPassword.setErrors(null);
        return null;
      }
    };
  }

  // Date range validator
  static dateRange(startDateField: string, endDateField: string): ValidatorFn {
    return (formGroup: AbstractControl): ValidationErrors | null => {
      const startDate = formGroup.get(startDateField)?.value;
      const endDate = formGroup.get(endDateField)?.value;

      if (!startDate || !endDate) return null;

      const start = new Date(startDate);
      const end = new Date(endDate);

      return start < end ? null : { dateRange: true };
    };
  }

  // Future date validator
  static futureDate(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) return null;

    const selectedDate = new Date(value);
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    return selectedDate >= today ? null : { futureDate: true };
  }

  // Min amount validator
  static minAmount(min: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (value === null || value === undefined || value === '') return null;

      return value >= min ? null : { minAmount: { min, actual: value } };
    };
  }
}
```

---

## Dashboard & Charts

### Provider Dashboard Component
**File:** `src/app/features/provider/components/dashboard/income-summary/income-summary.component.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { ChartConfiguration, ChartData, ChartType } from 'chart.js';
import { ProviderService } from '../../../services/provider.service';

export interface IncomeSummary {
  year: number;
  totalGrossIncome: number;
  totalCommission: number;
  totalWithholdingTax: number;
  totalNetIncome: number;
  totalCompletedBookings: number;
  monthlyBreakdown: MonthlyIncome[];
}

export interface MonthlyIncome {
  month: number;
  grossIncome: number;
  commission: number;
  tax: number;
  netIncome: number;
}

@Component({
  selector: 'app-income-summary',
  templateUrl: './income-summary.component.html',
  styleUrls: ['./income-summary.component.scss']
})
export class IncomeSummaryComponent implements OnInit {
  isLoading = false;
  selectedYear = new Date().getFullYear();
  years: number[] = [];
  incomeSummary?: IncomeSummary;

  // Line Chart Configuration
  lineChartData: ChartData<'line'> = {
    labels: [],
    datasets: []
  };

  lineChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'top'
      },
      tooltip: {
        callbacks: {
          label: function(context) {
            return context.dataset.label + ': ฿' + context.parsed.y.toLocaleString();
          }
        }
      }
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          callback: function(value) {
            return '฿' + value.toLocaleString();
          }
        }
      }
    }
  };

  lineChartType: ChartType = 'line';

  // Pie Chart Configuration
  pieChartData: ChartData<'pie'> = {
    labels: [],
    datasets: []
  };

  pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'right'
      },
      tooltip: {
        callbacks: {
          label: function(context) {
            const label = context.label || '';
            const value = context.parsed;
            const total = context.dataset.data.reduce((a: number, b: any) => a + b, 0) as number;
            const percentage = ((value / total) * 100).toFixed(1);
            return label + ': ฿' + value.toLocaleString() + ' (' + percentage + '%)';
          }
        }
      }
    }
  };

  pieChartType: ChartType = 'pie';

  constructor(private providerService: ProviderService) {}

  ngOnInit(): void {
    this.initYears();
    this.loadIncomeSummary();
  }

  initYears(): void {
    const currentYear = new Date().getFullYear();
    for (let i = 0; i < 5; i++) {
      this.years.push(currentYear - i);
    }
  }

  loadIncomeSummary(): void {
    this.isLoading = true;
    this.providerService.getIncomeSummary(this.selectedYear).subscribe({
      next: (summary) => {
        this.incomeSummary = summary;
        this.prepareCharts();
        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
      }
    });
  }

  prepareCharts(): void {
    if (!this.incomeSummary) return;

    // Line Chart - Monthly Income Trend
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 
                    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    
    this.lineChartData = {
      labels: months,
      datasets: [
        {
          label: 'Gross Income',
          data: this.incomeSummary.monthlyBreakdown.map(m => m.grossIncome),
          borderColor: '#3B82F6',
          backgroundColor: 'rgba(59, 130, 246, 0.1)',
          tension: 0.4
        },
        {
          label: 'Net Income',
          data: this.incomeSummary.monthlyBreakdown.map(m => m.netIncome),
          borderColor: '#10B981',
          backgroundColor: 'rgba(16, 185, 129, 0.1)',
          tension: 0.4
        }
      ]
    };

    // Pie Chart - Income Distribution
    this.pieChartData = {
      labels: ['Net Income', 'Commission (10%)', 'Withholding Tax (3%)'],
      datasets: [{
        data: [
          this.incomeSummary.totalNetIncome,
          this.incomeSummary.totalCommission,
          this.incomeSummary.totalWithholdingTax
        ],
        backgroundColor: ['#10B981', '#F59E0B', '#EF4444'],
        hoverBackgroundColor: ['#059669', '#D97706', '#DC2626']
      }]
    };
  }

  onYearChange(): void {
    this.loadIncomeSummary();
  }
}
```

**File:** `src/app/features/provider/components/dashboard/income-summary/income-summary.component.html`

```html
<div class="container mx-auto py-8 px-4">
  <!-- Header -->
  <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6 gap-4">
    <h1 class="text-3xl font-bold text-gray-900">Income Summary</h1>
    
    <mat-form-field appearance="outline" class="w-full sm:w-48">
      <mat-label>Year</mat-label>
      <mat-select [(value)]="selectedYear" (selectionChange)="onYearChange()">
        <mat-option *ngFor="let year of years" [value]="year">
          {{ year }}
        </mat-option>
      </mat-select>
    </mat-form-field>
  </div>

  <!-- Loading State -->
  <div *ngIf="isLoading" class="flex justify-center py-12">
    <mat-spinner></mat-spinner>
  </div>

  <!-- Summary Cards -->
  <div *ngIf="!isLoading && incomeSummary" class="space-y-8">
    <!-- Overview Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <mat-card class="bg-blue-50">
        <mat-card-content>
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 mb-1">Gross Income</p>
              <p class="text-2xl font-bold text-blue-600">
                ฿{{ incomeSummary.totalGrossIncome.toLocaleString() }}
              </p>
            </div>
            <mat-icon class="text-blue-600 text-4xl">attach_money</mat-icon>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card class="bg-green-50">
        <mat-card-content>
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 mb-1">Net Income</p>
              <p class="text-2xl font-bold text-green-600">
                ฿{{ incomeSummary.totalNetIncome.toLocaleString() }}
              </p>
            </div>
            <mat-icon class="text-green-600 text-4xl">account_balance_wallet</mat-icon>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card class="bg-yellow-50">
        <mat-card-content>
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 mb-1">Commission</p>
              <p class="text-2xl font-bold text-yellow-600">
                ฿{{ incomeSummary.totalCommission.toLocaleString() }}
              </p>
              <p class="text-xs text-gray-500">10% of gross</p>
            </div>
            <mat-icon class="text-yellow-600 text-4xl">business</mat-icon>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card class="bg-red-50">
        <mat-card-content>
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 mb-1">Withholding Tax</p>
              <p class="text-2xl font-bold text-red-600">
                ฿{{ incomeSummary.totalWithholdingTax.toLocaleString() }}
              </p>
              <p class="text-xs text-gray-500">3% of gross</p>
            </div>
            <mat-icon class="text-red-600 text-4xl">receipt</mat-icon>
          </div>
        </mat-card-content>
      </mat-card>
    </div>

    <!-- Completed Bookings -->
    <mat-card>
      <mat-card-content>
        <div class="text-center">
          <p class="text-gray-600 mb-2">Total Completed Bookings</p>
          <p class="text-4xl font-bold text-blue-600">{{ incomeSummary.totalCompletedBookings }}</p>
          <p class="text-sm text-gray-500 mt-2">jobs completed in {{ selectedYear }}</p>
        </div>
      </mat-card-content>
    </mat-card>

    <!-- Charts -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Line Chart -->
      <mat-card class="lg:col-span-2">
        <mat-card-header>
          <mat-card-title>Monthly Income Trend</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div class="chart-container" style="height: 400px;">
            <canvas baseChart
                    [data]="lineChartData"
                    [options]="lineChartOptions"
                    [type]="lineChartType">
            </canvas>
          </div>
        </mat-card-content>
      </mat-card>

      <!-- Pie Chart -->
      <mat-card>
        <mat-card-header>
          <mat-card-title>Income Distribution</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div class="chart-container" style="height: 400px;">
            <canvas baseChart
                    [data]="pieChartData"
                    [options]="pieChartOptions"
                    [type]="pieChartType">
            </canvas>
          </div>
        </mat-card-content>
      </mat-card>
    </div>

    <!-- Download Reports -->
    <mat-card>
      <mat-card-content>
        <div class="flex flex-col sm:flex-row items-center justify-between gap-4">
          <div>
            <h3 class="text-lg font-semibold mb-1">Tax Documents & Reports</h3>
            <p class="text-sm text-gray-600">Download your income reports and tax documents</p>
          </div>
          <div class="flex gap-2">
            <button mat-raised-button color="primary">
              <mat-icon>download</mat-icon>
              Income Report
            </button>
            <button mat-raised-button>
              <mat-icon>description</mat-icon>
              Tax Documents
            </button>
          </div>
        </div>
      </mat-card-content>
    </mat-card>
  </div>

  <!-- No Data -->
  <div *ngIf="!isLoading && !incomeSummary" class="text-center py-12">
    <mat-icon class="text-gray-400 text-6xl mb-4">trending_up</mat-icon>
    <p class="text-xl text-gray-600">No income data for {{ selectedYear }}</p>
    <p class="text-gray-500 mt-2">Complete bookings to see your income summary</p>
  </div>
</div>
```

---

## API Services

### Provider Service
**File:** `src/app/features/provider/services/provider.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface SearchProviderParams {
  keyword?: string;
  profession?: string;
  minRate?: number;
  maxRate?: number;
  minRating?: number;
  verifiedOnly?: boolean;
  sortBy?: string;
  page?: number;
  pageSize?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ProviderService {
  private apiUrl = `${environment.apiUrl}/providers`;

  constructor(private http: HttpClient) {}

  searchProviders(params: SearchProviderParams): Observable<PaginatedResponse<any>> {
    let httpParams = new HttpParams();
    
    Object.keys(params).forEach(key => {
      const value = params[key as keyof SearchProviderParams];
      if (value !== null && value !== undefined && value !== '') {
        httpParams = httpParams.set(key, value.toString());
      }
    });

    return this.http.get<PaginatedResponse<any>>(this.apiUrl, { params: httpParams });
  }

  getProviderById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  getMyProfile(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/my-profile`);
  }

  createProfile(profile: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, profile);
  }

  updateProfile(id: number, profile: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, profile);
  }

  getIncomeSummary(year: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/my-profile/income/summary`, {
      params: { year: year.toString() }
    });
  }

  getTaxDocuments(year: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/my-profile/tax-documents`, {
      params: { year: year.toString() }
    });
  }
}
```

### Booking Service
**File:** `src/app/features/client/services/booking.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

export enum BookingStatus {
  Pending = 1,
  Accepted = 2,
  InProgress = 3,
  Completed = 4,
  Cancelled = 5,
  Disputed = 6
}

export interface CreateBookingRequest {
  providerId: number;
  jobTitle: string;
  description: string;
  scheduledDate: string;
  estimatedHours: number;
  totalAmount: number;
}

export interface Booking {
  id: number;
  clientId: number;
  providerId: number;
  jobTitle: string;
  description: string;
  scheduledDate: string;
  estimatedHours: number;
  totalAmount: number;
  status: BookingStatus;
  createdAt: string;
  provider: any;
  client: any;
}

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private apiUrl = `${environment.apiUrl}/bookings`;

  constructor(private http: HttpClient) {}

  createBooking(booking: CreateBookingRequest): Observable<Booking> {
    return this.http.post<Booking>(this.apiUrl, booking);
  }

  getMyBookings(status?: BookingStatus): Observable<Booking[]> {
    const url = status 
      ? `${this.apiUrl}?status=${status}` 
      : this.apiUrl;
    return this.http.get<Booking[]>(url);
  }

  getBookingById(id: number): Observable<Booking> {
    return this.http.get<Booking>(`${this.apiUrl}/${id}`);
  }

  cancelBooking(id: number, reason: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/cancel`, { reason });
  }

  acceptBooking(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/accept`, {});
  }

  completeBooking(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/complete`, {});
  }
}
```

---

## Routing Configuration

### App Routing Module
**File:** `src/app/app-routing.module.ts`

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
    path: 'unauthorized',
    loadChildren: () => import('./features/errors/errors.module').then(m => m.ErrorsModule)
  },
  {
    path: '**',
    redirectTo: '/unauthorized'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
```

### Auth Guard
**File:** `src/app/core/guards/auth.guard.ts`

```typescript
import { Injectable } from '@angular/core';
import { Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (this.authService.isAuthenticated()) {
      return true;
    }

    return this.router.createUrlTree(['/auth/login']);
  }
}
```

### Role Guard
**File:** `src/app/core/guards/role.guard.ts`

```typescript
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService, UserRole } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    const allowedRoles = route.data['roles'] as UserRole[];
    const user = this.authService.getCurrentUser();

    if (user && allowedRoles.includes(user.role)) {
      return true;
    }

    return this.router.createUrlTree(['/unauthorized']);
  }
}
```

---

## Environment Configuration

### Environment File
**File:** `src/environments/environment.ts`

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api',
  apiTimeout: 30000,
  enableDebugLogs: true
};
```

**File:** `src/environments/environment.prod.ts`

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.jobservicemarketplace.com/api',
  apiTimeout: 30000,
  enableDebugLogs: false
};
```

---

## Tailwind Configuration

### tailwind.config.js

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
      },
      fontFamily: {
        sans: ['Inter', 'sans-serif'],
      },
    },
  },
  plugins: [
    require('@tailwindcss/forms'),
    require('@tailwindcss/typography'),
  ],
}
```

---

## Angular Material Theme

### Custom Theme
**File:** `src/styles/custom-theme.scss`

```scss
@use '@angular/material' as mat;

@include mat.core();

// Define custom palette
$my-primary: mat.define-palette(mat.$blue-palette, 600);
$my-accent: mat.define-palette(mat.$green-palette, 500);
$my-warn: mat.define-palette(mat.$red-palette, 500);

// Create theme
$my-theme: mat.define-light-theme((
  color: (
    primary: $my-primary,
    accent: $my-accent,
    warn: $my-warn,
  )
));

// Apply theme
@include mat.all-component-themes($my-theme);

// Custom overrides
.mat-mdc-raised-button {
  text-transform: uppercase;
  font-weight: 500;
}

.mat-mdc-card {
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  transition: box-shadow 0.3s ease;
  
  &:hover {
    box-shadow: 0 4px 8px rgba(0,0,0,0.15);
  }
}
```

---

**This concludes the comprehensive code examples documentation! 🎉**
