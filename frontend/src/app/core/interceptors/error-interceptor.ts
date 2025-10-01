import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An error occurred';

      if (error.error instanceof ErrorEvent) {
        // Client-side error
        errorMessage = `Error: ${error.error.message}`;
      } else {
        // Server-side error
        errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
        
        if (error.status === 401) {
          // Unauthorized - redirect to login
          router.navigate(['/auth/login']);
        } else if (error.status === 403) {
          // Forbidden
          errorMessage = 'You do not have permission to access this resource';
        } else if (error.status === 404) {
          errorMessage = 'Resource not found';
        } else if (error.status === 500) {
          errorMessage = 'Internal server error';
        }
      }

      console.error(errorMessage);
      return throwError(() => new Error(errorMessage));
    })
  );
};
