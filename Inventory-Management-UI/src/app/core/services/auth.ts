import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { LoginRequest } from "../models/login-request.model"
import { LoginResponse } from '../models/login-response.model';
import { finalize, Observable, shareReplay } from 'rxjs';
import { ApiResponse } from '../models/app-response.model';
import { jwtDecode } from 'jwt-decode';
import { UserRole } from '../models/enums/user-role.model';

interface JwtPayload {
  exp?: number;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }
  private refreshRequest$?: Observable<ApiResponse<LoginResponse>>;

  login(request: LoginRequest):
    Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(
      `${this.apiUrl}/auth/login`,
      request,
      { withCredentials: true }
    );
  }
  saveToken(accessToken: string) {
    localStorage.setItem("accessToken", accessToken);
  }
  getToken() {
    return localStorage.getItem("accessToken");
  }
  getRole(): UserRole | null {
    const accessToken = this.getToken();

    if (!accessToken) {
      return null;
    }

    try {
      const decoded = jwtDecode<JwtPayload>(accessToken);
      const role =
        decoded[
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        ];
      return role as UserRole ?? null;
    } catch {
      return null;
    }
  }

  isAdmin(): boolean {
    return this.getRole() === UserRole.Admin;
  }

  isLoggedIn(): boolean {
    // const accessToken = localStorage.getItem("accessToken");
    // // console.log("token", token)
    // if (!accessToken) {
    //   return false;
    // }
    // try {

    //   const decode = jwtDecode<JwtPayload>(accessToken);
    //   if (!decode.exp) {
    //     return false;
    //   }
    //   const currTime = Math.floor(Date.now() / 1000);

    //   if (decode.exp <= currTime) {
    //     this.clearSession();
    //     return false;
    //   }
    //   return true;
    // } catch {
    //   this.clearSession();
    //   return false;
    // }
    // return !!localStorage.getItem("token")
    return !!this.getToken();
  }

  // for refresh tkn

  refreshToken(): Observable<ApiResponse<LoginResponse>> {

    if (!this.refreshRequest$) {
      this.refreshRequest$ = this.http.post<ApiResponse<LoginResponse>>(
        `${this.apiUrl}/auth/refresh`,
        {},
        {
          withCredentials: true
        }
      ).pipe(
        shareReplay(1),
        finalize(() => {
          this.refreshRequest$ = undefined;
        })
      )
    }
    return this.refreshRequest$;
  }
  logout(): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(
      `${this.apiUrl}/auth/logout`,
      {},
      { withCredentials: true }
    );
  }
  clearSession() {
    localStorage.removeItem("accessToken");
  }
}
