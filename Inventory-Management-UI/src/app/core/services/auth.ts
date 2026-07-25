import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import {LoginRequest} from "../models/login-request.model"
import { LoginResponse } from '../models/login-response.model';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/app-response.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl=environment.apiUrl;
  constructor(private http:HttpClient){}

    login(request : LoginRequest):
    Observable<ApiResponse<LoginResponse>>
    {
      return this.http.post<ApiResponse<LoginResponse>>(
          `${this.apiUrl}/auth/login`,
          request
      );
    }
    saveToken(token:string){
      localStorage.setItem("token",token);
    }
    getToken(){
      return localStorage.getItem("token");
    }
    isLoggedIn():boolean{
      return !!localStorage.getItem("token")
    }
    logout(){
      localStorage.removeItem("token");
    }
}
