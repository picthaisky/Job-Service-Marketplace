import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/auth/login',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () => import('./features/auth/components/login/login').then(m => m.LoginComponent)
      },
      {
        path: 'register',
        loadComponent: () => import('./features/auth/components/register/register').then(m => m.RegisterComponent)
      }
    ]
  },
  {
    path: 'provider',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Provider'] },
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/provider/components/dashboard/dashboard').then(m => m.DashboardComponent)
      },
      {
        path: 'profile',
        loadComponent: () => import('./features/provider/components/profile/profile').then(m => m.ProfileComponent)
      },
      {
        path: 'income-summary',
        loadComponent: () => import('./features/provider/components/income-summary/income-summary').then(m => m.IncomeSummaryComponent)
      }
    ]
  },
  {
    path: 'client',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Client'] },
    children: [
      {
        path: 'marketplace',
        loadComponent: () => import('./features/client/components/marketplace/marketplace').then(m => m.MarketplaceComponent)
      },
      {
        path: 'booking',
        loadComponent: () => import('./features/client/components/booking/booking').then(m => m.BookingComponent)
      },
      {
        path: 'provider/:id',
        loadComponent: () => import('./features/client/components/provider-detail/provider-detail').then(m => m.ProviderDetailComponent)
      }
    ]
  },
  {
    path: 'admin',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin'] },
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/admin/components/dashboard/dashboard').then(m => m.AdminDashboardComponent)
      },
      {
        path: 'users',
        loadComponent: () => import('./features/admin/components/users/users').then(m => m.UsersComponent)
      },
      {
        path: 'analytics',
        loadComponent: () => import('./features/admin/components/analytics/analytics').then(m => m.AnalyticsComponent)
      }
    ]
  },
  {
    path: '**',
    redirectTo: '/auth/login'
  }
];
