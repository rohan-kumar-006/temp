import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/app-response.model';
import { User } from '../models/user.model';
import { CreateUser } from '../models/create-user.model';
import { UpdateUser } from '../models/update-user.model';
import { PagedResult } from '../models/paged-result-model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getAllStaff(page: number = 1, pageSize: number = 10, search: string = "") {
    let params = new HttpParams()
      .set("page", page)
      .set("pageSize", pageSize);

    if (search.trim()) {
      params = params.set("search", search.trim());
    }


    return this.http.get<ApiResponse<PagedResult<User>>>(
      `${this.apiUrl}/users`,
      { params }
    );
  }

  createStaff(request: CreateUser): Observable<ApiResponse<User>> {
    return this.http.post<ApiResponse<User>>(
      `${this.apiUrl}/users`,
      request
    )
  }

  updateStaff(id: number, request: UpdateUser): Observable<ApiResponse<User>> {
    return this.http.put<ApiResponse<User>>(
      `${this.apiUrl}/users/${id}`,
      request);
  }

  toggleStatus(id: number): Observable<ApiResponse<User>> {
    // console.log("service hit")
    return this.http.patch<ApiResponse<User>>(
      `${this.apiUrl}/users/${id}/status`, {}
    )
  }
}
