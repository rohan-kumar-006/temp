import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { ApiResponse } from "../models/app-response.model";
import { AdminDashboard } from "../models/admin-dashboard.model";
import { StaffDashboard } from "../models/staff-dashboard.model";

@Injectable({
    providedIn: 'root'
})
export class DashboardService {
    private apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    getAdminDashboard(): Observable<ApiResponse<AdminDashboard>> {
        return this.http.get<ApiResponse<AdminDashboard>>(
            `${this.apiUrl}/dashboard/admin`
        )
    }
    //backend will get the user from the Claim,, nhi to frontend dusra userid bhej skta..
    getStaffDashboard(): Observable<ApiResponse<StaffDashboard>> {
        return this.http.get<ApiResponse<StaffDashboard>>(
            `${this.apiUrl}/dashboard/staff`
        );
    }
}