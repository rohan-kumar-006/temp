import { Component, inject, OnInit, signal } from '@angular/core';
import { AuthService } from '../../../core/services/auth';
import { Router } from '@angular/router';
import { DashboardService } from '../../../core/services/dashboard';
import { ToastService } from '../../../core/services/toast';
import { AdminDashboard } from '../../../core/models/admin-dashboard.model';
import { StaffDashboard } from '../../../core/models/staff-dashboard.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {


}