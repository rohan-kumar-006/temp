import { HttpErrorResponse, HttpInterceptorFn, httpResource, HttpResponse } from '@angular/common/http';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth';
import { Router } from '@angular/router';
import { inject } from '@angular/core';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  const authService = inject(AuthService);
  const router = inject(Router);

  const accessToken = authService.getToken();

  if (req.url.includes('/auth/refresh') ||
    req.url.includes('/auth/logout')) {
    return next(
      req.clone({
        withCredentials: true
      })
    )
  }

  if (!accessToken) {
    return next(req);
  }

  const cloneRequest = req.clone({
    setHeaders: {
      Authorization: `Bearer ${accessToken}`
    }
  })

  return next(cloneRequest).pipe(

    catchError((error: HttpErrorResponse) => {
      if (error.status != 401) {
        return throwError(() => error);
      }

      return authService.refreshToken().pipe(
        switchMap(response => {
          const newToken = response.data.accessToken
          authService.saveToken(newToken);

          const retryRequest = req.clone({
            setHeaders: {
              Authorization: `Bearer ${newToken}`
            }
          });
          return next(retryRequest)
        }),
        catchError((refreshError) => {
          authService.clearSession();
          router.navigate(['/login']);

          return throwError(() => refreshError);
        })
      )
    })
  )
};

// ye working interceptor hai , old bala
// import { HttpErrorResponse, HttpInterceptorFn, httpResource, HttpResponse } from '@angular/common/http';
// import { catchError, throwError } from 'rxjs';
// import { AuthService } from '../services/auth';
// import { Router } from '@angular/router';
// import { inject } from '@angular/core';

// export const authInterceptor: HttpInterceptorFn = (req, next) => {

//   const authService = inject(AuthService);
//   const router = inject(Router);

//   const accessToken = localStorage.getItem("accessToken");
//   if (!accessToken) {
//     return next(req);
//   }

//   const cloneRequest = req.clone({
//     setHeaders: {
//       Authorization: `Bearer ${accessToken}`
//     }  
//   })

//   return next(cloneRequest)
//     .pipe(
//       catchError((error: HttpErrorResponse) => {
//         if (error.status == 401) {
//           authService.logout();
//           router.navigate(['/login'])
//         }
//         return throwError(()=>error)
//       })
//     )
// };
