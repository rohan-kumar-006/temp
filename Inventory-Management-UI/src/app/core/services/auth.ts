import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { LoginRequest } from "../models/login-request.model"
import { LoginResponse } from '../models/login-response.model';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/app-response.model';
import { jwtDecode } from 'jwt-decode';
import { UserRole } from '../models/enums/user-role.model';

interface JwtPayload {
  exp?: number;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }


  login(request: LoginRequest):
    Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(
      `${this.apiUrl}/auth/login`,
      request
    );
  }
  saveToken(token: string) {
    localStorage.setItem("token", token);
  }
  getToken() {
    return localStorage.getItem("token");
  }
  saveRole(role: UserRole) {
    localStorage.setItem("role", role);
  }

  getRole(): UserRole | null {
    return localStorage.getItem("role")  as UserRole | null;
  }
  isAdmin(): boolean {
    return this.getRole() === UserRole.Admin;
  }

  isLoggedIn(): boolean {
    const token = localStorage.getItem("token");
    // console.log("token", token)
    if (!token) {
      return false;
    }
    try {

      const decode = jwtDecode<JwtPayload>(token);
      if (!decode.exp) {
        return false;
      }
      const currTime = Math.floor(Date.now() / 1000);

      if (decode.exp <= currTime) {
        this.logout();
        return false;
      }
      return true;
    } catch {
      this.logout();
      return false;
    }

    // return !!localStorage.getItem("token")
  }

  logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
  }
}
