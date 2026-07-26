import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/app-response.model';
import { User } from '../models/user.model';
import { CreateUser } from '../models/create-user.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl=environment.apiUrl;
  
  constructor(private http:HttpClient){}

  getAllStaff() : Observable<ApiResponse<User[]>>{
    return this.http.get<ApiResponse<User[]>>(
        `${this.apiUrl}/users`
    )
  }
  createStaff(request:CreateUser):Observable<ApiResponse<User>>{
    return this.http.post<ApiResponse<User>>(
      `${this.apiUrl}/users`,
      request
    )
  }
}
