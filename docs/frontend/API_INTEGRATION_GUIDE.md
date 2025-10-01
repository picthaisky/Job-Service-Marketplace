# API Integration Guide

Complete guide for integrating Angular frontend with the Job Service Marketplace backend API.

---

## Table of Contents
1. [API Base Configuration](#api-base-configuration)
2. [Authentication Flow](#authentication-flow)
3. [HTTP Interceptors](#http-interceptors)
4. [Service Implementation](#service-implementation)
5. [Error Handling](#error-handling)
6. [Request/Response Types](#requestresponse-types)
7. [Pagination & Filtering](#pagination--filtering)
8. [File Upload](#file-upload)
9. [Real-time Updates](#real-time-updates)
10. [Testing API Integration](#testing-api-integration)

---

## API Base Configuration

### Environment Setup

**Development** (`environments/environment.ts`):
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api',
  apiTimeout: 30000,
  enableDebugLogs: true,
  maxRetries: 3,
  retryDelay: 1000
};
```

**Production** (`environments/environment.prod.ts`):
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.jobservicemarketplace.com/api',
  apiTimeout: 30000,
  enableDebugLogs: false,
  maxRetries: 3,
  retryDelay: 1000
};
```

### Base API Service

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError, TimeoutError } from 'rxjs';
import { catchError, retry, timeout } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // GET request
  get<T>(endpoint: string, params?: any): Observable<T> {
    const httpParams = this.buildParams(params);
    return this.http.get<T>(`${this.baseUrl}/${endpoint}`, { params: httpParams })
      .pipe(
        timeout(environment.apiTimeout),
        retry(environment.maxRetries),
        catchError(this.handleError)
      );
  }

  // POST request
  post<T>(endpoint: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}/${endpoint}`, body)
      .pipe(
        timeout(environment.apiTimeout),
        catchError(this.handleError)
      );
  }

  // PUT request
  put<T>(endpoint: string, body: any): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}/${endpoint}`, body)
      .pipe(
        timeout(environment.apiTimeout),
        catchError(this.handleError)
      );
  }

  // DELETE request
  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}/${endpoint}`)
      .pipe(
        timeout(environment.apiTimeout),
        catchError(this.handleError)
      );
  }

  // PATCH request
  patch<T>(endpoint: string, body: any): Observable<T> {
    return this.http.patch<T>(`${this.baseUrl}/${endpoint}`, body)
      .pipe(
        timeout(environment.apiTimeout),
        catchError(this.handleError)
      );
  }

  // Build HTTP params from object
  private buildParams(params?: any): HttpParams {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach(key => {
        const value = params[key];
        if (value !== null && value !== undefined && value !== '') {
          if (Array.isArray(value)) {
            value.forEach(item => {
              httpParams = httpParams.append(key, item.toString());
            });
          } else {
            httpParams = httpParams.set(key, value.toString());
          }
        }
      });
    }
    return httpParams;
  }

  // Error handler
  private handleError(error: any): Observable<never> {
    if (environment.enableDebugLogs) {
      console.error('API Error:', error);
    }
    
    let errorMessage = 'An unknown error occurred';
    
    if (error instanceof TimeoutError) {
      errorMessage = 'Request timed out. Please try again.';
    } else if (error.error instanceof ErrorEvent) {
      errorMessage = `Client Error: ${error.error.message}`;
    } else if (error.status) {
      errorMessage = `Server Error: ${error.status} - ${error.statusText}`;
    }
    
    return throwError(() => ({ message: errorMessage, error }));
  }
}
```

---

## Authentication Flow

### Complete Authentication Implementation

```typescript
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { ApiService } from './api.service';

export interface AuthResponse {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  token: string;
}

export enum UserRole {
  Client = 1,
  Provider = 2,
  Admin = 3
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<AuthResponse | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private readonly TOKEN_KEY = 'token';
  private readonly USER_KEY = 'currentUser';

  constructor(private api: ApiService) {
    this.loadStoredUser();
  }

  // Login
  login(email: string, password: string): Observable<AuthResponse> {
    return this.api.post<AuthResponse>('auth/login', { email, password })
      .pipe(
        tap(response => this.setSession(response))
      );
  }

  // Register
  register(userData: RegisterRequest): Observable<AuthResponse> {
    return this.api.post<AuthResponse>('auth/register', userData)
      .pipe(
        tap(response => this.setSession(response))
      );
  }

  // Logout
  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSubject.next(null);
  }

  // Refresh Token (if backend supports)
  refreshToken(): Observable<AuthResponse> {
    return this.api.post<AuthResponse>('auth/refresh', {})
      .pipe(
        tap(response => this.setSession(response))
      );
  }

  // Get current user
  getCurrentUser(): AuthResponse | null {
    return this.currentUserSubject.value;
  }

  // Check if authenticated
  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token && !this.isTokenExpired(token);
  }

  // Get token
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  // Check if user has specific role
  hasRole(role: UserRole): boolean {
    const user = this.getCurrentUser();
    return user ? user.role === role : false;
  }

  // Set session data
  private setSession(authResponse: AuthResponse): void {
    localStorage.setItem(this.TOKEN_KEY, authResponse.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(authResponse));
    this.currentUserSubject.next(authResponse);
  }

  // Load stored user
  private loadStoredUser(): void {
    const storedUser = localStorage.getItem(this.USER_KEY);
    if (storedUser) {
      try {
        const user = JSON.parse(storedUser);
        if (this.isAuthenticated()) {
          this.currentUserSubject.next(user);
        } else {
          this.logout();
        }
      } catch (e) {
        this.logout();
      }
    }
  }

  // Check if token is expired
  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiry = payload.exp * 1000; // Convert to milliseconds
      return Date.now() >= expiry;
    } catch (e) {
      return true;
    }
  }
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  role: UserRole;
}
```

---

## HTTP Interceptors

### Complete Interceptor Setup

**Auth Interceptor** - Add JWT token to requests:
```typescript
import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = this.authService.getToken();
    
    if (token && !this.isExcludedUrl(request.url)) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    return next.handle(request);
  }

  private isExcludedUrl(url: string): boolean {
    const excludedUrls = ['/auth/login', '/auth/register'];
    return excludedUrls.some(excluded => url.includes(excluded));
  }
}
```

**Loading Interceptor** - Show loading indicator:
```typescript
import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { LoadingService } from '../services/loading.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  private activeRequests = 0;

  constructor(private loadingService: LoadingService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (this.activeRequests === 0) {
      this.loadingService.show();
    }
    this.activeRequests++;

    return next.handle(request).pipe(
      finalize(() => {
        this.activeRequests--;
        if (this.activeRequests === 0) {
          this.loadingService.hide();
        }
      })
    );
  }
}
```

**Registering Interceptors** in `app.module.ts`:
```typescript
import { HTTP_INTERCEPTORS } from '@angular/common/http';

@NgModule({
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true }
  ]
})
export class AppModule { }
```

---

## Service Implementation

### Complete Service Examples

**Booking Service**:
```typescript
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';

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
}

export enum BookingStatus {
  Pending = 1,
  Accepted = 2,
  InProgress = 3,
  Completed = 4,
  Cancelled = 5,
  Disputed = 6
}

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private endpoint = 'bookings';

  constructor(private api: ApiService) {}

  // Create booking
  createBooking(booking: CreateBookingRequest): Observable<Booking> {
    return this.api.post<Booking>(this.endpoint, booking);
  }

  // Get all bookings with filters
  getBookings(filters?: BookingFilters): Observable<PaginatedResponse<Booking>> {
    return this.api.get<PaginatedResponse<Booking>>(this.endpoint, filters);
  }

  // Get single booking
  getBooking(id: number): Observable<Booking> {
    return this.api.get<Booking>(`${this.endpoint}/${id}`);
  }

  // Update booking
  updateBooking(id: number, data: Partial<Booking>): Observable<Booking> {
    return this.api.put<Booking>(`${this.endpoint}/${id}`, data);
  }

  // Accept booking (provider)
  acceptBooking(id: number): Observable<void> {
    return this.api.post<void>(`${this.endpoint}/${id}/accept`, {});
  }

  // Complete booking (provider)
  completeBooking(id: number): Observable<void> {
    return this.api.post<void>(`${this.endpoint}/${id}/complete`, {});
  }

  // Cancel booking
  cancelBooking(id: number, reason: string): Observable<void> {
    return this.api.post<void>(`${this.endpoint}/${id}/cancel`, { reason });
  }

  // Get booking statistics
  getStatistics(): Observable<BookingStatistics> {
    return this.api.get<BookingStatistics>(`${this.endpoint}/statistics`);
  }
}

export interface CreateBookingRequest {
  providerId: number;
  jobTitle: string;
  description: string;
  scheduledDate: string;
  estimatedHours: number;
  totalAmount: number;
}

export interface BookingFilters {
  status?: BookingStatus;
  startDate?: string;
  endDate?: string;
  page?: number;
  pageSize?: number;
}

export interface BookingStatistics {
  totalBookings: number;
  completedBookings: number;
  pendingBookings: number;
  totalRevenue: number;
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}
```

**Payment Service**:
```typescript
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';

export interface Payment {
  id: number;
  bookingId: number;
  grossAmount: number;
  commissionAmount: number;
  withholdingTaxAmount: number;
  netAmount: number;
  status: PaymentStatus;
  releaseDate?: string;
}

export enum PaymentStatus {
  Pending = 1,
  Held = 2,
  Released = 3,
  Refunded = 4
}

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private endpoint = 'payments';

  constructor(private api: ApiService) {}

  // Process payment
  processPayment(paymentData: ProcessPaymentRequest): Observable<Payment> {
    return this.api.post<Payment>(`${this.endpoint}/process`, paymentData);
  }

  // Get payment details
  getPayment(id: number): Observable<Payment> {
    return this.api.get<Payment>(`${this.endpoint}/${id}`);
  }

  // Get payment by booking
  getPaymentByBooking(bookingId: number): Observable<Payment> {
    return this.api.get<Payment>(`${this.endpoint}/booking/${bookingId}`);
  }

  // Release payment (admin)
  releasePayment(id: number): Observable<void> {
    return this.api.post<void>(`${this.endpoint}/${id}/release`, {});
  }

  // Get transactions
  getTransactions(filters?: TransactionFilters): Observable<Transaction[]> {
    return this.api.get<Transaction[]>('transactions', filters);
  }

  // Calculate payment breakdown
  calculatePayment(amount: number): Observable<PaymentCalculation> {
    return this.api.get<PaymentCalculation>(`${this.endpoint}/calculate`, { amount });
  }
}

export interface ProcessPaymentRequest {
  bookingId: number;
  paymentMethod: string;
  cardToken?: string;
}

export interface Transaction {
  id: number;
  paymentId: number;
  type: TransactionType;
  amount: number;
  description: string;
  createdAt: string;
}

export enum TransactionType {
  Payment = 1,
  Commission = 2,
  WithholdingTax = 3,
  Release = 4,
  Refund = 5
}

export interface TransactionFilters {
  startDate?: string;
  endDate?: string;
  type?: TransactionType;
  page?: number;
  pageSize?: number;
}

export interface PaymentCalculation {
  grossAmount: number;
  commissionAmount: number;
  commissionRate: number;
  withholdingTaxAmount: number;
  taxRate: number;
  netAmount: number;
}
```

**Review Service**:
```typescript
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';

export interface Review {
  id: number;
  bookingId: number;
  clientId: number;
  providerId: number;
  rating: number;
  comment: string;
  createdAt: string;
  client: {
    firstName: string;
    lastName: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
  private endpoint = 'reviews';

  constructor(private api: ApiService) {}

  // Create review
  createReview(review: CreateReviewRequest): Observable<Review> {
    return this.api.post<Review>(this.endpoint, review);
  }

  // Get provider reviews
  getProviderReviews(providerId: number, params?: ReviewFilters): Observable<PaginatedResponse<Review>> {
    return this.api.get<PaginatedResponse<Review>>(`providers/${providerId}/reviews`, params);
  }

  // Get single review
  getReview(id: number): Observable<Review> {
    return this.api.get<Review>(`${this.endpoint}/${id}`);
  }

  // Update review
  updateReview(id: number, data: UpdateReviewRequest): Observable<Review> {
    return this.api.put<Review>(`${this.endpoint}/${id}`, data);
  }

  // Delete review
  deleteReview(id: number): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}

export interface CreateReviewRequest {
  bookingId: number;
  rating: number;
  comment: string;
}

export interface UpdateReviewRequest {
  rating: number;
  comment: string;
}

export interface ReviewFilters {
  minRating?: number;
  page?: number;
  pageSize?: number;
}

interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
}
```

---

## Error Handling

### Comprehensive Error Handler

```typescript
import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

export interface ApiError {
  status: number;
  message: string;
  errors?: { [key: string]: string[] };
}

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {
  constructor(
    private snackBar: MatSnackBar,
    private router: Router
  ) {}

  handleError(error: HttpErrorResponse): void {
    let message = 'An error occurred';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      message = `Client Error: ${error.error.message}`;
    } else {
      // Server-side error
      switch (error.status) {
        case 400:
          message = this.handle400Error(error);
          break;
        case 401:
          message = 'Unauthorized - Please login again';
          this.router.navigate(['/auth/login']);
          break;
        case 403:
          message = 'Access forbidden - Insufficient permissions';
          this.router.navigate(['/unauthorized']);
          break;
        case 404:
          message = 'Resource not found';
          break;
        case 422:
          message = this.handle422Error(error);
          break;
        case 500:
          message = 'Internal server error - Please try again later';
          break;
        default:
          message = error.error?.message || `Error: ${error.status}`;
      }
    }

    this.showError(message);
  }

  private handle400Error(error: HttpErrorResponse): string {
    if (error.error?.errors) {
      // Validation errors
      const errors = Object.values(error.error.errors).flat();
      return errors.join(', ');
    }
    return error.error?.message || 'Bad request';
  }

  private handle422Error(error: HttpErrorResponse): string {
    if (error.error?.errors) {
      // Validation errors
      const errors = Object.values(error.error.errors).flat();
      return errors.join(', ');
    }
    return error.error?.message || 'Validation failed';
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: ['error-snackbar']
    });
  }

  showSuccess(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: ['success-snackbar']
    });
  }

  showInfo(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: ['info-snackbar']
    });
  }

  showWarning(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 4000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: ['warning-snackbar']
    });
  }
}
```

---

## Pagination & Filtering

### Reusable Pagination Component

```typescript
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-pagination',
  template: `
    <mat-paginator
      [length]="totalCount"
      [pageSize]="pageSize"
      [pageSizeOptions]="pageSizeOptions"
      [pageIndex]="pageIndex"
      (page)="onPageChange($event)"
      showFirstLastButtons>
    </mat-paginator>
  `
})
export class PaginationComponent {
  @Input() totalCount = 0;
  @Input() pageSize = 10;
  @Input() pageIndex = 0;
  @Input() pageSizeOptions = [10, 25, 50, 100];
  
  @Output() pageChange = new EventEmitter<PageEvent>();

  onPageChange(event: PageEvent): void {
    this.pageChange.emit(event);
  }
}
```

### Usage in Component:
```typescript
export class MyListComponent {
  page = 1;
  pageSize = 10;
  totalCount = 0;

  onPageChange(event: PageEvent): void {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadData();
  }

  loadData(): void {
    this.service.getData({ page: this.page, pageSize: this.pageSize })
      .subscribe(response => {
        this.items = response.data;
        this.totalCount = response.totalCount;
      });
  }
}
```

---

## File Upload

### File Upload Service

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpEventType, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface UploadProgress {
  progress: number;
  file?: any;
}

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
  constructor(private http: HttpClient) {}

  uploadFile(file: File, endpoint: string): Observable<UploadProgress> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    return this.http.post<any>(`${environment.apiUrl}/${endpoint}`, formData, {
      reportProgress: true,
      observe: 'events'
    }).pipe(
      map((event: HttpEvent<any>) => {
        switch (event.type) {
          case HttpEventType.UploadProgress:
            const progress = event.total ? Math.round(100 * event.loaded / event.total) : 0;
            return { progress };
          case HttpEventType.Response:
            return { progress: 100, file: event.body };
          default:
            return { progress: 0 };
        }
      })
    );
  }

  uploadMultipleFiles(files: File[], endpoint: string): Observable<UploadProgress> {
    const formData = new FormData();
    files.forEach(file => {
      formData.append('files', file, file.name);
    });

    return this.http.post<any>(`${environment.apiUrl}/${endpoint}`, formData, {
      reportProgress: true,
      observe: 'events'
    }).pipe(
      map((event: HttpEvent<any>) => {
        switch (event.type) {
          case HttpEventType.UploadProgress:
            const progress = event.total ? Math.round(100 * event.loaded / event.total) : 0;
            return { progress };
          case HttpEventType.Response:
            return { progress: 100, file: event.body };
          default:
            return { progress: 0 };
        }
      })
    );
  }
}
```

### Usage in Component:
```typescript
export class PortfolioUploadComponent {
  uploadProgress = 0;
  isUploading = false;

  constructor(private fileUploadService: FileUploadService) {}

  onFileSelected(event: any): void {
    const file: File = event.target.files[0];
    if (file) {
      this.isUploading = true;
      this.fileUploadService.uploadFile(file, 'portfolios/upload')
        .subscribe({
          next: (progress) => {
            this.uploadProgress = progress.progress;
            if (progress.file) {
              console.log('Upload complete:', progress.file);
              this.isUploading = false;
            }
          },
          error: (error) => {
            console.error('Upload failed:', error);
            this.isUploading = false;
          }
        });
    }
  }
}
```

---

## Testing API Integration

### Service Unit Tests

```typescript
import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { BookingService } from './booking.service';
import { environment } from '../../../environments/environment';

describe('BookingService', () => {
  let service: BookingService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [BookingService]
    });
    service = TestBed.inject(BookingService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create booking', () => {
    const mockBooking = {
      providerId: 1,
      jobTitle: 'Test Job',
      description: 'Test Description',
      scheduledDate: '2024-01-01',
      estimatedHours: 4,
      totalAmount: 2000
    };

    const mockResponse = {
      id: 1,
      ...mockBooking,
      status: 1,
      clientId: 1
    };

    service.createBooking(mockBooking).subscribe(booking => {
      expect(booking).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/bookings`);
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should get bookings with filters', () => {
    const filters = { status: 1, page: 1, pageSize: 10 };
    const mockResponse = {
      data: [],
      totalCount: 0,
      page: 1,
      pageSize: 10
    };

    service.getBookings(filters).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(req => 
      req.url.includes(`${environment.apiUrl}/bookings`) && 
      req.params.has('status')
    );
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
});
```

---

## API Endpoint Reference

### Complete Endpoint List

| Feature | Method | Endpoint | Description |
|---------|--------|----------|-------------|
| **Auth** | POST | `/auth/login` | User login |
| **Auth** | POST | `/auth/register` | User registration |
| **Auth** | POST | `/auth/refresh` | Refresh token |
| **Users** | GET | `/users/{id}` | Get user profile |
| **Users** | PUT | `/users/{id}` | Update user profile |
| **Providers** | GET | `/providers` | Search providers |
| **Providers** | GET | `/providers/{id}` | Get provider details |
| **Providers** | POST | `/providers` | Create provider profile |
| **Providers** | PUT | `/providers/{id}` | Update provider profile |
| **Availabilities** | GET | `/providers/{id}/availabilities` | Get availability |
| **Availabilities** | POST | `/providers/{id}/availabilities` | Add availability |
| **Portfolios** | GET | `/providers/{id}/portfolios` | Get portfolios |
| **Portfolios** | POST | `/providers/{id}/portfolios` | Add portfolio |
| **Bookings** | GET | `/bookings` | Get bookings |
| **Bookings** | POST | `/bookings` | Create booking |
| **Bookings** | GET | `/bookings/{id}` | Get booking details |
| **Bookings** | POST | `/bookings/{id}/accept` | Accept booking |
| **Bookings** | POST | `/bookings/{id}/complete` | Complete booking |
| **Bookings** | POST | `/bookings/{id}/cancel` | Cancel booking |
| **Payments** | POST | `/payments/process` | Process payment |
| **Payments** | GET | `/payments/{id}` | Get payment details |
| **Payments** | POST | `/payments/{id}/release` | Release payment |
| **Transactions** | GET | `/transactions` | Get transactions |
| **Reviews** | GET | `/providers/{id}/reviews` | Get provider reviews |
| **Reviews** | POST | `/reviews` | Create review |
| **Admin** | GET | `/admin/users` | Get all users |
| **Admin** | GET | `/admin/analytics/dashboard` | Get analytics |
| **Income** | GET | `/providers/{id}/income/summary` | Get income summary |
| **Tax** | GET | `/providers/{id}/tax-documents` | Get tax documents |

---

**Complete API Integration Guide! Ready for implementation! 🚀**
