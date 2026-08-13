import { Component, inject, OnInit, signal } from '@angular/core';
import { AuthService } from '../../../core/services/auth';
import { Router } from '@angular/router';
import { DashboardService } from '../../../core/services/dashboard';
import { ToastService } from '../../../core/services/toast';
import { AdminDashboard } from '../../../core/models/admin-dashboard.model';
import { StaffDashboard } from '../../../core/models/staff-dashboard.model';
import { CommonModule } from '@angular/common';
import { UserRole } from '../../../core/models/enums/user-role.model';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  private dashboardService = inject(DashboardService);
  private authService = inject(AuthService)
  private toast = inject(ToastService)

  adminDashboard = signal<AdminDashboard | null>(null);
  staffDashboard = signal<StaffDashboard | null>(null);

  loading = signal(false)
  isAdmin = signal(false)

  ngOnInit(): void {
    const role = this.authService.getRole()
    this.isAdmin.set(role == UserRole.Admin);
    if (this.isAdmin()) {
      this.loadAdminDashboard()
    } else {
      this.loadStaffDashboard()
    } 
  }

  loadAdminDashboard() {
    this.loading.set(true);

    this.dashboardService.getAdminDashboard()
      .subscribe({
        next: (response) => {
          this.adminDashboard.set(response.data)
          this.loading.set(false);
        },
        error: (err) => {
          this.toast.error(
            err.error?.message ??
            'Unable to load dashboard.'
          );
          this.loading.set(false);
        }
      })
  }

  loadStaffDashboard() {
    this.loading.set(true);

    this.dashboardService
      .getStaffDashboard()
      .subscribe({
        
        next: response => {
          console.log(response.data)
          this.staffDashboard.set(response.data);
          this.loading.set(false);
        },
        error: err => {
          this.toast.error(
            err.error?.message ??
            'Unable to load dashboard.'
          );
          this.loading.set(false);
        }
      });
  }
}