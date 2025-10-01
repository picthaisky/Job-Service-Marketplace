# Testing Strategy Guide

Comprehensive testing guide for the Job Service Marketplace Angular frontend application.

---

## Table of Contents
1. [Testing Philosophy](#testing-philosophy)
2. [Testing Tools Setup](#testing-tools-setup)
3. [Unit Testing](#unit-testing)
4. [Component Testing](#component-testing)
5. [Service Testing](#service-testing)
6. [Integration Testing](#integration-testing)
7. [E2E Testing](#e2e-testing)
8. [Test Coverage](#test-coverage)
9. [Best Practices](#best-practices)
10. [CI/CD Integration](#cicd-integration)

---

## Testing Philosophy

### Testing Pyramid
```
        /\
       /E2E\          (10% - Critical user flows)
      /______\
     /        \
    /Integration\    (20% - Feature workflows)
   /____________\
  /              \
 /  Unit Tests    \  (70% - Individual components/services)
/__________________\
```

### Goals
- **70%+ coverage** for services and utilities
- **60%+ coverage** for components
- **100% coverage** for guards and interceptors
- **E2E tests** for critical user journeys

---

## Testing Tools Setup

### 1. Install Testing Dependencies

```bash
# Karma & Jasmine (already included with Angular)
# Install additional testing utilities
npm install -D @testing-library/angular @testing-library/jest-dom
npm install -D karma-coverage jasmine-spec-reporter

# Install Cypress for E2E
npm install -D cypress @cypress/schematic
ng add @cypress/schematic

# Install additional matchers
npm install -D jest-extended
```

### 2. Karma Configuration

Edit `karma.conf.js`:

```javascript
module.exports = function(config) {
  config.set({
    basePath: '',
    frameworks: ['jasmine', '@angular-devkit/build-angular'],
    plugins: [
      require('karma-jasmine'),
      require('karma-chrome-launcher'),
      require('karma-jasmine-html-reporter'),
      require('karma-coverage'),
      require('jasmine-spec-reporter')
    ],
    client: {
      jasmine: {
        random: false // Disable random test order
      },
      clearContext: false
    },
    jasmineHtmlReporter: {
      suppressAll: true
    },
    coverageReporter: {
      dir: require('path').join(__dirname, './coverage/frontend'),
      subdir: '.',
      reporters: [
        { type: 'html' },
        { type: 'text-summary' },
        { type: 'lcovonly' }
      ],
      check: {
        global: {
          statements: 70,
          branches: 70,
          functions: 70,
          lines: 70
        }
      }
    },
    reporters: ['progress', 'kjhtml', 'spec'],
    port: 9876,
    colors: true,
    logLevel: config.LOG_INFO,
    autoWatch: true,
    browsers: ['Chrome'],
    singleRun: false,
    restartOnFileChange: true,
    customLaunchers: {
      ChromeHeadlessCI: {
        base: 'ChromeHeadless',
        flags: ['--no-sandbox', '--disable-gpu']
      }
    }
  });
};
```

### 3. Cypress Configuration

Edit `cypress.config.ts`:

```typescript
import { defineConfig } from 'cypress';

export default defineConfig({
  e2e: {
    baseUrl: 'http://localhost:4200',
    specPattern: 'cypress/e2e/**/*.cy.ts',
    supportFile: 'cypress/support/e2e.ts',
    video: true,
    screenshot OnRunFailure: true,
    viewportWidth: 1280,
    viewportHeight: 720,
    defaultCommandTimeout: 10000,
    requestTimeout: 10000,
    responseTimeout: 30000,
  },
  component: {
    devServer: {
      framework: 'angular',
      bundler: 'webpack',
    },
    specPattern: '**/*.cy.ts',
  },
});
```

---

## Unit Testing

### Test File Structure

```
src/app/
├── core/
│   ├── services/
│   │   ├── auth.service.ts
│   │   └── auth.service.spec.ts    # Unit test
│   └── guards/
│       ├── auth.guard.ts
│       └── auth.guard.spec.ts      # Unit test
├── features/
│   └── auth/
│       ├── components/
│       │   └── login/
│       │       ├── login.component.ts
│       │       └── login.component.spec.ts  # Component test
```

### Basic Service Test Template

```typescript
import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService, UserRole } from './auth.service';
import { environment } from '../../../environments/environment';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Ensure no outstanding requests
    localStorage.clear(); // Clean up
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('login', () => {
    it('should login successfully and store token', () => {
      const mockResponse = {
        id: 1,
        email: 'test@example.com',
        firstName: 'Test',
        lastName: 'User',
        role: UserRole.Client,
        token: 'mock-jwt-token'
      };

      service.login('test@example.com', 'password').subscribe(response => {
        expect(response).toEqual(mockResponse);
        expect(localStorage.getItem('token')).toBe('mock-jwt-token');
        expect(localStorage.getItem('currentUser')).toBeTruthy();
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({
        email: 'test@example.com',
        password: 'password'
      });
      req.flush(mockResponse);
    });

    it('should handle login error', () => {
      service.login('test@example.com', 'wrong').subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(401);
        }
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
      req.flush('Invalid credentials', { status: 401, statusText: 'Unauthorized' });
    });
  });

  describe('logout', () => {
    it('should clear storage and user state', () => {
      localStorage.setItem('token', 'test-token');
      localStorage.setItem('currentUser', JSON.stringify({ id: 1 }));

      service.logout();

      expect(localStorage.getItem('token')).toBeNull();
      expect(localStorage.getItem('currentUser')).toBeNull();
      expect(service.getCurrentUser()).toBeNull();
    });
  });

  describe('isAuthenticated', () => {
    it('should return true when valid token exists', () => {
      const mockToken = createMockJWT({ exp: Date.now() / 1000 + 3600 }); // Expires in 1 hour
      localStorage.setItem('token', mockToken);
      
      expect(service.isAuthenticated()).toBe(true);
    });

    it('should return false when token is expired', () => {
      const mockToken = createMockJWT({ exp: Date.now() / 1000 - 3600 }); // Expired 1 hour ago
      localStorage.setItem('token', mockToken);
      
      expect(service.isAuthenticated()).toBe(false);
    });

    it('should return false when no token exists', () => {
      expect(service.isAuthenticated()).toBe(false);
    });
  });

  describe('hasRole', () => {
    it('should return true when user has specified role', () => {
      const mockUser = {
        id: 1,
        email: 'test@example.com',
        firstName: 'Test',
        lastName: 'User',
        role: UserRole.Provider,
        token: 'token'
      };
      service['currentUserSubject'].next(mockUser);

      expect(service.hasRole(UserRole.Provider)).toBe(true);
      expect(service.hasRole(UserRole.Client)).toBe(false);
    });

    it('should return false when no user is logged in', () => {
      expect(service.hasRole(UserRole.Provider)).toBe(false);
    });
  });
});

// Helper function to create mock JWT
function createMockJWT(payload: any): string {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
  const body = btoa(JSON.stringify(payload));
  const signature = 'mock-signature';
  return `${header}.${body}.${signature}`;
}
```

---

## Component Testing

### Simple Component Test

```typescript
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from './login.component';
import { AuthService } from '../../../../core/services/auth.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { of, throwError } from 'rxjs';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let snackBarSpy: jasmine.SpyObj<MatSnackBar>;

  beforeEach(async () => {
    const authSpy = jasmine.createSpyObj('AuthService', ['login']);
    const routeSpy = jasmine.createSpyObj('Router', ['navigate']);
    const snackSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    await TestBed.configureTestingModule({
      declarations: [LoginComponent],
      imports: [
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        BrowserAnimationsModule
      ],
      providers: [
        { provide: AuthService, useValue: authSpy },
        { provide: Router, useValue: routeSpy },
        { provide: MatSnackBar, useValue: snackSpy }
      ]
    }).compileComponents();

    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    snackBarSpy = TestBed.inject(MatSnackBar) as jasmine.SpyObj<MatSnackBar>;

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize login form with empty fields', () => {
    expect(component.loginForm.get('email')?.value).toBe('');
    expect(component.loginForm.get('password')?.value).toBe('');
  });

  it('should mark form as invalid when empty', () => {
    expect(component.loginForm.valid).toBeFalsy();
  });

  it('should validate email format', () => {
    const emailControl = component.loginForm.get('email');
    
    emailControl?.setValue('invalid-email');
    expect(emailControl?.hasError('email')).toBeTruthy();
    
    emailControl?.setValue('valid@email.com');
    expect(emailControl?.hasError('email')).toBeFalsy();
  });

  it('should validate password minimum length', () => {
    const passwordControl = component.loginForm.get('password');
    
    passwordControl?.setValue('short');
    expect(passwordControl?.hasError('minlength')).toBeTruthy();
    
    passwordControl?.setValue('longpassword');
    expect(passwordControl?.hasError('minlength')).toBeFalsy();
  });

  it('should not submit invalid form', () => {
    component.onSubmit();
    
    expect(authServiceSpy.login).not.toHaveBeenCalled();
  });

  it('should call AuthService.login on valid form submission', () => {
    const mockUser = {
      id: 1,
      email: 'test@example.com',
      firstName: 'Test',
      lastName: 'User',
      role: 1,
      token: 'mock-token'
    };
    authServiceSpy.login.and.returnValue(of(mockUser));

    component.loginForm.patchValue({
      email: 'test@example.com',
      password: 'password123'
    });

    component.onSubmit();

    expect(authServiceSpy.login).toHaveBeenCalledWith({
      email: 'test@example.com',
      password: 'password123'
    });
  });

  it('should navigate to client dashboard on successful client login', () => {
    const mockUser = {
      id: 1,
      email: 'test@example.com',
      firstName: 'Test',
      lastName: 'User',
      role: 1, // Client
      token: 'mock-token'
    };
    authServiceSpy.login.and.returnValue(of(mockUser));

    component.loginForm.patchValue({
      email: 'test@example.com',
      password: 'password123'
    });

    component.onSubmit();

    expect(routerSpy.navigate).toHaveBeenCalledWith(['/client/marketplace']);
    expect(snackBarSpy.open).toHaveBeenCalledWith(
      'Login successful!',
      'Close',
      jasmine.any(Object)
    );
  });

  it('should handle login error', () => {
    authServiceSpy.login.and.returnValue(
      throwError(() => new Error('Login failed'))
    );

    component.loginForm.patchValue({
      email: 'test@example.com',
      password: 'wrongpassword'
    });

    component.onSubmit();

    expect(component.isLoading).toBe(false);
  });

  it('should toggle password visibility', () => {
    expect(component.hidePassword).toBe(true);
    
    const compiled = fixture.nativeElement;
    const button = compiled.querySelector('[aria-label="Hide password"]');
    button.click();
    
    expect(component.hidePassword).toBe(false);
  });

  it('should display error messages', () => {
    const emailControl = component.loginForm.get('email');
    const passwordControl = component.loginForm.get('password');

    emailControl?.setValue('');
    emailControl?.markAsTouched();
    expect(component.getErrorMessage('email')).toContain('required');

    passwordControl?.setValue('short');
    passwordControl?.markAsTouched();
    expect(component.getErrorMessage('password')).toContain('8 characters');
  });
});
```

### DOM Testing

```typescript
describe('LoginComponent DOM', () => {
  it('should render login form', () => {
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('h2').textContent).toContain('Sign in');
    expect(compiled.querySelector('form')).toBeTruthy();
  });

  it('should have email and password fields', () => {
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('[formControlName="email"]')).toBeTruthy();
    expect(compiled.querySelector('[formControlName="password"]')).toBeTruthy();
  });

  it('should disable submit button when form is invalid', () => {
    const compiled = fixture.nativeElement;
    const submitButton = compiled.querySelector('[type="submit"]');
    expect(submitButton.disabled).toBeTruthy();
  });

  it('should enable submit button when form is valid', () => {
    component.loginForm.patchValue({
      email: 'test@example.com',
      password: 'password123'
    });
    fixture.detectChanges();

    const compiled = fixture.nativeElement;
    const submitButton = compiled.querySelector('[type="submit"]');
    expect(submitButton.disabled).toBeFalsy();
  });
});
```

---

## Service Testing

### HTTP Service Testing

```typescript
import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { BookingService, BookingStatus } from './booking.service';
import { environment } from '../../../../environments/environment';

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

  describe('createBooking', () => {
    it('should create booking successfully', () => {
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
        clientId: 1,
        status: BookingStatus.Pending,
        createdAt: '2024-01-01T00:00:00Z'
      };

      service.createBooking(mockBooking).subscribe(booking => {
        expect(booking).toEqual(mockResponse);
        expect(booking.id).toBe(1);
        expect(booking.status).toBe(BookingStatus.Pending);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/bookings`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockBooking);
      req.flush(mockResponse);
    });
  });

  describe('getBookings', () => {
    it('should get bookings with filters', () => {
      const filters = { status: BookingStatus.Pending, page: 1, pageSize: 10 };
      const mockResponse = {
        data: [
          { id: 1, status: BookingStatus.Pending },
          { id: 2, status: BookingStatus.Pending }
        ],
        totalCount: 2,
        page: 1,
        pageSize: 10
      };

      service.getBookings(filters).subscribe(response => {
        expect(response.data.length).toBe(2);
        expect(response.totalCount).toBe(2);
      });

      const req = httpMock.expectOne(req => 
        req.url.includes(`${environment.apiUrl}/bookings`) &&
        req.params.has('status') &&
        req.params.has('page')
      );
      expect(req.request.method).toBe('GET');
      expect(req.request.params.get('status')).toBe('1');
      req.flush(mockResponse);
    });
  });

  describe('acceptBooking', () => {
    it('should accept booking', () => {
      const bookingId = 1;

      service.acceptBooking(bookingId).subscribe(() => {
        expect(true).toBeTruthy(); // Success
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/bookings/${bookingId}/accept`);
      expect(req.request.method).toBe('POST');
      req.flush({});
    });
  });

  describe('error handling', () => {
    it('should handle 404 error', () => {
      service.getBooking(999).subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(404);
        }
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/bookings/999`);
      req.flush('Not found', { status: 404, statusText: 'Not Found' });
    });
  });
});
```

---

## Integration Testing

### Feature Integration Test

```typescript
import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { BookingService } from './booking.service';
import { PaymentService } from './payment.service';

describe('Booking Flow Integration', () => {
  let authService: AuthService;
  let bookingService: BookingService;
  let paymentService: PaymentService;
  let httpMock: HttpTestingController;
  let router: Router;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService, BookingService, PaymentService]
    });

    authService = TestBed.inject(AuthService);
    bookingService = TestBed.inject(BookingService);
    paymentService = TestBed.inject(PaymentService);
    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should complete full booking and payment flow', (done) => {
    // Step 1: Login
    const mockUser = {
      id: 1,
      email: 'client@example.com',
      firstName: 'Test',
      lastName: 'Client',
      role: 1,
      token: 'mock-token'
    };

    authService.login('client@example.com', 'password').subscribe(user => {
      expect(user).toEqual(mockUser);
      expect(authService.isAuthenticated()).toBe(true);

      // Step 2: Create booking
      const bookingData = {
        providerId: 2,
        jobTitle: 'Home Cleaning',
        description: 'Regular cleaning',
        scheduledDate: '2024-02-01',
        estimatedHours: 3,
        totalAmount: 1500
      };

      const mockBooking = {
        id: 1,
        ...bookingData,
        clientId: 1,
        status: 1,
        createdAt: '2024-01-15T00:00:00Z'
      };

      bookingService.createBooking(bookingData).subscribe(booking => {
        expect(booking.id).toBe(1);
        expect(booking.totalAmount).toBe(1500);

        // Step 3: Process payment
        const paymentData = {
          bookingId: booking.id,
          paymentMethod: 'credit_card',
          cardToken: 'mock-card-token'
        };

        const mockPayment = {
          id: 1,
          bookingId: 1,
          grossAmount: 1500,
          commissionAmount: 150,
          withholdingTaxAmount: 45,
          netAmount: 1305,
          status: 2,
        };

        paymentService.processPayment(paymentData).subscribe(payment => {
          expect(payment.grossAmount).toBe(1500);
          expect(payment.netAmount).toBe(1305);
          done();
        });

        const paymentReq = httpMock.expectOne(/payments\/process/);
        paymentReq.flush(mockPayment);
      });

      const bookingReq = httpMock.expectOne(/bookings$/);
      bookingReq.flush(mockBooking);
    });

    const loginReq = httpMock.expectOne(/auth\/login/);
    loginReq.flush(mockUser);
  });
});
```

---

## E2E Testing

### Cypress E2E Tests

**Login Flow** (`cypress/e2e/auth/login.cy.ts`):

```typescript
describe('Login Flow', () => {
  beforeEach(() => {
    cy.visit('/auth/login');
  });

  it('should display login form', () => {
    cy.contains('h2', 'Sign in to your account').should('be.visible');
    cy.get('[formControlName="email"]').should('be.visible');
    cy.get('[formControlName="password"]').should('be.visible');
    cy.get('[type="submit"]').should('be.visible');
  });

  it('should show validation errors for empty fields', () => {
    cy.get('[type="submit"]').should('be.disabled');
    
    cy.get('[formControlName="email"]').focus().blur();
    cy.contains('email is required').should('be.visible');
    
    cy.get('[formControlName="password"]').focus().blur();
    cy.contains('password is required').should('be.visible');
  });

  it('should login successfully with valid credentials', () => {
    // Stub API response
    cy.intercept('POST', '/api/auth/login', {
      statusCode: 200,
      body: {
        id: 1,
        email: 'client@example.com',
        firstName: 'Test',
        lastName: 'Client',
        role: 1,
        token: 'mock-jwt-token'
      }
    }).as('loginRequest');

    // Fill form
    cy.get('[formControlName="email"]').type('client@example.com');
    cy.get('[formControlName="password"]').type('password123');
    cy.get('[type="submit"]').click();

    // Wait for API call
    cy.wait('@loginRequest');

    // Verify navigation
    cy.url().should('include', '/client/marketplace');
    
    // Verify token stored
    cy.window().then((win) => {
      expect(win.localStorage.getItem('token')).to.equal('mock-jwt-token');
    });
  });

  it('should handle login error', () => {
    cy.intercept('POST', '/api/auth/login', {
      statusCode: 401,
      body: {
        message: 'Invalid credentials'
      }
    }).as('loginError');

    cy.get('[formControlName="email"]').type('wrong@example.com');
    cy.get('[formControlName="password"]').type('wrongpassword');
    cy.get('[type="submit"]').click();

    cy.wait('@loginError');
    cy.contains('Invalid credentials').should('be.visible');
  });

  it('should toggle password visibility', () => {
    cy.get('[formControlName="password"]').should('have.attr', 'type', 'password');
    
    cy.get('[aria-label="Hide password"]').click();
    cy.get('[formControlName="password"]').should('have.attr', 'type', 'text');
    
    cy.get('[aria-label="Hide password"]').click();
    cy.get('[formControlName="password"]').should('have.attr', 'type', 'password');
  });
});
```

**Booking Flow** (`cypress/e2e/booking/create-booking.cy.ts`):

```typescript
describe('Create Booking Flow', () => {
  beforeEach(() => {
    // Login first
    cy.login('client@example.com', 'password123');
    cy.visit('/client/marketplace');
  });

  it('should complete booking creation flow', () => {
    // Step 1: Search for provider
    cy.get('[formControlName="keyword"]').type('Nurse');
    cy.get('[formControlName="profession"]').select('Nurse');
    
    // Wait for results
    cy.get('.provider-card').should('have.length.greaterThan', 0);
    
    // Step 2: Select provider
    cy.get('.provider-card').first().click();
    cy.url().should('include', '/client/providers/');
    
    // Step 3: Create booking
    cy.contains('button', 'Book Now').click();
    
    // Fill booking form
    cy.get('[formControlName="jobTitle"]').type('Home Nursing Care');
    cy.get('[formControlName="description"]').type('Regular health checkup');
    cy.get('[formControlName="scheduledDate"]').type('2024-02-15');
    cy.get('[formControlName="estimatedHours"]').type('3');
    
    // Submit
    cy.intercept('POST', '/api/bookings', {
      statusCode: 201,
      body: {
        id: 1,
        jobTitle: 'Home Nursing Care',
        totalAmount: 1500,
        status: 1
      }
    }).as('createBooking');
    
    cy.contains('button', 'Create Booking').click();
    cy.wait('@createBooking');
    
    // Verify success
    cy.contains('Booking created successfully').should('be.visible');
    cy.url().should('include', '/client/bookings/1');
  });
});
```

**Custom Cypress Commands** (`cypress/support/commands.ts`):

```typescript
declare namespace Cypress {
  interface Chainable {
    login(email: string, password: string): Chainable<void>;
    logout(): Chainable<void>;
  }
}

Cypress.Commands.add('login', (email: string, password: string) => {
  cy.session([email, password], () => {
    cy.visit('/auth/login');
    
    cy.intercept('POST', '/api/auth/login', {
      statusCode: 200,
      body: {
        id: 1,
        email: email,
        firstName: 'Test',
        lastName: 'User',
        role: 1,
        token: 'mock-jwt-token'
      }
    });

    cy.get('[formControlName="email"]').type(email);
    cy.get('[formControlName="password"]').type(password);
    cy.get('[type="submit"]').click();
    
    cy.window().then((win) => {
      win.localStorage.setItem('token', 'mock-jwt-token');
    });
  });
});

Cypress.Commands.add('logout', () => {
  cy.window().then((win) => {
    win.localStorage.clear();
  });
  cy.visit('/auth/login');
});
```

---

## Test Coverage

### Measuring Coverage

```bash
# Run tests with coverage
ng test --no-watch --code-coverage

# View coverage report
open coverage/frontend/index.html
```

### Coverage Report Interpretation

```
Statements   : 75.12% ( 512/681 )
Branches     : 68.23% ( 213/312 )
Functions    : 71.43% ( 150/210 )
Lines        : 74.89% ( 498/665 )
```

### Coverage Goals

| Component Type | Target Coverage |
|----------------|----------------|
| Services | 80%+ |
| Components | 70%+ |
| Guards | 100% |
| Interceptors | 100% |
| Pipes | 90%+ |
| Directives | 80%+ |

---

## Best Practices

### ✅ DO

```typescript
// Use descriptive test names
it('should return error when user credentials are invalid', () => {
  // test code
});

// Use AAA pattern (Arrange, Act, Assert)
it('should create booking', () => {
  // Arrange
  const bookingData = { /* ... */ };
  
  // Act
  service.createBooking(bookingData).subscribe(result => {
    // Assert
    expect(result.id).toBeDefined();
  });
});

// Mock dependencies
const authServiceSpy = jasmine.createSpyObj('AuthService', ['login']);

// Clean up after tests
afterEach(() => {
  localStorage.clear();
  httpMock.verify();
});

// Test edge cases
it('should handle empty response', () => {
  // test code
});

it('should handle null values', () => {
  // test code
});
```

### ❌ DON'T

```typescript
// Don't test implementation details
it('should call private method', () => {
  // DON'T DO THIS
});

// Don't test framework features
it('should have ngOnInit method', () => {
  // DON'T TEST THIS
});

// Don't write flaky tests
it('should work sometimes', () => {
  // DON'T USE setTimeout or random values
});

// Don't repeat setup code
// Use beforeEach instead
```

---

## CI/CD Integration

### GitHub Actions Workflow

`.github/workflows/test.yml`:

```yaml
name: Run Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3
    
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '20.x'
        cache: 'npm'
    
    - name: Install dependencies
      run: npm ci
      working-directory: ./frontend
    
    - name: Run linter
      run: npm run lint
      working-directory: ./frontend
    
    - name: Run unit tests
      run: npm run test:ci
      working-directory: ./frontend
    
    - name: Upload coverage to Codecov
      uses: codecov/codecov-action@v3
      with:
        files: ./frontend/coverage/lcov.info
        flags: frontend
        name: frontend-coverage
    
    - name: Run E2E tests
      run: npm run e2e
      working-directory: ./frontend
    
    - name: Upload E2E screenshots
      if: failure()
      uses: actions/upload-artifact@v3
      with:
        name: cypress-screenshots
        path: frontend/cypress/screenshots
```

### Test Scripts in package.json

```json
{
  "scripts": {
    "test": "ng test",
    "test:ci": "ng test --watch=false --browsers=ChromeHeadlessCI --code-coverage",
    "test:coverage": "ng test --code-coverage",
    "test:watch": "ng test --watch=true",
    "e2e": "cypress run",
    "e2e:open": "cypress open"
  }
}
```

---

## Testing Checklist

### Before Committing
- [ ] All unit tests pass
- [ ] Coverage meets minimum thresholds
- [ ] No console errors or warnings
- [ ] Code linter passes
- [ ] E2E tests for critical flows pass

### Code Review
- [ ] Tests cover happy path
- [ ] Tests cover error cases
- [ ] Tests cover edge cases
- [ ] No hardcoded test data
- [ ] Mocks are properly configured
- [ ] Tests are independent

### Pre-Release
- [ ] Full test suite passes
- [ ] E2E tests on staging environment
- [ ] Performance tests
- [ ] Accessibility tests
- [ ] Cross-browser testing

---

**Complete Testing Strategy! Write tests, ship with confidence! ✅**
