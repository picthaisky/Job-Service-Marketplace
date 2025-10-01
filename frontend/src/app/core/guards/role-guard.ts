import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated) {
    router.navigate(['/auth/login']);
    return false;
  }

  const allowedRoles = route.data['roles'] as string[];
  const userRole = authService.userRole;

  if (allowedRoles && userRole && allowedRoles.includes(userRole)) {
    return true;
  }

  // Redirect to unauthorized page or home
  router.navigate(['/']);
  return false;
};
