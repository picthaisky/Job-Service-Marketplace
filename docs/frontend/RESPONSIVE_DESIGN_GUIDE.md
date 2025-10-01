# Responsive Design Guide

Complete guide for implementing responsive, mobile-first design in the Job Service Marketplace frontend.

---

## Table of Contents
1. [Mobile-First Approach](#mobile-first-approach)
2. [Breakpoint System](#breakpoint-system)
3. [Layout Patterns](#layout-patterns)
4. [Navigation Patterns](#navigation-patterns)
5. [Form Layouts](#form-layouts)
6. [Table Responsiveness](#table-responsiveness)
7. [Card Layouts](#card-layouts)
8. [Touch Interactions](#touch-interactions)
9. [Performance Optimization](#performance-optimization)
10. [Testing Responsive Design](#testing-responsive-design)

---

## Mobile-First Approach

### Philosophy
Start with mobile design and progressively enhance for larger screens.

### Base Styles (Mobile)
```scss
// Mobile-first CSS
.container {
  padding: 1rem;
  width: 100%;
}

.heading {
  font-size: 1.5rem; // 24px
}

.button {
  width: 100%;
  padding: 0.75rem;
}

// Progressive enhancement
@media (min-width: 768px) {
  .container {
    padding: 2rem;
    max-width: 1200px;
    margin: 0 auto;
  }

  .heading {
    font-size: 2rem; // 32px
  }

  .button {
    width: auto;
    padding: 0.5rem 2rem;
  }
}
```

---

## Breakpoint System

### Tailwind CSS Breakpoints
```javascript
// tailwind.config.js
module.exports = {
  theme: {
    screens: {
      'sm': '640px',   // Small devices (landscape phones)
      'md': '768px',   // Medium devices (tablets)
      'lg': '1024px',  // Large devices (desktops)
      'xl': '1280px',  // Extra large devices (large desktops)
      '2xl': '1536px'  // 2X extra large devices
    }
  }
}
```

### Usage in HTML
```html
<!-- Stack on mobile, grid on tablet+ -->
<div class="flex flex-col md:flex-row gap-4">
  <div class="w-full md:w-1/2">Column 1</div>
  <div class="w-full md:w-1/2">Column 2</div>
</div>

<!-- Hide on mobile, show on desktop -->
<div class="hidden lg:block">Desktop only content</div>

<!-- Show on mobile, hide on desktop -->
<div class="block lg:hidden">Mobile only content</div>

<!-- Responsive text sizes -->
<h1 class="text-2xl md:text-3xl lg:text-4xl">Responsive Heading</h1>

<!-- Responsive padding -->
<div class="p-4 md:p-6 lg:p-8">Content with responsive padding</div>
```

---

## Layout Patterns

### Container Pattern
```html
<!-- Responsive container -->
<div class="container mx-auto px-4 sm:px-6 lg:px-8 max-w-7xl">
  <div class="py-8 sm:py-12 lg:py-16">
    <!-- Content -->
  </div>
</div>
```

### Grid Layout
```html
<!-- Responsive grid -->
<div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 sm:gap-6">
  <div class="bg-white p-4 rounded-lg shadow">Item 1</div>
  <div class="bg-white p-4 rounded-lg shadow">Item 2</div>
  <div class="bg-white p-4 rounded-lg shadow">Item 3</div>
  <div class="bg-white p-4 rounded-lg shadow">Item 4</div>
</div>
```

### Flex Layout
```html
<!-- Responsive flex -->
<div class="flex flex-col md:flex-row gap-4">
  <!-- Main content -->
  <main class="flex-1 order-2 md:order-1">
    Main content
  </main>
  
  <!-- Sidebar -->
  <aside class="w-full md:w-64 order-1 md:order-2">
    Sidebar
  </aside>
</div>
```

### Two-Column Layout
```html
<!-- Responsive two columns -->
<div class="grid grid-cols-1 lg:grid-cols-12 gap-6">
  <!-- Main content (8 columns on desktop) -->
  <div class="lg:col-span-8">
    <mat-card>Main content area</mat-card>
  </div>
  
  <!-- Sidebar (4 columns on desktop) -->
  <div class="lg:col-span-4">
    <mat-card>Sidebar content</mat-card>
  </div>
</div>
```

---

## Navigation Patterns

### Header with Mobile Menu

**Component TypeScript**:
```typescript
import { Component } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  isMobileMenuOpen = false;
  
  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }
  
  closeMobileMenu(): void {
    this.isMobileMenuOpen = false;
  }
}
```

**Component HTML**:
```html
<header class="bg-white shadow-md">
  <div class="container mx-auto px-4">
    <div class="flex items-center justify-between h-16">
      <!-- Logo -->
      <div class="flex-shrink-0">
        <a routerLink="/" class="text-2xl font-bold text-blue-600">
          JobMarket
        </a>
      </div>

      <!-- Desktop Navigation -->
      <nav class="hidden md:flex space-x-8">
        <a routerLink="/marketplace" 
           routerLinkActive="text-blue-600"
           class="text-gray-700 hover:text-blue-600">
          Find Jobs
        </a>
        <a routerLink="/providers" 
           routerLinkActive="text-blue-600"
           class="text-gray-700 hover:text-blue-600">
          Providers
        </a>
        <a routerLink="/about" 
           routerLinkActive="text-blue-600"
           class="text-gray-700 hover:text-blue-600">
          About
        </a>
      </nav>

      <!-- Desktop User Menu -->
      <div class="hidden md:flex items-center space-x-4">
        <button mat-button [matMenuTriggerFor]="userMenu">
          <mat-icon>account_circle</mat-icon>
          <span class="ml-2">Account</span>
        </button>
        <mat-menu #userMenu="matMenu">
          <button mat-menu-item routerLink="/profile">
            <mat-icon>person</mat-icon>
            <span>Profile</span>
          </button>
          <button mat-menu-item routerLink="/settings">
            <mat-icon>settings</mat-icon>
            <span>Settings</span>
          </button>
          <mat-divider></mat-divider>
          <button mat-menu-item (click)="logout()">
            <mat-icon>logout</mat-icon>
            <span>Logout</span>
          </button>
        </mat-menu>
      </div>

      <!-- Mobile Menu Button -->
      <button class="md:hidden" mat-icon-button (click)="toggleMobileMenu()">
        <mat-icon>{{ isMobileMenuOpen ? 'close' : 'menu' }}</mat-icon>
      </button>
    </div>

    <!-- Mobile Menu -->
    <div *ngIf="isMobileMenuOpen" 
         class="md:hidden pb-4 animate-slideDown">
      <nav class="flex flex-col space-y-2">
        <a routerLink="/marketplace" 
           (click)="closeMobileMenu()"
           class="px-4 py-2 text-gray-700 hover:bg-gray-100 rounded">
          Find Jobs
        </a>
        <a routerLink="/providers" 
           (click)="closeMobileMenu()"
           class="px-4 py-2 text-gray-700 hover:bg-gray-100 rounded">
          Providers
        </a>
        <a routerLink="/about" 
           (click)="closeMobileMenu()"
           class="px-4 py-2 text-gray-700 hover:bg-gray-100 rounded">
          About
        </a>
        <mat-divider class="my-2"></mat-divider>
        <a routerLink="/profile" 
           (click)="closeMobileMenu()"
           class="px-4 py-2 text-gray-700 hover:bg-gray-100 rounded">
          Profile
        </a>
        <a routerLink="/settings" 
           (click)="closeMobileMenu()"
           class="px-4 py-2 text-gray-700 hover:bg-gray-100 rounded">
          Settings
        </a>
        <button (click)="logout()" 
                class="px-4 py-2 text-left text-red-600 hover:bg-gray-100 rounded">
          Logout
        </button>
      </nav>
    </div>
  </div>
</header>
```

### Bottom Navigation (Mobile)
```html
<!-- Fixed bottom navigation for mobile -->
<nav class="md:hidden fixed bottom-0 left-0 right-0 bg-white border-t shadow-lg z-50">
  <div class="grid grid-cols-4 h-16">
    <a routerLink="/home" 
       routerLinkActive="text-blue-600"
       class="flex flex-col items-center justify-center text-gray-600">
      <mat-icon>home</mat-icon>
      <span class="text-xs mt-1">Home</span>
    </a>
    <a routerLink="/search" 
       routerLinkActive="text-blue-600"
       class="flex flex-col items-center justify-center text-gray-600">
      <mat-icon>search</mat-icon>
      <span class="text-xs mt-1">Search</span>
    </a>
    <a routerLink="/bookings" 
       routerLinkActive="text-blue-600"
       class="flex flex-col items-center justify-center text-gray-600">
      <mat-icon>event</mat-icon>
      <span class="text-xs mt-1">Bookings</span>
    </a>
    <a routerLink="/profile" 
       routerLinkActive="text-blue-600"
       class="flex flex-col items-center justify-center text-gray-600">
      <mat-icon>person</mat-icon>
      <span class="text-xs mt-1">Profile</span>
    </a>
  </div>
</nav>

<!-- Add padding to prevent content from hiding under bottom nav -->
<div class="md:pb-0 pb-16">
  <!-- Page content -->
</div>
```

---

## Form Layouts

### Responsive Form
```html
<form [formGroup]="form" (ngSubmit)="onSubmit()" class="space-y-6">
  <!-- Single column on mobile, two columns on desktop -->
  <div class="grid grid-cols-1 md:grid-cols-2 gap-4 md:gap-6">
    <mat-form-field appearance="outline" class="w-full">
      <mat-label>First Name</mat-label>
      <input matInput formControlName="firstName">
    </mat-form-field>

    <mat-form-field appearance="outline" class="w-full">
      <mat-label>Last Name</mat-label>
      <input matInput formControlName="lastName">
    </mat-form-field>
  </div>

  <!-- Full width field -->
  <mat-form-field appearance="outline" class="w-full">
    <mat-label>Email</mat-label>
    <input matInput type="email" formControlName="email">
  </mat-form-field>

  <!-- Textarea -->
  <mat-form-field appearance="outline" class="w-full">
    <mat-label>Bio</mat-label>
    <textarea matInput 
              formControlName="bio" 
              rows="4"
              class="resize-none"></textarea>
  </mat-form-field>

  <!-- Responsive buttons -->
  <div class="flex flex-col sm:flex-row gap-3 sm:gap-4 sm:justify-end">
    <button mat-button type="button" class="w-full sm:w-auto">
      Cancel
    </button>
    <button mat-raised-button color="primary" type="submit" 
            class="w-full sm:w-auto">
      Save
    </button>
  </div>
</form>
```

### Multi-Step Form (Mobile-Friendly)
```html
<mat-stepper orientation="vertical" linear #stepper class="md:hidden">
  <mat-step [stepControl]="step1Form">
    <ng-template matStepLabel>Personal Info</ng-template>
    <!-- Step 1 content -->
    <div class="py-4">
      <button mat-button matStepperNext class="w-full">Next</button>
    </div>
  </mat-step>
  
  <mat-step [stepControl]="step2Form">
    <ng-template matStepLabel>Professional Info</ng-template>
    <!-- Step 2 content -->
    <div class="py-4 flex gap-2">
      <button mat-button matStepperPrevious class="flex-1">Back</button>
      <button mat-raised-button matStepperNext color="primary" class="flex-1">Next</button>
    </div>
  </mat-step>
  
  <mat-step>
    <ng-template matStepLabel>Review</ng-template>
    <!-- Step 3 content -->
    <div class="py-4 flex gap-2">
      <button mat-button matStepperPrevious class="flex-1">Back</button>
      <button mat-raised-button color="primary" class="flex-1">Submit</button>
    </div>
  </mat-step>
</mat-stepper>

<!-- Horizontal stepper for desktop -->
<mat-stepper orientation="horizontal" linear #stepper class="hidden md:block">
  <!-- Same steps as above -->
</mat-stepper>
```

---

## Table Responsiveness

### Responsive Table with Cards
```html
<!-- Table view for desktop -->
<div class="hidden md:block overflow-x-auto">
  <table mat-table [dataSource]="dataSource" class="w-full">
    <ng-container matColumnDef="id">
      <th mat-header-cell *matHeaderCellDef>ID</th>
      <td mat-cell *matCellDef="let row">{{ row.id }}</td>
    </ng-container>

    <ng-container matColumnDef="name">
      <th mat-header-cell *matHeaderCellDef>Name</th>
      <td mat-cell *matCellDef="let row">{{ row.name }}</td>
    </ng-container>

    <ng-container matColumnDef="amount">
      <th mat-header-cell *matHeaderCellDef>Amount</th>
      <td mat-cell *matCellDef="let row">฿{{ row.amount }}</td>
    </ng-container>

    <ng-container matColumnDef="status">
      <th mat-header-cell *matHeaderCellDef>Status</th>
      <td mat-cell *matCellDef="let row">
        <mat-chip [color]="getStatusColor(row.status)" selected>
          {{ row.status }}
        </mat-chip>
      </td>
    </ng-container>

    <ng-container matColumnDef="actions">
      <th mat-header-cell *matHeaderCellDef>Actions</th>
      <td mat-cell *matCellDef="let row">
        <button mat-icon-button [matMenuTriggerFor]="menu">
          <mat-icon>more_vert</mat-icon>
        </button>
      </td>
    </ng-container>

    <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
    <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
  </table>
</div>

<!-- Card view for mobile -->
<div class="md:hidden space-y-4">
  <mat-card *ngFor="let item of dataSource.data" class="cursor-pointer hover:shadow-lg transition-shadow">
    <mat-card-content>
      <div class="flex justify-between items-start mb-2">
        <div>
          <p class="text-sm text-gray-500">ID: {{ item.id }}</p>
          <h3 class="text-lg font-semibold">{{ item.name }}</h3>
        </div>
        <button mat-icon-button [matMenuTriggerFor]="menu">
          <mat-icon>more_vert</mat-icon>
        </button>
      </div>

      <div class="flex justify-between items-center mt-4">
        <div>
          <p class="text-2xl font-bold text-blue-600">฿{{ item.amount }}</p>
        </div>
        <mat-chip [color]="getStatusColor(item.status)" selected>
          {{ item.status }}
        </mat-chip>
      </div>
    </mat-card-content>
  </mat-card>
</div>
```

### Horizontal Scroll Table
```html
<div class="overflow-x-auto">
  <div class="inline-block min-w-full align-middle">
    <div class="overflow-hidden shadow ring-1 ring-black ring-opacity-5 md:rounded-lg">
      <table class="min-w-full divide-y divide-gray-300">
        <!-- Table content -->
      </table>
    </div>
  </div>
</div>
```

---

## Card Layouts

### Responsive Provider Card
```html
<mat-card class="h-full flex flex-col hover:shadow-xl transition-shadow cursor-pointer">
  <!-- Card Header -->
  <mat-card-header class="pb-4">
    <div mat-card-avatar 
         class="w-12 h-12 sm:w-16 sm:h-16 bg-blue-500 text-white flex items-center justify-center text-xl font-bold rounded-full">
      {{ provider.firstName.charAt(0) }}{{ provider.lastName.charAt(0) }}
    </div>
    <mat-card-title class="text-base sm:text-lg">
      <div class="flex items-center">
        {{ provider.firstName }} {{ provider.lastName }}
        <mat-icon *ngIf="provider.isVerified" 
                  class="text-blue-500 text-sm ml-1"
                  matTooltip="Verified">
          verified
        </mat-icon>
      </div>
    </mat-card-title>
    <mat-card-subtitle class="text-sm">
      {{ provider.profession }}
    </mat-card-subtitle>
  </mat-card-header>

  <!-- Card Content -->
  <mat-card-content class="flex-grow">
    <!-- Bio - Show 2 lines on mobile, 3 on desktop -->
    <p class="text-sm text-gray-600 mb-3 line-clamp-2 md:line-clamp-3">
      {{ provider.bio }}
    </p>

    <!-- Rating -->
    <div class="flex items-center mb-2">
      <mat-icon class="text-yellow-500 text-base sm:text-lg mr-1">star</mat-icon>
      <span class="font-semibold text-sm sm:text-base">{{ provider.rating.toFixed(1) }}</span>
      <span class="text-gray-500 text-xs sm:text-sm ml-1">({{ provider.reviews }} reviews)</span>
    </div>

    <!-- Experience -->
    <div class="flex items-center mb-2">
      <mat-icon class="text-gray-500 text-base sm:text-lg mr-1">work</mat-icon>
      <span class="text-xs sm:text-sm text-gray-600">
        {{ provider.experience }} years experience
      </span>
    </div>

    <!-- Price -->
    <div class="flex items-center justify-between mt-4 pt-4 border-t">
      <div>
        <span class="text-xl sm:text-2xl font-bold text-blue-600">
          ฿{{ provider.hourlyRate }}
        </span>
        <span class="text-xs sm:text-sm text-gray-500">/hour</span>
      </div>
    </div>
  </mat-card-content>

  <!-- Card Actions -->
  <mat-card-actions class="px-4 pb-4">
    <button mat-raised-button color="primary" class="w-full">
      View Profile
    </button>
  </mat-card-actions>
</mat-card>
```

### Dashboard Widget Cards
```html
<!-- Responsive dashboard cards -->
<div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
  <!-- Card 1 -->
  <mat-card class="bg-gradient-to-br from-blue-500 to-blue-600 text-white">
    <mat-card-content>
      <div class="flex items-center justify-between">
        <div>
          <p class="text-sm opacity-90 mb-1">Total Revenue</p>
          <p class="text-2xl sm:text-3xl font-bold">฿250,000</p>
          <p class="text-xs opacity-75 mt-1">+12% from last month</p>
        </div>
        <mat-icon class="text-4xl sm:text-5xl opacity-75">
          attach_money
        </mat-icon>
      </div>
    </mat-card-content>
  </mat-card>

  <!-- Card 2 -->
  <mat-card class="bg-gradient-to-br from-green-500 to-green-600 text-white">
    <mat-card-content>
      <div class="flex items-center justify-between">
        <div>
          <p class="text-sm opacity-90 mb-1">Completed Jobs</p>
          <p class="text-2xl sm:text-3xl font-bold">45</p>
          <p class="text-xs opacity-75 mt-1">+5 from last week</p>
        </div>
        <mat-icon class="text-4xl sm:text-5xl opacity-75">
          check_circle
        </mat-icon>
      </div>
    </mat-card-content>
  </mat-card>

  <!-- Card 3 -->
  <mat-card class="bg-gradient-to-br from-purple-500 to-purple-600 text-white">
    <mat-card-content>
      <div class="flex items-center justify-between">
        <div>
          <p class="text-sm opacity-90 mb-1">Pending Bookings</p>
          <p class="text-2xl sm:text-3xl font-bold">8</p>
          <p class="text-xs opacity-75 mt-1">Awaiting response</p>
        </div>
        <mat-icon class="text-4xl sm:text-5xl opacity-75">
          hourglass_empty
        </mat-icon>
      </div>
    </mat-card-content>
  </mat-card>

  <!-- Card 4 -->
  <mat-card class="bg-gradient-to-br from-orange-500 to-orange-600 text-white">
    <mat-card-content>
      <div class="flex items-center justify-between">
        <div>
          <p class="text-sm opacity-90 mb-1">Rating</p>
          <p class="text-2xl sm:text-3xl font-bold">4.8</p>
          <p class="text-xs opacity-75 mt-1">From 120 reviews</p>
        </div>
        <mat-icon class="text-4xl sm:text-5xl opacity-75">
          star
        </mat-icon>
      </div>
    </mat-card-content>
  </mat-card>
</div>
```

---

## Touch Interactions

### Touch-Friendly Buttons
```scss
// Minimum touch target size: 44x44px (Apple) or 48x48px (Google)
.touch-button {
  min-width: 48px;
  min-height: 48px;
  padding: 12px 24px;
  
  // Increase tap area without changing visual size
  position: relative;
  &::after {
    content: '';
    position: absolute;
    top: -8px;
    right: -8px;
    bottom: -8px;
    left: -8px;
  }
}
```

### Swipe Actions
```typescript
import { Component } from '@angular/core';

@Component({
  selector: 'app-swipeable-card',
  template: `
    <div class="swipe-container"
         (touchstart)="onTouchStart($event)"
         (touchmove)="onTouchMove($event)"
         (touchend)="onTouchEnd()"
         [style.transform]="'translateX(' + translateX + 'px)'">
      <!-- Card content -->
      <mat-card>
        <mat-card-content>
          Swipe me!
        </mat-card-content>
      </mat-card>
      
      <!-- Swipe actions (revealed on swipe) -->
      <div class="swipe-actions">
        <button mat-icon-button color="primary">
          <mat-icon>check</mat-icon>
        </button>
        <button mat-icon-button color="warn">
          <mat-icon>delete</mat-icon>
        </button>
      </div>
    </div>
  `,
  styles: [`
    .swipe-container {
      position: relative;
      transition: transform 0.3s ease;
    }
    
    .swipe-actions {
      position: absolute;
      right: -100px;
      top: 0;
      height: 100%;
      display: flex;
      gap: 8px;
      align-items: center;
    }
  `]
})
export class SwipeableCardComponent {
  private startX = 0;
  translateX = 0;

  onTouchStart(event: TouchEvent): void {
    this.startX = event.touches[0].clientX;
  }

  onTouchMove(event: TouchEvent): void {
    const currentX = event.touches[0].clientX;
    const diff = currentX - this.startX;
    
    // Only allow left swipe
    if (diff < 0) {
      this.translateX = Math.max(diff, -100);
    }
  }

  onTouchEnd(): void {
    if (this.translateX < -50) {
      // Snap to open
      this.translateX = -100;
    } else {
      // Snap to closed
      this.translateX = 0;
    }
  }
}
```

---

## Performance Optimization

### Image Optimization
```html
<!-- Responsive images -->
<img 
  [src]="imageSrc"
  [srcset]="imageSrcSet"
  sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
  alt="Description"
  loading="lazy"
  class="w-full h-auto">

<!-- Using picture element -->
<picture>
  <source 
    media="(min-width: 1024px)" 
    srcset="image-large.jpg">
  <source 
    media="(min-width: 640px)" 
    srcset="image-medium.jpg">
  <img 
    src="image-small.jpg" 
    alt="Description"
    loading="lazy">
</picture>
```

### Virtual Scrolling for Long Lists
```html
<!-- Mobile optimized with virtual scroll -->
<cdk-virtual-scroll-viewport itemSize="120" class="h-screen">
  <div *cdkVirtualFor="let item of items" class="p-4 border-b">
    <mat-card>
      <!-- Card content -->
    </mat-card>
  </div>
</cdk-virtual-scroll-viewport>
```

### Lazy Loading Components
```typescript
// Lazy load modules for better initial load time
const routes: Routes = [
  {
    path: 'dashboard',
    loadChildren: () => import('./dashboard/dashboard.module')
      .then(m => m.DashboardModule)
  }
];
```

---

## Testing Responsive Design

### Device Testing Matrix
- **Mobile**: iPhone SE (375px), iPhone 12 (390px), Samsung Galaxy (360px)
- **Tablet**: iPad (768px), iPad Pro (1024px)
- **Desktop**: 1366px, 1920px, 2560px

### Chrome DevTools Responsive Testing
```typescript
// Component test
describe('SearchProvidersComponent Responsive', () => {
  it('should show grid on desktop', () => {
    // Set viewport to desktop
    cy.viewport(1920, 1080);
    cy.get('.provider-grid').should('be.visible');
    cy.get('.provider-list').should('not.exist');
  });

  it('should show list on mobile', () => {
    // Set viewport to mobile
    cy.viewport(375, 667);
    cy.get('.provider-list').should('be.visible');
    cy.get('.provider-grid').should('not.exist');
  });
});
```

### Accessibility Testing
```html
<!-- Ensure touch targets are accessible -->
<button 
  mat-button
  class="min-h-[48px] min-w-[48px]"
  [attr.aria-label]="'Delete item ' + item.name">
  <mat-icon>delete</mat-icon>
</button>

<!-- Ensure text is readable -->
<p class="text-base leading-relaxed">
  Text content with good line height
</p>
```

---

## CSS Utilities

### Custom Responsive Utilities
```scss
// Custom responsive mixins
@mixin respond-to($breakpoint) {
  @if $breakpoint == mobile {
    @media (max-width: 639px) { @content; }
  }
  @else if $breakpoint == tablet {
    @media (min-width: 640px) and (max-width: 1023px) { @content; }
  }
  @else if $breakpoint == desktop {
    @media (min-width: 1024px) { @content; }
  }
}

// Usage
.my-component {
  padding: 1rem;
  
  @include respond-to(mobile) {
    padding: 0.5rem;
  }
  
  @include respond-to(desktop) {
    padding: 2rem;
  }
}
```

### Container Queries (Modern Browsers)
```scss
// Component-based responsive design
.card-container {
  container-type: inline-size;
  
  .card {
    display: flex;
    flex-direction: column;
    
    @container (min-width: 400px) {
      flex-direction: row;
    }
  }
}
```

---

## Best Practices Checklist

### ✅ Mobile-First
- [ ] Start with mobile layout
- [ ] Use min-width media queries
- [ ] Test on real devices

### ✅ Touch-Friendly
- [ ] Minimum 48x48px touch targets
- [ ] Adequate spacing between interactive elements
- [ ] Clear hover/active states

### ✅ Performance
- [ ] Lazy load images
- [ ] Use responsive images
- [ ] Minimize layout shifts
- [ ] Optimize fonts

### ✅ Accessibility
- [ ] Keyboard navigation works
- [ ] Screen reader friendly
- [ ] Color contrast ratios
- [ ] Focus indicators visible

### ✅ Testing
- [ ] Test on multiple devices
- [ ] Test in different orientations
- [ ] Test with slow connections
- [ ] Test with assistive technologies

---

**Complete Responsive Design Guide! Ready for mobile-first development! 📱**
