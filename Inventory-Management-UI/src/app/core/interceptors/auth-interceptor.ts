import { HttpErrorResponse, HttpInterceptorFn, httpResource, HttpResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth';
import { Router } from '@angular/router';
import { inject } from '@angular/core';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  const authService = inject(AuthService);
  const router = inject(Router);

  const token = localStorage.getItem("token");
  if (!token) {
    return next(req);
  }

  const cloneRequest = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  })

  return next(cloneRequest)
    .pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status == 401) {
          authService.logout();
          router.navigate(['/login'])
        }
        return throwError(()=>error)
      })
    )
};
